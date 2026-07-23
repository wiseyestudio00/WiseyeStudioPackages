using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using WiseyeStudio.DataProvider;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = "Build Method - Exclude Addressable Groups But Include The Rest",
        menuName = "WiseyeStudio/Build Pipeline/Build Method/Exclude Addressable Groups",
        order = 6)]
    public class BuildMethodAddressableGroupsExcludeScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Required]
        [Tooltip("These groups will be excluded from the final build. The rest will be included.")]
        [OdinSerialize] IDataProvider<List<AddressableAssetGroup>> ExcludedAddressableGroups;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var groupNames = ExcludedAddressableGroups.GetData().Select(group => group.name).ToHashSet();
            foreach (var group in AddressableAssetSettingsDefaultObject.Settings.groups)
            {
                if (group.HasSchema<BundledAssetGroupSchema>())
                {
                    group.GetSchema<BundledAssetGroupSchema>().IncludeInBuild = (groupNames.Contains(group.name) == false);
                }
            }
        }
    }
}

