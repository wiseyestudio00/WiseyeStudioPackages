using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WiseyeStudio.TexturePadder.Editor
{
    public class TexturePadderWindow : EditorWindow
    {
        public const string MenuPath = "Wiseye Studio/Texture Padding";
        private const string ScanSelectionMenuPath = "Assets/Wiseye Studio/Texture Padding/Scan Selection";

        private static readonly Color OkColor = new(0.35f, 0.75f, 0.4f);
        private static readonly Color WarnColor = new(0.95f, 0.7f, 0.25f);
        private static readonly Color BadColor = new(0.9f, 0.4f, 0.35f);

        private enum Filter
        {
            All,
            NeedsPadding,
            Problems,
        }

        [SerializeField] private PadOptions options = new();
        [SerializeField] private bool showOptions = true;

        private Filter _filter = Filter.NeedsPadding;
        private List<TextureEntry> _entries = new();
        private List<string> _lastRoots = new();
        private Vector2 _scroll;
        private string _lastBackup;

        [MenuItem(MenuPath)]
        public static TexturePadderWindow Open()
        {
            var window = GetWindow<TexturePadderWindow>("Texture Padder");
            window.minSize = new Vector2(720, 320);
            return window;
        }

        [MenuItem(ScanSelectionMenuPath)]
        private static void ScanSelectionMenu() => Open().Scan(SelectedPaths());

        [MenuItem(ScanSelectionMenuPath, true)]
        private static bool CanScanSelection() => SelectedPaths().Count > 0;

        private static List<string> SelectedPaths() =>
            Selection.assetGUIDs.Select(AssetDatabase.GUIDToAssetPath).Where(path => path.StartsWith("Assets")).ToList();

        // Scans and file writes change the list mid-event, so run them after OnGUI.
        private static void Defer(Action action) => EditorApplication.delayCall += () => action();

        public void Scan(List<string> roots)
        {
            _lastRoots = roots.ToList();
            var paths = TextureAnalyzer.CollectTexturePaths(roots);
            var atlasIndex = TextureAnalyzer.BuildAtlasIndex();
            var entries = new List<TextureEntry>(paths.Count);
            try
            {
                for (var i = 0; i < paths.Count; i++)
                {
                    if (i % 20 == 0 && EditorUtility.DisplayCancelableProgressBar("Texture Padder", paths[i], (float)i / paths.Count))
                    {
                        break;
                    }
                    entries.Add(TextureAnalyzer.Analyze(paths[i], options, atlasIndex));
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.UnloadUnusedAssetsImmediate();
            }

            _entries = entries.OrderBy(entry => entry.Status).ThenBy(entry => entry.Path).ToList();
            Repaint();
        }

        private void OnGUI()
        {
            DrawToolbar();
            DrawOptions();
            DrawSummary();
            DrawList();
            DrawFooter();
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                using (new EditorGUI.DisabledScope(SelectedPaths().Count == 0))
                {
                    if (GUILayout.Button("Scan Selection", EditorStyles.toolbarButton, GUILayout.Width(100)))
                    {
                        Defer(() => Scan(SelectedPaths()));
                    }
                }
                if (GUILayout.Button("Scan All Assets", EditorStyles.toolbarButton, GUILayout.Width(100)))
                {
                    Defer(() => Scan(new List<string> { "Assets" }));
                }
                using (new EditorGUI.DisabledScope(_lastRoots.Count == 0))
                {
                    if (GUILayout.Button("Rescan", EditorStyles.toolbarButton, GUILayout.Width(60)))
                    {
                        Defer(() => Scan(_lastRoots));
                    }
                }
                GUILayout.FlexibleSpace();
                _filter = (Filter)EditorGUILayout.EnumPopup(_filter, EditorStyles.toolbarDropDown, GUILayout.Width(110));
            }
        }

        private void DrawOptions()
        {
            showOptions = EditorGUILayout.Foldout(showOptions, "Options", true);
            if (!showOptions)
            {
                return;
            }

            using (new EditorGUI.IndentLevelScope())
            using (var change = new EditorGUI.ChangeCheckScope())
            {
                options.horizontalAnchor = (HorizontalAnchor)EditorGUILayout.EnumPopup(
                    new GUIContent("Content horizontal", "Where the original image sits; padding goes on the other side(s)."),
                    options.horizontalAnchor);
                options.verticalAnchor = (VerticalAnchor)EditorGUILayout.EnumPopup("Content vertical", options.verticalAnchor);
                options.fillColor = EditorGUILayout.ColorField(
                    new GUIContent("Padding color", "Transparent by default. Opaque PNGs gain an alpha channel."),
                    options.fillColor);
                options.mipmapHandling = (MipmapHandling)EditorGUILayout.EnumPopup(
                    new GUIContent("Mipmapped textures",
                        "Unity won't compress a non-power-of-two texture that has mipmaps, in any format. " +
                        "Pad to power of two, or pad to ×4 and turn mipmaps off."),
                    options.mipmapHandling);
                options.skipAtlasPacked = EditorGUILayout.Toggle(
                    new GUIContent("Skip Sprite Atlas members", "Packed sprites ship inside the atlas texture, so their own compression doesn't matter."),
                    options.skipAtlasPacked);
                options.turnOffSliceOnImport = EditorGUILayout.Toggle(
                    new GUIContent("Turn off Slice on Import",
                        "Sprite sheets with Slice on Import get re-sliced on every import, which would undo the rect shift. " +
                        "When on, the flag is turned off and the current rects are kept (shifted)."),
                    options.turnOffSliceOnImport);
                options.backup = EditorGUILayout.Toggle(
                    new GUIContent("Back up originals", $"Copies the PNG and .meta to <project>/{TexturePadderCore.BackupRoot}/ first."),
                    options.backup);
                TexturePadderImportWatcher.Enabled = EditorGUILayout.Toggle(
                    new GUIContent("Warn on import", "Log a console warning whenever a texture imports at a size that blocks compression."),
                    TexturePadderImportWatcher.Enabled);

                if (change.changed && _lastRoots.Count > 0)
                {
                    Defer(() => Scan(_lastRoots));
                }
            }
        }

        private void DrawSummary()
        {
            if (_entries.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "Select textures or folders in the Project window and press Scan Selection.\n" +
                    "Sizes are checked after Max Size downscaling for every platform override: " +
                    "1400×500 at Max Size 1024 imports as 1024×366, which DXT/BC/ETC2 can't compress.",
                    MessageType.Info);
                return;
            }

            var needsPadding = _entries.Count(entry => entry.Status == PadStatus.NeedsPadding);
            var compressible = _entries.Count(entry => entry.Status == PadStatus.Compressible);
            var skipped = _entries.Count(entry => entry.Status == PadStatus.Skipped);
            var unsupported = _entries.Count(entry => entry.Status == PadStatus.Unsupported);
            EditorGUILayout.LabelField(
                $"{_entries.Count} textures · {needsPadding} need padding · {compressible} already fine · {skipped} skipped · " +
                $"{unsupported} unsupported   (active platform: {EditorUserBuildSettings.activeBuildTarget})",
                EditorStyles.miniBoldLabel);
        }

        private void DrawList()
        {
            var header = EditorStyles.miniBoldLabel;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(22);
                GUILayout.Label("Texture", header, GUILayout.MinWidth(160));
                GUILayout.Label("Source", header, GUILayout.Width(80));
                GUILayout.Label("Max sizes", header, GUILayout.Width(80));
                GUILayout.Label("Imported now", header, GUILayout.Width(150));
                GUILayout.Label("Proposed", header, GUILayout.Width(150));
                GUILayout.Label("Status", header, GUILayout.MinWidth(160));
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (var entry in _entries.Where(IsVisible))
            {
                DrawRow(entry);
            }
            EditorGUILayout.EndScrollView();
        }

        private static void DrawRow(TextureEntry entry)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(entry.Status != PadStatus.NeedsPadding))
                {
                    entry.Selected = EditorGUILayout.Toggle(entry.Selected, GUILayout.Width(18));
                }

                if (GUILayout.Button(new GUIContent(entry.FileName, entry.Path), EditorStyles.label, GUILayout.MinWidth(160)))
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(entry.Path));
                }

                GUILayout.Label(TextureAnalyzer.Format(entry.Source), GUILayout.Width(80));
                GUILayout.Label(string.Join(",", entry.MaxSizes), GUILayout.Width(80));

                var previousColor = GUI.contentColor;
                GUI.contentColor = entry.ActiveCompressed ? OkColor : WarnColor;
                GUILayout.Label(new GUIContent($"{TextureAnalyzer.Format(entry.ActiveImported)} {entry.ActiveFormat}",
                    entry.ActiveCompressed ? "Compressed on the active platform" : "Not compressed on the active platform"),
                    GUILayout.Width(150));
                GUI.contentColor = previousColor;

                GUILayout.Label(new GUIContent(ProposedText(entry), entry.Padding.ToString()), GUILayout.Width(150));

                GUI.contentColor = StatusColor(entry.Status);
                GUILayout.Label(new GUIContent(StatusText(entry), entry.Note), GUILayout.MinWidth(160));
                GUI.contentColor = previousColor;
            }
        }

        private void DrawFooter()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var candidates = _entries.Where(entry => entry.Status == PadStatus.NeedsPadding).ToList();
                if (GUILayout.Button("Select all", GUILayout.Width(80)))
                {
                    candidates.ForEach(entry => entry.Selected = true);
                }
                if (GUILayout.Button("Select none", GUILayout.Width(80)))
                {
                    candidates.ForEach(entry => entry.Selected = false);
                }
                if (GUILayout.Button("Restore backup…", GUILayout.Width(110)))
                {
                    Defer(RestoreBackup);
                }
                GUILayout.FlexibleSpace();

                var targets = candidates.Where(entry => entry.Selected).ToList();
                using (new EditorGUI.DisabledScope(targets.Count == 0))
                {
                    if (GUILayout.Button($"Pad {targets.Count} texture{(targets.Count == 1 ? "" : "s")}", GUILayout.Width(140), GUILayout.Height(24)))
                    {
                        Defer(() => ApplySelected(targets));
                    }
                }
            }

            if (!string.IsNullOrEmpty(_lastBackup))
            {
                EditorGUILayout.LabelField($"Last backup: {_lastBackup}", EditorStyles.miniLabel);
            }
        }

        private bool IsVisible(TextureEntry entry) => _filter switch
        {
            Filter.NeedsPadding => entry.Status == PadStatus.NeedsPadding,
            Filter.Problems => entry.Status != PadStatus.Compressible,
            _ => true,
        };

        private static string ProposedText(TextureEntry entry)
        {
            if (entry.Status != PadStatus.NeedsPadding)
            {
                return string.Empty;
            }
            if (entry.Padding.IsZero)
            {
                return "mipmaps off";
            }
            return TextureAnalyzer.Format(entry.Proposed) + (entry.DisableMipmaps ? " no mips" : string.Empty);
        }

        private static string StatusText(TextureEntry entry) => entry.Status switch
        {
            PadStatus.Compressible => string.IsNullOrEmpty(entry.Note) ? "OK" : "OK – " + entry.Note,
            PadStatus.NeedsPadding => "Needs padding " + (entry.Rule == PadRule.PowerOfTwo ? "(POT, mipmaps)" : "(×4)"),
            _ => entry.Note,
        };

        private static Color StatusColor(PadStatus status) => status switch
        {
            PadStatus.Compressible => OkColor,
            PadStatus.NeedsPadding => WarnColor,
            PadStatus.Unsupported => BadColor,
            _ => GUI.contentColor,
        };

        private void ApplySelected(List<TextureEntry> targets)
        {
            var backupFolder = options.backup ? TexturePadderCore.NewBackupFolder() : null;
            var message = $"Overwrite {targets.Count} PNG file(s) in place with padded versions?\n\n" +
                          "GUIDs and references stay the same; sprite rects and pivots are shifted to match.\n\n" +
                          (backupFolder != null
                              ? $"Originals are copied to:\n{backupFolder}"
                              : "Backups are OFF. Make sure the files are in version control.");
            if (!EditorUtility.DisplayDialog("Texture Padder", message, "Pad", "Cancel"))
            {
                return;
            }

            var failures = new List<string>();
            try
            {
                for (var i = 0; i < targets.Count; i++)
                {
                    var entry = targets[i];
                    EditorUtility.DisplayProgressBar("Texture Padder", entry.Path, (float)i / targets.Count);
                    var error = TexturePadderCore.Apply(entry, options, backupFolder);
                    if (error != null)
                    {
                        failures.Add($"{entry.Path}: {error}");
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            var hasBackup = backupFolder != null && Directory.Exists(backupFolder);
            if (hasBackup)
            {
                _lastBackup = backupFolder;
            }

            Debug.Log($"[Texture Padder] Padded {targets.Count - failures.Count} of {targets.Count} texture(s)." +
                      (hasBackup ? $" Backup: {backupFolder}" : string.Empty));
            if (failures.Count > 0)
            {
                Debug.LogWarning($"[Texture Padder] {failures.Count} texture(s) were not padded:\n{string.Join("\n", failures)}");
                EditorUtility.DisplayDialog("Texture Padder", $"{failures.Count} texture(s) were not padded. See the Console for reasons.", "OK");
            }

            Scan(_lastRoots);
        }

        private void RestoreBackup()
        {
            var backupRoot = Path.Combine(TexturePadderCore.ProjectRoot, TexturePadderCore.BackupRoot);
            var folder = EditorUtility.OpenFolderPanel("Restore Texture Padder backup",
                Directory.Exists(backupRoot) ? backupRoot : TexturePadderCore.ProjectRoot, string.Empty);
            if (string.IsNullOrEmpty(folder))
            {
                return;
            }
            if (!EditorUtility.DisplayDialog("Texture Padder", $"Copy every file in\n{folder}\nback over the project?", "Restore", "Cancel"))
            {
                return;
            }

            var count = TexturePadderCore.Restore(folder);
            Debug.Log($"[Texture Padder] Restored {count} texture(s) from {folder}");
            if (_lastRoots.Count > 0)
            {
                Scan(_lastRoots);
            }
        }
    }
}
