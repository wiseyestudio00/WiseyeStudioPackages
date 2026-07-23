using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Sirenix.OdinInspector;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = "Build Method - Android Keystore Setting",
        menuName = "WiseyeStudio/Build Pipeline/Build Method/Android Keystore Setting")]
    public class BuildMethodAndroidKeystoreSettingScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Title("Keystore Setting")]
        [SerializeField] string keyStorePath = string.Empty;
        [SerializeField] string keyStorePassword = string.Empty;
        [SerializeField] string keyAliasName = string.Empty;
        [SerializeField] string keyAliasPassword = string.Empty;

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