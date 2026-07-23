using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WiseyeStudio.DataProvider
{
    [CreateAssetMenu(
        fileName = "String List SO",
        menuName = "WiseyeStudio/Data Provider/String List SO",
        order = 1)]
    public class StringListScriptableObject : ScriptableObject, IDataProvider<List<string>>
    {
        [SerializeField] private List<string> stringList = new();

        public List<string> GetData()
        {
            return stringList;
        }
    }
}
