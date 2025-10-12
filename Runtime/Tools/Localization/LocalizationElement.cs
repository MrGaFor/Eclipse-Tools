using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

namespace EC.Localization
{
    [System.Serializable]
    public class LocalizationElement<T>
    {
        [System.Serializable]
        public class LanguagePair
        {
            [HorizontalGroup(), ValueDropdown("Languages"),] public string Language;
            [HorizontalGroup()] public T Value;

            public LanguagePair(string language)
            {
                Language = language;
                Value = default;
            }
            public LanguagePair(string language, T value)
            {
                Language = language;
                Value = value;
            }

            private string[] Languages => LanguageConfig.Languages;
        }

        [SerializeField] private LanguagePair[] _massValues;
        [SerializeField, DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, KeyLabel = "Lang", ValueLabel = "Val")] private Dictionary<string, T> _dictValues;

        public const LocalizationElement<T> Empty = null;
        public LocalizationElement()
        {
            _massValues = new LanguagePair[0];
            _dictValues = new Dictionary<string, T>();
        }
        public LocalizationElement(LanguagePair[] massValues)
        {
            _massValues = massValues;
            _dictValues = new Dictionary<string, T>();
        }

        public void SetValue(T value)
        {
            SetValue(value, LocalizationSystem.ActiveLanguage);
        }
        public void SetValue(T value, string language)
        {
            CheckValidData();
            foreach (var pair in _massValues)
                if (pair.Language == language)
                    pair.Value = value;
            if (_dictValues.ContainsKey(language))
                _dictValues[language] = value;
        }

        public T GetValue()
        {
            return GetValue(LocalizationSystem.ActiveLanguage);
        }
        public T GetValue(string language)
        {
            CheckValidData();
            return _dictValues[language];
        }

        private void CheckValidData()
        {
#if UNITY_EDITOR
            CheckValidMass();
            void CheckValidMass()
            {
                string[] langs = EC.Localization.LanguageConfig.Languages;
                if (_massValues == null)
                {
                    _massValues = new LanguagePair[langs.Length];
                    for (int i = 0; i < langs.Length; i++)
                        _massValues[i] = new LanguagePair(langs[i]);
                }
                if (_massValues.Length != langs.Length)
                {
                    LanguagePair[] newMass = new LanguagePair[langs.Length];
                    for (int i = 0; i < langs.Length; i++)
                        newMass[i] = new LanguagePair(langs[i], default);
                    for (int i = 0; i < _massValues.Length; i++)
                        if (langs.Contains(_massValues[i].Language))
                            foreach (var pair in newMass)
                                if (pair.Language == _massValues[i].Language)
                                    pair.Value = _massValues[i].Value;
                    _massValues = newMass;
                }
            }
#endif

            CheckValidDict();
            void CheckValidDict()
            {
                _dictValues ??= new Dictionary<string, T>();
                foreach (var pair in _massValues)
                    if (_dictValues.TryGetValue(pair.Language, out T value) && !Equals(value, pair.Value))
                        _dictValues[pair.Language] = pair.Value;
                    else if (!_dictValues.ContainsKey(pair.Language))
                        _dictValues.Add(pair.Language, pair.Value);
            }
        }

    }
}
