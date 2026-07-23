# Changelog

## [0.3.0] - 2026-07-24

- Added `AssetMenuConstants`: central menu paths and orders for `CreateAssetMenu`. The asset-creation menu root changed from `WiseyeStudio/` to `Wiseye Studio/`.
- Removed unused using directives across the package.

## [0.2.0] - 2026-07-23

- **Breaking:** renamed `VersionCode` public fields to camelCase (`Major`→`major`, `Minor`→`minor`, `SubVersion`→`subVersion`, `BuildNumber`→`buildNumber`, `Extra`→`extra`) and constants to PascalCase (`MAX_MAJOR`→`MaxMajor`, etc.). Serialized data is preserved via `FormerlySerializedAs`, but consuming code must update field references.

## [0.1.0] - 2026-07-23

- Initial extraction from Project Lily: VersionCode, VersionCodeConvertor, DataProvider family, OpenDirectoryHelper, Singleton (own asmdef).
- Script GUIDs preserved from Project Lily so existing serialized assets keep their references.
