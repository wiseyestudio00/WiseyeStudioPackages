using UnityEditor;
using UnityEngine;

namespace WiseyeStudio.TexturePadder.Editor
{
    /// <summary>Optional console warning when a texture imports at a size that blocks block compression.</summary>
    public class TexturePadderImportWatcher : AssetPostprocessor
    {
        private const string PrefKey = "WiseyeStudio.TexturePadder.WarnOnImport";

        public static bool Enabled
        {
            get => EditorPrefs.GetBool(PrefKey, false);
            set => EditorPrefs.SetBool(PrefKey, value);
        }

        private void OnPostprocessTexture(Texture2D texture)
        {
            if (!Enabled)
            {
                return;
            }

            var importer = (TextureImporter)assetImporter;
            if (importer.textureShape != TextureImporterShape.Texture2D || !TextureAnalyzer.WantsCompression(importer))
            {
                return;
            }

            // The texture here is already at its imported (Max Size) size.
            var width = texture.width;
            var height = texture.height;
            var isBlockAligned = width % 4 == 0 && height % 4 == 0;
            var isNpotWithMipmaps = importer.mipmapEnabled && !(PadMath.IsPowerOfTwo(width) && PadMath.IsPowerOfTwo(height));
            if (isBlockAligned && !isNpotWithMipmaps)
            {
                return;
            }

            var reason = isNpotWithMipmaps
                ? "mipmaps are on and the size is not a power of two (no format compresses this)"
                : "it is not a multiple of 4 (DXT/BC/ETC2 fall back to uncompressed; ASTC is unaffected)";
            Debug.LogWarning($"[Texture Padder] {assetPath} imports at {width}×{height}: {reason}. " +
                             $"Open {TexturePadderWindow.MenuPath} to pad it.");
        }
    }
}
