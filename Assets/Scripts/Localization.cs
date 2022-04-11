using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;

namespace Localization
{
    public static class Localization
    {
        public static event Action OnLanguageChanged;

        private static ILookup<string, string> _termsMap;

        public static SystemLanguage CurrentLanguage { get; private set; }

        public static TMP_FontAsset SuggestedFont { get; private set; }

        public static bool IsLoaded => _termsMap != null;

        public static void Load()
        {
            var lang = PlayerPrefs.GetString(LocalizationSettings.Instance.PrefKey, null);
            if (Enum.TryParse(lang, out SystemLanguage localizationLanguage))
            {
                CurrentLanguage = localizationLanguage;
            }
            else
            {
                CurrentLanguage = DetectLanguage();
            }

            LoadTermsMap();

        }

        private static void LoadTermsMap()
        {
            var language = LocalizationSettings.Instance.SupportedLanguages.First(x => x.Language == CurrentLanguage);
            var resource = Resources.Load<LocalizationResource>(language.ResourceFile);

            _termsMap = resource.Terms.ToLookup(item => item.Key, item => item.Value);

            SuggestedFont = resource.Font;

            OnLanguageChanged?.Invoke();
        }

        private static SystemLanguage DetectLanguage()
        {
            var systemLanguage = Application.systemLanguage;
            foreach (var lang in LocalizationSettings.Instance.SupportedLanguages)
            {
                if (lang.Language == systemLanguage)
                {
                    return lang.Language;
                }
            }
            return LocalizationSettings.Instance.DefaultLanguage;
        }

        internal static string GetTerm(string key, Dictionary<string, string> parameters = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                return String.Empty;
            }
            
            if (!IsLoaded)
            {
                Load();
            }

            var result = _termsMap[key].FirstOrDefault();
            if (result != null)
            {
                if (parameters != null && parameters.Count > 0)
                {
                    parameters.Aggregate(result,
                       (current, parameter) => current.Replace($"%{parameter.Key}%", parameter.Value));
                }
                return result;
            }

            if (Application.isPlaying)
            {
                Debug.LogWarning($"{key} not found in {CurrentLanguage}");
            }

            return $">> {key} <<";

        }

        public static void SetLanguage(SystemLanguage lang)
        {
            CurrentLanguage = lang;

            PlayerPrefs.SetString(LocalizationSettings.Instance.PrefKey, lang.ToString());
            PlayerPrefs.Save();

            LoadTermsMap();
        }


        public static readonly Regex Plurals = new Regex("(\\[(\\d+-?\\d*:\\w*,?)+\\])");

        public static string GetPlural(string key, int quantity = 1)
        {
            var term = GetTerm(key);
            var qty = CalculatePluralQuantity(quantity);
            return Plurals.Replace(term, x => PluralReplacer(x, qty));
        }

        private static string PluralReplacer(Capture x, int qty)
        {
            var pluralGroup = x.Value.Trim('[', ']');
            var variants = pluralGroup.Split(',');
            foreach(var variant in variants)
            {
                var data = variant.Split(':');
                var range = new PluralRange(data[0]);
                if (range.InRange(qty))
                {
                    return data[1];
                }
            }
            return null;
        }

        private static int CalculatePluralQuantity(int quantity)
        {
            var qty = Mathf.Abs(quantity) % 100;
            if (qty == 0 && quantity > 0)
            {
                qty = 100;
            }
            else
            {
                qty = qty < 20 ? qty : qty % 10;
                if (qty == 0 && quantity > 0)
                {
                    qty = 20;
                }
            }
            return qty;
        }


        private class PluralRange
        {
            private readonly int max;
            private readonly int min;

            public PluralRange(string range)
            {
                var values = range.Split('-');
                max = min = int.Parse(values[0]);
                if (values.Length == 2)
                {
                    var secondValue = values[1];
                    max = string.IsNullOrEmpty(secondValue) ? int.MaxValue : int.Parse(secondValue);
                }

            }

            public bool InRange(int val)
            {
                return val >= min && val <= max;
            }
        }
    }
}
