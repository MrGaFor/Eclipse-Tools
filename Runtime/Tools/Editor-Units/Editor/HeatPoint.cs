#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.IO;

namespace Eclipse.Tools
{
    public class HeatPoint : EditorWindow
    {
        [MenuItem("Tools/Eclipse/Heat point")]
        public static void ShowWindow()
        {
            var window = GetWindow<HeatPoint>("Heat point");
            window.minSize = new Vector2(400, 400);
        }

        [System.Serializable]
        private class PointsData
        {
            public List<PointData> _points;

            [System.Serializable]
            public class PointData
            {
                public Vector3 _position;
                public Vector3 _rotation;
            }
        }

        private string _savePath = "Assets/ThirdParties/EclipseTools/BigTools/Editor/HeatPointSave.json";
        private void Save()
        {
            string directory = Path.GetDirectoryName(_savePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            File.WriteAllText(_savePath, JsonUtility.ToJson(_points));
        }
        private void Load()
        {
            if (File.Exists(_savePath))
            {
                string text = File.ReadAllText(_savePath);
                _points = JsonUtility.FromJson<PointsData>(text);
            }
            else
            {
                _points = new PointsData();
                Save();
            }
        }

        private Transform[] GetSelectionTransform()
        {
            Transform[] objs = new Transform[Selection.gameObjects.Length];
            for (int i = 0; i < objs.Length; i++)
                objs[i] = Selection.gameObjects[i].transform;
            return objs;
        }
        private void SetSelectionTransform(Transform[] objs)
        {
            Object[] selection = new Object[objs.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                selection[i] = objs[i].gameObject;
            }
            Selection.objects = selection;
        }
        private void SetPoint(int id)
        {
            foreach (Transform obj in _targets)
            {
                obj.position = _points._points[id]._position;
                obj.rotation = Quaternion.Euler(_points._points[id]._rotation);
            }
        }
        private Vector2 _scrollPoints;
        private int _changeId;
        private Transform[] _targets;
        private PointsData _points;

        [Obsolete]
        private void OnGUI()
        {

            GUILayout.Label("Object", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Camera")) _targets = new Transform[] { Camera.main.transform };
            if (GUILayout.Button("Selection")) _targets = GetSelectionTransform();
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Ping")) SetSelectionTransform(_targets);

            GUILayout.Label("Points", EditorStyles.centeredGreyMiniLabel);
            GUILayout.BeginVertical();
            if (_points == null)
                _points = new PointsData();
            if (_points._points == null)
                _points._points = new List<PointsData.PointData>();
            if (_points._points.Count != 0)
            {
                for (int i = 0; i < _points._points.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(i.ToString(), GUILayout.Width(15));
                    if (GUILayout.Button("Set", GUILayout.Width(70), GUILayout.Height(40))) SetPoint(i);
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Position", GUILayout.Width(60));
                    _points._points[i]._position = EditorGUILayout.Vector3Field("", _points._points[i]._position);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Rotation", GUILayout.Width(60));
                    _points._points[i]._rotation = EditorGUILayout.Vector3Field("", _points._points[i]._rotation);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save", GUILayout.Width(70))) Save();
            if (GUILayout.Button("Load", GUILayout.Width(70))) Load();

            if (GUILayout.Button("Add", GUILayout.Width(70))) _points._points.Insert(_changeId, new PointsData.PointData());
            _changeId = EditorGUILayout.IntField("", _changeId, GUILayout.Width(30));
            if (GUILayout.Button("Del", GUILayout.Width(70))) _points._points.RemoveAt(_changeId);
            GUILayout.EndHorizontal();
        }
    }
}
#endif