using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = "Build Method - Delete DoNotShip folders",
        menuName = "WiseyeStudio/Build Pipeline/Build Method/Delete DoNotShip Folders",
        order = 2)]
    public class BuildMethodDeleteDoNotShipFoldersScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            foreach(string folderPath in Directory.GetDirectories(buildingData.BuildFolderPath, "*_DoNotShip", SearchOption.TopDirectoryOnly))
            {
                Directory.Delete(folderPath, true);
                buildingData.AddReportMessage($"Deleted DoNotShip Folder: {folderPath}");
            }
        }
    }
}
