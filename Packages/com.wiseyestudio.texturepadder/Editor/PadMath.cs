using System;
using System.Collections.Generic;
using UnityEngine;

namespace WiseyeStudio.TexturePadder.Editor
{
    public static class PadMath
    {
        public const int DefaultSearchLimit = 512;

        /// <summary>
        /// Size the Texture Importer produces for a given Max Size. Measured on Unity 6000.6:
        /// the long side becomes maxSize and the short side is round(short * maxSize / long).
        /// <paramref name="ambiguous"/> is true when the exact value lands on .5, where we don't want to
        /// depend on the rounding direction.
        /// </summary>
        public static Vector2Int ImportedSize(int width, int height, int maxSize, out bool ambiguous)
        {
            ambiguous = false;
            var longSide = Math.Max(width, height);
            if (maxSize <= 0 || longSide <= maxSize)
            {
                return new Vector2Int(width, height);
            }

            var shortSide = Math.Min(width, height);
            var exact = (double)shortSide * maxSize / longSide;
            ambiguous = Math.Abs(exact - Math.Floor(exact) - 0.5) < 1e-6;
            var scaled = Math.Max(1, (int)Math.Floor(exact + 0.5));
            return width >= height ? new Vector2Int(maxSize, scaled) : new Vector2Int(scaled, maxSize);
        }

        public static Vector2Int ImportedSize(int width, int height, int maxSize) => ImportedSize(width, height, maxSize, out _);

        public static bool IsPowerOfTwo(int value) => value > 0 && (value & (value - 1)) == 0;

        public static int NextPowerOfTwo(int value)
        {
            var result = 1;
            while (result < value)
            {
                result <<= 1;
            }
            return result;
        }

        public static int CeilToMultiple(int value, int multiple) => (value + multiple - 1) / multiple * multiple;

        public static bool Satisfies(Vector2Int size, PadRule rule) => rule == PadRule.PowerOfTwo
            ? IsPowerOfTwo(size.x) && IsPowerOfTwo(size.y)
            : size.x % 4 == 0 && size.y % 4 == 0;

        /// <summary>
        /// True when the source size and the imported size for every Max Size in <paramref name="maxSizes"/>
        /// satisfy <paramref name="rule"/>.
        /// </summary>
        public static bool IsCompressible(int width, int height, IEnumerable<int> maxSizes, PadRule rule)
        {
            if (!Satisfies(new Vector2Int(width, height), rule))
            {
                return false;
            }

            foreach (var maxSize in maxSizes)
            {
                var imported = ImportedSize(width, height, maxSize, out var ambiguous);
                if (ambiguous || !Satisfies(imported, rule))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Smallest padded size (by added area) that is compressible for every Max Size.
        /// Padding the long side is allowed too: it changes the downscale ratio, which is often the cheapest way
        /// to land the short side on a multiple of 4 (1400x500 becomes 1408x500, imported as 1024x364).
        /// </summary>
        public static bool TryFindPaddedSize(int width, int height, IReadOnlyCollection<int> maxSizes, PadRule rule,
            out Vector2Int padded, int searchLimit = DefaultSearchLimit)
        {
            if (rule == PadRule.PowerOfTwo)
            {
                padded = new Vector2Int(NextPowerOfTwo(width), NextPowerOfTwo(height));
                return IsCompressible(padded.x, padded.y, maxSizes, rule);
            }

            padded = default;
            var bestArea = long.MaxValue;
            var bestSpread = int.MaxValue;
            var minHeight = CeilToMultiple(height, 4);
            for (var w = CeilToMultiple(width, 4); w <= width + searchLimit; w += 4)
            {
                // Rows only get more expensive as w grows, so stop once even the shortest one can't win.
                if ((long)w * minHeight > bestArea)
                {
                    break;
                }

                for (var h = minHeight; h <= height + searchLimit; h += 4)
                {
                    var area = (long)w * h;
                    if (area > bestArea)
                    {
                        break;
                    }
                    if (!IsCompressible(w, h, maxSizes, rule))
                    {
                        continue;
                    }

                    var spread = Math.Max(w - width, h - height);
                    if (area < bestArea || spread < bestSpread)
                    {
                        bestArea = area;
                        bestSpread = spread;
                        padded = new Vector2Int(w, h);
                    }
                    break;
                }
            }
            return bestArea != long.MaxValue;
        }

        /// <summary>Splits the extra pixels around the original content.</summary>
        public static Padding Distribute(Vector2Int source, Vector2Int padded, HorizontalAnchor horizontal, VerticalAnchor vertical)
        {
            var extraWidth = padded.x - source.x;
            var extraHeight = padded.y - source.y;
            var padding = new Padding
            {
                Left = horizontal switch
                {
                    HorizontalAnchor.Left => 0,
                    HorizontalAnchor.Right => extraWidth,
                    _ => extraWidth / 2,
                },
                Bottom = vertical switch
                {
                    VerticalAnchor.Bottom => 0,
                    VerticalAnchor.Top => extraHeight,
                    _ => extraHeight / 2,
                },
            };
            padding.Right = extraWidth - padding.Left;
            padding.Top = extraHeight - padding.Bottom;
            return padding;
        }
    }
}
