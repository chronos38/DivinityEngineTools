using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace SkillStatsToOsirisDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: SkillStatsToOsirisDatabase <stats path>");
                return;
            }

            var fileLines = new List<string>();
            var files = Directory.EnumerateFiles(args[0]);
            var databaseName = "DB_Chronos38_AbilitySkillLevelMap";

            foreach (var filePath in files)
            {
                var skillType = Path.GetFileNameWithoutExtension(filePath);
                var document = new XmlDocument();
                document.Load(filePath);

                foreach (XmlNode skills in document.GetElementsByTagName("stat_object"))
                {
                    var requirementAttribute = skills.SelectSingleNode("fields/field[@name='MemorizationRequirements']/@value") as XmlAttribute;

                    if (requirementAttribute == null)
                    {
                        continue;
                    }

                    var requirementValue = requirementAttribute.Value;

                    if (requirementValue.Length == 0)
                    {
                        continue;
                    }

                    var nameAttribute = skills.SelectSingleNode("fields/field[@name='Name']/@value") as XmlAttribute;

                    if (nameAttribute == null)
                    {
                        continue;
                    }

                    var skillName = nameAttribute.Value;
                    var skillRequirements = requirementValue.Split("; ");
                    var skillSchools = "";
                    var skillLevel = 0;

                    foreach (var skillRequirement in skillRequirements)
                    {
                        var parts = skillRequirement.Split(' ');
                        skillSchools = skillSchools.Length > 0 
                            ? string.Format("{0}+{1}", skillSchools, parts.First()) 
                            : string.Format("{0}", parts.First());
                        skillLevel = Math.Max(skillLevel, int.Parse(parts.Last()));
                    }

                    fileLines.Add(string.Format("{0}(\"{1}\", {2}, \"{3}_{4}\");\r\n", databaseName, skillSchools, skillLevel, skillType, skillName));
                }
            }

            fileLines.Sort();
            var fileContent = fileLines.Aggregate((content, current) => content += current);
            File.WriteAllText("a.out", fileContent);
        }
    }
}
