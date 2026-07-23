using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

using Sirenix.OdinInspector;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(
        fileName = BuildPipelineAssetMenuConstants.BuildMethodFilePrefix + "Steam Upload by Batch",
        menuName = BuildPipelineAssetMenuConstants.BuildMethod + "Steam Upload by Batch",
        order = BuildPipelineAssetMenuConstants.Order.SteamUploadByBatch)]
    public class BuildMethodSteamUploadWithBatchScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Title("Batch")]
        [FormerlySerializedAs("UploadBatchScriptPath")]
        [Required, SerializeField] private string uploadBatchScriptPath;

        [Title("Content Folder")]
        [FormerlySerializedAs("SteamContentFolderPath")]
        [Required, SerializeField] private string steamContentFolderPath;

        [Title("Arguments")]
        [FormerlySerializedAs("SteamCMDPath")]
        [Required, SerializeField] private string steamCmdPath;
        [FormerlySerializedAs("SteamAccountName")]
        [Required, SerializeField] private string steamAccountName;
        [FormerlySerializedAs("SteamAccountPassword")]
        [Required, SerializeField] private string steamAccountPassword;
        [FormerlySerializedAs("SteamGuardCode")]
        [Required, SerializeField] private string steamGuardCode;
        [FormerlySerializedAs("SteamAppBuildVDFPath")]
        [Required, SerializeField] private string steamAppBuildVdfPath;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = Path.Combine(Application.dataPath, uploadBatchScriptPath),
                Arguments = $"\"{steamCmdPath}\" \"{steamAccountName}\" \"{steamAccountPassword}\" \"{steamGuardCode}\" \"{steamAppBuildVdfPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            buildingData.AddReportMessage($"Steam Upload: {startInfo.FileName} {startInfo.Arguments}");
            var process = Process.Start(startInfo);
            while (process.StandardOutput.EndOfStream == false)
            {
                var newLine = process.StandardOutput.ReadLine();
                buildingData.AddReportMessage($"Steam Upload Log: {newLine}");
            }
            process.WaitForExit();
        }
    }
}
