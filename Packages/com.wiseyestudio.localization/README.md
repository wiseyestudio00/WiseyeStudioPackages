# Wiseye Studio Localization

CSV-based localization for Unity: string tables synced from Google Sheets, per-language values, a current-language manager and a TextMeshPro text component.

> [!WARNING]
> Built for Wiseye Studio's own games. Use it as a reference or at your own risk.

## Installation

Install the prerequisites first (UPM can't resolve git dependencies):

```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
https://github.com/wiseyestudio00/WiseyeStudioPackages.git?path=Packages/com.wiseyestudio.core
```

[Odin Inspector](https://odininspector.com/) (paid asset) must also be in the project.

Then the package:

```
https://github.com/wiseyestudio00/WiseyeStudioPackages.git?path=Packages/com.wiseyestudio.localization
```

## Settings

Create one with **Create › Wiseye Studio › Localization › Localization Settings** and keep it at `Assets/Resources/WiseyeStudio/LocalizationSettings.asset` (it's loaded from that Resources path, in edit mode too).

| Field | Default | Used for |
| --- | --- | --- |
| Languages | English, ChineseTraditional, ChineseSimplified, Japanese, Korean | The supported languages, and the column order of tables without a header row |
| Fallback Language | English | Used when a translation is missing, and when the system language isn't supported |

Without the asset, the package logs a warning and uses the defaults.

## LocalizationManager

Add a `LocalizationManager` to a GameObject in your boot scene. It's a `DontDestroyOnLoad` singleton.

```csharp
var manager = LocalizationManager.Instance;
manager.SetLanguage(SystemLanguage.Japanese);
var language = manager.CurrentLanguage;
manager.LanguageChanged += OnLanguageChanged;
var settings = manager.Settings;
```

`CurrentLanguage` starts as the system language if it's supported, otherwise the fallback language. The manager doesn't save the player's choice; call `SetLanguage` from your own settings code at startup.

Lookups that don't take a language use `CurrentLanguage`. Without a manager in the scene they log an error and use English.

## Tables

Create one with **Create › Wiseye Studio › Localization › Localization Table (Google Sheet)** (`LocalizationTableFromGoogleSheetScriptableObject`).

1. Paste the sheet tab's URL into the **Set Sheet URL** button's field and press it. This fills in the sheet ID and gid.
2. Press **Sync**. The tab is downloaded into the assigned CSV, or into a new `.csv` next to the table if none is assigned.
3. Check the Console for the sync report: key count, missing translations per language, duplicate keys and unknown columns. The inspector's Preview section shows the key count, columns and any problems.

To sync many tables, select tables or folders in the Project window and use **Wiseye Studio › Localization › Sync Localization Tables** from the right-click menu. From code, call `await table.SyncAsync()` (editor only).

The sheet must be shared as **Anyone with the link can view**. The export URL has no sign-in, so a private sheet returns a login page and the sync fails.

```csharp
[SerializeField] private LocalizationTableFromGoogleSheetScriptableObject table;

var text = table.GetString("back_button");
var japanese = table.GetString("back_button", SystemLanguage.Japanese);
var replaced = table.ReplaceKeyedTags("{perfect}+");
```

Both the asset and the parsed `LocalizationTable` it caches implement `ILocalizationTable`. `LocalizationTable.Parse(csv, hasHeaderRow)` builds one straight from CSV text.

### CSV format

The first column is the key. With **Has Header Row** on, row 1 names the other columns with `SystemLanguage` names, in any order:

```
key,English,ChineseTraditional,ChineseSimplified,Japanese,Korean
back_button,Back,返回,返回,戻る,뒤로
```

With it off, the file starts with data and the columns follow the language order in the settings.

- Rows with an empty key are skipped.
- An empty cell falls back to the fallback language. If that is empty too, or the key isn't in the table, `GetString` returns the key itself, so missing text shows up on screen instead of going blank.
- `\n` and `\t` written literally in a cell become a line break and a tab.
- `{key}` in text passed to `ReplaceKeyedTags` is replaced with that key's string. Unknown keys are left as they are.

## Localized values

`ILocalizedValue<T>` is anything that returns a value for the current language (`Get()`) or a given one (`Get(language)`). Two implementations ship:

| Type | Implements | Source |
| --- | --- | --- |
| `LocalizedValue<T>` | `ILocalizedValue<T>` | Values typed in per language, edited as a language/value table |
| `LocalizedStringFromTable` | `ILocalizedValue<string>` | A key in any `ILocalizationTable` asset (e.g. `LocalizationTableFromGoogleSheetScriptableObject`), picked from a dropdown. Only works when Odin-serialized |

Serialize the interface with Odin so the inspector offers a type dropdown. The host must be a `SerializedMonoBehaviour` or `SerializedScriptableObject`:

```csharp
public class TitleCard : SerializedMonoBehaviour
{
    [OdinSerialize] private ILocalizedValue<string> title = new LocalizedStringFromTable();
    [OdinSerialize] private ILocalizedValue<Sprite> logo = new LocalizedValue<Sprite>();
}
```

Or use a concrete type directly with `[SerializeField]` in any MonoBehaviour or ScriptableObject:

```csharp
[SerializeField] private LocalizedValue<Sprite> logo;
```

In `LocalizedValue<T>`, empty strings, null and destroyed objects count as missing and fall back to the fallback language.

On IL2CPP platforms, Odin-serialized generic types must be covered by Odin's AOT generation (**Tools › Odin › Preferences › Serialization › AOT Generation**) or they fail to load in builds.

## LocalizationTextTag

Add **LocalizationTextTag** next to a TextMeshPro text and pick its `ILocalizedValue<string>`: a table key or text typed in per language. The text updates on enable and whenever `LocalizationManager.LanguageChanged` fires. Override `ApplyText(string)` to process text before it's shown.
