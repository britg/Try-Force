SCRIPT INSPECTOR 2
version 2.1.9, May 2014
Copyright © 2012-2014, Flipbook Games
--------------------------------

Unity's legendary custom inspector for C#, UnityScript and Boo scripts, now transformed into a powerful Script, Shader, and Text Editor!!!


NOTE TO UNITY 4 USERS:
This package has been exported from Unity 3.5.6 to allow using it in earlier versions of Unity. Unity 4 has different set of properties for Font assets and fonts imported from Unity 3 packages do not give the best results in Unity 4. In order to make the text in Script Inspector 2 crisp and more readable after importing this package you should set the import settings for all fonts located in Assets/FlipbookGames/ScriptInspector2/Editor/EditorResources to use the Dynamic font rendering and rendering mode to be Hinted Smooth.


Bug fixed in v2.1.9:
- Fixed crash caused by calling System.Reflection.Emit.AssemblyBuilder.GetExportedTypes() (thanks to makeshiftwings)

Bug fixed in v2.1.8:
- Rarely reloading a script would fail after being modified outside of Unity (thanks to Jim Vaughn)

New in v2.1.7:
- Improved performance and fixes for large single line files (thanks to mcmorry)

Bug fixed in v2.1.6:
- Incorrect handling of rich text tags (thanks to Callski)

New in v2.1.5:
- Shader code view added to shader inspector (thanks to I am da bawss for the idea)
- Shift-Space inserts a Space character instead of toggling maximize tab (thanks to joelfivat)

New in v2.1.4:
- Compatible with Unity 4.3
- Removed one more default parameter value

New in v2.1.3:
- Removed all default parameter values on methods to make Si2 code compatible with MonoDevelop

Bugs fixed in v2.1.2:
- Alternative indent/outdent keyboard shortcuts (Ctrl+[ and Ctrl+])
- Some Cmd-shortcuts on OS X were getting inserted into text after executing the command

Bugs fixed in v2.1.1:
- Exception on releasing the mouse above editor view after drag-select
- Exception on double-click on Project items on OS X 10.6.8 with Unity 3.5.7

New in v2.1:
- Editing shader assets in Si2 tabs
- ShaderLab syntax highlighting
- SI Console can open shaders to locate compile errors
- Editing text assets in Si2 tabs or in the custom inspector
- Single click hyperlink following from text assets
- Added menu options for creating new text asset
- Added Word Wrap feature, optional for code and for text
- Improved syntax highlighting for scripts
- SI Console can show full call stack of log entries and navigate to script locations
- Optionally opens script, shaders, and text assets on double-click on items in Project view
- Context menu on components in the Inspector to open their scripts in Si2
- Context menu on materials in the Inspector to open their shaders in Si2
- Opens shaders and text assets dropped on Si2 tabs
- Opens shaders assigned to materials dropped on Si2 tabs
- Opens component scripts of GameObjects or Prefabs dropped on Si2 tabs
- Color schemes can be different for code and text
- Font size controls (dynamic fonts only)
- Current line highlighting
- Added Monokai color scheme
- Added Son of Obsidian color scheme
- Added keyboard shortcuts to close Si2 tabs
- Added keyboard shortcuts to switch between Si2 tabs
- Added keyboard shortcuts to rearrange Si2 tabs
- Improved reloading and detection of unsaved changes

New in v2.0.2:
- Added support for "AltGr" characters
- Restored default behavior of Return key in Project view on OS X

New in v2.0.1:
- Added alternative shortcuts for Find Next/Previous: Ctrl+G/Ctrl+Shift+G
- Fixed a rare bug on drag-and-drop

New in v2.0:
- Advanced source code editor with extensive mouse and keyboard support
- Undo and Redo functions for each script independently
- Opening scripts in dedicated dockable windows
- Opening scripts at line from log entries in the Console window straight into a Script Inspector code window instead of external IDE
- Synchronized editing of the same script opened in multiple windows
- Syntax highlighting updated in real-time while typing
- Moving and copying selected text with mouse drags
- Track changes indicators on lines
- Preserved text encoding and line ending style
- Automatic saving before entering game mode
- External changes detection and reloading
- Saving unsaved changes on exit if user wants that
- Quick opening scripts by dragging then to an existing code window
- Multiple fonts available for displaying code
- Resources folder renamed to EditorResources to reduce size of final builds
- Best of all: It still comes with the full source code

