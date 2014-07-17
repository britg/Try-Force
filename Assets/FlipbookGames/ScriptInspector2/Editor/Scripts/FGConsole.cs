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
using UnityEditor;
using System.Reflection;

public class FGConsole : EditorWindow
{
	private static System.Type consoleWindowType;
	private static FieldInfo consoleWindowField;
	private static FieldInfo consoleListViewField;
	private static FieldInfo consoleActiveTextField;
	private static MethodInfo consoleOnGUIMethod;
	private static System.Type listViewStateType;
	private static FieldInfo listViewStateRowField;
	private static FieldInfo editorWindowPosField;
	private static System.Type logEntriesType;
	private static MethodInfo getEntryMethod;
	private static MethodInfo startGettingEntriesMethod;
	private static MethodInfo endGettingEntriesMethod;
	private static System.Type logEntryType;
	private static FieldInfo logEntryLineField;
	private static FieldInfo logEntryFileField;
	private static FieldInfo logEntryInstanceIDField;
	private static object logEntry;
	
	private static int repaintOnUpdateCounter = 0;

	static FGConsole()
	{
		consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
		if (consoleWindowType != null)
		{
			consoleWindowField = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			consoleListViewField = consoleWindowType.GetField("m_ListView", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			consoleActiveTextField = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			consoleOnGUIMethod = consoleWindowType.GetMethod("OnGUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		}
		listViewStateType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ListViewState");
		if (listViewStateType != null)
		{
			listViewStateRowField = listViewStateType.GetField("row", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		}
		editorWindowPosField = typeof(EditorWindow).GetField("m_Pos", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		logEntriesType = typeof(EditorWindow).Assembly.GetType("UnityEditorInternal.LogEntries");
		if (logEntriesType != null)
		{
			getEntryMethod = logEntriesType.GetMethod("GetEntryInternal", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			startGettingEntriesMethod = logEntriesType.GetMethod("StartGettingEntries", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			endGettingEntriesMethod = logEntriesType.GetMethod("EndGettingEntries", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
		}
		logEntryType = typeof(EditorWindow).Assembly.GetType("UnityEditorInternal.LogEntry");
		if (logEntryType != null)
		{
			logEntry = System.Activator.CreateInstance(logEntryType);
			logEntryFileField = logEntryType.GetField("file", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			logEntryLineField = logEntryType.GetField("line", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			logEntryInstanceIDField = logEntryType.GetField("instanceID", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		}
	}

	[MenuItem("Window/Script Inspector Console", false, 0x898)]
	public static void ShowConsole()
	{
		GetWindow(consoleWindowType);
		GetWindow<FGConsole>("SI Console", consoleWindowType);//.title = "SI Console";
	}
	
	public static FGConsole FindInstance()
	{
		UnityEngine.Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(FGConsole));
		return (objArray.Length <= 0) ? null : (objArray[0] as EditorWindow) as FGConsole;
	}

	public static void OpenIfConsoleIsOpen()
	{
		EditorWindow console = FindInstance();
		if (console != null)
			return;

		UnityEngine.Object[] objArray = Resources.FindObjectsOfTypeAll(consoleWindowType);
		console = (objArray.Length <= 0) ? null : (objArray[0] as EditorWindow);
		if (console != null)
		{
			EditorWindow wnd = EditorWindow.focusedWindow;
			FGConsole.ShowConsole();
			if (wnd != null)
				wnd.Focus();
		}
	}

	private static void OnLog(string a, string b, LogType logType)
	{
		if (repaintOnUpdateCounter == 0)
			repaintOnUpdateCounter = 10;
	}

	protected void OnEnable()
	{
		EditorApplication.update -= OnApplicationUpdate;
		EditorApplication.update += OnApplicationUpdate;
		EditorApplication.update -= OnFirstAppUpdate;
		EditorApplication.update += OnFirstAppUpdate;
	}

	protected void OnDisable()
	{
		EditorApplication.update -= OnApplicationUpdate;
		EditorApplication.update -= OnFirstAppUpdate;
		Application.RegisterLogCallback(null);
	}

	protected static void OnFirstAppUpdate()
	{
		EditorApplication.update -= OnFirstAppUpdate;
		Application.RegisterLogCallback(OnLog);
		repaintOnUpdateCounter = 1;
	}
	
	protected void OnApplicationUpdate()
	{
		if (repaintOnUpdateCounter > 0)
		{
			if (--repaintOnUpdateCounter == 0)
				Repaint();
		}
	}

	protected void OnGUI()
	{
		EditorWindow console = consoleWindowField.GetValue(null) as EditorWindow;
		if (console == null)
		{
			EditorGUILayout.HelpBox(@"Script Inspector Console can only work when the Console tab is also open.

Click the button below to open the Console window...", MessageType.Info);
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Open Console Widnow"))
			{
				GetWindow(consoleWindowType);
				Focus();
				Repaint();
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			return;
		}

		Rect oldPosition = console.position;
		editorWindowPosField.SetValue(console, position);

		try
		{
			bool contextClick = Event.current.type == EventType.ContextClick ||
				Event.current.type == EventType.MouseUp && Event.current.button == 1 && Application.platform == RuntimePlatform.OSXEditor;
			if (contextClick && Event.current.mousePosition.y > EditorStyles.toolbar.fixedHeight)
			{
				Event.current.type = EventType.MouseDown;
				Event.current.button = 0;
				Event.current.clickCount = 1;
				try { consoleOnGUIMethod.Invoke(console, null); } catch { }
				
				DoPopupMenu(console);
				GUIUtility.ExitGUI();
			}
			else if (Event.current.type == EventType.MouseDown && Event.current.clickCount == 2 && Event.current.mousePosition.y > EditorStyles.toolbar.fixedHeight
				|| Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
			{
				OpenLogEntry(console);
				GUIUtility.ExitGUI();
			}
			consoleOnGUIMethod.Invoke(console, null);
		}
		finally
		{
			editorWindowPosField.SetValue(console, oldPosition);
		}
	}

	private static void OpenLogEntry(EditorWindow console)
	{
		var listView = consoleListViewField.GetValue(console);
		int listViewRow = listView != null ? (int)listViewStateRowField.GetValue(listView) : -1;
		if (listViewRow >= 0)
		{
			bool gotIt = false;
			startGettingEntriesMethod.Invoke(null, new object[0]);
			try {
				gotIt = (bool) getEntryMethod.Invoke(null, new object[] { listViewRow, logEntry });
			} finally {
				endGettingEntriesMethod.Invoke(null, new object[0]);
			}
			
			if (gotIt)
			{
				int line = (int)logEntryLineField.GetValue(logEntry);
				string file = (string)logEntryFileField.GetValue(logEntry);
				string guid = AssetDatabase.AssetPathToGUID(file);
				if (string.IsNullOrEmpty(guid))
				{
					int instanceID = (int)logEntryInstanceIDField.GetValue(logEntry);
					if (instanceID != 0)
					{
						file = AssetDatabase.GetAssetPath(instanceID);
						guid = AssetDatabase.AssetPathToGUID(file);
					}
				}
				if (!string.IsNullOrEmpty(guid))
					FGCodeWindow.OpenAssetInTab(guid, line);
			}
		}
	}

	private void DoPopupMenu(EditorWindow console)
	{
		var listView = consoleListViewField.GetValue(console);
		int listViewRow = listView != null ? (int) listViewStateRowField.GetValue(listView) : -1;

		if (listViewRow < 0)
			return;

		string text = (string)consoleActiveTextField.GetValue(console);
		if (string.IsNullOrEmpty(text))
			return;
		
		GenericMenu codeViewPopupMenu = new GenericMenu();

		string[] lines = text.Split('\n');
		foreach (string line in lines)
		{
			int atAssetsIndex = line.IndexOf("(at Assets/");
			if (atAssetsIndex < 0)
				continue;

			int functionNameEnd = line.IndexOf('(');
			if (functionNameEnd < 0)
				continue;
			string functionName = line.Substring(0, functionNameEnd).TrimEnd(' ');

			int lineIndex = line.LastIndexOf(':');
			if (lineIndex <= atAssetsIndex)
				continue;
			int atLine = 0;
			for (int i = lineIndex + 1; i < line.Length; ++i)
			{
				char c = line[i];
				if (c < '0' || c > '9')
					break;
				atLine = atLine * 10 + (c - '0');
			}

			atAssetsIndex += "(at ".Length;
			string assetPath = line.Substring(atAssetsIndex, lineIndex - atAssetsIndex);
			string scriptName = assetPath.Substring(assetPath.LastIndexOf('/') + 1);

			string guid = AssetDatabase.AssetPathToGUID(assetPath);
			if (!string.IsNullOrEmpty(guid))
			{
				codeViewPopupMenu.AddItem(
					new GUIContent(scriptName + " - " + functionName.Replace(':', '.') + ", line " + atLine),
					false,
					() => FGCodeWindow.OpenAssetInTab(guid, atLine));
			}
		}

		if (codeViewPopupMenu.GetItemCount() > 0)
			codeViewPopupMenu.ShowAsContext();
	}
}
