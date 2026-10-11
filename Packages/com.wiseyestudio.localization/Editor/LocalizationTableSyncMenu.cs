using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace WiseyeStudio.Localization.Editor
{
    public static class LocalizationTableSyncMenu
    {
        private const string MenuPath = "Assets/Wiseye Studio/Localization/Sync Localization Tables";

        [MenuItem(MenuPath)]
        private static void SyncSelected()
        {
            SyncAllAsync(FindSelectedTables()).Forget();
        }

        [MenuItem(MenuPath, true)]
        private static bool CanSyncSelected() => FindSelectedTables().Count > 0;

        private static async UniTask SyncAllAsync(IReadOnlyList<LocalizationTableFromGoogleSheetScriptableObject> tables)
        {
            var failed = new List<string>();
            foreach (var table in tables)
            {
                if (!await table.SyncAsync())
                {
                    failed.Add(AssetDatabase.GetAssetPath(table));
                }
            }

            var summary = $"Synced {tables.Count - failed.Count} of {tables.Count} localization tables.";
            if (failed.Count == 0)
            {
                Debug.Log(summary);
            }
            else
            {
                Debug.LogWarning($"{summary} Failed:\n{string.Join("\n", failed)}");
            }
        }

        private static List<LocalizationTableFromGoogleSheetScriptableObject> FindSelectedTables()
        {
            var tables = new List<LocalizationTableFromGoogleSheetScriptableObject>();
            var folders = new List<string>();

            foreach (var selected in Selection.GetFiltered<Object>(SelectionMode.Assets))
            {
                if (selected is LocalizationTableFromGoogleSheetScriptableObject table)
                {
                    tables.Add(table);
                    continue;
                }

                var path = AssetDatabase.GetAssetPath(selected);
                if (AssetDatabase.IsValidFolder(path))
                {
                    folders.Add(path);
                }
            }

            if (folders.Count == 0)
            {
                return tables;
            }

            foreach (var guid in AssetDatabase.FindAssets($"t:{nameof(LocalizationTableFromGoogleSheetScriptableObject)}", folders.ToArray()))
            {
                var table = AssetDatabase.LoadAssetAtPath<LocalizationTableFromGoogleSheetScriptableObject>(AssetDatabase.GUIDToAssetPath(guid));
                if (table != null && !tables.Contains(table))
                {
                    tables.Add(table);
                }
            }

            return tables;
        }
    }
}
