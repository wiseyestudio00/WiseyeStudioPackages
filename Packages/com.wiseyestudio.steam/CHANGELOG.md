# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
