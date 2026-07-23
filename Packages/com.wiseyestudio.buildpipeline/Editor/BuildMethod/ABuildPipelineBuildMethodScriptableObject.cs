using Sirenix.OdinInspector;

namespace WiseyeStudio.Build.BuildMethod
{
    public abstract class ABuildPipelineBuildMethodScriptableObject : SerializedScriptableObject
    {
        public abstract void Activate(BuildPipelineBuildingData buildingData);
    }
}
