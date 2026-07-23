using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WiseyeStudio.Build.BuildMethod;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(fileName = "Build Method - Adjust DSP Buffer Size", menuName = "WiseyeStudio/Build Pipeline/Build Method/Adjust DSP Buffer Size")]
    public class BuildMethodAdjustDSPBufferSizeScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Required]
        [ValueDropdown("GetDSPBufferSizes")]
        [SerializeField] int DSPBufferSize = 1024;

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
            audioConfiguration.dspBufferSize = DSPBufferSize;
            AudioSettings.Reset(audioConfiguration);
        }
    }
}