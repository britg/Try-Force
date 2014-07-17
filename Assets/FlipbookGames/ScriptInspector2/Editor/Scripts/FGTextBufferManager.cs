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
using System.Runtime.InteropServices;
using System.Collections.Generic;

[Serializable, StructLayout(LayoutKind.Sequential)]
public class FGTextBufferManager : ScriptableObject
{
	[SerializeField]
	private List<FGTextBuffer> allBuffers = new List<FGTextBuffer>();

	private static bool reloadingAssembly = false;

	static FGTextBufferManager()
	{
		EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;
		EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
		AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
		AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
	}

	private void OnEnable()
	{
		hideFlags = HideFlags.HideAndDontSave;
		if (_instance == null)
			_instance = this;
		else if (_instance != this)
			Debug.LogError("Multiple Managers!!!");
	}

	static void CurrentDomain_DomainUnload(object sender, EventArgs e)
	{
		if (!reloadingAssembly)
		{
			SaveAllModified(true);
		}
	}

	//[PostProcessScene]
	//private static void OnBuild()
	//{
	//    SaveAllModified(false);
	//    //AssetDatabase.SaveAssets();
	//}

	private static void OnPlaymodeStateChanged()
	{
		if (_instance == null)
			return;

		if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
			SaveAllModified(false);
	}

	public static void SaveAllModified(bool onQuit)
	{
		if (_instance == null)
			return;
		
		bool locked = false;
		try
		{
			foreach (FGTextBuffer buffer in instance.allBuffers)
			{
				if (buffer == null)
					continue;

				if (buffer.IsModified)
				{
					string path = AssetDatabase.GUIDToAssetPath(buffer.guid);
					if (onQuit && !EditorUtility.DisplayDialog("Script Inspector 2", "Save changes to the following asset?\n\n" + path, "Save", "Don't Save"))
						continue;

					if (!onQuit && !locked)
					{
						EditorApplication.LockReloadAssemblies();
						locked = true;
					}
		
					buffer.Save();
					if (!onQuit)
					{
						AssetDatabase.ImportAsset(path, ImportAssetOptions.Default);
						var asset = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
						if (asset != null)
							EditorUtility.SetDirty(asset);
						buffer.UpdateViews();
					}
				}
			}
		}
		catch (Exception e) { Debug.LogError(e); }
		finally
		{
			if (locked)
			{
				EditorApplication.UnlockReloadAssemblies();
			}
		}
	}

	private static FGTextBufferManager _instance = null;
	public static FGTextBufferManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = ScriptableObject.CreateInstance<FGTextBufferManager>();
				_instance.hideFlags = HideFlags.HideAndDontSave;
			}
			return _instance;
		}
	}

	public static FGTextBuffer GetBuffer(UnityEngine.Object target)
	{
		string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(target));

		List<FGTextBuffer> buffers = instance.allBuffers.FindAll(x => x != null && guid == x.guid);
		if (buffers.Count > 0)
		{
			if (buffers.Count > 1)
			{
				Debug.Log("Removing " + (buffers.Count - 1) + " duplicates...");
				for (int i = 1; i < buffers.Count; ++i)
					instance.allBuffers.Remove(buffers[i]);
//				EditorUtility.SetDirty(instance);
			}
			return buffers[0];
		}

		FGTextBuffer buffer = CreateInstance<FGTextBuffer>();
		instance.allBuffers.Add(buffer);
		buffer.guid = guid;
//		EditorUtility.SetDirty(instance);
		return buffer;
	}

	public static void DestroyBuffer(FGTextBuffer buffer)
	{
		instance.allBuffers.Remove(buffer);
		DestroyImmediate(buffer);
	}

	public class FGScriptPostprocessor : AssetPostprocessor
	{
		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			if (_instance == null)
				return;

			foreach (string imported in importedAssets)
			{
				if (imported.EndsWith(".js", StringComparison.OrdinalIgnoreCase) ||
					imported.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) ||
					imported.EndsWith(".boo", StringComparison.OrdinalIgnoreCase))
				{
					if (!Array.Exists(movedAssets, (string path) => imported == path))
					{
						instance.OnAssetReimported(imported);
					}

					reloadingAssembly = true;
				}
				else
				{
					instance.OnAssetReimported(imported);
				}
			}

			//foreach (string str in deletedAssets)
			//    Debug.Log("== Deleted Asset: " + str);

			for (int i = 0; i < movedAssets.Length; ++i)
			{
				if (movedAssets[i].EndsWith(".js", StringComparison.OrdinalIgnoreCase) ||
					movedAssets[i].EndsWith(".cs", StringComparison.OrdinalIgnoreCase) ||
					movedAssets[i].EndsWith(".boo", StringComparison.OrdinalIgnoreCase))
				{
					instance.OnAssetMoved(movedAssets[i]);
				}
			}
			
			if (reloadingAssembly)
			{
				EditorApplication.update -= CompileErrorsCheck;
				EditorApplication.update += CompileErrorsCheck;
			}
		}
	}

	private static void CompileErrorsCheck()
	{
		if (EditorApplication.isCompiling)
			return;

		EditorApplication.update -= CompileErrorsCheck;
		reloadingAssembly = false;
		//EditorUtility.DisplayDialog("Script Inspector 2", "Compile errors!", "OK");
		FGTextEditor.RepaintAllInstances();
	}
	
	public void OnAssetReimported(string assetPath)
	{
		string guid = AssetDatabase.AssetPathToGUID(assetPath);
		FGTextBuffer buffer = allBuffers.Find((FGTextBuffer x) => guid == x.guid);
		if (buffer != null)
		{
			buffer.Reload();
		}
	}

	public void OnAssetMoved(string assetPath)
	{
		string guid = AssetDatabase.AssetPathToGUID(assetPath);
		FGTextBuffer buffer = allBuffers.Find(x => guid == x.guid);
		if (buffer != null)
		{
			buffer.justSavedNow = true;
		}
	}
}
