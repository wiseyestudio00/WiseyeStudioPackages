using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace WiseyeStudio.Build.BuildMethod
{
    [CreateAssetMenu(fileName = "Build Method - Steam Upload by Batch", menuName = "WiseyeStudio/Build Pipeline/Build Method/Steam Upload by Batch")]
    public class BuildMethodSteamUploadWithBatchScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Title("Batch")]
        [Required]
        [SerializeField] string UploadBatchScriptPath;

        [Title("Content Folder")]
        [Required]
        [SerializeField] string SteamContentFolderPath;

        [Title("Arguments")]
        [Required]
        [SerializeField] string SteamCMDPath;
        [Required]
        [SerializeField] string SteamAccountName;
        [Required]
        [SerializeField] string SteamAccountPassword;
        [Required]
        [SerializeField] string SteamGuardCode;
        [Required]
        [SerializeField] string SteamAppBuildVDFPath;

        public override void Activate(BuildPipelineBuildingData buildingData)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = Path.Combine(Application.dataPath, UploadBatchScriptPath),
                Arguments = $"\"{SteamCMDPath}\" \"{SteamAccountName}\" \"{SteamAccountPassword}\" \"{SteamGuardCode}\" \"{SteamAppBuildVDFPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            buildingData.AddReportMessage($"Steam Upload: {startInfo.FileName} {startInfo.Arguments}");
            Process process = Process.Start(startInfo);
            while (!process.StandardOutput.EndOfStream)
            {
                string newLine = process.StandardOutput.ReadLine();
                buildingData.AddReportMessage($"Steam Upload Log: {newLine}");
            }
            process.WaitForExit();
        }
    }
}
