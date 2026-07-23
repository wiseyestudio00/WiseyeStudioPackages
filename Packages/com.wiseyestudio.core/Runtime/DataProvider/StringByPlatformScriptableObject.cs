using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace WiseyeStudio.DataProvider
{
    [CreateAssetMenu(
        fileName = "String by Platform SO",
        menuName = AssetMenuConstants.DataProvider + "String by Platform SO",
        order = AssetMenuConstants.Order.StringByPlatformSo)]
    public class StringByPlatformScriptableObject : ScriptableObject, IDataProvider<string>
    {
        [Tooltip("This string will be used if the platform string does not exist/empty")]
        [FormerlySerializedAs("DefaultString")]
        [Required, SerializeField] private string defaultString;

        [FormerlySerializedAs("WindowsString")]
        [Required, SerializeField] private string windowsString;
        [FormerlySerializedAs("MacString")]
        [Required, SerializeField] private string macString;

        public string GetData()
        {
            // return the platform specific string if it exists (is not empty/null)
            // otherwise return the default string
            return string.IsNullOrEmpty(GetPlatformString()) ? defaultString : GetPlatformString();
        }

        private string GetPlatformString()
        {
            return Application.platform switch
            {
                RuntimePlatform.OSXPlayer => macString,
                RuntimePlatform.OSXEditor => macString,
                RuntimePlatform.WindowsPlayer => windowsString,
                RuntimePlatform.WindowsEditor => windowsString,
                _ => defaultString,
            };
        }
    }
}