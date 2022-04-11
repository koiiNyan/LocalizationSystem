using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Localization
{
    [CreateAssetMenu(fileName = "localization.asset", menuName = "Localization/Create Resource")]
    public class LocalizationResource : ScriptableObject
    {
        public TMP_FontAsset Font;
        public List<LocalizationTerm> Terms;
    }
    [System.Serializable]
    public class LocalizationTerm
    {
        public string Key;
        public string Value;
    }
}

