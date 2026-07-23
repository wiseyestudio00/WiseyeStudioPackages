#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace WiseyeStudio.DataProvider
{
    /// <summary>
    /// Can only be used in Editor
    /// </summary>
    [CreateAssetMenu(
        fileName = "Addressable Groups List SO",
        menuName = "WiseyeStudio/Data Provider/Addressable Groups List SO")]
    public class AddressableGroupsListProviderScriptableObject : ScriptableObject, IDataProvider<List<AddressableAssetGroup>>
    {
        [SerializeField] private List<AddressableAssetGroup> _addressableAssetGroup;

        public List<AddressableAssetGroup> GetData()
        {
            return _addressableAssetGroup;
        }
    }
}
#endif