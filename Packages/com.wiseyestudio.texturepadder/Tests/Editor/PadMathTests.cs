using NUnit.Framework;
using UnityEngine;
using WiseyeStudio.TexturePadder.Editor;

namespace WiseyeStudio.TexturePadder.Tests
{
    public class PadMathTests
    {
        // Expected sizes were measured by importing real PNGs in Unity 6000.6.
        [TestCase(1400, 500, 1024, 1024, 366)]
        [TestCase(1400, 480, 1024, 1024, 351)]
        [TestCase(1400, 486, 1024, 1024, 355)]
        [TestCase(1400, 512, 1024, 1024, 374)]
        [TestCase(2000, 1003, 1024, 1024, 514)]
        [TestCase(3000, 1001, 512, 512, 171)]
        [TestCase(500, 1400, 1024, 366, 1024)]
        [TestCase(1025, 1023, 1024, 1024, 1022)]
        [TestCase(1400, 500, 2048, 1400, 500)]
        public void ImportedSizeMatchesUnity(int width, int height, int maxSize, int expectedWidth, int expectedHeight)
        {
            Assert.AreEqual(new Vector2Int(expectedWidth, expectedHeight), PadMath.ImportedSize(width, height, maxSize));
        }

        [Test]
        public void SourceMultipleOfFourIsNotEnoughAfterDownscale()
        {
            Assert.IsTrue(PadMath.IsCompressible(1400, 500, new[] { 2048 }, PadRule.MultipleOfFour));
            Assert.IsFalse(PadMath.IsCompressible(1400, 500, new[] { 1024 }, PadRule.MultipleOfFour));
        }

        [Test]
        public void PadsLongSideWhenCheaper()
        {
            Assert.IsTrue(PadMath.TryFindPaddedSize(1400, 500, new[] { 1024 }, PadRule.MultipleOfFour, out var padded));
            Assert.AreEqual(new Vector2Int(1408, 500), padded);
            Assert.AreEqual(new Vector2Int(1024, 364), PadMath.ImportedSize(1408, 500, 1024));
        }

        [Test]
        public void PaddedSizeWorksForEveryMaxSize()
        {
            var maxSizes = new[] { 256, 512, 1024, 2048 };
            for (var width = 300; width < 2200; width += 37)
            {
                for (var height = 300; height < 2200; height += 53)
                {
                    Assert.IsTrue(PadMath.TryFindPaddedSize(width, height, maxSizes, PadRule.MultipleOfFour, out var padded), $"{width}x{height}");
                    Assert.IsTrue(padded.x >= width && padded.y >= height);
                    Assert.IsTrue(PadMath.IsCompressible(padded.x, padded.y, maxSizes, PadRule.MultipleOfFour), $"{width}x{height} -> {padded}");
                }
            }
        }

        [Test]
        public void PowerOfTwoRulePadsEachSideUp()
        {
            Assert.IsTrue(PadMath.TryFindPaddedSize(1400, 500, new[] { 1024 }, PadRule.PowerOfTwo, out var padded));
            Assert.AreEqual(new Vector2Int(2048, 512), padded);
        }

        [Test]
        public void DistributeCentersContent()
        {
            var padding = PadMath.Distribute(new Vector2Int(1401, 500), new Vector2Int(1408, 504),
                HorizontalAnchor.Center, VerticalAnchor.Middle);

            Assert.AreEqual(3, padding.Left);
            Assert.AreEqual(4, padding.Right);
            Assert.AreEqual(2, padding.Bottom);
            Assert.AreEqual(2, padding.Top);
        }
    }
}
