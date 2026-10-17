# Wiseye Studio Texture Padder

Editor tool that finds textures which **won't block-compress** (Unity silently stores them as RGBA32) and pads the
source PNGs in place with transparent pixels so they do. The check uses the size Unity *actually imports*, after
Max Size downscaling on every platform override, not the file's own size.

> [!WARNING]
> Built for Wiseye Studio's own games. Use it as a reference or at your own risk.

## Installation

```
https://github.com/wiseyestudio00/WiseyeStudioPackages.git?path=Packages/com.wiseyestudio.texturepadder
```

It depends on `com.unity.2d.sprite`, which UPM installs automatically.

## Usage

1. Open **Wiseye Studio › Texture Padding**, or right-click textures or folders and use
   **Wiseye Studio › Texture Padding › Scan Selection**.
2. Check the list. *Imported now* is the real size and format on the active platform (orange means not compressed).
   *Proposed* is the padded source size.
3. Press **Pad N textures**. Each PNG is overwritten at the same path, so GUIDs, sprite fileIDs and every reference
   stay the same. Originals are copied to `<project>/TexturePadderBackups/<timestamp>/` first, and
   **Restore backup…** puts them back.

| Option | Default | Used for |
| --- | --- | --- |
| Content horizontal / vertical | Center / Middle | Where the original image sits; padding goes on the other side(s) |
| Padding color | Transparent | The added pixels. Opaque PNGs gain an alpha channel |
| Mipmapped textures | Pad to power of two | Mipmapped non-power-of-two textures never compress. Pad to POT, or pad to ×4 and turn mipmaps off |
| Skip Sprite Atlas members | On | Packed sprites ship inside the atlas texture, so their own compression doesn't matter |
| Turn off Slice on Import | Off | Sheets with Slice on Import re-slice on every import and would lose the rect shift. When on, the flag is turned off and the current rects are kept |
| Back up originals | On | Copies the PNG and .meta before writing |
| Warn on import | Off | Logs a Console warning whenever a texture imports at a size that blocks compression |

## What it changes

| Texture | Change |
| --- | --- |
| Any | PNG pixels: the original is copied in, padding around it |
| Sprite (Multiple) | Every rect is shifted by the left/bottom padding; pivots, borders and outlines are rect-relative and unchanged |
| Sprite (Single) | The rect grows to the full texture; the pivot and 9-slice border move so they stay on the same content pixels; custom outline and physics shape are shifted |
| Mipmapped NPOT | Padded to power of two, or mipmaps turned off |

It refuses non-PNG files (PSD, TGA, JPG…), sprites with 2D Animation bones or meshes, read-only files, and sheets
with Slice on Import on (unless the option is enabled).

A Single sprite's rect now includes the padding. A UI `Image` stretched to its RectTransform draws the transparent strip
as part of the sprite, so the art looks slightly smaller. World-space `SpriteRenderer`s aren't affected (same pixels
per unit, same pivot pixel).

## Compression rules it follows

Measured on Unity 6000.6 with the Android build target.

- **Max Size downscale:** the long side becomes Max Size and the short side becomes `round(short × MaxSize / long)`.
  1400×500 at Max Size 1024 imports as **1024×366**, which isn't a multiple of 4 even though 1400 and 500 are.
- **DXT/BC (DXT1, DXT5, BC7) and ETC/ETC2** need the *imported* width and height to be multiples of 4. Otherwise Unity
  stores RGBA32 or RGB24. Since 2021.2 Unity refuses to compress these sizes instead of decompressing at load.
- **ASTC** (the Android and iOS default in Unity 6) compresses any size, so the ×4 rule doesn't apply to it.
- **Mipmaps on + a non-power-of-two imported size means no format compresses**, ASTC included.
- **Sprites ignore "Non-Power of 2"**. Other texture types with NPOT scaling on are rescaled to POT and always compress.

The padding search picks the least added area that works for **every** Max Size in use. Padding the long side is often
cheapest because it changes the scale ratio: 1400×500 becomes **1408×500**, which imports as 1024×364.
