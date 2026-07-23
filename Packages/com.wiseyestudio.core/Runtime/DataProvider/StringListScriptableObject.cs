using System.Collections.Generic;
using UnityEngine;

namespace WiseyeStudio.DataProvider
{
    [CreateAssetMenu(
        fileName = "String List SO",
        menuName = AssetMenuConstants.DataProvider + "String List SO",
        order = AssetMenuConstants.Order.StringListSo)]
    public class StringListScriptableObject : ScriptableObject, IDataProvider<List<string>>
    {
        [SerializeField] private List<string> stringList = new();

        public List<string> GetData()
        {
            return stringList;
        }
    }
}
