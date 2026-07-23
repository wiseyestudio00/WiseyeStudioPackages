using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using WiseyeStudio.DataProvider;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Set Scene List",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Set Scene List",
        order = BuildPipelineAssetMenuConstants.Order.SetSceneList)]
    public class BuildMethodSetSceneListScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Required]
        [FormerlySerializedAs("SceneNamesList")]
        [OdinSerialize] private IDataProvider<List<string>> _sceneNamesList;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var buildPlayerOptions = buildingData.BuildOptions;
            buildPlayerOptions.scenes = _sceneNamesList.GetData().ToArray();
            buildingData.BuildOptions = buildPlayerOptions;

            buildingData.AddReportMessage($"Set Scene List: {string.Join(", ", _sceneNamesList.GetData())}");
        }
    }
}