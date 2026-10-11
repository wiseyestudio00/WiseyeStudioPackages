using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine.Networking;
#endif

namespace WiseyeStudio.Localization
{
    [CreateAssetMenu(
        fileName = "Localization Table",
        menuName = AssetMenuConstants.Root + "Localization/Localization Table (Google Sheet)")]
    public class LocalizationTableFromGoogleSheetScriptableObject : ScriptableObject, ILocalizationTable
    {
        [SerializeField] private TextAsset csv;

        [Tooltip("Row 1 names the language columns. When off, columns follow the language order in Localization Settings.")]
        [SerializeField] private bool hasHeaderRow = false;

        [Title("Google Sheet")]
        [Tooltip("The long ID in the sheet URL: docs.google.com/spreadsheets/d/<ID>/edit")]
        [SerializeField] private string sheetId;

        [Tooltip("The tab's gid in the sheet URL: ...edit?gid=<gid>")]
        [SerializeField] private string gid;

        [NonSerialized] private LocalizationTable _table;

        public TextAsset Csv => csv;
        public bool HasHeaderRow => hasHeaderRow;
        public string SheetId => sheetId;
        public string Gid => gid;

        public LocalizationTable Table => _table ??= Load();

        public IReadOnlyList<string> Keys => Table.Keys;
        public IReadOnlyList<SystemLanguage> Languages => Table.Languages;

        public bool ContainsKey(string key) => Table.ContainsKey(key);

        public bool TryGetString(string key, SystemLanguage language, out string value) =>
            Table.TryGetString(key, language, out value);

        public string GetString(string key) => Table.GetString(key);

        public string GetString(string key, SystemLanguage language) => Table.GetString(key, language);

        public string ReplaceKeyedTags(string text) => Table.ReplaceKeyedTags(text);

        public string ReplaceKeyedTags(string text, SystemLanguage language) => Table.ReplaceKeyedTags(text, language);

        public void ClearCache() => _table = null;

        private void OnValidate() => ClearCache();

        private LocalizationTable Load()
        {
            if (csv == null)
            {
                Debug.LogError($"{name} has no CSV assigned.", this);
                return LocalizationTable.Parse(string.Empty, hasHeaderRow);
            }

            var table = LocalizationTable.Parse(csv.text, hasHeaderRow);
            if (Application.isPlaying)
            {
                foreach (var problem in table.Problems)
                {
                    Debug.LogWarning($"{name}: {problem}", this);
                }
            }

            return table;
        }

#if UNITY_EDITOR
        private bool HasSheet => !string.IsNullOrWhiteSpace(sheetId) && !string.IsNullOrWhiteSpace(gid);
        private bool HasProblems => csv != null && Table.Problems.Count > 0;

        [Title("Preview")]
        [ShowInInspector, ReadOnly, ShowIf(nameof(csv))]
        private int KeyCount => Table.Keys.Count;

        [ShowInInspector, ReadOnly, ShowIf(nameof(csv))]
        private string Columns => string.Join(", ", Table.Languages);

        [ShowInInspector, ReadOnly, ShowIf(nameof(HasProblems))]
        private IReadOnlyList<string> Problems => Table.Problems;

        [Button]
        private void SetSheetUrl(string url)
        {
            if (!TryParseSheetUrl(url, out var parsedSheetId, out var parsedGid))
            {
                Debug.LogError($"Couldn't read a Google Sheet ID from \"{url}\".", this);
                return;
            }

            Undo.RecordObject(this, "Set Sheet URL");
            sheetId = parsedSheetId;
            gid = parsedGid;
            EditorUtility.SetDirty(this);
        }

        [Button, EnableIf(nameof(HasSheet))]
        private void OpenSheet()
        {
            Application.OpenURL($"https://docs.google.com/spreadsheets/d/{sheetId}/edit?gid={gid}#gid={gid}");
        }

        [NonSerialized] private bool _isSyncing;

        private bool CanSync => HasSheet && !_isSyncing;

        [Button, EnableIf(nameof(CanSync))]
        public async UniTask<bool> SyncAsync()
        {
            if (!HasSheet)
            {
                Debug.LogError($"{name}: set the sheet ID and gid before syncing.", this);
                return false;
            }

            if (_isSyncing)
            {
                Debug.LogWarning($"{name}: a sync is already running.", this);
                return false;
            }

            _isSyncing = true;
            try
            {
                return await SyncUnguardedAsync();
            }
            finally
            {
                _isSyncing = false;
            }
        }

        private async UniTask<bool> SyncUnguardedAsync()
        {
            string downloaded;
            try
            {
                downloaded = await DownloadCsvAsync();
            }
            catch (Exception exception)
            {
                Debug.LogError($"{name}: download failed. {exception.Message}", this);
                return false;
            }

            var csvPath = csv != null
                ? AssetDatabase.GetAssetPath(csv)
                : Path.ChangeExtension(AssetDatabase.GetAssetPath(this), ".csv");

            try
            {
                File.WriteAllText(csvPath, downloaded);
                AssetDatabase.ImportAsset(csvPath);
            }
            catch (Exception exception)
            {
                Debug.LogError($"{name}: couldn't write {csvPath}. {exception.Message}", this);
                return false;
            }

            if (csv == null)
            {
                csv = AssetDatabase.LoadAssetAtPath<TextAsset>(csvPath);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
            }

            ClearCache();
            LogSyncReport();
            return true;
        }

        private async UniTask<string> DownloadCsvAsync()
        {
            var url = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format=csv&gid={gid}";
            using var request = UnityWebRequest.Get(url);
            await request.SendWebRequest();

            var contentType = request.GetResponseHeader("Content-Type") ?? string.Empty;
            if (!contentType.StartsWith("text/csv", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Google returned a web page instead of CSV. Share the sheet as \"Anyone with the link can view\".");
            }

            return request.downloadHandler.text;
        }

        private void LogSyncReport()
        {
            var table = Table;
            var report = new StringBuilder($"{name}: synced {table.Keys.Count} keys.");

            foreach (var language in LocalizationSettings.Instance.Languages)
            {
                if (!table.Languages.Contains(language))
                {
                    report.Append($"\n  {language}: no column");
                    continue;
                }

                var missing = table.Keys.Count(key => !table.TryGetString(key, language, out _));
                if (missing > 0)
                {
                    report.Append($"\n  {language}: {missing} missing");
                }
            }

            foreach (var problem in table.Problems)
            {
                report.Append($"\n  {problem}");
            }

            if (table.Problems.Count > 0)
            {
                Debug.LogWarning(report.ToString(), this);
            }
            else
            {
                Debug.Log(report.ToString(), this);
            }
        }

        private static bool TryParseSheetUrl(string url, out string parsedSheetId, out string parsedGid)
        {
            parsedSheetId = null;
            parsedGid = null;

            if (!Uri.TryCreate(url?.Trim(), UriKind.Absolute, out var uri))
            {
                return false;
            }

            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var idIndex = Array.IndexOf(segments, "d") + 1;
            if (idIndex <= 0 || idIndex >= segments.Length)
            {
                return false;
            }

            parsedSheetId = segments[idIndex];
            parsedGid = FindUrlParameter(uri.Query, "gid") ?? FindUrlParameter(uri.Fragment, "gid") ?? "0";
            return true;
        }

        private static string FindUrlParameter(string urlPart, string parameterName)
        {
            foreach (var parameter in urlPart.TrimStart('?', '#').Split('&'))
            {
                var pair = parameter.Split('=');
                if (pair.Length == 2 && pair[0] == parameterName && pair[1].Length > 0)
                {
                    return pair[1];
                }
            }

            return null;
        }
#endif
    }
}
