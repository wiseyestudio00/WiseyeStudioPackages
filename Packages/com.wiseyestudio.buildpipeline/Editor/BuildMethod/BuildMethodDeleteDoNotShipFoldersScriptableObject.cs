using System.IO;
using UnityEngine;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Delete DoNotShip Folders",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Delete DoNotShip Folders",
        order = BuildPipelineAssetMenuConstants.Order.DeleteDoNotShipFolders)]
    public class BuildMethodDeleteDoNotShipFoldersScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            foreach (var folderPath in Directory.GetDirectories(buildingData.BuildFolderPath, "*_DoNotShip", SearchOption.TopDirectoryOnly))
            {
                Directory.Delete(folderPath, true);
                buildingData.AddReportMessage($"Deleted DoNotShip Folder: {folderPath}");
            }
        }
    }
}
