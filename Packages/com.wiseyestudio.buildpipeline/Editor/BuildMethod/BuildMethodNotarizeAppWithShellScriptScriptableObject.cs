using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

using Sirenix.OdinInspector;
using Newtonsoft.Json;

using WiseyeStudio.Build.Apple;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Mac Notarize App With Shell Script",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Mac Notarize App With Shell Script",
        order = BuildPipelineAssetMenuConstants.Order.MacNotarizeAppWithShellScript)]
    public class BuildMethodNotarizeAppWithShellScriptScriptableObject : ABuildPipelineBuildMethodExecuteShellScriptScriptableObject
    {
        [Title("Shell Arguments")]
        [FormerlySerializedAs("AppleConnectAccountJsonPath")]
        [Required, SerializeField] private string appleConnectAccountJsonPath;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var builtAppPath = buildingData.BuildOptions.locationPathName;
            var zippedAppPath = Path.Combine(buildingData.BuildBaseFolderPath, $"{buildingData.BuildName}.zip");
            
            var absoluteShellScriptPath = Path.Combine(Application.dataPath, shellScriptPath);
            var absoluteAppleConnectAccountJsonPath = Path.Combine(Application.dataPath, appleConnectAccountJsonPath);
            var accountData = JsonConvert.DeserializeObject<AppleConnectAccountData>(File.ReadAllText(absoluteAppleConnectAccountJsonPath));

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
