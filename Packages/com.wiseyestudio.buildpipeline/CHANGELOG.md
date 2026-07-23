# Changelog

## [0.2.0] - 2026-07-24

- **Breaking:** removed `BuildMethodSetVersionCodeScriptableObject` (empty no-op; the pipeline sets the version code itself). Delete any "Set Version Code" assets and their build-method list entries when updating, or builds will throw on the null entry.
- **Breaking:** renamed `ABuildPipelineSetVersionCodeMethodscriptableObject` to `ABuildPipelineSetVersionCodeMethodScriptableObject`.
- Added `BuildPipelineAssetMenuConstants`: central menu paths, orders and fileName prefix for all `CreateAssetMenu` attributes. Menu root changed from `WiseyeStudio/` to `Wiseye Studio/`; build methods are now explicitly ordered and grouped with separators.
- Renamed all build-method serialized fields to camelCase (underscore-prefixed for Odin-serialized fields); values are preserved via `FormerlySerializedAs`.
- `ExecuteShellScript` is now `protected static` (was `public`).
- Removed unused using directives; requires core >= 0.3.0.

## [0.1.1] - 2026-07-23

- Updated to the renamed `VersionCode` fields from com.wiseyestudio.core 0.2.0; now requires core >= 0.2.0.

## [0.1.0] - 2026-07-23

- Initial extraction from Project Lily: pipeline runner, 19 built-in build methods, Apple/Steam account data types, Addressables editor data providers.
- Script GUIDs preserved from Project Lily so existing pipeline assets keep their references.
- Left in Lily (project-specific): StaticBuildMethods entry points, BuildAssetProcess, ClearFallbackFontAssignerAtlas, Google Drive upload.
