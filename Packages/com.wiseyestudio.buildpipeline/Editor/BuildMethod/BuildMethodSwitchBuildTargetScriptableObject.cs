using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Switch Build Target",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Switch Build Target",
        order = BuildPipelineAssetMenuConstants.Order.SwitchBuildTarget)]
    public class BuildMethodSwitchBuildTargetScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [FormerlySerializedAs("TargetGroup")]
        [SerializeField] private BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;
        [FormerlySerializedAs("Target")]
        [SerializeField] private BuildTarget target;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var buildPlayerOptions = buildingData.BuildOptions;
            buildPlayerOptions.targetGroup = targetGroup;
            buildPlayerOptions.target = target;
            buildingData.BuildOptions = buildPlayerOptions;

            // switch build target here.
            // NOTE: switching build target in batch mode is not supported
            // it can only be done in command line arguments
            // https://docs.unity.cn/2017.4/Documentation/ScriptReference/EditorUserBuildSettings.SwitchActiveBuildTarget.html
            if (!Application.isBatchMode)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);
            }

            buildingData.AddReportMessage($"Switched Build Target: {targetGroup} {target}");
        }
    }
}