#if UNITY_EDITOR
#pragma warning disable IDE1006 // Стили именования
using UnityEngine;
using UnityEditor;
using System.Timers;
using System;
using System.Diagnostics;

[InitializeOnLoad]
public class AutoSave : EditorWindow
{
	public static AutoSave instance = null;
	public static Texture2D logo = null;
	protected static System.Timers.Timer timer = null;
	protected static int hierarchyChangeCount = 0;
	protected static string logoPath = "Assets/Sixpolys/SIXP Autosaver/Editor/SixpolysLogo.png";
	protected static bool _saveNow = false;
	protected static bool savedBeforePlay = false;
	protected static bool saveAfterPlay = false;
	protected static Stopwatch stw1 = null;

	private const int saveInterval = 300000; // 5 minutes
	private const int hierarchyChangeCountTrigger = 10;

    [MenuItem("Window/Autosave Settings")]
	public static void ShowWindow ()
	{
		var window = EditorWindow.GetWindow<AutoSave> ();
		window.maxSize = new Vector2 (window.maxSize.x, 50);
		window.minSize = new Vector2 (0, 50);
	}

    [Obsolete]
    public static void LoadPreferences ()
	{
			if (timer == null) {
				timer = new System.Timers.Timer ();
				timer.Interval = saveInterval; // 5 minutes
            timer.Elapsed += new  ElapsedEventHandler (timerFired);
				timer.Start ();
			} else {
				if (timer.Interval != saveInterval) {
					timer.Interval = saveInterval;
				}
			}
		EditorApplication.hierarchyWindowChanged -= HierarchyChanged;
		EditorApplication.playmodeStateChanged -= playModeChanged;
		EditorApplication.hierarchyWindowChanged += HierarchyChanged;
		EditorApplication.playmodeStateChanged += playModeChanged;

		if (instance != null) {
			instance.Repaint ();
		}

	}

    [Obsolete]
    public static void playModeChanged ()
	{
		if (EditorApplication.isPlayingOrWillChangePlaymode && !savedBeforePlay) {
			savedBeforePlay = true;
			executeSave ();
		} else if (!EditorApplication.isPaused && !EditorApplication.isPlaying) {
			if (saveAfterPlay) {
				executeSave ();
			}
		}
	}

    [Obsolete]
    public static void HierarchyChanged ()
	{
		if (!EditorApplication.isPlaying) {
			hierarchyChangeCount++;
			if (hierarchyChangeCount >= hierarchyChangeCountTrigger) {
				hierarchyChangeCount = 0;
				executeSave ();
			}
		}
	}

	public static void timerFired (object sender, ElapsedEventArgs args)
	{
		if (!_saveNow) {
			_saveNow = true;
		}
	}
    
	[Obsolete]
    public static void executeSave ()
    {
		stw1.Stop ();
		stw1.Reset ();

		if (EditorApplication.isCompiling || BuildPipeline.isBuildingPlayer) {
			return;
		}

		// don't save during running game
		if (EditorApplication.isPlaying || EditorApplication.isPaused) {
			saveAfterPlay = true;
			stw1.Start ();
			return;
		}
		saveAfterPlay = false;

			string sceneName = EditorApplication.currentScene;
		if ((sceneName == "" || sceneName.StartsWith ("Untitled"))) {
			stw1.Start ();
			return;
		}

#if UNITY_5_3
		EditorSceneManager.SaveOpenScenes ();
#else
		EditorApplication.SaveScene ();
#endif
			AssetDatabase.SaveAssets ();
		if (instance != null) {
			instance.Repaint ();
		}
		stw1.Start ();
	}

	[InitializeOnLoadMethod]
    [Obsolete]
	public static void InitAutosave ()
	{
		stw1 = new Stopwatch ();
		stw1.Start ();
		logo = (Texture2D)AssetDatabase.LoadAssetAtPath (logoPath, typeof(Texture2D));
		EditorApplication.update += EditorUpdate;
		LoadPreferences ();
	}
    [Obsolete]
    public static void EditorUpdate ()
	{
		if (_saveNow) {
			_saveNow = false;
			executeSave ();
		}
		if (instance != null) {
			instance.Repaint ();
		}
	}

	public void OnEnable ()
	{
		instance = this;
	}
}
#pragma warning restore IDE1006 // Стили именования
#endif