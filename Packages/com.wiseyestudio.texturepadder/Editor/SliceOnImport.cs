using System;
using System.IO;

namespace WiseyeStudio.TexturePadder.Editor
{
    /// <summary>
    /// The 2D Sprite package's "Slice on Import" flag. When on, the sheet is re-sliced on every import, which would
    /// undo a rect shift. The flag lives in internal sprite custom data, so it is read and written through the .meta
    /// text: "- key: SpriteEditor.SliceOnImport" followed by "value: True".
    /// </summary>
    internal static class SliceOnImport
    {
        private const string KeyLine = "- key: SpriteEditor.SliceOnImport";
        private const string ValuePrefix = "value: ";

        public static bool IsEnabled(string assetPath)
        {
            var metaPath = MetaPath(assetPath);
            if (!File.Exists(metaPath))
            {
                return false;
            }

            var lines = File.ReadAllLines(metaPath);
            for (var i = 0; i < lines.Length - 1; i++)
            {
                if (lines[i].Trim() == KeyLine)
                {
                    return lines[i + 1].Trim().Equals(ValuePrefix + "True", StringComparison.OrdinalIgnoreCase);
                }
            }
            return false;
        }

        /// <summary>Sets the flag to False. The caller reimports so the importer picks it up.</summary>
        public static bool Disable(string assetPath)
        {
            var metaPath = MetaPath(assetPath);
            if (!File.Exists(metaPath))
            {
                return false;
            }

            // Edit the text in place so line endings and encoding stay as Unity wrote them.
            var text = File.ReadAllText(metaPath);
            var key = text.IndexOf(KeyLine, StringComparison.Ordinal);
            if (key < 0)
            {
                return false;
            }
            var value = text.IndexOf(ValuePrefix, key + KeyLine.Length, StringComparison.Ordinal);
            if (value < 0)
            {
                return false;
            }

            var start = value + ValuePrefix.Length;
            var end = start;
            while (end < text.Length && text[end] != '\r' && text[end] != '\n')
            {
                end++;
            }
            File.WriteAllText(metaPath, text.Substring(0, start) + "False" + text.Substring(end));
            return true;
        }

        private static string MetaPath(string assetPath) => Path.Combine(TexturePadderCore.ProjectRoot, assetPath + ".meta");
    }
}
