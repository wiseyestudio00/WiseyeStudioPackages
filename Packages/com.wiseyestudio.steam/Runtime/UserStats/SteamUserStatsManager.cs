#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WiseyeStudio.Steam.UserStats;

#if ENABLE_STEAMWORKS
using Steamworks;
#endif

namespace WiseyeStudio.Steam.UserStats
{
    /// <summary>
    /// This class should be accessed directly from any class
    /// But through another wrapper such as IOnlinePlayerDataBase
    /// </summary>
    public class SteamUserStatsManager : IDisposable
    {
#if ENABLE_STEAMWORKS
        private readonly Dictionary<CSteamID, IUserStatsAccessor> _userStatsAccessors = new();
        private readonly Dictionary<string, LeaderboardService> _leaderboardServices = new();
#endif

        /// <summary>
        /// Get an Int stats from a remote user (that is not the user).
        /// </summary>
        /// <param name="steamID"></param>
        /// <param name="statName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async UniTask<(bool, int)> GetUserStatsIntAsync(
            ulong steamID, string statName, CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            return await GetUserStatsIntAsync(new CSteamID(steamID), statName, cancellationToken);
#endif
            return (false, -1);
        }

        /// <summary>
        /// Get a Float stats from a remote user (that is not the user)
        /// </summary>
        /// <param name="steamID"></param>
        /// <param name="statName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async UniTask<(bool, float)> GetUserStatsFloatAsync(
            ulong steamID, string statName, CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            return await GetUserStatsFloatAsync(new CSteamID(steamID), statName, cancellationToken);
#endif
            return (false, -1f);
        }

        public async UniTask<bool> IdempotenceCreateLeaderboardAsync(
            string leaderboardId,
            LeaderboardSortMethod sortMethod,
            LeaderboardDisplayType displayType,
            CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            return await GetOrCreateLeaderboardService(leaderboardId, sortMethod, displayType)
                .CreateOrFindLeaderboardAsync(cancellationToken);
#endif
            return false;
        }

        public async UniTask<LeaderboardEntriesRequestResult> RequestGlobalLeaderboardTopScoresAsync(
            string leaderboardId, int count, int detailCount, CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            if (TryGetLeaderboardService(leaderboardId, out var leaderboardService) == false)
            {
                return LeaderboardEntriesRequestResult.CreateFailure(
                    errorMessage: $"Leaderboard {leaderboardId} is not created yet."
                );   
            }

            return await leaderboardService.RequestLeaderboardEntriesAsync(
                ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal,
                0,
                count,
                detailCount,
                cancellationToken
            );
#endif
            return LeaderboardEntriesRequestResult.CreateFailure("Steamworks is not enabled.");
        }

        public async UniTask<LeaderboardEntriesRequestResult> RequestFriendLeaderboardTopScoresAsync(
            string leaderboardId, int count, int detailCount, CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            if (TryGetLeaderboardService(leaderboardId, out var leaderboardService) == false)
            {
                return LeaderboardEntriesRequestResult.CreateFailure(
                    errorMessage: $"Leaderboard {leaderboardId} is not created yet."
                );   
            }

            return await leaderboardService.RequestLeaderboardEntriesAsync(
                ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends,
                0,
                count,
                detailCount,
                cancellationToken
            );
#endif
            return LeaderboardEntriesRequestResult.CreateFailure("Steamworks is not enabled.");
        }

        public async UniTask<LeaderboardEntriesRequestResult> RequestUserEntriesOnGlobalLeaderboardAsync(
            string leaderboardId, List<ulong> users, int detailCount, CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            if (TryGetLeaderboardService(leaderboardId, out var leaderboardService) == false)
            {
                return LeaderboardEntriesRequestResult.CreateFailure(
                    errorMessage: $"Leaderboard {leaderboardId} is not created yet."
                );   
            }

            return await leaderboardService.RequestLeaderboardEntriesAsync(
                users.Select(u => new CSteamID(u)).ToList(),
                detailCount, cancellationToken);
#endif
            return LeaderboardEntriesRequestResult.CreateFailure("Steamworks is not enabled.");
        }

        public async UniTask<LeaderboardScoreUploadResult> SubmitScoreAsync(
            string leaderboardId, int score, int[] details, CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            if (TryGetLeaderboardService(leaderboardId, out var leaderboardService) == false)
            {
                return LeaderboardScoreUploadResult.CreateFail(
                    errorMessage: $"Leaderboard {leaderboardId} is not created yet."
                );   
            }

            return await leaderboardService.UploadScoresAsync(
                ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest,
                score,
                details,
                cancellationToken
            );
#endif
            return LeaderboardScoreUploadResult.CreateFail(
                "Steamworks is not enabled."
            );
        }

        public int BooleanToDetail(bool b)
        {
            return b ? 1 : 0;
        }

        public bool DetailToBoolean(int i)
        {
            return i == 1 ? true : false;
        }

        public (int, int) DateTimeToDetails(DateTime t)
        {
            long binary = t.ToBinary();
            int high = (int)(binary >> 32);
            int low = (int)(binary & 0xffffffff);
            return (high, low);
        }

        public DateTime DetailsToTime(int high, int low)
        {
            long binary = ((long)high << 32) | (uint)low;
            return DateTime.FromBinary(binary);
        }

#if ENABLE_STEAMWORKS
        private async UniTask<(bool, int)> GetUserStatsIntAsync(
            CSteamID steamId, string statName, CancellationToken cancellationToken)
        {
            var accessor = GetAccessor(steamId);
            return await accessor.GetStatIntAsync(statName, cancellationToken);
        }

        private async UniTask<(bool, float)> GetUserStatsFloatAsync(
            CSteamID steamId, string statName, CancellationToken cancellationToken)
        {
            var accessor = GetAccessor(steamId);
            return await accessor.GetStatFloatAsync(statName, cancellationToken);
        }

        private IUserStatsAccessor GetAccessor(CSteamID steamId)
        {
            if (_userStatsAccessors.ContainsKey(steamId) == false)
            {
                _userStatsAccessors[steamId] = UserStatsAccessorFactory.Create(steamId);
            }

            return _userStatsAccessors[steamId];
        }

        private LeaderboardService GetOrCreateLeaderboardService(
            string leaderboardId, LeaderboardSortMethod sortMethod, LeaderboardDisplayType display)
        {
            if (_leaderboardServices.ContainsKey(leaderboardId) == false)
            {
                _leaderboardServices[leaderboardId] = LeaderboardServiceFactory.CreateLeaderboardService(
                    leaderboardId, sortMethod, display
                );
            }

            return _leaderboardServices[leaderboardId];
        }

        private bool TryGetLeaderboardService(string leaderboardId, out  LeaderboardService service)
        {
            return _leaderboardServices.TryGetValue(leaderboardId, out service);
        }
#endif

        public void Dispose()
        {
#if ENABLE_STEAMWORKS
            foreach (var accessor in _userStatsAccessors.Values)
            {
                accessor.Dispose();
            }
            _userStatsAccessors.Clear();

            foreach (var leaderboardService in _leaderboardServices.Values)
            {
                leaderboardService.Dispose();
            }
            _leaderboardServices.Clear();
#endif
        }
    }
}
