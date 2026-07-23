using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WiseyeStudio.DataProvider
{
    [CreateAssetMenu(
        fileName = "String SO",
        menuName = "WiseyeStudio/Data Provider/String SO",
        order = 0)]
    public class StringScriptableObject : ScriptableObject, IDataProvider<string>
    {
        [SerializeField] private string _string;

        public string GetData()
        {
            return _string;
        }
    }
}
