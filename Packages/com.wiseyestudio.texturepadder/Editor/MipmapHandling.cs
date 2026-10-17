namespace WiseyeStudio.TexturePadder.Editor
{
    /// <summary>
    /// What to do with mipmapped textures. Unity won't compress a non-power-of-two texture that has mipmaps,
    /// in any format (ASTC included).
    /// </summary>
    public enum MipmapHandling
    {
        /// <summary>Pad up to powers of two so the texture compresses with mipmaps.</summary>
        PadToPowerOfTwo,

        /// <summary>Pad to multiples of 4 and turn mipmaps off (the usual choice for UI and sprites).</summary>
        DisableMipmaps,

        /// <summary>Leave mipmapped textures alone.</summary>
        Skip,
    }
}
