using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;

using WiseyeStudio.Build.BuildMethod;

namespace WiseyeStudio.Build
{
    [CreateAssetMenu(
        fileName = "Build Pipeline",
        menuName = "WiseyeStudio/Build Pipeline/Build Pipeline",
        order = 1)]
    public class BuildPipelineScriptableObject : ScriptableObject
    {
        [Title("Build Target")]
        [SerializeField] BuildTargetGroup TargetGroup = BuildTargetGroup.Standalone;
        [SerializeField] BuildTarget Target;

        [Title("Output Setting")]
        [SerializeField] string BuildPipelineFolderPath;
        [SerializeField] string BuildName;
        [SerializeField] string ExecutableName;
        [SerializeField] string Exntesion;

        [Title("Build Setting")]
        [SerializeField] bool DevelopmentBuild;
        [SerializeField] bool DeepProfileSupport;

        [Title("Version Code")]
        [SerializeField] VersionCode VersionCode;

        [Title("Extra Define Symbols")]
        [SerializeField] BuildMethodSetExtraDefineSymbolsScriptableObject SetExtraDefineSymbolsMethod;

        [Title("Set Scenes")]
        [Required]
        [SerializeField] BuildMethodSetSceneListScriptableObject SetSceneListMethod;

        [Title("Pre Build Methods")]
        [Required]
        [SerializeField] List<ABuildPipelineBuildMethodScriptableObject> PrebuildBuildMethods = new();

        [Title("Asset Management")]
        [Required]
        [SerializeField] List<ABuildPipelineBuildMethodScriptableObject> AssetManagementBuildMethods = new();

        [Title("Post Build Methods")]
        [Required]
        [SerializeField] List<ABuildPipelineBuildMethodScriptableObject> PostbuildBuildMethods = new();

        [Title("Compress Build Folder")]
        [Required]
        [SerializeField] ABuildPipelineBuildMethodScriptableObject CompressBuildFolderMethod;

        [Title("Upload")]
        [SerializeField] bool EnableUpload = true;
        [ShowIf(nameof(EnableUpload))]
        [Required]
        [SerializeField] List<ABuildPipelineBuildMethodScriptableObject> UploadMethods = new();

        [Button]
        public BuildPipelineBuildingData Build()
        {
            DateTime startBuildTime = DateTime.Now;
            var buildingData = CreateBuildingData();

            try
            {
                SetBuildTarget(buildingData);
                SetVersionCode(buildingData);
             
                buildingData.AddReportMessage($"=== Start Pre-Building ===");

                if(SetExtraDefineSymbolsMethod != null)
                {
                    SetExtraDefineSymbolsMethod.Activate(buildingData);
                }

                if(SetSceneListMethod != null)
                {
                    SetSceneListMethod.Activate(buildingData);
                }

                foreach (var buildMethod in PrebuildBuildMethods)
                {
                    buildMethod.Activate(buildingData);
                }

                foreach (var buildMethod in AssetManagementBuildMethods)
                {
                    buildMethod.Activate(buildingData);
                }

                buildingData.AddReportMessage($"=== Start Building ===");
                var buildReport = BuildPipeline.BuildPlayer(buildingData.BuildOptions);

                if (buildReport.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                {
                    throw new Exception($"Build Failed: {buildReport.summary.result}");
                }

                buildingData.AddReportMessage($"=== Start Post-Building ===");
                foreach (var buildMethod in PostbuildBuildMethods)
                {
                    buildMethod.Activate(buildingData);
                }

                CompressBuildFolderMethod.Activate(buildingData);

                if (string.IsNullOrEmpty(buildingData.ResultReport.BuiltResultPath))
                {
                    buildingData.ResultReport.BuiltResultPath = buildingData.BuildOptions.locationPathName;
                    buildingData.AddReportMessage($"Built Result Path is not set. Added Built Result Path: {buildingData.ResultReport.BuiltResultPath}");
                }

                if (EnableUpload)
                {
                    foreach (var buildMethod in UploadMethods)
                    {
                        buildMethod.Activate(buildingData);
                    }
                }

                buildingData.ResultReport.Success = true;
                buildingData.ResultReport.BuildTimeInSeconds = (DateTime.Now - startBuildTime).TotalSeconds;
                buildingData.AddReportMessage($"=== Build Finished ===");
            }
            catch (Exception e)
            {
                buildingData.ResultReport.Success = false;
                buildingData.AddReportMessage(e.Message);
                buildingData.AddReportMessage(e.StackTrace);
                buildingData.AddReportMessage($"=== Build Failed ===");
            }

            File.WriteAllText(buildingData.ReportFilePath, buildingData.ResultReport.ToJsonString());
            return buildingData;
        }

        private BuildOptions GetBuildOptions()
        {
            BuildOptions buildOptions = BuildOptions.None;
            buildOptions = DevelopmentBuild ? buildOptions | BuildOptions.Development : buildOptions;
            buildOptions = DeepProfileSupport ? buildOptions | BuildOptions.EnableDeepProfilingSupport : buildOptions;
            return buildOptions;
        }

        private string GetExecutableName()
        {
            return $"{ExecutableName}.{Exntesion}";
        }

        private BuildPipelineBuildingData CreateBuildingData()
        {
            return new BuildPipelineBuildingData(
                BuildPipelineFolderPath,
                BuildName + (DevelopmentBuild ? "_Development" : string.Empty),
                GetExecutableName(),
                GetBuildOptions(),
                EditorApplication.timeSinceStartup);
        }

        private void SetBuildTarget(BuildPipelineBuildingData buildingData)
        {
            BuildPlayerOptions buildPlayerOptions = buildingData.BuildOptions;
            buildPlayerOptions.targetGroup = TargetGroup;
            buildPlayerOptions.target = Target;
            buildingData.BuildOptions = buildPlayerOptions;

            // switch build target here.
            // NOTE: switching build target in batch mode is not supported. It can only be done in command line arguments.
            // This is a double-work in batch mode.
            // https://docs.unity.cn/2017.4/Documentation/ScriptReference/EditorUserBuildSettings.SwitchActiveBuildTarget.html
            if (Application.isBatchMode == false)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(TargetGroup, Target);
            }

            buildingData.AddReportMessage($"Switched Build Target: {TargetGroup} {Target}");
        }

