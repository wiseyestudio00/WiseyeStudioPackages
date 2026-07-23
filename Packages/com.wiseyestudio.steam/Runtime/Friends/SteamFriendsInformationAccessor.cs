#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

#if ENABLE_STEAMWORKS
using System.Threading;
using Cysharp.Threading.Tasks;
using Steamworks;
using UnityEngine;

namespace WiseyeStudio.Steam.Friends
{
    internal class SteamFriendsInformationAccessor : ISteamFriendsInformationAccessor
    {
        private readonly CSteamID _steamID;
        private readonly Callback<PersonaStateChange_t>  _personaStateChangeCallback;
        private UniTaskCompletionSource<PersonaStateChange_t> _userInformationTcs;

        // PersonaStateChange_t is a global broadcast, so only one fetch may be in flight per accessor.
        private readonly SemaphoreSlim _fetchGate = new SemaphoreSlim(1, 1);
        // Ensures concurrent first requests load the avatar only once.
        private readonly SemaphoreSlim _avatarGate = new SemaphoreSlim(1, 1);

        // Canceled on Dispose to avoid hanging task
        private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();

        internal SteamFriendsInformationAccessor(CSteamID steamId)
        {
            _steamID = steamId;
            _personaStateChangeCallback = Callback<PersonaStateChange_t>.Create(OnPersonaStateChange);
        }
        
        async UniTask<string> ISteamFriendsInformationAccessor.GetPersonaNameAsync(CancellationToken cancellationToken)
        {
            await EnsureUserInformationFetchedAsync(cancellationToken);
            return SteamFriends.GetFriendPersonaName(_steamID);
        }

        private Sprite _avatarSprite;
        async UniTask<Sprite> ISteamFriendsInformationAccessor.GetAvatarAsync(CancellationToken cancellationToken)
        {
            if (_avatarSprite != null) return _avatarSprite;

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);

            await _avatarGate.WaitAsync(linkedCts.Token);
            try
            {
                // A previous holder of the gate may have loaded it already.
                if (_avatarSprite != null) return _avatarSprite;

                using var loader = new SteamAvatarLoader(_steamID);
                _avatarSprite = await loader.LoadPfpAsync(linkedCts.Token);
                return _avatarSprite;
            }
            finally
            {
                _avatarGate.Release();
            }
        }

        private async UniTask EnsureUserInformationFetchedAsync(CancellationToken cancellationToken)
        {
            using var linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);

            await _fetchGate.WaitAsync(linkedCts.Token);
            try
            {
                _userInformationTcs = new UniTaskCompletionSource<PersonaStateChange_t>();

                // false means it is already available locally.
                if (SteamFriends.RequestUserInformation(_steamID, false))
                {
                    await _userInformationTcs.Task.AttachExternalCancellation(linkedCts.Token);
                }
            }
            finally
            {
                _userInformationTcs = null;
                _fetchGate.Release();
            }
        }

        private void OnPersonaStateChange(PersonaStateChange_t personaStateChange)
        {
            if (new CSteamID(personaStateChange.m_ulSteamID) != _steamID)
            {
                return;                
            }
            
            _userInformationTcs?.TrySetResult(personaStateChange);
        }

        public void Dispose()
        {
            _disposeCts.Cancel();
            _disposeCts.Dispose();

            _personaStateChangeCallback?.Dispose();

            // Sprites and their textures are engine objects and are not
            // garbage collected; destroy them explicitly.
            if (_avatarSprite != null)
            {
                Object.Destroy(_avatarSprite.texture);
                Object.Destroy(_avatarSprite);
                _avatarSprite = null;
            }

            // _fetchGate is intentionally not disposed: SemaphoreSlim only
            // holds resources once AvailableWaitHandle is used (we never do),
            // and a canceled waiter's finally block may still Release() after
            // this point, which would throw on a disposed semaphore.
        }
    }
}
#endif