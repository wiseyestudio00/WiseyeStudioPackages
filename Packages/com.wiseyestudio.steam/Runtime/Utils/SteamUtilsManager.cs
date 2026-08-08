#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
#define ENABLE_STEAMWORKS
#endif

using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;

#if ENABLE_STEAMWORKS
using Steamworks;
#endif

namespace WiseyeStudio.Steam.Utils
{
    public class SteamUtilsManager : IDisposable
    {
#if ENABLE_STEAMWORKS
        private readonly Callback<GamepadTextInputDismissed_t> _gamepadTextInputDismissedCallback;
        private readonly Callback<FloatingGamepadTextInputDismissed_t> _floatingGamepadTextInputDismissedCallback;

        private UniTaskCompletionSource<GamepadTextInputDismissed_t> _gamepadTextInputTcs;
        private UniTaskCompletionSource<FloatingGamepadTextInputDismissed_t> _floatingGamepadTextInputTcs;

        // The dismissed callbacks are global broadcasts, so only one dialog of each kind may be in flight.
        private readonly SemaphoreSlim _gamepadTextInputGate = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _floatingGamepadTextInputGate = new SemaphoreSlim(1, 1);
#endif

        // Canceled on Dispose to avoid hanging task
        private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();

        internal SteamUtilsManager()
        {
#if ENABLE_STEAMWORKS
            _gamepadTextInputDismissedCallback =
                Callback<GamepadTextInputDismissed_t>.Create(OnGamepadTextInputDismissed);
            _floatingGamepadTextInputDismissedCallback =
                Callback<FloatingGamepadTextInputDismissed_t>.Create(OnFloatingGamepadTextInputDismissed);
#endif
        }

        /// <summary>
        /// Whether the Steam Overlay needs a present. Only needed if you are using event driven
        /// rendering, otherwise ignore this.
        /// </summary>
        public bool OverlayNeedsPresent()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.BOverlayNeedsPresent();
#endif
            return false;
        }

        /// <summary>
        /// App ID of the current process.
        /// </summary>
        public uint GetAppId()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.GetAppID().m_AppId;
#endif
            return 0;
        }

        /// <summary>
        /// Current battery power as a percentage from 0 to 100, or 255 when on AC power.
        /// </summary>
        public byte GetCurrentBatteryPower()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.GetCurrentBatteryPower();
#endif
            return 255;
        }

        /// <summary>
        /// Two letter ISO 3166-1-alpha-2 country code the client is running in, e.g. "US" or "UK",
        /// looked up via an IP-to-location database.
        /// </summary>
        public string GetIpCountry()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.GetIPCountry();
#endif
            return string.Empty;
        }

        /// <summary>
        /// Number of IPC calls made since the last time this function was called.
        /// Only meant for debugging performance.
        /// </summary>
        public uint GetIpcCallCount()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.GetIPCCallCount();
#endif
            return 0;
        }

        /// <summary>
        /// Number of seconds since the application was active.
        /// </summary>
        public uint GetSecondsSinceAppActive()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.GetSecondsSinceAppActive();
#endif
            return 0;
        }

        /// <summary>
        /// Number of seconds since the user last moved the mouse.
        /// </summary>
        public uint GetSecondsSinceComputerActive()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.GetSecondsSinceComputerActive();
#endif
            return 0;
        }

        /// <summary>
        /// Steam server time as a Unix epoch timestamp.
        /// </summary>
        public uint GetServerRealTime()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.GetServerRealTime();
#endif
            return 0;
        }

        /// <summary>
        /// Language the Steam client is running in, e.g. "english".
        /// </summary>
        public string GetSteamUiLanguage()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.GetSteamUILanguage();
#endif
            return string.Empty;
        }

        /// <summary>
        /// Size of an image handle obtained from other Steam interfaces, such as avatars.
        /// </summary>
        public bool GetImageSize(int imageId, out uint width, out uint height)
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.GetImageSize(imageId, out width, out height);
#endif
            width = 0;
            height = 0;
            return false;
        }

        /// <summary>
        /// Copies the RGBA bytes of an image handle into the destination buffer, which must be
        /// at least width * height * 4 bytes as reported by <see cref="GetImageSize"/>.
        /// </summary>
        public bool GetImageRgba(int imageId, byte[] destination, int destinationSize)
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.GetImageRGBA(imageId, destination, destinationSize);
#endif
            return false;
        }

        /// <summary>
        /// Whether the Steam Overlay is enabled and ready to be used. Can be false during the
        /// first seconds after launch while the overlay hooks in.
        /// </summary>
        public bool IsOverlayEnabled()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.IsOverlayEnabled();
#endif
            return false;
        }

        /// <summary>
        /// Whether the game was launched through the Steam China launcher.
        /// </summary>
        public bool IsSteamChinaLauncher()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.IsSteamChinaLauncher();
#endif
            return false;
        }

        public bool IsSteamInBigPictureMode()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.IsSteamInBigPictureMode();
