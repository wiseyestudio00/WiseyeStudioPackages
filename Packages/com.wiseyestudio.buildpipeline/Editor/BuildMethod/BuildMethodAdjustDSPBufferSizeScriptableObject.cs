using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Adjust DSP Buffer Size",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Adjust DSP Buffer Size",
        order = BuildPipelineAssetMenuConstants.Order.AdjustDspBufferSize)]
    public class BuildMethodAdjustDSPBufferSizeScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Required]
        [ValueDropdown(nameof(GetDSPBufferSizes))]
        [FormerlySerializedAs("DSPBufferSize")]
        [SerializeField] private int dspBufferSize = 1024;

        private IEnumerable GetDSPBufferSizes()
        {
            yield return 256;
            yield return 512;
            yield return 1024;
            yield return 2048;
        }

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            AudioConfiguration audioConfiguration = AudioSettings.GetConfiguration();
            audioConfiguration.dspBufferSize = dspBufferSize;
            AudioSettings.Reset(audioConfiguration);
        }
    }
}