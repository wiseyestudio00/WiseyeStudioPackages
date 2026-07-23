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
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Exclude Addressable Groups",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Exclude Addressable Groups",
        order = BuildPipelineAssetMenuConstants.Order.ExcludeAddressableGroups)]
    public class BuildMethodAddressableGroupsExcludeScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Required]
        [Tooltip("These groups will be excluded from the final build. The rest will be included.")]
        [FormerlySerializedAs("ExcludedAddressableGroups")]
        [OdinSerialize] private IDataProvider<List<AddressableAssetGroup>> _excludedAddressableGroups;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var groupNames = _excludedAddressableGroups.GetData().Select(group => group.name).ToHashSet();
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

