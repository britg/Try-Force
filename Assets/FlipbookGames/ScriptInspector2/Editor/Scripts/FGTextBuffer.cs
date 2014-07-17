/* SCRIPT INSPECTOR 2
 * version 2.1.9, May 2014
 * Copyright © 2012-2014, Flipbook Games
 * 
 * Unity's legendary custom inspector for C#, UnityScript and Boo scripts,
 * now transformed into a powerful Script, Shader, and Text Editor!!!
 * 
 * Follow me on http://twitter.com/FlipbookGames
 * Like Flipbook Games on Facebook http://facebook.com/FlipbookGames
 * Join Unity forum discusion http://forum.unity3d.com/threads/138329
 * Contact info@flipbookgames.com for feedback, bug reports, or suggestions.
 * Visit http://flipbookgames.com/ for more info.
 */

using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Reflection;

[Serializable, StructLayout(LayoutKind.Sequential)]
public class FGTextBuffer : ScriptableObject
{
	//[Serializable, StructLayout(LayoutKind.Sequential)]
	public class TextBlock
	{
		public GUIStyle style;
		public string text;

		public TextBlock(string t, GUIStyle s)
		{
			style = s;
			text = t;
		}
	}

	[Flags]
	public enum BlockState
	{
		None = 0,
		CommentBlock = 1,
		StringBlock = 2,
	}

	[Serializable, StructLayout(LayoutKind.Sequential)]
	public class FormatedLine
	{
		[NonSerialized]// SerializeField, HideInInspector]
		public TextBlock[] textBlocks = null;//new TextBlock[0];
		[SerializeField, HideInInspector]
		public BlockState blockState = 0;
		[SerializeField, HideInInspector]
		public int lastChange = -1;
		[SerializeField, HideInInspector]
		public int savedVersion = -1;
	}
	[SerializeField, HideInInspector]
	public FormatedLine[] formatedLines = new FormatedLine[0];
	[SerializeField, HideInInspector]
	public List<string> lines = new List<string>();
	[SerializeField, HideInInspector]
	private string lineEnding = "\n";
	[SerializeField, HideInInspector]
	//[NonSerialized]
	public string[] hyperlinks = null;
	[NonSerialized]
	private StreamReader streamReader;
	[SerializeField, HideInInspector]
	public int codePage = Encoding.UTF8.CodePage;
	public Encoding fileEncoding
	{
		get
		{
			return isShader || (isText && codePage == Encoding.UTF8.CodePage) ? new UTF8Encoding(false) : Encoding.GetEncoding(codePage);
		}
	}
	[NonSerialized]
	public int numParsedLines = 0;
	[SerializeField, HideInInspector]
	public int longestLine = 0;

	[SerializeField, HideInInspector]
	public bool isJsFile = false;
	[SerializeField, HideInInspector]
	public bool isCsFile = false;
	[SerializeField, HideInInspector]
	public bool isBooFile = false;
	[SerializeField, HideInInspector]
	public bool isText = false;
	[SerializeField, HideInInspector]
	public bool isShader = false;
	
	[SerializeField, HideInInspector]
	public FGTextEditor.Styles styles = null;

	[SerializeField, HideInInspector]
	public string guid = "";
	[SerializeField, HideInInspector]
	public bool justSavedNow = false;
	[SerializeField, HideInInspector]
	public bool needsReload = false;

	[NonSerialized]
	private List<FGTextEditor> editors = new List<FGTextEditor>();

	public void AddEditor(FGTextEditor editor)
	{
		editors.Add(editor);
		if (!IsLoading && lines.Count > 0)
			editor.ValidateCarets();
	}

	public void RemoveEditor(FGTextEditor editor)
	{
		editors.Remove(editor);
		if (IsModified && editors.Count == 0)
		{
			//ScriptableObject.DestroyImmediate(this);
			EditorApplication.update -= CheckSaveOnUpdate;
			EditorApplication.update += CheckSaveOnUpdate;
		}
	}

	public void CheckSaveOnUpdate()
	{
		EditorApplication.update -= CheckSaveOnUpdate;

		if (IsModified && editors.Count == 0 && !IsAnyWindowMaximized())
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);

