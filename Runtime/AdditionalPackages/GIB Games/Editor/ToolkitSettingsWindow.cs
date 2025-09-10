using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace GIB.EditorUtilities
{
    // This EditorScript handles many of the backend processes which impact how
    // EventsPro and Auspex Attributes are displayed. Modifying this script is not recommended.
    public class ToolkitSettingsWindow : EditorWindow
    {
        const string filePath = "Assets/GIB Games/GIB Toolkit/Editor/StyleSheets/Extensions/dark.uss";
        private bool useEventsTMP;
        private bool hasCheckedDefines;
        private Color currentEPColor;
        private Color currentNullColor;
        private System.Collections.Generic.List<string> defines;
        private BuildTargetGroup buildTargetGroup;
        private Texture2D epLogo;
        private Texture2D gibLogo;
        private Color titleBarColor;
        private Color tempTitleBarColor;
        [MenuItem("Tools/GIB Toolkit/GIB Toolkit Settings", priority = -999)]
        public static void ShowWindow()
        {
            GetWindow<ToolkitSettingsWindow>("EventsPro + GIB Toolkit");
        }
        private void OnEnable()
        {
            epLogo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/GIB Games/Editor/Images/AS_bar_sm.png");
            gibLogo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/GIB Games/Editor/Images/GIB New Splash.png");
            titleBarColor = GetCurrentColorFromStyleSheet();
        }
        private void OnGUI()
        {
            if (tempTitleBarColor == Color.clear)
                tempTitleBarColor = titleBarColor;

            currentEPColor = ToolkitSettings.EPColor;
            currentNullColor = ToolkitSettings.NullColor;
            var urlStyle = new GUIStyle(GUI.skin.label);
            urlStyle.normal.textColor = new Color(.5f,.6f,.7f);
            if (epLogo != null)
                GUI.DrawTexture(new Rect(0, 0, epLogo.width, epLogo.height), epLogo, ScaleMode.ScaleToFit);
            EditorGUILayout.Space(epLogo.height*1.25f);
            // TMP support to come soon
            // useEventsTMP = EditorGUILayout.Toggle("Enable TextMeshPro Components", useEventsTMP);
            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            Color newEPColor = EditorGUILayout.ColorField(new GUIContent("EventsPro Color", "General color for EventsPro components."), currentEPColor);
            if (GUILayout.Button("Reset"))
            {
                newEPColor = new Color(1.0f, .973f, .678f, 1.0f);
                currentEPColor = newEPColor;
                ToolkitSettings.EPColor = newEPColor;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            Color newNullColor = EditorGUILayout.ColorField(new GUIContent("Null Highlight", "Highlight color for empty nullable parameters."), currentNullColor);
            if (GUILayout.Button("Reset"))
            {
                newNullColor = new Color(1.0f, .5f, 0f, 1.0f);
                currentNullColor = newNullColor;
                ToolkitSettings.NullColor = newNullColor;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            tempTitleBarColor = EditorGUILayout.ColorField(new GUIContent("Title Bar Color", "Cosmetic color for Unity."), tempTitleBarColor);
            if (GUILayout.Button("Set"))
            {
                UpdateStyleSheet(tempTitleBarColor);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Support", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("For support, visit:");
            EditorGUILayout.Space();
            if (GUILayout.Button("Documentation", urlStyle))
            {
                Application.OpenURL("https://dorktoast.github.io/EventsPro/index.html");
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Getting Started Guide", urlStyle))
            {
                Application.OpenURL("https://dorktoast.github.io/EventsPro/GettingStarted.pdf");
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Discord", urlStyle))
            {
                Application.OpenURL("https://discord.gg/gibgames");
            }
            if (gibLogo != null)
            {
                float logoWidth = gibLogo.width * 0.5f;
                float logoHeight = gibLogo.height * 0.5f;

                float centerX = (position.width - logoWidth) * 0.5f;
                float posY = 300;
                GUI.backgroundColor = Color.clear;
                if (GUI.Button(new Rect(centerX, posY, logoWidth, logoHeight), GUIContent.none))
                    Application.OpenURL("https://gib.games");
                GUI.backgroundColor = Color.white;
                GUI.DrawTexture(new Rect(centerX, posY, logoWidth, logoHeight), gibLogo, ScaleMode.ScaleToFit);
            }
            if (GUI.changed)
            {
                UpdateSymbols();
                ToolkitSettings.EPColor = newEPColor;
            }
        }
        private void UpdateSymbols()
        {
            SetDefine("GIB_EVENTSPRO", true);
            CompilationPipeline.RequestScriptCompilation();
        }
        public bool ToolkitDefined(string targetDefine)
        {
            buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup)
                .Split(';')
                .ToList();

            return defines.Contains(targetDefine);
        }

        private void UpdateStyleSheet(Color color)
        {
            try
            {
                string content = System.IO.File.ReadAllText(filePath);
                string colorHex = ColorUtility.ToHtmlStringRGB(color);
                string newContent = Regex.Replace(content, @"\.dockHeader\s*\{\s*background-color:\s*#[0-9a-fA-F]{6};\s*\}", $".dockHeader {{ background-color: #{colorHex}; }}");
                System.IO.File.WriteAllText(filePath, newContent);

                AssetDatabase.Refresh(); // Refresh the AssetDatabase to apply changes
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error updating stylesheet: {ex.Message}");
            }
        }

        private Color GetCurrentColorFromStyleSheet()
        {
            try
            {
                string content = System.IO.File.ReadAllText(filePath);
                var match = Regex.Match(content, @"\.dockHeader\s*\{\s*background-color:\s*#([0-9a-fA-F]{6});");

                if (match.Success)
                {
                    string colorHex = match.Groups[1].Value;
                    if (ColorUtility.TryParseHtmlString($"#{colorHex}", out Color color))
                    {
                        return color;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error reading stylesheet: {ex.Message}");
            }
            return Color.white; // Default color if parsing fails
        }
        public void SetDefine(string targetDefine, bool state)
        {
            buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var currentDefines = GetDefines();
            if (state)
            {
                if(!currentDefines.Contains(targetDefine))
                    currentDefines.Add(targetDefine);
            }
            else
            {
                if (currentDefines.Contains(targetDefine))
                    currentDefines.Remove(targetDefine);
            }
            SetDefines(currentDefines);
        }
        private List<string> GetDefines()
        {
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            return new List<string>(defineSymbols.Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
        }
        private void SetDefines(List<string> defines)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", defines));
        }
    }
    public static class ToolkitSettings
    {
        public static Color EPColor
        {
            get
            {
                if (cachedEPColor == null)
                {
                    cachedEPColor = new Color(
                        EditorPrefs.GetFloat("GIB_EPColor_Red", 1.0f),
                        EditorPrefs.GetFloat("GIB_EPColor_Green", 0.973f),
                        EditorPrefs.GetFloat("GIB_EPColor_Blue", 0.678f),
                        EditorPrefs.GetFloat("GIB_EPColor_Alpha", 1.0f)
                    );
                }

                return cachedEPColor.Value;
            }
            set
            {
                cachedEPColor = value;

                EditorPrefs.SetFloat("GIB_EPColor_Red", value.r);
                EditorPrefs.SetFloat("GIB_EPColor_Green", value.g);
                EditorPrefs.SetFloat("GIB_EPColor_Blue", value.b);
                EditorPrefs.SetFloat("GIB_EPColor_Alpha", value.a);
            }
        }
        public static Color NullColor
        {
            get
            {
                if (cachedNullColor == null)
                {
                    cachedNullColor = new Color(
                        EditorPrefs.GetFloat("GIB_EPNull_Red", 1.0f),
                        EditorPrefs.GetFloat("GIB_EPNull_Green", 0.5f),
                        EditorPrefs.GetFloat("GIB_EPNull_Blue", 0.0f),
                        EditorPrefs.GetFloat("GIB_EPNull_Alpha", 1.0f)
                    );
                }

                return cachedNullColor.Value;
            }
            set
            {
                cachedNullColor = value;

                EditorPrefs.SetFloat("GIB_EPNull_Red", value.r);
                EditorPrefs.SetFloat("GIB_EPNull_Green", value.g);
                EditorPrefs.SetFloat("GIB_EPNull_Blue", value.b);
                EditorPrefs.SetFloat("GIB_EPNull_Alpha", value.a);
            }
        }
        public static Color SelectColor
        {
            get
            {
                if (cachedSelectColor == null)
                {
                    cachedSelectColor = new Color(
                        EditorPrefs.GetFloat("GIB_EPSelect_Red", 1.0f),
                        EditorPrefs.GetFloat("GIB_EPSelect_Green", 0.5f),
                        EditorPrefs.GetFloat("GIB_EPSelect_Blue", 0.0f),
                        EditorPrefs.GetFloat("GIB_EPSelect_Alpha", 1.0f)
                    );
                }

                return cachedSelectColor.Value;
            }
            set
            {
                cachedSelectColor = value;

                EditorPrefs.SetFloat("GIB_EPSelect_Red", value.r);
                EditorPrefs.SetFloat("GIB_EPSelect_Green", value.g);
                EditorPrefs.SetFloat("GIB_EPSelect_Blue", value.b);
                EditorPrefs.SetFloat("GIB_EPSelect_Alpha", value.a);
            }
        }
        private static Color? cachedEPColor = null;
        private static Color? cachedNullColor = null;
        private static Color? cachedSelectColor = null;


    }

    sealed class CheckForTextMeshPro
    {
        const string tmpDefine = "TMP_PRESENT";

        [UnityEditor.Callbacks.DidReloadScripts]
        static void CheckForTMPro()
        {
            var namespaceFound = (from assembly in System.AppDomain.CurrentDomain.GetAssemblies()
                                  from type in assembly.GetTypes()
                                  where type.Namespace == "TMPro"
                                  select type).Any();

            if (namespaceFound)
                DefineUtilities.AddDefine(tmpDefine, DefineUtilities.GetValidBuildTargets());
            else
                DefineUtilities.RemoveDefine(tmpDefine, DefineUtilities.GetValidBuildTargets());
        }
    }

    public static class DefineUtilities
    {
        /// <summary>
        /// ScriptingDefineSymbols are separated into a collection by any of these,
        /// but always written back using index 0.
        /// </summary>
        public static char[] separators = { ';', ' ' };

        public static void AddDefine(string _define, IEnumerable<BuildTargetGroup> _buildTargets)
        {
            foreach (var target in _buildTargets)
            {
                var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Trim();

                var list = defines.Split(separators)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();

                if (list.Contains(_define))
                    continue;

                list.Add(_define);
                defines = list.Aggregate((a, b) => a + separators[0] + b);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
            }
        }

        public static void RemoveDefine(string _define, IEnumerable<BuildTargetGroup> _buildTargets)
        {
            foreach (var target in _buildTargets)
            {
                var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Trim();

                var list = defines.Split(separators)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();

                if (!list.Remove(_define)) //If not in list then no changes needed
                    continue;

                defines = list.Aggregate((a, b) => a + separators[0] + b);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
            }
        }

        public static IEnumerable<BuildTargetGroup> GetValidBuildTargets()
        {
            return System.Enum.GetValues(typeof(BuildTargetGroup))
                .Cast<BuildTargetGroup>()
                .Where(x => x != BuildTargetGroup.Unknown)
                .Where(x => !IsObsolete(x));
        }

        public static bool IsObsolete(BuildTargetGroup group)
        {
            var obsoleteAttributes = typeof(BuildTargetGroup)
                .GetField(group.ToString())
                .GetCustomAttributes(typeof(System.ObsoleteAttribute), false);

            return obsoleteAttributes != null && obsoleteAttributes.Length > 0;
        }
    }


}
