using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Sirenix.OdinInspector;
using UnityEditor;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(fileName = "Set Version Code", menuName = "WiseyeStudio/Build Pipeline/Build Method/Set Version Code", order = 2)]
    public class BuildMethodSetVersionCodeScriptableObject : ABuildPipelineSetVersionCodeMethodscriptableObject
    {
        [Title("Version Code")]


        public override void Activate(BuildPipelineBuildingData buildingData)
        {

        }
    }
}