        private void SetVersionCode(BuildPipelineBuildingData buildingData)
        {
            string cacheVersionCodeFilePath = Path.Combine(buildingData.CacheFolderPath, "VersionCode.txt");

            int buildNumber;
            if (File.Exists(cacheVersionCodeFilePath) == false)
            {
                buildNumber = 0;
            }
            else
            {
                string cachedVersionCodeString = File.ReadAllText(cacheVersionCodeFilePath);
                VersionCode cachedVersionCode = VersionCode.Parse(cachedVersionCodeString);

                if (cachedVersionCode.Major != VersionCode.Major
                    || cachedVersionCode.Minor != VersionCode.Minor
                    || cachedVersionCode.SubVersion != VersionCode.SubVersion)
                {
                    buildNumber = 0;
                }
                else
                {
                    buildNumber = cachedVersionCode.BuildNumber + 1;
                }
            }

            VersionCode vc = new(VersionCode.Major, VersionCode.Minor, VersionCode.SubVersion, buildNumber, VersionCode.Extra);

            switch (TargetGroup)
            {
                case BuildTargetGroup.Android:
                    SetVersionCodeAndroid(buildingData, vc);
                    break;
                case BuildTargetGroup.iOS:
                    SetVersionCodeIOS(buildingData, vc);
                    break;
                case BuildTargetGroup.Standalone:
                    SetVersionCodeStandalone(buildingData, vc);
                    break;
                default:
                    SetVersionCodeStandalone(buildingData, vc);
                    break;
            }

            FileIOUtility.SafeWriteAllText(cacheVersionCodeFilePath, $"{vc.Major}.{vc.Minor}.{vc.SubVersion}.{vc.BuildNumber}");
        }

        private void SetVersionCodeAndroid(BuildPipelineBuildingData buildingData, VersionCode versionCode)
        {
            string versionCodeString = $"{versionCode.Major}.{versionCode.Minor}.{versionCode.SubVersion}.{versionCode.BuildNumber}";
            PlayerSettings.bundleVersion = versionCodeString;
#if UNITY_ANDROID
            PlayerSettings.Android.bundleVersionCode = new VersionCodeConvertor().VersionToInteger(versionCode);
            buildingData.AddReportMessage($"Android Bundle Version Code: {PlayerSettings.Android.bundleVersionCode}");
#endif
            buildingData.ResultReport.VersionCode = versionCodeString;
            buildingData.AddReportMessage($"Set Version Code: {versionCodeString}");
        }

        private void SetVersionCodeIOS(BuildPipelineBuildingData buildingData, VersionCode versionCode)
        {
            string versionCodeString = $"{versionCode.Major}.{versionCode.Minor}.{versionCode.SubVersion}";
            PlayerSettings.bundleVersion = versionCodeString;
            buildingData.ResultReport.VersionCode = versionCodeString;
            buildingData.AddReportMessage($"Set Version Code: {versionCodeString}");
        }

        private void SetVersionCodeStandalone(BuildPipelineBuildingData buildingData, VersionCode versionCode)
        {
            string versionCodeString = $"{versionCode.Major}.{versionCode.Minor}.{versionCode.SubVersion}.{versionCode.BuildNumber}";
            buildingData.ResultReport.VersionCode = versionCodeString;
            buildingData.AddReportMessage($"Set Version Code: {versionCodeString}");

            if (string.IsNullOrWhiteSpace(versionCode.Extra) == false)
            {
                versionCodeString += $"-{versionCode.Extra}";
            }

            PlayerSettings.bundleVersion = versionCodeString;
        }

#if UNITY_EDITOR
        [Button]
        private void OpenOutputDirectory()
        {
            string path = Path.Combine(BuildPipelineFolderPath, "Build", BuildName);
            Debug.Log($"Opening Directory: {path}");
            OpenDirectoryHelper.OpenDirectory(path);
        }
#endif
    }
}