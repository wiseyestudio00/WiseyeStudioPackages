namespace WiseyeStudio
{
    /// <summary>
    /// Central definition of the "Wiseye Studio" asset-creation menu tree.
    /// CreateAssetMenu arguments must be compile-time constants, so packages
    /// compose their menu paths and orders from these consts instead of
    /// hard-coding strings. Lives in the runtime assembly because core's own
    /// CreateAssetMenu attributes sit on runtime ScriptableObjects.
    /// Unity draws a separator between adjacent menu items whose order values
    /// differ by more than 10 — leave gaps of 20 between logical groups.
    /// </summary>
    public static class AssetMenuConstants
    {
        public const string Root = "Wiseye Studio/";
        public const string DataProvider = Root + "Data Provider/";

        /// <summary>
        /// The "Data Provider" submenu, top to bottom. Other packages append
        /// their own providers starting at 20 (next group band).
        /// </summary>
        public static class Order
        {
            public const int StringSo = 0;
            public const int StringListSo = 1;
            public const int StringByPlatformSo = 2;
        }
    }
}
