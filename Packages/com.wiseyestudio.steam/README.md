# Wiseye Studio Steamworks Package

A UniTask-based [Steamworks.NET](https://github.com/rlabrecque/Steamworks.NET) wrapper for Unity: initialization, stats, achievements, leaderboards, friends and avatars — exposed as awaitable, cancellable async APIs.

> [!WARNING]
> **Not recommended for use in other commercial projects (yet).**
> This package is built for Wiseye Studio's own games and is shared publicly for convenience. The code quality is not yet at a level we'd stand behind for third-party production use — there are known rough edges around object lifecycle/disposal, error handling and concurrency. Use it as a reference or at your own risk. This will improve in future releases.

## Why not just use other Steamworks C# wrappers?

There are two widely used Steamworks C# wrappers: [Facepunch.Steamworks](https://github.com/facepunch/facepunch.steamworks) and [Steamworks.NET](https://github.com/rlabrecque/steamworks.net).

They take opposite approaches. Steamworks.NET replicates Valve's original C++ API one to one, which means you work with raw callbacks to get results back from Steam's servers. Facepunch.Steamworks is more modern and more "the C# way": it hides those callbacks behind async/await.

For most users, Facepunch.Steamworks is the nicer choice — async/await makes most codes easier to write, and the result is cleaner and easier to maintain. But it has one important gap: it does not implement [`RequestUserStats`](https://partner.steamgames.com/doc/api/ISteamUserStats#RequestUserStats), so there is no way to read *another* user's stats, even though the official Steamworks API supports it. (For those who know not yet: Steam stats are optional values a game can store on Steam's servers — most games use them as a kind of remote save-data storage.)

Admittedly, this is a niche feature: the other user must explicitly allow others to read their stats in their account setting, and the game would need something to do with those stats. I can't think of a shipped game that uses this feature. However, in our rhythm game **[Lily Fantasia](https://store.steampowered.com/app/2527300/Lily_Fantasia/)**, we happen to need exactly this, so Facepunch.Steamworks was out. That left Steamworks.NET, which does support `RequestUserStats` but is as bare-bones as a wrapper can get — no async/await at all.

I wanted the modern convenience of Facepunch.Steamworks *and* `RequestUserStats`. That is why this package exists. I perosnally wrote this package to support Lily Fantasia, and later decide to clean up the code and publish this as a UPM package for ease of use for other potential projects, and as a way to motivate myself to keep practicing clean-code for the future.

As stated above: I do **NOT** recommend other developers - indie developers, studios, no matter - to actually use this package in their project. It is missing quite a lot of features from Steam API, as it only implements the features that Lily Fantasia needs.

NULL, Wiseye Studio

## Installation

Before you install this package, you have to manually add these two package via UPM (`add package via git url`).

```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
https://github.com/rlabrecque/Steamworks.NET.git?path=/com.rlabrecque.steamworks.net
```

You can know more about these two packages on their offical repos: [UniTask](https://github.com/Cysharp/UniTask) and [Steamworks.NET](https://github.com/rlabrecque/Steamworks.NET).

```
https://github.com/wiseyestudio00/SteamworksPackage.git?path=Packages/com.wiseyestudio.steam
```

This package is specified to support Unity 2022.3 or newer. However, it should work on older Unity versions, too.

## Getting started

1. Add a `SteamworksManager` component to a GameObject in your boot scene.
2. Implement `ISteamAppIdProvider` to supply your Steam App ID, and initialize:

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

3. Use the managers through `SteamworksManager.Instance`:

```csharp
var steam = SteamworksManager.Instance;
if (steam.Available)
{
    string name = steam.Player.GetName();
    await steam.Player.SetAchievementAsync("ACH_NAME", ct);
    await steam.UserStats.SubmitScoreAsync("leaderboard_id", score, details, ct);
    Sprite avatar = await steam.Friends.GetUserAvatarAsync(friendSteamId, ct);
}
```

Call `SteamworksManager.Instance.ShutDown()` when the application quits.

On non-standalone platforms the APIs still compile and return safe fallback values, so no platform guards are needed in calling code.

## License

[MIT](Packages/com.wiseyestudio.steam/LICENSE.md)
