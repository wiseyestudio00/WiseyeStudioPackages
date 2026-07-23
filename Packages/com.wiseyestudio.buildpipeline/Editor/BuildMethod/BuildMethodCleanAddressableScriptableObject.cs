using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = "Build Method - Clean Local Addressable Data",
        menuName = "WiseyeStudio/Build Pipeline/Build Method/Clean Addressable Data",
        order = 2)]
    public class BuildMethodCleanAddressableScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            AddressableAssetSettings.CleanPlayerContent();

            // Get the Addressable Asset Settings
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings == null)
            {
                Debug.LogError("Addressable Asset Settings not found.");
            }

            // Get the profile settings
            var profileSettings = settings.profileSettings;

            // Get the active profile ID
            var activeProfileId = settings.activeProfileId;

            // Get the LocalBuildPath value
            string localAssetPath = profileSettings.GetValueByName(activeProfileId, AddressableAssetSettings.kLocalBuildPath);
            localAssetPath = settings.profileSettings.EvaluateString(activeProfileId, localAssetPath);

            if (!Directory.Exists(localAssetPath))
            {
                return;
            }

            if (AssetDatabase.DeleteAsset(localAssetPath.Replace(Application.dataPath, "Assets")))
            {
                buildingData.AddReportMessage($"Cleaned Addressable Data: {@localAssetPath}");
            }
            else
            {
                buildingData.AddReportMessage($"Failed to Clean Addressable Data: {@localAssetPath}");
            }
        }
    }
}
