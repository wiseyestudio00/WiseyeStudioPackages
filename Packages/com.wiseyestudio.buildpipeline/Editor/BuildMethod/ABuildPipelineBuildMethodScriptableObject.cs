using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WiseyeStudio.Build.BuildMethod
{
    public abstract class ABuildPipelineBuildMethodScriptableObject : SerializedScriptableObject
    {
        public abstract void Activate(BuildPipelineBuildingData buildingData);
    }
}
