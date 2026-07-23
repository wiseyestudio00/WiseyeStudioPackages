using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Sirenix.OdinInspector;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = "Build Method - Copy Build Result To Directory",
        menuName = "WiseyeStudio/Build Pipeline/Build Method/Copy Build Result To Directory")]
    public class BuildMethodCopyBuildResultToDirectoryScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Required]
        [SerializeField] string SteamSDKFolderPath;
        [Required]
        [SerializeField] string SDKSubFolderPath;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            string denstinationDirectoryPath = Path.Combine(SteamSDKFolderPath, SDKSubFolderPath);
            // delete everything in the content folder (recursively)
            DirectoryInfo contentFolder = new(denstinationDirectoryPath);
            if (contentFolder.Exists)
            {
                contentFolder.Delete(true);
                contentFolder.Create();
            }

            DirectoryInfo buildFolder = new(buildingData.BuildFolderPath);
            buildFolder.CopyTo(denstinationDirectoryPath);

            buildingData.AddReportMessage($"Copied build folder to folder: {buildFolder.FullName} -> {denstinationDirectoryPath}");
        }
    }
}