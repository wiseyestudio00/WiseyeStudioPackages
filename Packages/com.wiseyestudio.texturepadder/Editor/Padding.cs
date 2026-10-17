namespace WiseyeStudio.TexturePadder.Editor
{
    /// <summary>Pixels added on each side, in Unity texture space (y up).</summary>
    public struct Padding
    {
        public int Left;
        public int Right;
        public int Bottom;
        public int Top;

        public bool IsZero => Left == 0 && Right == 0 && Bottom == 0 && Top == 0;

        public override string ToString() => $"L{Left} R{Right} B{Bottom} T{Top}";
    }
}
