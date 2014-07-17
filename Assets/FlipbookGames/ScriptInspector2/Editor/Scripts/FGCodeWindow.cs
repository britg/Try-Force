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
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

[System.Serializable, StructLayout(LayoutKind.Sequential), InitializeOnLoad]
public class FGCodeWindow : EditorWindow
{
	[HideInInspector, SerializeField]
	private Object targetAsset;

	[HideInInspector, SerializeField]
	private string targetAssetGuid;

	private static Object useTargetAsset = null;

	[SerializeField, HideInInspector]
	private FGTextEditor textEditor = new FGTextEditor();

	[System.NonSerialized]
	private int pingLineWhenLoaded = -1;

	private static HashSet<FGCodeWindow> codeWindows = new HashSet<FGCodeWindow>();
	
	private static class API
	{
		public static System.Type containerWindowType;
		public static System.Type viewType;
		public static System.Type dockAreaType;
		public static FieldInfo panesField;
		public static PropertyInfo windowsField;
		public static PropertyInfo mainViewField;
		public static PropertyInfo allChildrenField;
		public static MethodInfo addTabMethod;
		public static FieldInfo parentField;
		public static EditorApplication.CallbackFunction windowsReordered;
		public static MethodInfo createAssetMethod;
		
		static API()
		{
			var editorAssembly = typeof(EditorWindow).Assembly;
			containerWindowType = editorAssembly.GetType("UnityEditor.ContainerWindow");
			viewType = editorAssembly.GetType("UnityEditor.View");
			dockAreaType = editorAssembly.GetType("UnityEditor.DockArea");
			parentField = typeof(EditorWindow).GetField("m_Parent", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic);
			if (dockAreaType != null)
			{
				panesField = dockAreaType.GetField("m_Panes", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic);
				addTabMethod = dockAreaType.GetMethod("AddTab", new System.Type[] { typeof(EditorWindow) });
			}
			if (containerWindowType != null)
			{
				windowsField = containerWindowType.GetProperty("windows", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic);
				mainViewField = containerWindowType.GetProperty("mainView", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic);
			}
			if (viewType != null)
				allChildrenField = viewType.GetProperty("allChildren", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic);
				
			FieldInfo windowsReorderedField = typeof(EditorApplication).GetField("windowsReordered", BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic);
			windowsReordered = windowsReorderedField.GetValue(null) as EditorApplication.CallbackFunction;

			System.Type projectWindowUtilType = editorAssembly.GetType("UnityEditor.ProjectWindowUtil");
			if (projectWindowUtilType != null)
				createAssetMethod = projectWindowUtilType.GetMethod("CreateAsset", new System.Type[] { typeof(Object), typeof(string) });
		}

		public static bool CreateAsset(Object asset, string pathName)
		{
			if (createAssetMethod == null)
				createAssetMethod.Invoke(null, new object[] { asset, pathName });
			return createAssetMethod == null;
		}
	};
	
	static FGCodeWindow()
	{
		EditorApplication.update -= InitOnLoad;
		EditorApplication.update += InitOnLoad;
	}

	private static void InitOnLoad()
	{
		EditorApplication.update -= InitOnLoad;
		EditorApplication.projectWindowItemOnGUI -= OnProjectItemGUI;
		EditorApplication.projectWindowItemOnGUI += OnProjectItemGUI;

		FGConsole.OpenIfConsoleIsOpen();
	}

