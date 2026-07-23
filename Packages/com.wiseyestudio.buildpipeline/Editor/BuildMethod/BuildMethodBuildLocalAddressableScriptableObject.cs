using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Build Local Addressable Data",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Build Local Addressable Data",
        order = BuildPipelineAssetMenuConstants.Order.BuildLocalAddressableData)]
    public class BuildMethodBuildLocalAddressableScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                buildingData.AddReportMessage("AddressableAssetSettings not found.");
            }
            
            // Build/Clear Build Cache/All
            // Clear all cached data used by Addressables builds
            AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder.ClearCachedData();
            BuildCache.PurgeCache(false); // Clears Unity’s build cache (optional)
            
            AddressableAssetSettings.BuildPlayerContent();
            buildingData.AddReportMessage("Built Local Addressable Data");
        }
    }
}
