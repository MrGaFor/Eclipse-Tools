#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace EC
{
    public class ECModules : OdinEditorWindow
    {
        private enum ModuleState { On, Off }

        [System.Serializable]
        private struct Module
        {
            [HideInInspector] public string Name;
            [HideInInspector] public string Key;
            [HideInInspector] public bool State;
        }

        [SerializeField, ListDrawerSettings(
            ShowFoldout = false,
            HideAddButton = true,
            HideRemoveButton = true,
            DraggableItems = false, 
            OnBeginListElementGUI = "DrawModuleElement")]
        private Module[] _modules;

        private static readonly (string Name, string Key)[] DefaultModules =
        {
            ("Themes", "EC_THEMES")
        };

        [MenuItem("Tools/EC/Configurator")]
        private static void OpenWindow()
        {
            GetWindow<ECModules>().Show();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Revert();
        }

        private void DrawModuleElement(InspectorProperty property, int index, Module[] array, ECModules root)
        {
            var module = array[index];
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(2.5f);
            EditorGUILayout.LabelField(module.Name, new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 13
            });
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            Color b1Color = GUI.backgroundColor;
            GUI.backgroundColor = module.State ? Color.green : Color.red;
            if (GUILayout.Button(module.State ? "On" : "Off", new GUIStyle(GUI.skin.button) 
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12,
                normal = { textColor = GUI.backgroundColor }
            }, GUILayout.Width(50), GUILayout.Height(25)))
            {
                module.State = !module.State;
                array[index] = module;
            }
            GUI.backgroundColor = b1Color;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(2.5f);
        }

        [ButtonGroup("Actions"), Button("Revert")]
        private void Revert()
        {
            var defines = ECDefineService.GetAllDefines();

            _modules = DefaultModules
                .Select(x => new Module
                {
                    Name = x.Name,
                    Key = x.Key,
                    State = defines.Contains(x.Key)
                }).ToArray();
        }

        [ButtonGroup("Actions"), Button("Apply")]
        private void Apply()
        {
            var defines = ECDefineService.GetAllDefines();

            foreach (var module in _modules)
            {
                if (module.State)
                    defines.Add(module.Key);
                else
                    defines.Remove(module.Key);
            }

            ECDefineService.ApplyDefines(defines);
            AssetDatabase.SaveAssets();
        }
    }

    internal static class ECDefineService
    {
        public static HashSet<string> GetAllDefines()
        {
            var result = new HashSet<string>();
            foreach (var target in GetTargets())
            {
                var defines = PlayerSettings.GetScriptingDefineSymbols(target);
                foreach (var d in Split(defines))
                    result.Add(d);
            }
            return result;
        }

        public static void ApplyDefines(HashSet<string> defines)
        {
            var joined = string.Join(";", defines);
            foreach (var target in GetTargets())
                PlayerSettings.SetScriptingDefineSymbols(target, joined);
        }

        private static IEnumerable<NamedBuildTarget> GetTargets()
        {
            foreach (BuildTargetGroup group in System.Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (group == BuildTargetGroup.Unknown)
                    continue;
                if (!BuildPipeline.IsBuildTargetSupported(group, EditorUserBuildSettings.activeBuildTarget))
                    continue;
                NamedBuildTarget target;
                try { target = NamedBuildTarget.FromBuildTargetGroup(group); }
                catch { continue; }
                yield return target;
            }
        }

        private static IEnumerable<string> Split(string defines)
        {
            if (string.IsNullOrWhiteSpace(defines)) yield break;
            foreach (var d in defines.Split(';'))
                if (!string.IsNullOrWhiteSpace(d))
                    yield return d;
        }
    }
}
#endif