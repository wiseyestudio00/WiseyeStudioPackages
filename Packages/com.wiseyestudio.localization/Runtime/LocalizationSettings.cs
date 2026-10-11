using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WiseyeStudio.Localization
{
    [CreateAssetMenu(
        fileName = "LocalizationSettings",
        menuName = AssetMenuConstants.Root + "Localization/Localization Settings")]
    [InfoBox("Must be at Resources/" + ResourcesPath + ".asset to be found.")]
    public class LocalizationSettings : ScriptableObject
    {
        public const string ResourcesPath = "WiseyeStudio/LocalizationSettings";

        private static LocalizationSettings _instance;
        private static bool _isFallbackInstance;

        [Tooltip("Supported languages. Also the column order of tables without a header row.")]
        [ValidateInput(nameof(HasNoDuplicates), "A language is listed more than once.")]
        [SerializeField] private List<SystemLanguage> languages = new()
        {
            SystemLanguage.English,
            SystemLanguage.ChineseTraditional,
            SystemLanguage.ChineseSimplified,
            SystemLanguage.Japanese,
            SystemLanguage.Korean,
        };

        [Tooltip("Used when a translation is missing, and when the system language isn't supported.")]
        [ValidateInput(nameof(IsSupported), "The fallback language must be in the language list.")]
        [SerializeField] private SystemLanguage fallbackLanguage = SystemLanguage.English;

        public IReadOnlyList<SystemLanguage> Languages => languages;
        public SystemLanguage FallbackLanguage => fallbackLanguage;

        public SystemLanguage DefaultLanguage =>
            IsSupported(Application.systemLanguage) ? Application.systemLanguage : fallbackLanguage;

        public static LocalizationSettings Instance
        {
            get
            {
                if (_instance != null && (!_isFallbackInstance || !Application.isEditor))
                {
                    return _instance;
                }

                var loaded = Resources.Load<LocalizationSettings>(ResourcesPath);
                if (loaded != null)
                {
                    _instance = loaded;
                    _isFallbackInstance = false;
                    return _instance;
                }

                if (_instance == null)
                {
                    Debug.LogWarning($"No {nameof(LocalizationSettings)} at Resources/{ResourcesPath}; using the defaults.");
                    _instance = CreateInstance<LocalizationSettings>();
                    _instance.hideFlags = HideFlags.DontSave;
                    _isFallbackInstance = true;
                }

                return _instance;
            }
        }

        public bool IsSupported(SystemLanguage language) => languages.Contains(language);

        private bool HasNoDuplicates(List<SystemLanguage> value) => value.Distinct().Count() == value.Count;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _instance = null;
            _isFallbackInstance = false;
        }
    }
}