#endif
            return false;
        }

        public bool IsSteamRunningInVr()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.IsSteamRunningInVR();
#endif
            return false;
        }

        public bool IsSteamRunningOnSteamDeck()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.IsSteamRunningOnSteamDeck();
#endif
            return false;
        }

        /// <summary>
        /// Whether the HMD view is being streamed via Steam Remote Play.
        /// </summary>
        public bool IsVrHeadsetStreamingEnabled()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.IsVRHeadsetStreamingEnabled();
#endif
            return false;
        }

        /// <summary>
        /// Whether the HMD view should be streamed via Steam Remote Play.
        /// </summary>
        public void SetVrHeadsetStreamingEnabled(bool enabled)
        {
#if ENABLE_STEAMWORKS
            SteamUtils.SetVRHeadsetStreamingEnabled(enabled);
#endif
        }

        /// <summary>
        /// Inset of the overlay notification popup from the corner set by
        /// <see cref="SetOverlayNotificationPosition"/>, in pixels.
        /// </summary>
        public void SetOverlayNotificationInset(int horizontalInset, int verticalInset)
        {
#if ENABLE_STEAMWORKS
            SteamUtils.SetOverlayNotificationInset(horizontalInset, verticalInset);
#endif
        }

        /// <summary>
        /// Which corner the overlay notification popup should appear in.
        /// </summary>
        public void SetOverlayNotificationPosition(NotificationPosition position)
        {
#if ENABLE_STEAMWORKS
            SteamUtils.SetOverlayNotificationPosition(Convert(position));
#endif
        }

        /// <summary>
        /// In game launchers that don't have controller support, this enables Steam Input
        /// to translate gamepad input to mouse/keyboard so the user can navigate the launcher.
        /// </summary>
        public void SetGameLauncherMode(bool launcherMode)
        {
#if ENABLE_STEAMWORKS
            SteamUtils.SetGameLauncherMode(launcherMode);
#endif
        }

        /// <summary>
        /// Asks Steam to create and render the OpenVR dashboard.
        /// </summary>
        public void StartVrDashboard()
        {
#if ENABLE_STEAMWORKS
            SteamUtils.StartVRDashboard();
#endif
        }

        /// <summary>
        /// Opens a floating keyboard over the game content, sending OS keyboard keys directly
        /// to the game. The text field position and size are in pixels, relative to the origin
        /// of the game window, and are used to position the keyboard so it does not cover the field.
        /// The task completes when the keyboard is dismissed; false if it could not be shown.
        /// </summary>
        public async UniTask<bool> ShowFloatingGamepadTextInputAsync(
            FloatingGamepadTextInputMode keyboardMode,
            int textFieldX, int textFieldY, int textFieldWidth, int textFieldHeight,
            CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            using var linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);

            await _floatingGamepadTextInputGate.WaitAsync(linkedCts.Token);
            try
            {
                _floatingGamepadTextInputTcs = new UniTaskCompletionSource<FloatingGamepadTextInputDismissed_t>();

                if (SteamUtils.ShowFloatingGamepadTextInput(
                        Convert(keyboardMode), textFieldX, textFieldY, textFieldWidth, textFieldHeight) == false)
                {
                    return false;
                }

                await _floatingGamepadTextInputTcs.Task.AttachExternalCancellation(linkedCts.Token);
                return true;
            }
            finally
            {
                _floatingGamepadTextInputTcs = null;
                _floatingGamepadTextInputGate.Release();
            }
#endif
            return false;
        }

        /// <summary>
        /// Opens the Big Picture text input dialog and completes when the user dismisses it.
        /// Only works in Big Picture Mode. Returns the entered text, or null when the user
        /// canceled or the dialog could not be shown.
        /// </summary>
        public async UniTask<string> ShowGamepadTextInputAsync(
            GamepadTextInputMode inputMode, GamepadTextInputLineMode lineInputMode,
            string description, uint maxCharacters, string existingText,
            CancellationToken cancellationToken)
        {
#if ENABLE_STEAMWORKS
            using var linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);

            await _gamepadTextInputGate.WaitAsync(linkedCts.Token);
            try
            {
                _gamepadTextInputTcs = new UniTaskCompletionSource<GamepadTextInputDismissed_t>();

                if (SteamUtils.ShowGamepadTextInput(
                        Convert(inputMode), Convert(lineInputMode), description, maxCharacters,
                        existingText) == false)
                {
                    return null;
                }

                var dismissed = await _gamepadTextInputTcs.Task.AttachExternalCancellation(linkedCts.Token);
                if (dismissed.m_bSubmitted == false) return null;

                return SteamUtils.GetEnteredGamepadTextInput(
                    out var text, SteamUtils.GetEnteredGamepadTextLength() + 1)
                    ? text
                    : null;
            }
            finally
            {
                _gamepadTextInputTcs = null;
                _gamepadTextInputGate.Release();
            }
