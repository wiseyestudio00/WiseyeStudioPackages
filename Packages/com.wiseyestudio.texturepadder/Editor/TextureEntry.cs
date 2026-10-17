using System.Collections.Generic;
using UnityEngine;

namespace WiseyeStudio.TexturePadder.Editor
{
    /// <summary>One scanned texture: what Unity imports today and what padding would fix it.</summary>
    public class TextureEntry
    {
        public string Path;
        public Vector2Int Source;
        public Vector2Int ActiveImported;
        public string ActiveFormat;
        public bool ActiveCompressed;
        public List<int> MaxSizes = new();
        public PadRule Rule;
        public bool DisableMipmaps;
        public bool TurnOffSliceOnImport;
        public PadStatus Status;
        public string Note;
        public Vector2Int Proposed;
        public Padding Padding;
        public string Atlas;
        public bool Selected;

        public string FileName => System.IO.Path.GetFileName(Path);
    }
}
