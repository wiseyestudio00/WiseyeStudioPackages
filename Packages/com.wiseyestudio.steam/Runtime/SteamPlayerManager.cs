#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if ENABLE_STEAMWORKS
using Steamworks;
#endif

namespace WiseyeStudio.Steam
{
    public class SteamPlayerManager : IDisposable
    {
#if ENABLE_STEAMWORKS
        private readonly Callback<UserStatsStored_t> _userStatsStoredCallback;
        private UniTaskCompletionSource<UserStatsStored_t> _userStatsStoredTcs;

        // UserStatsStored_t is a global broadcast, so only one StoreStats may be in flight at a time.
        private readonly SemaphoreSlim _storeStatsGate = new SemaphoreSlim(1, 1);

        // Canceled on Dispose to avoid hanging task
        private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();
#endif
        
        internal SteamPlayerManager()
        {
#if ENABLE_STEAMWORKS
            _userStatsStoredCallback = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
#endif
        }

        public ulong GetSteamId()
        {
#if ENABLE_STEAMWORKS
            return SteamUser.GetSteamID().m_SteamID;
#endif
            return 0;
        }
        
        public string GetName()
        {
#if ENABLE_STEAMWORKS
            return SteamworksManager.Instance.Available ? SteamFriends.GetPersonaName() : string.Empty;
#endif
            return String.Empty;
        }

        public bool LoggedOn()
        {
#if ENABLE_STEAMWORKS
            return SteamworksManager.Instance.Available && SteamUser.BLoggedOn();
#endif
            return false;
        }

        private Sprite _avatarSprite;
        private async UniTask LazyLoadAvatarAsync(CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            if (_avatarSprite != null)
            {
                return;
            }
            
            try
            {
                using var linkedCts =
                    CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);
                var userSteamId = SteamUser.GetSteamID();
                using var loader = new SteamAvatarLoader(userSteamId);
                _avatarSprite = await loader.LoadPfpAsync(linkedCts.Token);
            }
            catch ( Exception e )
            {
                // If something goes wrong, log it
                Debug.Log( e );
            }
#endif
            await UniTask.CompletedTask;
        }

        public async UniTask<Sprite> GetSteamAvatarAsSpriteAsync(CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            await LazyLoadAvatarAsync(cancellationToken);
            return _avatarSprite;
#endif
            return null;
        }

        internal bool GetStat(string statName, out int stat)
        {
#if ENABLE_STEAMWORKS
            return SteamUserStats.GetStat(statName, out stat);
#endif
            stat = -1;
            return false;
        }

        internal bool GetStat(string statName, out float stat)
        {
#if ENABLE_STEAMWORKS
            return SteamUserStats.GetStat(statName, out stat);
#endif
            stat = -1f;
            return false;
        }

        public UniTask<bool> SetStatAsync(string statName, int value, CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            // This call is no longer required, as it is handled in SteamClient.Init, at least from 2025.162.1
            // SteamUserStats.RequestCurrentStats()
            return UniTask.FromResult(SteamUserStats.SetStat(statName, value));
#endif
            return UniTask.FromResult(false);
        }

        public UniTask<bool> SetStatAsync(string statName, float value, CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            return UniTask.FromResult(SteamUserStats.SetStat(statName, value));
#endif
            return UniTask.FromResult(false);
        }

        public async UniTask<bool> StoreStatAsync(CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);

                await _storeStatsGate.WaitAsync(linkedCts.Token);
                try
                {
                    _userStatsStoredTcs = new UniTaskCompletionSource<UserStatsStored_t>();
                    if (SteamUserStats.StoreStats() == false) return false;

                    var callback = await _userStatsStoredTcs.Task.AttachExternalCancellation(linkedCts.Token);
                    return callback.m_eResult == EResult.k_EResultOK;
                }
                finally
                {
                    _userStatsStoredTcs = null;
                    _storeStatsGate.Release();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("StoreStatAsync was cancelled.");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
#endif
            return false;
        }

        public UniTask<bool> SetAchievementAsync(string achievementName, CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            return UniTask.FromResult(SteamUserStats.SetAchievement(achievementName));
#endif
            return UniTask.FromResult(false);
        }
        
#if ENABLE_STEAMWORKS
        private void OnUserStatsStored(UserStatsStored_t userStatsStored)
        {
            _userStatsStoredTcs?.TrySetResult(userStatsStored);
        }
#endif
        public void Dispose()
        {
#if ENABLE_STEAMWORKS
            _disposeCts.Cancel();
            _disposeCts.Dispose();

            _userStatsStoredCallback?.Dispose();

            // Sprites and their textures are engine objects and are not
            // garbage collected; destroy them explicitly.
            if (_avatarSprite != null)
            {
                UnityEngine.Object.Destroy(_avatarSprite.texture);
                UnityEngine.Object.Destroy(_avatarSprite);
                _avatarSprite = null;
            }

            // _storeStatsGate is intentionally not disposed: SemaphoreSlim only
            // holds resources once AvailableWaitHandle is used (we never do),
            // and a canceled waiter's finally block may still Release() after
            // this point, which would throw on a disposed semaphore.
#endif
        }
    }
}
