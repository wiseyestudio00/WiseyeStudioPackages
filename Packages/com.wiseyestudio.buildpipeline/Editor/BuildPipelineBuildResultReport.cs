using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using Newtonsoft.Json;

namespace WiseyeStudio.Build
{
    [Serializable]
    public class BuildPipelineBuildResultReport
    {
        [JsonProperty]
        public bool Success = false;
        [JsonProperty]
        public string VersionCode = string.Empty;
        [JsonProperty]
        public string BuiltResultPath;
        [JsonProperty]
        public double BuildTimeInSeconds;
        [JsonProperty]
        public List<string> ShareLinks = new();
        [JsonProperty]
        public List<string> BuildMessages = new();

        public void AddMessage(string message)
        {
            BuildMessages.Add(message);
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public string ToReadableString()
        {
            StringBuilder stringBuilder = new();

            stringBuilder.AppendLine($"Success: {Success}");
            stringBuilder.AppendLine($"Version Code: {VersionCode}");
            stringBuilder.AppendLine($"BuiltResultPath: {BuiltResultPath}");
            stringBuilder.AppendLine($"BuildTimeInSeconds: {BuildTimeInSeconds}");
            stringBuilder.AppendLine($"=========== ShareLinks =========== ");
            foreach (var link in ShareLinks)
            {
                stringBuilder.AppendLine($"{link}");
            }
            stringBuilder.AppendLine($"=========== BuildMessages =========== ");
            foreach (var message in BuildMessages)
            {
                stringBuilder.AppendLine($"{message}");
            }

            return stringBuilder.ToString();
        }
    }
}
