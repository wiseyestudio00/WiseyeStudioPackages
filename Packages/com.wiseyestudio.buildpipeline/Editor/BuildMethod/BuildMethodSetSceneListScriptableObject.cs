using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using WiseyeStudio.DataProvider;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(fileName = "Build Method - Set Scene List", menuName = "WiseyeStudio/Build Pipeline/Build Method/Set Scene List", order = 1)]
    public class BuildMethodSetSceneListScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Required]
        [OdinSerialize] IDataProvider<List<string>> SceneNamesList;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var buildPlayerOptions = buildingData.BuildOptions;
            buildPlayerOptions.scenes = SceneNamesList.GetData().ToArray();
            buildingData.BuildOptions = buildPlayerOptions;

            buildingData.AddReportMessage($"Set Scene List: {string.Join(", ", SceneNamesList.GetData())}");
        }
    }
}