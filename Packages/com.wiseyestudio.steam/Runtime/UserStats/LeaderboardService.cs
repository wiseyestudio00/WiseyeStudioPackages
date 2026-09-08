#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

#if ENABLE_STEAMWORKS
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Steamworks;
using UnityEngine;

namespace WiseyeStudio.Steam.UserStats
{
    internal class LeaderboardService : IDisposable
    {
        private readonly string _leaderboardId;
        private readonly ELeaderboardSortMethod _sortMethod;
        private readonly ELeaderboardDisplayType _displayType;

        private SteamLeaderboard_t? _leaderboard;
        // Ensures concurrent cold requests issue only one FindOrCreateLeaderboard round trip.
        private readonly SemaphoreSlim _findGate = new SemaphoreSlim(1, 1);
        // Canceled on Dispose to avoid hanging task
        private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();

        internal LeaderboardService(
            string leaderboardId,
            ELeaderboardSortMethod sortMethod,
            ELeaderboardDisplayType displayType)
        {
            _leaderboardId = leaderboardId;
            _sortMethod = sortMethod;
            _displayType = displayType;

            if (_sortMethod == ELeaderboardSortMethod.k_ELeaderboardSortMethodNone)
            {
                throw new NotSupportedException(
                    $"_sortMethod can not be {ELeaderboardSortMethod.k_ELeaderboardSortMethodNone}");
            }

            if (_displayType == ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNone)
            {
                throw new NotSupportedException(
                    $"_displayType can not be {ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNone}");
            }
        }

        internal async UniTask<bool> CreateOrFindLeaderboardAsync(CancellationToken cancellationToken)
        {
            if (_leaderboard.HasValue) return true;

            try
            {
                using var linkedCts =
                    CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);

                // Concurrent cold callers queue here; all but the first find the handle already cached.
                await _findGate.WaitAsync(linkedCts.Token);
                try
                {
                    if (_leaderboard.HasValue) return true;

                    var handle = SteamUserStats.FindOrCreateLeaderboard(_leaderboardId, _sortMethod, _displayType);
                    var callback = await handle.ToUniTask<LeaderboardFindResult_t>(linkedCts.Token);

                    if (callback.m_bLeaderboardFound == 1)
                    {
                        Debug.Log($"Successfully found/created leaderboard: {_leaderboardId}");
                        _leaderboard = callback.m_hSteamLeaderboard;
                        return true;
                    }

                    Debug.LogError($"Failed to find/create leaderboard: {_leaderboardId}");
                    return false;
                }
                finally
                {
                    _findGate.Release();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Finding/creating leaderboard was cancelled: {_leaderboardId}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to find/create leaderboard {_leaderboardId}: {e.Message}");
                return false;
            }
        }

        internal async UniTask<LeaderboardScoreUploadResult> UploadScoresAsync(
            ELeaderboardUploadScoreMethod updateScoreMethod,
            int score,
            int[] details,
            CancellationToken cancellationToken)
        {
            if (await CreateOrFindLeaderboardAsync(cancellationToken) == false)
            {
                return LeaderboardScoreUploadResult.CreateFail("Leaderboard is not found/fetched.");
            }

            try
            {
                using var linkedCts =
                    CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);
                var handle = SteamUserStats.UploadLeaderboardScore(
                    _leaderboard.Value,
                    updateScoreMethod,
                    score,
                    details,
                    details.Length);
                var callback = await handle.ToUniTask<LeaderboardScoreUploaded_t>(linkedCts.Token);

                if (callback.m_bSuccess == 0)
                {
                    return LeaderboardScoreUploadResult.CreateFail(
                        "Leaderboard score uploaded failed."
                    );
                }

                return LeaderboardScoreUploadResult.CreateSuccess(
                    callback.m_nScore,
                    callback.m_bScoreChanged == 1,
                    callback.m_nGlobalRankNew,
                    callback.m_nGlobalRankPrevious
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to upload score to leaderboard: {e}");
                return LeaderboardScoreUploadResult.CreateFail(
                    $"An exception happened when uploading score to leaderboard: {e}"
                );
            }
        }

        internal async UniTask<LeaderboardEntriesRequestResult> RequestLeaderboardEntriesAsync(
            ELeaderboardDataRequest dataRequest,
            int startIndex,
            int length,
            int detailCount,
            CancellationToken cancellationToken)
        {
            var downloadResult = await DownloadLeaderboardEntriesAsync(dataRequest, startIndex, length, cancellationToken);
            return DownloadResultToRequestResult(downloadResult, detailCount);
        }

