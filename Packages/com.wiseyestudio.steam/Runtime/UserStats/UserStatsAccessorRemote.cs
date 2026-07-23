#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

#if ENABLE_STEAMWORKS
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Steamworks;
using UnityEngine;

namespace WiseyeStudio.Steam.UserStats
{
    /// <summary>
    /// Provides read-only access to another user's (friend's) Steam stats and achievements.
    /// Does not support writing stats/achievements.
    /// </summary>
    internal class UserStatsAccessorRemote : IUserStatsAccessor
    {
        private readonly CSteamID _steamId;

        private readonly Callback<UserStatsUnloaded_t> _userStatsUnloaded;
        // Canceled on Dispose to avoid hanging task
        private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();

        private bool _needToLoadStats = true;
        private bool _statsLoaded = false;

        public UserStatsAccessorRemote(CSteamID steamId)
        {
            _steamId = steamId;
            _userStatsUnloaded = Callback<UserStatsUnloaded_t>.Create(OnUserStatsUnloaded);
        }

        public async UniTask<(bool, int)> GetStatIntAsync(string statName, CancellationToken cancellationToken)
        {
            await EnsureStatsLoadedAsync(cancellationToken);

            if (_statsLoaded == false)
            {
                Debug.LogWarning($"RemoteUserStatsAccessor: Failed to load stats for user {_steamId}");
                return (false, -1);
            }
            
            var success = SteamUserStats.GetUserStat(_steamId, statName, out int stat);
            return (success, stat);
        }

        public async UniTask<(bool,float)> GetStatFloatAsync(string statName, CancellationToken cancellationToken)
        {
            await EnsureStatsLoadedAsync(cancellationToken);

            if (_statsLoaded == false)
            {
                Debug.LogWarning($"RemoteUserStatsAccessor: Failed to load stats for user {_steamId}");
                return (false, -1f);
            }

            var success = SteamUserStats.GetUserStat(_steamId, statName, out float stat);
            return (success, stat);
        }

        public void InvalidateCache()
        {
            _needToLoadStats = true;
            _statsLoaded = false;
        }

        private async UniTask EnsureStatsLoadedAsync(CancellationToken cancellationToken)
        {
            if (_needToLoadStats == false)
            {
                return;
            }

            try
            {
                using var linkedCts =
                    CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);

                // Request stats for the remote user asynchronously
                var handle = SteamUserStats.RequestUserStats(_steamId);
                var callback = await handle.ToUniTask<UserStatsReceived_t>(linkedCts.Token);

                if (callback.m_eResult != EResult.k_EResultOK)
                {
                    _statsLoaded = false;
                    return;
                }

                _statsLoaded = true;
                _needToLoadStats = false;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                _statsLoaded = false;
            }
        }

        private void OnUserStatsUnloaded(UserStatsUnloaded_t callBack)
        {
            if (callBack.m_steamIDUser != _steamId)
            {
                return;
            }
            
            InvalidateCache();
        }

        public void Dispose()
        {
            _disposeCts.Cancel();
            _disposeCts.Dispose();

            _userStatsUnloaded?.Dispose();
        }
    }
}
#endif
