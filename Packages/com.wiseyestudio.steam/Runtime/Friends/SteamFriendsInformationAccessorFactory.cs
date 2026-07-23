#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

#if ENABLE_STEAMWORKS
using Steamworks;

namespace WiseyeStudio.Steam.Friends
{
    internal class SteamFriendsInformationAccessorFactory
    {
        internal static ISteamFriendsInformationAccessor Create(CSteamID steamID)
        {
            return new SteamFriendsInformationAccessor(steamID);
        }
    }
}
#endif