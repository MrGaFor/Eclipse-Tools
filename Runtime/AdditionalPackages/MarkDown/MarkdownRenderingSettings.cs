using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace LogicUI.FancyTextRendering
{
    [Serializable]
    public class MarkdownRenderingSettings
    {
        public static MarkdownRenderingSettings Default => new MarkdownRenderingSettings();

        [TabGroup("Bold"), HideLabel] public BoldSettings Bold = new BoldSettings();
        [Serializable]
        public class BoldSettings
        {
            public bool RenderBold = true;
        }

        [TabGroup("Itelics"), HideLabel] public ItalicSettings Italics = new ItalicSettings();
        [Serializable]
        public class ItalicSettings
        {
            public bool RenderItalics = true;
        }

        [TabGroup("Strikethrough"), HideLabel] public StrikethroughSettings Strikethrough = new StrikethroughSettings();
        [Serializable]
        public class StrikethroughSettings
        {
            public bool RenderStrikethrough = true;
        }

        [TabGroup("Monospace"), HideLabel] public MonospaceSettings Monospace = new MonospaceSettings();
        [Serializable]
        public class MonospaceSettings
        {
            public bool RenderMonospace = true;

            [Space]
            [ShowIf(nameof(RenderMonospace))]
            public bool UseCustomFont = true;

            [ShowIf("@RenderMonospace && UseCustomFont")]
            public string FontAssetPathRelativeToResources = "Noto/Noto Mono/NotoMono-Regular";

            [Space]
            [ShowIf(nameof(RenderMonospace))]
            public bool DrawOverlay = true;

            [ShowIf("@RenderMonospace && DrawOverlay")]
            public Color OverlayColor = new Color32(0, 0, 0, 60);

            [ShowIf("@RenderMonospace && DrawOverlay")]
            public float OverlayPaddingPixels = 25;

            [Space]
            [ShowIf(nameof(RenderMonospace))]
            public bool ManuallySetCharacterSpacing = false;

            [ShowIf("@RenderMonospace && ManuallySetCharacterSpacing")]
            public float CharacterSpacing = 0.69f;

            [Space]
            [ShowIf(nameof(RenderMonospace))]
            public bool AddSeparationSpacing = true;

            [ShowIf("@RenderMonospace && AddSeparationSpacing")]
            public float SeparationSpacing = 0.3f;
        }

        [TabGroup("Lists"), HideLabel] public ListSettings Lists = new ListSettings();
        [Serializable]
        public class ListSettings
        {
            public bool RenderUnorderedLists = true;
            public bool RenderOrderedLists = true;

            [Space]
            [ShowIf(nameof(RenderUnorderedLists))]
            public string UnorderedListBullet = "•";

            [ShowIf(nameof(RenderOrderedLists))]
            public string OrderedListNumberSuffix = ".";

            [Space]
            [ShowIf("@RenderUnorderedLists || RenderOrderedLists")]
            public float VerticalOffset = 0.76f;

            [ShowIf("@RenderUnorderedLists || RenderOrderedLists")]
            public float BulletOffsetPixels = 100f;

            [ShowIf("@RenderUnorderedLists || RenderOrderedLists")]
            public float ContentSeparationPixels = 20f;
        }

        [TabGroup("Links"), HideLabel] public LinkSettings Links = new LinkSettings();
        [Serializable]
        public class LinkSettings
        {
            public bool RenderLinks = true;
            public bool RenderAutoLinks = true;

            [ShowIf("@RenderLinks || RenderAutoLinks")]
            [ColorUsage(showAlpha: false)]
            public Color LinkColor = new Color32(29, 124, 234, byte.MaxValue);
        }

        [TabGroup("Headers"), HideLabel] public HeaderSettings Headers = new HeaderSettings();
        [Serializable]
        public class HeaderSettings
        {
            public bool RenderPoundSignHeaders = true;
            public bool RenderLineHeaders = true;

            [Space]
            public HeaderData[] Levels = new HeaderData[]
            {
                new HeaderData(2f, true, true, 0.45f),
                new HeaderData(1.7f, true, true, 0.3f),
                new HeaderData(1.5f, true, false),
                new HeaderData(1.3f, true, false),
            };

            [Serializable]
            public class HeaderData
            {
                public float Size;
                public bool Bold;
                public bool Underline;
                public HeaderCase Case = HeaderCase.None;
                public float VerticalSpacing;

                public HeaderData() { }

                public HeaderData(float size, bool bold, bool underline, float verticalSpacing = 0)
                {
                    Size = size;
                    Bold = bold;
                    Underline = underline;
                    VerticalSpacing = verticalSpacing;
                }

                public enum HeaderCase
                {
                    None = 0,
                    Uppercase,
                    Smallcaps,
                    Lowercase
                }
            }
        }

        [TabGroup("Superscript"), HideLabel] public SuperscriptSettings Superscript = new SuperscriptSettings();
        [Serializable]
        public class SuperscriptSettings
        {
            public bool RenderSuperscript = false;

            [ShowIf(nameof(RenderSuperscript))]
            public bool RenderChainSuperscript = true;
        }
    }
}
