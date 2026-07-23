using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

using Sirenix.OdinInspector;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Copy Build Result To Directory",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Copy Build Result To Directory",
        order = BuildPipelineAssetMenuConstants.Order.CopyBuildResultToDirectory)]
    public class BuildMethodCopyBuildResultToDirectoryScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Required]
        [FormerlySerializedAs("SteamSDKFolderPath")]
        [SerializeField] private string steamSdkFolderPath;
        [Required]
        [FormerlySerializedAs("SDKSubFolderPath")]
        [SerializeField] private string sdkSubFolderPath;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var destinationDirectoryPath = Path.Combine(steamSdkFolderPath, sdkSubFolderPath);
            // delete everything in the content folder (recursively)
            DirectoryInfo contentFolder = new(destinationDirectoryPath);
            if (contentFolder.Exists)
            {
                contentFolder.Delete(true);
                contentFolder.Create();
            }

            DirectoryInfo buildFolder = new(buildingData.BuildFolderPath);
            buildFolder.CopyTo(destinationDirectoryPath);

            buildingData.AddReportMessage($"Copied build folder to folder: {buildFolder.FullName} -> {destinationDirectoryPath}");
        }
    }
}