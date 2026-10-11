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
    }
}
