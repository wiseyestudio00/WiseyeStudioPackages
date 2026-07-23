using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace WiseyeStudio.DataProvider
{
    [Serializable]
    public class SerializableStringListProvider : IDataProvider<List<string>>
    {
        [FormerlySerializedAs("Strings")]
        [Required, SerializeField] private List<string> strings = new();

        public List<string> GetData()
        {
            return strings;
        }
    }
}