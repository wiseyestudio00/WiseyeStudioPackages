using UnityEngine;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace WiseyeStudio.DataProvider
{
    [Serializable]
    public class SerializableStringListProvider : IDataProvider<List<string>>
    {
        [Required]
        [SerializeField] List<string> Strings = new();

        public List<string> GetData()
        {
            return Strings;
        }
    }
}