using System.Collections.Generic;
using System.Text;

namespace WiseyeStudio.Localization
{
    internal static class CsvParser
    {
        private const char ByteOrderMark = '﻿';

        public static List<List<string>> Parse(string text)
        {
            var rows = new List<List<string>>();
            var row = new List<string>();
            var cell = new StringBuilder();
            var insideQuotes = false;
            var start = text.Length > 0 && text[0] == ByteOrderMark ? 1 : 0;

            for (var i = start; i < text.Length; i++)
            {
                var c = text[i];

                if (c == '\r')
                {
                    continue;
                }

                if (insideQuotes)
                {
                    if (c != '"')
                    {
                        cell.Append(c);
                    }
                    else if (i + 1 < text.Length && text[i + 1] == '"')
                    {
                        cell.Append('"');
                        i++;
                    }
                    else
                    {
                        insideQuotes = false;
                    }

                    continue;
                }

                switch (c)
                {
                    case '"':
                        insideQuotes = true;
                        break;
                    case ',':
                        row.Add(cell.ToString());
                        cell.Clear();
                        break;
                    case '\n':
                        row.Add(cell.ToString());
                        cell.Clear();
                        rows.Add(row);
                        row = new List<string>();
                        break;
                    default:
                        cell.Append(c);
                        break;
                }
            }

            if (cell.Length > 0 || row.Count > 0)
            {
                row.Add(cell.ToString());
                rows.Add(row);
            }

            return rows;
        }
    }
}
