using System.IO;
using UnityEngine;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Compress Mac Build With Shell Script",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Compress Mac Build With Shell Script",
        order = BuildPipelineAssetMenuConstants.Order.CompressMacBuildWithShellScript)]
    public class BuildMethodCompressMacBuildWithShellScriptScriptableObject : ABuildPipelineBuildMethodExecuteShellScriptScriptableObject
    {
        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var builtAppPath = buildingData.BuildOptions.locationPathName;
            var absoluteShellScriptPath = Path.Combine(Application.dataPath, shellScriptPath);

            if(ChmodOnFile(absoluteShellScriptPath, "+x"))
            {
                buildingData.AddReportMessage($"chmod +x {absoluteShellScriptPath}: success");
            }
            else
            {
                buildingData.AddReportMessage($"chmod +x {absoluteShellScriptPath}: failed");
            }

            ExecuteShellScript(absoluteShellScriptPath,
                newLine =>
                {
                    buildingData.AddReportMessage("Compress with shell: " + newLine);
                },
                builtAppPath, buildingData.TargetZippedBuildFilePath);

            buildingData.ResultReport.builtResultPath = buildingData.TargetZippedBuildFilePath;
            buildingData.AddReportMessage($"Compressed Mac Build: {absoluteShellScriptPath} {@builtAppPath} {@buildingData.TargetZippedBuildFilePath}");
        }
    }
}