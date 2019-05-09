using System;
using System.IO;

namespace TranslationKeys
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];
            var attributes = new string[] { "Finesse", "Intelligence", "Constitution", "Wits" };
            var content = @"<?xml version=""1.0"" encoding=""utf-8""?>
<save>  
    <header version=""2"" time=""0"" />
    <version major=""3"" minor=""6"" revision=""3"" build=""1"" />
    <region id=""TranslatedStringKeys"">
        <node id=""root"">
            <children>
";

            foreach (var attribute in attributes)
            {
                for (int i = -10; i <= 50; i++)
                {
                    content += "<node id=\"TranslatedStringKey\">";
                    content += $"<attribute id=\"Content\" value=\"{attribute}\" type=\"28\" handle=\"ls::TranslatedStringRepository::s_HandleUnknown\" />";
                    content += "<attribute id=\"ExtraData\" value=\"\" type=\"23\" />";
                    content += "<attribute id=\"Speaker\" value=\"\" type=\"22\" />";
                    content += "<attribute id=\"Stub\" value=\"True\" type=\"19\" />";
                    content += $"<attribute id=\"UUID\" value=\"Chronos38_Stats_{attribute}_{i}\" type=\"22\" />";
                    content += "</node>";
                }
            }

            content += @"			</children>
		</node>
	</region>
</save>";
            File.WriteAllText(fileName, content);
        }
    }
}
