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
using System;
using FlipbookGames;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

[Serializable, StructLayout(LayoutKind.Sequential)]
public class FGTextEditor
{
	private static string[] availableThemes = new string[]
	{
		"Visual Studio",
		"Xcode",
		"Tango Dark (Oblivion)",
		"Tango Light",
		"MD Brown",
		"MD Brown - Dark",
		"Monokai",
		"Son of Obsidian",
	};

	private class Theme
	{
		public Color background;
		public Color text;
		public Color comments;
		public Color strings;
		public Color keywords;
		public Color constants;
		public Color knownTypes;
		public Color hyperlinks;
		public Color preprocessor;
		public Color lineNumbers;
		public Color lineNumbersHighlight;
		public Color lineNumbersBackground;
		public Color fold;
		public Color searchResults;
		public Color activeSelection;
		public Color passiveSelection;
		public Color trackSaved;
		public Color trackChanged;
		public Color trackReverted;
		public Color currentLine;
		public Color currentLineInactive;

		public FontStyle commentsStyle = FontStyle.Normal;
		public FontStyle stringsStyle = FontStyle.Normal;
		public FontStyle keywordsStyle = FontStyle.Normal;
		public FontStyle constantsStyle = FontStyle.Normal;
		public FontStyle typesStyle = FontStyle.Normal;
		public FontStyle hyperlinksStyle = FontStyle.Normal;
		public FontStyle preprocessorStyle = FontStyle.Normal;
	}
	
	private static Theme[] themes = new Theme[] {
		// Visual Studio
		new Theme {
			background				= Color.white,
			text					= Color.black,
			comments				= new Color32(0x00, 0x80, 0x00, 0xff),
			strings					= new Color32(0x80, 0x00, 0x00, 0xff),
			keywords				= Color.blue,
			constants               = Color.blue,
			knownTypes				= new Color32(0x2b, 0x91, 0xaf, 0xff),
			hyperlinks				= Color.blue,
			preprocessor            = Color.blue,
			lineNumbers				= new Color32(0x2b, 0x91, 0xaf, 0xff),
			lineNumbersHighlight	= Color.blue,
			lineNumbersBackground	= Color.white,
			fold					= new Color32(165, 165, 165, 255),
			searchResults			= new Color32(219, 224, 204, 255),
			activeSelection			= new Color32(51, 153, 255, 102),
			passiveSelection		= new Color32(191, 205, 219, 102),
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238, 98, 255),
			trackReverted           = new Color32(246, 201, 60, 255),
			currentLine             = new Color32(213, 213, 241, 255),
			currentLineInactive     = new Color32(228, 228, 228, 255)
		},
		
		// Xcode
		new Theme {
			background				= Color.white,
			text					= Color.black,
			comments				= new Color32(0x23, 0x97, 0x2d, 0xff),
			strings					= new Color32(0xce, 0x2f, 0x30, 0xff),
			keywords				= new Color32(0xc1, 0x2d, 0xad, 0xff),
			constants               = new Color32(0x65, 0x3c, 0x98, 0xff),
			knownTypes				= new Color32(0x80, 0x46, 0xb0, 0xff),
			hyperlinks				= new Color32(0x50, 0x6f, 0x73, 0xff),
			preprocessor            = new Color32(0x77, 0x4b, 0x31, 0xff),
			lineNumbers				= new Color32(0x6c, 0x6c, 0x6c, 0xff),
			lineNumbersHighlight	= Color.black,
			lineNumbersBackground	= Color.white,
			fold					= TangoColors.aluminium3,
			searchResults			= new Color32(219, 224, 204, 255),
			activeSelection			= new Color32(0xc7, 0xd0, 0xdb, 0xff),
			passiveSelection		= new Color32(0xc7, 0xd0, 0xdb, 0x7f),
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238, 98, 255),
			trackReverted           = new Color32(246, 201, 60, 255),
			currentLine             = new Color32(213, 213, 241, 255),
			currentLineInactive     = new Color32(228, 228, 228, 255)
		},
		
		// Tango Dark (Oblivion)
		new Theme {
			background				= TangoColors.aluminium6,
			text					= TangoColors.aluminium2,
			comments				= TangoColors.aluminium4,
			strings					= TangoColors.butter2,
			keywords				= TangoColors.plum1,
			constants               = TangoColors.orange3,
			knownTypes				= TangoColors.chameleon1,
			hyperlinks				= TangoColors.butter2,
			preprocessor            = TangoColors.skyblue1,
			lineNumbers				= TangoColors.aluminium5,
			lineNumbersHighlight	= TangoColors.aluminium3,
			lineNumbersBackground	= TangoColors.aluminium7,
			fold					= TangoColors.aluminium3,
			searchResults			= new Color32(0x00, 0x60, 0x60, 0xff),
			activeSelection			= TangoColors.aluminium5,
			passiveSelection		= TangoColors.aluminium5,
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238, 98, 255),
			trackReverted           = new Color32(246, 201, 60, 255),
			currentLine             = TangoColors.aluminium7,
			currentLineInactive     = new Color32(0x11, 0x11, 0x11, 0x80),

			preprocessorStyle       = FontStyle.Italic
		},
		
		// Tango Light
		new Theme {
			background				= Color.white,
			text					= TangoColors.aluminium7,
			comments				= TangoColors.chameleon3,
			strings					= TangoColors.plum2,
			keywords				= TangoColors.skyblue3,
			constants               = TangoColors.skyblue3,
			knownTypes				= TangoColors.chameleon3,
			hyperlinks				= Color.blue,
			preprocessor            = TangoColors.skyblue3,
			lineNumbers				= TangoColors.aluminium4,
			lineNumbersHighlight	= TangoColors.aluminium5,
			lineNumbersBackground	= Color.white,
			fold					= TangoColors.aluminium3,
			searchResults			= new Color32(0xff, 0xe2, 0xb9, 0xff),
			activeSelection			= new Color32(51, 153, 255, 102),
			passiveSelection		= new Color32(191, 205, 219, 102),
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238, 98, 255),
			trackReverted           = new Color32(246, 201, 60, 255),
			currentLine             = TangoColors.aluminium1,
			currentLineInactive     = TangoColors.aluminium1,

			preprocessorStyle       = FontStyle.Italic
		},

		// MD Brown (courtesy of Little Angel)
		new Theme {
            background              = Color.white,
            text                    = Color.black,
            comments                = new Color (0.20f, 0.60f, 0.0f),		// Green
            strings                 = new Color (1.0f, 0.14f, 1.0f),		// Pink
            keywords                = new Color (0.98f, 0.23f, 0.01f),		// Red
            constants               = new Color (1.0f, 0.14f, 1.0f),
			knownTypes              = new Color (0.58f, 0.04f, 0.0f),		// Dark Red  
            hyperlinks              = Color.blue,                           // Didn't change
			preprocessor            = new Color32(0x33, 0x66, 0x99, 0xff),
			lineNumbers             = new Color (0.50f, 0.40f, 0.28f),		// Tan, Dark
            lineNumbersHighlight    = new Color (0.25f, 0.20f, 0.14f),		// Tan, Very Dark
            lineNumbersBackground   = new Color (1.0f, 0.80f, 0.56f),		// Tan, Light
            fold                    = new Color (0.20f, 0.60f, 0.0f),		// Green
            searchResults           = new Color32(0xff, 0xe2, 0xb9, 0xff),  // Didn't change
			activeSelection			= new Color32(51, 153, 255, 102),
			passiveSelection		= new Color32(191, 205, 219, 102),
			trackSaved              = new Color32(98, 201, 98, 255),
			trackChanged            = new Color32(255, 243, 158, 255),
			trackReverted           = new Color32(236, 175, 50, 255),
			currentLine             = new Color32(253, 255, 153, 255),
			currentLineInactive     = new Color32(253, 255, 153, 192)
        },

		// MD Brown - Dark
		new Theme {
			background              = new Color (0.22f, 0.22f, 0.22f),  // Dark Grey (Pro)
			text                    = new Color (0.85f, 0.85f, 0.85f),  // Light Grey
			comments                = new Color (0.20f, 0.60f, 0.0f),   // Green
			strings                 = new Color (0.85f, 0.15f, 0.85f),  // Pink for Pro
			keywords                = new Color (1.0f, 0.33f, 0.01f),   // Red for Pro
			constants               = new Color (1.0f, 0.33f, 0.01f),
			knownTypes              = new Color (0.85f, 0.15f, 0.0f),   // Dark Red for Pro
			hyperlinks              = new Color (0.0f, 0.75f, 0.75f),   // Light Blue
			preprocessor            = new Color (1.0f, 0.33f, 0.01f),
			lineNumbers             = new Color (0.25f, 0.20f, 0.14f),  // Tan, Very Dark
			lineNumbersHighlight    = new Color (1.0f, 0.80f, 0.56f),   // Tan, Light
			lineNumbersBackground   = new Color (0.50f, 0.40f, 0.28f),  // Tan, Dark
			fold                    = new Color (0.20f, 0.60f, 0.0f),   // Green
			searchResults           = new Color (0.50f, 0.45f, 0.14f, 0.5f),
			activeSelection			= new Color (0.30f, 0.40f, 0.48f, 0.7f),
			passiveSelection		= new Color (0.30f, 0.40f, 0.48f, 0.4f),
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238, 98, 255),
			trackReverted           = new Color32(246, 201, 60, 255),
			currentLine             = new Color(0.20f, 0.18f, 0.14f),
			currentLineInactive     = new Color(0.25f, 0.20f, 0.14f)
		},

		// Monokai
		new Theme {
			background              = new Color32(39, 40, 34, 255),
			text                    = new Color32(248, 248, 242, 255),
			comments                = new Color32(117, 113, 94, 255),
			strings                 = new Color32(230, 219, 106, 255),
			keywords                = new Color32(249, 38, 114, 255),
			constants               = new Color32(174, 129, 255, 255),
			knownTypes              = new Color32(102, 218, 236, 255),
			hyperlinks              = new Color32(127, 74, 129, 255),
			preprocessor            = new Color32(166, 226, 46, 255),
			lineNumbers             = new Color32(188, 188, 188, 255),
			lineNumbersHighlight    = new Color32(248, 248, 242, 255),
			lineNumbersBackground   = new Color32(39, 40, 34, 255),
			fold                    = new Color32(59, 58, 50, 255),
			searchResults           = new Color32(0, 96, 96, 0),
			activeSelection			= new Color32(73, 72, 62, 255),
			passiveSelection		= new Color32(56, 56, 48, 255),
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238, 98, 255),
			trackReverted           = new Color32(246, 201, 60, 255),
			currentLine             = new Color32(62, 61, 49, 255),
			currentLineInactive     = new Color32(50, 50, 41, 255),

			typesStyle              = FontStyle.Italic
		},

		// Son of Obsidian
		new Theme {
			background              = new Color32(0x22, 0x28, 0x2A, 0xFF),
			text                    = new Color32(0xF1, 0xF2, 0xF3, 0xFF),
			comments                = new Color32(0x66, 0x74, 0x7B, 0xFF),
			strings                 = new Color32(0xEC, 0x76, 0x00, 0xFF),
			keywords                = new Color32(0x93, 0xC7, 0x63, 0xFF),
			constants               = new Color32(0x93, 0xC7, 0x63, 0xFF),
			knownTypes              = new Color32(0x67, 0x8C, 0xB1, 0xFF),
			hyperlinks              = new Color32(0x99, 0xDA, 0xF9, 0xFF),
			preprocessor            = new Color32(0xA0, 0x82, 0xBD, 0xFF),
			lineNumbers             = new Color32(0x3F, 0x4E, 0x49, 0xFF),
			lineNumbersHighlight    = new Color32(0x7E, 0x9D, 0x92, 0xFF),
			lineNumbersBackground   = new Color32(0x29, 0x31, 0x34, 0xFF),
			fold                    = new Color32(0x29, 0x31, 0x34, 0xFF),
			searchResults           = new Color32(0x47, 0x47, 0x47, 0xFF),
			//activeSelection			= new Color32(0x30, 0x3A, 0x3B, 0xFF),
			activeSelection			= new Color32(0x96, 0xAD, 0xB2, 0x44),
			passiveSelection		= new Color32(0x17, 0x1B, 0x1C, 0xFF),
			trackSaved              = new Color32(108, 226, 108, 255),
			trackChanged            = new Color32(255, 238, 98, 255),
			trackReverted           = new Color32(246, 201, 60, 255),
			currentLine             = new Color32(0x29, 0x31, 0x34, 0xFF),
			currentLineInactive     = new Color32(0x25, 0x2C, 0x2F, 0xFF),

			//typesStyle              = FontStyle.Italic
		},
	};
	private static Theme currentThemeCode = themes[0];
	private static Theme currentThemeText = themes[0];

	private static string[] availableFonts = {
		"Inconsolata.otf",
		"Monaco 12.ttf",
		"Monaco 13.ttf",
		"SourceCodePro/SourceCodePro-Regular.otf",
		"SourceCodePro/SourceCodePro-Semibold.otf",
		"VeraMono.ttf",
	};
	private static string currentFont = null;
	private static int currentFontSizeDelta = 0;
	private static bool resetCodeFont = true;
	private static bool resetTextFont = true;

	[NonSerialized]
	private float margin = 0f;
	private static bool highlightCurrentLine = true;
	private static bool showLineNumbers = true;
	private static bool trackChangesCode = true;
	private static bool trackChangesText = true;
	[NonSerialized]
	private bool trackChanges = true;

	public class Styles
	{
		public GUIStyle scrollViewStyle;
		public GUIStyle normalStyle;
		public GUIStyle hyperlinkStyle;
		public GUIStyle mailtoStyle;
		public GUIStyle keywordStyle;
		public GUIStyle constantStyle;
		public GUIStyle userTypeStyle;
		public GUIStyle commentStyle;
		public GUIStyle stringStyle;
		public GUIStyle lineNumbersStyle;
		public GUIStyle preprocessorStyle;

		public GUIStyle lineNumbersBackground;
		public GUIStyle lineNumbersSeparator;

		public GUIStyle searchResultStyle;
		public GUIStyle ping;

		public GUIStyle toolbarSearchField;
		public GUIStyle toolbarSearchFieldCancelButton;
		public GUIStyle toolbarSearchFieldCancelButtonEmpty;
		public GUIStyle upArrowStyle;
		public GUIStyle downArrowStyle;

		public GUIStyle caretStyle;
		public GUIStyle activeSelectionStyle;
		public GUIStyle passiveSelectionStyle;

		public GUIStyle trackChangesAfterSaveStyle;
		public GUIStyle trackChangesBeforeSaveStyle;
		public GUIStyle trackChangesRevertedStyle;

		public GUIStyle currentLineStyle;
		public GUIStyle currentLineInactiveStyle;
	}
	public static Styles stylesCode = new Styles();
	public static Styles stylesText = new Styles();
	[SerializeField, HideInInspector]
	public Styles styles = stylesCode;

	[SerializeField, HideInInspector]
	private Vector2 scrollPosition = new Vector2();
	[SerializeField, HideInInspector]
	private int scrollPositionLine = 0;
	[SerializeField, HideInInspector]
	private float scrollPositionOffset = 0f;

