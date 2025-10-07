using JimmysUnityUtilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LogicUI.FancyTextRendering
{
    [RequireComponent(typeof(MarkdownRenderer))]
    public class LoadMarkdownFromResources : MonoBehaviour
    {
        [SerializeField] string MarkdownResourcesPath;

        private void Awake()
        {
            LoadMarkdown();
        }

        [Button("Load")]
        private void LoadMarkdown()
        {
            string markdown = ResourcesUtilities.ReadTextFromFile(MarkdownResourcesPath);
            GetComponent<MarkdownRenderer>().Source = markdown;
        }
    }
}