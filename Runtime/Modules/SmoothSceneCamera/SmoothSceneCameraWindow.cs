#if UNITY_EDITOR && EC_SMOOTH_SCENE_CAMERA
using UnityEditor;
using UnityEngine;

namespace SmoothSceneCamera
{
    public class SmoothSceneCameraWindow : EditorWindow
    {
        [MenuItem("EC/Modules/Smooth Scene Camera")]
        public static void Open()
        {
            var window = GetWindow<SmoothSceneCameraWindow>();
            window.minSize = new Vector2(270.0f, 270.0f);
            window.name = "Smooth Camera";
            window.titleContent = new GUIContent("Smooth Camera", Sirenix.Utilities.Editor.EditorIcons.Image.Active);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Zoom", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            bool useSmoothZoom = EditorGUILayout.Toggle(
                new GUIContent("Smooth Zoom", "Toggles the smooth zoom option"),
                SmoothSceneCamera.UseSmoothZoom);

            float zoomAmount = EditorGUILayout.FloatField(
                new GUIContent("Zoom Amount", "Amount of zoom per scroll step"),
                SmoothSceneCamera.ZoomAmount);

            float zoomDistancePower = EditorGUILayout.FloatField(
                new GUIContent("Zoom Distance Power", "Exponent multiplier"),
                SmoothSceneCamera.ZoomDistancePower);

            float zoomDuration = EditorGUILayout.FloatField(
                new GUIContent("Zoom Duration", "Duration in seconds"),
                SmoothSceneCamera.ZoomDuration);

            var zoomEasing = (Easings.Type)EditorGUILayout.EnumPopup(
                new GUIContent("Zoom Easing", "Easing type"),
                SmoothSceneCamera.ZoomEasing);

            float nearZoomLimit = EditorGUILayout.FloatField(
                new GUIContent("Near Zoom Limit", "Minimum distance"),
                SmoothSceneCamera.NearZoomLimit);

            if (EditorGUI.EndChangeCheck())
            {
                SmoothSceneCamera.UseSmoothZoom = useSmoothZoom;
                SmoothSceneCamera.ZoomAmount = zoomAmount;
                SmoothSceneCamera.ZoomDistancePower = zoomDistancePower;
                SmoothSceneCamera.ZoomDuration = zoomDuration;
                SmoothSceneCamera.ZoomEasing = zoomEasing;
                SmoothSceneCamera.NearZoomLimit = nearZoomLimit;
            }
        }
    }
}
#endif