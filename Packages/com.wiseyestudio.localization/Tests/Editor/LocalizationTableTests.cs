using NUnit.Framework;
using UnityEngine;

namespace WiseyeStudio.Localization.Tests
{
    public class LocalizationTableTests
    {
        private static readonly SystemLanguage[] DefaultColumns =
        {
            SystemLanguage.English,
            SystemLanguage.ChineseTraditional,
            SystemLanguage.Japanese,
        };

        private static LocalizationTable Parse(string csv, bool hasHeaderRow) =>
            LocalizationTable.Parse(csv, hasHeaderRow, DefaultColumns, SystemLanguage.English);

        [Test]
        public void HeaderlessTableUsesDefaultColumnOrder()
        {
            var table = Parse("back,Back,返回,戻る", false);

            Assert.AreEqual("Back", table.GetString("back", SystemLanguage.English));
            Assert.AreEqual("返回", table.GetString("back", SystemLanguage.ChineseTraditional));
            Assert.AreEqual("戻る", table.GetString("back", SystemLanguage.Japanese));
        }

        [Test]
        public void HeaderMapsColumnsByName()
        {
            var table = Parse("key,Japanese,English\nback,戻る,Back", true);

            Assert.AreEqual("Back", table.GetString("back", SystemLanguage.English));
            Assert.AreEqual("戻る", table.GetString("back", SystemLanguage.Japanese));
            CollectionAssert.AreEqual(new[] { "back" }, table.Keys);
            CollectionAssert.AreEqual(new[] { SystemLanguage.Japanese, SystemLanguage.English }, table.Languages);
        }

        [Test]
        public void QuotedCellsKeepCommasQuotesAndLineBreaks()
        {
            var table = Parse("a,\"one, two\"\r\nb,\"say \"\"hi\"\"\"\nc,\"line 1\nline 2\"", false);

            Assert.AreEqual("one, two", table.GetString("a", SystemLanguage.English));
            Assert.AreEqual("say \"hi\"", table.GetString("b", SystemLanguage.English));
            Assert.AreEqual("line 1\nline 2", table.GetString("c", SystemLanguage.English));
        }

        [Test]
        public void EscapedNewlineAndTabAreUnescaped()
        {
            var table = Parse("a,first\\nsecond\\tthird", false);

            Assert.AreEqual("first\nsecond\tthird", table.GetString("a", SystemLanguage.English));
        }

        [Test]
        public void MissingCellFallsBackToFallbackLanguage()
        {
            var table = Parse("a,Hello,,\nb,Bye", false);

            Assert.AreEqual("Hello", table.GetString("a", SystemLanguage.Japanese));
            Assert.AreEqual("Bye", table.GetString("b", SystemLanguage.ChineseTraditional));
            Assert.IsFalse(table.TryGetString("a", SystemLanguage.Japanese, out _));
        }

        [Test]
        public void UnknownKeyReturnsKey()
        {
            var table = Parse("a,Hello", false);

            Assert.IsFalse(table.ContainsKey("missing"));
            Assert.AreEqual("missing", table.GetString("missing", SystemLanguage.English));
        }

        [Test]
        public void KeyWithNoValueInAnyLanguageReturnsKey()
        {
            var table = Parse("empty,,,", false);

            Assert.IsTrue(table.ContainsKey("empty"));
            Assert.AreEqual("empty", table.GetString("empty", SystemLanguage.Japanese));
        }

        [Test]
        public void DuplicateKeyIsReportedAndLastRowWins()
        {
            var table = Parse("a,First\na,Second", false);

            Assert.AreEqual("Second", table.GetString("a", SystemLanguage.English));
            Assert.AreEqual(1, table.Keys.Count);
            Assert.AreEqual(1, table.Problems.Count);
        }

        [Test]
        public void UnknownHeaderColumnIsReportedAndIgnored()
        {
            var table = Parse("key,English,Notes\na,Hello,ignore me", true);

            Assert.AreEqual("Hello", table.GetString("a", SystemLanguage.English));
            CollectionAssert.AreEqual(new[] { SystemLanguage.English }, table.Languages);
            Assert.AreEqual(1, table.Problems.Count);
        }

        [Test]
        public void ReplaceKeyedTagsReplacesKnownKeysOnly()
        {
            var table = Parse("perfect,Perfect,完美,パーフェクト", false);

            Assert.AreEqual(
                "完美+ {unknown}",
                table.ReplaceKeyedTags("{perfect}+ {unknown}", SystemLanguage.ChineseTraditional));
        }
    }
}
