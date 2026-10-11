using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;

namespace WiseyeStudio.Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizationTextTag : SerializedMonoBehaviour
    {
        [Required]
        [OdinSerialize] private ILocalizedValue<string> localizedText = new LocalizedStringFromTable();

        private TMP_Text _textComponent;
        private LocalizationManager _subscribedManager;

        public ILocalizedValue<string> LocalizedText => localizedText;

        protected TMP_Text TextComponent =>
            _textComponent != null ? _textComponent : _textComponent = GetComponent<TMP_Text>();

        public void Set(ILocalizedValue<string> value)
        {
            localizedText = value;
            Refresh();
        }

        public void Refresh()
        {
            if (localizedText == null)
            {
                return;
            }

            ApplyText(localizedText.Get());
        }

        protected virtual void ApplyText(string text)
        {
            TextComponent.text = text;
        }

        protected virtual void OnEnable()
        {
            _subscribedManager = LocalizationManager.Instance;
            if (_subscribedManager != null)
            {
                _subscribedManager.LanguageChanged += OnLanguageChanged;
            }

            Refresh();
        }

        protected virtual void OnDisable()
        {
            if (_subscribedManager != null)
            {
                _subscribedManager.LanguageChanged -= OnLanguageChanged;
            }

            _subscribedManager = null;
        }

        private void OnLanguageChanged(SystemLanguage language) => Refresh();

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, MultiLineProperty, ShowIf("@localizedText != null")]
        private string Preview => localizedText.Get(PreviewLanguage);

        private static SystemLanguage PreviewLanguage => LocalizationManager.Instance != null
            ? LocalizationManager.Instance.CurrentLanguage
            : LocalizationSettings.Instance.DefaultLanguage;
#endif
    }
}
