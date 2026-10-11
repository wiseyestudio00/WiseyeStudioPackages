using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WiseyeStudio.Localization
{
    [Serializable]
    public class LocalizedStringFromTable : ILocalizedValue<string>
    {
        [Required]
        [TypeSelectorSettings(FilterTypesFunction = nameof(IsSelectableTableType))]
        [OdinSerialize] private ILocalizationTable table;

        [ValueDropdown(nameof(GetKeys))]
        [SerializeField] private string key;

        public LocalizedStringFromTable()
        {
        }

        public LocalizedStringFromTable(ILocalizationTable table, string key)
        {
            this.table = table;
            this.key = key;
        }

        public ILocalizationTable Table => table;
        public string Key => key;

        private bool HasTable => table is Object unityObject ? unityObject != null : table != null;

        public string Get() => Get(LocalizationManager.ResolveCurrentLanguage());

        public string Get(SystemLanguage language) => HasTable ? table.GetString(key, language) : key ?? string.Empty;

        private IEnumerable<string> GetKeys() => HasTable ? table.Keys : Array.Empty<string>();

        private bool IsSelectableTableType(Type type) => type != typeof(LocalizationTable);

#if UNITY_EDITOR
        private bool HasPreview => HasTable && !string.IsNullOrEmpty(key);

        private struct PreviewRow
        {
            [DisplayAsString(false)] public SystemLanguage Language;
            [DisplayAsString(false)] public string Text;
        }

        [FoldoutGroup("Preview")]
        [ShowInInspector, EnableGUI, ShowIf(nameof(HasPreview)), HideLabel]
        [TableList(IsReadOnly = true, AlwaysExpanded = true, HideToolbar = true)]
        private List<PreviewRow> Preview
        {
            get
            {
                var rows = new List<PreviewRow>();
                foreach (var language in LocalizationSettings.Instance.Languages)
                {
                    table.TryGetString(key, language, out var text);
                    rows.Add(new PreviewRow { Language = language, Text = text });
                }

                return rows;
            }
        }
#endif
    }
}