	private static void OnProjectItemGUI(string item, Rect selectionRect)
	{
		if (string.IsNullOrEmpty(item))
			return;

		if (Event.current.isMouse)
		{
			if (Event.current.type != EventType.MouseDown || Event.current.clickCount != 2 || Event.current.button != 0)
				return;

			if (!selectionRect.Contains(Event.current.mousePosition))
				return;
		}
		else
		{
			return;
		}

		string path = AssetDatabase.GUIDToAssetPath(item);
		if (!path.EndsWith(".cs", System.StringComparison.OrdinalIgnoreCase) &&
			!path.EndsWith(".js", System.StringComparison.OrdinalIgnoreCase) &&
			!path.EndsWith(".boo", System.StringComparison.OrdinalIgnoreCase))
		{
			// not a script

			TextAsset txt = null;
			if (EditorPrefs.GetBool("ScriptInspector.HandleOpenTextFromProject", false))
			{
				if (!path.EndsWith(".dll", System.StringComparison.OrdinalIgnoreCase))
					txt = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
			}
			if (txt == null)
			{
				// not a text asset

				Shader shader = null;
				if (EditorPrefs.GetBool("ScriptInspector.HandleOpenShaderFromProject", false))
					shader = AssetDatabase.LoadAssetAtPath(path, typeof(Shader)) as Shader;
				if (shader == null)
				{
					// not even a shader
					return;
				}
			}

			Event.current.Use();
			OpenAssetInTab(item);
			GUIUtility.ExitGUI();
			return;
		}

		if (!EditorPrefs.GetBool("ScriptInspector.HandleOpenFromProject", false))
			return;

		MonoScript script = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)) as MonoScript;
		if (script != null)
		{
			Event.current.Use();
			OpenAssetInTab(item);
			GUIUtility.ExitGUI();
		}
	}

	private static string MaterialContextMenuToShaderGUID(MenuCommand mc)
	{
		Material target = mc.context as Material;
		if (target == null)
			return null;
		if (target.shader == null)
			return null;
		int shaderID = target.shader.GetInstanceID();
		if (shaderID == 0)
			return null;
		string assetPath = AssetDatabase.GetAssetPath(shaderID);
		if (string.IsNullOrEmpty(assetPath))
			return null;
		return AssetDatabase.AssetPathToGUID(assetPath);
	}

	[MenuItem("CONTEXT/Material/Edit in Script Inspector...", true, 101)]
	private static bool ValidateEditMaterialShader(MenuCommand mc)
	{
		return !string.IsNullOrEmpty(MaterialContextMenuToShaderGUID(mc));
	}

	[MenuItem("CONTEXT/Material/Edit in Script Inspector...", false, 101)]
	private static void EditMaterialShader(MenuCommand mc)
	{
		string guid = MaterialContextMenuToShaderGUID(mc);
		if (!string.IsNullOrEmpty(guid))
			OpenAssetInTab(guid);
	}

	[MenuItem("CONTEXT/MonoBehaviour/Edit in Script Inspector", false, 101)]
	private static void OpenBehaviourScript(MenuCommand mc)
	{
		MonoBehaviour target = mc.context as MonoBehaviour;
		if (target == null)
			return;
		MonoScript monoScript = MonoScript.FromMonoBehaviour(target);
		if (monoScript == null)
			return;
		string scriptPath = AssetDatabase.GetAssetPath(monoScript);
		if (!string.IsNullOrEmpty(scriptPath))
			OpenAssetInTab(AssetDatabase.AssetPathToGUID(scriptPath));
	}
	
	[MenuItem("Assets/Create/Text", false, 90)]
	public static void CreateTextAsset()
	{
		string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
		if (!AssetDatabase.Contains(Selection.activeInstanceID))
			path = "Assets";

		if (!System.IO.Directory.Exists(path))
			path = System.IO.Path.GetDirectoryName(path);
		path = System.IO.Path.Combine(path, "New Text.txt");
		path = AssetDatabase.GenerateUniqueAssetPath(path);

		System.IO.StreamWriter writer = System.IO.File.CreateText(path);
		writer.Close();
		writer.Dispose();

		AssetDatabase.ImportAsset(path);
		Selection.activeObject = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
	}
	
	public static void RepaintAllWindows()
	{
		foreach (FGCodeWindow wnd in codeWindows)
			wnd.Repaint();
	}

	public static void OpenAssetInTab(string guid)
	{
		OpenAssetInTab(guid, -1);
	}

	public static void OpenAssetInTab(string guid, int line)
	{
		foreach (FGCodeWindow codeWindow in codeWindows)
		{
			if (codeWindow.textEditor.targetGuid == guid)
			{
				codeWindow.Focus();
				if (line >= 0)
					codeWindow.PingLine(line);
				return;
			}
		}

		string path = AssetDatabase.GUIDToAssetPath(guid);
		Object target = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)) as MonoScript;
		if (target == null)
		{
			target = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
			if (target == null)
			{
				target = AssetDatabase.LoadAssetAtPath(path, typeof(Shader)) as Shader;
				if (target == null)
					return;
			}
		}

		FGCodeWindow newWindow = OpenNewWindow(target);
		if (newWindow != null && line >= 0)
			newWindow.PingLine(line);
	}

	private void PingLine(int line)
	{
		pingLineWhenLoaded = line;
		EditorApplication.update -= PingLineWhenLoaded;
		EditorApplication.update += PingLineWhenLoaded;
	}

	public static FGCodeWindow OpenNewWindow()
	{
		return OpenNewWindow(null, null, false);
	}

	public static FGCodeWindow OpenNewWindow(Object target)
	{
		return OpenNewWindow(target, null, false);
	}

	public static FGCodeWindow OpenNewWindow(Object target, FGCodeWindow nextTo)
	{
		return OpenNewWindow(target, nextTo, false);
	}

	public static FGCodeWindow OpenNewWindow(Object target, FGCodeWindow nextTo, bool reuseExisting)
	{
		if (reuseExisting || target == null)
		{
			if (target == null)
				target = Selection.activeObject as MonoScript;
			if (target == null)
				target = Selection.activeObject as TextAsset;
			if (target == null)
				target = Selection.activeObject as Shader;
			if (target == null)
				return null;

			string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(target));

			foreach (FGCodeWindow codeWindow in codeWindows)
			{
				if (codeWindow.textEditor.targetGuid == guid)
				{
					codeWindow.Focus();
					return codeWindow;
				}
			}
		}

		useTargetAsset = target;
		FGCodeWindow window = ScriptableObject.CreateInstance<FGCodeWindow>();
		useTargetAsset = null;

		if (!window.TryDockNextToSimilarTab(nextTo))
			window.Show();

		window.Focus();
		return window;
	}

	private bool TryDockNextToSimilarTab(FGCodeWindow nextTo)
	{
		if (API.windowsField == null || API.mainViewField == null || API.panesField == null || API.addTabMethod == null)
			return false;
		
		System.Array windows = API.windowsField.GetValue(null, new object[0]) as System.Array;
		if (windows == null)
			return false;

		foreach (var window in windows)
		{
			var mainView = API.mainViewField.GetValue(window, new object[0]);
			System.Array allChildren = API.allChildrenField.GetValue(mainView, new object[0]) as System.Array;
			if (allChildren == null)
				continue;

		    foreach (var view in allChildren)
		    {
				if (view.GetType() != API.dockAreaType)
					continue;

				List<EditorWindow> panes = API.panesField.GetValue(view) as List<EditorWindow>;
				if (panes == null)
					continue;

				if (nextTo != null ? panes.Contains(nextTo) : panes.Find((EditorWindow pane) => pane is FGCodeWindow))
				{
					API.addTabMethod.Invoke(view, new object[] { this });
					return true;
				}
		    }
		}
		return false;
	}
	
	private FGCodeWindow GetAdjacentCodeTab(bool right)
	{
		var parent = API.parentField.GetValue(this);
		if (parent == null || parent.GetType() != API.dockAreaType)
			return null;
		
		List<EditorWindow> panes = API.panesField.GetValue(parent) as List<EditorWindow>;
		if (panes == null)
			return null;

		int index = panes.FindIndex(wnd => wnd == this);
		if (index < 0)
			return null;

		if (right)
		{
			if (index + 1 < panes.Count)
				index = panes.FindIndex(index + 1, wnd => wnd is FGCodeWindow);
			else
				index = -1;
		}
		else
		{
			if (index > 0)
				index = panes.FindLastIndex(index - 1, wnd => wnd is FGCodeWindow);
			else
				index = -1;
		}
		if (index >= 0)
			return panes[index] as FGCodeWindow;
		
		return null;
	}
	
	private void SelectAdjacentCodeTab(bool right)
	{
		FGCodeWindow codeTab = GetAdjacentCodeTab(right);
		if (codeTab != null)
			codeTab.Focus();
	}
	
	private void MoveThisTab(bool right)
	{
		var parent = API.parentField.GetValue(this);
		if (parent == null || parent.GetType() != API.dockAreaType)
			return;
		
		List<EditorWindow> panes = API.panesField.GetValue(parent) as List<EditorWindow>;
		if (panes == null)
			return;

		int index = panes.FindIndex(wnd => wnd == this);
		if (index < 0)
			return;

		if (right && index < panes.Count - 1)
		{
			panes[index] = panes[index + 1];
			panes[index + 1] = this;
			Focus();
			Repaint();
		}
		else if (!right && index > 0)
		{
			panes[index] = panes[index - 1];
			panes[index - 1] = this;
			Focus();
			Repaint();
		}
	}
	
	//private void ToggleMaximized()
	//{
	//}
	
	private void OnEnable()
	{
		codeWindows.Add(this);

		hideFlags = HideFlags.HideAndDontSave;
		textEditor.onRepaint = /*Repaint;
		textEditor.onChange =*/ OnTextBufferChanged;

		if (targetAsset == null)
			targetAsset = useTargetAsset;
		if (targetAsset == null && !string.IsNullOrEmpty(targetAssetGuid))
		{
			string path = AssetDatabase.GUIDToAssetPath(targetAssetGuid);
			if (!string.IsNullOrEmpty(path))
				targetAsset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
		}
		if (targetAsset == null)
			targetAsset = Selection.activeObject as MonoScript;
		if (targetAsset == null)
			targetAsset = Selection.activeObject as TextAsset;
		if (targetAsset == null)
			targetAsset = Selection.activeObject as Shader;

		if (targetAsset != null)
			targetAssetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(targetAsset));

		EditorApplication.update -= OnFirstUpdate;
		EditorApplication.update += OnFirstUpdate;
	}

	private void OnFirstUpdate()
	{
		EditorApplication.update -= OnFirstUpdate;

		if (targetAsset != null)
		{
			title = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(targetAsset));
			textEditor.OnEnable(targetAsset);
			UpdateWindowTitle();
		}
	}

	public static void CheckAssetRename(string guid)
	{
		foreach (FGCodeWindow wnd in codeWindows)
		{
			if (wnd.targetAssetGuid == guid)
			{
				wnd.title = System.IO.Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid));
				wnd.UpdateWindowTitle();
				wnd.Repaint();
			}
		}
	}

	private void PingLineWhenLoaded()
	{
		if (textEditor.CanEdit())
		{
			EditorApplication.update -= PingLineWhenLoaded;
			textEditor.PingLine(pingLineWhenLoaded);
		}
	}

	private void OnDestroy()
	{
		EditorApplication.update -= InitOnLoad;
		EditorApplication.update -= OnFirstUpdate;
		EditorApplication.update -= PingLineWhenLoaded;
	}
	
	private void OnDisable()
	{
		codeWindows.Remove(this);
		textEditor.onRepaint = null;
		//textEditor.onChange = null;
		textEditor.OnDisable();
	}
	
	private void OnTextBufferChanged()
	{
		UpdateWindowTitle();
		Repaint();
	}
	
	private void OnGUI()
	{
		switch (Event.current.type)
		{
			case EventType.KeyDown:
				if ((Event.current.modifiers & ~EventModifiers.FunctionKey) == EventModifiers.Control &&
					(Event.current.keyCode == KeyCode.PageUp || Event.current.keyCode == KeyCode.PageDown))
				{
					SelectAdjacentCodeTab(Event.current.keyCode == KeyCode.PageDown);
					Event.current.Use();
					GUIUtility.ExitGUI();
				}
				else if (Event.current.alt && EditorGUI.actionKey)
				{
					if (Event.current.keyCode == KeyCode.RightArrow || Event.current.keyCode == KeyCode.LeftArrow)
					{
						if (Event.current.shift)
						{
							MoveThisTab(Event.current.keyCode == KeyCode.RightArrow);
						}
						else
						{
							SelectAdjacentCodeTab(Event.current.keyCode == KeyCode.RightArrow);
						}
						Event.current.Use();
						GUIUtility.ExitGUI();
					}
				}
				else if (!Event.current.alt && !Event.current.shift && EditorGUI.actionKey)
				{
					if (Event.current.keyCode == KeyCode.W || Event.current.keyCode == KeyCode.F4)
					{
						Event.current.Use();
						FGCodeWindow codeTab = GetAdjacentCodeTab(false);
						if (codeTab == null)
							codeTab = GetAdjacentCodeTab(true);
						Close();
						if (codeTab != null)
							codeTab.Focus();
					}
				}
				//else if (!Event.current.alt && !Event.current.shift && EditorGUI.actionKey)
				//{
				//	if (Event.current.keyCode == KeyCode.M)
				//	{
				//		Event.current.Use();
				//		ToggleMaximized();
				//		GUIUtility.ExitGUI();
				//	}
				//}
				break;

			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (DragAndDrop.objectReferences.Length > 0)
				{
					bool ask = false;

					HashSet<Object> accepted = new HashSet<Object>();
					foreach (Object obj in DragAndDrop.objectReferences)
					{
						if (AssetDatabase.GetAssetPath(obj).EndsWith(".dll", System.StringComparison.OrdinalIgnoreCase))
							continue;
						
						if (obj is MonoScript)
							accepted.Add(obj);
						else if (obj is TextAsset || obj is Shader)
							accepted.Add(obj);
						else if (obj is Material)
						{
							Material material = obj as Material;
							if (material.shader != null)
							{
								int shaderID = material.shader.GetInstanceID();
								if (shaderID != 0)
								{
									if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(shaderID)))
										accepted.Add(material.shader);
								}
							}
						}
						else if (obj is GameObject)
						{
							GameObject gameObject = obj as GameObject;
							MonoBehaviour[] monoBehaviours = gameObject.GetComponents<MonoBehaviour>();
							foreach (MonoBehaviour mb in monoBehaviours)
							{
								MonoScript monoScript = MonoScript.FromMonoBehaviour(mb);
								if (monoScript != null)
								{
									if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(monoScript)))
									{
										accepted.Add(monoScript);
										ask = true;
									}
								}
							}
						}
					}

					if (accepted.Count > 0)
					{
						DragAndDrop.AcceptDrag();
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						if (Event.current.type == EventType.DragPerform)
						{
							Object[] sorted = accepted.OrderBy((x) => x.name, System.StringComparer.OrdinalIgnoreCase).ToArray();

							if (ask && sorted.Length > 1)
							{
								GenericMenu popupMenu = new GenericMenu();
								foreach (Object target in sorted)
								{
									Object tempTarget = target;
									popupMenu.AddItem(
										new GUIContent("Open " + System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(target))),
										false,
										() => { OpenNewWindow(tempTarget, this, true); });
								}
								popupMenu.AddSeparator("");
								popupMenu.AddItem(
									new GUIContent("Open All"),
									false,
									() => { foreach (Object target in sorted) OpenNewWindow(target, this); });
								
								popupMenu.ShowAsContext();
							}
							else
							{
								foreach (Object target in sorted)
									OpenNewWindow(target, this, sorted.Length == 1);
							}
						}
						Event.current.Use();
						return;
					}
				}
				break;

			case EventType.ValidateCommand:
				if (Event.current.commandName == "ScriptInspector.AddTab")
				{
					Event.current.Use();
					return;
				}
				break;

			case EventType.ExecuteCommand:
				if (Event.current.commandName == "ScriptInspector.AddTab")
				{
					Event.current.Use();
					OpenNewWindow(targetAsset, this);
					return;
				}
				break;
		}
		
		wantsMouseMove = true;
		textEditor.OnWindowGUI(this, new RectOffset(0, 0, 19, 1));
	}

	private void UpdateWindowTitle()
	{
		bool isModified = textEditor.IsModified;
		if (title.StartsWith("*"))
		{
			if (!isModified)
				title = title.Substring(1);
			else
				return;
		}
		else
		{
			if (isModified)
				title = "*" + title;
			else
				return;
		}
		
		foreach (FGCodeWindow wnd in codeWindows)
			if (wnd.targetAssetGuid == targetAssetGuid)
				wnd.UpdateWindowTitle();
	}
}
