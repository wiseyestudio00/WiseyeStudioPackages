using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using UnityEngine;

using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(fileName = "Build Method - Mac Sign Entitlement With Shell Script", menuName = "WiseyeStudio/Build Pipeline/Build Method/Sign Entitlement With Shell Script", order = 4)]
    public class BuildMethodSignEntitlementWithShellScriptScriptableObject : ABuildPipelineBuildMethodExecuteShellScriptScriptableObject
    {
        [Title("Arguments")]
        [Required]
        [SerializeField] TextAsset EntitlementTextAsset;
        [Required]
        [SerializeField] string SigningIdentity;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            string entitlementFolderPath = Path.Combine(buildingData.BuildFolderPath, "Entitlements");
            if (!Directory.Exists(entitlementFolderPath))
            {
                Directory.CreateDirectory(entitlementFolderPath);
            }

            string entitlementFilePath = Path.Combine(entitlementFolderPath, "LilyFantasia.entitlements");
            File.WriteAllText(entitlementFilePath, EntitlementTextAsset.text);
            buildingData.AddReportMessage($"Created Entitlements: {entitlementFilePath}");

            string absoluteShellScriptPath = Path.Combine(Application.dataPath, ShellScriptPath);

            if(ChmodOnFile(absoluteShellScriptPath, "+x"))
            {
                buildingData.AddReportMessage($"chmod +x {absoluteShellScriptPath}: success");
            }
            else
            {
                buildingData.AddReportMessage($"chmod +x {absoluteShellScriptPath}: failed");
            }


            buildingData.AddReportMessage($"Start signing entitlements: {absoluteShellScriptPath} {@entitlementFilePath} {@buildingData.BuildOptions.locationPathName} {SigningIdentity}");
            ExecuteShellScript(absoluteShellScriptPath,
                newLine =>
                {
                    buildingData.AddReportMessage("Signing Entitlements: " + newLine);
                },
                entitlementFilePath, buildingData.BuildOptions.locationPathName, SigningIdentity);

            // delete entitlements
            if(File.Exists(entitlementFilePath))
            {
                File.Delete(entitlementFilePath);
            }

            buildingData.AddReportMessage($"Signed Entitlements");
        }
    }
}