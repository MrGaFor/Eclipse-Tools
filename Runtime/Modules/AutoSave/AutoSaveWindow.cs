#if UNITY_EDITOR && EC_AUTOSAVE
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class AutoSaveWindow : OdinEditorWindow
{
    [MenuItem("EC/Modules/AutoSave")]
    private static void Open()
    {
        var window = GetWindow<AutoSaveWindow>();
        window.minSize = new Vector2(300, 150);
        window.Show();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EditorApplication.update += Repaint;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EditorApplication.update -= Repaint;
    }

    [ShowInInspector]
    public bool Enabled
    {
        get => AutoSave.IsEnabled;
        set => AutoSave.IsEnabled = value;
    }

    [ShowInInspector]
    public SaveInterval Interval
    {
        get => SecondsToEnum(AutoSave.SaveInterval);
        set => AutoSave.SaveInterval = EnumToSeconds(value);
    }

    [ShowInInspector]
    public bool Logging
    {
        get => AutoSave.IsLogging;
        set => AutoSave.IsLogging = value;
    }

    [OnInspectorGUI]
    private void DrawTimer()
    {
        if (!AutoSave.IsEnabled)
            return;

        GUILayout.Label($"Next Save In: {FormatTime()}");
    }
    private string FormatTime()
    {
        var t = AutoSave.TimeToNextSave;
        if (t < 0) return "-";

        int min = (int)t / 60;
        int sec = (int)t % 60;
        return $"{min:00}:{sec:00}";
    }

    [Button]
    private void SaveNow()
    {
        AutoSave.ForceSave();
    }

    public enum SaveInterval { m3, m5, m10, m15, m20, m30, m60 }

    private SaveInterval SecondsToEnum(float seconds)
    {
        if (seconds == 180) return SaveInterval.m3;
        if (seconds == 300) return SaveInterval.m5;
        if (seconds == 600) return SaveInterval.m10;
        if (seconds == 900) return SaveInterval.m15;
        if (seconds == 1200) return SaveInterval.m20;
        if (seconds == 1800) return SaveInterval.m30;
        return SaveInterval.m60;
    }

    private float EnumToSeconds(SaveInterval interval)
    {
        switch (interval)
        {
            case SaveInterval.m3: return 180;
            case SaveInterval.m5: return 300;
            case SaveInterval.m10: return 600;
            case SaveInterval.m15: return 900;
            case SaveInterval.m20: return 1200;
            case SaveInterval.m30: return 1800;
            case SaveInterval.m60: return 3600;
            default: return 300;
        }
    }
}
#endif