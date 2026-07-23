using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(fileName = "Build Method - Compress Build Folder to Zip", menuName = "WiseyeStudio/Build Pipeline/Build Method/Compress Build Folder to Zip", order = 0)]
    public class BuildMethodCompressBuildFolderToZipScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            if (File.Exists(buildingData.TargetZippedBuildFilePath))
            {
                File.Delete(buildingData.TargetZippedBuildFilePath);
            }
            ZipFile.CreateFromDirectory(buildingData.BuildFolderPath, buildingData.TargetZippedBuildFilePath);

            buildingData.ResultReport.builtResultPath = buildingData.TargetZippedBuildFilePath;
            buildingData.AddReportMessage($"Compressed Build Output: {buildingData.TargetZippedBuildFilePath}");
        }
    }
}