#if UNITY_EDITOR && EC_AUTOSAVE
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class AutoSave
{
	static double _lastSaveTime;
	static double _blockUntilTime;

	static AutoSave()
	{
		_lastSaveTime = EditorApplication.timeSinceStartup;

		EditorApplication.playModeStateChanged += OnPlayModeChanged;
		EditorApplication.update += Update;
	}

	public static bool IsEnabled
	{
		get => EditorPrefs.GetBool("EC_AutoSaveEnabled", true);
		set => EditorPrefs.SetBool("EC_AutoSaveEnabled", value);
	}

	public static float SaveInterval
	{
		get => EditorPrefs.GetFloat("EC_AutoSaveInterval", 300f);
		set => EditorPrefs.SetFloat("EC_AutoSaveInterval", value);
	}

	public static bool IsLogging
	{
		get => EditorPrefs.GetBool("EC_AutoSaveLogging", false);
		set => EditorPrefs.SetBool("EC_AutoSaveLogging", value);
	}

	public static double TimeToNextSave =>
		IsEnabled ? Mathf.Max(0, SaveInterval - (float)(EditorApplication.timeSinceStartup - _lastSaveTime)) : -1;

	static void Update()
	{
		if (!CanSave())
			return;

		if (!HasDirtyScenes())
			return;

		Save();
	}

	static bool CanSave()
	{
		if (!IsEnabled)
			return false;

		if (EditorApplication.isCompiling)
			return false;

		if (EditorApplication.isPlayingOrWillChangePlaymode)
			return false;

		if (EditorApplication.isPlaying || EditorApplication.isPaused)
			return false;

		if (EditorApplication.timeSinceStartup < _blockUntilTime)
			return false;

		if (QuantumGuard.IsRunning)
			return false;

		if (EditorApplication.timeSinceStartup - _lastSaveTime < SaveInterval)
			return false;

		return true;
	}

	static bool HasDirtyScenes()
	{
		for (int i = 0; i < EditorSceneManager.sceneCount; i++)
		{
			if (EditorSceneManager.GetSceneAt(i).isDirty)
				return true;
		}
		return false;
	}

	static void Save()
	{
		EditorSceneManager.SaveOpenScenes();
		AssetDatabase.SaveAssets();

		_lastSaveTime = EditorApplication.timeSinceStartup;

		if (IsLogging)
			Debug.Log("AutoSave: Saved");
	}

	static void OnPlayModeChanged(PlayModeStateChange state)
	{
		if (state == PlayModeStateChange.EnteredEditMode)
			_blockUntilTime = EditorApplication.timeSinceStartup + 2.0;
	}

	public static void ForceSave()
	{
		if (!HasDirtyScenes())
			return;

		Save();
	}
}
#endif