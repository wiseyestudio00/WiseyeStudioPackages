#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

using System;

#if ENABLE_STEAMWORKS
using Steamworks;
#endif

namespace WiseyeStudio.Steam.Apps
{
    public class SteamAppsManager : IDisposable
    {
        private const int AppInstallDirBufferSize = 1024;
        private const uint DefaultMaxDepots = 32;

        internal SteamAppsManager()
        {

        }

        /// <summary>
        /// Whether the app is installed, which does not imply that the active user owns it.
        /// </summary>
        public bool IsAppInstalled(uint appId)
        {
#if ENABLE_STEAMWORKS
            return SteamApps.BIsAppInstalled(new AppId_t(appId));
#endif
            return false;
        }

        public bool IsDlcInstalled(uint appId)
        {
#if ENABLE_STEAMWORKS
            return SteamApps.BIsDlcInstalled(new AppId_t(appId));
#endif
            return false;
        }
        
        /// <summary>
        /// Whether the license only grants the low violence depots, which some territories require.
        /// </summary>
        public bool IsLowViolence()
        {
#if ENABLE_STEAMWORKS
            return SteamApps.BIsLowViolence();
#endif
            return false;
        }

        /// <summary>
        /// Whether the active user owns the current app.
        /// </summary>
        public bool IsSubscribed()
        {
#if ENABLE_STEAMWORKS
            return SteamApps.BIsSubscribed();
#endif
            return false;
        }

        /// <summary>
        /// Whether the active user owns the given app. Only meant for apps related to yours,
        /// such as a demo of your game.
        /// </summary>
        public bool IsSubscribed(uint appId)
        {
#if ENABLE_STEAMWORKS
            return SteamApps.BIsSubscribedApp(new AppId_t(appId));
#endif
            return false;
        }

        /// <summary>
        /// Whether the license is a temporary one borrowed through Family Sharing.
        /// When true, <see cref="GetAppOwnerSteamId"/> returns the lender.
        /// </summary>
        public bool IsSubscribedFromFamilySharing()
        {
#if ENABLE_STEAMWORKS
            return SteamApps.BIsSubscribedFromFamilySharing();
#endif
            return false;
        }

        /// <summary>
        /// Whether the license comes from a free weekend promotion. Always false for retail licenses.
        /// </summary>
        public bool IsSubscribedFromFreeWeekend()
        {
#if ENABLE_STEAMWORKS
            return SteamApps.BIsSubscribedFromFreeWeekend();
#endif
            return false;
        }

        /// <summary>
        /// Whether the active user account has a VAC ban.
        /// </summary>
        public bool IsVacBanned()
        {
#if ENABLE_STEAMWORKS
            return SteamApps.BIsVACBanned();
#endif
            return false;
        }

        /// <summary>
        /// Steam ID of the original owner of the license. Differs from the active user's Steam ID
        /// when the app is borrowed through Family Sharing.
        /// </summary>
        public ulong GetAppOwnerSteamId()
        {
#if ENABLE_STEAMWORKS
            return SteamApps.GetAppOwner().m_SteamID;
#endif
            return 0;
        }

        public void Dispose()
        {
            
        }
    }
}
