#if UNITY_EDITOR && EC_AUTOSAVE
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class AutoSaveWindow : OdinEditorWindow
{
    private double _nextRepaintTime;

    private static readonly float[] IntervalValues =
    {
        180f, 300f, 600f, 900f, 1200f, 1800f, 3600f
    };

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

        _nextRepaintTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += TickRepaint;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EditorApplication.update -= TickRepaint;
    }

    private void TickRepaint()
    {
        if (EditorApplication.timeSinceStartup < _nextRepaintTime)
            return;

        _nextRepaintTime = EditorApplication.timeSinceStartup + 0.2;
        Repaint();
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

    [ShowInInspector, ReadOnly]
    private string NextSave =>
        AutoSave.IsEnabled
            ? FormatTime(AutoSave.TimeToNextSave)
            : "Disabled";

    [Button]
    private void SaveNow()
    {
        AutoSave.ForceSave();
    }

    public enum SaveInterval
    {
        m3,
        m5,
        m10,
        m15,
        m20,
        m30,
        m60
    }

    private SaveInterval SecondsToEnum(float seconds)
    {
        for (int i = 0; i < IntervalValues.Length; i++)
        {
            if (Mathf.Approximately(IntervalValues[i], seconds))
                return (SaveInterval)i;
        }

        return SaveInterval.m5;
    }

    private float EnumToSeconds(SaveInterval interval)
    {
        return IntervalValues[(int)interval];
    }

    private string FormatTime(double t)
    {
        if (t < 0)
            return "-";

        int total = (int)t;
        return $"{total / 60:00}:{total % 60:00}";
    }
}
#endif