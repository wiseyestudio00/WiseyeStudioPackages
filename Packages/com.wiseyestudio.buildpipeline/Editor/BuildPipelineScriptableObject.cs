using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.Serialization;
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
        [FormerlySerializedAs("TargetGroup")]
        [SerializeField] private BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;
        [FormerlySerializedAs("Target")]
        [SerializeField] private BuildTarget target;

        [Title("Output Setting")]
        [FormerlySerializedAs("BuildPipelineFolderPath")]
        [SerializeField] private string buildPipelineFolderPath;
        [FormerlySerializedAs("BuildName")]
        [SerializeField] private string buildName;
        [FormerlySerializedAs("ExecutableName")]
        [SerializeField] private string executableName;
        [FormerlySerializedAs("Exntesion")]
        [SerializeField] private string extension;

        [Title("Build Setting")]
        [FormerlySerializedAs("DevelopmentBuild")]
        [SerializeField] private bool developmentBuild;
        [FormerlySerializedAs("DeepProfileSupport")]
        [SerializeField] private bool deepProfileSupport;

        [Title("Version Code")]
        [FormerlySerializedAs("VersionCode")]
        [SerializeField] private VersionCode versionCode;

        [Title("Extra Define Symbols")]
        [FormerlySerializedAs("SetExtraDefineSymbolsMethod")]
        [SerializeField] private BuildMethodSetExtraDefineSymbolsScriptableObject setExtraDefineSymbolsMethod;

        [Title("Set Scenes")]
        [FormerlySerializedAs("SetSceneListMethod")]
        [Required, SerializeField] private BuildMethodSetSceneListScriptableObject setSceneListMethod;

        [Title("Pre Build Methods")]
        [FormerlySerializedAs("PrebuildBuildMethods")]
        [Required, SerializeField] private List<ABuildPipelineBuildMethodScriptableObject> prebuildBuildMethods = new();

        [Title("Asset Management")]
        [FormerlySerializedAs("AssetManagementBuildMethods")]
        [Required, SerializeField] private List<ABuildPipelineBuildMethodScriptableObject> assetManagementBuildMethods = new();

        [Title("Post Build Methods")]
        [FormerlySerializedAs("PostbuildBuildMethods")]
        [Required, SerializeField] private List<ABuildPipelineBuildMethodScriptableObject> postBuildBuildMethods = new();

        [Title("Compress Build Folder")]
        [FormerlySerializedAs("CompressBuildFolderMethod")]
        [Required, SerializeField] private ABuildPipelineBuildMethodScriptableObject compressBuildFolderMethod;

        [Title("Upload")]
        [FormerlySerializedAs("EnableUpload")]
        [SerializeField] private bool enableUpload = true;

        [FormerlySerializedAs("UploadMethods")]
        [ShowIf(nameof(enableUpload))]
        [Required, SerializeField] private List<ABuildPipelineBuildMethodScriptableObject> uploadMethods = new();

        [Button]
        public BuildPipelineBuildingData Build()
        {
            var startBuildTime = DateTime.Now;
            var buildingData = CreateBuildingData();

            try
            {
                SetBuildTarget(buildingData);
                SetVersionCode(buildingData);
             
                buildingData.AddReportMessage($"=== Start Pre-Building ===");

                if(setExtraDefineSymbolsMethod != null)
                {
                    setExtraDefineSymbolsMethod.Activate(buildingData);
                }

                if(setSceneListMethod != null)
                {
                    setSceneListMethod.Activate(buildingData);
                }

                foreach (var buildMethod in prebuildBuildMethods)
                {
                    buildMethod.Activate(buildingData);
                }

                foreach (var buildMethod in assetManagementBuildMethods)
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
                foreach (var buildMethod in postBuildBuildMethods)
                {
                    buildMethod.Activate(buildingData);
                }

                compressBuildFolderMethod.Activate(buildingData);

                if (string.IsNullOrEmpty(buildingData.ResultReport.builtResultPath))
                {
                    buildingData.ResultReport.builtResultPath = buildingData.BuildOptions.locationPathName;
                    buildingData.AddReportMessage($"Built Result Path is not set. Added Built Result Path: {buildingData.ResultReport.builtResultPath}");
                }

                if (enableUpload)
                {
                    foreach (var buildMethod in uploadMethods)
                    {
                        buildMethod.Activate(buildingData);
                    }
                }

                buildingData.ResultReport.success = true;
                buildingData.ResultReport.buildTimeInSeconds = (DateTime.Now - startBuildTime).TotalSeconds;
                buildingData.AddReportMessage($"=== Build Finished ===");
            }
            catch (Exception e)
            {
                buildingData.ResultReport.success = false;
                buildingData.AddReportMessage(e.Message);
                buildingData.AddReportMessage(e.StackTrace);
                buildingData.AddReportMessage($"=== Build Failed ===");
            }

            File.WriteAllText(buildingData.ReportFilePath, buildingData.ResultReport.ToJsonString());
            return buildingData;
        }

        private BuildOptions GetBuildOptions()
        {
            var buildOptions = BuildOptions.None;
            buildOptions = developmentBuild ? buildOptions | BuildOptions.Development : buildOptions;
            buildOptions = deepProfileSupport ? buildOptions | BuildOptions.EnableDeepProfilingSupport : buildOptions;
            return buildOptions;
        }

        private string GetExecutableName()
        {
            return $"{executableName}.{extension}";
        }

        private BuildPipelineBuildingData CreateBuildingData()
        {
            return new BuildPipelineBuildingData(
                buildPipelineFolderPath,
                buildName + (developmentBuild ? "_Development" : string.Empty),
                GetExecutableName(),
                GetBuildOptions(),
                EditorApplication.timeSinceStartup);
        }

        private void SetBuildTarget(BuildPipelineBuildingData buildingData)
        {
            var buildPlayerOptions = buildingData.BuildOptions;
            buildPlayerOptions.targetGroup = targetGroup;
            buildPlayerOptions.target = target;
            buildingData.BuildOptions = buildPlayerOptions;

            // switch build target here.
            // NOTE: switching build target in batch mode is not supported. It can only be done in command line arguments.
            // This is a double-work in batch mode.
            // https://docs.unity.cn/2017.4/Documentation/ScriptReference/EditorUserBuildSettings.SwitchActiveBuildTarget.html
            if (Application.isBatchMode == false)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);
            }

            buildingData.AddReportMessage($"Switched Build Target: {targetGroup} {target}");
        }

        private void SetVersionCode(BuildPipelineBuildingData buildingData)
        {
            var cacheVersionCodeFilePath = Path.Combine(buildingData.CacheFolderPath, "VersionCode.txt");

            int buildNumber;
            if (File.Exists(cacheVersionCodeFilePath) == false)
            {
                buildNumber = 0;
            }
            else
            {
                string cachedVersionCodeString = File.ReadAllText(cacheVersionCodeFilePath);
                VersionCode cachedVersionCode = VersionCode.Parse(cachedVersionCodeString);

                if (cachedVersionCode.major != versionCode.major
                    || cachedVersionCode.minor != versionCode.minor
                    || cachedVersionCode.subVersion != versionCode.subVersion)
                {
                    buildNumber = 0;
                }
                else
                {
                    buildNumber = cachedVersionCode.buildNumber + 1;
                }
            }

            VersionCode vc = new(versionCode.major, versionCode.minor, versionCode.subVersion, buildNumber, versionCode.extra);

            switch (targetGroup)
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

            FileIOUtility.SafeWriteAllText(cacheVersionCodeFilePath, $"{vc.major}.{vc.minor}.{vc.subVersion}.{vc.buildNumber}");
        }

        private void SetVersionCodeAndroid(BuildPipelineBuildingData buildingData, VersionCode version)
        {
            var versionCodeString = $"{version.major}.{version.minor}.{version.subVersion}.{version.buildNumber}";
            PlayerSettings.bundleVersion = versionCodeString;
#if UNITY_ANDROID
            PlayerSettings.Android.bundleVersionCode = new VersionCodeConvertor().VersionToInteger(version);
            buildingData.AddReportMessage($"Android Bundle Version Code: {PlayerSettings.Android.bundleVersionCode}");
#endif
            buildingData.ResultReport.versionCode = versionCodeString;
            buildingData.AddReportMessage($"Set Version Code: {versionCodeString}");
        }

        private void SetVersionCodeIOS(BuildPipelineBuildingData buildingData, VersionCode version)
        {
            var versionCodeString = $"{version.major}.{version.minor}.{version.subVersion}";
            PlayerSettings.bundleVersion = versionCodeString;
            buildingData.ResultReport.versionCode = versionCodeString;
            buildingData.AddReportMessage($"Set Version Code: {versionCodeString}");
        }

        private void SetVersionCodeStandalone(BuildPipelineBuildingData buildingData, VersionCode version)
        {
            var versionCodeString = $"{version.major}.{version.minor}.{version.subVersion}.{version.buildNumber}";
            buildingData.ResultReport.versionCode = versionCodeString;
            buildingData.AddReportMessage($"Set Version Code: {versionCodeString}");

            if (string.IsNullOrWhiteSpace(version.extra) == false)
            {
                versionCodeString += $"-{version.extra}";
            }

            PlayerSettings.bundleVersion = versionCodeString;
        }

#if UNITY_EDITOR
        [Button]
        private void OpenOutputDirectory()
        {
            var path = Path.Combine(buildPipelineFolderPath, "Build", buildName);
            Debug.Log($"Opening Directory: {path}");
            OpenDirectoryHelper.OpenDirectory(path);
        }
#endif
    }
}