# Wiseye Studio Packages

Unity packages shared across Wiseye Studio projects, installable via UPM git URLs.

| Package | Description |
| --- | --- |
| [com.wiseyestudio.core](Packages/com.wiseyestudio.core/README.md) | Shared core utilities: VersionCode, DataProvider, Singleton, event bus and small helpers |
| [com.wiseyestudio.buildpipeline](Packages/com.wiseyestudio.buildpipeline/README.md) | ScriptableObject-based batch build pipeline with composable build methods |
| [com.wiseyestudio.steam](Packages/com.wiseyestudio.steam/README.md) | UniTask-based Steamworks.NET wrapper: init, stats, achievements, leaderboards, friends, avatars |

> [!WARNING]
> These packages are not well written or well maintained - We just released these packages for easier internal access. Wiseye Studio does not recommend other developers to depend on these packages.

## Installation

Install these packages via UPM by entering the git URL.

| Package | Description |
| --- | --- |
| [com.wiseyestudio.core](Packages/com.wiseyestudio.core/README.md) | https://github.com/wiseyestudio00/WiseyeStudioPackages.git?path=Packages/com.wiseyestudio.core |
| [com.wiseyestudio.buildpipeline](Packages/com.wiseyestudio.buildpipeline/README.md) | https://github.com/wiseyestudio00/WiseyeStudioPackages.git?path=Packages/com.wiseyestudio.buildpipeline |
| [com.wiseyestudio.steam](Packages/com.wiseyestudio.steam/README.md) | https://github.com/wiseyestudio00/WiseyeStudioPackages.git?path=Packages/com.wiseyestudio.steam |

To pin a specific release, append a tag to the URL, e.g. `...?path=Packages/com.wiseyestudio.steam#v0.1.2`.

### Prerequisites not resolved by UPM

UPM cannot auto-resolve git-based or Asset Store dependencies, so install these first where required:

- **UniTask** (`com.wiseyestudio.buildpipeline`, `com.wiseyestudio.steam`): `https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask`
- **Steamworks.NET** (`com.wiseyestudio.steam`): `https://github.com/rlabrecque/Steamworks.NET.git?path=/com.rlabrecque.steamworks.net`
- **Odin Inspector** (`com.wiseyestudio.core`, `com.wiseyestudio.buildpipeline`): paid asset, must be present in the project
- **com.wiseyestudio.core** must be installed before `com.wiseyestudio.buildpipeline`

See each package's README for details.