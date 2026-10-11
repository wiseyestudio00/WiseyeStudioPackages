using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WiseyeStudio.Localization
{
    [Serializable]
    public class LocalizedValue<T> : ILocalizedValue<T>
    {
        [Serializable]
        public struct Entry
        {
            [SerializeField] private SystemLanguage language;
            [SerializeField] private T value;

            public Entry(SystemLanguage language, T value)
            {
                this.language = language;
                this.value = value;
            }

            public SystemLanguage Language => language;
            public T Value => value;
        }

        [TableList]
        [SerializeField] private List<Entry> entries = new();

        public IReadOnlyList<Entry> Entries => entries;

        private static T MissingValue => typeof(T) == typeof(string) ? (T)(object)string.Empty : default;

        public T Get() => Get(LocalizationManager.ResolveCurrentLanguage());

        public T Get(SystemLanguage language)
        {
            if (TryGet(language, out var value))
            {
                return value;
            }

            return TryGet(LocalizationSettings.Instance.FallbackLanguage, out value) ? value : MissingValue;
        }

        public bool TryGet(SystemLanguage language, out T value)
        {
            foreach (var entry in entries)
            {
                if (entry.Language == language && HasValue(entry.Value))
                {
                    value = entry.Value;
                    return true;
                }
            }

            value = MissingValue;
            return false;
        }

        public void Set(SystemLanguage language, T value)
        {
            for (var i = 0; i < entries.Count; i++)
            {
                if (entries[i].Language == language)
                {
                    entries[i] = new Entry(language, value);
                    return;
                }
            }

            entries.Add(new Entry(language, value));
        }

        private static bool HasValue(T value) => value switch
        {
            null => false,
            string text => text.Length > 0,
            Object unityObject => unityObject != null,
            _ => true,
        };
    }
}
