namespace WiseyeStudio.Steam.Utils
{
    public enum TextFilteringContext
    {
        Unknown = 0,
        /// <summary>
        /// Game content, only legally required filtering is performed.
        /// </summary>
        GameContent = 1,
        /// <summary>
        /// Chat from another player.
        /// </summary>
        Chat = 2,
        /// <summary>
        /// Character or item name.
        /// </summary>
        Name = 3
    }
}