        internal async UniTask<LeaderboardEntriesRequestResult> RequestLeaderboardEntriesAsync(
            List<CSteamID> users,
            int detailCount,
            CancellationToken cancellationToken)
        {
            var downloadResult = await DownloadLeaderboardEntriesAsync(users, cancellationToken);
            return DownloadResultToRequestResult(downloadResult, detailCount);
        }

        private static LeaderboardEntriesRequestResult DownloadResultToRequestResult(
            LeaderboardEntriesDownloadResult downloadResult, int detailCount)
        {
            if (downloadResult.Success == false)
            {
                return LeaderboardEntriesRequestResult.CreateFailure(
                    "Leaderboard entries download failed."
                );
            }

            List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
            for (int i = 0; i < downloadResult.Count; ++i)
            {
                int[] details = new int[detailCount];
                SteamUserStats.GetDownloadedLeaderboardEntry(
                    downloadResult.EntriesHandle,
                    i,
                    out var entryT,
                    details,
                    detailCount);

                var entry = new LeaderboardEntry(
                    entryT.m_steamIDUser.m_SteamID,
                    entryT.m_nGlobalRank,
                    entryT.m_nScore,
                    details);
                entries.Add(entry);
            }

            return LeaderboardEntriesRequestResult.CreateSuccess(entries);
        }

        private async UniTask<LeaderboardEntriesDownloadResult> DownloadLeaderboardEntriesAsync(
            ELeaderboardDataRequest dataRequest,
            int startIndex,
            int length,
            CancellationToken cancellationToken)
        {
            if (await CreateOrFindLeaderboardAsync(cancellationToken) == false)
            {
                Debug.LogError("Leaderboard is not found/fetched, so can not be downloaded.");
                return LeaderboardEntriesDownloadResult.CreateFail();
            }

            try
            {
                using var linkedCts =
                    CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);
                int endIndex = startIndex + length - 1;
                var handle = SteamUserStats.DownloadLeaderboardEntries(
                    _leaderboard.Value,
                    dataRequest,
                    startIndex,
                    endIndex);
                var callback = await handle.ToUniTask<LeaderboardScoresDownloaded_t>(linkedCts.Token);

                return LeaderboardEntriesDownloadResult.CreateSuccess(
                    callback.m_hSteamLeaderboardEntries, callback.m_cEntryCount);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error downloading leaderboard: {e}");
                return LeaderboardEntriesDownloadResult.CreateFail();
            }
        }

        private async UniTask<LeaderboardEntriesDownloadResult> DownloadLeaderboardEntriesAsync(
            List<CSteamID> users,
            CancellationToken cancellationToken)
        {
            if (await CreateOrFindLeaderboardAsync(cancellationToken) == false)
            {
                Debug.LogError("Leaderboard is not found/fetched, so can not be downloaded.");
                return LeaderboardEntriesDownloadResult.CreateFail();
            }

            try
            {
                using var linkedCts =
                    CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);
                var handle = SteamUserStats.DownloadLeaderboardEntriesForUsers(
                    _leaderboard.Value,
                    users.ToArray(),
                    users.Count
                );
                var callback = await handle.ToUniTask<LeaderboardScoresDownloaded_t>(linkedCts.Token);

                return LeaderboardEntriesDownloadResult.CreateSuccess(
                    callback.m_hSteamLeaderboardEntries, callback.m_cEntryCount);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error downloading leaderboard: {e}");
                return LeaderboardEntriesDownloadResult.CreateFail();
            }
        }

        public void Dispose()
        {
            _disposeCts.Cancel();
            _disposeCts.Dispose();
        }

        /// <summary>
        /// Different from LeaderboardEntriesRequestResult
        /// </summary>
        private class LeaderboardEntriesDownloadResult
        {
            public readonly bool Success;
            public readonly SteamLeaderboardEntries_t EntriesHandle;
            public readonly int Count;

            private LeaderboardEntriesDownloadResult(SteamLeaderboardEntries_t entriesHandle, int count)
            {
                Success = true;
                EntriesHandle = entriesHandle;
                Count = count;
            }

            private LeaderboardEntriesDownloadResult()
            {
                Success = false;
                EntriesHandle = default;
                Count = 0;
            }

            internal static LeaderboardEntriesDownloadResult CreateSuccess(
                SteamLeaderboardEntries_t entriesHandle, int count)
            {
                return new LeaderboardEntriesDownloadResult(entriesHandle, count);
            }

            internal static LeaderboardEntriesDownloadResult CreateFail()
            {
                return new  LeaderboardEntriesDownloadResult();
            }
        }
    }
}
#endif