			switch (EditorUtility.DisplayDialogComplex(
				"Script Inspector 2",
				"Save changes to the following asset?\n\n" + path,
				"Save",
				"Discard Changes",
				"Keep in Memory"))
			{
				case 0:
					Save();
					AssetDatabase.ImportAsset(path, ImportAssetOptions.Default);
					//EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)));
					break;
				case 1:
					FGTextBufferManager.DestroyBuffer(this);
					break;
				case 2:
					break;
			}
		}
	}

	static FGTextBuffer()
	{
		Assembly thisAssembly = typeof(FGTextBuffer).Assembly;

		HashSet<string> typeNames = new HashSet<string>();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			if (assembly is System.Reflection.Emit.AssemblyBuilder)
				continue;

			Type[] assemblyTypes = assembly == thisAssembly ? assembly.GetTypes() : assembly.GetExportedTypes();
			foreach (Type type in assemblyTypes)
			{
				string name = type.Name;
				int index = name.IndexOf('`');
				if (index >= 0)
					name = name.Remove(index);
				typeNames.Add(name);
				if (type.IsSubclassOf(typeof(Attribute)) && type.Name.EndsWith("Attribute", StringComparison.OrdinalIgnoreCase))
					typeNames.Add(type.Name.Substring(0, type.Name.Length - "Attribute".Length));
			}
		}
		unityTypes = typeNames.ToArray();

		Array.Sort<string>(unityTypes);
		Array.Sort<string>(jsKeywords);
		Array.Sort<string>(booKeywords);
	}

	public static FGTextBuffer GetBuffer(UnityEngine.Object target)
	{
		return FGTextBufferManager.GetBuffer(target);
	}

	public void OnEnable()
	{
		hideFlags = HideFlags.HideAndDontSave;
		if (needsReload)
		{
			//string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			//Debug.LogError("needsReload == true !!! " + Path.GetFileName(assetPath));
			Reload();
		}
	}

	public void OnDisable()
	{
		guidsToLoadFirst.Remove(guid);
	}

	public void OnDestroy()
	{
		guidsToLoadFirst.Remove(guid);
	}

	[Serializable, StructLayout(LayoutKind.Sequential)]
	public class CaretPos : System.Object, IComparable<CaretPos>, IEquatable<CaretPos>
	{
		[SerializeField]
		public int virtualColumn;
		[SerializeField]
		public int column;
		[SerializeField]
		public int characterIndex;
		[SerializeField]
		public int line;

		public CaretPos Clone()
		{
			return new CaretPos { virtualColumn = virtualColumn, column = column, characterIndex = characterIndex, line = line };
		}

		public void Set(int line, int characterIndex, int column)
		{
			this.column = column;
			this.characterIndex = characterIndex;
			this.line = line;
		}

		public void Set(int line, int characterIndex, int column, int virtualColumn)
		{
			this.virtualColumn = virtualColumn;
			this.column = column;
			this.characterIndex = characterIndex;
			this.line = line;
		}

		public void Set(CaretPos other)
		{
			virtualColumn = other.virtualColumn;
			column = other.column;
			characterIndex = other.characterIndex;
			line = other.line;
		}

		public bool IsSameAs(CaretPos other)
		{
			return Equals(other) && column == other.column && virtualColumn == other.virtualColumn;
		}

		public int CompareTo(CaretPos other)
		{
			return line == other.line ? characterIndex - other.characterIndex : line - other.line;
		}
		public static bool operator <  (CaretPos A, CaretPos B) { return A.CompareTo(B) < 0; }
		public static bool operator >  (CaretPos A, CaretPos B) { return A.CompareTo(B) > 0; }
		public static bool operator <= (CaretPos A, CaretPos B) { return A.CompareTo(B) <= 0; }
		public static bool operator >= (CaretPos A, CaretPos B) { return A.CompareTo(B) >= 0; }


		public static bool operator == (CaretPos A, CaretPos B)
		{
			if (object.ReferenceEquals(A, B))
				return true;
			if (object.ReferenceEquals(A, null))
				return false;
			if (object.ReferenceEquals(B, null))
				return false;
			return A.Equals(B);
		}
		public static bool operator != (CaretPos A, CaretPos B) { return !(A == B); }

		public bool Equals(CaretPos other)
		{
			return line == other.line && characterIndex == other.characterIndex;
		}

		public override bool Equals(System.Object obj)
		{
			if (obj == null) return base.Equals(obj);

			if (!(obj is CaretPos))
				throw new InvalidCastException("The 'obj' argument is not a CaretPos object.");
			else
				return this == (CaretPos) obj;
		}

		public override int GetHashCode()
		{
			return line.GetHashCode() ^ characterIndex.GetHashCode();
		}
	}

	[SerializeField, HideInInspector]
	private bool initialized = false;

	public void Initialize()
	{
		if (initialized && numParsedLines > 0)
			return;

		if (lines == null || lines.Count == 0)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			if (string.IsNullOrEmpty(assetPath))
				return;

			isJsFile = assetPath.EndsWith(".js", StringComparison.OrdinalIgnoreCase);
			isCsFile = assetPath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase);
			isBooFile = assetPath.EndsWith(".boo", StringComparison.OrdinalIgnoreCase);
			isShader = assetPath.EndsWith(".shader", StringComparison.OrdinalIgnoreCase) ||
				assetPath.EndsWith(".cg", StringComparison.OrdinalIgnoreCase) ||
				assetPath.EndsWith(".cginc", StringComparison.OrdinalIgnoreCase);
			isText = !(isJsFile || isCsFile || isBooFile || isShader);
			
			styles = isText ? FGTextEditor.stylesText : FGTextEditor.stylesCode;

			lines = new List<string>();
			lineEnding = "\n";
			try
			{
				Stream stream = new BufferedStream(new FileStream(assetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), 1024);
				if (stream != null)
					streamReader = new StreamReader(stream, true);
				codePage = Encoding.UTF8.CodePage;
			}
			catch (Exception error)
			{
				Debug.LogError("Could not read the content of '" + assetPath + "' because of the following error:");
				Debug.LogError(error);
				if (streamReader != null)
				{
					streamReader.Close();
					streamReader.Dispose();
					streamReader = null;
				}
			}

			formatedLines = new FormatedLine[0];
			hyperlinks = new string[0];
			longestLine = 0;
			numParsedLines = 0;
			savedAtUndoPosition = undoPosition;

			EditorApplication.update -= ProgressiveLoadOnUpdate;
			EditorApplication.update += ProgressiveLoadOnUpdate;
		}
		else if (numParsedLines == 0)
		{
			EditorApplication.update -= ProgressiveLoadOnUpdate;
			EditorApplication.update += ProgressiveLoadOnUpdate;
		}
		else
		{
			initialized = true;
		}
	}

	public void Reload()
	{
		needsReload = !justSavedNow;
		EditorApplication.update -= ReloadOnUpdate;
		EditorApplication.update += ReloadOnUpdate;
	}

	private void ReloadOnUpdate()
	{
		EditorApplication.update -= ReloadOnUpdate;

		if (justSavedNow)
		{
			justSavedNow = false;
			RescanHyperlinks();

			UpdateViews();
		}
		else
		{
			FGCodeWindow.CheckAssetRename(guid);

			if (IsModified)
			{
				if (!EditorUtility.DisplayDialog(
					"Script Inspector 2",
					AssetDatabase.GUIDToAssetPath(guid)
						+ "\n\nThis asset has been modified outside of Unity Editor.\nDo you want to reload it and lose the changes made in Script Inspector?",
					"Reload",
					"Keep changes"))
				{
					needsReload = false;

					savedAtUndoPosition = 0;
					UpdateViews();
					return;
				}
			}

			formatedLines = new FormatedLine[0];
			lines = new List<string>();
			lineEnding = "\n";
			hyperlinks = null;
			if (streamReader != null)
			{
				streamReader.Close();
				streamReader.Dispose();
				streamReader = null;
			}
			codePage = Encoding.UTF8.CodePage;
			numParsedLines = 0;
			longestLine = 0;

			isJsFile = false;
			isCsFile = false;
			isBooFile = false;
			isShader = false;
			isText = false;

			undoBuffer = new List<UndoRecord>();
			undoPosition = 0;
			currentChangeId = 0;
			savedAtUndoPosition = 0;
			//recordUndo = true;
			//beginEditDepth = 0;

			initialized = false;
			Initialize();
		}
	}

	public void RescanHyperlinks()
	{
		hyperlinks = new string[0];
		foreach (var line in formatedLines)
		{
			if (line.textBlocks == null)
				continue;
			
			foreach (var block in line.textBlocks)
			{
				if (block.style == styles.mailtoStyle || block.style == styles.hyperlinkStyle)
				{
					int index = Array.BinarySearch<string>(hyperlinks, block.text, StringComparer.OrdinalIgnoreCase);
					if (index < 0)
						ArrayUtility.Insert(ref hyperlinks, -1 - index, block.text);
				}
			}
		}
	}

	public void Save()
	{
		justSavedNow = true;

		StreamWriter writer = null;
		try
		{
			writer = new StreamWriter(AssetDatabase.GUIDToAssetPath(guid), false, fileEncoding);
			writer.NewLine = lineEnding;
			int numLines = lines.Count;
			for (int i = 0; i < numLines - 1; ++i)
				writer.WriteLine(lines[i]);
			writer.Write(lines[numLines - 1]);

			for (int i = 0; i < numParsedLines; ++i)
				formatedLines[i].savedVersion = formatedLines[i].lastChange;

			savedAtUndoPosition = undoPosition;

			foreach (UndoRecord record in undoBuffer)
				foreach (UndoRecord.TextChange change in record.changes)
					change.savedVersions = null;
		}
		catch
		{
			if (writer != null)
			{
				writer.Close();
				writer.Dispose();
			}
			EditorUtility.DisplayDialog("Error Saving Script", "The script '" + AssetDatabase.GUIDToAssetPath(guid) + "' could not be saved!", "OK");
		}

		if (writer != null)
		{
			writer.Close();
			writer.Dispose();
		}
	}
	
	private static bool IsAnyWindowMaximized()
	{
		System.Type maximizedType = typeof(EditorWindow).Assembly.GetType("UnityEditor.MaximizedHostView");
		return Resources.FindObjectsOfTypeAll(maximizedType).Length != 0;
	}

	public delegate void ChangeDelegate();
	public ChangeDelegate onChange;

	public void UpdateViews()
	{
		if (onChange != null)
			onChange();
	}

	private static List<string> guidsToLoadFirst = new List<string>();

	public void LoadFaster()
	{
		//if (!guidsToLoadFirst.Contains(guid))
		//	guidsToLoadFirst.Add(guid);
	}

	public void ProgressiveLoadOnUpdate()
	{
		initialized = true;

		if (guidsToLoadFirst.Count > 0 && !guidsToLoadFirst.Contains(guid))
			return;
		
		if (streamReader != null)
		{
			try
			{
				Parse(numParsedLines + 32);
			}
			catch (Exception error)
			{
				Debug.LogError("Could not read the content of '" + AssetDatabase.GUIDToAssetPath(guid) + "' because of the following error:");
				Debug.LogError(error);
				if (streamReader != null)
				{
					streamReader.Close();
					streamReader.Dispose();
					streamReader = null;
				}
			}

			if (streamReader == null)
			{
				//if (searchString != string.Empty)
				//    SetSearchText(searchString);
				//focusCodeView = false;
				for (int i = formatedLines.Length; i-- > 0; )
					formatedLines[i].lastChange = -1;
				UpdateViews();
			}
		}
		else if (numParsedLines < lines.Count)
		{
			int toLine = Math.Min(numParsedLines + 32, lines.Count - 1);
			ReformatLines(numParsedLines, toLine);
			numParsedLines = toLine + 1;
			UpdateViews();
		}
		else
		{
			EditorApplication.update -= ProgressiveLoadOnUpdate;
			guidsToLoadFirst.Remove(guid);
			ValidateCarets();
		}
	}

	private void ValidateCarets()
	{
		foreach (FGTextEditor editor in editors)
			editor.ValidateCarets();
	}

	public bool CanUndo()
	{
		return undoPosition > 0;
	}

	public bool CanRedo()
	{
		return undoPosition < undoBuffer.Count;
	}

	public void Undo()
	{
		if (!CanUndo())
			return;

		recordUndo = false;

		UndoRecord record = undoBuffer[--undoPosition];
		for (int i = record.changes.Count; i-- != 0; )
		{
			UndoRecord.TextChange change = record.changes[i];

			int changeFromLine = change.from.line;
			int changeToLine = change.to.line;
			if (changeFromLine > changeToLine)
			{
				int temp = changeToLine;
				changeToLine = changeFromLine;
				changeFromLine = temp;
			}

			int[] tempSavedVersions = null;

			CaretPos insertAt = change.from.Clone();
			if (change.newText != string.Empty)
			{
				// Undo inserting text
				string[] textLines = change.newText.Split('\n');
				CaretPos to = change.from.Clone();
				to.characterIndex = textLines.Length > 1 ? textLines[textLines.Length - 1].Length
					: to.characterIndex + change.newText.Length;
				to.line += textLines.Length - 1;
				to.virtualColumn = to.column = CharIndexToColumn(to.characterIndex, to.line);

				int numLinesChanging = 1 + to.line - changeFromLine;

				tempSavedVersions = new int[numLinesChanging];
				for (int j = 0; j < numLinesChanging; ++j)
					tempSavedVersions[j] = formatedLines[changeFromLine + j].savedVersion;

				insertAt = DeleteText(change.from, to);
			}
			if (change.oldText != string.Empty)
			{
				// Undo deleting text
				InsertText(insertAt, change.oldText);
			}
			UpdateHighlighting(changeFromLine, changeToLine);
			for (int j = change.oldLineChanges.Length; j-- > 0; )
			{
				formatedLines[j + changeFromLine].lastChange = change.oldLineChanges[j];
				if (change.savedVersions != null && change.savedVersions.Length == 1 + changeToLine - changeFromLine)
				{
					formatedLines[j + changeFromLine].savedVersion = change.savedVersions[j];
				}
			}

			change.savedVersions = tempSavedVersions;
		}
		activeEditor.caretPosition = record.preCaretPos.Clone();
		if (record.preCaretPos == record.preSelectionPos)
			activeEditor.selectionStartPosition = null;
		else
			activeEditor.selectionStartPosition = record.preSelectionPos.Clone();
		activeEditor.caretMoveTime = Time.realtimeSinceStartup;
		activeEditor.scrollToCaret = true;

		recordUndo = true;
	}

	public void Redo()
	{
		if (!CanRedo())
			return;

		recordUndo = false;

		UndoRecord record = undoBuffer[undoPosition++];

		for (int i = 0; i < record.changes.Count; ++i)
		{
			UndoRecord.TextChange change = record.changes[i];

			int changeFromLine = change.from.line;
			int changeToLine = change.to.line;
			if (changeFromLine > changeToLine)
			{
				int temp = changeToLine;
				changeToLine = changeFromLine;
				changeFromLine = temp;
			}

			int numLinesChanging = 1 + changeToLine - changeFromLine;

			int[] tempSavedVersions = new int[numLinesChanging];
			for (int j = numLinesChanging; j-- > 0; )
				tempSavedVersions[j] = formatedLines[j + changeFromLine].savedVersion;

			CaretPos newPos = change.from.Clone();
			if (change.oldText != string.Empty)
			{
				// Redo deleting text
				newPos = DeleteText(change.from, change.to);
			}
			if (change.newText != string.Empty)
			{
				// Redo inserting text
				newPos = InsertText(newPos, change.newText);
			}
			UpdateHighlighting(changeFromLine, newPos.line);
			for (int j = changeFromLine; j <= newPos.line; ++j)
			{
				formatedLines[j].lastChange = record.changeId;
				if (change.savedVersions != null && change.savedVersions.Length != 0)
				{
					formatedLines[j].savedVersion = change.savedVersions[j - changeFromLine];
				}
			}

			change.savedVersions = tempSavedVersions;
		}
		activeEditor.caretPosition = record.postCaretPos.Clone();
		if (record.postCaretPos == record.postSelectionPos)
			activeEditor.selectionStartPosition = null;
		else
			activeEditor.selectionStartPosition = record.postSelectionPos.Clone();
		activeEditor.caretMoveTime = Time.realtimeSinceStartup;
		activeEditor.scrollToCaret = true;

		recordUndo = true;
	}

	public int CharIndexToColumn(int charIndex, int line)
	{
		if (lines.Count == 0 || line >= lines.Count)
			return 0;
		string s = lines[line];
		if (s.Length < charIndex)
			charIndex = s.Length;

		int col = 0;
		for (int i = 0; i < charIndex; ++i)
			col += s[i] != '\t' ? 1 : 4 - (col & 3);
		return col;
	}

	public int ColumnToCharIndex(ref int column, int line)
	{
		line = Math.Max(0, Math.Min(line, numParsedLines - 1));
		column = Math.Max(0, column);

		if (lines.Count == 0 || line >= lines.Count)
			return 0;
		string s = lines[line];

		int i = 0;
		int col = 0;
		while (i < s.Length && col < column)
		{
			if (s[i] == '\t')
				col = (col & ~3) + 4;
			else
				++col;
			++i;
		}
		if (i == s.Length)
		{
			column = col;
		}
		else if (col > column)
		{
			if ((column & 3) < 2)
			{
				--col;
				--i;
				column &= ~3;
			}
			else
			{
				column = (column & ~3) + 4;
			}
		}
		return i;
	}

	public string GetTextRange(CaretPos from, CaretPos to)
	{
		int fromCharIndex, fromLine, toCharIndex, toLine;
		if (from < to)
		{
			fromCharIndex = from.characterIndex;
			fromLine = from.line;
			toCharIndex = to.characterIndex;
			toLine = to.line;
		}
		else
		{
			fromCharIndex = to.characterIndex;
			fromLine = to.line;
			toCharIndex = from.characterIndex;
			toLine = from.line;
		}

		StringBuilder buffer = new StringBuilder();
		if (fromLine == toLine)
		{
			buffer.Append(lines[fromLine].Substring(fromCharIndex, toCharIndex - fromCharIndex));
		}
		else
		{
			buffer.Append(lines[fromLine].Substring(fromCharIndex) + '\n');
			for (int i = fromLine + 1; i < toLine; ++i)
				buffer.Append(lines[i] + '\n');
			buffer.Append(lines[toLine].Substring(0, toCharIndex));
		}

		return buffer.ToString();
	}

	public static int GetCharClass(char c)
	{
		if (c == ' ' || c == '\t')
			return 0;
		if (c >= '0' && c <= '9')
			return 1;
		if (c == '_' || c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z')
			return 2;
		return 3;
	}

	public bool GetWordExtents(int column, int line, out int wordStart, out int wordEnd)
	{
		wordStart = column;
		wordEnd = column;
		if (line >= formatedLines.Length)
			return false;

		string text = lines[line];
		int length = text.Length;
		wordStart = wordEnd = Math.Min(column, length - 1);
		if (wordStart < 0)
			return false;

		int cc = GetCharClass(text[wordStart]);
		if (wordStart > 0 && cc == 0)
		{
			--wordStart;
			cc = GetCharClass(text[wordStart]);
			if (cc != 0)
				--wordEnd;
		}
		if (cc == 3)
		{
			++wordEnd;
		}
		else if (cc == 0)
		{
			while (wordStart > 0 && GetCharClass(text[wordStart - 1]) == 0)
				--wordStart;
			while (wordEnd < length && GetCharClass(text[wordEnd]) == 0)
				++wordEnd;
		}
		else
		{
			while (wordStart > 0)
			{
				char ch = text[wordStart - 1];
				int c = GetCharClass(ch);
				if (c == 1 || c == 2 || cc == 1 && ch == '.')
					--wordStart;
				else
					break;
				cc = c;
			}
			while (wordEnd < length)
			{
				int c = GetCharClass(text[wordEnd]);
				if (c == 1 || c == 2 || cc == 1 && text[wordEnd] == '.')
					++wordEnd;
				else
					break;
			}
		}
		return true;
	}

	private static int DigitsAsLetters(int characterClass)
	{
		return characterClass == 2 ? 1 : characterClass;
	}

	public CaretPos WordStopLeft(CaretPos from)
	{
		int column = from.characterIndex;
		int line = from.line;

		if (column == 0)
		{
			if (line == 0)
				return new CaretPos { characterIndex = 0, column = 0, line = 0, virtualColumn = 0 };

			--line;
			column = lines[line].Length;
		}

		string s = lines[line];

		if (column > 0)
		{
			int characterClass = DigitsAsLetters(GetCharClass(s[--column]));

			while (column > 0 && characterClass == 0)
				characterClass = DigitsAsLetters(GetCharClass(s[--column]));

			while (column > 0 && DigitsAsLetters(GetCharClass(s[column - 1])) == characterClass)
				--column;
		}

		return new CaretPos { characterIndex = column, column = CharIndexToColumn(column, line), line = line, virtualColumn = column };
	}

	public CaretPos WordStopRight(CaretPos from)
	{
		int column = from.characterIndex;
		int line = from.line;

		if (column >= lines[line].Length)
		{
			if (line == lines.Count - 1)
				return new CaretPos { characterIndex = column, column = CharIndexToColumn(column, line), line = line, virtualColumn = column };

			++line;
			column = 0;
		}

		string s = lines[line];

		if (column < s.Length)
		{
			int characterClass = DigitsAsLetters(GetCharClass(s[column++]));

			if (characterClass != 0)
			{
				while (column < s.Length)
				{
					int nextClass = DigitsAsLetters(GetCharClass(s[column]));
					if (nextClass != characterClass)
					{
						characterClass = nextClass;
						break;
					}
					else
					{
						++column;
					}
				}
			}

			if (characterClass == 0)
				while (column < s.Length && GetCharClass(s[column]) == 0)
					++column;
		}

		return new CaretPos { characterIndex = column, column = CharIndexToColumn(column, line), line = line, virtualColumn = column };
	}

	[Serializable, StructLayout(LayoutKind.Sequential)]
	private class UndoRecord
	{
		[Serializable, StructLayout(LayoutKind.Sequential)]
		public class TextChange
		{
			[SerializeField, HideInInspector]
			public CaretPos from;
			[SerializeField, HideInInspector]
			public CaretPos to;
			[SerializeField, HideInInspector]
			public string oldText;
			[SerializeField, HideInInspector]
			public string newText;
			[SerializeField, HideInInspector]
			public int[] oldLineChanges;
			[SerializeField, HideInInspector]
			public int[] savedVersions;
		}
		[SerializeField, HideInInspector]
		public List<TextChange> changes;
		[SerializeField, HideInInspector]
		public int changeId;
		[SerializeField, HideInInspector]
		public CaretPos preCaretPos;
		[SerializeField, HideInInspector]
		public CaretPos preSelectionPos;
		[SerializeField, HideInInspector]
		public CaretPos postCaretPos;
		[SerializeField, HideInInspector]
		public CaretPos postSelectionPos;
		[SerializeField, HideInInspector]
		public string actionType;
	}
	[SerializeField, HideInInspector]
	private List<UndoRecord> undoBuffer = new List<UndoRecord>();
	[NonSerialized]
	private UndoRecord tempUndoRecord;
	[SerializeField, HideInInspector]
	public int undoPosition = 0;
	[SerializeField, HideInInspector]
	public int savedAtUndoPosition = 0;
	[SerializeField, HideInInspector]
	public int currentChangeId = 0;
	[NonSerialized]
	private bool recordUndo = true;
	[NonSerialized]
	private int beginEditDepth = 0;
	[NonSerialized]
	private List<FormatedLine> updatedLines = new List<FormatedLine>();

	public bool IsModified { get { return undoPosition != savedAtUndoPosition; } }
	public bool IsLoading { get { return streamReader != null || numParsedLines != lines.Count; } }

	public void BeginEdit(string description)
	{
		if (!recordUndo)
			return;

		if (beginEditDepth++ == 0)
		{
			tempUndoRecord = new UndoRecord();
			tempUndoRecord.changes = new List<UndoRecord.TextChange>();
			tempUndoRecord.changeId = currentChangeId + 1;
			//tempUndoRecord.oldText = string.Empty;
			//tempUndoRecord.newText = string.Empty;
			tempUndoRecord.actionType = description;
			tempUndoRecord.preCaretPos = activeEditor.caretPosition.Clone();
			tempUndoRecord.preSelectionPos = activeEditor.selectionStartPosition != null ? activeEditor.selectionStartPosition.Clone() : activeEditor.caretPosition.Clone();

			updatedLines = new List<FormatedLine>();
		}
	}

	private void RegisterUndoText(string actionType, CaretPos from, CaretPos to, string text)
	{
		if (!recordUndo)
			return;

		UndoRecord.TextChange change = new UndoRecord.TextChange();
		if (from < to)
		{
			change.from = from.Clone();
			change.to = to.Clone();
		}
		else
		{
			change.from = to.Clone();
			change.to = from.Clone();
		}
		change.oldText = GetTextRange(from, to);
		change.newText = text;
		change.oldLineChanges = new int[1 + change.to.line - change.from.line];
		change.savedVersions = new int[1 + change.to.line - change.from.line];
		for (int i = change.oldLineChanges.Length; i-- > 0; )
		{
			change.oldLineChanges[i] = formatedLines[i + change.from.line].lastChange;
			change.savedVersions[i] = formatedLines[i + change.from.line].savedVersion;
		}
		tempUndoRecord.changes.Add(change);

		tempUndoRecord.actionType = actionType;
	}

	// TODO: REMOVE THIS!
	public void SetUndoActionType(string actionType)
	{
		if (recordUndo && beginEditDepth == 1)
			tempUndoRecord.actionType = actionType;
	}

	public static FGTextEditor activeEditor = null;

	public void EndEdit()
	{
		if (!recordUndo)
			return;

		if (--beginEditDepth > 0)
			return;

		if (tempUndoRecord.changes.Count == 0)
			return;

		tempUndoRecord.postCaretPos = activeEditor.caretPosition.Clone();
		tempUndoRecord.postSelectionPos = activeEditor.selectionStartPosition != null ? activeEditor.selectionStartPosition.Clone() : activeEditor.caretPosition.Clone();

		bool addNewRecord = true;

		if (undoPosition < undoBuffer.Count)
		{
			undoBuffer.RemoveRange(undoPosition, undoBuffer.Count - undoPosition);
			if (savedAtUndoPosition > undoPosition)
				savedAtUndoPosition = -1;
		}
		else
		{
			// Check is it fine to combine with previous record
			if (undoPosition > 0 && tempUndoRecord.changes.Count == 1)
			{
				UndoRecord last = undoBuffer[undoPosition - 1];
				if (IsModified && last.changes.Count == 1 && last.postCaretPos == tempUndoRecord.preCaretPos && last.postSelectionPos == tempUndoRecord.preSelectionPos)
				{
					UndoRecord.TextChange currChange = tempUndoRecord.changes[0];
					UndoRecord.TextChange prevChange = last.changes[0];
					if (currChange.oldText == string.Empty && currChange.newText.Length == 1 && prevChange.newText != string.Empty)
					{
						int currCharClass = GetCharClass(currChange.newText[0]);
						int prevCharClass = GetCharClass(prevChange.newText[prevChange.newText.Length - 1]);
						if (currCharClass == prevCharClass)
						{
							addNewRecord = false;
							prevChange.newText += currChange.newText;
							last.changes[0] = prevChange;
							last.postCaretPos = tempUndoRecord.postCaretPos.Clone();
							last.postSelectionPos = tempUndoRecord.postSelectionPos.Clone();
							//undoBuffer[undoPosition - 1] = prevRecord;
						}
					}
				}
			}
		}

		if (addNewRecord)
		{
			undoBuffer.Add(tempUndoRecord);
			++undoPosition;
			++currentChangeId;
		}
		tempUndoRecord = new UndoRecord();

		foreach (FormatedLine formatedLine in updatedLines)
			formatedLine.lastChange = currentChangeId;
	}

	public CaretPos DeleteText(CaretPos fromPos, CaretPos toPos)
	{
		CaretPos from = fromPos.Clone();
		CaretPos to = toPos.Clone();

		int fromTo = from.CompareTo(to);
		if (fromTo == 0)
			return from.Clone();

		RegisterUndoText("Delete Text", from, to, string.Empty);

		if (fromTo > 0)
		{
			CaretPos temp = from;
			from = to;
			to = temp;
		}

		if (from.line == to.line)
		{
			lines[from.line] = lines[from.line].Remove(from.characterIndex, to.characterIndex - from.characterIndex);
		}
		else
		{
			lines[from.line] = lines[from.line].Substring(0, from.characterIndex) + lines[to.line].Substring(to.characterIndex);
			lines.RemoveRange(from.line + 1, to.line - from.line);
			for (int i = 1; to.line + i < formatedLines.Length; ++i)
				formatedLines[from.line + i] = formatedLines[to.line + i];
			Array.Resize(ref formatedLines, formatedLines.Length - to.line + from.line);
			numParsedLines -= to.line - from.line;

			NotifyRemovedLines(from.line + 1, to.line - from.line);
		}

		return from;
	}

	public CaretPos InsertText(CaretPos position, string text)
	{
		RegisterUndoText("Insert Text", position, position, text);

		CaretPos pos = new CaretPos { characterIndex = position.characterIndex, column = position.column, virtualColumn = position.column, line = position.line };
		CaretPos end = new CaretPos { characterIndex = position.characterIndex, column = position.column, virtualColumn = position.column, line = position.line };

		string[] insertLines = text.Split(new char[] { '\n' }, StringSplitOptions.None);

		if (insertLines.Length == 1)
		{
			lines[pos.line] = lines[pos.line].Insert(pos.characterIndex, text);

			end.characterIndex += text.Length;
			end.column = end.virtualColumn = CharIndexToColumn(end.characterIndex, end.line);
		}
		else
		{
			lines.Insert(pos.line + 1, insertLines[insertLines.Length - 1] + lines[pos.line].Substring(pos.characterIndex));
			lines[pos.line] = lines[pos.line].Substring(0, pos.characterIndex) + insertLines[0];
			for (int i = 1; i < insertLines.Length - 1; ++i)
				lines.Insert(pos.line + i, insertLines[i]);

			end.characterIndex = insertLines[insertLines.Length - 1].Length;
			end.line = pos.line + insertLines.Length - 1;
			end.column = end.virtualColumn = CharIndexToColumn(end.characterIndex, end.line);

			Array.Resize(ref formatedLines, formatedLines.Length + insertLines.Length - 1);
			for (int i = formatedLines.Length - 1; i > end.line; --i)
				formatedLines[i] = formatedLines[i - insertLines.Length + 1];
			for (int i = 1; i <= insertLines.Length - 1; ++i)
				formatedLines[pos.line + i] = new FormatedLine();
			numParsedLines = formatedLines.Length;

			NotifyInsertedLines(pos.line + 1, insertLines.Length - 1);
		}

		return end;
	}

	public delegate void InsertedLinesDelegate(int lineIndex, int numLines);
	public InsertedLinesDelegate onInsertedLines;

	private void NotifyInsertedLines(int lineIndex, int numLines)
	{
		if (onInsertedLines != null)
			onInsertedLines(lineIndex, numLines);
	}

	public delegate void RemovedLinesDelegate(int lineIndex, int numLines);
	public RemovedLinesDelegate onRemovedLines;

	private void NotifyRemovedLines(int lineIndex, int numLines)
	{
		if (onRemovedLines != null)
			onRemovedLines(lineIndex, numLines);
	}

	public int FirstNonWhitespace(int atLine)
	{
		int index = 0;
		string line = lines[atLine];
		while (index < line.Length)
		{
			char c = line[index];
			if (c != ' ' && c != '\t')
				break;
			++index;
		}
		return index;
	}
	
#region Parser
	private static string[] spaces = { "    ", "   ", "  ", " " };
	public static string ExpandTabs(string s)
	{
		// Tabs must be replaced with spaces for proper alignment
		int tabPos;
		int startFrom = 0;
		StringBuilder sb = new StringBuilder();
		while ((tabPos = s.IndexOf('\t', startFrom)) != -1)
		{
			sb.Append(s, startFrom, tabPos - startFrom);
			sb.Append(spaces[sb.Length & 3]);
			startFrom = tabPos + 1;
		}
		sb.Append(s.Substring(startFrom));
		return sb.ToString();
	}

	private static char[] whitespaces = { ' ', '\t' };

	private void Parse(int parseToLine)
	{
		// Is there still anything left for reading/parsing?
		if (streamReader == null)
			return;

		// Reading lines till parseToLine-th line
		for (int i = numParsedLines; i < parseToLine; ++i)
		{
			string line = "";
			if (i == 0)
			{
				StringBuilder sb = new StringBuilder();
				while (!streamReader.EndOfStream)
				{
					char[] buffer = new char[1];
					streamReader.ReadBlock(buffer, 0, 1);
					if (buffer[0] == '\r' || buffer[0] == '\n')
					{
						lineEnding = buffer[0].ToString();
						if (!streamReader.EndOfStream)
						{
							string next = char.ConvertFromUtf32(streamReader.Peek());
							if (next != lineEnding && (next == "\r" || next == "\n"))
							{
								lineEnding += next;
								streamReader.ReadBlock(buffer, 0, 1);
							}
						}
						break;
					}
					else
					{
						sb.Append(buffer[0]);
					}
				}
				line = sb.ToString();

				if (streamReader != null)
				{
					codePage = streamReader.CurrentEncoding.CodePage;
				}
			}
			else
			{
				line = streamReader.ReadLine();
			}

			if (line == null)
			{
				if (streamReader.BaseStream.Position > 0)
				{
					streamReader.BaseStream.Position -= 1;
					int last = streamReader.BaseStream.ReadByte();
					if (last == 0 && streamReader.BaseStream.Position > 1)
					{
						streamReader.BaseStream.Position -= 2;
						last = streamReader.BaseStream.ReadByte();
					}
					if (last == 10 || last == 13)
					{
						lines.Add(String.Empty);
					}
				}

				streamReader.Close();
				streamReader.Dispose();
				streamReader = null;
				needsReload = false;
				break;
			}

			lines.Add(line);
		}
		if (formatedLines.Length == parseToLine)
			return;

		parseToLine = Math.Min(parseToLine, lines.Count);
		Array.Resize(ref formatedLines, parseToLine);

		for (int currentLine = numParsedLines; currentLine < parseToLine; ++currentLine)
		{
			FormatLine(currentLine);
		}

		numParsedLines = parseToLine;
	}

	public void UpdateHighlighting(int fromLine, int toLineInclusive)
	{
		int line = fromLine;
		while (line <= toLineInclusive)
		{
			FormatLine(line);
			//formatedLines[line].lineFlags = LineFlags.TrackChangesBeforeSave;
			formatedLines[line].lastChange = currentChangeId;
			updatedLines.Add(formatedLines[line]);
			++line;
		}

		while (line < formatedLines.Length)
		{
			BlockState prevState = formatedLines[line].blockState;
			FormatLine(line);
			if (prevState == formatedLines[line].blockState)
				break;
			++line;
		}

		UpdateViews();
	}

	public void ReformatLines(int fromLine, int toLineInclusive)
	{
		int line = fromLine;
		while (line <= toLineInclusive)
		{
			FormatLine(line);
			++line;
		}
	}

	public delegate void LineFormattedDelegate(int line);
	public LineFormattedDelegate onLineFormatted;

	private void FormatLine(int currentLine)
	{
		FormatLineInternal(currentLine);
		if (onLineFormatted != null)
			onLineFormatted(currentLine);
	}

	private void FormatLineInternal(int currentLine)
	{
		FormatedLine formatedLine = formatedLines[currentLine];
		if (formatedLine == null)
		{
			formatedLine = formatedLines[currentLine] = new FormatedLine();
			formatedLine.lastChange = currentChangeId;
		}

		formatedLine.blockState = currentLine > 0 ? formatedLines[currentLine - 1].blockState : 0;

		string line = ExpandTabs(lines[currentLine]);
		if (line.Length == 0)
		{
			formatedLine.textBlocks = new TextBlock[] { new TextBlock(string.Empty, styles.normalStyle) };
			return;
		}

		if (line.Length > longestLine)
			longestLine = line.Length;

		List<TextBlock> blocks = new List<TextBlock>();
		
		if (isText)
		{
			PushComment(ref blocks, line, styles.normalStyle);
			formatedLine.textBlocks = blocks.ToArray();
			return;
		}

		bool checkPreprocessor = true;
		int startIndex = 0;
		while (startIndex < line.Length)
		{
			int index;

			if (formatedLine.blockState == BlockState.CommentBlock)
			{
				index = line.IndexOf("*/", startIndex);
				if (index == -1)
				{
					PushComment(ref blocks, line.Substring(startIndex));
					break;
				}
				else
				{
					PushComment(ref blocks, line.Substring(startIndex, index - startIndex + 2));
					startIndex = index + 2;
					formatedLine.blockState = BlockState.None;
					continue;
				}
			}
			else if (formatedLine.blockState == BlockState.StringBlock)
			{
				//int firstIndex = IndexOf2(line, startIndex, '\\', '\"');
				index = line.IndexOf("\"\"\"", startIndex);
				if (index == -1)
				{
					blocks.Add(new TextBlock(line.Substring(startIndex), styles.stringStyle));
					break;
				}
				else
				{
					blocks.Add(new TextBlock(line.Substring(startIndex, index - startIndex + 3), styles.stringStyle));
					startIndex = index + 3;
					formatedLine.blockState = BlockState.None;
					continue;
				}
			}

			if (isBooFile)
				index = IndexOf5(line, startIndex, "\"", "'", "#", "//", "/*");
			else if (!checkPreprocessor)
				index = IndexOf6(line, startIndex, "\"", "'", "#", "@\"", "//", "/*");
			else
				index = IndexOf5(line, startIndex, "\"", "'", "@\"", "//", "/*");
			if (index == -1)
				index = line.Length;

			if (index > 0)
			{
				string directive = PushCode(ref blocks, line.Substring(startIndex, index - startIndex), startIndex == 0
					|| checkPreprocessor && line.Substring(0, startIndex).Trim(whitespaces) == string.Empty);
				if (directive != null)
				{
					index = line.IndexOf(directive) + directive.Length;
					if (index == line.Length)
						break;

					int indexLineComment = directive.Trim(whitespaces) == "#region" ? -1 : line.IndexOf("//", index);
					if (indexLineComment != -1)
					{
						blocks.Add(new TextBlock(line.Substring(index, indexLineComment - index), styles.normalStyle));
						PushComment(ref blocks, line.Substring(indexLineComment));
					}
					else
					{
						blocks.Add(new TextBlock(line.Substring(index), styles.normalStyle));
					}
					break;
				}
				else
					checkPreprocessor = false;
			}

			startIndex = index;

			if (index < line.Length)
			{
				if (line[index] == '@')
					++index;
				else if (isBooFile && index < line.Length - 2 && line.Substring(index, 3) == "\"\"\"")
				{
					// String block starting with """
					blocks.Add(new TextBlock("\"\"\"", styles.stringStyle));
					startIndex += 3;
					formatedLine.blockState = BlockState.StringBlock;
					continue;
				}

				char terminalChar = line[index];
				if (terminalChar == '\"' || terminalChar == '\'')
				{
					// String, Char, or RegExp literal
					for (++index; index < line.Length; )
					{
						index = IndexOf2(line, index, terminalChar, '\\');
						if (index == -1)
						{
							index = line.Length;
							break;
						}
						else if (line[index] == '\\')
						{
							++index;
							if (index == line.Length)
								break;
							++index;
						}
						else
						{
							++index;
							break;
						}
					};

					blocks.Add(new TextBlock(line.Substring(startIndex, index - startIndex), styles.stringStyle));
					startIndex = index;
				}
				else if (line[index] == '#' || line[index + 1] == '/')
				{
					// Comment till end of line
					PushComment(ref blocks, line.Substring(index));
					break;
				}
				else
				{
					// Comment block starting with /*
					PushComment(ref blocks, line.Substring(index, 2));
					startIndex += 2;
					formatedLine.blockState = BlockState.CommentBlock;
				}
			}
		}

		formatedLine.textBlocks = blocks.ToArray();
	}

	static Regex emailRegex = new Regex(@"\b([A-Z0-9._%-]+)@([A-Z0-9.-]+\.[A-Z]{2,6})\b", RegexOptions.IgnoreCase);

	private void PushComment(ref List<TextBlock> blocks, string line)
	{
		PushComment(ref blocks, line, null);
	}

	private void PushComment(ref List<TextBlock> blocks, string line, GUIStyle commentStyle)
	{
		string address;
		int index;
		
		if (commentStyle == null)
			commentStyle = styles.commentStyle;

		for (int startAt = 0; startAt < line.Length; )
		{
			int hyperlink = IndexOf3(line, startAt, "http://", "https://", "ftp://");
			if (hyperlink == -1)
				hyperlink = line.Length;

			while (hyperlink != startAt)
			{
				Match emailMatch = emailRegex.Match(line, startAt, hyperlink - startAt);
				if (emailMatch.Success)
				{
					if (emailMatch.Index > startAt)
						blocks.Add(new TextBlock(line.Substring(startAt, emailMatch.Index - startAt), commentStyle));

					address = line.Substring(emailMatch.Index, emailMatch.Length);
					blocks.Add(new TextBlock(address, styles.mailtoStyle));
					address = "mailto:" + address;
					if (IsLoading)
					{
						index = Array.BinarySearch<string>(hyperlinks, address, StringComparer.OrdinalIgnoreCase);
						if (index < 0)
							ArrayUtility.Insert(ref hyperlinks, -1 - index, address);
					}

					startAt = emailMatch.Index + emailMatch.Length;
					continue;
				}

				blocks.Add(new TextBlock(line.Substring(startAt, hyperlink - startAt), commentStyle));
				startAt = hyperlink;
			}

			if (startAt == line.Length)
				break;

			int i = line.IndexOf(':', startAt) + 3;
			while (i < line.Length)
			{
				char c = line[i];
				if (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c >= '0' && c <= '9' || c == '_' || c == '.' ||
					c == '-' || c == '=' || c == '+' || c == '%' || c == '&' || c == '?' || c == '/' || c == '#')
					++i;
				else
					break;
			}

			address = line.Substring(startAt, i - startAt);
			blocks.Add(new TextBlock(address, styles.hyperlinkStyle));
			if (IsLoading)
			{
				index = Array.BinarySearch<string>(hyperlinks, address, StringComparer.OrdinalIgnoreCase);
				if (index < 0)
					ArrayUtility.Insert(ref hyperlinks, -1 - index, address);
			}

			startAt = i;
		}
	}

	private string PushCode(ref List<TextBlock> blocks, string line, bool checkPreprocessor)
	{
		int startAt = 0;
		while (startAt < line.Length)
		{
			char c = line[startAt];
			if (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c == '_' || checkPreprocessor && c == '#')
			{
				int i = startAt + 1;
				for (; i < line.Length; ++i)
				{
					c = line[i];
					if (!(c >= '0' && c <= '9' || c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c == '_'))
						break;
				}
				string word = line.Substring(startAt, i - startAt);

				if (checkPreprocessor && IsPreprocessorDirective(word))
				{
					blocks.Add(new TextBlock(word, styles.preprocessorStyle));
					return word;
				}
				checkPreprocessor = false;

				if (IsKeyword(word))
					blocks.Add(new TextBlock(word, styles.keywordStyle));
				else if (IsBuiltInLiteral(word))
					blocks.Add(new TextBlock(word, styles.constantStyle));
				else if (IsBuiltInType(word) || IsUnityType(word))
					blocks.Add(new TextBlock(word, styles.userTypeStyle));
				else
					blocks.Add(new TextBlock(word, styles.normalStyle));
				startAt = i;
			}
			else
			{
				int i = startAt + 1;
				for (; i < line.Length; ++i)
				{
					c = line[i];
					if (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c == '_' || c == '#')
						break;
				}
				blocks.Add(new TextBlock(line.Substring(startAt, i - startAt), styles.normalStyle));
				startAt = i;

				checkPreprocessor = checkPreprocessor && line.Substring(0, i).Trim(whitespaces) == string.Empty;
			}
		}

		return null;
	}

	public string[] PreprocessorDirectives { get { return isShader ? shaderPreprocessor : preprocessor; } }
	public string[] Keywords { get { return isCsFile ? csKeywords : isJsFile ? jsKeywords : isBooFile ? booKeywords : isShader ? shaderKeywords : new string[0]; } }
	public string[] BuiltInLiterals { get { return isShader ? shaderLiterals : scriptLiterals; } }
	public string[] BuiltInTypes { get { return isCsFile ? csTypes : isJsFile ? jsTypes : isBooFile ? booTypes : isShader ? shaderTypes : new string[0]; } }
	//public string[] BuiltInConstants { get { return isCsFile ? csTypes : isJsFile ? jsTypes : isBooFile ? booTypes : new string[0]; } }

	#region private static definitions

	private static string[] preprocessor = new string[] {
		"#define", "#elif", "#else", "#endif", "#endregion", "#error", "#if", "#line", "#pragma", "#region", "#undef", "#warning"
	};

	private static string[] shaderPreprocessor = new string[] {
		"#define", "#elif", "#else", "#endif", "#error", "#if", "#ifdef", "#ifndef", "#include", "#pragma", "#undef",
		"CGINCLUDE", "CGPROGRAM", "ENDCG", "GLSLEND", "GLSLPROGRAM"
	};

	private static string[] scriptLiterals = new string[] {
		"false", "null", "true",
	};

	private static string[] csKeywords = new string[] {
		"abstract", "as", "base", "break", "case", "catch", "checked", "class", "const", "continue",
		"default", "delegate", "do", "else", "enum", "event", "explicit", "extern", "finally",
		"fixed", "for", "foreach", "goto", "if", "implicit", "in", "interface", "internal", "is", "lock",
		"namespace", "new", "operator", "out", "override", "params", "private", "protected",
		"public", "readonly", "ref", "return", "sealed", "sizeof", "stackalloc", "static", "struct",
		"switch", "this", "throw", "try", "typeof", "unchecked", "unsafe", "using", "virtual",
		"volatile", "while", "yield"
	};

	private static string[] csTypes = new string[] {
		"bool", "byte", "char", "decimal", "double", "float", "int", "long", "object", "sbyte", "short",
		"string", "uint", "ulong", "ushort", "var", "void"
	};

	private static string[] jsKeywords = new string[] {
		"abstract", "else", "instanceof", "super", "enum", "switch", "break", "static", "export",
		"interface", "synchronized", "extends", "let", "this", "case", "with", "throw",
		"catch", "final", "native", "throws", "finally", "new", "transient", "class",
		"const", "for", "package", "try", "continue", "private", "typeof", "debugger", "goto",
		"protected", "default", "if", "public", "delete", "implements", "return", "volatile", "do",
		"import", "while", "in", "function"
	};

	private static string[] jsTypes = new string[] {
		"boolean", "byte", "char", "double", "float", "int", "long", "short", "var", "void"
	};

	private static string[] booKeywords = new string[] {
		"abstract", "and", "as", "break", "callable", "cast", "class", "const", "constructor", "destructor", "continue",
		"def", "do", "elif", "else", "enum", "ensure", "event", "except", "final", "for", "from", "given", "get", "goto",
		"if", "interface", "in", "include", "import", "is", "isa", "mixin", "namespace", "not", "or", "otherwise",
		"override", "pass", "raise", "retry", "self", "struct", "return", "set", "success", "try", "transient", "virtual",
		"while", "when", "unless", "yield", 

		"public", "protected", "private", "internal", "static", 

		// builtin
		"len", "__addressof__", "__eval__", "__switch__", "array", "matrix", "typeof", "assert", "print", "gets", "prompt", 
		"enumerate", "zip", "filter", "map",
	};

	private static string[] booTypes = new string[] {
		"bool", "byte", "char", "date", "decimal", "double", "int", "long", "object", "sbyte", "short", "single", "string",
		"timespan", "uint", "ulong", "ushort", "void"
	};

	private static string[] shaderKeywords = new string[] {
		"AlphaTest", "Ambient", "Bind", "Blend", "BorderScale", "ColorMask", "ColorMaterial", "Combine", "ConstantColor",
		"Cull", "Density", "Diffuse", "Emission", "Fallback", "Fog", "Lerp", "Lighting", "LightmapMode", "LightMode",
		"LightTexCount", "Material", "Matrix", "Mode", "Name", "Offset", "RequireOptions", "SeparateSpecular", "SetTexture",
		"Shininess", "Specular", "TexGen", "TextureScale", "TextureSize", "UsePass", "ZTest", "ZWrite",
	};

	private static string[] shaderLiterals = new string[] {
		"A", "Always", "AmbientAndDiffuse", "AppDstAdd", "AppSrcAdd", "Back", "CubeNormal", "CubeReflect", "DstAlpha",
		"DstColor", "Emission", "EyeLinear", "Exp", "Exp2", "Front", "GEqual", "Greater", "LEqual", "Less", "Linear",
		"None", "Normal", "NotEqual", "ObjectLinear", "Off", "On", "One", "OneMinusDstAlpha", "OneMinusDstColor",
		"OneMinusSrcAlpha", "OneMinusSrcColor", "Pixel", "PixelOnly", "PixelOrNone", "RGB", "SoftVegetation", "SrcAlpha",
		"SrcColor", "SphereMap", "Vertex", "VertexAndPixel", "VertexOnly", "VertexOrNone", "VertexOrPixel", "Tangent",
		"Texcoord", "Texcoord0", "Texcoord1", "Zero",
	};

	private static string[] shaderTypes = new string[] {
		"2D", "BindChannels", "Category", "Color", "Constant", "Cube", "Float", "Fog", "GrabPass", "Pass", "Previous",
		"Properties", "Range", "Rect", "Shader", "SubShader", "Tags", "Texture", "Vector", "_CosTime", "_CubeNormalize",
		"_Light2World", "_ModelLightColor", "_Object2Light", "_Object2World", "_ObjectSpaceCameraPos", "_ObjectSpaceLightPos",
		"_ProjectionParams", "_SinTime", "_SpecFalloff", "_SpecularLightColor", "_Time", "_World2Light", "_World2Object",
	};

	private static string[] unityTypes = null; /*new string[] /*{
		// Runtime classes
		"ADBannerView", "ADError", "ADInterstitialAd", "AccelerationEvent", "ActionScript", "AndroidInput",
		"AndroidJNIHelper", "AndroidJNI", "AndroidJavaObject", "AndroidJavaClass", "AnimationCurve", "AnimationEvent",
		"AnimationState", "Application", "Array", "AudioSettings", "BitStream", "BoneWeight", "Bounds", "Caching",
		"ClothSkinningCoefficient", "Collision", "Color32", "Color", "CombineInstance", "Compass", "ContactPoint",
		"ControllerColliderHit", "Debug", "DetailPrototype", "Event", "GL", "GUIContent", "GUILayoutOption",
		"GUILayoutUtility", "GUILayout", "GUISettings", "GUIStyleState", "GUIStyle", "GUIUtility", "GUI", "GeometryUtility",
		"Gizmos", "Graphics", "Gyroscope", "Handheld", "Hashtable", "HostData", "IAchievementDescription", "IAchievement",
		"ILeaderboard", "IScore", "ISocialPlatform", "GameCenterPlatform", "IUserProfile", "ILocalUser", "Input",
		"JointDrive", "JointLimits", "JointMotor", "JointSpring", "Keyframe", "LayerMask", "LightmapData", "LightmapSettings",
		"LocalNotification", "LocationInfo", "LocationService", "MasterServer", "MaterialPropertyBlock", "Mathf", "Matrix4x4",
		"Microphone", "NavMeshHit", "NavMeshPath", "NetworkMessageInfo", "NetworkPlayer", "NetworkViewID", "Network",
		"NotificationServices", "Object", "AnimationClip", "AssetBundle", "AudioClip", "Component", "Behaviour", "Animation",
		"AudioChorusFilter", "AudioDistortionFilter", "AudioEchoFilter", "AudioHighPassFilter", "AudioListener",
		"AudioLowPassFilter", "AudioReverbFilter", "AudioReverbZone", "AudioSource", "Camera", "ConstantForce", "GUIElement",
		"GUIText", "GUITexture", "GUILayer", "LensFlare", "Light", "MonoBehaviour", "Terrain", "NavMeshAgent", "NetworkView",
		"Projector", "Skybox", "Cloth", "InteractiveCloth", "SkinnedCloth", "Collider", "BoxCollider", "CapsuleCollider",
		"CharacterController", "MeshCollider", "SphereCollider", "TerrainCollider", "WheelCollider", "Joint", "CharacterJoint",
		"ConfigurableJoint", "FixedJoint", "HingeJoint", "SpringJoint", "LODGroup", "LightProbeGroup", "MeshFilter",
		"OcclusionArea", "OcclusionPortal", "OffMeshLink", "ParticleAnimator", "ParticleEmitter", "ParticleSystem", "Renderer",
		"ClothRenderer", "LineRenderer", "MeshRenderer", "ParticleRenderer", "ParticleSystemRenderer", "SkinnedMeshRenderer",
		"TrailRenderer", "Rigidbody", "TextMesh", "Transform", "Tree", "Flare", "Font", "GameObject", "LightProbes",
		"Material", "ProceduralMaterial", "Mesh", "NavMesh", "PhysicMaterial", "QualitySettings", "ScriptableObject",
		"GUISkin", "Shader", "TerrainData", "TextAsset", "Texture", "Cubemap", "MovieTexture", "RenderTexture", "Texture2D",
		"WebCamTexture", "OffMeshLinkData", "ParticleSystem", "Particle", "Path", "Physics", "Ping", "Plane",
		"PlayerPrefsException", "PlayerPrefs", "ProceduralPropertyDescription", "Profiler", "Quaternion", "Random", "Range",
		"Ray", "RaycastHit", "RectOffset", "Rect", "RemoteNotification", "RenderBuffer", "RenderSettings", "Resolution",
		"Resources", "Screen", "Security", "SleepTimeout", "Social", "SoftJointLimit", "SplatPrototype",
		"StaticBatchingUtility", "String", "SystemInfo", "Time", "TouchScreenKeyboard", "Touch", "TreeInstance",
		"TreePrototype", "Vector2", "Vector3", "Vector4", "WWWForm", "WWW", "WebCamDevice", "WheelFrictionCurve", "WheelHit",
		"YieldInstruction", "AsyncOperation", "AssetBundleCreateRequest", "AssetBundleRequest", "Coroutine",
		"WaitForEndOfFrame", "WaitForFixedUpdate", "WaitForSeconds", "iPhoneInput", "iPhoneSettings", "iPhoneUtils", "iPhone",

		// Runtime attributes
		"AddComponentMenu", "ContextMenu", "ExecuteInEditMode", "HideInInspector", "ImageEffectOpaque",
		"ImageEffectTransformsToLDR", "InitializeOnLoad", "NonSerialized", "NotConvertedAttribute", "NotRenamedAttribute",
		"RPC", "RequireComponent", "Serializable", "SerializeField",

		// Runtime enumerations
		"ADErrorCode", "ADPosition", "ADSizeIdentifier", "AnimationBlendMode", "AnimationCullingType", "AnisotropicFiltering",
		"AudioReverbPreset", "AudioRolloffMode", "AudioSpeakerMode", "AudioType", "AudioVelocityUpdateMode", "BlendWeights",
		"CalendarIdentifier", "CalendarUnit", "CameraClearFlags", "CollisionDetectionMode", "CollisionFlags", "ColorSpace",
		"ConfigurableJointMotion", "ConnectionTesterStatus", "CubemapFace", "DepthTextureMode", "DetailRenderMode",
		"DeviceOrientation", "DeviceType", "EventModifiers", "EventType", "FFTWindow", "FilterMode", "FocusType", "FogMode",
		"FontStyle", "ForceMode", "FullScreenMovieControlMode", "FullScreenMovieScalingMode", "HideFlags", "IMECompositionMode",
		"ImagePosition", "JointDriveMode", "JointProjectionMode", "KeyCode", "LightRenderMode", "LightShadows", "LightType",
		"LightmapsMode", "LocationServiceStatus", "LogType", "MasterServerEvent", "NavMeshPathStatus", "NetworkConnectionError",
		"NetworkDisconnection", "NetworkLogLevel", "NetworkPeerType", "NetworkReachability", "NetworkStateSynchronization",
		"ObstacleAvoidanceType", "OffMeshLinkType", "ParticleRenderMode", "ParticleSystemRenderMode", "PhysicMaterialCombine",
		"PlayMode", "PrimitiveType", "ProceduralCacheSize", "ProceduralProcessorUsage", "ProceduralPropertyType", "QueueMode",
		"RPCMode", "RemoteNotificationType", "RenderTextureFormat", "RenderTextureReadWrite", "RenderingPath",
		"RigidbodyConstraints", "RigidbodyInterpolation", "RotationDriveMode", "RuntimePlatform", "ScaleMode",
		"ScreenOrientation", "SendMessageOptions", "ShadowProjection", "SkinQuality", "Space", "SystemLanguage", "TextAlignment",
		"TextAnchor", "TextClipping", "TextureCompressionQuality", "TextureFormat", "TextureWrapMode", "ThreadPriority",
		"TimeScope", "TouchPhase", "TouchScreenKeyboardType", "UserAuthorization", "UserScope", "UserState", "WrapMode",
		"iPhoneGeneration",

		// Editor classes
		"AnimationClipCurveData", "AnimationUtility", "ArrayUtility", "AssetDatabase", "AssetImporter", "AudioImporter",
		"ModelImporter", "MovieImporter", "SubstanceImporter", "TextureImporter", "TrueTypeFontImporter",
		"AssetModificationProcessor", "AssetPostprocessor", "AssetStore", "BuildPipeline", "DragAndDrop", "EditorApplication",
		"EditorBuildSettings", "EditorGUILayout", "EditorGUIUtility", "EditorGUI", "EditorPrefs", "EditorStyles",
		"EditorUserBuildSettings", "EditorUtility", "EditorWindow", "ScriptableWizard", "Editor", "FileUtil",
		"GameObjectUtility", "GenericMenu", "HandleUtility", "Handles", "Help", "LODUtility", "LightmapEditorSettings",
		"Lightmapping", "MenuCommand", "MeshUtility", "ModelImporterClipAnimation", "MonoScript", "NavMeshBuilder",
		"ObjectNames", "Android", "Wii", "iOS", "PlayerSettings", "PrefabUtility", "ProceduralTexture", "PropertyModification",
		"Selection", "SerializedObject", "SerializedProperty", "StaticOcclusionCullingVisualization", "StaticOcclusionCulling",
		"SubstanceArchive", "TextureImporterSettings", "Tools", "Undo", "UnwrapParam", "Unwrapping",

		// Editor attributes
		"CanEditMultipleObjects", "CustomEditor", "DrawGizmo", "MenuItem", "PreferenceItem",

		// Editor enumerations
		"AndroidBuildSubtarget", "AndroidPreferredInstallLocation", "AndroidSdkVersions",
		"AndroidShowActivityIndicatorOnLoading", "AndroidSplashScreenScale", "AndroidTargetDevice", "AndroidTargetGraphics",
		"ApiCompatibilityLevel", "AspectRatio", "AssetDeleteResult", "AssetMoveResult", "AudioImporterFormat",
		"AudioImporterLoadType", "BuildAssetBundleOptions", "BuildOptions", "BuildTargetGroup", "BuildTarget",
		"DragAndDropVisualMode", "DrawCameraMode", "EditorSkin", "ExportPackageOptions", "FontRenderMode", "FontTextureCase",
		"GizmoType", "ImportAssetOptions", "InspectorMode", "LightmapBakeQuality", "MessageType",
		"ModelImporterAnimationCompression", "ModelImporterGenerateAnimations", "ModelImporterMaterialName",
		"ModelImporterMaterialSearch", "ModelImporterMeshCompression", "ModelImporterTangentSpaceMode", "MouseCursor",
		"PS3BuildSubtarget", "PivotMode", "PivotRotation", "PrefabType", "ProceduralOutputType", "RemoveAssetOptions",
		"ReplacePrefabOptions", "ResolutionDialogSetting", "ScriptCallOptimizationLevel", "SelectionMode",
		"SerializedPropertyType", "StaticEditorFlags", "StaticOcclusionCullingMode", "StrippingLevel", "TextureImporterFormat",
		"TextureImporterGenerateCubemap", "TextureImporterMipFilter", "TextureImporterNPOTScale", "TextureImporterNormalFilter",
		"TextureImporterType", "Tool", "UIOrientation", "ViewTool", "WiiBuildDebugLevel", "WiiBuildSubtarget", "WiiHio2Usage",
		"WiiMemoryArea", "WiiMemoryLabel", "WiiRegion", "XboxBuildSubtarget", "XboxRunMethod", "iOSSdkVersion",
		"iOSShowActivityIndicatorOnLoading", "iOSStatusBarStyle", "iOSTargetDevice", "iOSTargetOSVersion", "iOSTargetPlatform",
		"iOSTargetResolution", 
	};*/
	#endregion

	private bool IsPreprocessorDirective(string word)
	{
		return Array.BinarySearch<string>(PreprocessorDirectives, word, isShader ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal) >= 0;
	}

	private bool IsKeyword(string word)
	{
		return Array.BinarySearch<string>(Keywords, word, isShader ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal) >= 0;
	}

	private bool IsBuiltInLiteral(string word)
	{
		return Array.BinarySearch<string>(BuiltInLiterals, word, isShader ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal) >= 0;
	}

	private bool IsBuiltInType(string word)
	{
		return Array.BinarySearch<string>(BuiltInTypes, word, isShader ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal) >= 0;
	}

	private bool IsUnityType(string word)
	{
		return isShader ? false : Array.BinarySearch<string>(unityTypes, word) >= 0;
	}

	private static int IndexOf2(string line, int startIndex, char s1, char s2)
	{
		uint i1 = (uint)line.IndexOf(s1, startIndex);
		uint i2 = (uint)line.IndexOf(s2, startIndex);
		return (int)System.Math.Min(i1, i2);
	}

	private static int IndexOf3(string line, int startIndex, string s1, string s2, string s3)
	{
		uint i1 = (uint)line.IndexOf(s1, startIndex, StringComparison.OrdinalIgnoreCase);
		uint i2 = (uint)line.IndexOf(s2, startIndex, StringComparison.OrdinalIgnoreCase);
		uint i3 = (uint)line.IndexOf(s3, startIndex, StringComparison.OrdinalIgnoreCase);
		return (int)System.Math.Min(System.Math.Min(i1, i2), i3);
	}

	private static int IndexOf5(string line, int startIndex, string s1, string s2, string s3, string s4, string s5)
	{
		uint i1 = (uint)line.IndexOf(s1, startIndex);
		uint i2 = (uint)line.IndexOf(s2, startIndex);
		uint i3 = (uint)line.IndexOf(s3, startIndex);
		uint i4 = (uint)line.IndexOf(s4, startIndex);
		uint i5 = (uint)line.IndexOf(s5, startIndex);
		return (int)System.Math.Min(i5, System.Math.Min(System.Math.Min(i1, i2), System.Math.Min(i3, i4)));
	}

	private static int IndexOf6(string line, int startIndex, string s1, string s2, string s3, string s4, string s5, string s6)
	{
		uint i1 = (uint)line.IndexOf(s1, startIndex);
		uint i2 = (uint)line.IndexOf(s2, startIndex);
		uint i3 = (uint)line.IndexOf(s3, startIndex);
		uint i4 = (uint)line.IndexOf(s4, startIndex);
		uint i5 = (uint)line.IndexOf(s5, startIndex);
		uint i6 = (uint)line.IndexOf(s6, startIndex);
		return (int)System.Math.Min(i6, System.Math.Min(i5, System.Math.Min(System.Math.Min(i1, i2), System.Math.Min(i3, i4))));
	}
#endregion
}
