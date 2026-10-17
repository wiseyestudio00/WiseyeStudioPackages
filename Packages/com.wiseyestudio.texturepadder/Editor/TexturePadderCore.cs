using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace WiseyeStudio.TexturePadder.Editor
{
    /// <summary>Rewrites a PNG with padding and fixes up its sprite data so nothing moves.</summary>
    public static class TexturePadderCore
    {
        public const string BackupRoot = "TexturePadderBackups";

        public static string ProjectRoot => Path.GetDirectoryName(Application.dataPath);

        public static string NewBackupFolder() =>
            Path.Combine(ProjectRoot, BackupRoot, DateTime.Now.ToString("yyyyMMdd_HHmmss"));

        /// <summary>Applies the entry's padding. Returns null on success, otherwise why it failed.</summary>
        public static string Apply(TextureEntry entry, PadOptions options, string backupFolder)
        {
            if (AssetImporter.GetAtPath(entry.Path) is not TextureImporter importer)
            {
                return "Importer not found";
            }

            var fullPath = Path.Combine(ProjectRoot, entry.Path);
            if (!File.Exists(fullPath))
            {
                return "Source file not found";
            }
            if ((File.GetAttributes(fullPath) & FileAttributes.ReadOnly) != 0)
            {
                return "File is read-only (check it out in version control first)";
            }

            importer.GetSourceTextureWidthAndHeight(out var sourceWidth, out var sourceHeight);
            if (sourceWidth != entry.Source.x || sourceHeight != entry.Source.y)
            {
                return "File changed since the scan; scan again";
            }

            var isSprite = importer.textureType == TextureImporterType.Sprite &&
                           importer.spriteImportMode != SpriteImportMode.None;
            var padding = entry.Padding;
            if (isSprite && !padding.IsZero)
            {
                var reason = CheckSpriteData(importer);
                if (reason != null)
                {
                    return reason;
                }
            }

            if (options.backup && !string.IsNullOrEmpty(backupFolder))
            {
                Backup(entry.Path, backupFolder);
            }

            if (entry.TurnOffSliceOnImport)
            {
                if (!SliceOnImport.Disable(entry.Path))
                {
                    return "Could not turn off Slice on Import";
                }
                AssetDatabase.ImportAsset(entry.Path, ImportAssetOptions.ForceUpdate);
                importer = (TextureImporter)AssetImporter.GetAtPath(entry.Path);
            }

            if (!padding.IsZero)
            {
                var reason = WritePaddedPng(fullPath, entry, options.fillColor);
                if (reason != null)
                {
                    return reason;
                }

                // Import at the new size first so the sprite rects can be shifted inside it.
                AssetDatabase.ImportAsset(entry.Path, ImportAssetOptions.ForceUpdate);
                importer = (TextureImporter)AssetImporter.GetAtPath(entry.Path);
                if (isSprite)
                {
                    ShiftSpriteData(importer, entry.Source, entry.Proposed, padding);
                }
            }

            if (entry.DisableMipmaps)
            {
                importer.mipmapEnabled = false;
            }
            importer.SaveAndReimport();
            return null;
        }

        /// <summary>Copies every file in a backup folder back over the project and reimports.</summary>
        public static int Restore(string backupFolder)
        {
            var restored = new List<string>();
            foreach (var file in Directory.GetFiles(backupFolder, "*", SearchOption.AllDirectories))
            {
                var relativePath = file.Substring(backupFolder.Length).TrimStart('\\', '/').Replace('\\', '/');
                if (!relativePath.StartsWith("Assets/", StringComparison.Ordinal))
                {
                    continue;
                }

                File.Copy(file, Path.Combine(ProjectRoot, relativePath), true);
                if (!relativePath.EndsWith(".meta", StringComparison.Ordinal))
                {
                    restored.Add(relativePath);
                }
            }

            AssetDatabase.Refresh();
            foreach (var path in restored)
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
            return restored.Count;
        }

        private static string WritePaddedPng(string fullPath, TextureEntry entry, Color32 fill)
        {
            var source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            Texture2D padded = null;
            try
            {
                if (!source.LoadImage(File.ReadAllBytes(fullPath), false))
                {
                    return "Could not decode PNG";
                }
                if (source.width != entry.Source.x || source.height != entry.Source.y)
                {
                    return $"Decoded size {source.width}×{source.height} does not match the importer";
                }

                var width = entry.Proposed.x;
                var height = entry.Proposed.y;
                var padding = entry.Padding;
                var isHighBitDepth = source.format is TextureFormat.RGBA64 or TextureFormat.RGB48 or TextureFormat.R16 or TextureFormat.RG32;
                if (isHighBitDepth)
                {
                    padded = new Texture2D(width, height, TextureFormat.RGBA64, false);
                    padded.SetPixels(Enumerable.Repeat((Color)fill, width * height).ToArray());
                    padded.SetPixels(padding.Left, padding.Bottom, source.width, source.height, source.GetPixels());
                }
                else
                {
                    padded = new Texture2D(width, height, TextureFormat.RGBA32, false);
                    padded.SetPixels32(Enumerable.Repeat(fill, width * height).ToArray());
                    padded.SetPixels32(padding.Left, padding.Bottom, source.width, source.height, source.GetPixels32());
                }
                padded.Apply(false);
                File.WriteAllBytes(fullPath, padded.EncodeToPNG());
                return null;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(source);
                if (padded != null)
                {
                    UnityEngine.Object.DestroyImmediate(padded);
                }
            }
        }

        private static ISpriteEditorDataProvider GetProvider(TextureImporter importer)
        {
            var factories = new SpriteDataProviderFactories();
            factories.Init();
            var provider = factories.GetSpriteEditorDataProviderFromObject(importer);
            provider?.InitSpriteEditorDataProvider();
            return provider;
        }

        /// <summary>
        /// 2D Animation skinning data (bones, custom meshes) has its own coordinate rules;
        /// refuse rather than risk corrupting a rig.
        /// </summary>
        private static string CheckSpriteData(TextureImporter importer)
        {
            var provider = GetProvider(importer);
            if (provider == null)
            {
                return "No sprite data provider";
            }

            var bones = provider.HasDataProvider(typeof(ISpriteBoneDataProvider)) ? provider.GetDataProvider<ISpriteBoneDataProvider>() : null;
            var mesh = provider.HasDataProvider(typeof(ISpriteMeshDataProvider)) ? provider.GetDataProvider<ISpriteMeshDataProvider>() : null;
            foreach (var rect in provider.GetSpriteRects())
            {
                if (bones?.GetBones(rect.spriteID)?.Count > 0 || mesh?.GetVertices(rect.spriteID)?.Length > 0)
                {
                    return $"Sprite '{rect.name}' has 2D Animation skinning data; pad it manually";
                }
            }
            return null;
        }

        private static void ShiftSpriteData(TextureImporter importer, Vector2Int oldSize, Vector2Int newSize, Padding padding)
        {
            var provider = GetProvider(importer);
            if (provider == null)
            {
                return;
            }

            var rects = provider.GetSpriteRects();
            if (provider.spriteImportMode == SpriteImportMode.Multiple)
            {
                // Pivots, borders and outlines are relative to each rect, so moving the rect is enough.
                foreach (var rect in rects)
                {
                    rect.rect = new Rect(rect.rect.x + padding.Left, rect.rect.y + padding.Bottom, rect.rect.width, rect.rect.height);
                }
            }
            else if (rects.Length > 0)
            {
                // Single sprite: the rect is the whole texture, so it grows. Keep the pivot and the
                // 9-slice lines on the same content pixels.
                var sprite = rects[0];
                var oldPivot = PivotOf(sprite.alignment, sprite.pivot);
                var pivotPixels = Vector2.Scale(oldPivot, oldSize);
                var newPivot = new Vector2((pivotPixels.x + padding.Left) / newSize.x, (pivotPixels.y + padding.Bottom) / newSize.y);
                if ((oldPivot - newPivot).sqrMagnitude > 1e-8f)
                {
                    sprite.alignment = SpriteAlignment.Custom;
                }
                sprite.pivot = newPivot;
                if (sprite.border != Vector4.zero)
                {
                    sprite.border += new Vector4(padding.Left, padding.Bottom, padding.Right, padding.Top);
                }
                sprite.rect = new Rect(0, 0, newSize.x, newSize.y);

                // Outlines are stored relative to the rect centre, which moved.
                var shift = new Vector2((padding.Left - padding.Right) * 0.5f, (padding.Bottom - padding.Top) * 0.5f);
                if (shift != Vector2.zero)
                {
                    if (provider.HasDataProvider(typeof(ISpriteOutlineDataProvider)))
                    {
                        var outlines = provider.GetDataProvider<ISpriteOutlineDataProvider>();
                        outlines.SetOutlines(sprite.spriteID, Offset(outlines.GetOutlines(sprite.spriteID), shift));
                    }
                    if (provider.HasDataProvider(typeof(ISpritePhysicsOutlineDataProvider)))
                    {
                        var physics = provider.GetDataProvider<ISpritePhysicsOutlineDataProvider>();
                        physics.SetOutlines(sprite.spriteID, Offset(physics.GetOutlines(sprite.spriteID), shift));
                    }
                }
            }

            provider.SetSpriteRects(rects);
            provider.Apply();
        }

        private static List<Vector2[]> Offset(List<Vector2[]> shapes, Vector2 shift) =>
            shapes?.Select(shape => shape.Select(point => point + shift).ToArray()).ToList();

        private static Vector2 PivotOf(SpriteAlignment alignment, Vector2 custom) => alignment switch
        {
            SpriteAlignment.Center => new Vector2(0.5f, 0.5f),
            SpriteAlignment.TopLeft => new Vector2(0f, 1f),
            SpriteAlignment.TopCenter => new Vector2(0.5f, 1f),
            SpriteAlignment.TopRight => new Vector2(1f, 1f),
            SpriteAlignment.LeftCenter => new Vector2(0f, 0.5f),
            SpriteAlignment.RightCenter => new Vector2(1f, 0.5f),
            SpriteAlignment.BottomLeft => new Vector2(0f, 0f),
            SpriteAlignment.BottomCenter => new Vector2(0.5f, 0f),
            SpriteAlignment.BottomRight => new Vector2(1f, 0f),
            _ => custom,
        };

        private static void Backup(string assetPath, string backupFolder)
        {
            foreach (var path in new[] { assetPath, assetPath + ".meta" })
            {
                var source = Path.Combine(ProjectRoot, path);
                if (!File.Exists(source))
                {
                    continue;
                }

                var destination = Path.Combine(backupFolder, path);
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
                if (!File.Exists(destination))
                {
                    File.Copy(source, destination);
                }
            }
        }
    }
}
