using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace WiseyeStudio.Build
{
    [Serializable]
    public class BuildPipelineBuildResultReport
    {
        [JsonProperty]
        public bool success = false;
        [JsonProperty]
        public string versionCode = string.Empty;
        [JsonProperty]
        public string builtResultPath;
        [JsonProperty]
        public double buildTimeInSeconds;
        [JsonProperty]
        public List<string> shareLinks = new();
        [JsonProperty]
        public List<string> buildMessages = new();

        public void AddMessage(string message)
        {
            buildMessages.Add(message);
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public string ToReadableString()
        {
            StringBuilder stringBuilder = new();

            stringBuilder.AppendLine($"Success: {success}");
            stringBuilder.AppendLine($"Version Code: {versionCode}");
            stringBuilder.AppendLine($"BuiltResultPath: {builtResultPath}");
            stringBuilder.AppendLine($"BuildTimeInSeconds: {buildTimeInSeconds}");
            stringBuilder.AppendLine($"=========== ShareLinks =========== ");
            foreach (var link in shareLinks)
            {
                stringBuilder.AppendLine($"{link}");
            }
            stringBuilder.AppendLine($"=========== BuildMessages =========== ");
            foreach (var message in buildMessages)
            {
                stringBuilder.AppendLine($"{message}");
            }

            return stringBuilder.ToString();
        }
    }
}
