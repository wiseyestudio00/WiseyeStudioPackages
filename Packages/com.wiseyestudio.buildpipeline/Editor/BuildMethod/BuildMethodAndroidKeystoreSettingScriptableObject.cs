using UnityEngine;
using UnityEditor;

using Sirenix.OdinInspector;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Android Keystore Setting",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Android Keystore Setting",
        order = BuildPipelineAssetMenuConstants.Order.AndroidKeystoreSetting)]
    public class BuildMethodAndroidKeystoreSettingScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Title("Keystore Setting")]
        [SerializeField] private string keyStorePath = string.Empty;
        [SerializeField] private string keyStorePassword = string.Empty;
        [SerializeField] private string keyAliasName = string.Empty;
        [SerializeField] private string keyAliasPassword = string.Empty;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = keyStorePath;
            PlayerSettings.Android.keystorePass = keyStorePassword;
            PlayerSettings.Android.keyaliasName = keyAliasName;
            PlayerSettings.Android.keyaliasPass = keyAliasPassword;

            buildingData.AddReportMessage("Android AAB Setting Build Method Finished");
        }
    }
}