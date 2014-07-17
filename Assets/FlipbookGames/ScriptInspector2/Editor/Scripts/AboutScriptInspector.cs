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

using UnityEditor;
using UnityEngine;

public class AboutScriptInspector : EditorWindow
{
	private GUIStyle textStyle;
	private GUIStyle bigTextStyle;
	private GUIStyle miniTextStyle;
	private Texture2D flipbookLogo;

	private void OnEnable()
	{
		title = "About";
		minSize = new Vector2(265f, 155f);
		maxSize = new Vector2(265f, 155.1f);
	}

	void Initialize()
	{
		textStyle = new GUIStyle();
		textStyle.alignment = TextAnchor.UpperCenter;
		
		bigTextStyle = new GUIStyle(EditorStyles.boldLabel);
		bigTextStyle.fontSize = 24;
		bigTextStyle.alignment = TextAnchor.UpperCenter;
		
		miniTextStyle = new GUIStyle(EditorStyles.miniLabel);
		miniTextStyle.alignment = TextAnchor.UpperCenter;

		flipbookLogo = FGTextEditor.LoadEditorResource<Texture2D>("CreatedByFlipbookGames.png");
	}

	private void OnGUI()
	{
		if (textStyle == null)
			Initialize();

		EditorGUILayout.BeginVertical();

		GUILayout.Box("Script Inspector 2", bigTextStyle);
		GUILayout.Label("\xa9 Flipbook Games. All Rights Reserved.", miniTextStyle);
		GUILayout.Label("Version " + ScriptInspector.GetVersionString(), textStyle);

		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		if (GUILayout.Button(flipbookLogo, GUIStyle.none))
		{
			Application.OpenURL("http://www.flipbookgames.com/");
		}
		if (Event.current.type == EventType.repaint)
		{
			EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
		}
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Close"))
		{
			Close();
		}
		GUILayout.Space(2f);
		GUILayout.EndVertical();
		GUILayout.Space(10f);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(10f);

		EditorGUILayout.EndVertical();
	}
}