Bugs fixed in v1.4.2:
- Loading resources after importing Script Inspector package
- Keyboard shortcut on Open at line context menu
- Text width calculation used for horizontal scrollbar

Bugs fixed in v1.4.1:
- Texture leaking on Save Scene
- File locks preventing external editor to save script
- Quick search for right-to-left selections
- "Zero length selection" used as search text
- Updating multiple Inspector views on theme change

New in v1.4:
- Cursor navigation and selection
- Extensive keyboard and mouse support
- Copy selection functionality
- Smooth automatic scroll for mouse drag selections
- Search field shows current and total number of results
- Quick search keyboard shortcuts
- Two additional color schemes (thanks to Little Angel)

Bugs fixed in v1.3.1:
- Opening hyperlinks from code view

New in v1.3:
- Search functionality
- All hyperlinks accessible from toolbar
- More compact UI
- Faster parsing

New in v1.2:
- Line numbers (optional)
- Opening at a specific line
- Fixed a Tab-expansion bug
- More optimization


1. Description

SCRIPT INSPECTOR 2 is the latest HUGE improvement of that legendary editor extension for Unity which you've been using so far only as a quick and fancy viewer for your scripts, now transformed into a powerful script, shader and text assets editor embedded right inside the Unity Editor!

Built with performance in mind since the beginning while keeping the quality at the highest standards, it was gradually growing with new features, to finally become the fastest and most comfortable solution for programmers to write their scripts from inside Unity. Although at this stage it still cannot rival the most advanced external programming applications, neither can fully replace their original purpose, Script Inspector 2 using its greatest advantage, being integrated inside Unity, is definitely a great tool for improving programmer’s workflow, fulfilling their everyday basic needs. Accessibility combined with quick iteration cycles achieved with Script Inspector 2 makes this extension the preferred script editor for any Unity programmer including myself, the author. As a result of that addiction considerable amount of work, especially in the finalizing stage during development of this extension was done in it, making Script Inspector 2 the first Unity extension programmed in itself! ;)


2. Motivation

After surprising popularity of all of the previous versions of Script Inspector and by popular demand for adding at least some basic editing functionality to it, Script Inspector was transformed into a script editor that’s not only a handy and nice tool for fixing typos or closing forgotten parentheses, but also a really solid script editor strong enough to support professional game development and to be used in real projects.


3. How to use the Script Inspector 2

First thing you will notice when you start using Script Inspector 2 is the SI Console tab which opens automatically and docks next to Unity’s default Console tab. You can also open it manually from the Window menu, select Window->Script Inspector Console and the new SI Console tab will dock next to the Console tab. SI Console works exactly the same as the original console with the only difference that on double-click or Enter/Return key it opens the associated script or shader with the selected log entry in a Si2 tab instead of opening it in the external IDE. You can still open log entries in the external IDE from the standard Console tab.

Whenever a text asset or a C#, JavaScript, or Boo script asset is selected in Unity Editor, the Inspector tab displays the whole content of the script in a way typical for viewing code or text files, with syntax highlighting, optional word-wrapping, proper tab expansion and alignment, optional line numbers, and with underlined hyperlinks for single-click navigation. In addition Script Inspector 2 allows script or text assets editing inside the Inspector or in dedicated script/text editor tabs. It also allows editing shader assets in dedicated script editor tabs.

To open a script, shader, or text asset in dedicated editor tabs there are a couple of options. One of them is to follow a log entry from the SI Console, as mentioned before. From the SI Console you can also see the call stack on some entries and navigate to any of those locations from the context popup menu on log entries. Another way to open scripts, shaders, and text assets is to use the New Tab toolbar button in the upper left corner of any script or text asset displayed in the Inspector tab or dedicated script, shader, and text editor tabs (or use the keyboard shortcut Ctrl+T, or ⌘T on Mac). Script editor tabs (Si2 tabs) can be arranged, docked, undocked, maximized, or closed as the regular Unity tabs, and even saved and restored with editor Layouts.

