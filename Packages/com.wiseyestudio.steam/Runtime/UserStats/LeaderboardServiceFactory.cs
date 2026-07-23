#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

#if ENABLE_STEAMWORKS
using Steamworks;

namespace WiseyeStudio.Steam.UserStats
{
    internal static class LeaderboardServiceFactory
    {
        internal static LeaderboardService CreateLeaderboardService(
            string leaderboardId, LeaderboardSortMethod sortMethod, LeaderboardDisplayType displayType)
        {
            return new LeaderboardService(
                leaderboardId,
                Convert(sortMethod),
                Convert(displayType)
            );
        }

        private static ELeaderboardDisplayType Convert(LeaderboardDisplayType displayType)
        {
            return displayType switch
            {
                LeaderboardDisplayType.None => ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNone,
                LeaderboardDisplayType.Numeric => ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric,
                LeaderboardDisplayType.TimeSeconds => ELeaderboardDisplayType.k_ELeaderboardDisplayTypeTimeSeconds,
                LeaderboardDisplayType.TimeMilliseconds => ELeaderboardDisplayType
                    .k_ELeaderboardDisplayTypeTimeMilliSeconds,
                _ => throw new System.ArgumentException($"Unknown LeaderboardDisplayType: {displayType}")
            };
        }

        private static ELeaderboardSortMethod Convert(LeaderboardSortMethod sortMethod)
        {
            return sortMethod switch
            {
                LeaderboardSortMethod.None => ELeaderboardSortMethod.k_ELeaderboardSortMethodNone,
                LeaderboardSortMethod.Ascending => ELeaderboardSortMethod.k_ELeaderboardSortMethodAscending,
                LeaderboardSortMethod.Descending => ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending,
                _ => throw new System.ArgumentException($"Unknown LeaderboardSortMethod: {sortMethod}")
            };
        }
    }
}
#endif