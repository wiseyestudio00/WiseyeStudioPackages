#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

#if ENABLE_STEAMWORKS
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace WiseyeStudio.Steam.UserStats
{
    /// <summary>
    /// Interface for accessing Steam user stats and achievements.
    /// </summary>
    internal interface IUserStatsAccessor : IDisposable
    {
        /// <summary>
        /// Gets an integer stat value for this user.
        /// </summary>
        /// <param name="statName">The name of the stat to retrieve</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>The stat value, or 0 if not found</returns>
        UniTask<(bool, int)> GetStatIntAsync(string statName, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a float stat value for this user.
        /// </summary>
        /// <param name="statName">The name of the stat to retrieve</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>The stat value, or 0f if not found</returns>
        UniTask<(bool, float)> GetStatFloatAsync(string statName, CancellationToken cancellationToken);
        
        /// <summary>
        /// Forces a reload of stats from Steam servers on the next access.
        /// </summary>
        void InvalidateCache();
    }
}
#endif