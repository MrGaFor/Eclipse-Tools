#if UNITY_EDITOR
#pragma warning disable IDE1006 // Стили именования
using UnityEngine;
using UnityEditor;
using System.Timers;
using System;
using System.Diagnostics;

[InitializeOnLoad]
public class AutoSave
{
	protected static Timer timer = null;
	protected static bool _saveNow = false;
	protected static Stopwatch stw1 = null;
	private const int saveInterval = 300000; // 5 minutes

    [InitializeOnLoadMethod]
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    [Obsolete]
    private static void OnRuntimeMethodLoad ()
	{
        _saveNow = false;

		if (stw1 == null)
            stw1.Stop();
        stw1 = new Stopwatch();
        stw1.Start();
        EditorApplication.update -= EditorUpdate;
        EditorApplication.update += EditorUpdate;

		if (timer != null)
			timer.Dispose();
		timer = new Timer();
		timer.Interval = saveInterval;
        timer.Elapsed += new ElapsedEventHandler(timerFired);
        timer.Start();
    }

	public static void timerFired (object sender, ElapsedEventArgs args)
	{
		if (!_saveNow) {
			_saveNow = true;
		}
	}

	[Obsolete]
	public static void executeSave()
	{
		stw1.Stop();
		stw1.Reset();

		if (EditorApplication.isCompiling || BuildPipeline.isBuildingPlayer)
		{
			return;
		}

		// don't save during running game
		if (EditorApplication.isPlaying || EditorApplication.isPaused)
		{
			stw1.Start();
			return;
		}

		string sceneName = EditorApplication.currentScene;
		if ((sceneName == "" || sceneName.StartsWith("Untitled")))
		{
			stw1.Start();
			return;
		}

#if UNITY_5_3
		EditorSceneManager.SaveOpenScenes ();
#else
		EditorApplication.SaveScene();
#endif
		AssetDatabase.SaveAssets();
		stw1.Start();
	}

    [Obsolete]
    public static void EditorUpdate ()
	{
		if (_saveNow) {
			_saveNow = false;
			executeSave ();
		}
	}
}
#pragma warning restore IDE1006 // Стили именования
#endif