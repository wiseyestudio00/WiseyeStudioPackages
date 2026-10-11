using System.Collections.Generic;
using UnityEngine;

namespace WiseyeStudio.Localization
{
    public interface ILocalizationTable
    {
        IReadOnlyList<string> Keys { get; }
        IReadOnlyList<SystemLanguage> Languages { get; }

        bool ContainsKey(string key);
        bool TryGetString(string key, SystemLanguage language, out string value);
        string GetString(string key);
        string GetString(string key, SystemLanguage language);
        string ReplaceKeyedTags(string text);
        string ReplaceKeyedTags(string text, SystemLanguage language);
    }
}
