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

[CustomEditor(typeof(TextAsset))]
public class FGTextInspector : ScriptInspector
{
	protected override void DoGUI()
	{
#if UNITY_4_3
		textEditor.OnInspectorGUI(false, new RectOffset(0, -4, 14, -13));
#else
		textEditor.OnInspectorGUI(false, new RectOffset(0, 0, 14, -13));
#endif
	}
}
