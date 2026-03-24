#if UNITY_EDITOR && EC_TIMESCALE_TOOLBAR
using UnityEditor;
using UnityEngine;
using System;
using UnityToolbarExtender;

namespace TimeScaleToolbar
{
    [InitializeOnLoad]
    public class TimeScaleToolbar
    {
        static readonly string key_timeScale = "TimeScaleToolbar_TimeScale";

        const float maxScale = 5f;

        static float _timeScale = 1f;
        static float timeScale
        {
            get => _timeScale;
            set
            {
                if (_timeScale != value)
                {
                    _timeScale = value;
                    EditorPrefs.SetFloat(key_timeScale, _timeScale);
                }
            }
        }

        static readonly float _sliderWidth = 150f;

        static TimeScaleToolbar()
        {
            timeScale = Mathf.Min(EditorPrefs.GetFloat(key_timeScale, 1f), maxScale);
            SetVisibility();

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void UpdateDrawCallbacks()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }

        static void SetVisibility()
        {
            UpdateDrawCallbacks();
            SceneView.RepaintAll();
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Time.timeScale = timeScale;
            }
        }

        static void OnToolbarGUI()
        {
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(1));
            GUI.DrawTexture(new Rect(0, 0, 17, 17), Sirenix.Utilities.Editor.EditorIcons.Clock.Active);

            GUILayout.Space(22);
            timeScale = GUILayout.HorizontalSlider(timeScale, 0f, maxScale, GUILayout.Width(_sliderWidth));

            GUILayout.Space(4);
            GUILayout.Label(timeScale.ToString("F2"), GUILayout.Width(45));
            GUILayout.Space(-15);

            GUIStyle style = EditorStyles.iconButton;
            style.fixedHeight = 17;
            style.fixedWidth = 17;
            if (GUILayout.Button(Sirenix.Utilities.Editor.EditorIcons.Refresh.Active, style))
            {
                timeScale = 1f;
                Time.timeScale = timeScale;
            }
            GUILayout.EndHorizontal();

            if (Time.timeScale != timeScale)
                Time.timeScale = timeScale;
        }
    }
}
#endif