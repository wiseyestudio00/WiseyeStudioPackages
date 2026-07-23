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
        fileName = "Build Method - Only Include Addressable Groups",
        menuName = "WiseyeStudio/Build Pipeline/Build Method/Only Include Addressable Groups",
        order = 5)]
    public class BuildMethodAddressableGroupsIncludeScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Title("Included Addressable Groups")]
        [Tooltip("Only these groups will be included in final build.")]
        [Required]
        [OdinSerialize] IDataProvider<List<AddressableAssetGroup>> IncludedAddressableGroups;

        [Button]
        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var groupNames = IncludedAddressableGroups.GetData().Select(group => group.name).ToHashSet();
            foreach (var group in AddressableAssetSettingsDefaultObject.Settings.groups)
            {
                if (group.HasSchema<BundledAssetGroupSchema>())
                {
                    group.GetSchema<BundledAssetGroupSchema>().IncludeInBuild = groupNames.Contains(group.name);

                }
            }
        }
    }
}

