using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace CustomStates
{
    class Program
    {
        static void Main(string[] args)
        {
            var potionLines = new List<string>();
            var statusLines = new List<string>();
            var potionXmlLines = new List<string>();
            var statusXmlLines = new List<string>();
            var entries = new Dictionary<string, List<Tuple<string, int>>>
            {
                // Attributes
                //{ "Strength", new List<Tuple<string, int>> { } },
                { "Finesse", new List<Tuple<string, int>> { Tuple.Create("Initiative", 1), Tuple.Create("DodgeBoost", 1)/*, Tuple.Create("Movement", 20)*/ } },
                { "Intelligence", new List<Tuple<string, int>> { Tuple.Create("FireResistance", 1), Tuple.Create("WaterResistance", 1), Tuple.Create("AirResistance", 1), Tuple.Create("EarthResistance", 1) } },
                { "Constitution", new List<Tuple<string, int>> { Tuple.Create("PhysicalResistance", 1), Tuple.Create("PoisonResistance", 1) } },
                //{ "Memory", new List<Tuple<string, int>> { } },
                { "Wits", new List<Tuple<string, int>> { Tuple.Create("AccuracyBoost", 1)/*, Tuple.Create("RangeBoost", 100)*/ } }
            };

            foreach (var entry in entries)
            {
                if (entry.Value.Count == 0)
                {
                    continue;
                }

                var stat = entry.Key;
                var dataPoints = entry.Value;
                potionLines.AddRange(new List<string>
                {
                    string.Format("new entry \"_Chronos38_Stats_{0}\"\r\n", stat),
                    "type \"Potion\"\r\n",
                    "\r\n"
                });
                statusLines.AddRange(new List<string>
                {
                    string.Format("new entry \"_Chronos38_CustomStatValue_{0}\"\r\n", stat),
                    "type \"StatusData\"\r\n",
                    "data \"StatusType\" \"CONSUME\"\r\n",
                    "\r\n"
                });
                potionXmlLines.AddRange(new List<string>
                {
                    "<stat_object color=\"#FF5F9EA0\" is_substat=\"false\">\r\n",
                    "<fields>\r\n",
                    string.Format("<field name=\"Name\" type=\"NameStatObjectFieldDefinition\" value=\"_Chronos38_Stats_{0}\" />\r\n", stat),
                    "</fields>\r\n",
                    "</stat_object>\r\n"
                });
                statusXmlLines.AddRange(new List<string>
                {
                    "<stat_object color=\"#FF5F9EA0\" is_substat=\"false\">\r\n",
                    "<fields>\r\n",
                    string.Format("<field name=\"Name\" type=\"NameStatObjectFieldDefinition\" value=\"_Chronos38_CustomStatValue_{0}\" />\r\n", stat),
                    "</fields>\r\n",
                    "</stat_object>\r\n"
                });

                for (int i = -10; i <= 50; ++i)
                {
                    if (i == 0)
                    {
                        continue;
                    }

                    /////////////////////////////////////////////////////////////////////////////////////
                    potionLines.Add(string.Format("new entry \"Chronos38_Stats_{0}_{1}\"\r\n", stat, i));
                    potionLines.Add("type \"Potion\"\r\n");

                    potionXmlLines.Add("<stat_object is_substat=\"false\">\r\n");
                    potionXmlLines.Add("<fields>\r\n");
                    potionXmlLines.Add(string.Format("<field name=\"Name\" type=\"NameStatObjectFieldDefinition\" value=\"Chronos38_Stats_{0}_{1}\" />\r\n", stat, i));

                    foreach (var dataPoint in dataPoints)
                    {
                        potionLines.Add(string.Format("data \"{0}\" \"{1}\"\r\n", dataPoint.Item1, dataPoint.Item2 * i));

                        potionXmlLines.Add(string.Format("<field name=\"{0}\" type=\"IntegerStatObjectFieldDefinition\" value=\"{1}\" />\r\n", dataPoint.Item1, dataPoint.Item2 * i));
                    }

                    potionLines.Add("\r\n");

                    potionXmlLines.Add("</fields>\r\n");
                    potionXmlLines.Add("</stat_object>\r\n");

                    /////////////////////////////////////////////////////////////////////////////////////
                    statusLines.Add(string.Format("new entry \"Chronos38_CustomStatValue_{0}_{1}\"\r\n", stat, i));
                    statusLines.Add("type \"StatusData\"\r\n");
                    statusLines.Add("data \"StatusType\" \"CONSUME\"\r\n");
                    statusLines.Add(string.Format("data \"StatsId\" \"Chronos38_Stats_{0}_{1}\"\r\n", stat, i));
                    statusLines.Add("\r\n");

                    statusXmlLines.Add("<stat_object is_substat=\"false\">\r\n");
                    statusXmlLines.Add("<fields>\r\n");
                    statusXmlLines.Add(string.Format("<field name=\"Name\" type=\"NameStatObjectFieldDefinition\" value=\"Chronos38_CustomStatValue_{0}_{1}\" />\r\n", stat, i));
                    statusXmlLines.Add(string.Format("<field name=\"StatsId\" type=\"StringStatObjectFieldDefinition\" value=\"Chronos38_Stats_{0}_{1}\" />\r\n", stat, i));
                    statusXmlLines.Add("</fields>\r\n");
                    statusXmlLines.Add("</stat_object>\r\n");
                }
            }

            var potionContent = potionLines.Aggregate((content, line) => content += line);
            var statusContent = statusLines.Aggregate((content, line) => content += line);
            potionXmlLines = potionXmlLines
                .Prepend("<stat_objects>\r\n")
                .Prepend("<stats stat_object_definition_id=\"48e63fff-81e0-42bb-ac51-39f5904b97be\">\r\n")
                .Prepend("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n")
                .ToList();
            potionXmlLines.Add("</stat_objects>\r\n");
            potionXmlLines.Add("</stats>\r\n");
            var potionXmlContent = potionXmlLines.Aggregate((content, line) => content += line);
            statusXmlLines = statusXmlLines
                .Prepend("<stat_objects>\r\n")
                .Prepend("<stats stat_object_definition_id=\"e2a8d59b-0e34-4a7c-bf5f-db7a2bb34cde\">\r\n")
                .Prepend("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n")
                .ToList();
            statusXmlLines.Add("</stat_objects>\r\n");
            statusXmlLines.Add("</stats>\r\n");
            var statusXmlContent = statusXmlLines.Aggregate((content, line) => content += line);

            File.WriteAllText("Potion.txt", potionContent);
            File.WriteAllText("Status_CONSUME.txt", statusContent);
            File.WriteAllText("Potion.stats", potionXmlContent);
            File.WriteAllText("Status_CONSUME.stats", statusXmlContent);
        }
    }
}