You can open additional script, shader, or text assets into a particular tab group by dragging the assets from the Project tab to one of the existing Si2 tabs. You can also drag Materials to open the shader used in them directly. Similarly you can drag GameObjects from the current scene or prefabs to open their components based on MonoBehaviour scripts. Component scripts can also be easily accessed from the component’s wrench menus in the Inspector. You similarly open shaders used in materials from the wrench menu of the Inspector.

The Script Inspector 2 can optionally open a new Si2 tab or focus an existing tab for script, shader, or text assets by double-clicking them in the Project tab. These options (turned off by default) are located in the wrench menu of any of the script/text editor views. Pressing Enter (Command-Down Arrow on Mac) on the selected asset in the Project tab would still open them in the external IDE, so you still have that option available.

Script, shader, and text assets can be opened in multiple Script Inspector views at the same time showing different parts of their content with different caret position and selection, but the editing is synchronized. Changes made in one of these tabs are immediately visible in the other tabs. The Undo/Redo buffer is also shared so that changes made in tab can be reverted in another one, for example.

Script Inspector 2 holds unlimited Undo/Redo buffers for each edited asset independently. These buffers are also independent of the Unity’s built-in undo buffer, so that changes in scripts don’t interfere with the changes made in the rest of Unity and each can be reverted or repeated independently. There are Undo and Redo toolbar buttons in each script/text editor view, and there are keyboard shortcuts associated with them – Ctrl+Shift+Z and Ctrl+Shift+Y on Windows and ⌃Z and ⇧⌃Z on OS X for Undo and Redo respectively.

Script Inspector 2 handles very wide range of mouse and keyboard interactions, providing the users with an experience similar to modern IDE’s. Cursor navigation, selecting words, selecting lines, Cut/Copy/Paste, drag selections, dragging selected text to move or copy, search functionality, quick search, are only some of the features implemented in Script Inspector 2. Since version 2.1 there are also shortcuts for closing Si2 tabs, switching between Si2 tabs within a dock group, and for rearranging the tabs. See the Shortcuts section for a more complete list of included features.

Optionally script/text editor view can display line numbers and track changes indicators, which can be turned on or off from the wrench toolbar button. Additionally from that button you can choose a color scheme for code syntax highlighting or for the text assets, and the font used for displaying the content. The About box can also be accessed through the wrench button menu.

All hyperlinks found in the comments of the script or anywhere in the text assets will be shown as underlined clickable links and also listed in the Links toolbar button drop-down menu so you can easily open any of them directly from there.


4. Saving and Reloading

Script Inspector 2 keeps track of all changes made using it. Asterisk signs in Si2 tab’s titles indicate unsaved changed. You can save the changes with the Save toolbar button or with the keyboard shortcut Ctrl+Alt+S on Windows or ⌥⌘S on OS X. All modified assets will be automatically saved before entering the game mode.

Navigating away from a modified asset in the Inspector or closing the last open Si2 tab for that asset will warn you and asking if you want to save the changes made in the asset, discard them, or keep them in memory to continue with editing later. You’ll also get similar warnings on quitting the Editor or loading another Unity project, just without the option to keep the changes in memory, of course.

After saving the changes made in Script Inspector 2 the Unity will compile them as usual and display all compile warnings and errors in both the Console and SI Console.

Reimported scripts, either manually or automatically as a result of external changes, will be updated in the Script Inspector views automatically. Well, unless those scripts have been modified and not saved before reimporting, in which case the Script Inspector 2 gives you a warning and asking which version to keep.


5. List of keyboard shortcuts and mouse functions
  (assume ⌘ instead of Ctrl on OS X if not noted otherwise)

a) Caret Navigation (all these clear the selection)

Arrow keys - caret navigation
Mouse click – sets caret position
Ctrl+Left and Ctrl+Right arrow key - Moves the cursor to the previous or next word
Ctrl+Up and Ctrl+Down arrow key - Scrolls the code view by one line
PageUp and PageDown - move the cursor by one page
End - moves the cursor at the end of a line
Home - moves the cursor before the first non-whitespace character on a line or at the beginning of the line
Ctrl+Home and Ctrl+PageUp - move the cursor at the beginning of the whole script
Ctrl+End and Ctrl+PageDown - move the cursor at the end of the whole script

b) Selections

