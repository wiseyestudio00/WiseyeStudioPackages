#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

#if ENABLE_STEAMWORKS
using System.Threading;
using Cysharp.Threading.Tasks;
using WiseyeStudio.Steam.UserStats;

namespace WiseyeStudio.Steam.UserStats
{
    internal class UserStatsAccessorLocal : IUserStatsAccessor
    {
        public UniTask<(bool, int)> GetStatIntAsync(string statName, CancellationToken cancellationToken)
        {
            bool success = SteamworksManager.Instance.Player.GetStat(statName, out int stat);
            return UniTask.FromResult((success, stat));
        }

        public UniTask<(bool, float)> GetStatFloatAsync(string statName, CancellationToken cancellationToken)
        {
            bool success = SteamworksManager.Instance.Player.GetStat(statName, out float stat);
            return  UniTask.FromResult((success, stat));
        }

        public void InvalidateCache()
        {
        }
        
        public void Dispose()
        {
        }
    }
}
#endif