# Wiseye Studio Steamworks

A UniTask-based [Steamworks.NET](https://github.com/rlabrecque/Steamworks.NET) wrapper for Unity: initialization, stats, achievements, leaderboards, friends and avatars — all exposed as awaitable, cancellable async APIs.

## Requirements

- Unity **2022.3** or newer
- [UniTask](https://github.com/Cysharp/UniTask)
- [Steamworks.NET](https://github.com/rlabrecque/Steamworks.NET)

> **Important:** UPM cannot automatically resolve git-based dependencies, so you must install UniTask and Steamworks.NET **before** (or together with) this package, otherwise it will not compile.

## Installation

Add all three entries to your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
    "com.rlabrecque.steamworks.net": "https://github.com/rlabrecque/Steamworks.NET.git?path=/com.rlabrecque.steamworks.net",
    "com.wiseyestudio.steam": "https://github.com/wiseyestudio00/WiseyeStudioPackages.git?path=Packages/com.wiseyestudio.steam"
  }
}
```

To pin a specific release, append a tag: `...?path=Packages/com.wiseyestudio.steam#v0.1.0`

Alternatively, in the Unity Editor: **Window ▸ Package Manager ▸ + ▸ Add package from git URL…** and paste each URL in the same order.

## Quick start

Add a `SteamworksManager` component to a GameObject in your boot scene, then initialize it with your App ID:

```csharp
using WiseyeStudio.Steam;

public class SteamAppIdProvider : ISteamAppIdProvider
{
    public uint GetAppId() => 480; // your Steam App ID
}

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private SteamworksManager steamworksManager;

    private void Awake()
    {
        steamworksManager.Initialize(
            restartAppIfNecessary: true,
            appIdProvider: new SteamAppIdProvider());
    }
}
```

Once initialized, everything is available through `SteamworksManager.Instance`:

```csharp
var steam = SteamworksManager.Instance;
if (steam.Available)
{
    // Player
    string name = steam.Player.GetName();
    Sprite avatar = await steam.Player.GetSteamAvatarAsSpriteAsync(ct);
    await steam.Player.SetStatAsync("stat_name", 42, ct);
    await steam.Player.StoreStatAsync(ct);
    await steam.Player.SetAchievementAsync("ACH_NAME", ct);

    // Leaderboards
    await steam.UserStats.IdempotenceCreateLeaderboardAsync(
        "leaderboard_id", LeaderboardSortMethod.Descending,
        LeaderboardDisplayType.Numeric, ct);
    await steam.UserStats.SubmitScoreAsync("leaderboard_id", score, details, ct);
    var top = await steam.UserStats.RequestGlobalLeaderboardTopScoresAsync(
        "leaderboard_id", count: 10, detailCount: 0, ct);

    // Friends
    string friendName = await steam.Friends.GetUserPersonaNameAsync(friendSteamId, ct);
    Sprite friendAvatar = await steam.Friends.GetUserAvatarAsync(friendSteamId, ct);
}
```

Call `SteamworksManager.Instance.ShutDown()` when the application quits.

## Platform support

Steam functionality is compiled in on standalone platforms (Windows, Linux, macOS). On other platforms the same APIs exist but return safe fallback values (`false`, `null`, empty strings), so calling code does not need platform guards.

## License

[MIT](LICENSE.md)
