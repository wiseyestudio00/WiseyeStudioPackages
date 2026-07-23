using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(fileName = "Build Method - Switch Build Target", menuName = "WiseyeStudio/Build Pipeline/Build Method/Switch Build Target", order = 0)]
    public class BuildMethodSwitchBuildTargetScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [SerializeField] BuildTargetGroup TargetGroup = BuildTargetGroup.Standalone;
        [SerializeField] BuildTarget Target;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var buildPlayerOptions = buildingData.BuildOptions;
            buildPlayerOptions.targetGroup = TargetGroup;
            buildPlayerOptions.target = Target;
            buildingData.BuildOptions = buildPlayerOptions;

            // switch build target here.
            // NOTE: switching build target in batch mode is not supported
            // it can only be done in command line arguments
            // https://docs.unity.cn/2017.4/Documentation/ScriptReference/EditorUserBuildSettings.SwitchActiveBuildTarget.html
            if (!Application.isBatchMode)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(TargetGroup, Target);
            }   

            buildingData.AddReportMessage($"Switched Build Target: {TargetGroup} {Target}");
        }
    }
}