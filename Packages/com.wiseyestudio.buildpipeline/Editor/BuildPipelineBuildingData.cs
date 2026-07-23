using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace WiseyeStudio.Build
{
    public class BuildPipelineBuildingData
    {
        public string BuildBaseFolderPath { get; private set; }
        public string BuildName { get; private set; }
        public string BuildFolderPath { get; private set; }
        public string TargetZippedBuildFilePath { get; private set; }
        public string ReportFilePath { get; private set; }
        public string CacheFolderPath { get; private set; }
        public string ExecutableName { get; private set; }

        public BuildPlayerOptions BuildOptions { get; set; }
        public BuildPipelineBuildResultReport ResultReport { get; private set; }
        public double BuildStartTimeInSeconds { get; private set; }

        public BuildPipelineBuildingData(
            string buildBaseFolderPath,
            string buildName,
            string executableName,
            BuildOptions buildOptions,
            double buildStartTimeInSeconds)
        {
            BuildBaseFolderPath = buildBaseFolderPath;
            BuildName = buildName;
            BuildFolderPath = Path.Combine(BuildBaseFolderPath, "Build", BuildName);
            TargetZippedBuildFilePath = BuildFolderPath + ".zip";
            ReportFilePath = Path.Combine(BuildBaseFolderPath, "Build", $"{BuildName}_build_report.json");
            CacheFolderPath = Path.Combine(BuildBaseFolderPath, "Cache", BuildName);
            ExecutableName = executableName;

            BuildOptions = new BuildPlayerOptions()
            {
                options = buildOptions,
                locationPathName = Path.Combine(BuildFolderPath, executableName)
            };
            BuildStartTimeInSeconds = buildStartTimeInSeconds;

            ResultReport = new BuildPipelineBuildResultReport();
        }

        private string GetTimeStamp()
        {
            var secondElapsed = EditorApplication.timeSinceStartup - BuildStartTimeInSeconds;
            var t = TimeSpan.FromSeconds(secondElapsed);
            var timeStamp = $"{t.Hours:D2}h:{t.Minutes:D2}m:{t.Seconds:D2}s:{t.Milliseconds:D3}ms";
            return timeStamp + ">";
        }

        public void AddReportMessage(string message)
        {
            ResultReport.AddMessage($"{GetTimeStamp()} {message}");
            Debug.Log(message);
            File.WriteAllText(
                Path.Combine(Application.persistentDataPath, $"buildpipeline_{BuildName}.log"),
                ResultReport.ToReadableString());
        }
    }
}