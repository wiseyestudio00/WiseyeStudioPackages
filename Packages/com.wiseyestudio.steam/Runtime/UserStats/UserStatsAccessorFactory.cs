#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

#if ENABLE_STEAMWORKS
using Steamworks;
using WiseyeStudio.Steam.UserStats;

namespace WiseyeStudio.Steam.UserStats
{
    internal class UserStatsAccessorFactory
    {
        internal static IUserStatsAccessor Create(CSteamID steamId)
        {
            if (steamId == SteamUser.GetSteamID())
            {
                return new UserStatsAccessorLocal();
            }
            else
            {
                return new UserStatsAccessorRemote(steamId);
            }
        }
    }
}
#endif