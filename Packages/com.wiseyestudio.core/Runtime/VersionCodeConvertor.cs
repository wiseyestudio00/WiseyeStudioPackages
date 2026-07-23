using UnityEngine;

namespace WiseyeStudio
{
    public class VersionCodeConvertor
    {
        public string IntegerToVersion(int i)
        {
            int major = (int)((i & 0xFF000000) >> 24);
            int minor = ((i & 0x00FF0000) >> 16);
            int tiny = ((i & 0x0000F000) >> 12);
            int build = (i & 0x00000FFF);
            return $"{major}.{minor}.{tiny}.{build}";
        }

        public int VersionToInteger(string version)
        {
            var codes = version.Split('.');

            if (codes.Length != 4)
            {
                Debug.LogError($"Invalid version code format: {version}");
                return -1;
            }

            int major = int.Parse(codes[0]);
            int minor = int.Parse(codes[1]);
            int specialCode = int.Parse(codes[2]);
            int build = int.Parse(codes[3]);

            long majorL = (major << 24) & 0xFF000000;
            long minorL = (minor << 16) & 0x00FF0000;
            long specialCodeL = (specialCode << 12) & 0x0000F000;
            long buildL = build & 0x00000FFF;

            return (int)(majorL | minorL | specialCodeL | buildL);
        }

        public int VersionToInteger(VersionCode versionCode)
        {
            return VersionToInteger($"{versionCode.Major}.{versionCode.Minor}.{versionCode.SubVersion}.{versionCode.BuildNumber}");
        }
    }
}