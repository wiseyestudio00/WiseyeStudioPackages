using UnityEngine;

namespace WiseyeStudio.DataProvider
{
    [CreateAssetMenu(
        fileName = "String SO",
        menuName = AssetMenuConstants.DataProvider + "String SO",
        order = AssetMenuConstants.Order.StringSo)]
    public class StringScriptableObject : ScriptableObject, IDataProvider<string>
    {
        [SerializeField] private string _string;

        public string GetData()
        {
            return _string;
        }
    }
}
