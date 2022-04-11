using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizationComponent : MonoBehaviour
    {
        private Dictionary<string, string> _parameters;

        [SerializeField]
        private TextMeshProUGUI _textMeshPro;

        [SerializeField]
        private string _key;

        public string Key
        {
            get => _key;
            set
            {
                _key = value;
                UpdateTerm();
            }
        }

        private void OnEnable()
        {
            Localization.OnLanguageChanged += UpdateTerm;
            UpdateTerm();
        }

        private void OnDisable()
        {
            Localization.OnLanguageChanged -= UpdateTerm;
        }


        private void Awake()
        {
            if (_textMeshPro == null)
            {
                _textMeshPro = GetComponent<TextMeshProUGUI>();
            }
        }

        private void OnValidate()
        {
            if (_textMeshPro == null)
            {
                _textMeshPro = GetComponent<TextMeshProUGUI>();
            }

            UpdateTerm();
        }

        private void UpdateTerm()
        {
            _textMeshPro.font = Localization.SuggestedFont;
            _textMeshPro.text = Localization.GetTerm(_key, _parameters);
        }

        public void SetParameters(Dictionary <string, string> parameters)
        {
            _parameters = parameters;
            UpdateTerm();
        }
    }
}