using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.Serialization;
using WiseyeStudio.DataProvider;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Only Include Addressable Groups",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Only Include Addressable Groups",
        order = BuildPipelineAssetMenuConstants.Order.OnlyIncludeAddressableGroups)]
    public class BuildMethodAddressableGroupsIncludeScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Title("Included Addressable Groups")]
        [Tooltip("Only these groups will be included in final build.")]
        [Required]
        [FormerlySerializedAs("IncludedAddressableGroups")]
        [OdinSerialize] private IDataProvider<List<AddressableAssetGroup>> _includedAddressableGroups;

        [Button]
        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var groupNames = _includedAddressableGroups.GetData().Select(group => group.name).ToHashSet();
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

