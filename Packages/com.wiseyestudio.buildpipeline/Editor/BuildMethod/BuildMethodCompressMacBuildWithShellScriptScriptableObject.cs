using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(fileName = "Build Method - Compress Mac Build With Shell Script", menuName = "WiseyeStudio/Build Pipeline/Build Method/Compress Mac Build With Shell Script", order = 4)]
    public class BuildMethodCompressMacBuildWithShellScriptScriptableObject : ABuildPipelineBuildMethodExecuteShellScriptScriptableObject
    {
        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            string builtAppPath = buildingData.BuildOptions.locationPathName;
            string absoluteShellScriptPath = Path.Combine(Application.dataPath, ShellScriptPath);

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

            buildingData.ResultReport.BuiltResultPath = buildingData.TargetZippedBuildFilePath;
            buildingData.AddReportMessage($"Compressed Mac Build: {absoluteShellScriptPath} {@builtAppPath} {@buildingData.TargetZippedBuildFilePath}");
        }
    }
}