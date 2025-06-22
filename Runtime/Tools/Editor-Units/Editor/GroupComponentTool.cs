#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace Eclipse.Tools
{
    public class GroupComponentTool : EditorWindow
    {
        [MenuItem("Tools/Eclipse/Group component settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<GroupComponentTool>("Group component settings");
            window.minSize = new Vector2(350, 350);
        }

        public enum ComponentType { MeshRenderer, GameObject }
        private ComponentType _targetType;
        private bool _useChild;
        private Type GetTargetType()
        {
            switch (_targetType)
            {
                case ComponentType.MeshRenderer: return typeof(MeshRenderer);
                case ComponentType.GameObject: return typeof(Transform);
                default: throw new ArgumentException("Unsupported component type");
            }
        }
        private Component[] GetTargetComponents()
        {
            List<Component> foundComponents = new List<Component>();
            foreach (var obj in Selection.gameObjects)
            {
                if (_useChild)
                    foundComponents.AddRange(obj.GetComponentsInChildren(GetTargetType()));
                else
                    if (obj.TryGetComponent(GetTargetType(), out Component component))
                        foundComponents.Add(component);
            }
            return foundComponents.ToArray();
        }

        private void OnGUI()
        {

            GUILayout.Label("Component", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.BeginHorizontal();
            // use child
            EditorGUILayout.BeginHorizontal(GUILayout.Width(100));
            EditorGUILayout.LabelField("Use Child", GUILayout.Width(70));
            _useChild = EditorGUILayout.Toggle(_useChild, GUILayout.Width(30));
            EditorGUILayout.EndHorizontal();
            // type
            EditorStyles.popup.alignment = TextAnchor.MiddleCenter;
            _targetType = (ComponentType)EditorGUILayout.EnumPopup("", _targetType);
            EditorGUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label($"Selected: {Selection.gameObjects.Length}", EditorStyles.boldLabel);
            GUILayout.Label($"Components: {GetTargetComponents().Length}", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.Label("Settings", EditorStyles.centeredGreyMiniLabel);

            if (_targetType == ComponentType.MeshRenderer)
                MeshRendererGUI();
            else if (_targetType == ComponentType.GameObject)
                GameObjectGUI();
        }

        private int _groupId;
        private void BeginUndoGroup(string groupName)
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName(groupName);
            _groupId = Undo.GetCurrentGroup();
        }
        private void EndUndoGroup()
        {
            Undo.CollapseUndoOperations(_groupId);
        }

        #region MeshRenderer
        private void MeshRendererGUI()
        {
            GUILayout.BeginHorizontal();
            _meshRendererShadow = (ShadowCastingMode)EditorGUILayout.EnumPopup("Shadows", _meshRendererShadow);
            if (GUILayout.Button("🔁", GUILayout.Width(30))) MeshRendererShadow();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _meshRendererStaticShadowCaster = (bool)EditorGUILayout.Toggle("Static shadow caster", _meshRendererStaticShadowCaster);
            if (GUILayout.Button("🔁", GUILayout.Width(30))) MeshRendererStaticShadowCaster();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _meshRendererLightProbes = (LightProbeUsage)EditorGUILayout.EnumPopup("Light probes", _meshRendererLightProbes);
            if (GUILayout.Button("🔁", GUILayout.Width(30))) MeshRendererLightProbes();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _meshRendererMotionVectors = (MotionVectorGenerationMode)EditorGUILayout.EnumPopup("Motion vectors", _meshRendererMotionVectors);
            if (GUILayout.Button("🔁", GUILayout.Width(30))) MeshRendererMotionVectors();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _meshRendererDynamicOcclusion = (bool)EditorGUILayout.Toggle("Dynamic occlusion", _meshRendererDynamicOcclusion);
            if (GUILayout.Button("🔁", GUILayout.Width(30))) MeshRendererDynamicOcclusion();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _meshRendererRenderingLayerMask = EditorGUILayout.MaskField(
        "Rendering Layer Mask", _meshRendererRenderingLayerMask, RenderingLayerMask.GetDefinedRenderingLayerNames());
            if (GUILayout.Button("🔁", GUILayout.Width(30))) MeshRendererRenderingLayerMask();
            GUILayout.EndHorizontal();
        }

        private MeshRenderer[] GetMeshRenderers()
        {
            Component[] components = GetTargetComponents();
            List<MeshRenderer> meshs = new List<MeshRenderer>();
            foreach (var comp in components)
                if (comp is MeshRenderer mesh)
                    meshs.Add(mesh);
            return meshs.ToArray();
        }
        private ShadowCastingMode _meshRendererShadow;
        private bool _meshRendererStaticShadowCaster;
        private LightProbeUsage _meshRendererLightProbes;
        private MotionVectorGenerationMode _meshRendererMotionVectors;
        private bool _meshRendererDynamicOcclusion;
        private int _meshRendererRenderingLayerMask;

        private void MeshRendererShadow()
        {
            BeginUndoGroup("Mesh shadow");
            foreach (MeshRenderer mesh in GetMeshRenderers())
            {
                if (!mesh || mesh.shadowCastingMode == _meshRendererShadow) continue;
                Undo.RecordObject(mesh, "Toggle shadow");
                mesh.shadowCastingMode = _meshRendererShadow;
                EditorUtility.SetDirty(mesh);
            }
            EndUndoGroup();
        }
        private void MeshRendererStaticShadowCaster()
        {
            BeginUndoGroup("Mesh static shadow casting");
            foreach (MeshRenderer mesh in GetMeshRenderers())
            {
                if (!mesh || mesh.staticShadowCaster == _meshRendererStaticShadowCaster) continue;
                Undo.RecordObject(mesh, "Toggle static shadow casting");
                mesh.staticShadowCaster = _meshRendererStaticShadowCaster;
                EditorUtility.SetDirty(mesh);
            }
            EndUndoGroup();
        }
        private void MeshRendererLightProbes()
        {
            BeginUndoGroup("Mesh light probes");
            foreach (MeshRenderer mesh in GetMeshRenderers())
            {
                if (!mesh || mesh.lightProbeUsage == _meshRendererLightProbes) continue;
                Undo.RecordObject(mesh, "Toggle light probes");
                mesh.lightProbeUsage = _meshRendererLightProbes;
                EditorUtility.SetDirty(mesh);
            }
            EndUndoGroup();
        }
        private void MeshRendererMotionVectors()
        {
            BeginUndoGroup("Mesh motion vectors");
            foreach (MeshRenderer mesh in GetMeshRenderers())
            {
                if (!mesh || mesh.motionVectorGenerationMode == _meshRendererMotionVectors) continue;
                Undo.RecordObject(mesh, "Toggle motion vectors");
                mesh.motionVectorGenerationMode = _meshRendererMotionVectors;
                EditorUtility.SetDirty(mesh);
            }
            EndUndoGroup();
        }
        private void MeshRendererDynamicOcclusion()
        {
            BeginUndoGroup("Mesh dynamic occlusion");
            foreach (MeshRenderer mesh in GetMeshRenderers())
            {
                if (!mesh || mesh.allowOcclusionWhenDynamic == _meshRendererDynamicOcclusion) continue;
                Undo.RecordObject(mesh, "Toggle dynamic occlusion");
                mesh.allowOcclusionWhenDynamic = _meshRendererDynamicOcclusion;
                EditorUtility.SetDirty(mesh);
            }
            EndUndoGroup();
        }
        private void MeshRendererRenderingLayerMask()
        {
            BeginUndoGroup("Mesh rendering layer mask");
            foreach (MeshRenderer mesh in GetMeshRenderers())
            {
                if (!mesh || mesh.renderingLayerMask == (uint)_meshRendererRenderingLayerMask) continue;
                Undo.RecordObject(mesh, "Toggle rendering layer mask");
                mesh.renderingLayerMask = (uint)_meshRendererRenderingLayerMask;
                EditorUtility.SetDirty(mesh);
            }
            EndUndoGroup();
        }

        #endregion

        #region GameObject
        private void GameObjectGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", GUILayout.Width(70));
            _gameObjectName = (string)EditorGUILayout.TextField("", _gameObjectName, GUILayout.MinWidth(40));
            EditorGUILayout.LabelField("Delta", GUILayout.Width(70));
            _gameObjectNameDelta = (string)EditorGUILayout.TextField("", _gameObjectNameDelta, GUILayout.MinWidth(40));
            if (GUILayout.Button("🔁", GUILayout.Width(30))) GameObjectName();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Layer", GUILayout.Width(70));
            _gameObjectLayer = EditorGUILayout.LayerField("", _gameObjectLayer);
            if (GUILayout.Button("🔁", GUILayout.Width(30))) GameObjectLayer();
            GUILayout.EndHorizontal();

        }

        private GameObject[] GetGameObjects()
        {
            Component[] components = GetTargetComponents();
            List<GameObject> objs = new List<GameObject>();
            foreach (var comp in components)
                if (comp is Transform obj)
                    objs.Add(obj.gameObject);
            return objs.ToArray();
        }
        private string _gameObjectName;
        private string _gameObjectNameDelta;
        private LayerMask _gameObjectLayer;

        private void GameObjectName()
        {
            if (string.IsNullOrEmpty(_gameObjectName) && string.IsNullOrEmpty(_gameObjectNameDelta))
            {
                Debug.LogError("Name and Delta not equel NULL!");
                return;
            }
            int id = 0;
            BeginUndoGroup("Object name");
            foreach (GameObject obj in GetGameObjects())
            {
                string name = _gameObjectName + _gameObjectNameDelta + id.ToString();
                id++;
                if (obj.name == name) continue;
                string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                if (!string.IsNullOrEmpty(prefabPath))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    if (prefab)
                    {
                        Undo.RecordObject(prefab, "Rename prefab");
                        prefab.name = name;
                        EditorUtility.SetDirty(prefab);
                        AssetDatabase.SaveAssets();
                        string newPath = System.IO.Path.GetDirectoryName(prefabPath) + "/" + name + ".prefab";
                        AssetDatabase.MoveAsset(prefabPath, newPath);
                        AssetDatabase.Refresh();
                    }
                }
                else
                {
                    Undo.RecordObject(obj, "Change name");
                    obj.name = name;
                    EditorUtility.SetDirty(obj);
                }
            }
            EndUndoGroup();
        }
        private void GameObjectLayer()
        {
            BeginUndoGroup("Object layer");
            foreach (GameObject obj in GetGameObjects())
            {
                if (!obj || obj.layer == _gameObjectLayer) continue;
                Undo.RecordObject(obj, "Change layer");
                obj.layer = _gameObjectLayer;
                EditorUtility.SetDirty(obj);
            }
            EndUndoGroup();
        }
        #endregion
    }
}
#endif
