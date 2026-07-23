#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.Serialization;

namespace WiseyeStudio.DataProvider
{
    /// <summary>
    /// Can only be used in Editor
    /// </summary>
    [CreateAssetMenu(
        fileName = "Addressable Groups List SO",
        menuName = AssetMenuConstants.DataProvider + "Addressable Groups List SO",
        order = Build.BuildPipelineAssetMenuConstants.Order.AddressableGroupsListSo)]
    public class AddressableGroupsListProviderScriptableObject : ScriptableObject, IDataProvider<List<AddressableAssetGroup>>
    {
        [FormerlySerializedAs("_addressableAssetGroup")]
        [Required, SerializeField] private List<AddressableAssetGroup> addressableAssetGroup;

        public List<AddressableAssetGroup> GetData()
        {
            return addressableAssetGroup;
        }
    }
}
#endif