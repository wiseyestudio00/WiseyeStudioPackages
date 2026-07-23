# WiseyeStudio Core

Shared core utilities for WiseyeStudio projects.

## Contents

- `WiseyeStudio.Core` (Runtime)
  - `VersionCode` / `VersionCodeConvertor` — semantic version struct used by the build pipeline and in-game version display
  - `DataProvider` — `IDataProvider<T>` and basic string/string-list providers (ScriptableObject and inline serializable)
  - `OpenDirectoryHelper` — open a folder in the OS file browser
  - `GameEventBus` — type-keyed publish/subscribe event bus (DontDestroyOnLoad singleton)
  - `Singleton/` — MonoBehaviour singleton base classes
  - `Utilities/` — `EasingFunctions`, `FloatUtility` (culture-invariant parsing), `TimeUtility`, `ExponentialLerpUtility`, `FolderReference` (editor-only folder asset reference)

## Requirements

- **Odin Inspector** (paid asset, not included) must be present in the project.

## Installation

Add via git URL (install order: this package before packages that depend on it):

```
https://github.com/wiseyestudio00/WiseyeStudioPackages.git?path=Packages/com.wiseyestudio.core
```

Or embed the repository's `Packages/com.wiseyestudio.core` folder into your project's `Packages/` directory for local development.
