using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Sirenix.OdinInspector;
using Newtonsoft.Json;

using WiseyeStudio.Build.Apple;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = "Build Method - Mac Notarize App With Shell Script",
        menuName = "WiseyeStudio/Build Pipeline/Build Method/Mac Notarize App With Shell Script",
        order = 4)]
    public class BuildMethodNotarizeAppWithShellScriptScriptableObject : ABuildPipelineBuildMethodExecuteShellScriptScriptableObject
    {
        [Title("Shell Arugments")]
        [Required]
        [SerializeField] string AppleConnectAccountJsonPath;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            string builtAppPath = buildingData.BuildOptions.locationPathName;
            string zippedAppPath = Path.Combine(buildingData.BuildBaseFolderPath, $"{buildingData.BuildName}.zip");
            
            string absoluteShellScriptPath = Path.Combine(Application.dataPath, ShellScriptPath);
            string appleConnectAccountJsonPath = Path.Combine(Application.dataPath, AppleConnectAccountJsonPath);
            AppleConnectAccountData accountData = JsonConvert.DeserializeObject<AppleConnectAccountData>(File.ReadAllText(appleConnectAccountJsonPath));

            if (ChmodOnFile(absoluteShellScriptPath, "+x"))
            {
                buildingData.AddReportMessage($"chmod +x {absoluteShellScriptPath}: success");
            }
            else
            {
                buildingData.AddReportMessage($"chmod +x {absoluteShellScriptPath}: failed");
            }

            buildingData.AddReportMessage($"Start Notarizing App: {absoluteShellScriptPath} {@builtAppPath} {@zippedAppPath}");
            ExecuteShellScript(absoluteShellScriptPath,
                newLine =>
                {
                    buildingData.AddReportMessage("Notarizing App: " + newLine);
                },
                builtAppPath, zippedAppPath, accountData.AppleID, accountData.TeamID, accountData.Password);
            buildingData.AddReportMessage($"Notarized App: {absoluteShellScriptPath} {@builtAppPath} {@zippedAppPath}");
        }
    }
}
