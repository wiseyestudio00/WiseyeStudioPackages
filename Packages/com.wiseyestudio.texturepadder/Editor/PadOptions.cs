using System;
using UnityEngine;

namespace WiseyeStudio.TexturePadder.Editor
{
    [Serializable]
    public class PadOptions
    {
        public HorizontalAnchor horizontalAnchor = HorizontalAnchor.Center;
        public VerticalAnchor verticalAnchor = VerticalAnchor.Middle;
        public Color32 fillColor = new(0, 0, 0, 0);
        public MipmapHandling mipmapHandling = MipmapHandling.PadToPowerOfTwo;
        public bool skipAtlasPacked = true;
        public bool turnOffSliceOnImport;
        public bool backup = true;
    }
}
