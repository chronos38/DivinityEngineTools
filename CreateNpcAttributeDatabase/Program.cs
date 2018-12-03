using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace CreateNpcAttributeDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            var databaseName = "DB_Chronos38_NpcAttributeMap";
            var levelAttributeList = new List<List<int>>
            {
                new List<int> { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 },
                new List<int> { 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30 },
                new List<int> { 10, 13, 16, 19, 22, 25, 28, 31, 34, 37, 40 },
                new List<int> { 10, 14, 18, 22, 26, 30, 34, 38, 42, 46, 49 },
                new List<int> { 10, 15, 20, 25, 30, 35, 40, 45, 49, 54, 59 },
                new List<int> { 10, 16, 22, 28, 34, 40, 46, 51, 57, 63, 69 },
                new List<int> { 10, 17, 24, 31, 38, 45, 51, 58, 65, 72, 79 },
                new List<int> { 10, 18, 26, 34, 42, 49, 57, 65, 73, 81, 88 },
                new List<int> { 10, 19, 28, 37, 46, 54, 63, 72, 81, 89, 98 },
                new List<int> { 10, 20, 30, 40, 49, 59, 69, 79, 88, 98, 108 },
                new List<int> { 10, 21, 32, 43, 53, 64, 75, 86, 96, 107, 118 },
                new List<int> { 10, 22, 34, 46, 57, 69, 81, 92, 104, 116, 127 },
                new List<int> { 10, 23, 36, 49, 61, 74, 87, 99, 112, 125, 137 },
                new List<int> { 10, 24, 38, 51, 65, 79, 92, 106, 120, 133, 147 },
                new List<int> { 10, 25, 40, 54, 69, 84, 98, 113, 127, 142, 157 },
                new List<int> { 10, 26, 42, 57, 73, 88, 104, 120, 135, 151, 166 },
                new List<int> { 10, 27, 44, 60, 77, 93, 110, 127, 143, 160, 176 },
                new List<int> { 10, 28, 46, 63, 81, 98, 116, 133, 151, 168, 186 },
                new List<int> { 10, 29, 48, 66, 85, 103, 122, 140, 159, 177, 196 },
                new List<int> { 10, 30, 49, 69, 88, 108, 128, 147, 166, 186, 205 }
            };

            var databaseLines = new List<string>();

            for (int level = 1; level <= levelAttributeList.Count; ++level)
            {
                var attributeList = levelAttributeList[level - 1];

                for (int attribute = 0; attribute < attributeList.Count - 1; ++attribute)
                {
                    var currentValue = attributeList[attribute];
                    var nextValue = attributeList[attribute + 1];

                    for (int valueIterator = currentValue; valueIterator < nextValue; ++valueIterator)
                    {
                        var dk = valueIterator - currentValue;
                        var interpolate = dk / (float)(nextValue - currentValue);
                        var mappedValue = 2 * attribute + 10 + (int)Math.Round(interpolate * 2);
                        databaseLines.Add(string.Format("{0}({1}, {2}, {3});\r\n", databaseName, level, valueIterator, mappedValue));
                    }
                }

                databaseLines.Add(string.Format("{0}({1}, {2}, {3});\r\n", databaseName, level, attributeList.Last(), 30));
            }

            var databaseContent = databaseLines.Aggregate((content, line) => content += line);
            File.WriteAllText("a.out", databaseContent);
        }
    }
}
