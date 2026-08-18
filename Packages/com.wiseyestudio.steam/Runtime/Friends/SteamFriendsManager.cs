#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

#if ENABLE_STEAMWORKS
using Steamworks;
#endif

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WiseyeStudio.Steam.Friends
{
    public class SteamFriendsManager : IDisposable
    {
#if ENABLE_STEAMWORKS
        private readonly Dictionary<CSteamID, ISteamFriendsInformationAccessor> _accessors = new Dictionary<CSteamID, ISteamFriendsInformationAccessor>();
        private readonly Callback<GameOverlayActivated_t> _gameOverlayActivatedCallback;
#endif

        /// <summary>
        /// Raised when the Steam Overlay is opened (true) or closed (false).
        /// </summary>
        public event Action<bool> GameOverlayActivated;

        internal SteamFriendsManager()
        {
#if ENABLE_STEAMWORKS
            _gameOverlayActivatedCallback = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
#endif
        }

#if ENABLE_STEAMWORKS
        private void OnGameOverlayActivated(GameOverlayActivated_t callback)
        {
            GameOverlayActivated?.Invoke(callback.m_bActive != 0);
        }
#endif

        public async UniTask<string> GetUserPersonaNameAsync(ulong steamId, CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            return await GetUserPersonaNameAsync(new CSteamID(steamId), cancellationToken);
#endif
            return "[unknown]";
        }

        public async UniTask<Sprite> GetUserAvatarAsync(ulong steamId, CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            return await GetUserAvatarAsync(new CSteamID(steamId), cancellationToken);
#endif
            return null;
        }

        /// <summary>
        /// Opens the Steam Overlay on the store page of the given app. Does nothing when the
        /// overlay is disabled; check <c>Utils.IsOverlayEnabled()</c> and fall back to a
        /// <c>steam://url/StoreAppPage/</c> URL in that case.
        /// </summary>
        public void OpenStorePageInOverlay(uint appId)
        {
#if ENABLE_STEAMWORKS
            SteamFriends.ActivateGameOverlayToStore(new AppId_t(appId), EOverlayToStoreFlag.k_EOverlayToStoreFlag_None);
#endif
        }
        
#if ENABLE_STEAMWORKS
        private async UniTask<string> GetUserPersonaNameAsync(CSteamID steamId, CancellationToken cancellationToken)
        {
            return await GetAccessor(steamId).GetPersonaNameAsync(cancellationToken);
        }

        private async UniTask<Sprite> GetUserAvatarAsync(CSteamID steamId, CancellationToken cancellationToken)
        {
            return await GetAccessor(steamId).GetAvatarAsync(cancellationToken);
        }

        private ISteamFriendsInformationAccessor GetAccessor(CSteamID steamId)
        {
            if (_accessors.ContainsKey(steamId) == false)
            {
                _accessors.Add(steamId, SteamFriendsInformationAccessorFactory.Create(steamId));
            }

            return _accessors[steamId];
        }
#endif

        public void Dispose()
        {
            GameOverlayActivated = null;
#if ENABLE_STEAMWORKS
            foreach (var accessor in _accessors.Values)
            {
                accessor.Dispose();
            }
            _accessors.Clear();
#endif
        }
    }
}
