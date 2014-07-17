/* SCRIPT INSPECTOR 2
 * version 2.1.9, May 2014
 * Copyright © 2012-2014, Flipbook Games
 * 
 * Unity's legendary custom inspector for C#, UnityScript and Boo scripts,
 * now transformed into a powerful Script, Shader, and Text Editor!!!
 * 
 * NOTE TO UNITY 4 USERS:
 * This package has been exported from Unity 3.5.6 to allow using it in earlier
 * versions of Unity. Unity 4 has different set of properties for Font assets and
 * fonts imported from Unity 3 packages do not give the best results in Unity 4.
 * In order to make the text in Script Inspector 2 crisp and more readable after
 * importing this package you should set the import settings for all fonts
 * located in Assets/FlipbookGames/ScriptInspector2/Editor/EditorResources to use
 * the Dynamic font rendering and rendering mode to be Hinted Smooth.
 * 
 * Bug fixed in v2.1.9:
 * - Fixed crash caused by calling System.Reflection.Emit.AssemblyBuilder.GetExportedTypes() (thanks to makeshiftwings)
 * 
 * Bug fixed in v2.1.8:
 * - Rarely reloading a script would fail after being modified outside of Unity (thanks to Jim Vaughn)
 * 
 * New in v2.1.7:
 * - Improved performance and fixes for large single line files (thanks to mcmorry)
 * 
 * Bug fixed in v2.1.6:
 * - Incorrect handling of rich text tags (thanks to Callski)
 * 
 * New in v2.1.5:
 * - Shader code view added to shader inspector (thanks to I am da bawss for the idea)
 * - Shift-Space inserts a Space character instead of toggling maximize tab (thanks to joelfivat)
 * 
 * New in v2.1.4:
 * - Compatible with Unity 4.3
 * - Removed one more default parameter value
 * 
 * New in v2.1.3:
 * - Removed all default parameter values on methods to make Si2 code compatible with MonoDevelop
 * 
 * Bugs fixed in v2.1.2:
 * - Alternative indent/outdent keyboard shortcuts (Ctrl+[ and Ctrl+])
 * - Some Cmd-shortcuts on OS X were getting inserted into text after executing the command
 *
 * Bugs fixed in v2.1.1:
 * - Exception on releasing the mouse above editor view after drag-select
 * - Exception on double-click on Project items on OS X 10.6.8 with Unity 3.5.7
 *
 * New in v2.1:
 * - Editing shader assets in Si2 tabs
 * - ShaderLab syntax highlighting
 * - SI Console can open shaders to locate compile errors
 * - Editing text assets in Si2 tabs or in the custom inspector
 * - Single click hyperlink following from text assets
 * - Added menu options for creating new text asset
 * - Added Word Wrap feature, optional for code and for text
 * - Improved syntax highlighting for scripts
 * - SI Console can show full call stack of log entries and navigate to script locations
 * - Optionally opens script, shaders, and text assets on double-click on items in Project view
 * - Context menu on components in the Inspector to open their scripts in Si2
 * - Context menu on materials in the Inspector to open their shaders in Si2
 * - Opens shaders and text assets dropped on Si2 tabs
 * - Opens shaders assigned to materials dropped on Si2 tabs
 * - Opens component scripts of GameObjects or Prefabs dropped on Si2 tabs
 * - Color schemes can be different for code and text
 * - Font size controls (dynamic fonts only)
 * - Current line highlighting
 * - Added Monokai color scheme
 * - Added Son of Obsidian color scheme
 * - Added keyboard shortcuts to close Si2 tabs
 * - Added keyboard shortcuts to switch between Si2 tabs
 * - Added keyboard shortcuts to rearrange Si2 tabs
 * - Improved reloading and detection of unsaved changes
 * 
 * New in 2.0.1:
 * - Added alternative shortcuts for Find Next/Previous: Ctrl+G/Ctrl+Shift+G
 * - Fixed a rare bug on drag-and-drop
 *
 * New Features in 2.0:
 * - Advanced source code editor with extensive mouse and keyboard support
 * - Undo and Redo functions for each script independently
 * - Opening scripts in dedicated dockable windows
 * - Opening scripts at line from log entries in the Console window straight
 *   into a Script Inspector code window instead of external IDE
 * - Synchronized editing of the same script opened in multiple windows
 * - Syntax highlighting updated in real-time while typing
 * - Moving and copying selected text with mouse drags
 * - Track changes indicators on lines
 * - Preserved text encoding and line ending style
 * - Automatic saving before entering game mode
 * - External changes detection and reloading
 * - Saving unsaved changes on exit if user wants that
 * - Quick opening scripts by dragging them to an existing code window
 * - Multiple fonts available for displaying code
 * - Resources folder renamed to EditorResources to reduce size of final builds
 * - Best of all: It still comes with the full source code
 * 
 * Bugs fixed in 1.4.2:
 * - Loading resources after importing Script Inspector package
 * - Keyboard shortcut on Open at line context menu
 * - Calculating script content width
 * 
 * Bugs fixed in 1.4.1:
 * - Texture leaking on Save Scene
 * - File locks preventing external editor to save script
 * - Quick search for right-to-left selections
 * - "Zero length selection" used as search text
 * - Updating multiple Inspector views on theme change
 * 
 * New Features in 1.4:
 * - Cursor navigation and selection
 * - Extensive keyboard and mouse support
 * - Copy selection functionality
 * - Smooth automatic scroll for mouse drag selections
 * - Search field shows current and total number of results
 * - Quick search keyboard shortcuts
 * - Two additional color schemes (thanks to Little Angel)
 * 
 * Bugs fixed in 1.3.1:
 * - Opening hyperlinks from code view
 * 
 * New Features in 1.3:
 * - Search functionality
 * - All hyperlinks accessible from toolbar
 * - More compact UI
 * - Faster parsing
 * 
 * New Features in 1.2:
 * - Line numbers (optional)
 * - Open at line functionality
 * 
 * New Features in 1.1:
 * - Progresive processing of very long files
 * 
 * Initial Release Features:
 * - Fast source code syntax analysis of C#, JavaScript and Boo scripts
 * - Single-click opening of URL and email addresses found in comments
 * - Shows the whole content of a script
 * - Four built-in color schemes
 * - Optimized rendering for smooth and responsive GUI
 * - Full source code provided for your reference or modification
 * 
 * Follow me on http://twitter.com/FlipbookGames
 * Like Flipbook Games on Facebook http://facebook.com/FlipbookGames
 * Join Unity forum discusion http://forum.unity3d.com/threads/138329
 * Contact info@flipbookgames.com for feedback, bug reports, or suggestions.
 * Visit http://flipbookgames.com/ for more info.
 */

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MonoScript))]
public class ScriptInspector : Editor
{
	[SerializeField, HideInInspector]
	protected FGTextEditor textEditor = new FGTextEditor();

	[System.NonSerialized]
	protected bool enableEditor = true;

	public static string GetVersionString()
	{
		return "2.1.9, May 2014";
	}

	public void OnDisable()
	{
		textEditor.onRepaint = null;
		textEditor.OnDisable();
	}

	public override void OnInspectorGUI()
	{
		if (enableEditor)
		{
			textEditor.onRepaint = Repaint;
			textEditor.OnEnable(target);
			enableEditor = false;
		}

		if (Event.current.type == EventType.ValidateCommand)
		{
			if (Event.current.commandName == "ScriptInspector.AddTab")
			{
				Event.current.Use();
				return;
			}
		}
		else if (Event.current.type == EventType.ExecuteCommand)
		{
			if (Event.current.commandName == "ScriptInspector.AddTab")
			{
				FGCodeWindow.OpenNewWindow();
				Event.current.Use();
				return;
			}
		}
		
		DoGUI();
	}

	protected virtual void DoGUI()
	{
		textEditor.OnInspectorGUI(true, new RectOffset(0, -4, 0, 0));
	}
}
