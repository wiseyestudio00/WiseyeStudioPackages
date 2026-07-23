using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using WiseyeStudio.DataProvider;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Set Extra Define Symbols",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Set Extra Define Symbols",
        order = BuildPipelineAssetMenuConstants.Order.SetExtraDefineSymbols)]
    public class BuildMethodSetExtraDefineSymbolsScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Required]
        [FormerlySerializedAs("ExtraDefineSymbols")]
        [OdinSerialize] private IDataProvider<List<string>> _extraDefineSymbols;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var buildOptions = buildingData.BuildOptions;
            buildOptions.extraScriptingDefines = _extraDefineSymbols.GetData().ToArray();
            buildingData.BuildOptions = buildOptions;
            buildingData.AddReportMessage($"Set Extra Define Symbols: {string.Join(", ", buildOptions.extraScriptingDefines)}");
        }
    }
}
