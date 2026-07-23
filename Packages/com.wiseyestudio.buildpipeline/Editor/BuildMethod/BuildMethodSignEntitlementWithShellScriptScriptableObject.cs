using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

using Sirenix.OdinInspector;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Sign Entitlement With Shell Script",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Sign Entitlement With Shell Script",
        order = BuildPipelineAssetMenuConstants.Order.SignEntitlementWithShellScript)]
    public class BuildMethodSignEntitlementWithShellScriptScriptableObject : ABuildPipelineBuildMethodExecuteShellScriptScriptableObject
    {
        [Title("Arguments")]
        [Required]
        [FormerlySerializedAs("EntitlementTextAsset")]
        [SerializeField] private TextAsset entitlementTextAsset;
        [Required]
        [FormerlySerializedAs("SigningIdentity")]
        [SerializeField] private string signingIdentity;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            string entitlementFolderPath = Path.Combine(buildingData.BuildFolderPath, "Entitlements");
            if (!Directory.Exists(entitlementFolderPath))
            {
                Directory.CreateDirectory(entitlementFolderPath);
            }

            string entitlementFilePath = Path.Combine(entitlementFolderPath, "LilyFantasia.entitlements");
            File.WriteAllText(entitlementFilePath, entitlementTextAsset.text);
            buildingData.AddReportMessage($"Created Entitlements: {entitlementFilePath}");

            string absoluteShellScriptPath = Path.Combine(Application.dataPath, shellScriptPath);

            if(ChmodOnFile(absoluteShellScriptPath, "+x"))
            {
                buildingData.AddReportMessage($"chmod +x {absoluteShellScriptPath}: success");
            }
            else
            {
                buildingData.AddReportMessage($"chmod +x {absoluteShellScriptPath}: failed");
            }


            buildingData.AddReportMessage($"Start signing entitlements: {absoluteShellScriptPath} {@entitlementFilePath} {@buildingData.BuildOptions.locationPathName} {signingIdentity}");
            ExecuteShellScript(absoluteShellScriptPath,
                newLine =>
                {
                    buildingData.AddReportMessage("Signing Entitlements: " + newLine);
                },
                entitlementFilePath, buildingData.BuildOptions.locationPathName, signingIdentity);

            // delete entitlements
            if(File.Exists(entitlementFilePath))
            {
                File.Delete(entitlementFilePath);
            }

            buildingData.AddReportMessage($"Signed Entitlements");
        }
    }
}