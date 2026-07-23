#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

#if ENABLE_STEAMWORKS
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Steamworks;

namespace WiseyeStudio.Steam
{
    /// <summary>
    /// Converts a Steam API call handle into an awaitable UniTask.
    /// Each call gets its own CallResult and completion source, so concurrent
    /// operations cannot interfere with each other
    /// </summary>
    internal static class SteamCallResultExtensions
    {
        internal static async UniTask<T> ToUniTask<T>(this SteamAPICall_t handle, CancellationToken cancellationToken)
            where T : struct
        {
            var tcs = new UniTaskCompletionSource<T>();
            var callResult = CallResult<T>.Create((result, bIOFailure) =>
            {
                if (bIOFailure)
                {
                    tcs.TrySetException(new Exception($"Steam IO failure while awaiting {typeof(T).Name}"));
                }
                else
                {
                    tcs.TrySetResult(result);
                }
            });
            callResult.Set(handle);

            try
            {
                return await tcs.Task.AttachExternalCancellation(cancellationToken);
            }
            finally
            {
                callResult.Dispose();
            }
        }
    }
}
#endif
