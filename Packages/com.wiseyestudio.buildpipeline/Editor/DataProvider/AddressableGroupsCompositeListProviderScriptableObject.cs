#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace WiseyeStudio.DataProvider
{
    /// <summary>
    /// Can only be used in Editor
    /// </summary>
    [CreateAssetMenu(
        fileName = "Addressable Groups Composite List SO",
        menuName = AssetMenuConstants.DataProvider + "Addressable Groups Composite List SO",
        order = Build.BuildPipelineAssetMenuConstants.Order.AddressableGroupsCompositeListSo)]
    public class AddressableGroupsCompositeListProviderScriptableObject : SerializedScriptableObject, IDataProvider<List<AddressableAssetGroup>>
    {
        [OdinSerialize] private List<IDataProvider<List<AddressableAssetGroup>>> _addressableAssetGroups;
        
        public List<AddressableAssetGroup> GetData()
        {
            List<AddressableAssetGroup> addressableAssetGroups = new();
            foreach (var addressableAssetGroup in _addressableAssetGroups)
            {
                addressableAssetGroups.AddRange(addressableAssetGroup.GetData());
            }
            return addressableAssetGroups;
        }
    }
}
#endif