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

            var levelMapLines = new List<string>();
            var dependencyMapLines = new List<string>();
            var files = Directory.EnumerateFiles(args[0]);
            var abilitySkillLevelMap = "DB_Chronos38_AbilitySkillLevelMap";
            var abilitySkillCrossDependencyMap = "DB_Chronos38_AbilitySkillCrossDependencyMap";

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
                    var skillTypes = "";
                    var skillLevel = 0;

                    foreach (var skillRequirement in skillRequirements)
                    {
                        var parts = skillRequirement.Split(' ');
                        skillTypes = skillTypes.Length > 0 
                            ? string.Format("{0}+{1}", skillTypes, parts.First()) 
                            : string.Format("{0}", parts.First());
                        skillLevel = Math.Max(skillLevel, int.Parse(parts.Last()));
                    }

                    if (skillTypes.Contains("+"))
                    {
                        var skillDependencies = skillTypes.Split('+');
                        var firstSkillDependency = skillDependencies.First();
                        var secondSkillDependency = skillDependencies.Last();
                        var dependencyMapLine = string.Format("{0}(\"{1}\", \"{2}\");\r\n", abilitySkillCrossDependencyMap, firstSkillDependency, secondSkillDependency);

                        if (!dependencyMapLines.Contains(dependencyMapLine))
                        {
                            dependencyMapLines.Add(dependencyMapLine);
                        }
                    }

                    levelMapLines.Add(string.Format("{0}(\"{1}\", {2}, \"{3}_{4}\");\r\n", abilitySkillLevelMap, skillTypes, skillLevel, skillType, skillName));
                }
            }

            levelMapLines.Sort();
            dependencyMapLines.Sort();
            var levelMapFileContent = levelMapLines.Aggregate((content, current) => content += current);
            var dependencyMapFileContent = dependencyMapLines.Aggregate((content, current) => content += current);
            var fileContent = string.Format("{0}\r\n{1}", levelMapFileContent, dependencyMapFileContent);

            File.WriteAllText("a.out", fileContent);
        }
    }
}
