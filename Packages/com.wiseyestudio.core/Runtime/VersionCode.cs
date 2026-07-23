using UnityEngine;

namespace WiseyeStudio
{
    [System.Serializable]
    public struct VersionCode
    {
        public const int MAX_MAJOR = 127;
        public const int MAX_MINOR = 255;
        public const int MAX_SUB_VERSION = 15;
        public const int MAX_BUILD_NUMBER = 4095;

        [Range(0, MAX_MAJOR)]
        [SerializeField] public int Major;
        [Range(0, MAX_MINOR)]
        [SerializeField] public int Minor;
        [Range(0, MAX_SUB_VERSION)]
        [SerializeField] public int SubVersion;
        [HideInInspector] public int BuildNumber;

        [SerializeField] public string Extra;

        public VersionCode(
            int major,
            int minor,
            int subVersion,
            int buildNumber,
            string extra)
        {
            Major = major;
            Minor = minor;
            SubVersion = subVersion;
            BuildNumber = buildNumber;
            Extra = extra;
        }

        public static VersionCode Parse(string versionCode)
        {
            string[] versionCodeSplit = versionCode.Split('.');

            int major = 0;
            int minor = 1;
            int subVersion = 1;
            int buildNumber = 0;

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
