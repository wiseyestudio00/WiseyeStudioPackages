# Changelog

## [0.1.0] - 2026-10-11

- Initial release, rewritten from Project Lily's localization code rather than moved, so there are no GUID or type-name ties to Lily.
- `LocalizationTableFromGoogleSheetScriptableObject`: holds a CSV and syncs it from a Google Sheet tab.
- `LocalizationTable`: the parsed table; both implement `ILocalizationTable`.
- `LocalizationSettings`: project-wide language list and fallback language.
- `LocalizationManager`: current language and `LanguageChanged` event.
- `ILocalizedValue<T>` with `LocalizedValue<T>` (typed in per language) and `LocalizedStringFromTable` (a table key), meant to be Odin-serialized.
- `LocalizationTextTag` TextMeshPro component, showing any `ILocalizedValue<string>`.
- Inspectors use Odin Inspector attributes; Odin is required.
