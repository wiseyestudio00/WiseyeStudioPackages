using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WiseyeStudio.DataProvider
{
    [CreateAssetMenu(
        fileName = "String Platform SO",
        menuName = "WiseyeStudio/Data Provider/String by Platform SO",
        order = 1)]
    public class StringByPlatformScriptableObject : ScriptableObject, IDataProvider<string>
    {
        [Tooltip("This string will be used if the platform string does not exist/empty")]
        [Required]
        [SerializeField] string DefaultString;

        [Required]
        [SerializeField] string MacString;
        [Required]
        [SerializeField] string WindowsString;

        public string GetData()
        {
            // return the playform specific string if it exists (is not empty/null)
            // otherwise return the default string
            return string.IsNullOrEmpty(GetPlatformString()) ? DefaultString : GetPlatformString();
        }

        private string GetPlatformString()
        {
            // use switch statement to avoid multiple if-else statements
            return Application.platform switch
            {
                RuntimePlatform.OSXPlayer => MacString,
                RuntimePlatform.OSXEditor => MacString,
                RuntimePlatform.WindowsPlayer => WindowsString,
                RuntimePlatform.WindowsEditor => WindowsString,
                _ => DefaultString,
            };
        }
    }
}