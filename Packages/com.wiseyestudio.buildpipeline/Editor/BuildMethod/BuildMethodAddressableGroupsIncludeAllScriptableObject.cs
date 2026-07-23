using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = "Build Method - Include All Addressable Groups",
        menuName = "WiseyeStudio/Build Pipeline/Build Method/Include All Addressable Groups",
        order = 6)]
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

