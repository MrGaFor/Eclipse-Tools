#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GIB.EditorUtilities
{
    public class FindAShader : EditorWindow
    {
        private Shader selectedShader = null; // Field to store the currently selected shader
        private Vector2 scrollPos; // for scrolling
        private List<Material> foundMaterials = new List<Material>(); // To store found materials

        [MenuItem("Tools/GIB Toolkit/Find Materials by Shader", false, 55)]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindAShader), false, "Find Materials by Shader"); // Makes the window appear.
        }

        public void OnGUI()
        {
            GUILayout.Label("This tool finds materials that use a specified shader.");

            GUILayout.Label("Select Shader:");
            
            // Display the object field for selecting a shader
            selectedShader = EditorGUILayout.ObjectField("Shader", selectedShader, typeof(Shader), false) as Shader;

            if (GUILayout.Button("Find Materials"))
            {
                ShaderSearch(selectedShader);
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height - 60));

            if (foundMaterials.Count > 0)
            {
                foreach (Material mat in foundMaterials)
                {
                    if (GUILayout.Button(mat.name))
                    {
                        // Selects the material in the Unity Editor when clicked.
                        Selection.activeObject = mat;
                    }
                }
            }
            else
            {
                GUILayout.Label("No materials found.");
            }

            EditorGUILayout.EndScrollView();
        }

        private void ShaderSearch(Shader shader)
        {
            if (shader == null) return; // Return early if no shader is selected

            foundMaterials.Clear();
            List<Material> allMaterials = new List<Material>();

            Renderer[] allRenderers = (Renderer[])Resources.FindObjectsOfTypeAll(typeof(Renderer));
            foreach (Renderer rend in allRenderers)
            {
                foreach (Material mat in rend.sharedMaterials)
                {
                    if (!allMaterials.Contains(mat))
                    {
                        allMaterials.Add(mat);
                    }
                }
            }

            foreach (Material mat in allMaterials)
            {
                if (mat != null && mat.shader == shader)
                {
                    foundMaterials.Add(mat);
                }
            }
        }
    }
}
#endif