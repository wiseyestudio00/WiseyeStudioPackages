using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Include All Addressable Groups",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Include All Addressable Groups",
        order = BuildPipelineAssetMenuConstants.Order.IncludeAllAddressableGroups)]
    public class BuildMethodAddressableGroupsIncludeAllScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            foreach (var group in AddressableAssetSettingsDefaultObject.Settings.groups)
            {
                if (group.HasSchema<BundledAssetGroupSchema>())
                {
                    group.GetSchema<BundledAssetGroupSchema>().IncludeInBuild = true;
                }
            }
        }
    }
}

