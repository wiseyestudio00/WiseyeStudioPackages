using UnityEngine;
using UnityEngine.Serialization;

namespace WiseyeStudio.DataProvider
{
    [CreateAssetMenu(
        fileName = "String SO",
        menuName = AssetMenuConstants.DataProvider + "String SO",
        order = AssetMenuConstants.Order.StringSo)]
    public class StringScriptableObject : ScriptableObject, IDataProvider<string>
    {
        [FormerlySerializedAs("_string")]
        [SerializeField] private string value;

        public string GetData()
        {
            return value;
        }
    }
}
