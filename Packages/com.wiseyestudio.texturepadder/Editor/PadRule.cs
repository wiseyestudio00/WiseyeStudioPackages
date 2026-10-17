namespace WiseyeStudio.TexturePadder.Editor
{
    /// <summary>What a padded size has to satisfy.</summary>
    public enum PadRule
    {
        /// <summary>Width and height multiples of 4 (the DXT/BC/ETC2 block size). Enough when mipmaps are off.</summary>
        MultipleOfFour,

        /// <summary>Width and height powers of two. Needed for compression when mipmaps are on.</summary>
        PowerOfTwo,
    }
}
