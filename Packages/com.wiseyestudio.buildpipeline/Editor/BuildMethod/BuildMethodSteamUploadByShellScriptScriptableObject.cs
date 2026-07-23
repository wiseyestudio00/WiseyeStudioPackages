using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Sirenix.OdinInspector;
using Newtonsoft.Json;

using WiseyeStudio.Build.Steam;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = "Build Method - Steam Upload by Shell Script",
        menuName = "WiseyeStudio/Build Pipeline/Build Method/Build Method - Steam Upload by Shell Script",
        order = 0)]
    public class BuildMethodSteamUploadByShellScriptScriptableObject : ABuildPipelineBuildMethodExecuteShellScriptScriptableObject
    {
        [Title("Arguments")]
        [Required]
        [SerializeField] string SteamSDKPath;
        [Required]
        [SerializeField] string SteamCMDSubPath;
        [Required]
        [SerializeField] string SteamAppBuildVDFSubPath;
        [Required]
        [SerializeField] string SteamAccountJsonPath;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var absoluteShellScriptPath = Path.Combine(Application.dataPath, ShellScriptPath);
            var steamAccountJsonPath = Path.Combine(Application.dataPath, SteamAccountJsonPath);

            var accountData = JsonConvert.DeserializeObject<SteamAccountData>(File.ReadAllText(steamAccountJsonPath));
            if (ChmodOnFile(absoluteShellScriptPath, "+x"))
            {
                buildingData.AddReportMessage($"chmod +x {absoluteShellScriptPath}: success");
            }
            else
            {
                buildingData.AddReportMessage($"chmod +x {absoluteShellScriptPath}: failed");
            }

            var absoluteSteamCMDPath = Path.Combine(SteamSDKPath, SteamCMDSubPath);
            var absoluteSteamAppBuildVDFPath = Path.Combine(SteamSDKPath, SteamAppBuildVDFSubPath);

            var description = $"{buildingData.BuildName}: {buildingData.ResultReport.VersionCode}";

            ExecuteShellScript(absoluteShellScriptPath,
                newLine =>
                {
                    buildingData.AddReportMessage("Upload to Steam with shell: " + newLine);
                },
                absoluteSteamCMDPath,
                accountData.AccountName,
                accountData.Password,
                absoluteSteamAppBuildVDFPath,
                $"Build {buildingData.BuildName} of Version {buildingData.ResultReport.VersionCode}");

            buildingData.AddReportMessage($"Uploaded to Steam: {absoluteShellScriptPath}");
        }
    }
}