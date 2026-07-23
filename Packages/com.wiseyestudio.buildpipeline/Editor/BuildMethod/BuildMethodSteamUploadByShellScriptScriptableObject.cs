using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

using Sirenix.OdinInspector;
using Newtonsoft.Json;

using WiseyeStudio.Build.Steam;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Steam Upload by Shell Script",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Steam Upload by Shell Script",
        order = BuildPipelineAssetMenuConstants.Order.SteamUploadByShellScript)]
    public class BuildMethodSteamUploadByShellScriptScriptableObject : ABuildPipelineBuildMethodExecuteShellScriptScriptableObject
    {
        [Title("Arguments")]
        [FormerlySerializedAs("SteamSDKPath")]
        [Required, SerializeField] private string steamSdkPath;
        [FormerlySerializedAs("SteamCMDSubPath")]
        [Required, SerializeField] private string steamCmdSubPath;
        [FormerlySerializedAs("SteamAppBuildVDFSubPath")]
        [Required, SerializeField] private string steamAppBuildVdfSubPath;
        [FormerlySerializedAs("SteamAccountJsonPath")]
        [Required, SerializeField] private string steamAccountJsonPath;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var absoluteShellScriptPath = Path.Combine(Application.dataPath, shellScriptPath);
            var absoluteSteamAccountJsonPath = Path.Combine(Application.dataPath, steamAccountJsonPath);

            var accountData = JsonConvert.DeserializeObject<SteamAccountData>(File.ReadAllText(absoluteSteamAccountJsonPath));
            if (ChmodOnFile(absoluteShellScriptPath, "+x"))
            {
                buildingData.AddReportMessage($"chmod +x {absoluteShellScriptPath}: success");
            }
            else
            {
                buildingData.AddReportMessage($"chmod +x {absoluteShellScriptPath}: failed");
            }

            var absoluteSteamCmdPath = Path.Combine(steamSdkPath, steamCmdSubPath);
            var absoluteSteamAppBuildVdfPath = Path.Combine(steamSdkPath, steamAppBuildVdfSubPath);

            ExecuteShellScript(absoluteShellScriptPath,
                newLine =>
                {
                    buildingData.AddReportMessage("Upload to Steam with shell: " + newLine);
                },
                absoluteSteamCmdPath,
                accountData.AccountName,
                accountData.Password,
                absoluteSteamAppBuildVdfPath,
                $"Build {buildingData.BuildName} of Version {buildingData.ResultReport.versionCode}");

            buildingData.AddReportMessage($"Uploaded to Steam: {absoluteShellScriptPath}");
        }
    }
}