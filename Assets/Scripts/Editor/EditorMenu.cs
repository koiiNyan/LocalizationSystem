using UnityEditor;

namespace Localization.Editor
{
    public static class EditorMenu
    {
        [MenuItem("Game/Localization/English")]
        public static void SetLanguageEnglish()
        {
            Localization.SetLanguage(UnityEngine.SystemLanguage.English);
            UpdateCheckbox();
        }


        [MenuItem("Game/Localization/Russian")]
        public static void SetLanguageRussian()
        {
            Localization.SetLanguage(UnityEngine.SystemLanguage.Russian);
            UpdateCheckbox();
        }

        [MenuItem("Game/Localization/Korean")]
        public static void SetLanguageKorean()
        {
            Localization.SetLanguage(UnityEngine.SystemLanguage.Korean);
            UpdateCheckbox();
        }

        
        [MenuItem("Game/Localization/Reload", priority = 200)]
        public static void ReloadLocalization()
        {
            Localization.Load();
        }

        private static void UpdateCheckbox()
        {
            Menu.SetChecked("Game/Localization/English", Localization.CurrentLanguage == UnityEngine.SystemLanguage.English);
            Menu.SetChecked("Game/Localization/Russian", Localization.CurrentLanguage == UnityEngine.SystemLanguage.Russian);
            Menu.SetChecked("Game/Localization/Korean", Localization.CurrentLanguage == UnityEngine.SystemLanguage.Korean);
        }

        [InitializeOnLoadMethod]
        public static void LoadCheckboxes()
        {
            EditorApplication.delayCall += UpdateCheckbox;
        }
    }


}
