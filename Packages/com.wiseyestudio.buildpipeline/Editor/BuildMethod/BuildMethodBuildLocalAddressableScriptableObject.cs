using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(fileName = "Build Method - Build Local Addressable Data", menuName = "WiseyeStudio/Build Pipeline/Build Method/Build Local Addressable Data", order = 3)]
    public class BuildMethodBuildLocalAddressableScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
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
