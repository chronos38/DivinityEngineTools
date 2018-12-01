using System;
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

            var fileContent = "";
            var files = Directory.EnumerateFiles(args[0]);
            var databaseName = "DB_Chronos38_AbilitySkillLevelMap";

            foreach (var filePath in files)
            {
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
                    var parts = requirementValue.Split(' ');
                    fileContent += string.Format("{0}(\"{1}\", {2}, \"{3}\");\r\n", databaseName, parts.First(), int.Parse(parts.Last()), skillName);
                }
            }

            File.WriteAllText("a.out", fileContent);
        }
    }
}
