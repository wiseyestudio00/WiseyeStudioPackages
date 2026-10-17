using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.U2D;

namespace WiseyeStudio.TexturePadder.Editor
{
    public static class TextureAnalyzer
    {
        // Importer platform names. Ones that aren't installed just report "not overridden".
        private static readonly string[] Platforms =
        {
            "Standalone", "Android", "iPhone", "WebGL", "tvOS", "VisionOS",
            "PS4", "PS5", "GameCoreXboxOne", "GameCoreScarlett", "Switch",
        };

        public static bool CanRewrite(string path) =>
            string.Equals(Path.GetExtension(path), ".png", StringComparison.OrdinalIgnoreCase);

        /// <summary>Every Max Size this texture can be imported at: the default plus each platform override.</summary>
        public static List<int> CollectMaxSizes(TextureImporter importer)
        {
            var sizes = new SortedSet<int> { importer.maxTextureSize };
            foreach (var platform in Platforms)
            {
                var settings = importer.GetPlatformTextureSettings(platform);
                if (settings != null && settings.overridden)
                {
                    sizes.Add(settings.maxTextureSize);
                }
            }

            // Build Settings > "Max Texture Size" override caps everything below it.
            var buildCap = EditorUserBuildSettings.overrideMaxTextureSize;
            if (buildCap > 0)
            {
                sizes = new SortedSet<int>(sizes.Select(size => Math.Min(size, buildCap)));
            }
            return sizes.ToList();
        }

        public static bool WantsCompression(TextureImporter importer)
        {
            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                return true;
            }

            foreach (var platform in Platforms)
            {
                var settings = importer.GetPlatformTextureSettings(platform);
                if (settings != null && settings.overridden && IsCompressedFormat(settings.format))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Sprite Atlas packing index: texture path to atlas path.</summary>
        public static Dictionary<string, string> BuildAtlasIndex()
        {
            var files = new Dictionary<string, string>();
            var folders = new List<(string Folder, string Atlas)>();
            foreach (var guid in AssetDatabase.FindAssets("t:SpriteAtlas"))
            {
                var atlasPath = AssetDatabase.GUIDToAssetPath(guid);
                var packed = new HashSet<string>();
                var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
                if (atlas != null)
                {
                    foreach (var packable in atlas.GetPackables())
                    {
                        packed.Add(AssetDatabase.GetAssetPath(packable));
                    }
                }

                // V2 atlases (.spriteatlasv2) declare their packables as import dependencies.
                if (atlasPath.EndsWith(".spriteatlasv2", StringComparison.OrdinalIgnoreCase))
                {
                    packed.UnionWith(AssetDatabase.GetDependencies(atlasPath, false));
                }

                foreach (var path in packed)
                {
                    if (string.IsNullOrEmpty(path) || path == atlasPath)
                    {
                        continue;
                    }

                    if (AssetDatabase.IsValidFolder(path))
                    {
                        folders.Add((path, atlasPath));
                    }
                    else
                    {
                        files[path] = atlasPath;
                    }
                }
            }

            // Expand folder packables to the textures inside them.
            foreach (var (folder, atlasPath) in folders)
            {
                foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { folder }))
                {
                    files.TryAdd(AssetDatabase.GUIDToAssetPath(guid), atlasPath);
                }
            }
            return files;
        }

