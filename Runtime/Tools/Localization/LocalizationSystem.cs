using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using System;
using System.Collections.Generic;

namespace EC.Localization
{
    public static class LocalizationSystem
    {
        public static string ActiveLanguage => _activeLanguage;
        public static void SetLanguage(int languageIndex)
        {
            if (languageIndex >= LanguageConfig.Languages.Length || languageIndex < 0)
            {
                Debug.LogError($"Language {languageIndex} out of array Language list");
                return;
            }
            SetLanguage(LanguageConfig.Languages[languageIndex]);
        }
        public static void SetLanguage(string language)
        {
            if (!LanguageConfig.Languages.Contains(language))
            {
                Debug.LogError($"Language {language} dont include in LanguageConfig [path:Resources/LanguageConfig]");
                return;
            }
            _activeLanguage = language;
            OnChangeLanguage(_activeLanguage);
        }
        private static string _activeLanguage = LanguageConfig.Languages[0];

        private static List<Action> _onSChange = new();
        private static List<Action<string>> _onVChange = new();

        public static void SubscribeChange(Action onChange)
        {
            _onSChange.Add(onChange);
        }
        public static void SubscribeChange(Action<string> onChange)
        {
            _onVChange.Add(onChange);
        }
        public static void UnsubscribeChange(Action onChange)
        {
            _onSChange.Remove(onChange);
        }
        public static void UnsubscribeChange(Action<string> onChange)
        {
            _onVChange.Remove(onChange);
        }
        private static void OnChangeLanguage(string language)
        {
            if (!Application.isPlaying) return;
            if (_onSChange.Count > 0)
                for (int i = _onSChange.Count - 1; i >= 0; i--)
                    if (_onSChange[i] == null)
                        _onSChange.RemoveAt(i);
                    else
                        _onSChange[i].Invoke();
            if (_onVChange.Count > 0)
                for (int i = _onVChange.Count - 1; i >= 0; i--)
                    if (_onVChange[i] == null)
                        _onVChange.RemoveAt(i);
                    else
                        _onVChange[i].Invoke(language);
        }

#if UNITY_EDITOR
        public static string ActiveLanguageEditor => _activeLanguageEditor;
        public static void SetLanguageEditor(int languageIndex)
        {
            if (languageIndex >= LanguageConfig.Languages.Length || languageIndex < 0)
            {
                Debug.LogError($"Language {languageIndex} out of array Language list");
                return;
            }
            SetLanguageEditor(LanguageConfig.Languages[languageIndex]);
        }
        public static void SetLanguageEditor(string language)
        {
            if (!LanguageConfig.Languages.Contains(language))
            {
                Debug.LogError($"Language {language} dont include in LanguageConfig [path:Resources/LanguageConfig]");
                return;
            }
            _activeLanguageEditor = language;
            OnChangeLanguageEditor(_activeLanguageEditor);
        }
        private static string _activeLanguageEditor = LanguageConfig.Languages[0];

        private static List<Action> _onSChangeEditor = new();
        private static List<Action<string>> _onVChangeEditor = new();

        public static void SubscribeChangeEditor(Action onChange)
        {
            _onSChangeEditor.Add(onChange);
        }
        public static void SubscribeChangeEditor(Action<string> onChange)
        {
            _onVChangeEditor.Add(onChange);
        }
        public static void UnsubscribeChangeEditor(Action onChange)
        {
            _onSChangeEditor.Remove(onChange);
        }
        public static void UnsubscribeChangeEditor(Action<string> onChange)
        {
            _onVChangeEditor.Remove(onChange);
        }
        private static void OnChangeLanguageEditor(string language)
        {
            if (_onSChangeEditor.Count > 0)
                for (int i = _onSChangeEditor.Count - 1; i >= 0; i--)
                    if (_onSChangeEditor[i] == null)
                        _onSChangeEditor.RemoveAt(i);
                    else
                        _onSChangeEditor[i].Invoke();
            if (_onVChangeEditor.Count > 0)
                for (int i = _onVChangeEditor.Count - 1; i >= 0; i--)
                    if (_onVChangeEditor[i] == null)
                        _onVChangeEditor.RemoveAt(i);
                    else
                        _onVChangeEditor[i].Invoke(language);
        }
#endif
    }
}
