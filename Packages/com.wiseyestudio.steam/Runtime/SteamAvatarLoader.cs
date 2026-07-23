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
    internal class SteamAvatarLoader : IDisposable
    {
#if ENABLE_STEAMWORKS
        private readonly CSteamID _steamId;
        private UniTaskCompletionSource<int> _imageIdTcs;
        private Callback<AvatarImageLoaded_t> _avatarImageLoadedCallback;
#endif

#if ENABLE_STEAMWORKS
        internal SteamAvatarLoader(CSteamID steamId)
        {
            _steamId = steamId;
        }
#endif

        internal async UniTask<Sprite> LoadPfpAsync(CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            if (TryLazyInitializeCallback() == false)
            {
                // Steam is not available, so there is no avatar to load.
                return null;
            }

            _imageIdTcs = new UniTaskCompletionSource<int>();

            var imageId = SteamFriends.GetLargeFriendAvatar(_steamId);
            switch (imageId)
            {
                case 0:
                    // 0 means there is no avatar to load.
                    return null;
                case -1:
                    imageId = await _imageIdTcs.Task.AttachExternalCancellation(cancellationToken);
                    break;
                default:
                    // otherwise imageId is already ready to be read
                    break;
            }

            if (SteamUtils.GetImageSize(imageId, out var width, out var height) == false)
            {
                Debug.LogError($"Failed to get avatar image size: {imageId}");
                return null;
            }

            var imageByteArray = new byte[width * height * 4]; // RGBA (4 bytes per pixel)
            if (SteamUtils.GetImageRGBA(imageId, imageByteArray, (int)(4 * width * height)) == false)
            {
                Debug.LogError($"Failed to get avatar image RGBA data: {imageId}");
                return null;
            }

            var texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Trilinear
            };

            // Flip the image vertically since Steam provides the data upside down
            var flippedImageData = new byte[imageByteArray.Length];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int sourceIndex = (int)((y * width + x) * 4);
                    int destIndex = (int)(((height - 1 - y) * width + x) * 4);

                    flippedImageData[destIndex] = imageByteArray[sourceIndex];
                    flippedImageData[destIndex + 1] = imageByteArray[sourceIndex + 1];
                    flippedImageData[destIndex + 2] = imageByteArray[sourceIndex + 2];
                    flippedImageData[destIndex + 3] = imageByteArray[sourceIndex + 3];
                }
            }

            texture.LoadRawTextureData(flippedImageData);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
#endif
            return null;
        }

        private bool TryLazyInitializeCallback()
        {
#if ENABLE_STEAMWORKS
            if (_avatarImageLoadedCallback != null) return true;

            var manager = SteamworksManager.Instance;
            if (manager == null || manager.Available == false) return false;

            _avatarImageLoadedCallback = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
            return true;
#endif
            return false;
        }

#if ENABLE_STEAMWORKS
        private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
        {
            if (callback.m_steamID != _steamId) return;
            _imageIdTcs?.TrySetResult(callback.m_iImage);
        }
#endif

        public void Dispose()
        {
#if ENABLE_STEAMWORKS
            _imageIdTcs?.TrySetCanceled();
            _imageIdTcs = null;

            _avatarImageLoadedCallback?.Dispose();
#endif
        }
    }
}
