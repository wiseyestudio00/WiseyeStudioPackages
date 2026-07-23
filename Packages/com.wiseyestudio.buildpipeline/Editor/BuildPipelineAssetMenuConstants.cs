namespace WiseyeStudio.Build
{
    /// <summary>
    /// Build-pipeline additions to the shared Wiseye Studio asset-creation
    /// menu (see <see cref="AssetMenuConstants"/> in com.wiseyestudio.core).
    /// The Order class lists each submenu top to bottom; gaps of 20 between
    /// bands render as separators in the Create menu.
    /// </summary>
    public static class BuildPipelineAssetMenuConstants
    {
        public const string Root = AssetMenuConstants.Root + "Build Pipeline/";
        public const string BuildMethod = Root + "Build Method/";

        public const string BuildMethodFilePrefix = "Build Method - ";

        public static class Order
        {
            public const int Pipeline = 0;

            // Build Method: player and build settings
            public const int SwitchBuildTarget = 0;
            public const int SetSceneList = 1;
            public const int SetExtraDefineSymbols = 3;
            public const int AndroidKeystoreSetting = 4;
            public const int AdjustDspBufferSize = 5;

            // Build Method: Addressables
            public const int IncludeAllAddressableGroups = 20;
            public const int OnlyIncludeAddressableGroups = 21;
            public const int ExcludeAddressableGroups = 22;
            public const int BuildLocalAddressableData = 23;
            public const int CleanAddressableData = 24;

            // Build Method: packaging
            public const int DeleteDoNotShipFolders = 40;
            public const int CopyBuildResultToDirectory = 41;
            public const int CompressBuildFolderToZip = 42;
            public const int CompressMacBuildWithShellScript = 43;
            public const int SignEntitlementWithShellScript = 44;
            public const int MacNotarizeAppWithShellScript = 45;

            // Build Method: upload
            public const int SteamUploadByShellScript = 60;
            public const int SteamUploadByBatch = 61;

            // Data Provider (appears in the shared "Wiseye Studio/Data Provider/"
            // menu after core's providers)
            public const int AddressableGroupsListSo = 20;
            public const int AddressableGroupsCompositeListSo = 21;
        }
    }
}