Shift + any cursor navigation shortcut or mouse click - selects text or alters the selection
Mouse double-click or Ctrl+mouse click - selects the whole word
Mouse click and drag on text - mouse drag selection
Mouse click on line numbers - selects the whole line
Mouse click and drag on line numbers - selects multiple lines
Ctrl+A, or Edit->Select All from the main menu, or Select All from the context menu - selects the whole content of a script
Escape - clears the selection

c) Editing

Typing text – inserts text (replacing the selected text, if any)
Backspace – deletes selected text or the character before of the caret
Delete – deletes selected text or the character after the caret
Ctrl+Backspace – deletes selected text or the word or part of it before the caret
Ctrl+Delete – deletes selected text or the word or part of it after the caret

d) Cut, Copy, and Paste

Ctrl+X, or Edit->Cut from the main menu, or Cut from the context menu - cuts the selected text
Ctrl+C, or Edit->Copy from the main menu, or Copy from the context menu - copies the selected text
Ctrl+V, or Edit->Paste from the main menu, or Paste from the context menu – pastes the clipboard text (replacing the selected text, if any)
Mouse dragging selected text – moves the selected text
Ctrl+mouse dragging selected text – duplicates the selected text

e) More editing

Ctrl+Shift+Z (⌃Z on OS X) – Undo
Ctrl+Shift+Y (⇧⌃Z on OS X) – Redo
Ctrl+K or Ctrl+/ - toggles on or off code comments on a single line or selected lines (except while editing text assets, of course)
Ctrl+[ - decreases indentation of a single line or more selected lines
Ctrl+] - increases indentation of a single line or more selected lines
Tab – inserts a Tab character if nothing is selected, otherwise increases indentation
Shift+Tab – deletes the Tab character before caret, or otherwise decreases indentation

f) Searching

Ctrl+Shift+Up – finds the previous occurrence of the word at caret or of the selected text
Ctrl+Shift+Down – finds the next occurrence of the word at caret or of the selected text
Ctrl+F – sets the keyboard input to the search field in the toolbar
Escape – sets the keyboard input back to the code editing control
Enter – finds the first occurrence of the searched text and sets the focus on code
Up/Down Arrows and arrow keys inside the search field – find previous/next occurrence of the searched text
F3, or Ctrl+G – finds next occurrence of the searched text
Shift+F3, or Ctrl+Shift+G – finds the previous occurrence of the searched text

g) Si2 tabs related

Ctrl+F4 or Ctrl+W – closes the active Si2 tab
Ctrl+PageUp or Ctrl+Alt+Left arrow key – activates the first Si2 tab to the left of the current one
Ctrl+PageDown or Ctrl+Alt+Right arrow key – activates the first Si2 tab to the right of the current one
Ctrl+Shift+Alt+Left – moves the active Si2 tab one position to the left in a dock group
Ctrl+Shift+Alt+Right – moves the active Si2 tab one position to the right in a dock group

h) Other functions

Right mouse button click (also Command+click on OS X) - displays context menu
Ctrl+Alt+S (Command-Option-S on OS X) – saves changes in a script
Ctrl+T – opens the script in a new tab
Ctrl+Enter, or Open at Line from the context menu - opens the script in the external editor at line where the caret is

i) Font size (only for dynamic fonts)

Ctrl+Minus, Ctrl+Plus, Ctrl+Equals sign, Ctrl+Mouse Wheel, and magnify gestures – increase and decrease font size

6. Future plans

Now that the code editing functionality is implemented, the short term plan is to extend that with code navigation, code completion, parameters hints for function calls, etc.

The long term plan is to add full debugging capabilities and turn Unity into something that only the greatest AAA game engines may offer.


7. Support, Bugs, Requests, and Feedback

Feel free to contact Flipbook Games at info@flipbookgames.com or visit http://flipbookgames.com/ for support, bug reports, suggestions, feedback, etc...


Thank you for purchasing the Script Inspector 2! I hope you will enjoy using it, and that if you do, you will support its future development with suggestions, comments, some nice reviews and maybe five star ratings ;)

 * Like Flipbook Games on Facebook http://facebook.com/FlipbookGames
 * Join Unity forum discussion http://forum.unity3d.com/threads/138329
 * Contact info@flipbookgames.com for feedback, bug reports, or suggestions.
 * Visit http://flipbookgames.com/ for more info.

P.S.: Make sure you also check the Favorites Tab[s] http://u3d.as/3hG, another great Unity extension published by Flipbook Games!
