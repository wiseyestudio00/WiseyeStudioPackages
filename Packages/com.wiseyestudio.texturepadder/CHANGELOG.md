# Changelog

## [0.1.0] - 2026-10-18

- Initial release.
- **Wiseye Studio › Texture Padding** window and **Assets › Wiseye Studio › Texture Padding › Scan Selection**: scan textures, see the imported size and format on the active platform, and pad the ones that won't compress.
- Checks the size after Max Size downscaling for the default and every platform override, and picks the padding with the least added area that works for all of them.
- Pads PNGs in place, so GUIDs and sprite fileIDs are kept. Shifts Multiple-mode sprite rects; moves a Single sprite's pivot, 9-slice border and outlines to stay on the same pixels.
- Mipmapped non-power-of-two textures: pad to power of two, or turn mipmaps off.
- Skips Sprite Atlas members, sprites with 2D Animation skinning data, non-PNG files and sheets with Slice on Import (optionally turns it off).
- Backups to `<project>/TexturePadderBackups/` with a restore button, and an optional warning on import.
