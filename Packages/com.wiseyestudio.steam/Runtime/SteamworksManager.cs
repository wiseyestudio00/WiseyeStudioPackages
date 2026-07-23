#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

using System;
using UnityEngine;
using WiseyeStudio.Steam.Friends;
using WiseyeStudio.Steam.UserStats;

#if ENABLE_STEAMWORKS
using Steamworks;
#endif

namespace WiseyeStudio.Steam
{
    public class SteamworksManager : MonoBehaviour
    {
        public static SteamworksManager Instance { get; private set; }
        public void Initialize(bool restartAppIfNecessary, ISteamAppIdProvider appIdProvider)
        {
            if (Instance != null) return;
            DontDestroyOnLoad(gameObject);
            Instance = this;
            Instance.InitializeInternal(restartAppIfNecessary, appIdProvider);
        }
        
        private ISteamAppIdProvider _appIdProvider;

        private bool SteamworksEnabled
        {
            get
            {
#if ENABLE_STEAMWORKS
                return true;
#endif
                return false; 
            }
        }

        private bool _alreadyTriedInitialize = false;
        private bool _available = false;

        public bool Available => SteamworksEnabled && _available;

        public SteamPlayerManager Player { get; private set; }
        public SteamUserStatsManager UserStats { get; private set; }
        public SteamFriendsManager Friends { get; private set; }
        
#if ENABLE_STEAMWORKS
        // I don't know what this is. It is from the original SteammManager.cs from Steamworks.Net
        private SteamAPIWarningMessageHook_t _steamAPIWarningMessageHook;
        [AOT.MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
        protected static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
        {
            Debug.LogWarning(pchDebugText);
        }
#endif
        
        private void InitializeInternal(bool restartAppIfNecessary, ISteamAppIdProvider appIdProvider)
        {
            _appIdProvider = appIdProvider;
#if ENABLE_STEAMWORKS
            if (SteamworksEnabled == false) return;
            
            // If already initialized, return
            if (Available) return;

            if (_alreadyTriedInitialize)
            {
                Debug.LogError("Tried to Initialize the SteamAPI twice in one session!");
                return;
            }

            if (!Packsize.Test())
            {
                Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
            }
            
            if (!DllCheck.Test())
            {
                Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
            }

            // The following is not strictly required
            // As stated in the documentation: optional but highly recommended
            // If launched directly via exe, we won't be able to access Steam functionalities,
            // But we sometimes need to be able to do just that (debug, expo...etc)
            if (restartAppIfNecessary)
            {
                try
                {
                    // If Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the
                    // Steam client and also launches this game again if the User owns it. This can act as a rudimentary form of DRM.

                    // Once you get a Steam AppID assigned by Valve, you need to replace AppId_t.Invalid with it and
                    // remove steam_appid.txt from the game depot. eg: "(AppId_t)480" or "new AppId_t(480)".
                    // See the Valve documentation for more information: https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
                    if (SteamAPI.RestartAppIfNecessary(new AppId_t(GetAppID())))
                    {
                        Application.Quit();
                        return;
                    }
                }
                catch (DllNotFoundException e)
                {
                    // We catch this exception here, as it will be the first occurrence of it.
                    Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e, this);

                    Application.Quit();
                    return;
                }   
            }
            
            _available = SteamAPI.Init();
            if (_available == false)
            {
                Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
            }
            else
            {
                Debug.Log($"[Steamworks.NET] SteamAPI_Init() is successful with AppId {GetAppID()}.");
            }
            
            if (_available && _steamAPIWarningMessageHook == null)
            {
                // Set up our callback to receive warning messages from Steam.
                // You must launch with "-debug_steamapi" in the launch args to receive warnings.
                _steamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
                SteamClient.SetWarningMessageHook(_steamAPIWarningMessageHook);   
            }

            if (_available)
            {
                Player = new SteamPlayerManager();
                UserStats  = new SteamUserStatsManager();
                Friends =  new SteamFriendsManager();
            }
            
            _alreadyTriedInitialize = true;
#endif
        }

        private uint GetAppID() => _appIdProvider.GetAppId();

        public void ShutDown()
        {
#if ENABLE_STEAMWORKS
            if (Available == false)
                return;

            Debug.Log("Shutting down Steam Client...");

            Player?.Dispose();
            UserStats?.Dispose();
            Friends?.Dispose();
            Player = null;
            UserStats = null;
            Friends = null;
            _available = false;

            SteamAPI.Shutdown();
#endif
        }

        private void Update()
        {
#if ENABLE_STEAMWORKS
            if (Available == false)
                return;

            SteamAPI.RunCallbacks(); 
#endif
        }

        private void OnDisable()
        {
#if ENABLE_STEAMWORKS
            _steamAPIWarningMessageHook = null;
#endif
        }
        
        private void OnDestroy()
        {
            if (Instance != this) return;
            ShutDown();
            Instance = null;
        }
    }
}