#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

#if ENABLE_STEAMWORKS
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Steamworks;
using UnityEngine;

namespace WiseyeStudio.Steam.Friends
{
    internal interface ISteamFriendsInformationAccessor : IDisposable
    {
        internal UniTask<string> GetPersonaNameAsync(CancellationToken cancellationToken);
        internal UniTask<Sprite> GetAvatarAsync(CancellationToken cancellationToken);
    }
}
#endif