//	[NonSerialized]
//	private float contentWidth = 0;

	[NonSerialized]
	private Rect scrollViewRect;
	[NonSerialized]
	private Rect contentRect;
	[NonSerialized]
	private bool needsRepaint = false;
	[NonSerialized]
	private bool hasCodeViewFocus = false;
	[NonSerialized]
	private EditorWindow parentWindow = null;
	private static Vector2 charSize;
	
	[NonSerialized]
	private int[] bufferToEditorLine = null;
	[NonSerialized]
	private int numFormatedBufferLines = 0;

	[SerializeField]
	private bool wordWrapping = true;
	private static bool wordWrappingCode = true;
	private static bool wordWrappingText = true;

	private static GUIContent lineNumberContent = new GUIContent(string.Empty);
	private static int buttonHash = "Button".GetHashCode();
	private static Texture2D wrenchIcon;
	private static Texture2D saveIcon;
	private static Texture2D undoIcon;
	private static Texture2D redoIcon;
	private static Texture2D hyperlinksIcon;
	private static Texture2D popOutIcon;

	[NonSerialized]
	private bool hasSearchBoxFocus;
	[NonSerialized]
	private bool focusSearchBox = false;
	[NonSerialized]
	private bool focusCodeView = true;
	[NonSerialized]
	private bool focusCodeViewOnEscapeUp = false;
	private static string defaultSearchString = string.Empty;
	[NonSerialized]
	private string searchString = defaultSearchString;
	[NonSerialized]
	private List<FGTextBuffer.CaretPos> searchResults = new List<FGTextBuffer.CaretPos>();
	[NonSerialized]
	private int currentSearchResult = -1;
	[NonSerialized]
	private int searchResultAge = 0;
	[NonSerialized]
	private float pingTimer = 0f;
	[NonSerialized]
	private float pingStartTime = 0f;
	[NonSerialized]
	private GUIContent pingContent = new GUIContent();
	[NonSerialized]
	private Rect scrollToRect;
	[NonSerialized]
	public bool scrollToCaret = false;

	// Editor

	[NonSerialized]
	private bool isCaretOn = true;
	[NonSerialized]
	public float caretMoveTime = 0f;
	[SerializeField, HideInInspector]
	public FGTextBuffer.CaretPos caretPosition = new FGTextBuffer.CaretPos();
	[SerializeField, HideInInspector]
	private FGTextBuffer.CaretPos _selectionStartPosition = null;
	[SerializeField, HideInInspector]
	private bool hasSelection = false;

	public FGTextBuffer.CaretPos selectionStartPosition
	{
		get { return hasSelection ? _selectionStartPosition : null; }
		set
		{
			if (value == null)
			{
				_selectionStartPosition = null;
				hasSelection = false;
			}
			else
			{
				_selectionStartPosition = value;
				hasSelection = true;
			}
		}
	}

	[NonSerialized]
	private bool codeViewDragging = false;
	[NonSerialized]
	private bool mouseDownOnSelection = false;
	[NonSerialized]
	private FGTextBuffer.CaretPos mouseDropPosition = new FGTextBuffer.CaretPos();
	[NonSerialized]
	private Vector2 autoScrolling = Vector2.zero;
	[NonSerialized]
	private Vector2 autoScrollDelta = Vector2.zero;
	[NonSerialized]
	private float lastAutoScrollTime = 0f;

	[NonSerialized]
	private bool autoScrollLeft = false;
	[NonSerialized]
	private bool autoScrollRight = false;
	[NonSerialized]
	private bool autoScrollUp = false;
	[NonSerialized]
	private bool autoScrollDown = false;

	[NonSerialized]
	private FGTextBuffer textBuffer = null;
	public FGTextBuffer TextBuffer { get { return textBuffer; } }
	public bool IsModified { get { return textBuffer != null ? textBuffer.IsModified : false; } }
	public bool IsLoading { get { return textBuffer != null ? textBuffer.IsLoading : true; } }
	public bool CanEdit() { return !IsLoading && !EditorApplication.isCompiling && !textBuffer.justSavedNow; }

	public string targetGuid { get { return textBuffer != null ? textBuffer.guid : string.Empty; } }

	[SerializeField]
	private List<float> yLineOffsets;
	public float GetLineOffset(int index)
	{
		if (!wordWrapping /*|| !CanEdit()*/)
			return charSize.y * index;
		if (index <= 0)
			return 0f;

		if (yLineOffsets == null || yLineOffsets.Count != textBuffer.lines.Count)
		{
			yLineOffsets = new List<float>(textBuffer.lines.Count);
			float yOffset = 0f;
			for (int i = 0; i < textBuffer.lines.Count; ++i)
			{
				yOffset += charSize.y * (GetSoftLineBreaks(i).Count + 1);
				yLineOffsets.Add(yOffset);
			}
		}

		//if (index <= 0 || index > yLineOffsets.Count)
		//	Debug.Log(index + "/" + yLineOffsets.Count);
		if (index > yLineOffsets.Count)
			return yLineOffsets.Count > 0 ? yLineOffsets[yLineOffsets.Count - 1] : 0;
		return yLineOffsets[index - 1];
	}

	public int GetLineAt(float yOffset)
	{
		if (!wordWrapping /*|| !CanEdit()*/ || textBuffer.lines.Count <= 1)
			return Mathf.Min((int) (yOffset / charSize.y), textBuffer.lines.Count - 1);

		GetLineOffset(textBuffer.lines.Count);

		int line = FindFirstIndexGreaterThanOrEqualTo<float>(yLineOffsets, yOffset + 1f);
		//if (line >= 0 && line < yLineOffsets.Count && yOffset != yLineOffsets[line])
		//	++line;
		return line;
	}
	
	public void FocusCodeView()
	{
		caretMoveTime = 0f;
		focusCodeView = true;
		Repaint();
	}

	public void OnEnable(UnityEngine.Object targetFile)
	{
		if (selectionStartPosition != null && selectionStartPosition.line == -1)
			selectionStartPosition = null;

		if (string.IsNullOrEmpty(currentFont))
			currentFont = EditorPrefs.GetString("ScriptInspectorFont", availableFonts[3]);
		currentFontSizeDelta = EditorPrefs.GetInt("ScriptInspectorFontSize", 0);

		bool isText = (!(targetFile is MonoScript)) && (!(targetFile is Shader));
		
		int themeIndex = -1;
		if (EditorPrefs.HasKey("ScriptInspectorTheme"))
		{
			string themeName = EditorPrefs.GetString("ScriptInspectorTheme");
			themeIndex = Array.IndexOf(availableThemes, themeName);
		}
		if (isText)
		{
			if (EditorPrefs.HasKey("ScriptInspectorThemeText"))
			{
				string themeName = EditorPrefs.GetString("ScriptInspectorThemeText");
				themeIndex = Array.IndexOf(availableThemes, themeName);
			}
		}
		if (themeIndex == -1)
		{
			if (EditorGUIUtility.isProSkin)
				themeIndex = 2;
			else
				themeIndex = 1;
		}
		if (isText)
			currentThemeText = themes[themeIndex];
		else
			currentThemeCode = themes[themeIndex];

		highlightCurrentLine = EditorPrefs.GetBool("FlipbookGames.ScriptInspector.HighlightCurrentLine", true);
		showLineNumbers = EditorPrefs.GetBool("FlipbookGames.ScriptInspector.LineNumbers", true);
		trackChangesCode = EditorPrefs.GetBool("FlipbookGames.ScriptInspector.TrackChanges", true);
		trackChangesText = EditorPrefs.GetBool("FlipbookGames.ScriptInspector.TrackChangesText", true);
		trackChanges = isText ? trackChangesText : trackChangesCode;
		wordWrappingCode = EditorPrefs.GetBool("FlipbookGames.ScriptInspector.WordWrapCode", false);
		wordWrappingText = EditorPrefs.GetBool("FlipbookGames.ScriptInspector.WordWrapText", false);
		wordWrapping = isText ? wordWrappingText : wordWrappingCode;

		styles = isText ? stylesText : stylesCode;
		Initialize();

		if (textBuffer == null)
		{
			try
			{
				textBuffer = FGTextBuffer.GetBuffer(targetFile);
			}
			catch (Exception e)
			{
				Debug.LogError("Exception while trying to get buffer!!!\n" + e);
				return;
			}
		}
		textBuffer.styles = isText ? stylesText : stylesCode;
		textBuffer.Initialize();

		//caretMoveTime = Time.realtimeSinceStartup;

		EditorApplication.update -= OnUpdate;
		EditorApplication.update += OnUpdate;

		textBuffer.onChange -= Repaint;
		textBuffer.onChange += Repaint;

		textBuffer.onLineFormatted -= OnLineFormatted;
		textBuffer.onLineFormatted += OnLineFormatted;

		textBuffer.onInsertedLines -= OnInsertedLines;
		textBuffer.onInsertedLines += OnInsertedLines;

		textBuffer.onRemovedLines -= OnRemovedLines;
		textBuffer.onRemovedLines += OnRemovedLines;

		textBuffer.AddEditor(this);

		Repaint();
	}

	private void OnLineFormatted(int line)
	{
		if (_softLineBreaks != null && line < _softLineBreaks.Count)
			_softLineBreaks[line] = null;
	}

	private void OnInsertedLines(int lineIndex, int numLines)
	{
		if (_softLineBreaks != null && lineIndex <= _softLineBreaks.Count)
		{
			_softLineBreaks.InsertRange(lineIndex, new List<int>[numLines]);
		}
		if (yLineOffsets != null && lineIndex < yLineOffsets.Count)
		{
			yLineOffsets.RemoveRange(lineIndex, yLineOffsets.Count - lineIndex);
		}
		if (lineIndex < scrollPositionLine)
		{
			scrollPositionOffset = 0f;
			scrollPositionLine += numLines;
		}
	}

	private void OnRemovedLines(int lineIndex, int numLines)
	{
		if (_softLineBreaks != null && lineIndex < _softLineBreaks.Count)
		{
			_softLineBreaks.RemoveRange(lineIndex, Math.Min(numLines, _softLineBreaks.Count - lineIndex));
		}
		if (yLineOffsets != null && lineIndex < yLineOffsets.Count)
		{
			yLineOffsets.RemoveRange(lineIndex, yLineOffsets.Count - lineIndex);
		}
		if (lineIndex < scrollPositionLine)
		{
			scrollPositionOffset = 0f;
			if (lineIndex + numLines <= scrollPositionLine)
				scrollPositionLine -= numLines;
			else
				scrollPositionLine = lineIndex;
		}
	}

	private void SaveBuffer()
	{
		if (CanEdit())
		{
			RepaintAllInstances();
			textBuffer.Save();
			AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(textBuffer.guid));
		}
	}

	public void OnDisable()
	{
		//if (autocompleteWindow != null)
		//	CloseAutocomplete();

		EditorApplication.update -= OnUpdate;

		if (textBuffer != null)
		{
			textBuffer.RemoveEditor(this);
			textBuffer.onChange -= Repaint;
			textBuffer.onLineFormatted -= OnLineFormatted;
			textBuffer.onInsertedLines -= OnInsertedLines;
		}
	}

	public delegate void NotificationDelegate();
	public NotificationDelegate onRepaint;
	//public NotificationDelegate onChange;

	private void Repaint()
	{
		if (onRepaint != null)
			onRepaint();
	}

	//private void NotifyChange()
	//{
	//    if (onChange != null)
	//        onChange();
	//}

	public void OnUpdate()
	{
		float time = Time.realtimeSinceStartup;

		if (autoScrolling != Vector2.zero || autoScrollLeft || autoScrollRight || autoScrollUp || autoScrollDown)
		{
			float deltaTime = time - lastAutoScrollTime;

			if (!autoScrollLeft && !autoScrollRight)
			{
				autoScrolling.x = autoScrolling.x * 0.9f;
				if (autoScrolling.x != 0f)
					autoScrolling.x = autoScrolling.x > 0f ? Mathf.Max(0f, autoScrolling.x - 50f * deltaTime) : Mathf.Min(0f, autoScrolling.x + 50f * deltaTime);
			}
			else
			{
				autoScrolling.x = Mathf.Clamp(autoScrolling.x + (autoScrollLeft ? -500f : 500f) * deltaTime, -2000f, 2000f);
			}
			if (!autoScrollUp && !autoScrollDown)
			{
				autoScrolling.y = autoScrolling.y * 0.9f;
				if (autoScrolling.y != 0f)
					autoScrolling.y = autoScrolling.y > 0f ? Mathf.Max(0f, autoScrolling.y - 50f * deltaTime) : Mathf.Min(0f, autoScrolling.y + 50f * deltaTime);
			}
			else
			{
				autoScrolling.y = Mathf.Clamp(autoScrolling.y + (autoScrollUp ? -500f : 500f) * deltaTime, -2000f, 2000f);
			}

			autoScrollDelta = autoScrolling * deltaTime;
			if (lastMouseEvent != null)
			{
				simulateLastMouseEvent = codeViewDragging && !mouseDownOnSelection;
			}
			lastAutoScrollTime = time;
			if (EditorWindow.focusedWindow)
				EditorWindow.focusedWindow.wantsMouseMove = true;
			Repaint();
		}
		else if (hasCodeViewFocus)
		{
			lastAutoScrollTime = time;

			float caretTime = (time - caretMoveTime) % 1f;
			bool shouldCaretBeVisible = caretTime < 0.5f;
			if (isCaretOn != shouldCaretBeVisible)
			{
				Repaint();
			}
		}
	}

	private static string editorResourcesPath;

	public static T LoadEditorResource<T>(string indieAndProName) where T : UnityEngine.Object
	{
		return LoadEditorResource<T>(indieAndProName, null);
	}
	
	public static T LoadEditorResource<T>(string indieName, string proName) where T : UnityEngine.Object
	{
		if (editorResourcesPath == null)
		{
			MonoScript managerScript = MonoScript.FromScriptableObject(FGTextBufferManager.instance);
			editorResourcesPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(managerScript)));
			editorResourcesPath = System.IO.Path.Combine(editorResourcesPath, "EditorResources");
		}
		
		string fileName = proName == null ? indieName : EditorGUIUtility.isProSkin ? proName : indieName;
		string path = System.IO.Path.Combine(editorResourcesPath, fileName);
		return AssetDatabase.LoadMainAssetAtPath(path) as T;
	}

	// returns 0 for non-dynamic fonts
	private static int GetDynamicFontSize(Font font)
	{
		if (font == null)
			return 0;
		
		TrueTypeFontImporter fontImporter = TrueTypeFontImporter.GetAtPath(AssetDatabase.GetAssetPath(font)) as TrueTypeFontImporter;
		return fontImporter != null && fontImporter.fontTextureCase == FontTextureCase.Dynamic ? fontImporter.fontSize : 0;
	}

	public void Initialize()
	{
		Vector2 cs = styles.normalStyle != null && styles.normalStyle.font != null ? styles.normalStyle.CalcSize(new GUIContent("W")) : charSize;
		bool isText = textBuffer != null && textBuffer.isText;
		bool reset = isText ? resetTextFont : resetCodeFont;

		if (cs != charSize || styles.normalStyle == null || styles.normalStyle.font == null || undoIcon == null
			|| (currentFontSizeDelta == 0) != (styles.normalStyle.fontSize == 0) || reset // || currentFontSizeDelta != appliedFontSizeDelta
			|| styles.normalStyle.font.name != Path.GetFileNameWithoutExtension(currentFont))
		{
			_softLineBreaks = null;
			yLineOffsets = null;

			if (isText)
				resetTextFont = false;
			else
				resetCodeFont = false;

			styles.scrollViewStyle = styles.scrollViewStyle ?? new GUIStyle(GUIStyle.none);
			styles.searchResultStyle = styles.searchResultStyle ?? new GUIStyle(GUIStyle.none);

			styles.normalStyle = styles.normalStyle ?? new GUIStyle(GUIStyle.none);
#if !UNITY_3_5
			styles.normalStyle.richText = false;
#endif
			styles.normalStyle.font = LoadEditorResource<Font>(currentFont); //(Font)Resources.Load(currentFont, typeof(Font));
			for (int i = 0; styles.normalStyle.font == null && i < availableFonts.Length; ++i)
			{
				currentFont = availableFonts[i];
				styles.normalStyle.font = (Font)Resources.Load(currentFont, typeof(Font));
			}

			int currentFontSize = GetDynamicFontSize(styles.normalStyle.font);
			bool isDynamicFont = currentFontSize != 0;
			if (!isDynamicFont && currentFontSizeDelta != 0)
			{
				currentFontSizeDelta = 0;
				EditorPrefs.SetInt("ScriptInspectorFontSize", 0);
			}
			if (currentFontSizeDelta != 0)
				styles.normalStyle.fontSize = currentFontSize + currentFontSizeDelta;
			else
				styles.normalStyle.fontSize = 0;
			
			cs = styles.normalStyle.font != null ? styles.normalStyle.CalcSize(new GUIContent("W")) : charSize;
			charSize = cs;

			styles.hyperlinkStyle = styles.hyperlinkStyle ?? new GUIStyle(styles.normalStyle);
			styles.mailtoStyle = styles.mailtoStyle ?? new GUIStyle(styles.hyperlinkStyle);
			styles.keywordStyle = styles.keywordStyle ?? new GUIStyle(styles.normalStyle);
			styles.constantStyle = styles.constantStyle ?? new GUIStyle(styles.normalStyle);
			styles.userTypeStyle = styles.userTypeStyle ?? new GUIStyle(styles.normalStyle);
			styles.commentStyle = styles.commentStyle ?? new GUIStyle(styles.normalStyle);
			styles.stringStyle = styles.stringStyle ?? new GUIStyle(styles.normalStyle);
			styles.lineNumbersStyle = styles.lineNumbersStyle ?? new GUIStyle(styles.normalStyle);
			styles.preprocessorStyle = styles.preprocessorStyle ?? new GUIStyle(styles.normalStyle);

			styles.hyperlinkStyle.font = styles.normalStyle.font;
			styles.mailtoStyle.font = styles.normalStyle.font;
			styles.keywordStyle.font = styles.normalStyle.font;
			styles.constantStyle.font = styles.normalStyle.font;
			styles.userTypeStyle.font = styles.normalStyle.font;
			styles.commentStyle.font = styles.normalStyle.font;
			styles.stringStyle.font = styles.normalStyle.font;
			styles.lineNumbersStyle.font = styles.normalStyle.font;
			styles.preprocessorStyle.font = styles.normalStyle.font;

			if (isDynamicFont)
			{
				styles.hyperlinkStyle.fontSize = styles.normalStyle.fontSize;
				styles.mailtoStyle.fontSize = styles.normalStyle.fontSize;
				styles.keywordStyle.fontSize = styles.normalStyle.fontSize;
				styles.constantStyle.fontSize = styles.normalStyle.fontSize;
				styles.userTypeStyle.fontSize = styles.normalStyle.fontSize;
				styles.commentStyle.fontSize = styles.normalStyle.fontSize;
				styles.stringStyle.fontSize = styles.normalStyle.fontSize;
				styles.lineNumbersStyle.fontSize = styles.normalStyle.fontSize;
				styles.preprocessorStyle.fontSize = styles.normalStyle.fontSize;
			}
			else
			{
				styles.normalStyle.fontSize = 0;
				styles.hyperlinkStyle.fontSize = 0;
				styles.mailtoStyle.fontSize = 0;
				styles.keywordStyle.fontSize = 0;
				styles.constantStyle.fontSize = 0;
				styles.userTypeStyle.fontSize = 0;
				styles.commentStyle.fontSize = 0;
				styles.stringStyle.fontSize = 0;
				styles.lineNumbersStyle.fontSize = 0;
				styles.preprocessorStyle.fontSize = 0;
			}

			styles.lineNumbersBackground = styles.lineNumbersBackground ?? new GUIStyle();
			styles.lineNumbersSeparator = styles.lineNumbersSeparator ?? new GUIStyle();
			styles.caretStyle = styles.caretStyle ?? new GUIStyle();
			styles.activeSelectionStyle = styles.activeSelectionStyle ?? new GUIStyle();
			styles.passiveSelectionStyle = styles.passiveSelectionStyle ?? new GUIStyle();
			styles.trackChangesAfterSaveStyle = styles.trackChangesAfterSaveStyle ?? new GUIStyle();
			styles.trackChangesBeforeSaveStyle = styles.trackChangesBeforeSaveStyle ?? new GUIStyle();
			styles.trackChangesRevertedStyle = styles.trackChangesRevertedStyle ?? new GUIStyle();
			styles.currentLineStyle = styles.currentLineStyle ?? new GUIStyle();
			styles.currentLineInactiveStyle = styles.currentLineInactiveStyle ?? new GUIStyle();

			styles.upArrowStyle = styles.upArrowStyle ?? new GUIStyle();
			styles.downArrowStyle = styles.downArrowStyle ?? new GUIStyle();
			styles.upArrowStyle.normal.background = LoadEditorResource<Texture2D>("upArrowOff.png", "d_upArrowOff.png");
			styles.upArrowStyle.hover.background = styles.upArrowStyle.active.background
				= LoadEditorResource<Texture2D>("upArrow.png", "d_upArrow.png");
			styles.downArrowStyle.normal.background = LoadEditorResource<Texture2D>("downArrowOff.png", "d_downArrowOff.png");
			styles.downArrowStyle.hover.background = styles.downArrowStyle.active.background
				= LoadEditorResource<Texture2D>("downArrow.png", "d_downArrow.png");

			wrenchIcon = LoadEditorResource<Texture2D>("l_wrench.png", "d_wrench.png");

			saveIcon = LoadEditorResource<Texture2D>("saveIconBW.png");
			undoIcon = LoadEditorResource<Texture2D>("editUndoIconBW.png");
			redoIcon = LoadEditorResource<Texture2D>("editRedoIconBW.png");
			hyperlinksIcon = LoadEditorResource<Texture2D>("hyperlinksIconBW.png");
			popOutIcon = LoadEditorResource<Texture2D>("popOutIconBW.png");

			styles.ping = styles.ping ?? new GUIStyle();
#if !UNITY_3_5
			styles.ping.richText = false;
#endif
			styles.ping.normal.background = LoadEditorResource<Texture2D>("yellowPing.png");
			styles.ping.normal.textColor = Color.black;
			styles.ping.font = styles.normalStyle.font;
			if (isDynamicFont)
				styles.ping.fontSize = styles.normalStyle.fontSize;
			else
				styles.ping.fontSize = 0;
			styles.ping.border = new RectOffset(10, 10, 10, 10);
			styles.ping.overflow = new RectOffset(7, 7, 6, 6);
			styles.ping.stretchWidth = false;
			styles.ping.stretchHeight = false;

			ApplyTheme(styles, isText ? currentThemeText : currentThemeCode);
		}
	}

	private static void ApplyTheme(Styles styles, Theme currentTheme)
	{
		styles.scrollViewStyle.normal.background = FlatColorTexture(currentTheme.background);
		styles.searchResultStyle.normal.background = FlatColorTexture(currentTheme.searchResults);
		styles.caretStyle.normal.background = FlatColorTexture(currentTheme.text);
		styles.activeSelectionStyle.normal.background = FlatColorTexture(currentTheme.activeSelection);
		styles.passiveSelectionStyle.normal.background = FlatColorTexture(currentTheme.passiveSelection);
		styles.trackChangesBeforeSaveStyle.normal.background = FlatColorTexture(currentTheme.trackChanged);
		styles.trackChangesAfterSaveStyle.normal.background = FlatColorTexture(currentTheme.trackSaved);
		styles.trackChangesRevertedStyle.normal.background = FlatColorTexture(currentTheme.trackReverted);
		styles.currentLineStyle.normal.background = FlatColorTexture(currentTheme.currentLine);
		styles.currentLineInactiveStyle.normal.background = FlatColorTexture(currentTheme.currentLineInactive);
		
		styles.normalStyle.normal.textColor = currentTheme.text;
		styles.keywordStyle.normal.textColor = currentTheme.keywords;
		styles.constantStyle.normal.textColor = currentTheme.constants;
		styles.userTypeStyle.normal.textColor = currentTheme.knownTypes;
		styles.commentStyle.normal.textColor = currentTheme.comments;
		styles.stringStyle.normal.textColor = currentTheme.strings;
		styles.preprocessorStyle.normal.textColor = currentTheme.preprocessor;

		styles.hyperlinkStyle.normal.textColor = currentTheme.hyperlinks;
		styles.mailtoStyle.normal.textColor = currentTheme.hyperlinks;
		styles.hyperlinkStyle.normal.background = styles.mailtoStyle.normal.background =
			UnderlineTexture(currentTheme.hyperlinks, (int)styles.mailtoStyle.lineHeight);

		styles.lineNumbersBackground.normal.background = FlatColorTexture(currentTheme.lineNumbersBackground);
		styles.lineNumbersSeparator.normal.background = FlatColorTexture(currentTheme.fold);

		styles.lineNumbersStyle.normal.textColor = currentTheme.lineNumbers;
		styles.lineNumbersStyle.hover.textColor = currentTheme.lineNumbersHighlight;
		styles.lineNumbersStyle.hover.background = styles.lineNumbersBackground.normal.background;

		bool isDynamic = GetDynamicFontSize(styles.normalStyle.font) != 0;
		styles.keywordStyle.fontStyle = isDynamic ? currentTheme.keywordsStyle : 0;
		styles.constantStyle.fontStyle = isDynamic ? currentTheme.constantsStyle : 0;
		styles.userTypeStyle.fontStyle = isDynamic ? currentTheme.typesStyle : 0;
		styles.commentStyle.fontStyle = isDynamic ? currentTheme.commentsStyle : 0;
		styles.stringStyle.fontStyle = isDynamic ? currentTheme.stringsStyle : 0;
		styles.preprocessorStyle.fontStyle = isDynamic ? currentTheme.preprocessorStyle : 0;
		styles.hyperlinkStyle.fontStyle = isDynamic ? currentTheme.hyperlinksStyle : 0;
		styles.mailtoStyle.fontStyle = isDynamic ? currentTheme.hyperlinksStyle : 0;
	}

	private static Texture2D FlatColorTexture(Color color)
	{
		Texture2D flat = new Texture2D(1, 1, TextureFormat.RGBA32, false);
		flat.SetPixels(new Color[] { color });
		flat.Apply();
		flat.hideFlags = HideFlags.HideAndDontSave;
		return flat;
	}

	private static Texture2D UnderlineTexture(Color color, int lineHeight)
	{
		return CreateUnderlineTexture(color, lineHeight, Color.clear);
	}

	private static Texture2D CreateUnderlineTexture(Color color, int lineHeight, Color bgColor)
	{
		Texture2D underlined = new Texture2D(1, lineHeight, TextureFormat.RGBA32, false);
		underlined.SetPixel(0, 0, color);
		for (int i = 1; i < lineHeight; ++i)
			underlined.SetPixel(0, i, new Color32(0, 0, 0, 0));
		underlined.Apply();
		underlined.hideFlags = HideFlags.HideAndDontSave;
		return underlined;
	}

	public static EditorWindow GetFocusedInspector()
	{
		EditorWindow wnd = EditorWindow.focusedWindow;
		if (wnd == null)
			return null;

		Type wndType = wnd.GetType();
		if (wndType.Name == "InspectorWindow")
		{
			FieldInfo fi = wndType.GetField("s_CurrentInspectorWindow", BindingFlags.Public | BindingFlags.Static);
			if (fi != null)
			{
				EditorWindow currentInspector = fi.GetValue(null) as EditorWindow;
				if (currentInspector == wnd)
					return currentInspector;
			}
		}
		
		return null;
	}

	private bool helpButtonClicked = false;

	public void OnWindowGUI(EditorWindow window, RectOffset margins)
	{
		parentWindow = window;
		if (EditorWindow.focusedWindow == window)
			FGTextBuffer.activeEditor = this;

		if (Event.current.type != EventType.layout)
		{
			scrollViewRect = GUILayoutUtility.GetRect(1f, Screen.width, 1f, Screen.height);
			if (!(window is FGCodeWindow))
			{
				scrollViewRect.xMin = 0;
			//	scrollViewRect.xMax = Screen.width - 1;
			}
			scrollViewRect = margins.Remove(scrollViewRect);
		}
		else
		{
			GUILayoutUtility.GetRect(1f, Screen.width, 112f, Screen.height);
		}

		bool enabled = GUI.enabled;
		GUI.enabled = CanEdit();

		if (textBuffer != null)
		{
			Rect rc = new Rect(scrollViewRect.xMax - 21f, scrollViewRect.yMin - 17f, 18f, 16f);
			if (GUI.Button(rc, GUIContent.none, EditorStyles.toolbarButton))
			{
				GenericMenu codeViewPopupMenu = new GenericMenu();
				bool handleDblClick = EditorPrefs.GetBool("ScriptInspector.HandleOpenFromProject", false);
				bool handleTextDblClick = EditorPrefs.GetBool("ScriptInspector.HandleOpenTextFromProject", false);
				bool handleShaderDblClick = EditorPrefs.GetBool("ScriptInspector.HandleOpenShaderFromProject", false);
				
				codeViewPopupMenu.AddItem(new GUIContent("Open on Double-Click/Scripts"), handleDblClick, ToggleHandleOpenFromProject);
				codeViewPopupMenu.AddItem(new GUIContent("Open on Double-Click/Shaders"), handleShaderDblClick, ToggleHandleOpenShadersFromProject);
				codeViewPopupMenu.AddItem(new GUIContent("Open on Double-Click/Text Assets"), handleTextDblClick, ToggleHandleOpenTextsFromProject);
	
				codeViewPopupMenu.AddSeparator(string.Empty);
				if (textBuffer.isText)
				{
					codeViewPopupMenu.AddItem(new GUIContent("View Options/Word Wrap (Text)"), wordWrappingText, ToggleWordWrapText);
					codeViewPopupMenu.AddItem(new GUIContent("View Options/Highlight Current Line"), highlightCurrentLine, ToggleHighlightCurrentLine);
					codeViewPopupMenu.AddItem(new GUIContent("View Options/Line Numbers"), showLineNumbers, ToggleLineNumbers);
					codeViewPopupMenu.AddItem(new GUIContent("View Options/Track Changes (Text)"), trackChanges, ToggleTrackChangesText);
				}
				else
				{
					codeViewPopupMenu.AddItem(new GUIContent("View Options/Word Wrap (Code)"), wordWrappingCode, ToggleWordWrapCode);
					codeViewPopupMenu.AddItem(new GUIContent("View Options/Highlight Current Line"), highlightCurrentLine, ToggleHighlightCurrentLine);
					codeViewPopupMenu.AddItem(new GUIContent("View Options/Line Numbers"), showLineNumbers, ToggleLineNumbers);
					codeViewPopupMenu.AddItem(new GUIContent("View Options/Track Changes (Code)"), trackChanges, ToggleTrackChangesCode);
				}
	
				//codeViewPopupMenu.AddSeparator(string.Empty);
				for (int i = 0; i < availableFonts.Length; ++i)
					codeViewPopupMenu.AddItem(new GUIContent("Font/" + Path.GetFileNameWithoutExtension(availableFonts[i])), currentFont == availableFonts[i], (System.Object x) => SelectFont((int)x), i);
	
				string[] sortedThemes = availableThemes.Clone() as string[];
				Array.Sort<string>(sortedThemes, StringComparer.OrdinalIgnoreCase);
				for (int i = 0; i < sortedThemes.Length; ++i)
				{
					int themeIndex = Array.IndexOf<string>(availableThemes, sortedThemes[i]);
					if (textBuffer.isText)
						codeViewPopupMenu.AddItem(new GUIContent("Color Scheme (Text)/" + sortedThemes[i]), currentThemeText == themes[themeIndex], (System.Object x) => SelectTheme((int)x, textBuffer.isText), themeIndex);
					else
						codeViewPopupMenu.AddItem(new GUIContent("Color Scheme (Code)/" + sortedThemes[i]), currentThemeCode == themes[themeIndex], (System.Object x) => SelectTheme((int)x, textBuffer.isText), themeIndex);
				}
				
				codeViewPopupMenu.AddSeparator(string.Empty);
				codeViewPopupMenu.AddItem(new GUIContent("About"), false, About);
	
				codeViewPopupMenu.DropDown(rc);
				GUIUtility.ExitGUI();
			}
		}

		Color oldColor = GUI.color;
		if (!GUI.enabled && Event.current.type == EventType.Repaint)
		{
			GUI.color = new Color(0.85f, 0.85f, 0.85f);
			if (textBuffer != null)
				textBuffer.LoadFaster();
		}

		try
		{
			if (hasCodeViewFocus && GUI.enabled)
			{
				textBuffer.BeginEdit("change");
				try
				{
					DoGUIWithAutocomplete(enabled);
				}
				finally
				{
					textBuffer.EndEdit();
				}
			}
			else
			{
				DoGUIWithAutocomplete(enabled);
			}
		}
		finally
		{
			GUI.color = oldColor;
			GUI.enabled = enabled;
		}
	}
	
	public void OnInspectorGUI(bool isScriptInspector, RectOffset margins)
	{
		if (!isScriptInspector)
		{
			OnWindowGUI(GetFocusedInspector(), margins);
			return;
		}
		
		// Disabling the functionality of the default inspector's header help button
		// (located just below the cancel search button) by zeroing hotControl on
		// mouse down, which effectivly deactivates the button so it doesn't fire up
		// on mouse up. Detection is done by comparing hotControl with the next available
		// controlID - 2, which is super-hacky, but so far I haven't found any nicer way
		// of doing this.
		int nextControlID = GUIUtility.GetControlID(buttonHash, FocusType.Native, new Rect());
		if (GUIUtility.hotControl != 0)
		{
			//Debug.Log("hotControl: " + GUIUtility.hotControl + "  nextControlID: " + nextControlID + "  Event: " + Event.current);
			if (GUIUtility.hotControl != 0 && GUIUtility.hotControl == nextControlID - 2)
			{
				GUIUtility.hotControl = 0;
				helpButtonClicked = true;
				Repaint();
			}
		}

		if (Event.current.type != EventType.layout)
		{
			scrollViewRect = GUILayoutUtility.GetRect(1f, Screen.width, 1f, Screen.height);
			scrollViewRect.xMin = 0f;
			scrollViewRect.xMax = Screen.width - 1f;
			scrollViewRect.yMin -= 32f;
			scrollViewRect.yMax += 13f;
		}
		else
		{
			GUILayoutUtility.GetRect(1f, Screen.width, 1f, Screen.height);
		}

		bool enabled = GUI.enabled;
		GUI.enabled = true;

		if (hasCodeViewFocus)
		{
			FGTextBuffer.activeEditor = this;

			textBuffer.BeginEdit("change");
			try
			{
				DoGUIWithAutocomplete(enabled);
			}
			finally
			{
				textBuffer.EndEdit();
			}
		}
		else
		{
			DoGUIWithAutocomplete(enabled);
		}

		GUI.enabled = enabled;
	}

	private Rect codeViewRect = new Rect();
	private int codeViewControlID = 0;

	private Event lastMouseEvent = null;
	private bool simulateLastMouseEvent = false;

	private void DoGUIWithAutocomplete(bool enableGUI)
	{
		if (caretMoveTime == 0f && parentWindow != null)
		{
			caretMoveTime = Time.realtimeSinceStartup;
//			if (EditorWindow.focusedWindow == parentWindow)
//				parentWindow.Focus();
		}

//		if (autocompleteWindow == null)
		{
			DoGUI(enableGUI);
			return;
		}

//		FGTextBuffer.CaretPos caretPosBefore = caretPosition.Clone();
//		try
//		{
//			DoGUI(enableGUI);
//		}
//		catch { }
//		if (autocompleteWindow != null)
//		{
//			if (!hasCodeViewFocus)
//			{
//				CloseAutocomplete();
//				return;
//			}
//
//			if (caretPosition.line == caretPosBefore.line && caretPosition != caretPosBefore)
//			{
//				string line = textBuffer.lines[caretPosition.line];
//				int charIndex = caretPosition.characterIndex;
//				while (charIndex > 0 && char.IsLetterOrDigit(line, charIndex - 1))
//					--charIndex;
//				string wordAtLeft = line.Substring(charIndex, caretPosition.characterIndex - charIndex);
//				autocompleteWindow.SetCurrentWord(wordAtLeft);
//			}
//		}
	}

	[SerializeField]
	private List<List<int>> _softLineBreaks;
	private static readonly List<int> NO_SOFT_LINE_BREAKS = new List<int>();
	
	private List<int> GetSoftLineBreaks(int line)
	{
		if (!wordWrapping /*|| IsLoading*/)
			return NO_SOFT_LINE_BREAKS;

		if (_softLineBreaks == null)
			_softLineBreaks = new List<List<int>>(textBuffer.lines.Count);
		if (line < _softLineBreaks.Count && _softLineBreaks[line] != null)
			return _softLineBreaks[line];

		if (line >= _softLineBreaks.Count)
		{
			if (_softLineBreaks.Capacity < textBuffer.lines.Count)
				_softLineBreaks.Capacity = textBuffer.lines.Count;
			for (int i = _softLineBreaks.Count; i < textBuffer.lines.Count; ++i)
				_softLineBreaks.Add(null);
		}

		if (charSize.x == 0 || charSize.x * 2f > codeViewRect.width)
			return NO_SOFT_LINE_BREAKS; // _softLineBreaks[line];
		
		FGTextBuffer.FormatedLine formatedLine = textBuffer.formatedLines[line];
		if (formatedLine.textBlocks == null)
			return NO_SOFT_LINE_BREAKS; // _softLineBreaks[line];

		var lineBreaks = _softLineBreaks[line] = NO_SOFT_LINE_BREAKS;
		int maxChars = (int) (codeViewRect.width / charSize.x);
		int rowLength = 0;
		
		int lineLength = 0;
		foreach (FGTextBuffer.TextBlock textBlock in formatedLine.textBlocks)
		{
			int remaining = textBlock.text.Length;
			while (remaining > 0)
			{
				int exceed = rowLength + remaining - maxChars;
				if (exceed <= 0)
				{
					rowLength += remaining;
					lineLength += remaining;
					break;
				}

				int startAt = textBlock.text.Length - remaining;
				int i = textBlock.text.Length - exceed;
				i = i > 0 ? textBlock.text.LastIndexOf(' ', i - 1, i - startAt) : -1;
				i -= startAt;
				++i;
				if (i <= 0)
				{
					if (rowLength == 0)
						i = maxChars;
					else
						i = 0;
				}
				lineLength += i;
				remaining -= i;
				if (rowLength + i > 0)
				{
					if (lineBreaks == NO_SOFT_LINE_BREAKS)
						lineBreaks = _softLineBreaks[line] = new List<int>();
					lineBreaks.Add(lineLength);
				}
				rowLength = 0;
			}
		}

		if (yLineOffsets != null && yLineOffsets.Count > line)
		{
			float y = line > 0 ? yLineOffsets[line - 1] : 0f;
			float next = y + charSize.y * (lineBreaks.Count + 1);
			if (next != yLineOffsets[line])
			{
				y = next - yLineOffsets[line];
				for (int i = line; i < yLineOffsets.Count; ++i)
					yLineOffsets[i] += y;
			}
		}

		return lineBreaks;
	}
	
	private int BufferToEditorLine(int bufferLine)
	{
		if (!wordWrapping)
			return bufferLine;
		if (bufferToEditorLine != null && bufferToEditorLine.Length == numFormatedBufferLines && bufferLine < numFormatedBufferLines)
			return bufferToEditorLine[bufferLine];
		return bufferLine;
	}
	
	private FGTextBuffer.CaretPos EditorToBufferPos(int column, int line)
	{
		var caretPos = new FGTextBuffer.CaretPos { column = column, virtualColumn = column, line = line };
		if (!wordWrapping)
		{
			caretPos.characterIndex = textBuffer.ColumnToCharIndex(ref caretPos.column, line);
		}
		return caretPos;
	}

	private static EditorWindow lastFocusedWindow = null;

	public void DoGUI(bool enableGUI)
	{
		if (textBuffer == null)
			return;

		if (AssetDatabase.GUIDToAssetPath(textBuffer.guid).EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
			return;
		
		//if (!(textBuffer.isCsFile || textBuffer.isJsFile || textBuffer.isBooFile))
		//{
		//	if (parentWindow != null)
		//		parentWindow.Close();
		//	return;
		//}
		
		textBuffer.styles = styles = textBuffer.isText ? stylesText : stylesCode;
		wordWrapping = textBuffer.isText ? wordWrappingText : wordWrappingCode;
		trackChanges = textBuffer.isText ? trackChangesText : trackChangesCode;
		
		Initialize();

		if (Event.current.rawType == EventType.MouseMove && mouseDownOnSelection && codeViewDragging)
		{
			mouseDownOnSelection = false;
			codeViewDragging = false;
			lineSelectMode = false;
		}

		bool windowFocusChanged = lastFocusedWindow != EditorWindow.focusedWindow;
		lastFocusedWindow = EditorWindow.focusedWindow;
		if (windowFocusChanged)
		{
			if (parentWindow != null && parentWindow == lastFocusedWindow || GetFocusedInspector() == lastFocusedWindow)
				focusCodeView = true;
		}

		if (hasCodeViewFocus)
		{
			//if (autocompleteWindow != null)
			//{
			//	string committedWord = autocompleteWindow.OnOwnerGUI();
			//	if (committedWord != null)
			//	{
			//		FGTextBuffer.CaretPos insertAt = caretPosition.Clone();
			//		CloseAutocomplete();
			//		if (Event.current.type != EventType.Used)
			//		{
			//			try {
			//				DoGUI(enableGUI);
			//			} catch {}
			//		}

			//		textBuffer.EndEdit();
			//		textBuffer.BeginEdit("Auto Completion '" + committedWord + "'");
					
			//		if (caretPosition != insertAt)
			//		{
			//			string insertedChar = textBuffer.GetTextRange(insertAt, caretPosition);
			//			if (insertedChar != "\n" && insertedChar != "\t")
			//			{
			//				committedWord += insertedChar;
			//			}
			//			textBuffer.DeleteText(insertAt, caretPosition);
			//		}
			//		caretPosition = textBuffer.InsertText(insertAt, committedWord);
			//		textBuffer.UpdateHighlighting(insertAt.line, insertAt.line);

			//		caretMoveTime = Time.realtimeSinceStartup;
			//		scrollToCaret = true;
			//		Repaint();
			//	}
			//	if (Event.current.type == EventType.Used)
			//		return;
			//}

			if (ProcessCodeViewCommands())
				return;
				
			bool isOSX = Application.platform == RuntimePlatform.OSXEditor;
			bool contextClick = Event.current.type == EventType.ContextClick
				|| isOSX && Event.current.type == EventType.MouseUp && Event.current.button == 1;
			if (contextClick && scrollViewRect.Contains(Event.current.mousePosition))
			{
				Event.current.Use();
				
				GenericMenu codeViewPopupMenu = new GenericMenu();
				if (selectionStartPosition != null)
				{
					codeViewPopupMenu.AddItem(new GUIContent("Copy %c"), false, () => EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Copy")));
					codeViewPopupMenu.AddItem(new GUIContent("Cut %x"), false, () => EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Cut")));
				}
				else
				{
					codeViewPopupMenu.AddItem(new GUIContent("Copy %c"), false, null);
					codeViewPopupMenu.AddItem(new GUIContent("Cut %x"), false, null);
				}
				if (string.IsNullOrEmpty(EditorGUIUtility.systemCopyBuffer))
					codeViewPopupMenu.AddItem(new GUIContent("Paste %v"), false, null);
				else
					codeViewPopupMenu.AddItem(new GUIContent("Paste %v"), false, () => EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Paste")));
				codeViewPopupMenu.AddSeparator(string.Empty);

				codeViewPopupMenu.AddItem(new GUIContent("Select All %a"), false, () => EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("SelectAll")));
				codeViewPopupMenu.AddSeparator(string.Empty);
				if (!textBuffer.isText)
					codeViewPopupMenu.AddItem(new GUIContent("Toggle Comment Selection %k"), false, ToggleCommentSelection);
				codeViewPopupMenu.AddItem(new GUIContent("Increase Line Indent %]"), false, IndentMore);
				codeViewPopupMenu.AddItem(new GUIContent("Decrease Line Indent %["), false, IndentLess);
				codeViewPopupMenu.AddItem(new GUIContent("Open at Line " + (caretPosition.line + 1) + (isOSX ? "... %\n" : "... %Enter")), false,
					() => EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("OpenAtCursor")));

				codeViewPopupMenu.ShowAsContext();
				return;
			}

			if (Event.current.isKey)
			{
				ProcessEditorKeyboard(Event.current);
				if (Event.current == null || Event.current.type == EventType.used)
				{
					GUIUtility.ExitGUI();
					return;
				}
			}
		}

		if (Event.current.type == EventType.scrollWheel)
			needsRepaint = true;

		float contentWidth = charSize.x * textBuffer.longestLine;
		contentRect.Set(-4, -4, contentWidth + 8f, 8f + charSize.y * textBuffer.formatedLines.Length);
		
		DoToolbar();

		margin = 0f;
		float lineNumbersWidth = 0f;
		int lineNumbersMaxLength = 0;

		if (Event.current.type != EventType.layout)
		{
			if (showLineNumbers)
			{
				lineNumbersMaxLength = textBuffer.formatedLines.Length.ToString().Length;
				lineNumbersWidth = charSize.x * lineNumbersMaxLength;
				margin = lineNumbersWidth;
			}
			if (trackChanges)
			{
				margin += showLineNumbers ? 7f : 3f;
			}
			if (showLineNumbers || trackChanges)
			{
				margin += 9f;
			}
		}
		
		scrollPosition.y = GetLineOffset(scrollPositionLine) + scrollPositionOffset;

		int fromLine = wordWrapping ? GetLineAt(scrollPosition.y) : Math.Max(0, ((int)(scrollPosition.y / styles.normalStyle.lineHeight)) - 1);
		int toLine = wordWrapping ?
			1 + GetLineAt(scrollPosition.y + scrollViewRect.height) :
			scrollViewRect.height > 0f ?
				fromLine + 2 + (int)(scrollViewRect.height / charSize.y) :
				(int)(Screen.height / styles.normalStyle.lineHeight);

		if (toLine > textBuffer.formatedLines.Length)
			toLine = textBuffer.formatedLines.Length;

		List<int> softLineBreaks = null;

		////List<float> visibleLinePositions = null;
		//if (wordWrapping && !IsLoading)
		//{
		//    float vOffset = charSize.y * fromLine;
		//    //visibleLinePositions = new List<float> { vOffset };
		//    for (int i = fromLine; i < toLine; ++i)
		//    {
		//        int numSoftLineBreaks = GetSoftLineBreaks(i).Count;
		//        vOffset += charSize.y * (1 + numSoftLineBreaks);
		//        //visibleLinePositions.Add(vOffset);
		//        toLine -= numSoftLineBreaks;
		//    }
		//}

		if (scrollToCaret && Event.current.type != EventType.layout)
		{
			scrollToCaret = false;
			FGTextBuffer.CaretPos caretPos = codeViewDragging && mouseDownOnSelection ? mouseDropPosition : caretPosition;

			if (showLineNumbers || trackChanges)
				contentRect.xMax += margin;

			codeViewRect.x = scrollPosition.x + margin;
			codeViewRect.y = scrollPosition.y;
			codeViewRect.width = scrollViewRect.width - margin - 4f;
			codeViewRect.height = scrollViewRect.height - 4f;

			bool hasHorizontalSB = !wordWrapping && contentRect.width - 4f - margin > codeViewRect.width;

			float yOffset;
			if (wordWrapping /*&& !IsLoading*/)
			{
				int row, column;
				BufferToViewPosition(caretPosition, out row, out column);
				yOffset = charSize.y * row + GetLineOffset(caretPos.line);// visibleLinePositions[caretPos.line - fromLine];
			}
			else
			{
				yOffset = charSize.y * caretPos.line;
			}

			if (yOffset < scrollPosition.y)
			{
				scrollPosition.y = yOffset;
				needsRepaint = true;
				scrollToCaret = true;
			}
			else if (yOffset + charSize.y > scrollPosition.y + scrollViewRect.height - (hasHorizontalSB ? 23f : 8f))
			{
				scrollPosition.y = Mathf.Max(0f, yOffset + charSize.y - scrollViewRect.height + (hasHorizontalSB ? 23f : 8f));
				needsRepaint = true;
				scrollToCaret = true;
			}

			if (!wordWrapping)
			{
				if (caretPos.column * charSize.x < scrollPosition.x)
				{
					scrollPosition.x = Mathf.Max(0, (caretPos.column - 20) * charSize.x);
					needsRepaint = true;
					scrollToCaret = true;
				}
				else if (((caretPos.column + 1) * charSize.x) > (scrollPosition.x + scrollViewRect.width - margin - 22f))
				{
					scrollPosition.x = Mathf.Max(0f, (caretPos.column + 21) * charSize.x - scrollViewRect.width + margin + 22f);
					needsRepaint = true;
					scrollToCaret = true;
				}
			}
		}

		if (pingTimer == 1f && scrollViewRect.height > 1f && Event.current.type != EventType.repaint)
		{
			if (scrollToRect.yMin < scrollPosition.y + 30f ||
				scrollToRect.yMax > scrollPosition.y + scrollViewRect.height - 50f)
			{
				scrollPosition.y = Mathf.Max(0f, scrollToRect.center.y - scrollViewRect.height * 0.5f);
				needsRepaint = true;
			}

			if (scrollToRect.xMin < scrollPosition.x + 30f ||
				scrollToRect.xMax > scrollPosition.x + scrollViewRect.width - 30f - margin)
			{
				scrollPosition.x = Mathf.Max(0f, scrollToRect.center.x - scrollViewRect.width * 0.5f);
				needsRepaint = true;
			}

			pingStartTime = Time.realtimeSinceStartup;
		}

		if (Event.current.type == EventType.repaint)
		{
			if (needsRepaint)
			{
				needsRepaint = false;
				Repaint();
			}
		}

		if (Event.current.type == EventType.layout)
		{
			if (CanEdit())
			{
				scrollPositionLine = GetLineAt(scrollPosition.y);
				scrollPositionOffset = scrollPosition.y - GetLineOffset(scrollPositionLine);
			}
			return;
		}

		if (showLineNumbers || trackChanges)
			contentRect.xMax += margin;

		// Filling the background
		GUI.Box(scrollViewRect, GUIContent.none, styles.scrollViewStyle);

		if (lastMouseEvent != null && autoScrollDelta != Vector2.zero)
		{
			lastMouseEvent.mousePosition -= scrollPosition;
			scrollPosition += autoScrollDelta;
		}

		float contentHeight = wordWrapping ? GetLineOffset(textBuffer.lines.Count) + 8f : contentRect.height;
		Vector2 newScrollPosition = GUI.BeginScrollView(scrollViewRect, scrollPosition, wordWrapping ? new Rect(contentRect.x, contentRect.y, 1, contentHeight) : contentRect);
		if (CanEdit())
		{
			scrollPosition = newScrollPosition;
			scrollPositionLine = GetLineAt(scrollPosition.y);
			scrollPositionOffset = scrollPosition.y - GetLineOffset(scrollPositionLine);
		}
		if (textBuffer.lines.Count == 0)
		{
			GUI.EndScrollView();
			return;
		}

		// Hack: Workaround for not receiving mouseUp event if dragged outside of the scrollview's clipped content.
		// Note: mousePosition here will be incorrect, don't use it!
		if (Event.current.rawType == EventType.mouseUp && GUIUtility.hotControl == codeViewControlID)
		{
			//Event.current.mousePosition = Event.current.mousePosition - new Vector2(scrollViewRect.x, scrollViewRect.y);
			ProcessEditorMouse(margin - 2f, Event.current);
			return;
		}

		if (lastMouseEvent != null && autoScrollDelta != Vector2.zero)
		{
			lastMouseEvent.mousePosition += scrollPosition;
			autoScrollDelta = Vector2.zero;
		}

		codeViewControlID = GUIUtility.GetControlID(Math.Abs(GetHashCode()), FocusType.Keyboard);
		if (focusCodeView && codeViewControlID > 0)
		{
			focusCodeView = false;
			caretMoveTime = Time.realtimeSinceStartup;
			GUIUtility.keyboardControl = codeViewControlID;
			Repaint();
		}
		hasCodeViewFocus = codeViewControlID > 0 ? GUIUtility.keyboardControl == codeViewControlID : false;
		if (hasCodeViewFocus && Event.current.rawType != EventType.mouseUp)
		{
			EditorWindow wnd = EditorWindow.focusedWindow;
			if (wnd == null)
				hasCodeViewFocus = false;
			else if (parentWindow != null)
				hasCodeViewFocus = wnd == parentWindow;
			else
				hasCodeViewFocus = wnd == GetFocusedInspector();
		}

		Rect lastCodeViewRect = codeViewRect;
		if (Event.current.type != EventType.layout)
		{
			codeViewRect.x = scrollPosition.x + margin;
			codeViewRect.y = scrollPosition.y;
			codeViewRect.width = scrollViewRect.width - margin - 4f;
			codeViewRect.height = scrollViewRect.height - 4f;

			bool hasHorizontalSB = !wordWrapping && contentRect.width - 4f - margin > codeViewRect.width;
			bool hasVerticalSB = (wordWrapping ? contentHeight : contentRect.height) - 4f > codeViewRect.height;
			if (hasHorizontalSB && hasVerticalSB)
			{
				codeViewRect.width -= 15f;
				codeViewRect.height -= 15f;
			}
			else if (hasHorizontalSB)
			{
				codeViewRect.height -= 15f;
				hasVerticalSB = contentRect.height - 4f > codeViewRect.height;
				if (hasVerticalSB)
					codeViewRect.width -= 15f;
			}
			else if (hasVerticalSB)
			{
				codeViewRect.width -= 15f;
				hasHorizontalSB = contentRect.width - 4f - margin > codeViewRect.width;
				if (hasHorizontalSB)
					codeViewRect.height -= 15f;
			}

			codeViewRect.xMin = Mathf.Ceil((codeViewRect.x - 1f - margin) / charSize.x) * charSize.x + 0f + margin;
			codeViewRect.width = Mathf.Floor(codeViewRect.width / charSize.x) * charSize.x;
			codeViewRect.yMin = Mathf.Ceil(codeViewRect.y / charSize.y) * charSize.y;
			codeViewRect.height = Mathf.Floor(codeViewRect.height / charSize.y) * charSize.y;

			// Uncomment for debugging only
			//GUI.Box(codeViewRect, GUIContent.none);
		}

		//if (needsRepaint)
		//{
		//	GUI.EndScrollView();
		//	return;
		//}

		if (Event.current.type == EventType.repaint /*&& !IsLoading*/)
		{
			Rect rc = new Rect();

			if (wordWrapping && lastCodeViewRect.width != codeViewRect.width)
			{
				_softLineBreaks = null;
				yLineOffsets = null;
			}

			// Current line highlighting
			if (highlightCurrentLine && !hasSelection && Event.current.type == EventType.repaint && caretPosition.line >= fromLine && caretPosition.line < toLine /*&& CanEdit()*/)
			{
				int row;
				int column;
				BufferToViewPosition(caretPosition, out row, out column);
				float yOffset = charSize.y * row + GetLineOffset(caretPosition.line);// visibleLinePositions[caretPosition.line - fromLine];
				Rect currentLineRect = new Rect(margin - 4f + scrollPosition.x, yOffset, codeViewRect.width + charSize.x + 4f, 1f);
				GUI.Box(currentLineRect, GUIContent.none, hasCodeViewFocus ? styles.currentLineStyle : styles.currentLineInactiveStyle);

				currentLineRect.y += charSize.y - 1f;
				GUI.Box(currentLineRect, GUIContent.none, hasCodeViewFocus ? styles.currentLineStyle : styles.currentLineInactiveStyle);

				currentLineRect.y -= charSize.y - 2f;
				currentLineRect.height = charSize.y - 2f;
				Color oldColor = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, 0.5f);
				GUI.Box(currentLineRect, GUIContent.none, hasCodeViewFocus ? styles.currentLineStyle : styles.currentLineInactiveStyle);
				GUI.color = oldColor;
			}

			// Highlighting search results
			if (textBuffer.undoPosition == searchResultAge)
			{
				if (!wordWrapping)
				{
					int searchStringLength = searchString.Length;
					rc.height = charSize.y;
					foreach (FGTextBuffer.CaretPos highlightRect in searchResults)
					{
						if (highlightRect.line >= fromLine && highlightRect.line < toLine)
						{
							rc.y = charSize.y * highlightRect.line;
							rc.x = margin + charSize.x * highlightRect.column;
							int columnTo = textBuffer.CharIndexToColumn(highlightRect.characterIndex + searchStringLength, highlightRect.line);
							rc.width = charSize.x * (columnTo - highlightRect.column);
							GUI.Label(rc, GUIContent.none, styles.searchResultStyle);
						}
					}
				}
			}

			// Drawing selection
			if (!IsLoading && hasSelection)
			{
				if (selectionStartPosition.line == caretPosition.line)
				{
					// Single line selection
					DrawSelectionRect(caretPosition.line, Math.Min(caretPosition.column, selectionStartPosition.column),
						Math.Abs(caretPosition.column - selectionStartPosition.column), margin);
				}
				else
				{
					int firstLine = Math.Min(caretPosition.line, selectionStartPosition.line);
					int lastLine = Math.Max(caretPosition.line, selectionStartPosition.line);

					if (caretPosition.line < selectionStartPosition.line)
					{
						DrawSelectionRect(caretPosition.line, caretPosition.column, FGTextBuffer.ExpandTabs(textBuffer.lines[caretPosition.line]).Length - caretPosition.column + 1, margin);
						DrawSelectionRect(selectionStartPosition.line, 0, selectionStartPosition.column, margin);
						firstLine = caretPosition.line + 1;
						lastLine = selectionStartPosition.line - 1;
					}
					else
					{
						DrawSelectionRect(selectionStartPosition.line, selectionStartPosition.column,
							FGTextBuffer.ExpandTabs(textBuffer.lines[selectionStartPosition.line]).Length - selectionStartPosition.column + 1, margin);
						DrawSelectionRect(caretPosition.line, 0, caretPosition.column, margin);
						firstLine = selectionStartPosition.line + 1;
						lastLine = caretPosition.line - 1;
					}

					if (firstLine < fromLine)
						firstLine = fromLine;
					if (lastLine >= toLine)
						lastLine = toLine - 1;
					for (int line = firstLine; line <= lastLine; ++line)
						DrawSelectionRect(line, 0, FGTextBuffer.ExpandTabs(textBuffer.lines[line]).Length + 1, margin);
				}
			}

			if (pingTimer > 0f && pingStartTime != 0f)
			{
				pingTimer = 1f - (Time.realtimeSinceStartup - pingStartTime) * 0.5f;
				if (pingTimer < 0f)
				{
					pingTimer = 0f;
					pingStartTime = 0f;
				}

				DrawPing(margin, true);
			}
		}

		if (Event.current.type == EventType.ScrollWheel && EditorGUI.actionKey)
		{
			Event.current.Use();
			ModifyFontSize(-(int)Event.current.delta.y);
			return;
		}

		FGTextBuffer.TextBlock[] tempTextBlocks = null;
		FGTextBuffer.TextBlock[] textBlocks;
		Rect tempRC = new Rect();
		tempRC.y = wordWrapping ? GetLineOffset(fromLine) - charSize.y : (fromLine - 1) * charSize.y;
		for (int i = fromLine; i < toLine; ++i)
		{
			tempRC.x = margin;
			tempRC.y += charSize.y;
			tempRC.height = charSize.y;

			FGTextBuffer.FormatedLine line = textBuffer.formatedLines[i];

			textBlocks = line.textBlocks;
			if (line.textBlocks == null)
			{
				if (tempTextBlocks == null)
					tempTextBlocks = new FGTextBuffer.TextBlock[] { new FGTextBuffer.TextBlock(FGTextBuffer.ExpandTabs(textBuffer.lines[i]), styles.normalStyle) };
				else
					tempTextBlocks[0].text = FGTextBuffer.ExpandTabs(textBuffer.lines[i]);
				textBlocks = tempTextBlocks;
			}

			softLineBreaks = GetSoftLineBreaks(i);
			int numBreaks = wordWrapping /*&& !IsLoading*/ ? softLineBreaks.Count : 0;
			if (numBreaks == 0)
			{
				foreach (FGTextBuffer.TextBlock block in textBlocks)
				{
					tempRC.width = charSize.x * block.text.Length;

					if (block.style == styles.hyperlinkStyle || block.style == styles.mailtoStyle)
					{
						if (GUI.Button(tempRC, block.text, block.style))
						{
							if (block.style == styles.hyperlinkStyle)
								Application.OpenURL(block.text);
							else
								Application.OpenURL("mailto:" + block.text);
						}

						if (Event.current.type == EventType.repaint)
						{
							// show the "Link" cursor when the mouse is howering over this rectangle.
							EditorGUIUtility.AddCursorRect(tempRC, MouseCursor.Link);
						}
					}
					else
					{
						if (tempRC.x - margin < scrollPosition.x && tempRC.xMax - margin > scrollPosition.x)
						{
							int skipBeginning = (int)(scrollPosition.x - tempRC.x + margin) / (int)charSize.x;
							int maxChars = (int)scrollViewRect.width / (int)charSize.x + 1;
							GUI.Label(new Rect(tempRC){ x = tempRC.x + skipBeginning * charSize.x },
								block.text.Substring(skipBeginning, Mathf.Min(block.text.Length - skipBeginning, maxChars)), block.style);
						}
						else
						{
							GUI.Label(tempRC, block.text, block.style);
						}
					}

					tempRC.x += tempRC.width;
				}
			}
			else
			{
				int lineLength = 0;
				int column = 0;
				int softRow = 0;
				foreach (FGTextBuffer.TextBlock block in textBlocks)
				{
					int textStart = 0;
					int textLength = block.text.Length;
					while (textStart < textLength)
					{
						int rowLength = softRow < numBreaks ? softLineBreaks[softRow] - lineLength : int.MaxValue;
						int charsToDraw = Math.Min(textLength - textStart, rowLength);
						
						if (charsToDraw > 0)
						{
							tempRC.width = charSize.x * charsToDraw;
	
							if (block.style == styles.hyperlinkStyle || block.style == styles.mailtoStyle)
							{
								if (GUI.Button(tempRC, block.text.Substring(textStart, charsToDraw), block.style))
								{
									if (block.style == styles.hyperlinkStyle)
										Application.OpenURL(block.text);
									else
										Application.OpenURL("mailto:" + block.text);
								}
	
								if (Event.current.type == EventType.repaint)
								{
									// show the "Link" cursor when the mouse is howering over this rectangle.
									EditorGUIUtility.AddCursorRect(tempRC, MouseCursor.Link);
								}
							}
							else
							{
								GUI.Label(tempRC, block.text.Substring(textStart, charsToDraw), block.style);
							}
	
							tempRC.x += tempRC.width;
						}
						
						lineLength += charsToDraw;
						column += charsToDraw;
						textStart += charsToDraw;
						if (textStart < textLength)
						{
							tempRC.x = margin;
							tempRC.y += charSize.y;
							++softRow;
							column = 0;
						}
					}
				}
			}
		}

		bool isDragEvent = Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragUpdated;
		if (isDragEvent || Event.current.isMouse || Event.current.type == EventType.Repaint && simulateLastMouseEvent)
		{
			simulateLastMouseEvent = simulateLastMouseEvent && !Event.current.isMouse && !isDragEvent;
			Event current = simulateLastMouseEvent ? new Event(lastMouseEvent) : Event.current;
			if (!simulateLastMouseEvent)
				lastMouseEvent = new Event(Event.current);
			simulateLastMouseEvent = false;

			ProcessEditorMouse(margin - 2f, current);

			if (codeViewDragging)
			{
				autoScrollLeft = !wordWrapping && lastMouseEvent.mousePosition.x < codeViewRect.x;
				autoScrollRight = !wordWrapping && lastMouseEvent.mousePosition.x >= codeViewRect.xMax;
				autoScrollUp = lastMouseEvent.mousePosition.y < codeViewRect.y;
				autoScrollDown = lastMouseEvent.mousePosition.y >= codeViewRect.yMax;
			}

			if (Event.current.type == EventType.used)
			{
				GUI.EndScrollView();
				return;
			}
		}

		if (hasCodeViewFocus && Event.current.type == EventType.repaint && CanEdit())
		{
			FGTextBuffer.CaretPos position = codeViewDragging && mouseDownOnSelection ? mouseDropPosition.Clone() : caretPosition.Clone();

			float caretTime = (Time.realtimeSinceStartup - caretMoveTime) % 1f; // / 1f;
			isCaretOn = caretTime < 0.5f;
			if ((isCaretOn || pingTimer > 0f && pingStartTime != 0f) && position.line >= fromLine && position.line < toLine)
			{
				int row;
				int column;
				BufferToViewPosition(position, out row, out column);
				Rect caretRect = new Rect(
					charSize.x * column + margin,
					charSize.y * row + GetLineOffset(position.line), // visibleLinePositions[position.line - fromLine],
					1,
					charSize.y);
				GUI.Box(caretRect, GUIContent.none, isCaretOn ? styles.caretStyle : styles.scrollViewStyle);
			}
		}

		if (Event.current.type == EventType.repaint)
		{
			if (showLineNumbers || trackChanges)
			{
				tempRC.Set(-4f, -4f, margin - 2f + scrollPosition.x, contentHeight);
				EditorGUIUtility.AddCursorRect(tempRC, MouseCursor.Arrow);

				tempRC.Set(-4f, -4f, margin - 1f + scrollPosition.x, contentHeight);

				// if the source code is shorter than the view...
				if (tempRC.height < scrollViewRect.height)
					tempRC.height = scrollViewRect.height;
				GUI.Label(tempRC, GUIContent.none, styles.lineNumbersBackground);

				tempRC.xMin = margin - 5f + scrollPosition.x;
				tempRC.width = 1f;
				GUI.Label(tempRC, GUIContent.none, styles.lineNumbersSeparator);
			}

			if (showLineNumbers)
			{
				if (wordWrapping /*&& !IsLoading*/)
				{
					for (int i = fromLine; i < toLine; ++i)
					{
						lineNumberContent.text = (i + 1).ToString().PadLeft(lineNumbersMaxLength);
						tempRC.Set(scrollPosition.x, GetLineOffset(i) /*visibleLinePositions[i - fromLine]*/, lineNumbersWidth, charSize.y);
						styles.lineNumbersStyle.Draw(tempRC, lineNumberContent, caretPosition.line == i, false, false, false);
					}
				}
				else
				{
					for (int i = fromLine; i < toLine; ++i)
					{
						lineNumberContent.text = (i + 1).ToString().PadLeft(lineNumbersMaxLength);
						tempRC.Set(scrollPosition.x, charSize.y * i, lineNumbersWidth, charSize.y);
						styles.lineNumbersStyle.Draw(tempRC, lineNumberContent, caretPosition.line == i, false, false, false);
					}
				}
			}

			if (trackChanges)
			{
				tempRC.xMin = margin - 13f + scrollPosition.x;
				//tempRC.yMin = GetLineOffset(fromLine);
				tempRC.width = 5f;
				//tempRC.height = GetLineOffset(fromLine + 1) - tempRC.yMin;

				for (int i = fromLine; i < toLine; ++i)
				{
					int savedVersion = textBuffer.formatedLines[i].savedVersion;
					int version = textBuffer.formatedLines[i].lastChange;
					if (savedVersion > 0 || version > 0)
					{
						tempRC.yMin = GetLineOffset(i);
						tempRC.yMax = GetLineOffset(i + 1);

						if (version == savedVersion)
							GUI.Label(tempRC, GUIContent.none, styles.trackChangesAfterSaveStyle);
						else if (version > savedVersion)
							GUI.Label(tempRC, GUIContent.none, styles.trackChangesBeforeSaveStyle);
						else
							GUI.Label(tempRC, GUIContent.none, styles.trackChangesRevertedStyle);
					}
				}
			}
		}

		EditorGUIUtility.AddCursorRect(wordWrapping ? new Rect(contentRect.x, contentRect.y, contentRect.width, contentHeight) : contentRect, MouseCursor.Text);

		if (Event.current.type == EventType.repaint && pingTimer > 0f)
		{
			DrawPing(margin, false);
			if (pingTimer > 0f)
				Repaint();
		}

		GUI.EndScrollView();
	}

	private void DrawSelectionRect(int line, int startColumn, int numColumns, float margin)
	{
		if (!wordWrapping)
		{
			Rect selectionRect = new Rect(charSize.x * startColumn + margin, charSize.y * line, charSize.x * numColumns, charSize.y);
			GUI.Box(selectionRect, GUIContent.none, hasCodeViewFocus ? styles.activeSelectionStyle : styles.passiveSelectionStyle);

			if (!codeViewDragging)
			{
				// show the "Arrow" cursor when the mouse is howering over this rectangle.
				EditorGUIUtility.AddCursorRect(selectionRect, MouseCursor.Arrow);
			}
		}
		else
		{
			float yOffset = GetLineOffset(line);

			List<int> softLineBreaks = GetSoftLineBreaks(line);
			int row = FindFirstIndexGreaterThanOrEqualTo<int>(softLineBreaks, startColumn);
			if (row < softLineBreaks.Count && startColumn == softLineBreaks[row])
				++row;

			int rowStart = row > 0 ? softLineBreaks[row - 1] : 0;
			startColumn -= rowStart;
			while (numColumns > 0)
			{
				int rowLength = (row < softLineBreaks.Count ? softLineBreaks[row] - rowStart : startColumn + numColumns);
				int nCols = Math.Min(numColumns, rowLength - startColumn);

				Rect selectionRect = new Rect(charSize.x * startColumn + margin, yOffset + charSize.y * row, charSize.x * nCols, charSize.y);
				GUI.Box(selectionRect, GUIContent.none, hasCodeViewFocus ? styles.activeSelectionStyle : styles.passiveSelectionStyle);
				if (!codeViewDragging)
					EditorGUIUtility.AddCursorRect(selectionRect, MouseCursor.Arrow);

				numColumns -= nCols;
				rowStart += rowLength;
				startColumn = 0;
				++row;
			}
		}
	}

	public void ValidateCarets()
	{
		if (CanEdit())
		{
			ValidateCaret(ref caretPosition);
			if (hasSelection)
				ValidateCaret(ref _selectionStartPosition);
			Repaint();
		}
	}

	private void ValidateCaret(ref FGTextBuffer.CaretPos caret)
	{
		if (caret != null && textBuffer.lines.Count > 0)
		{
			if (caret.line < 0)
				return;
			else if (caret.line >= textBuffer.lines.Count)
				caret = new FGTextBuffer.CaretPos() { line = textBuffer.lines.Count - 1, characterIndex = 0, column = 0, virtualColumn = 0 };
			else if (caret.characterIndex > textBuffer.lines[caret.line].Length)
				caret = new FGTextBuffer.CaretPos() { line = caret.line, characterIndex = 0, column = 0, virtualColumn = 0 };
			else
				caret.column = caret.virtualColumn = textBuffer.CharIndexToColumn(caret.characterIndex, caret.line);
		}
	}
	
	private bool lineSelectMode = false;
	private bool mouseIsDown = false;

	private void ProcessEditorMouse(float margin, Event current)
	{
		if (!CanEdit())
			return;

		if (GUIUtility.hotControl != 0 && GUIUtility.hotControl != codeViewControlID && DragAndDrop.GetGenericData("ScriptInspector.Text") == null)
			return;

		if (current.type == EventType.MouseDown && current.button == 0)
		{
			mouseIsDown = true;
		}
		if (!mouseIsDown && current.button == 0)
			return;
		if (current.rawType == EventType.MouseUp && current.button == 0)
		{
			mouseIsDown = false;
			codeViewDragging = false;
			lineSelectMode = false;
		}

		EventType eventType = current.type;
		EventModifiers modifiers = current.modifiers;
		bool isDrag = current.type == EventType.mouseDrag;

		int nextCaretColumn = caretPosition.virtualColumn;
		int nextCharacterIndex = caretPosition.characterIndex;
		int nextCaretLine = caretPosition.line;
		FGTextBuffer.CaretPos nextCaretPos = caretPosition.Clone();

		float x = current.mousePosition.x;
		float y = current.mousePosition.y;
		if (isDrag)
		{
			x = Mathf.Clamp(x, codeViewRect.x, codeViewRect.xMax);
			y = Mathf.Clamp(y, codeViewRect.y, codeViewRect.yMax);
		}
		x -= margin;

		int clickedColumn;
		int clickedLine;
		int clickedCharIndex;
		FGTextBuffer.CaretPos clickedPos;
		if (!wordWrapping)
		{
			clickedColumn = (int)(x / charSize.x);
			clickedLine = Math.Min((int)(y / charSize.y), textBuffer.lines.Count - 1);
			clickedCharIndex = textBuffer.ColumnToCharIndex(ref clickedColumn, clickedLine);

			clickedPos = new FGTextBuffer.CaretPos
			{
				line = clickedLine,
				virtualColumn = clickedColumn,
				column = clickedColumn,
				characterIndex = clickedCharIndex
			};
		}
		else
		{
			clickedColumn = (int)(x / charSize.x);
			clickedLine = GetLineAt(y);
			int row = (int) ((y - GetLineOffset(clickedLine)) / charSize.y);
			clickedPos = ViewToBufferPosition(clickedLine, row, clickedColumn);
			clickedCharIndex = clickedPos.characterIndex;
			clickedColumn = clickedPos.column;
		}

		//if (current.type == EventType.DragExited)
		//{
		//}

		if (current.type == EventType.DragPerform || current.type == EventType.DragUpdated /*|| current.type == EventType.DragExited*/)
		{
			int mousePosToCaretPos = clickedPos.CompareTo(caretPosition);
			int mousePosToSelStartPos = selectionStartPosition != null ? clickedPos.CompareTo(selectionStartPosition) : mousePosToCaretPos;
			bool mouseOnSelection = !((mousePosToCaretPos < 0 && mousePosToSelStartPos < 0) || (mousePosToCaretPos > 0 && mousePosToSelStartPos > 0));
			if (EditorGUI.actionKey && (mousePosToCaretPos == 0 || mousePosToSelStartPos == 0))
				mouseOnSelection = false;

			DragAndDrop.visualMode = mouseOnSelection ? DragAndDropVisualMode.Rejected : EditorGUI.actionKey ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Move;

			if (current.type == EventType.DragPerform)
			{
				object data = DragAndDrop.GetGenericData("ScriptInspector.Text");
				if (!string.IsNullOrEmpty(data as string))
				{
					textBuffer.BeginEdit("Drag selection");

					if (!EditorGUI.actionKey)
					{
						FGTextBuffer.CaretPos temp = textBuffer.DeleteText(selectionStartPosition, caretPosition);
						textBuffer.UpdateHighlighting(temp.line, temp.line);

						if (mouseDropPosition > caretPosition)
						{
							int linesDeleted = Math.Abs(caretPosition.line - selectionStartPosition.line);
							if (linesDeleted == 0)
							{
								if (caretPosition.line == mouseDropPosition.line)
									mouseDropPosition.characterIndex -= Math.Abs(caretPosition.characterIndex - selectionStartPosition.characterIndex);
							}
							else
							{
								if (Math.Max(caretPosition.line, selectionStartPosition.line) == mouseDropPosition.line)
								{
									mouseDropPosition.line = temp.line;
									mouseDropPosition.characterIndex -= (caretPosition > selectionStartPosition ?
										caretPosition.characterIndex : selectionStartPosition.characterIndex) - temp.characterIndex;
								}
								else
								{
									mouseDropPosition.line -= linesDeleted;
								}
							}
							mouseDropPosition.column = mouseDropPosition.virtualColumn = textBuffer.CharIndexToColumn(mouseDropPosition.characterIndex, mouseDropPosition.line);
						}
					}

					caretPosition = textBuffer.InsertText(mouseDropPosition, data as string);
					textBuffer.UpdateHighlighting(mouseDropPosition.line, caretPosition.line);
					selectionStartPosition = mouseDropPosition.Clone();

					textBuffer.EndEdit();
				}
			}

			if (current.type != EventType.DragUpdated)
			{
				DragAndDrop.AcceptDrag();
				DragAndDrop.SetGenericData("ScriptInspector.Text", null);

				GUIUtility.hotControl = 0;
				lineSelectMode = false;
				codeViewDragging = false;
				mouseDownOnSelection = false;
				autoScrollLeft = false;
				autoScrollRight = false;
				autoScrollUp = false;
				autoScrollDown = false;
				current.Use();
				return;
			}

			if (mouseOnSelection)
				clickedPos = caretPosition.Clone();

			if (clickedPos != mouseDropPosition)
			{
				mouseDropPosition = clickedPos.Clone();
				caretMoveTime = Time.realtimeSinceStartup;
				//scrollToCaret = true;
				needsRepaint = true;
			}

			GUIUtility.hotControl = codeViewControlID;
			focusCodeView = true;
			current.Use();
			return;
		}

		if (!codeViewDragging && current.mousePosition.y >= 0 &&
			current.type == EventType.MouseDown &&
			(isDrag && current.mousePosition.x >= codeViewRect.x || current.type == EventType.MouseDown && current.mousePosition.x >= 0))
		{
			if (hasSelection)
			{
				int clickedPosToCaretPos = clickedPos.CompareTo(caretPosition);
				int clickedPosToSelStartPos = clickedPos.CompareTo(selectionStartPosition);
				mouseDownOnSelection = !((clickedPosToCaretPos < 0 && clickedPosToSelStartPos < 0) || (clickedPosToCaretPos >= 0 && clickedPosToSelStartPos >= 0));
			}
			else
			{
				mouseDownOnSelection = false;
			}
		}

		if (isDrag && current.button == 0)
		{
			if (!codeViewDragging)
			{
//				if (!codeViewRect.Contains(current.mousePosition))
//					return;

				lastAutoScrollTime = Time.realtimeSinceStartup;

				if (mouseDownOnSelection && !current.shift)
				{
					DragAndDrop.PrepareStartDrag();
					DragAndDrop.objectReferences = new UnityEngine.Object[] { textBuffer };
					DragAndDrop.StartDrag("Dragging selected text");
					DragAndDrop.SetGenericData("ScriptInspector.Text", textBuffer.GetTextRange(selectionStartPosition, caretPosition));

					GUIUtility.hotControl = 0;
					current.Use();
					codeViewDragging = true;
					mouseDropPosition = caretPosition.Clone();
					return;
				}
				else
				{
					mouseDownOnSelection = false;
				}
			}
			codeViewDragging = true;
			//needsRepaint = true;
		}
		else if (current.button == 1)
		{
			if (current.type == EventType.mouseDown && current.mousePosition.x >= 0 && current.mousePosition.y >= 0)
			{
				if (!mouseDownOnSelection)
				{
					nextCharacterIndex = clickedPos.characterIndex;
					nextCaretColumn = clickedColumn;
					nextCaretLine = clickedLine;
					nextCaretPos = clickedPos.Clone();
					scrollToCaret = true;
				}
				current.Use();
			}
			else
			{
				return;
			}
		}

		if (isDrag || current.type == EventType.mouseDown)
		{
			GUIUtility.hotControl = codeViewControlID;
			focusCodeView = true;
			if (current.mousePosition.x >= 0 && current.mousePosition.y >= 0 || isDrag)
			{
				if (!isDrag && current.mousePosition.x < margin + scrollPosition.x)
					lineSelectMode = true;

				scrollToCaret = !isDrag && !mouseDownOnSelection;

				nextCharacterIndex = clickedCharIndex;
				nextCaretColumn = clickedColumn;
				nextCaretLine = clickedLine;
				nextCaretPos = clickedPos.Clone();
					
				if (current.button == 0)
				{
					if (current.clickCount == 1 && mouseDownOnSelection)
					{
						needsRepaint = true;
						return;
					}

					if (current.clickCount == 2 || EditorGUI.actionKey)
					{
						int wordStart;
						int wordEnd;
						if (textBuffer.GetWordExtents(nextCharacterIndex, clickedLine, out wordStart, out wordEnd))
						{
							// select word
							selectionStartPosition = null;
							caretPosition.line = clickedLine;
							caretPosition.characterIndex = wordStart;
							caretPosition.virtualColumn = caretPosition.column = textBuffer.CharIndexToColumn(wordStart, clickedLine);
							nextCharacterIndex = wordEnd;
							nextCaretColumn = textBuffer.CharIndexToColumn(wordEnd, clickedLine);
							nextCaretPos = clickedPos.Clone();
							nextCaretPos.column = nextCaretColumn;
							nextCaretPos.characterIndex = nextCharacterIndex;
							modifiers |= EventModifiers.Shift;
						}
					}
				}
			}
			current.Use();
		}

		int lineSelectOffset = 0;
		if (lineSelectMode && selectionStartPosition != null && selectionStartPosition < caretPosition)
			lineSelectOffset = -1;

		if (current.rawType == EventType.mouseUp && current.button == 0 && GUIUtility.hotControl != 0)
		{
			if (mouseDownOnSelection)
			{
				if (!codeViewDragging)
				{
					nextCharacterIndex = clickedCharIndex;
					nextCaretColumn = clickedColumn;
					nextCaretLine = clickedLine;
					nextCaretPos = clickedPos.Clone();
					--caretPosition.virtualColumn;
				}
			}

			GUIUtility.hotControl = 0;
			//autoScrolling = Vector2.zero;
			lineSelectMode = false;
			codeViewDragging = false;
			mouseDownOnSelection = false;
			autoScrollLeft = false;
			autoScrollRight = false;
			autoScrollUp = false;
			autoScrollDown = false;
			current.Use();

			needsRepaint = true;
			lineSelectOffset = 0;
		}

		//if (nextCaretColumn != caretPosition.virtualColumn ||
		//    nextCaretLine != (caretPosition.line + lineSelectOffset) ||
		//    eventType == EventType.mouseDown && current.button == 0)
		if (!nextCaretPos.IsSameAs(caretPosition) ||
			nextCaretPos.line != (caretPosition.line + lineSelectOffset) ||
			eventType == EventType.mouseDown && current.button == 0)
		{
			caretMoveTime = Time.realtimeSinceStartup;

			//if (nextCaretLine < 0)
			//    nextCaretLine = 0;
			if (nextCaretPos.line < 0)
				nextCaretPos.Set(0, 0, 0, 0);
			//if (nextCaretLine >= textBuffer.numParsedLines)
			//    nextCaretLine = textBuffer.numParsedLines - 1;
			if (nextCaretPos.line >= textBuffer.numParsedLines)
				nextCaretPos.Set(textBuffer.numParsedLines - 1, 0, 0, 0);
			nextCaretLine = nextCaretPos.line;

			if (selectionStartPosition == null && (isDrag || (modifiers & EventModifiers.Shift) != 0))
				selectionStartPosition = caretPosition.Clone();

			if (lineSelectMode)
			{
				if (selectionStartPosition == null || !isDrag && (modifiers & EventModifiers.Shift) == 0)
					selectionStartPosition = new FGTextBuffer.CaretPos { column = 0, virtualColumn = 0, characterIndex = 0, line = nextCaretLine };

				nextCharacterIndex = 0;
				nextCaretColumn = 0;
				if (nextCaretLine >= selectionStartPosition.line)
				{
					++nextCaretLine;
					nextCaretPos.Set(nextCaretPos.line + 1, 0, 0, 0);
					if (nextCaretLine >= textBuffer.numParsedLines)
					{
						nextCaretLine = textBuffer.numParsedLines - 1;
						nextCharacterIndex = textBuffer.lines[nextCaretLine].Length;
						nextCaretColumn = textBuffer.CharIndexToColumn(nextCharacterIndex, nextCaretLine);
						nextCaretPos.Set(textBuffer.numParsedLines - 1, textBuffer.lines[nextCaretLine].Length, nextCaretColumn);
					}
					selectionStartPosition = new FGTextBuffer.CaretPos { characterIndex = 0, column = 0, virtualColumn = 0, line = selectionStartPosition.line };
				}
				else
				{
					int charIndex = textBuffer.lines[selectionStartPosition.line].Length;
					int column = textBuffer.CharIndexToColumn(charIndex, selectionStartPosition.line);
					nextCaretPos.Set(nextCaretPos.line, 0, 0, 0);
					selectionStartPosition = new FGTextBuffer.CaretPos { characterIndex = charIndex, column = column, virtualColumn = column, line = selectionStartPosition.line };
				}
			}

			caretPosition = nextCaretPos;
			//caretPosition.line = nextCaretLine;
			if (nextCaretLine >= 0)
			{
				caretPosition.characterIndex = nextCharacterIndex;
				caretPosition.column = caretPosition.virtualColumn = Math.Min(nextCaretColumn, FGTextBuffer.ExpandTabs(textBuffer.lines[nextCaretLine]).Length);
				if (wordWrapping)
				{
					List<int> softLineBreaks = GetSoftLineBreaks(caretPosition.line);
					int row = FindFirstIndexGreaterThanOrEqualTo<int>(softLineBreaks, caretPosition.column);
					if (row < softLineBreaks.Count && caretPosition.column == softLineBreaks[row] && clickedPos.virtualColumn == 0)
						++row;
					caretPosition.virtualColumn -= row > 0 ? softLineBreaks[row - 1] : 0;
				}
			}

			if (!isDrag && !lineSelectMode && (modifiers & EventModifiers.Shift) == 0)
				selectionStartPosition = null;

			if (!codeViewDragging)
				scrollToCaret = true;
			//Repaint();
			needsRepaint = true;
		}
	}

	private bool ProcessCodeViewCommands()
	{
		if (Event.current.type == EventType.ValidateCommand)
		{
			if (Event.current.commandName == "SelectAll")
			{
				Event.current.Use();
				return true;
			}
			else if (Event.current.commandName == "Copy" || Event.current.commandName == "Cut")
			{
				if (selectionStartPosition != null)
				{
					Event.current.Use();
					return true;
				}
			}
			else if (Event.current.commandName == "Paste")
			{
				if (!string.IsNullOrEmpty(EditorGUIUtility.systemCopyBuffer))
				{
					Event.current.Use();
					return true;
				}
			}
			else if (Event.current.commandName == "Delete")
			{
				Event.current.Use();
				return true;
			}
			else if (Event.current.commandName == "OpenAtCursor")
			{
				Event.current.Use();
				return true;
			}
		}
		else if (Event.current.type == EventType.ExecuteCommand)
		{
			if (Event.current.commandName == "SelectAll")
			{
				selectionStartPosition = new FGTextBuffer.CaretPos { column = 0, virtualColumn = 0, characterIndex = 0, line = 0 };

				caretPosition.line = textBuffer.numParsedLines - 1;
				caretPosition.characterIndex = textBuffer.numParsedLines > 0 ? textBuffer.lines[textBuffer.numParsedLines - 1].Length : 0;
				caretPosition.column = caretPosition.virtualColumn = textBuffer.CharIndexToColumn(caretPosition.characterIndex, textBuffer.numParsedLines - 1);

				Event.current.Use();
				caretMoveTime = Time.realtimeSinceStartup;
				scrollToCaret = true;
				Repaint();
				return true;
			}
			else if (Event.current.commandName == "Paste")
			{
				if (!string.IsNullOrEmpty(EditorGUIUtility.systemCopyBuffer))
				{
					if (selectionStartPosition != null)
					{
						caretPosition = textBuffer.DeleteText(selectionStartPosition, caretPosition);
						selectionStartPosition = null;
					}
					
					string copyBuffer = EditorGUIUtility.systemCopyBuffer;
					copyBuffer = copyBuffer.Replace("\r\n", "\n");
					copyBuffer = copyBuffer.Replace('\r', '\n');

					int insertedAtLine = caretPosition.line;
					caretPosition = textBuffer.InsertText(caretPosition, copyBuffer);
					textBuffer.UpdateHighlighting(insertedAtLine, caretPosition.line);
					textBuffer.SetUndoActionType("Paste Text");

					Event.current.Use();
					caretMoveTime = Time.realtimeSinceStartup;
					scrollToCaret = true;
					Repaint();
					return true;
				}
			}
			else if (Event.current.commandName == "Copy" || Event.current.commandName == "Cut")
			{
				if (selectionStartPosition != null)
				{
					EditorGUIUtility.systemCopyBuffer = textBuffer.GetTextRange(caretPosition, selectionStartPosition);

					if (Event.current.commandName == "Cut")
					{
						caretPosition = textBuffer.DeleteText(selectionStartPosition, caretPosition);
						textBuffer.UpdateHighlighting(caretPosition.line, caretPosition.line);
						textBuffer.SetUndoActionType("Cut Text");
						selectionStartPosition = null;

						caretMoveTime = Time.realtimeSinceStartup;
						scrollToCaret = true;
						Repaint();
					}

					Event.current.Use();
					return true;
				}
			}
			else if (Event.current.commandName == "Delete")
			{
				Event simKey = new Event();
				simKey.type = EventType.keyDown;
				simKey.keyCode = KeyCode.Delete;
				simKey.modifiers = EventModifiers.Shift;
				ProcessEditorKeyboard(simKey);

				Event.current.Use();
				return true;
			}
			else if (Event.current.commandName == "OpenAtCursor")
			{
				Event.current.Use();
				OpenAtCursor();
				return true;
			}
		}
		return false;
	}
	
	private void OpenAtCursor()
	{
		if (IsModified)
		{
			if (EditorUtility.DisplayDialog(
				"Script Inspector 2",
				AssetDatabase.GUIDToAssetPath(textBuffer.guid)
					+ "\n\nThis asset has been modified inside the Script Inspector.\nDo you want to save the changes before opening it in the external IDE?",
				"Save",
				"Cancel"))
			{
				SaveBuffer();
			}
			else
			{
				return;
			}
		}
		AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(textBuffer.guid), typeof(UnityEngine.Object)), caretPosition.line + 1);
	}

	private void UseSelectionForSearch()
	{
		if (selectionStartPosition != null && selectionStartPosition.line == caretPosition.line)
		{
			if (caretPosition > selectionStartPosition)
				searchString = textBuffer.lines[caretPosition.line].Substring(
					selectionStartPosition.characterIndex, caretPosition.characterIndex - selectionStartPosition.characterIndex);
			else
				searchString = textBuffer.lines[caretPosition.line].Substring(
					caretPosition.characterIndex, selectionStartPosition.characterIndex - caretPosition.characterIndex);
			SetSearchText(searchString);
			return;
		}

		int wordStart;
		int wordEnd;
		if (textBuffer.GetWordExtents(caretPosition.characterIndex, caretPosition.line, out wordStart, out wordEnd))
		{
			searchString = textBuffer.lines[caretPosition.line].Substring(wordStart, wordEnd - wordStart);
			SetSearchText(searchString);
		}
	}

	private void IndentMoreOrInsertTab()
	{
		if (selectionStartPosition != null)
		{
			IndentMore();
		}
		else
		{
			textBuffer.BeginEdit("Insert Tab");

			int spaces = 0;
			while (((caretPosition.column - spaces) & 3) > 0)
			{
				int prev = caretPosition.characterIndex - spaces - 1;
				if (prev >= 0 && textBuffer.lines[caretPosition.line][prev] == ' ')
					++spaces;
				else
					break;
			}
			if (spaces > 0)
			{
				FGTextBuffer.CaretPos replaceSpaces = caretPosition.Clone();
				replaceSpaces.characterIndex -= spaces;
				replaceSpaces.column -= spaces;
				replaceSpaces.virtualColumn -= spaces;
				caretPosition = textBuffer.DeleteText(replaceSpaces, caretPosition);
			}
			caretPosition = textBuffer.InsertText(caretPosition, "\t");
			textBuffer.UpdateHighlighting(caretPosition.line, caretPosition.line);

			textBuffer.EndEdit();
		}
	}

	private void IndentMore()
	{
		textBuffer.BeginEdit("Increase Indent");

		bool hasSelection = selectionStartPosition != null;
		if (!hasSelection)
			selectionStartPosition = caretPosition.Clone();

		FGTextBuffer.CaretPos from = caretPosition.Clone();
		FGTextBuffer.CaretPos to = caretPosition.Clone();
		int fromLine = caretPosition.line;
		int toLine = caretPosition.line;
		if (caretPosition < selectionStartPosition)
		{
			to = selectionStartPosition.Clone();
			toLine = to.line;
		}
		else
		{
			from = selectionStartPosition.Clone();
			fromLine = from.line;
		}
		if (to.characterIndex == 0 && fromLine < toLine)
			--toLine;

		bool moveFromPos = textBuffer.FirstNonWhitespace(fromLine) < from.characterIndex;
		bool moveToPos = to.line == toLine && textBuffer.FirstNonWhitespace(toLine) <= to.characterIndex;
		for (FGTextBuffer.CaretPos i = new FGTextBuffer.CaretPos { characterIndex = 0, column = 0, line = fromLine, virtualColumn = 0 }; i.line <= toLine; ++i.line)
			textBuffer.InsertText(i, "\t");
		textBuffer.UpdateHighlighting(fromLine, toLine);

		if (moveFromPos)
			++from.characterIndex;
		if (moveToPos)
			++to.characterIndex;
		from.column = from.virtualColumn = textBuffer.CharIndexToColumn(from.characterIndex, from.line);
		to.column = to.virtualColumn = textBuffer.CharIndexToColumn(to.characterIndex, to.line);

		if (caretPosition < selectionStartPosition)
		{
			caretPosition = from.Clone();
			selectionStartPosition = to.Clone();
		}
		else
		{
			selectionStartPosition = from.Clone();
			caretPosition = to.Clone();
		}

		if (!hasSelection)
			selectionStartPosition = null;

		textBuffer.EndEdit();
	}

	private void IndentLess()
	{
		textBuffer.BeginEdit("Decrease Indent");

		bool hasSelection = selectionStartPosition != null;
		if (!hasSelection)
			selectionStartPosition = caretPosition.Clone();

		FGTextBuffer.CaretPos from = caretPosition.Clone();
		FGTextBuffer.CaretPos to = caretPosition.Clone();
		int fromLine = caretPosition.line;
		int toLine = caretPosition.line;
		if (caretPosition < selectionStartPosition)
		{
			to = selectionStartPosition.Clone();
			toLine = to.line;
		}
		else
		{
			from = selectionStartPosition.Clone();
			fromLine = from.line;
		}
		if (to.characterIndex == 0 && fromLine < toLine)
			--toLine;

		bool moveFromPos = from.characterIndex > 0 && (caretPosition.line == selectionStartPosition.line || textBuffer.FirstNonWhitespace(fromLine) <= from.characterIndex);
		bool moveToPos = to.characterIndex > 0 && textBuffer.FirstNonWhitespace(toLine) <= to.characterIndex;
		for (FGTextBuffer.CaretPos i = new FGTextBuffer.CaretPos { characterIndex = 0, column = 0, line = fromLine, virtualColumn = 0 }; i.line <= toLine; ++i.line)
		{
			FGTextBuffer.CaretPos j = i.Clone();
			while (j.characterIndex < textBuffer.lines[i.line].Length && FGTextBuffer.GetCharClass(textBuffer.lines[i.line][j.characterIndex]) == 0)
			{
				j.column = j.virtualColumn = textBuffer.CharIndexToColumn(++j.characterIndex, j.line);
				if (j.column == 4)
					break;
			}
			if (i != j)
			{
				textBuffer.DeleteText(i, j);
			}
			else
			{
				if (i.line == fromLine)
					moveFromPos = false;
				if (i.line == toLine)
					moveToPos = false;
			}
		}
		textBuffer.UpdateHighlighting(fromLine, toLine);

		if (moveFromPos)
			--from.characterIndex;
		if (moveToPos)
			--to.characterIndex;
		from.column = from.virtualColumn = textBuffer.CharIndexToColumn(from.characterIndex, from.line);
		to.column = to.virtualColumn = textBuffer.CharIndexToColumn(to.characterIndex, to.line);

		if (caretPosition < selectionStartPosition)
		{
			caretPosition = from.Clone();
			selectionStartPosition = to.Clone();
		}
		else
		{
			selectionStartPosition = from.Clone();
			caretPosition = to.Clone();
		}

		if (!hasSelection)
			selectionStartPosition = null;

		textBuffer.EndEdit();
	}

	//private FGListPopup autocompleteWindow;
	//private string autoCompleteWord;

	//private void Autocomplete()
	//{
	//	if (autocompleteWindow == null)
	//	{
	//		Rect caretRect = new Rect(charSize.x * caretPosition.column + margin, charSize.y * caretPosition.line, 1, charSize.y);
	//		caretRect.x += 4f + scrollViewRect.x - scrollPosition.x;
	//		caretRect.y += 4f + scrollViewRect.y - scrollPosition.y;

	//		List<string> data = new List<string>();
	//		//data.AddRange(textBuffer.PreprocessorDirectives);
	//		data.AddRange(textBuffer.Keywords);
	//		data.AddRange(textBuffer.BuiltInTypes);
	//		autocompleteWindow = FGListPopup.Create(caretRect, data.ToArray());
	//	}
	//}

	//private void CloseAutocomplete()
	//{
	//	if (autocompleteWindow != null)
	//	{
	//		autocompleteWindow.Close();
	//		autocompleteWindow = null;
	//	}
	//}

	public void BufferToViewPosition(FGTextBuffer.CaretPos position, out int row, out int column)
	{
		List<int> softLineBreaks = GetSoftLineBreaks(position.line);
		row = FindFirstIndexGreaterThanOrEqualTo<int>(softLineBreaks, position.column);
		if (row < softLineBreaks.Count && position.column == softLineBreaks[row] && position.virtualColumn == 0)
			++row;

		int rowStart = row > 0 ? softLineBreaks[row - 1] : 0;
		//int rowLength = (row < softLineBreaks.Count ? softLineBreaks[row] : FGTextBuffer.ExpandTabs(textBuffer.lines[position.line]).Length) - rowStart;
		column = position.column - rowStart;
//		if (column > rowLength)
//			column = rowLength;
	}

	public FGTextBuffer.CaretPos ViewToBufferPosition(int line, int row, int column)
	{
		if (line >= textBuffer.formatedLines.Length)
			line = textBuffer.formatedLines.Length - 1;
		if (line < 0)
			line = 0;
		if (row < 0)
			row = 0;

		FGTextBuffer.CaretPos position = new FGTextBuffer.CaretPos { column = column, line = line, virtualColumn = column };

		List<int> softLineBreaks = GetSoftLineBreaks(line);
		if (row > softLineBreaks.Count)
			row = softLineBreaks.Count;
		position.column += row > 0 ? softLineBreaks[row - 1] : 0;
		if (row < softLineBreaks.Count && position.column > softLineBreaks[row])
			position.column = softLineBreaks[row];
		position.characterIndex = textBuffer.ColumnToCharIndex(ref position.column, line);
		return position;
	}

	public FGTextBuffer.CaretPos GetLinesOffset(FGTextBuffer.CaretPos position, int linesDown)
	{
		FGTextBuffer.CaretPos pos = position.Clone();

		if (!wordWrapping)
		{
			pos.line += linesDown;
			if (pos.line < 0)
				pos.line = 0;
			if (pos.line >= textBuffer.lines.Count)
				pos.line = textBuffer.lines.Count - 1;
		}
		else
		{
			List<int> softLineBreaks = GetSoftLineBreaks(pos.line);
			int softRow = FindFirstIndexGreaterThanOrEqualTo<int>(softLineBreaks, pos.column);
			if (softRow < softLineBreaks.Count && pos.column == softLineBreaks[softRow] && pos.virtualColumn == 0)
				++softRow;
			int numSoftRows = softLineBreaks.Count + 1;

			while (linesDown > 0)
			{
				--linesDown;
				if (softRow < numSoftRows - 1)
				{
					++softRow;
				}
				else
				{
					if (pos.line == textBuffer.lines.Count - 1)
						break;
					++pos.line;
					softRow = 0;
					softLineBreaks = GetSoftLineBreaks(pos.line);
					numSoftRows = softLineBreaks.Count + 1;
				}
			}
			while (linesDown < 0)
			{
				++linesDown;
				if (softRow > 0)
				{
					--softRow;
				}
				else
				{
					if (pos.line == 0)
						break;
					--pos.line;
					softLineBreaks = GetSoftLineBreaks(pos.line);
					numSoftRows = softLineBreaks.Count + 1;
					softRow = numSoftRows - 1;
				}
			}

			int rowStart = softRow > 0 ? softLineBreaks[softRow - 1] : 0;
			int rowEnd = softRow < softLineBreaks.Count ? softLineBreaks[softRow] : FGTextBuffer.ExpandTabs(textBuffer.lines[pos.line]).Length;
			pos.column = rowStart + pos.virtualColumn;
			if (pos.column > rowEnd)
				pos.column = rowEnd;
		}

		pos.characterIndex = textBuffer.ColumnToCharIndex(ref pos.column, pos.line);
		return pos;
	}

	private void ProcessEditorKeyboard(Event current)
	{
		if (!CanEdit())
			return;
		
		bool isOSX = Application.platform == RuntimePlatform.OSXEditor;
		EventModifiers modifiers = current.modifiers;

		if (isOSX && current.type == EventType.keyDown)
		{
			if (current.keyCode == KeyCode.Z)
			{
				if (modifiers == EventModifiers.Control)
				{
					Undo();
					current.Use();
					return;
				}
				else if (modifiers == (EventModifiers.Control | EventModifiers.Shift))
				{
					Redo();
					current.Use();
					return;
				}
			}
		}

		if (EditorGUI.actionKey && current.type == EventType.keyDown)
		{
			EventModifiers mods = modifiers & ~(EventModifiers.Control | EventModifiers.Command);

			/*if (current.keyCode == KeyCode.Space && mods == 0)
			{
				Autocomplete();
				current.Use();
				return;
			}
			else*/ if (current.keyCode == KeyCode.T && mods == 0)
			{
				current.Use();
				OpenInNewTab();
				return;
			}
			else if ((current.keyCode == KeyCode.K || current.keyCode == KeyCode.Slash) && mods == 0)
			{
				ToggleCommentSelection();
				current.Use();
				return;
			}
			else if (!isOSX && current.keyCode == KeyCode.Z && mods == EventModifiers.Shift)
			{
				Undo();
				current.Use();
				return;
			}
			else if (!isOSX && current.keyCode == KeyCode.Y && mods == EventModifiers.Shift)
			{
				Redo();
				current.Use();
				return;
			}
			else if (current.keyCode == KeyCode.S && mods == EventModifiers.Alt)
			{
				current.Use();
				SaveBuffer();
				return;
			}
			else if (current.keyCode == KeyCode.Minus || current.keyCode == KeyCode.KeypadMinus)
			{
				current.Use();
				ModifyFontSize(-1);
				return;
			}
			else if (current.keyCode == KeyCode.Plus || current.keyCode == KeyCode.Equals || current.keyCode == KeyCode.KeypadPlus)
			{
				current.Use();
				ModifyFontSize(1);
				return;
			}
		}

		int nextCharacterIndex = caretPosition.characterIndex;
		int nextCaretColumn = caretPosition.virtualColumn;
		int nextCaretLine = caretPosition.line;
		bool isHorizontal = false;
		bool clearSelection = false;

		if (current.type == EventType.keyDown)
		{
			switch (current.keyCode)
			{
				case KeyCode.Escape:
					/*if (autocompleteWindow != null)
					{
						CloseAutocomplete();
						current.Use();
					}
					else*/ if (selectionStartPosition != null)
					{
						clearSelection = true;
						current.Use();
					}
					break;

				case KeyCode.Return:
				case KeyCode.KeypadEnter:
					if (EditorGUI.actionKey && EditorWindow.focusedWindow != null)
					{
						//current.Use();
						//OpenAtCursor();
						//EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("OpenAtCursor"));
						//GUIUtility.ExitGUI();
						return;
					}
					break;

				case KeyCode.PageUp:
					if (EditorGUI.actionKey)
					{
						nextCaretLine = 0;
						nextCaretColumn = 0;
					}
					else
					{
						FGTextBuffer.CaretPos nextCaretPos = GetLinesOffset(caretPosition, -(int) (codeViewRect.height / charSize.y));
						nextCaretLine = nextCaretPos.line;
						nextCaretColumn = wordWrapping ? nextCaretPos.column : nextCaretPos.virtualColumn;
						nextCharacterIndex = nextCaretPos.characterIndex;
					}
					current.Use();
					break;

				case KeyCode.PageDown:
					if (EditorGUI.actionKey)
					{
						nextCaretLine = textBuffer.lines.Count - 1;
						nextCaretColumn = textBuffer.lines[nextCaretLine].Length;
					}
					else
					{
						FGTextBuffer.CaretPos nextCaretPos = GetLinesOffset(caretPosition, (int)(codeViewRect.height / charSize.y));
						nextCaretLine = nextCaretPos.line;
						nextCaretColumn = wordWrapping ? nextCaretPos.column : nextCaretPos.virtualColumn;
						nextCharacterIndex = nextCaretPos.characterIndex;
					}
					current.Use();
					break;

				case KeyCode.Home:
					if (EditorGUI.actionKey)
					{
						nextCaretLine = 0;
						nextCaretColumn = 0;
					}
					else
					{
						isHorizontal = true;

						int firstNonWhitespace = textBuffer.FirstNonWhitespace(nextCaretLine);
						if (firstNonWhitespace == textBuffer.lines[nextCaretLine].Length)
							firstNonWhitespace = 0;
						if (nextCharacterIndex == firstNonWhitespace)
							nextCharacterIndex = 0;
						else
							nextCharacterIndex = firstNonWhitespace;
					}
					current.Use();
					break;

				case KeyCode.End:
					if (EditorGUI.actionKey)
					{
						nextCaretLine = textBuffer.lines.Count - 1;
						nextCaretColumn = textBuffer.lines[nextCaretLine].Length;
					}
					else
					{
						isHorizontal = true;

						if (!current.shift && selectionStartPosition != null)
						{
							if (selectionStartPosition.line > nextCaretLine)
								nextCaretLine = selectionStartPosition.line;
						}
						nextCharacterIndex = textBuffer.lines[nextCaretLine].Length;
					}
					current.Use();
					break;

				case KeyCode.UpArrow:
					if (!EditorGUI.actionKey)
					{
						if (!current.shift && selectionStartPosition != null && selectionStartPosition.line < nextCaretLine)
							//nextCaretLine = selectionStartPosition.line;
							caretPosition = selectionStartPosition.Clone();
						
						//--nextCaretLine;
						FGTextBuffer.CaretPos nextCaretPos = GetLinesOffset(caretPosition, -1);
						nextCaretLine = nextCaretPos.line;
						nextCaretColumn = wordWrapping ? nextCaretPos.column : nextCaretPos.virtualColumn;
						nextCharacterIndex = nextCaretPos.characterIndex;
					}
					else if (!current.shift)
					{
						scrollPosition.y -= charSize.y;
						if (scrollPosition.y < 0f)
							scrollPosition.y = 0f;
						scrollPositionLine = GetLineAt(scrollPosition.y);
						scrollPositionOffset = scrollPosition.y - GetLineOffset(scrollPositionLine);
						//Repaint();
						needsRepaint = true;
						current.Use();
						return;
					}
					else
					{
						UseSelectionForSearch();
						SearchPrevious();
						current.Use();
						focusCodeView = true;
						return;
					}
					current.Use();
					break;

				case KeyCode.DownArrow:
					if (!EditorGUI.actionKey)
					{
						if (!current.shift && selectionStartPosition != null && selectionStartPosition.line > nextCaretLine)
							//nextCaretLine = selectionStartPosition.line;
							caretPosition = selectionStartPosition.Clone();
						
						//++nextCaretLine;
						FGTextBuffer.CaretPos nextCaretPos = GetLinesOffset(caretPosition, 1);
						nextCaretLine = nextCaretPos.line;
						nextCaretColumn = wordWrapping ? nextCaretPos.column : nextCaretPos.virtualColumn;
						nextCharacterIndex = nextCaretPos.characterIndex;
					}
					else if (!current.shift)
					{
						scrollPosition.y += charSize.y;
						scrollPositionLine = GetLineAt(scrollPosition.y);
						scrollPositionOffset = scrollPosition.y - GetLineOffset(scrollPositionLine);
						//Repaint();
						needsRepaint = true;
						current.Use();
						return;
					}
					else
					{
						UseSelectionForSearch();
						SearchNext();
						current.Use();
						focusCodeView = true;
						return;
					}
					current.Use();
					break;

				case KeyCode.LeftArrow:
					isHorizontal = true;

					if (!current.shift && !EditorGUI.actionKey && selectionStartPosition != null)
					{
						if (selectionStartPosition < caretPosition)
						{
							nextCaretLine = selectionStartPosition.line;
							nextCaretColumn = selectionStartPosition.column;
							nextCharacterIndex = selectionStartPosition.characterIndex;
						}
						else
						{
							clearSelection = true;
						}
						current.Use();
						break;
					}

					if (EditorGUI.actionKey)
					{
						FGTextBuffer.CaretPos nextPos = textBuffer.WordStopLeft(caretPosition);
						nextCaretLine = nextPos.line;
						nextCaretColumn = nextPos.column;
						nextCharacterIndex = nextPos.characterIndex;
						current.Use();
						break;
					}

					--nextCharacterIndex;
					if (nextCharacterIndex < 0)
					{
						if (--nextCaretLine >= 0)
						{
							nextCharacterIndex = textBuffer.lines[nextCaretLine].Length;
						}
						else
						{
							nextCaretLine = 0;
							nextCharacterIndex = 0;
						}
					}
					current.Use();
					break;

				case KeyCode.RightArrow:
					isHorizontal = true;

					if (!current.shift && !EditorGUI.actionKey && selectionStartPosition != null)
					{
						if (selectionStartPosition > caretPosition)
						{
							nextCaretLine = selectionStartPosition.line;
							nextCaretColumn = selectionStartPosition.column;
							nextCharacterIndex = selectionStartPosition.characterIndex;
						}
						else
						{
							clearSelection = true;
						}
						current.Use();
						break;
					}

					if (EditorGUI.actionKey)
					{
						FGTextBuffer.CaretPos nextPos = textBuffer.WordStopRight(caretPosition);
						nextCaretLine = nextPos.line;
						nextCaretColumn = nextPos.column;
						nextCharacterIndex = nextPos.characterIndex;
						current.Use();
						break;
					}

					if (nextCaretLine >= 0)
					{
						++nextCharacterIndex;
						if (nextCharacterIndex > textBuffer.lines[nextCaretLine].Length)
						{
							if (++nextCaretLine < textBuffer.numParsedLines)
							{
								nextCharacterIndex = 0;
							}
							else
							{
								--nextCaretLine;
								--nextCharacterIndex;
							}
						}
					}
					current.Use();
					break;
			}
		}

		if (Event.current.shift && Event.current.keyCode == KeyCode.Space)
			Event.current.Use();

		if (current.type == EventType.keyDown)
		{
			if (modifiers == EventModifiers.Control && current.character == ' ')
			{
				current.Use();
				return;
			}
			else if (EditorGUI.actionKey && current.character == '\n' && !current.shift && !current.alt)
			{
				if (EditorWindow.focusedWindow != null)
					OpenAtCursor();
				current.Use();
				return;
			}
			else if ((current.character == '\t' || current.character == 25) && (modifiers & ~EventModifiers.Shift) == 0)
			{
				if (modifiers == EventModifiers.Shift)
					IndentLess();
				else
					IndentMoreOrInsertTab();

				focusCodeView = true;
				caretMoveTime = Time.realtimeSinceStartup;
				needsRepaint = true;
				current.Use();
				return;
			}
			else if ((current.keyCode == KeyCode.LeftBracket || current.keyCode == KeyCode.RightBracket) && EditorGUI.actionKey)
			{
				if (current.keyCode == KeyCode.LeftBracket)
					IndentLess();
				else
					IndentMore();
				
                focusCodeView = true;
                caretMoveTime = Time.realtimeSinceStartup;
                needsRepaint = true;
                current.Use();
                return;
			}
			else if ((current.character >= ' ' || current.character == '\n')
				&& (!EditorGUI.actionKey || (modifiers & EventModifiers.Command) == 0 && current.keyCode == KeyCode.None))
			{
				string text = current.character.ToString();
				if (current.character == '\n')
				{
					// Keep the same indent level for newly inserted lines
					int firstNonWhitespace = textBuffer.FirstNonWhitespace(caretPosition.line);
					if (firstNonWhitespace > caretPosition.characterIndex)
						firstNonWhitespace = caretPosition.characterIndex;
					text += textBuffer.lines[caretPosition.line].Substring(0, firstNonWhitespace);
				}

				FGTextBuffer.CaretPos newCaretPosition = caretPosition.Clone();
				if (selectionStartPosition != null)
				{
					newCaretPosition = textBuffer.DeleteText(selectionStartPosition, caretPosition);
					clearSelection = true;
				}

				int insertedAtLine = newCaretPosition.line;

				newCaretPosition = textBuffer.InsertText(newCaretPosition, text);
				nextCharacterIndex = newCaretPosition.characterIndex;
				nextCaretColumn = newCaretPosition.column;
				nextCaretLine = newCaretPosition.line;
				isHorizontal = true;

				textBuffer.UpdateHighlighting(insertedAtLine, newCaretPosition.line);

				modifiers &= ~EventModifiers.Shift;
				current.Use();
			}
			else if (current.keyCode == KeyCode.Delete && modifiers == EventModifiers.Shift && selectionStartPosition == null)
			{
				modifiers = 0;

				if (caretPosition.line == textBuffer.numParsedLines - 1)
				{
					textBuffer.lines[caretPosition.line] = string.Empty;
					nextCharacterIndex = 0;
					nextCaretColumn = 0;
					//isHorizontal = true;
				}
				else
				{
					textBuffer.DeleteText(new FGTextBuffer.CaretPos { characterIndex = 0, column = 0, virtualColumn = 0, line = caretPosition.line },
						new FGTextBuffer.CaretPos { characterIndex = 0, column = 0, virtualColumn = 0, line = caretPosition.line + 1 });
					nextCaretColumn = caretPosition.column;
					nextCharacterIndex = textBuffer.ColumnToCharIndex(ref nextCaretColumn, caretPosition.line);
				}

				clearSelection = true;
				textBuffer.UpdateHighlighting(caretPosition.line, caretPosition.line);
				current.Use();
			}
			else if (current.keyCode == KeyCode.Backspace || current.keyCode == KeyCode.Delete)
			{
				modifiers &= ~EventModifiers.Shift;

				FGTextBuffer.CaretPos newCaretPosition = caretPosition.Clone();
				if (selectionStartPosition == null)
				{
					Event simKey = new Event(current);
					if (current.keyCode == KeyCode.Delete)
						simKey.keyCode = KeyCode.RightArrow;
					else
						simKey.keyCode = KeyCode.LeftArrow;
					simKey.modifiers |= EventModifiers.Shift;
					ProcessEditorKeyboard(simKey);
				}

				modifiers &= ~EventModifiers.Shift;

				if (selectionStartPosition != null)
				{
					newCaretPosition = textBuffer.DeleteText(selectionStartPosition, caretPosition);

					nextCharacterIndex = newCaretPosition.characterIndex;
					nextCaretColumn = newCaretPosition.column;
					nextCaretLine = newCaretPosition.line;
					isHorizontal = true;
					clearSelection = true;

					textBuffer.UpdateHighlighting(newCaretPosition.line, newCaretPosition.line);
					current.Use();
				}
			}
		}

		//if (clearSelection ||
		//    isHorizontal && nextCharacterIndex != caretPosition.characterIndex ||
		//    nextCaretColumn != caretPosition.virtualColumn ||
		//    nextCaretLine != caretPosition.line)
		if (current.type == EventType.Used)
		{
			caretMoveTime = Time.realtimeSinceStartup;

			if (selectionStartPosition == null && current.shift)
			{
				selectionStartPosition = new FGTextBuffer.CaretPos
				{
					column = caretPosition.column,
					line = caretPosition.line,
					virtualColumn = caretPosition.column,
					characterIndex = caretPosition.characterIndex
				};
				if (!isHorizontal && nextCaretLine != caretPosition.line)
					nextCaretColumn = caretPosition.column;
			}

			if (nextCaretLine < 0)
				nextCaretLine = 0;
			if (nextCaretLine >= textBuffer.numParsedLines)
				nextCaretLine = textBuffer.numParsedLines - 1;

			//caretPosition.virtualColumn = nextCaretColumn;
			if (isHorizontal)
			{
				caretPosition.virtualColumn = nextCaretColumn = textBuffer.CharIndexToColumn(nextCharacterIndex, nextCaretLine);
				if (wordWrapping)
				{
					List<int> softLineBreaks = GetSoftLineBreaks(nextCaretLine);
					int softRow = FindFirstIndexGreaterThanOrEqualTo<int>(softLineBreaks, nextCaretColumn);
					if (softRow < softLineBreaks.Count && nextCaretColumn == softLineBreaks[softRow])
						++softRow;
					caretPosition.virtualColumn -= softRow > 0 ? softLineBreaks[softRow - 1] : 0;
				}
			}
			else
			{
				nextCharacterIndex = textBuffer.ColumnToCharIndex(ref nextCaretColumn, nextCaretLine);
			}
			caretPosition.column = nextCaretColumn;
			caretPosition.characterIndex = nextCharacterIndex;
			caretPosition.line = nextCaretLine;

			if (!isHorizontal && nextCaretLine >= 0)
			{
				caretPosition.characterIndex = Math.Min(nextCharacterIndex, textBuffer.lines[nextCaretLine].Length);
				caretPosition.column = textBuffer.CharIndexToColumn(caretPosition.characterIndex, nextCaretLine);
			}

			if (clearSelection || selectionStartPosition != null && ((modifiers & EventModifiers.Shift) == 0 || selectionStartPosition == caretPosition))
				selectionStartPosition = null;

			scrollToCaret = true;
			//Repaint();
			needsRepaint = true;
		}
	}

	private void DrawPing(float indent, bool bgOnly)
	{
		if (styles.ping == null)
			return;

		float t = (1f - pingTimer) * 64f;
		if (t > 0f && t < 64f)
		{
			scrollToRect.x += indent;

			GUIStyle ping = styles.ping;
			int left = ping.padding.left;
			ping.padding.left = 0;

			Color oldColor = GUI.color;
			Color oldBgColor = GUI.backgroundColor;
			if (t > 4f)
			{
				if (!bgOnly)
					GUI.backgroundColor = new Color(oldBgColor.r, oldBgColor.g, oldBgColor.b, oldBgColor.a * (8f - t) * 0.125f);
				if (t > 56f)
					GUI.color = new Color(oldColor.r, oldColor.g, oldColor.b, oldColor.a * (64f - t) * 0.125f);
			}

			Matrix4x4 matrix = GUI.matrix;
			if (t < 4f)
			{
				float scale = 2f - Mathf.Abs(1f - t * 0.5f);
				Vector2 pos = scrollToRect.center;
				GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), pos);
			}
			ping.Draw(scrollToRect, bgOnly ? GUIContent.none : pingContent, false, false, false, false);
			GUI.matrix = matrix;

			ping.padding.left = left;
			GUI.color = oldColor;
			GUI.backgroundColor = oldBgColor;
			scrollToRect.x -= indent;
		}
	}

	// "Links" drop-down menu items handler
	private void FollowHyperlink(object hyperlink)
	{
		Application.OpenURL((string) hyperlink);
	}

	private bool CanUndo()
	{
		return CanEdit() && textBuffer.CanUndo();
	}

	private bool CanRedo()
	{
		return CanEdit() && textBuffer.CanRedo();
	}

	private void Undo()
	{
		textBuffer.Undo();
	}

	private void Redo()
	{
		textBuffer.Redo();
	}

	private void ToggleCommentSelection()
	{
		textBuffer.BeginEdit("Toggle Comment");

		FGTextBuffer.CaretPos from = caretPosition.Clone();
		FGTextBuffer.CaretPos to = caretPosition.Clone();
		int fromLine = caretPosition.line;
		int toLine = caretPosition.line;

		if (selectionStartPosition != null)
		{
			if (caretPosition < selectionStartPosition)
			{
				to = selectionStartPosition.Clone();
				toLine = to.line;
			}
			else
			{
				from = selectionStartPosition.Clone();
				fromLine = from.line;
		    }
		    if (to.characterIndex == 0)
		        --toLine;
		}
		
		int leftmostNWS = int.MaxValue;
		int[] fnws = new int[toLine - fromLine + 1];
		for (int i = 0; i < fnws.Length; ++i)
		{
			fnws[i] = textBuffer.FirstNonWhitespace(fromLine + i);
			if (fnws[i] < textBuffer.lines[fromLine + i].Length)
			{
				int column = textBuffer.CharIndexToColumn(fnws[i], fromLine + i);
				if (column < leftmostNWS)
					leftmostNWS = column;
			}
		}
		if (leftmostNWS == int.MaxValue)
		{
			textBuffer.EndEdit();
			return; // No code is selected, nothing to comment out
		}

		bool allCommentedOut = true;
		for (int i = 0; i < fnws.Length; ++i)
		{
			int lineLength = textBuffer.lines[fromLine + i].Length;
			if (fnws[i] < lineLength)
			{
				int index = textBuffer.ColumnToCharIndex(ref leftmostNWS, fromLine + i);
				if (textBuffer.isBooFile ? textBuffer.lines[fromLine + i][index] != '#' : textBuffer.lines[fromLine + i][index] != '/'
					|| index + 1 >= lineLength || textBuffer.lines[fromLine + i][index + 1] != '/')
				{
					allCommentedOut = false;
					break;
				}
			}
		}

		bool moveFromPos = fnws[0] <= from.characterIndex;
		bool moveToPos = to.line == toLine && fnws[fnws.Length - 1] <= to.characterIndex;
		if (allCommentedOut)
		{
			for (FGTextBuffer.CaretPos i = new FGTextBuffer.CaretPos { characterIndex = 0, column = leftmostNWS, line = fromLine, virtualColumn = leftmostNWS }; i.line <= toLine; ++i.line)
			{
				if (fnws[i.line - fromLine] < textBuffer.lines[i.line].Length)
				{
					i.characterIndex = textBuffer.ColumnToCharIndex(ref leftmostNWS, i.line);
					FGTextBuffer.CaretPos j = i.Clone();
					j.column = j.virtualColumn += textBuffer.isBooFile ? 1 : 2;
					j.characterIndex += textBuffer.isBooFile ? 1 : 2;
					textBuffer.DeleteText(i, j);
				}
			}
		}
		else
		{
			for (FGTextBuffer.CaretPos i = new FGTextBuffer.CaretPos { characterIndex = 0, column = leftmostNWS, line = fromLine, virtualColumn = leftmostNWS }; i.line <= toLine; ++i.line)
			{
				if (fnws[i.line - fromLine] < textBuffer.lines[i.line].Length)
				{
					i.characterIndex = textBuffer.ColumnToCharIndex(ref leftmostNWS, i.line);
					textBuffer.InsertText(i, textBuffer.isBooFile ? "#" : "//");
				}
			}
		}
		for (int i = 0; i < fnws.Length; ++i)
			if (fnws[i] < textBuffer.lines[fromLine + i].Length)
				textBuffer.UpdateHighlighting(fromLine + i, fromLine + i);

		if (moveFromPos)
			from.characterIndex += textBuffer.isBooFile ? (allCommentedOut ? -1 : 1) : (allCommentedOut ? -2 : 2);
		if (moveToPos)
			to.characterIndex += textBuffer.isBooFile ? (allCommentedOut ? -1 : 1) : (allCommentedOut ? -2 : 2);
		from.column = from.virtualColumn = textBuffer.CharIndexToColumn(from.characterIndex, from.line);
		to.column = to.virtualColumn = textBuffer.CharIndexToColumn(to.characterIndex, to.line);

		if (selectionStartPosition != null)
		{
			if (caretPosition < selectionStartPosition)
			{
				caretPosition = from;
				selectionStartPosition = to;
			}
			else
			{
				selectionStartPosition = from;
				caretPosition = to;
			}
		}
		else
		{
			caretPosition = from;
		}
		caretMoveTime = Time.realtimeSinceStartup;
		scrollToCaret = true;

		textBuffer.EndEdit();
	}

	public void PingLine(int line)
	{
		if (line >= textBuffer.lines.Count)
			line = textBuffer.lines.Count - 1;
		
		int fnws = textBuffer.FirstNonWhitespace(line - 1);
		int fromColumn = textBuffer.CharIndexToColumn(fnws, line - 1);
		string expanded = FGTextBuffer.ExpandTabs(textBuffer.lines[line - 1]);
		
		pingContent.text = expanded.Trim();
		if (!string.IsNullOrEmpty(pingContent.text))
		{
			int toColumn = expanded.Length;
			caretPosition = new FGTextBuffer.CaretPos { line = line - 1, column = fromColumn, virtualColumn = fromColumn, characterIndex = fnws };
			selectionStartPosition = new FGTextBuffer.CaretPos { line = line - 1, column = toColumn, virtualColumn = toColumn, characterIndex = textBuffer.lines[line - 1].Length };
		}
		else
		{
			caretPosition = new FGTextBuffer.CaretPos { line = line - 1, column = 0, virtualColumn = 0, characterIndex = 0 };
			selectionStartPosition = new FGTextBuffer.CaretPos { line = line, column = 0, virtualColumn = 0, characterIndex = 0 };
		}
		//ValidateCarets();

		pingTimer = 1f;
		scrollToRect.x = charSize.x * fromColumn;
		scrollToRect.y = GetLineOffset(caretPosition.line);
		scrollToRect.width = charSize.x * pingContent.text.Length;
		scrollToRect.height = charSize.y;

		caretMoveTime = Time.realtimeSinceStartup;
		Repaint();
	}

	private void DoToolbar()
	{
		Rect toolbarRect = new Rect(scrollViewRect.xMin, scrollViewRect.yMin - 18f, scrollViewRect.width, 17f);
		GUI.Box(toolbarRect, GUIContent.none, EditorStyles.toolbar);

		bool isOSX = Application.platform == RuntimePlatform.OSXEditor;

		string targetName = System.IO.Path.GetFileName(AssetDatabase.GUIDToAssetPath(targetGuid));
		
		Color oldColor = GUI.color;
		GUI.contentColor = EditorStyles.toolbarButton.normal.textColor;

		GUIContent popOut = new GUIContent(popOutIcon, "New Tab" + (isOSX ? " ⌘T" : "\n(Ctrl+T)"));
		Vector2 contentSize = EditorStyles.toolbarButton.CalcSize(popOut);
		Rect rc = new Rect(toolbarRect.xMin + 6f, toolbarRect.yMin, contentSize.x, contentSize.y);
		if (GUI.Button(rc, popOut, EditorStyles.toolbarButton))
		{
			OpenInNewTab();
			return;
		}
		
		GUI.enabled = CanEdit() && IsModified;
		contentSize = EditorStyles.toolbarButton.CalcSize(new GUIContent(saveIcon));
		rc.Set(rc.xMax + 6f, toolbarRect.yMin, contentSize.x, contentSize.y);
		//if (Event.current.type != EventType.Repaint)
			if (GUI.Button(rc, new GUIContent(saveIcon, "Save " + targetName + (isOSX ? " ⌥⌘S" : "\n(Ctrl+Alt+S)")), EditorStyles.toolbarButton))
				SaveBuffer();
		/* WIP Code
		GUIStyle splitButton = new GUIStyle(EditorStyles.toolbarDropDown);
		splitButton.padding.right -= 4;
		splitButton.padding.left -= 2;
		splitButton.contentOffset = new Vector2(4f, 0f);
		contentSize = splitButton.CalcSize(GUIContent.none);
		contentSize.x -= 5f;
		Rect rc2 = new Rect(rc.xMax + 1f, toolbarRect.yMin, contentSize.x, contentSize.y);
		GUI.enabled = CanEdit();

		if (GUI.Button(rc2, GUIContent.none, splitButton))
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Save " + targetName + " %&s"), false, SaveScript);
			menu.AddItem(new GUIContent("Save as"), false, SaveScript);
			menu.AddSeparator(string.Empty);
			menu.AddItem(new GUIContent("Save All"), false, () => FGTextBufferManager.SaveAllModified(false));
			menu.DropDown(rc);
		}
		if (Event.current.type == EventType.Repaint)
		{
			GUI.enabled = CanEdit();
			if (GUI.Button(rc, new GUIContent(saveIcon, "Save " + targetName + (isOSX ? " ⌥⌘S" : "\n(Ctrl+Alt+S)")), EditorStyles.toolbarButton))
				SaveScript();
		}
		*/

		GUI.enabled = CanUndo();
		contentSize = EditorStyles.toolbarButton.CalcSize(new GUIContent(undoIcon));
		rc = new Rect(rc.xMax + 6f, toolbarRect.yMin, contentSize.x, contentSize.y);
		if (GUI.Button(rc, new GUIContent(undoIcon, "Undo" + (isOSX ? " ⌃Z" : "\n(Ctrl+Shift+Z)")), EditorStyles.toolbarButton))
		{
			focusCodeView = true;
			Undo();
		}

		GUI.enabled = CanRedo();
		contentSize = EditorStyles.toolbarButton.CalcSize(new GUIContent(redoIcon));
		rc = new Rect(rc.xMax, toolbarRect.yMin, contentSize.x, contentSize.y);
		if (GUI.Button(rc, new GUIContent(redoIcon, "Redo" + (isOSX ? " ⇧⌃Z" : "\n(Ctrl+Shift+Y)")), EditorStyles.toolbarButton))
		{
			focusCodeView = true;
			Redo();
		}

		if (textBuffer != null && !textBuffer.isText)
		{
			GUI.enabled = CanEdit();
			contentSize = EditorStyles.toolbarButton.CalcSize(new GUIContent(textBuffer.isBooFile ? "#..." : "//..."));
			rc = new Rect(rc.xMax, toolbarRect.yMin, contentSize.x, contentSize.y);
			if (GUI.Button(rc, new GUIContent(textBuffer.isBooFile ? "#..." : "//...", "Toggle Comment Selection\n" + (isOSX ? "⌘K or ⌘/" : "(Ctrl+K or Ctrl+/)")), EditorStyles.toolbarButton))
			{
				focusCodeView = true;
				ToggleCommentSelection();
			}
		}

		GUI.enabled = textBuffer.hyperlinks.Length > 0;
		GUIContent links = new GUIContent(textBuffer.hyperlinks.Length.ToString(), hyperlinksIcon);

		contentSize = EditorStyles.toolbarDropDown.CalcSize(links);
		rc.Set(rc.xMax + 6f, toolbarRect.yMin, contentSize.x, EditorStyles.toolbar.fixedHeight);
		if (GUI.Button(rc, links, EditorStyles.toolbarDropDown))
		{
			GenericMenu menu = new GenericMenu();
			foreach (string hyperlink in textBuffer.hyperlinks)
			{
				if (hyperlink.StartsWith("mailto:"))
				{
					menu.AddItem(new GUIContent(hyperlink.Substring(7)), false, FollowHyperlink, hyperlink);
				}
				else
				{
					// Shortenning the URL since we cannot display slashes in the menu item,
					// so in most cases we'll end up with something like www.flipbookgames.com...
					// The first two or three slashes and the last one will be trimmed too.
					string escaped = hyperlink.Substring(hyperlink.IndexOf(':') + 1);

					// Unity cannot display a slash in menu items. Replacing any remaining slashes with
					// the best alternative - backslash
					escaped = escaped.Replace('/', '\\');

					// On Windows the '&' has special meaning, unless it's escaped with another '&'
					if (Application.platform == RuntimePlatform.WindowsEditor)
					{
						int index = 0;
						while (index < escaped.Length && (index = escaped.IndexOf('&', index + 1)) >= 0)
							escaped = escaped.Insert(index++, "&");
					}

					menu.AddItem(new GUIContent(escaped.Trim('\\')), false, FollowHyperlink, hyperlink);
				}
			}
			menu.DropDown(rc);
		}

		GUI.contentColor = oldColor;
		GUI.enabled = false;

		rc.xMin = rc.xMax + 8f;
		rc.xMax = toolbarRect.xMax - 25f;
		string infoText = EditorApplication.isCompiling ? "Compiling..."
			: textBuffer.IsLoading ? "Loading..."
			: textBuffer.justSavedNow ? "Saved..."
			: textBuffer.fileEncoding.EncodingName;
		GUI.Label(rc, infoText);

		GUI.enabled = CanEdit();

		rc.y += 2f;
		rc.height = 16f;
		if (rc.width > 231f)
			rc.xMin = rc.xMax - 231f;
		DoSearchBox(rc);
		
		// Only redrawing the default wrench icon after being covered with our toolbar.
		// The defult icon still handles the functionality in the Inspector tab.
		if (wrenchIcon != null)
			GUI.Label(new Rect(toolbarRect.xMax - 20f, toolbarRect.yMin + 2f, 15f, 14f), wrenchIcon, GUIStyle.none);

		if (Event.current.type == EventType.ContextClick && toolbarRect.Contains(Event.current.mousePosition))
			Event.current.Use();

		GUI.enabled = true;
	}

	private void OpenInNewTab()
	{
		if ((textBuffer.isShader || textBuffer.isText) && EditorWindow.focusedWindow.GetType() != typeof(FGCodeWindow))
			FGCodeWindow.OpenNewWindow();
		else
			EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("ScriptInspector.AddTab"));
		GUIUtility.ExitGUI();
	}

	private void DoSearchBox(Rect position)
	{
		if (Event.current.type == EventType.ValidateCommand)
		{
			if (Event.current.commandName == "Find")
			{
				Event.current.Use();
				return;
			}
		}
		else if (Event.current.type == EventType.ExecuteCommand)
		{
			if (Event.current.commandName == "Find")
			{
				if (hasCodeViewFocus)
					UseSelectionForSearch();
				focusSearchBox = true;
				Event.current.Use();
			}
		}

		if (textBuffer.undoPosition != searchResultAge)
			currentSearchResult = -1;

		if ((Event.current.type == EventType.MouseDown) && position.Contains(Event.current.mousePosition))
		{
			focusSearchBox = true;
		}

		GUI.SetNextControlName("SearchBox");
		if (focusSearchBox)
		{
			GUI.FocusControl("SearchBox");
#if UNITY_4_3
			EditorGUI.FocusTextInControl("SearchBox");
#endif
			if (Event.current.type == EventType.Repaint)
			{
				focusSearchBox = false;
			}
		}
		hasSearchBoxFocus = GUI.GetNameOfFocusedControl() == "SearchBox";

		if (Event.current.type == EventType.KeyDown && !Event.current.alt)
		{
			if (Event.current.keyCode == KeyCode.F3 && !EditorGUI.actionKey ||
				Event.current.keyCode == KeyCode.G && EditorGUI.actionKey)
			{
				if (Event.current.shift)
					SearchPrevious();
				else
					SearchNext();
				
				Event.current.Use();
			}
		}

		if (focusCodeViewOnEscapeUp && Event.current.rawType == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape)
		{
			focusCodeViewOnEscapeUp = false;
			focusCodeView = true;
			Event.current.Use();
		}

		if (hasSearchBoxFocus && Event.current.type == EventType.KeyDown)
		{
			if (Event.current.keyCode == KeyCode.Escape)
			{
				searchString = string.Empty;
				SetSearchText(searchString);
				focusCodeViewOnEscapeUp = true;
				Event.current.Use();
			}
			else if (Event.current.keyCode == KeyCode.UpArrow)
			{
				SearchPrevious();
				Event.current.Use();
			}
			else if (Event.current.keyCode == KeyCode.DownArrow)
			{
				SearchNext();
				Event.current.Use();
			}
			else if (Event.current.character == '\n')
			{
				currentSearchResult = currentSearchResult < 0 ? 0 :
					currentSearchResult < searchResults.Count ? currentSearchResult : searchResults.Count - 1;
				ShowSearchResult(currentSearchResult);
				focusCodeView = true;
				Event.current.Use();
			}
		}

		string text = ToolbarSearchField(position, searchString);
		
		if (searchString != text)
		{
			searchString = text;
			SetSearchText(searchString);
			hasSearchBoxFocus = true;
		}
	}

	private void SetSearchText(string text)
	{
		searchResultAge = textBuffer.undoPosition;
		defaultSearchString = text;

		searchResults.Clear();
		currentSearchResult = -1;
		int textLength = text.Length;

		if (textLength == 0)
		{
			Repaint();
			return;
		}

		int i = 0;
		foreach (string line in textBuffer.lines)
		{
			for (int pos = 0; (pos = line.IndexOf(text, pos, StringComparison.InvariantCultureIgnoreCase)) != -1; pos += textLength )
			{
				int columnFrom = textBuffer.CharIndexToColumn(pos, i);
				FGTextBuffer.CaretPos caretPos = new FGTextBuffer.CaretPos { line = i, characterIndex = pos, column = columnFrom, virtualColumn = columnFrom };
				searchResults.Add(caretPos);
			}
			++i;
		}

		Repaint();
	}

	public static bool OverrideButton(Rect position, GUIContent content, GUIStyle style, bool forceHot)
	{
		int controlID = GUIUtility.GetControlID(buttonHash, FocusType.Passive, position);
		if (forceHot)
			GUIUtility.hotControl = controlID;

		switch (Event.current.GetTypeForControl(controlID))
		{
			case EventType.MouseDown:
				if (position.Contains(Event.current.mousePosition))
				{
					GUIUtility.hotControl = controlID;
					Event.current.Use();
				}
				return false;

			case EventType.MouseUp:
				if (GUIUtility.hotControl != controlID)
					return false;

				GUIUtility.hotControl = 0;
				Event.current.Use();
				return position.Contains(Event.current.mousePosition);

			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
					Event.current.Use();
				break;

			case EventType.Repaint:
				style.Draw(position, content, controlID);
				break;
		}

		return false;
	}

	private string ToolbarSearchField(Rect position, string text)
    {
		if (styles.toolbarSearchField == null)
		{
			styles.toolbarSearchField = "ToolbarSeachTextField";
			styles.toolbarSearchFieldCancelButton = "ToolbarSeachCancelButton";
			styles.toolbarSearchFieldCancelButtonEmpty = "ToolbarSeachCancelButtonEmpty";
		}

		Rect rc = position;
		rc.width -= 14f;
		if (Event.current.type == EventType.repaint)
		{
			styles.toolbarSearchField.Draw(rc, GUIContent.none, false, false, false, hasSearchBoxFocus);

			if (searchResults.Count > 0)
			{
				bool enabled = GUI.enabled;
				GUI.enabled = false;
				Color color = GUI.backgroundColor;
				GUI.backgroundColor = Color.clear;
				styles.toolbarSearchField.alignment = TextAnchor.UpperRight;
				rc.width -= 20f;
				styles.toolbarSearchField.Draw(rc, (currentSearchResult >= 0 ? (currentSearchResult + 1).ToString() + " of " + searchResults.Count :
					searchResults.Count.ToString() + " results") + '\xa0', false, false, false, hasSearchBoxFocus);
				rc.width += 20f;
				styles.toolbarSearchField.alignment = TextAnchor.UpperLeft;
				GUI.enabled = enabled;
				GUI.backgroundColor = color;
			}
		}
		rc.width -= 20f;

		Color bgColor = GUI.backgroundColor;
		GUI.backgroundColor = Color.clear;
		text = EditorGUI.TextField(rc, text, styles.toolbarSearchField);
		GUI.backgroundColor = bgColor;

		bool isEmpty = text == string.Empty;

		rc = position;
		rc.x += position.width - 14f;
		rc.width = 14f;
		if (!isEmpty)
		{
			if (OverrideButton(rc, GUIContent.none, styles.toolbarSearchFieldCancelButton, helpButtonClicked))
			{
				text = string.Empty;
				focusCodeView = true;
				//GUIUtility.keyboardControl = 0;
			}
		}
		else
		{
			GUI.Label(rc, GUIContent.none, styles.toolbarSearchFieldCancelButtonEmpty);
			if (helpButtonClicked)
				focusSearchBox = true;
		}
		helpButtonClicked = false;

		rc.x -= 10f;
		rc.y += 1f;
        rc.width = 10f;
		rc.height = 13f;
		if (!isEmpty && searchResults.Count != 0 && GUI.Button(rc, GUIContent.none, styles.downArrowStyle))
			SearchNext();

		rc.x -= 10f;
		if (!isEmpty && searchResults.Count != 0 && GUI.Button(rc, GUIContent.none, styles.upArrowStyle))
			SearchPrevious();

		return text;
	}

	private static int FindFirstIndexGreaterThanOrEqualTo<T>(IList<T> sortedCollection, T key)
	{
		return FindFirstIndexGreaterThanOrEqualTo<T>(sortedCollection, key, Comparer<T>.Default);
	}

	private static int FindFirstIndexGreaterThanOrEqualTo<T>(IList<T> sortedCollection, T key, IComparer<T> comparer)
	{
		int begin = 0;
		int end = sortedCollection.Count;
		while (end > begin)
		{
			int index = (begin + end) / 2;
			T el = sortedCollection[index];
			if (comparer.Compare(el, key) >= 0)
				end = index;
			else
				begin = index + 1;
		}
		return end;
	}

	private void SearchPrevious()
	{
		if (searchResultAge != textBuffer.undoPosition)
			SetSearchText(searchString);

		FGTextBuffer.CaretPos beginning = caretPosition;
		if (selectionStartPosition != null && selectionStartPosition < caretPosition)
			beginning = selectionStartPosition;
		currentSearchResult = FindFirstIndexGreaterThanOrEqualTo(searchResults, beginning) - 1;
		currentSearchResult = Math.Max(0, Math.Min(searchResults.Count - 1, currentSearchResult));
		ShowSearchResult(currentSearchResult);
	}

	private void SearchNext()
	{
		if (searchResultAge != textBuffer.undoPosition)
			SetSearchText(searchString);

		FGTextBuffer.CaretPos end = caretPosition;
		if (selectionStartPosition != null && selectionStartPosition > caretPosition)
			end = selectionStartPosition;
		currentSearchResult = FindFirstIndexGreaterThanOrEqualTo(searchResults, end);
		currentSearchResult = Math.Max(0, Math.Min(searchResults.Count - 1, currentSearchResult));
		ShowSearchResult(currentSearchResult);
	}

	private void ShowSearchResult(int index)
	{
		if (searchResultAge != textBuffer.undoPosition)
			SetSearchText(searchString);

		if (index >= 0 && index < searchResults.Count)
		{
			selectionStartPosition = searchResults[index].Clone();
			int columnTo = textBuffer.CharIndexToColumn(selectionStartPosition.characterIndex + searchString.Length, selectionStartPosition.line);
			caretPosition = new FGTextBuffer.CaretPos {
				characterIndex = selectionStartPosition.characterIndex + searchString.Length,
				column = columnTo,
				virtualColumn = columnTo,
				line = selectionStartPosition.line
			};
			caretMoveTime = Time.realtimeSinceStartup;

			int fromRow, fromColumn, toRow, toColumn;
			int fromCharIndex = selectionStartPosition.characterIndex;
			BufferToViewPosition(selectionStartPosition, out fromRow, out fromColumn);
			BufferToViewPosition(caretPosition, out toRow, out toColumn);
			if (fromRow != toRow)
			{
				fromColumn = 0;
				int tempColumn = GetSoftLineBreaks(caretPosition.line)[toRow - 1];
				fromCharIndex = textBuffer.ColumnToCharIndex(ref tempColumn, caretPosition.line);
			}

			scrollToRect.x = charSize.x * fromColumn;
			scrollToRect.y = GetLineOffset(caretPosition.line) + charSize.y * toRow;
			scrollToRect.xMax = charSize.x * toColumn;
			scrollToRect.height = charSize.y;

			pingTimer = 1f;
			pingContent.text = textBuffer.lines[caretPosition.line].Substring(fromCharIndex, searchString.Length);
			//pingStartTime = Time.realtimeSinceStartup;

			Repaint();
		}
	}

	public static void RepaintAllInstances()
	{
		foreach (Editor editor in ActiveEditorTracker.sharedTracker.activeEditors)
			editor.Repaint();
		FGCodeWindow.RepaintAllWindows();
	}

	[MenuItem("CONTEXT/MonoScript/Word Wrap (Code)", false, 141)]
	private static void ToggleWordWrapCode()
	{
		wordWrappingCode = !wordWrappingCode;
		EditorPrefs.SetBool("FlipbookGames.ScriptInspector.WordWrapCode", wordWrappingCode);
		RepaintAllInstances();
	}

	[MenuItem("CONTEXT/MonoScript/Highlight Current Line", false, 142)]
	private static void ToggleHighlightCurrentLine()
	{
		highlightCurrentLine = !highlightCurrentLine;
		EditorPrefs.SetBool("FlipbookGames.ScriptInspector.HighlightCurrentLine", highlightCurrentLine);
		RepaintAllInstances();
	}

	[MenuItem("CONTEXT/MonoScript/Line Numbers", false, 143)]
	private static void ToggleLineNumbers()
	{
		showLineNumbers = !showLineNumbers;
		EditorPrefs.SetBool("FlipbookGames.ScriptInspector.LineNumbers", showLineNumbers);
		RepaintAllInstances();
	}

	[MenuItem("CONTEXT/MonoScript/Track Changes (Code)", false, 144)]
	private static void ToggleTrackChangesCode()
	{
		trackChangesCode = !trackChangesCode;
		EditorPrefs.SetBool("FlipbookGames.ScriptInspector.TrackChanges", trackChangesCode);
		RepaintAllInstances();
	}

	private static void ToggleTrackChangesText()
	{
		trackChangesText = !trackChangesText;
		EditorPrefs.SetBool("FlipbookGames.ScriptInspector.TrackChangesText", trackChangesText);
		RepaintAllInstances();
	}

	private static void ToggleWordWrapText()
	{
		wordWrappingText = !wordWrappingText;
		EditorPrefs.SetBool("FlipbookGames.ScriptInspector.WordWrapText", wordWrappingText);
		RepaintAllInstances();
	}

	[MenuItem("CONTEXT/MonoScript/MD Brown", false, 160)]
	private static void SetCodeStyleMDBrown() { SelectTheme(4, false); }

	[MenuItem("CONTEXT/MonoScript/MD Brown - Dark", false, 161)]
	private static void SetCodeStyleMDBrownDark() { SelectTheme(5, false); }

	[MenuItem("CONTEXT/MonoScript/Monokai", false, 162)]
	private static void SetCodeStyleMonokai() { SelectTheme(6, false); }

	[MenuItem("CONTEXT/MonoScript/Son of Obsidian", false, 163)]
	private static void SetCodeStyleSonOfObsidian() { SelectTheme(7, false); }

	[MenuItem("CONTEXT/MonoScript/Tango Dark (Oblivion) Style", false, 164)]
	private static void SetCodeStyleTangoDark() { SelectTheme(2, false); }

	[MenuItem("CONTEXT/MonoScript/Tango Light Style", false, 165)]
	private static void SetCodeStyleTangoLight() { SelectTheme(3, false); }

	[MenuItem("CONTEXT/MonoScript/Visual Studio Style", false, 166)]
	private static void SetCodeStyleVisualStudio() { SelectTheme(0, false); }

	[MenuItem("CONTEXT/MonoScript/Xcode Style", false, 167)]
	private static void SetCodeStyleXcode() { SelectTheme(1, false); }

	private static void SelectTheme(int themeIndex, bool forText)
	{
		EditorPrefs.SetString(forText ? "ScriptInspectorThemeText" : "ScriptInspectorTheme", availableThemes[themeIndex]);
		if (forText)
			currentThemeText = themes[themeIndex];
		else
			currentThemeCode = themes[themeIndex];
		ApplyTheme(forText ? stylesText : stylesCode, themes[themeIndex]);
		RepaintAllInstances();
	}

	private static void ResetFontSize()
	{
		currentFontSizeDelta = 0;
		EditorPrefs.SetInt("ScriptInspectorFontSize", 0);
		resetCodeFont = true;
		resetTextFont = true;
		if (stylesCode.normalStyle != null)
		{
			stylesCode.normalStyle.fontSize = 0;
			stylesCode.hyperlinkStyle.fontSize = 0;
			stylesCode.mailtoStyle.fontSize = 0;
			stylesCode.keywordStyle.fontSize = 0;
			stylesCode.constantStyle.fontSize = 0;
			stylesCode.userTypeStyle.fontSize = 0;
			stylesCode.commentStyle.fontSize = 0;
			stylesCode.stringStyle.fontSize = 0;
			stylesCode.lineNumbersStyle.fontSize = 0;
			stylesCode.preprocessorStyle.fontSize = 0;
			stylesCode.ping.fontSize = 0;

			stylesCode.normalStyle.fontStyle = 0;
			stylesCode.hyperlinkStyle.fontStyle = 0;
			stylesCode.mailtoStyle.fontStyle = 0;
			stylesCode.keywordStyle.fontStyle = 0;
			stylesCode.constantStyle.fontStyle = 0;
			stylesCode.userTypeStyle.fontStyle = 0;
			stylesCode.commentStyle.fontStyle = 0;
			stylesCode.stringStyle.fontStyle = 0;
			stylesCode.lineNumbersStyle.fontStyle = 0;
			stylesCode.preprocessorStyle.fontStyle = 0;
			stylesCode.ping.fontStyle = 0;
		}
		if (stylesText.normalStyle != null)
		{
			stylesText.normalStyle.fontSize = 0;
			stylesText.hyperlinkStyle.fontSize = 0;
			stylesText.mailtoStyle.fontSize = 0;
			stylesText.keywordStyle.fontSize = 0;
			stylesText.constantStyle.fontSize = 0;
			stylesText.userTypeStyle.fontSize = 0;
			stylesText.commentStyle.fontSize = 0;
			stylesText.stringStyle.fontSize = 0;
			stylesText.lineNumbersStyle.fontSize = 0;
			stylesText.preprocessorStyle.fontSize = 0;
			stylesText.ping.fontSize = 0;

			stylesText.normalStyle.fontStyle = 0;
			stylesText.hyperlinkStyle.fontStyle = 0;
			stylesText.mailtoStyle.fontStyle = 0;
			stylesText.keywordStyle.fontStyle = 0;
			stylesText.constantStyle.fontStyle = 0;
			stylesText.userTypeStyle.fontStyle = 0;
			stylesText.commentStyle.fontStyle = 0;
			stylesText.stringStyle.fontStyle = 0;
			stylesText.lineNumbersStyle.fontStyle = 0;
			stylesText.preprocessorStyle.fontStyle = 0;
			stylesText.ping.fontStyle = 0;
		}
	}

	private static void SelectFont(int fontIndex)
	{
		currentFont = availableFonts[fontIndex];
		EditorPrefs.SetString("ScriptInspectorFont", currentFont);
		//ApplyTheme();
		ResetFontSize();
		RepaintAllInstances();
	}

	private static void ModifyFontSize(int delta)
	{
		currentFontSizeDelta = Math.Max(-10, Math.Min(10, currentFontSizeDelta + Math.Sign(delta)));
		EditorPrefs.SetInt("ScriptInspectorFontSize", currentFontSizeDelta);
		resetCodeFont = true;
		resetTextFont = true;
		RepaintAllInstances();
	}

	private static void ToggleHandleOpenFromProject()
	{
		bool handleDblClick = EditorPrefs.GetBool("ScriptInspector.HandleOpenFromProject", false);
		EditorPrefs.SetBool("ScriptInspector.HandleOpenFromProject", !handleDblClick);
	}

	private static void ToggleHandleOpenTextsFromProject()
	{
		bool handleTextDblClick = EditorPrefs.GetBool("ScriptInspector.HandleOpenTextFromProject", false);
		EditorPrefs.SetBool("ScriptInspector.HandleOpenTextFromProject", !handleTextDblClick);
	}

	private static void ToggleHandleOpenShadersFromProject()
	{
		bool handleShaderDblClick = EditorPrefs.GetBool("ScriptInspector.HandleOpenShaderFromProject", false);
		EditorPrefs.SetBool("ScriptInspector.HandleOpenShaderFromProject", !handleShaderDblClick);
	}

	[MenuItem("Help/About Script Inspector 2", false, 1000)]
	[MenuItem("CONTEXT/MonoScript/About", false, 192)]
	private static void About()
	{
		EditorWindow.GetWindow<AboutScriptInspector>();
	}
}