#endif
            return null;
        }

        /// <summary>
        /// Initializes text filtering, loading the dictionaries for the language the game is
        /// running in. Must be called before <see cref="FilterText"/>.
        /// Returns false if filtering is unavailable for the language, in which case
        /// <see cref="FilterText"/> only performs legally required filtering.
        /// </summary>
        public bool InitFilterText()
        {
#if ENABLE_STEAMWORKS
            return SteamUtils.InitFilterText();
#endif
            return false;
        }

        /// <summary>
        /// Filters the input message and returns the filtered result, censoring or removing
        /// text according to the context and the user's settings.
        /// </summary>
        /// <param name="context">What the text is used for.</param>
        /// <param name="sourceSteamId">Steam ID of the user the text originates from, or 0 if not applicable.</param>
        /// <param name="inputMessage">The text to filter.</param>
        public string FilterText(TextFilteringContext context, ulong sourceSteamId, string inputMessage)
        {
#if ENABLE_STEAMWORKS
            if (string.IsNullOrEmpty(inputMessage)) return inputMessage;

            // The filtered text can only be as long as or shorter than the input
            var bufferSize = (uint)Encoding.UTF8.GetByteCount(inputMessage) + 1;
            SteamUtils.FilterText(
                Convert(context), new CSteamID(sourceSteamId), inputMessage, out var filteredText, bufferSize);
            return filteredText;
#endif
            return inputMessage;
        }

        public void Dispose()
        {
            _disposeCts.Cancel();
            _disposeCts.Dispose();

#if ENABLE_STEAMWORKS
            _gamepadTextInputDismissedCallback?.Dispose();
            _floatingGamepadTextInputDismissedCallback?.Dispose();

            // The gates are intentionally not disposed: SemaphoreSlim only
            // holds resources once AvailableWaitHandle is used (we never do),
            // and a canceled waiter's finally block may still Release() after
            // this point, which would throw on a disposed semaphore.
#endif
        }

#if ENABLE_STEAMWORKS
        private void OnGamepadTextInputDismissed(GamepadTextInputDismissed_t dismissed)
        {
            _gamepadTextInputTcs?.TrySetResult(dismissed);
        }

        private void OnFloatingGamepadTextInputDismissed(FloatingGamepadTextInputDismissed_t dismissed)
        {
            _floatingGamepadTextInputTcs?.TrySetResult(dismissed);
        }

        private static ENotificationPosition Convert(NotificationPosition position)
        {
            return position switch
            {
                NotificationPosition.TopLeft => ENotificationPosition.k_EPositionTopLeft,
                NotificationPosition.TopRight => ENotificationPosition.k_EPositionTopRight,
                NotificationPosition.BottomLeft => ENotificationPosition.k_EPositionBottomLeft,
                NotificationPosition.BottomRight => ENotificationPosition.k_EPositionBottomRight,
                _ => throw new ArgumentException($"Unknown NotificationPosition: {position}")
            };
        }

        private static EFloatingGamepadTextInputMode Convert(FloatingGamepadTextInputMode keyboardMode)
        {
            return keyboardMode switch
            {
                FloatingGamepadTextInputMode.SingleLine =>
                    EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine,
                FloatingGamepadTextInputMode.MultipleLines =>
                    EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeMultipleLines,
                FloatingGamepadTextInputMode.Email =>
                    EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeEmail,
                FloatingGamepadTextInputMode.Numeric =>
                    EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeNumeric,
                _ => throw new ArgumentException($"Unknown FloatingGamepadTextInputMode: {keyboardMode}")
            };
        }

        private static EGamepadTextInputMode Convert(GamepadTextInputMode inputMode)
        {
            return inputMode switch
            {
                GamepadTextInputMode.Normal => EGamepadTextInputMode.k_EGamepadTextInputModeNormal,
                GamepadTextInputMode.Password => EGamepadTextInputMode.k_EGamepadTextInputModePassword,
                _ => throw new ArgumentException($"Unknown GamepadTextInputMode: {inputMode}")
            };
        }

        private static EGamepadTextInputLineMode Convert(GamepadTextInputLineMode lineInputMode)
        {
            return lineInputMode switch
            {
                GamepadTextInputLineMode.SingleLine => EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine,
                GamepadTextInputLineMode.MultipleLines =>
                    EGamepadTextInputLineMode.k_EGamepadTextInputLineModeMultipleLines,
                _ => throw new ArgumentException($"Unknown GamepadTextInputLineMode: {lineInputMode}")
            };
        }

        private static ETextFilteringContext Convert(TextFilteringContext context)
        {
            return context switch
            {
                TextFilteringContext.Unknown => ETextFilteringContext.k_ETextFilteringContextUnknown,
                TextFilteringContext.GameContent => ETextFilteringContext.k_ETextFilteringContextGameContent,
                TextFilteringContext.Chat => ETextFilteringContext.k_ETextFilteringContextChat,
                TextFilteringContext.Name => ETextFilteringContext.k_ETextFilteringContextName,
                _ => throw new ArgumentException($"Unknown TextFilteringContext: {context}")
            };
        }
#endif
    }
}
