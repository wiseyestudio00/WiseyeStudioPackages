using UnityEngine;
using UnityEngine.Serialization;

namespace WiseyeStudio
{
    [System.Serializable]
    public struct VersionCode
    {
        public const int MaxMajor = 127;
        public const int MaxMinor = 255;
        public const int MaxSubVersion = 15;
        public const int MaxBuildNumber = 4095;

        [FormerlySerializedAs("Major")]
        [Range(0, MaxMajor)]
        [SerializeField] public int major;
        
        [FormerlySerializedAs("Minor")]
        [Range(0, MaxMinor)]
        [SerializeField] public int minor;

        [FormerlySerializedAs("SubVersion")]
        [Range(0, MaxSubVersion)]
        [SerializeField] public int subVersion;

        [FormerlySerializedAs("BuildNumber")]
        [HideInInspector] public int buildNumber;

        [FormerlySerializedAs("Extra")]
        [SerializeField] public string extra;

        public VersionCode(
            int major,
            int minor,
            int subVersion,
            int buildNumber,
            string extra)
        {
            this.major = major;
            this.minor = minor;
            this.subVersion = subVersion;
            this.buildNumber = buildNumber;
            this.extra = extra;
        }

        public static VersionCode Parse(string versionCode)
        {
            var versionCodeSplit = versionCode.Split('.');

            var major = 0;
            var minor = 1;
            var subVersion = 1;
            var buildNumber = 0;

            if(versionCodeSplit.Length > 0)
            {
                major = int.Parse(versionCodeSplit[0]);
            }

            if (versionCodeSplit.Length > 1)
            {
                minor = int.Parse(versionCodeSplit[1]);
            }

            if (versionCodeSplit.Length > 2)
            {
                subVersion = int.Parse(versionCodeSplit[2]);
            }

            if (versionCodeSplit.Length > 3)
            {
                buildNumber = int.Parse(versionCodeSplit[3]);
            }

            return new VersionCode(major, minor, subVersion, buildNumber, string.Empty);
        }
    }
}
