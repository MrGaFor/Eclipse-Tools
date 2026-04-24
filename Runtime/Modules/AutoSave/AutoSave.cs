#if UNITY_EDITOR && EC_AUTOSAVE
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class AutoSave
{
	private static double _lastSaveTime;

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

	public static double TimeToNextSave
	{
		get
		{
			if (!IsEnabled)
				return -1;

			var elapsed = EditorApplication.timeSinceStartup - _lastSaveTime;
			return Mathf.Max(0, SaveInterval - (float)elapsed);
		}
	}

	static AutoSave()
	{
		_lastSaveTime = EditorApplication.timeSinceStartup;

		EditorApplication.update -= EditorUpdate;
		EditorApplication.update += EditorUpdate;
	}

	private static void EditorUpdate()
	{
		if (!IsEnabled)
			return;

		if (EditorApplication.isCompiling || BuildPipeline.isBuildingPlayer)
			return;

		if (EditorApplication.isPlaying || EditorApplication.isPaused)
			return;

		if (EditorApplication.timeSinceStartup - _lastSaveTime < SaveInterval)
			return;

		if (!HasValidScene())
			return;

		if (!IsDirty())
		{
			_lastSaveTime = EditorApplication.timeSinceStartup;
			return;
		}

		ExecuteSave();
	}

	private static bool HasValidScene()
	{
		var scene = EditorSceneManager.GetActiveScene();
		return scene.IsValid() && !string.IsNullOrEmpty(scene.path);
	}

	private static bool IsDirty()
	{
		return EditorSceneManager.GetActiveScene().isDirty;
	}

	public static void ForceSave()
	{
		if (!HasValidScene())
			return;

		ExecuteSave();
	}

	private static void ExecuteSave()
	{
		EditorSceneManager.SaveOpenScenes();
		AssetDatabase.SaveAssets();

		_lastSaveTime = EditorApplication.timeSinceStartup;

		if (IsLogging)
			Debug.Log("AutoSave: Saved");
	}
}
#endif