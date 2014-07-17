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


[CustomEditor(typeof(Shader))]
public class ShaderInspector : ScriptInspector
{
	private static System.Type unityShaderInspectorType;
	private static MethodInfo internalSetTargetsMethod;

	public bool showInfo = true;
	private Editor unityShaderInspector;

	public override void OnInspectorGUI()
	{
		EditorGUIUtility.LookLikeControls();
		EditorGUI.indentLevel = 0;

		var rc = GUILayoutUtility.GetRect(1f, 13f);
		rc.yMin -= 5f;
		var enabled = GUI.enabled;
		GUI.enabled = true;
		showInfo = InspectorFoldout(rc, showInfo, targets);
		GUI.enabled = enabled;
		if (showInfo)
		{
			if (unityShaderInspectorType == null)
			{
				unityShaderInspectorType = typeof(Editor).Assembly.GetType("UnityEditor.ShaderInspector");
				if (unityShaderInspectorType != null)
				{
					const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
					internalSetTargetsMethod = unityShaderInspectorType.GetMethod("InternalSetTargets", flags);
				}
			}
			if (targets != null && internalSetTargetsMethod != null)
			{
				if (unityShaderInspector == null)
				{
					unityShaderInspector = (Editor) CreateInstance(unityShaderInspectorType);
					internalSetTargetsMethod.Invoke(unityShaderInspector, new object[] { targets.Clone() });
				}
				unityShaderInspector.OnInspectorGUI();
			}
		}

		if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(target)))
			base.OnInspectorGUI();
	}

	protected override void DoGUI()
	{
#if UNITY_4_3
		textEditor.OnInspectorGUI(false, new RectOffset(0, -4, showInfo ? 29 : 22, -13));
#else
		textEditor.OnInspectorGUI(false, new RectOffset(0, 0, showInfo ? 29 : 22, -13));
#endif
	}

	private static GUIStyle inspectorTitlebar;
	private static GUIStyle inspectorTitlebarText;

	public static bool InspectorFoldout(Rect position, bool foldout, UnityEngine.Object[] targetObjs)
	{
		if (inspectorTitlebar == null)
		{
			inspectorTitlebar = "IN Title";
			inspectorTitlebarText = "IN TitleText";
		} 

		EditorGUIUtility.LookLikeControls(Screen.width, 0f);
		foldout = EditorGUI.Foldout(position, foldout, GUIContent.none, true, inspectorTitlebar);
		
		position = inspectorTitlebar.padding.Remove(position);
		if (Event.current.type == EventType.Repaint)
			inspectorTitlebarText.Draw(position, "Shader Info", false, false, foldout, false);
		EditorGUIUtility.LookLikeControls();
		
		return foldout;
	}
}
