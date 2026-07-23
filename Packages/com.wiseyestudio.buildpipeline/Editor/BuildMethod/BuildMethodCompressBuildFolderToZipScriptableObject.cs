using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Compress Build Folder to Zip",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Compress Build Folder to Zip",
        order = BuildPipelineAssetMenuConstants.Order.CompressBuildFolderToZip)]
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