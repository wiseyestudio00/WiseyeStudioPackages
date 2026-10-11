using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace WiseyeStudio.Localization
{
    public class LocalizationTable : ILocalizationTable
    {
        private static readonly Regex KeyTagRegex = new(@"\{([^{}]+)\}");

        private readonly SystemLanguage _fallbackLanguage;
        private readonly List<string> _keys = new();
        private readonly HashSet<string> _keySet = new();
        private readonly List<SystemLanguage> _languages = new();
        private readonly Dictionary<SystemLanguage, Dictionary<string, string>> _values = new();
        private readonly List<string> _problems = new();

        private LocalizationTable(SystemLanguage fallbackLanguage)
        {
            _fallbackLanguage = fallbackLanguage;
        }

        public IReadOnlyList<string> Keys => _keys;
        public IReadOnlyList<SystemLanguage> Languages => _languages;
        public IReadOnlyList<string> Problems => _problems;

        public static LocalizationTable Parse(string csv, bool hasHeaderRow)
        {
            var settings = LocalizationSettings.Instance;
            return Parse(csv, hasHeaderRow, settings.Languages, settings.FallbackLanguage);
        }

        public static LocalizationTable Parse(
            string csv,
            bool hasHeaderRow,
            IReadOnlyList<SystemLanguage> defaultColumns,
            SystemLanguage fallbackLanguage)
        {
            var table = new LocalizationTable(fallbackLanguage);
            var rows = CsvParser.Parse(csv ?? string.Empty);

            var firstDataRow = 0;
            List<SystemLanguage?> columns;
            if (hasHeaderRow)
            {
                columns = rows.Count > 0 ? table.ReadHeader(rows[0]) : new List<SystemLanguage?>();
                firstDataRow = 1;
            }
            else
            {
                columns = new List<SystemLanguage?>();
                foreach (var language in defaultColumns)
                {
                    columns.Add(language);
                }
            }

            foreach (var language in columns)
            {
                if (language.HasValue && !table._values.ContainsKey(language.Value))
                {
                    table._languages.Add(language.Value);
                    table._values[language.Value] = new Dictionary<string, string>();
                }
            }

            for (var rowIndex = firstDataRow; rowIndex < rows.Count; rowIndex++)
            {
                table.ReadRow(rows[rowIndex], rowIndex + 1, columns);
            }

            return table;
        }

        public bool ContainsKey(string key) => key != null && _keySet.Contains(key);

        public bool TryGetString(string key, SystemLanguage language, out string value)
        {
            if (key != null
                && _values.TryGetValue(language, out var strings)
                && strings.TryGetValue(key, out var found)
                && !string.IsNullOrEmpty(found))
            {
                value = found;
                return true;
            }

            value = string.Empty;
            return false;
        }

        public string GetString(string key) => GetString(key, LocalizationManager.ResolveCurrentLanguage());

        public string GetString(string key, SystemLanguage language)
        {
            if (TryGetString(key, language, out var value))
            {
                return value;
            }

            return TryGetString(key, _fallbackLanguage, out value) ? value : key ?? string.Empty;
        }

        public string ReplaceKeyedTags(string text) =>
            ReplaceKeyedTags(text, LocalizationManager.ResolveCurrentLanguage());

        public string ReplaceKeyedTags(string text, SystemLanguage language)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return KeyTagRegex.Replace(text, match =>
            {
                var key = match.Groups[1].Value;
                return TryGetString(key, language, out var value) || TryGetString(key, _fallbackLanguage, out value)
                    ? value
                    : match.Value;
            });
        }

        private List<SystemLanguage?> ReadHeader(List<string> header)
        {
            var columns = new List<SystemLanguage?>();
            var seen = new HashSet<SystemLanguage>();

            for (var i = 1; i < header.Count; i++)
            {
                var columnName = header[i].Trim();
                if (columnName.Length == 0)
                {
                    columns.Add(null);
                    continue;
                }

                if (!char.IsLetter(columnName[0])
                    || !Enum.TryParse(columnName, true, out SystemLanguage language)
                    || !Enum.IsDefined(typeof(SystemLanguage), language))
                {
                    _problems.Add($"Header column {i + 1} \"{columnName}\" isn't a SystemLanguage name; the column is ignored.");
                    columns.Add(null);
                    continue;
                }

                if (!seen.Add(language))
                {
                    _problems.Add($"Header column {i + 1} repeats {language}; the column is ignored.");
                    columns.Add(null);
                    continue;
                }

                columns.Add(language);
            }

            return columns;
        }

        private void ReadRow(List<string> cells, int rowNumber, List<SystemLanguage?> columns)
        {
            var key = cells.Count > 0 ? cells[0].Trim() : string.Empty;
            if (key.Length == 0)
            {
                return;
            }

            if (_keySet.Add(key))
            {
                _keys.Add(key);
            }
            else
            {
                _problems.Add($"Row {rowNumber}: duplicate key \"{key}\" replaces the earlier row.");
            }

            for (var i = 0; i < columns.Count; i++)
            {
                var language = columns[i];
                if (!language.HasValue)
                {
                    continue;
                }

                var cellIndex = i + 1;
                var value = cellIndex < cells.Count ? Unescape(cells[cellIndex]) : string.Empty;
                _values[language.Value][key] = value;
            }
        }

        private static string Unescape(string value) => value.Replace("\\n", "\n").Replace("\\t", "\t");
    }
}
