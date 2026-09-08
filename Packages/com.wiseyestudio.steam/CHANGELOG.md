# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.3.3] - 2026-09-08

### Fixed

- Concurrent requests against a leaderboard whose handle is not cached yet now issue a single
  `FindOrCreateLeaderboard` round trip instead of one per request, so a cold parallel pair (for
  example a page request and a player-rank request) costs the same as a cold single request.

## [0.3.2] - 2026-08-18

### Added

- `SteamFriendsManager.GameOverlayActivated`: event raised when the Steam Overlay opens (true) or
  closes (false), wrapping the `GameOverlayActivated_t` callback. Cleared on shutdown with the
  other manager state.

## [0.3.1] - 2026-08-09

### Added

- `SteamFriendsManager.OpenStorePageInOverlay`: opens the Steam Overlay on the store page of a
  given app (e.g. a DLC), wrapping `ISteamFriends::ActivateGameOverlayToStore`. No-op when the
  overlay is disabled; callers should check `Utils.IsOverlayEnabled()` and fall back to a
  `steam://url/StoreAppPage/` URL.

## [0.3.0] - 2026-08-09

### Added

- `SteamUtilsManager`, exposed as `SteamworksManager.Instance.Utils` and disposed with the other
  managers on shutdown. It wraps the callback-free parts of ISteamUtils: app/session info
  (`GetAppId`, `GetServerRealTime`, `GetIpCountry`, `GetSteamUiLanguage`, `GetSecondsSinceAppActive`,
  `GetSecondsSinceComputerActive`, `GetCurrentBatteryPower`, `GetIpcCallCount`), environment checks
  (`IsOverlayEnabled`, `OverlayNeedsPresent`, `IsSteamChinaLauncher`, `IsSteamInBigPictureMode`,
  `IsSteamRunningInVr`, `IsSteamRunningOnSteamDeck`, `IsVrHeadsetStreamingEnabled`), overlay, VR and
  launcher control (`SetOverlayNotificationPosition`, `SetOverlayNotificationInset`,
  `SetVrHeadsetStreamingEnabled`, `StartVrDashboard`, `SetGameLauncherMode`), image helpers
  (`GetImageSize`, `GetImageRgba`) and text filtering (`InitFilterText`, `FilterText`).
- Awaitable gamepad text input: `ShowGamepadTextInputAsync` opens the Big Picture text input dialog
  and returns the entered text (or null on cancel), `ShowFloatingGamepadTextInputAsync` opens the
  floating keyboard and completes when it is dismissed. Both are cancellable and resolve through the
  Steam dismissed callbacks internally.

## [0.2.0] - 2026-08-08

### Added

- `SteamAppsManager`, exposed as `SteamworksManager.Instance.Apps` and disposed with the other
  managers on shutdown. It wraps the ownership and installation parts of ISteamApps:
  `IsAppInstalled`, `IsDlcInstalled`, `IsSubscribed` (for the current app and for a given app),
  `IsSubscribedFromFamilySharing`, `IsSubscribedFromFreeWeekend`, `IsLowViolence`, `IsVacBanned`
  and `GetAppOwnerSteamId`.

## [0.1.2] - 2026-07-22

### Fixed

- Friend avatars are now cached per user: repeated `GetUserAvatarAsync` calls reuse the loaded `Sprite` instead of allocating a new `Texture2D` and `Sprite` on every call, which leaked GPU memory (engine objects are not garbage collected). Concurrent first requests are deduplicated so the avatar downloads only once.
- Cached avatar sprites and their textures (friend avatars and the local player's) are now destroyed on shutdown.

## [0.1.1] - 2026-07-22

### Fixed

- `SteamworksManager` now shuts down Steam and disposes all managers automatically in `OnDestroy`; `ShutDown()` is idempotent and safe to call manually.
- Operations still in flight at shutdown now resolve as cancelled or failed instead of awaiting callbacks that can no longer fire.
- Leaderboard and remote-stats operations use a dedicated `CallResult` per Steam API call, removing races between overlapping requests (crossed results, orphaned awaits, permanently stuck operations).
- `StoreStatAsync` and persona-information fetches are serialized with a semaphore gate; a failed `StoreStats` call no longer blocks all future stores.
- Fixed a `NullReferenceException` in `SteamPlayerManager.Dispose` when no store operation was in flight.
- Initializing twice in one session is now rejected instead of re-running initialization.
- `SteamAvatarLoader` registers its Steam callback lazily and returns null when Steam is unavailable instead of calling Steam APIs regardless.

### Removed

- Internal frame-polling wait utility (`UniTaskCompletionSourceUtility`), superseded by the per-call and gated patterns above.

## [0.1.0] - 2026-07-21

### Added

- `SteamworksManager`: Steam API initialization (with optional `RestartAppIfNecessary`), callback pump and shutdown.
- `ISteamAppIdProvider` for injecting the Steam App ID from the hosting game.
- `SteamPlayerManager`: local player identity (Steam ID, persona name, logged-on state), avatar loading as `Sprite`, int/float stats, stat storing and achievement unlocking.
- `SteamUserStatsManager`: remote user stats access and leaderboard workflows (find-or-create, submit score, global/friends/user entry requests) with detail encoding helpers.
- `SteamFriendsManager`: persona name and avatar retrieval for arbitrary users.
- All async operations are UniTask-based and cancellable.
- Safe no-op fallbacks on non-standalone platforms.