        /// <summary>All texture paths under the given assets and folders (only inside Assets/).</summary>
        public static List<string> CollectTexturePaths(IEnumerable<string> roots)
        {
            var result = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var root in roots)
            {
                if (string.IsNullOrEmpty(root) || !root.StartsWith("Assets", StringComparison.Ordinal))
                {
                    continue;
                }

                if (AssetDatabase.IsValidFolder(root))
                {
                    foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { root }))
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        if (AssetImporter.GetAtPath(path) is TextureImporter)
                        {
                            result.Add(path);
                        }
                    }
                }
                else if (AssetImporter.GetAtPath(root) is TextureImporter)
                {
                    result.Add(root);
                }
            }
            return result.ToList();
        }

        public static TextureEntry Analyze(string path, PadOptions options, Dictionary<string, string> atlasIndex)
        {
            var entry = new TextureEntry { Path = path };
            if (AssetImporter.GetAtPath(path) is not TextureImporter importer)
            {
                return Fail(entry, PadStatus.Unsupported, "Not a texture");
            }

            importer.GetSourceTextureWidthAndHeight(out var width, out var height);
            entry.Source = new Vector2Int(width, height);
            entry.MaxSizes = CollectMaxSizes(importer);
            ReadActive(entry);

            if (importer.textureShape != TextureImporterShape.Texture2D)
            {
                return Fail(entry, PadStatus.Unsupported, "Only 2D textures (not cubemaps or arrays)");
            }

            if (atlasIndex != null && atlasIndex.TryGetValue(path, out var atlas))
            {
                entry.Atlas = atlas;
            }

            if (!WantsCompression(importer))
            {
                return Fail(entry, PadStatus.Skipped, "Compression is turned off for this texture");
            }

            // Sprites ignore the Non-Power of 2 setting; other types rescale NPOT to POT, which always compresses.
            if (importer.textureType != TextureImporterType.Sprite && importer.npotScale != TextureImporterNPOTScale.None)
            {
                entry.Status = PadStatus.Compressible;
                entry.Note = $"Importer rescales to power of two ({importer.npotScale})";
                return entry;
            }

            entry.Rule = PadRule.MultipleOfFour;
            if (importer.mipmapEnabled && !PadMath.IsCompressible(width, height, entry.MaxSizes, PadRule.PowerOfTwo))
            {
                switch (options.mipmapHandling)
                {
                    case MipmapHandling.PadToPowerOfTwo:
                        entry.Rule = PadRule.PowerOfTwo;
                        break;
                    case MipmapHandling.DisableMipmaps:
                        entry.DisableMipmaps = true;
                        break;
                    default:
                        return Fail(entry, PadStatus.Skipped,
                            "Mipmaps on + non-power-of-two size: can't compress. Change Mipmapped textures to fix");
                }
            }

            if (!entry.DisableMipmaps && PadMath.IsCompressible(width, height, entry.MaxSizes, entry.Rule))
            {
                entry.Status = PadStatus.Compressible;
                return entry;
            }

            if (!PadMath.TryFindPaddedSize(width, height, entry.MaxSizes, entry.Rule, out entry.Proposed))
            {
                return Fail(entry, PadStatus.Unsupported, $"No padded size within {PadMath.DefaultSearchLimit}px");
            }

            entry.Padding = PadMath.Distribute(entry.Source, entry.Proposed, options.horizontalAnchor, options.verticalAnchor);
            if (entry.Padding.IsZero && entry.DisableMipmaps)
            {
                entry.Note = "Size is fine; only mipmaps need turning off";
            }

            if (!CanRewrite(path))
            {
                return Fail(entry, PadStatus.Unsupported, $"Needs {Format(entry.Proposed)}, but only PNG files can be rewritten");
            }

            if (importer.textureType == TextureImporterType.Sprite &&
                importer.spriteImportMode == SpriteImportMode.Multiple && SliceOnImport.IsEnabled(path))
            {
                if (!options.turnOffSliceOnImport)
                {
                    return Fail(entry, PadStatus.Skipped,
                        "Slice on Import is on: Unity re-slices on every import and would undo the rect shift. " +
                        "Enable 'Turn off Slice on Import' or turn it off in the Sprite Editor");
                }
                entry.TurnOffSliceOnImport = true;
                entry.Note = "Slice on Import will be turned off";
            }

            if (entry.Atlas != null && options.skipAtlasPacked)
            {
                return Fail(entry, PadStatus.Skipped, $"Packed in {Path.GetFileName(entry.Atlas)}; the atlas texture is what ships");
            }

            entry.Status = PadStatus.NeedsPadding;
            entry.Selected = true;
            return entry;
        }

        public static string Format(Vector2Int size) => $"{size.x}×{size.y}";

        private static bool IsCompressedFormat(TextureImporterFormat format)
        {
            var name = format.ToString();
            return name.StartsWith("DXT") || name.StartsWith("BC") || name.StartsWith("ETC") ||
                   name.StartsWith("EAC") || name.StartsWith("ASTC") || name.StartsWith("PVRTC");
        }

        private static void ReadActive(TextureEntry entry)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(entry.Path);
            if (texture == null)
            {
                return;
            }

            entry.ActiveImported = new Vector2Int(texture.width, texture.height);
            entry.ActiveFormat = texture.format.ToString();
            entry.ActiveCompressed = GraphicsFormatUtility.IsCompressedFormat(texture.graphicsFormat);
        }

        private static TextureEntry Fail(TextureEntry entry, PadStatus status, string note)
        {
            entry.Status = status;
            entry.Note = note;
            entry.Selected = false;
            return entry;
        }
    }
}
