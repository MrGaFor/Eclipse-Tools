using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Conversa.Editor
{
    public class GraphToolbar : Toolbar
    {
        public UnityEvent OnSave { get; } = new UnityEvent();
        public UnityEvent<string> OnLanguageChange { get; } = new UnityEvent<string>(); 
        private static int LanguageIndex = 0;
        public UnityEvent<bool> OnToggleInspector { get; } = new UnityEvent<bool>();
        public UnityEvent OnReset { get; } = new UnityEvent();
        
        public GraphToolbar()
        {
            var saveButton = new ToolbarButton(OnSave.Invoke) {text = "Save"};

            var resetButton = new ToolbarButton(OnReset.Invoke) {text = "Reset"};

            if (LanguageIndex >= EC.Localization.LanguageConfig.Languages.Length)
                LanguageIndex = 0;
            EC.Localization.LocalizationSystem.SetLanguageEditor(LanguageIndex);
            var languageButton = new ToolbarButton();
            languageButton.clicked += LanguageClick;
            languageButton.text = EC.Localization.LocalizationSystem.ActiveLanguageEditor;

            void LanguageClick()
            {
                LanguageIndex++;
                if (LanguageIndex == EC.Localization.LanguageConfig.Languages.Length)
                    LanguageIndex = 0;
                EC.Localization.LocalizationSystem.SetLanguageEditor(LanguageIndex);
                languageButton.text = EC.Localization.LocalizationSystem.ActiveLanguageEditor;
                OnLanguageChange.Invoke(EC.Localization.LocalizationSystem.ActiveLanguageEditor);
            }

            var toggle = new ToolbarToggle {text = "Inspector", value = true};
            toggle.RegisterCallback<ClickEvent>(evt => OnToggleInspector.Invoke(toggle.value));
            
            Add(saveButton);
            Add(languageButton);
            Add(toggle);
        }
    }
}
