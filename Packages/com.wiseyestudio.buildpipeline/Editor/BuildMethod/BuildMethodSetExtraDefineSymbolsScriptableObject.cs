using UnityEngine;
using System.Collections.Generic;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using WiseyeStudio.DataProvider;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = "Set Extra Define Symbols",
        menuName = "WiseyeStudio/Build Pipeline/Build Method/Set Extra Define Symbols")]
    public class BuildMethodSetExtraDefineSymbolsScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Required]
        [OdinSerialize] IDataProvider<List<string>> ExtraDefineSymbols;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            var buildOptions = buildingData.BuildOptions;
            buildOptions.extraScriptingDefines = ExtraDefineSymbols.GetData().ToArray();
            buildingData.BuildOptions = buildOptions;
            buildingData.AddReportMessage($"Set Extra Define Symbols: {string.Join(", ", buildOptions.extraScriptingDefines)}");
        }
    }
}
