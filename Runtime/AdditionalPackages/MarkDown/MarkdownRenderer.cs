using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace LogicUI.FancyTextRendering
{
    [RequireComponent(typeof(TMP_Text))]
    public class MarkdownRenderer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textMesh;
        [SerializeField, TextArea(minLines: 3, maxLines: 10)] private string _source;
        [SerializeField, HideLabel] private MarkdownRenderingSettings _renderSettings = MarkdownRenderingSettings.Default;

        public void SetText(string text)
        {
            _source = text;
            Markdown.RenderToTextMesh(_source, _textMesh, _renderSettings);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            Markdown.RenderToTextMesh(_source, _textMesh, _renderSettings);
        }
#endif
    }
}