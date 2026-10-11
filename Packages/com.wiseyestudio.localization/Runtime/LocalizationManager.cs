using System;
using UnityEngine;
using WiseyeStudio.Singleton;

namespace WiseyeStudio.Localization
{
    [DefaultExecutionOrder(-1000)]
    public class LocalizationManager : DontDestroyOnLoadSingleton<LocalizationManager>
    {
        private static bool _hasLoggedMissingManager;

        private SystemLanguage? _currentLanguage;

        public event Action<SystemLanguage> LanguageChanged;

        public LocalizationSettings Settings => LocalizationSettings.Instance;

        public SystemLanguage CurrentLanguage => _currentLanguage ??= Settings.FallbackLanguage;

        public void SetLanguage(SystemLanguage language)
        {
            if (!Settings.IsSupported(language))
            {
                Debug.LogWarning($"{language} isn't a supported language; using {Settings.FallbackLanguage}.", this);
                language = Settings.FallbackLanguage;
            }

            if (_currentLanguage == language)
            {
                return;
            }

            _currentLanguage = language;
            LanguageChanged?.Invoke(language);
        }

        internal static SystemLanguage ResolveCurrentLanguage()
        {
            if (Instance != null)
            {
                return Instance.CurrentLanguage;
            }

            if (!_hasLoggedMissingManager)
            {
                Debug.LogError($"No {nameof(LocalizationManager)} in the scene; using English.");
                _hasLoggedMissingManager = true;
            }

            return SystemLanguage.English;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _hasLoggedMissingManager = false;
        }
    }
}
