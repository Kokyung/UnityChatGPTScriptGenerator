using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BKK.ChatGPTEditor
{
    public static class ScriptGenerator
    {
        private const string pattern =
            @"(((internal)|(public)|(private)|(protected)|(sealed)|(abstract)|(static))?[\s\r\n\t]+){0,2}class[\s\S]+?(?={)";

        public static void CreateCsFile(string contents, string name, string savePath)
        {
            var nameIsEmpty = name == string.Empty;

            var matches = Regex.Matches(contents, pattern, RegexOptions.Multiline);
            var classes =
                matches.Cast<Match>()
                    .Select(x => x.Value.Trim()); // return: public class PlayerController : MonoBehaviour;
            var firstClass = classes.First();
            var classType = Regex.Match(firstClass, @"class (.+?):").Groups[1].Value;
            var className = nameIsEmpty ? classType : name;

            if (string.IsNullOrEmpty(savePath)) savePath = "Assets";

            var filePath = $"{savePath}/{className}.cs";
            var existScripts = AssetDatabase.FindAssets($"{className} t:script");

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            if (existScripts.Length == 0)
            {
                using var outfile = new StreamWriter(filePath);

                if (nameIsEmpty)
                {
                    outfile.WriteLine(contents);
                }
                else
                {
                    var replacedContents = contents.Replace(className, name); // Change class name
                    outfile.WriteLine(replacedContents);
                }

                outfile.Close();

                Debug.Log($"{name}.cs Created. : {filePath}");
                AssetDatabase.Refresh();
            }
            else
            {
                var path = AssetDatabase.GUIDToAssetPath(existScripts[0]);

                Debug.Log($"{name}.cs Already Exists. : {path}");
            }
        }

        public static bool IsScript(string contents)
        {
            return Regex.Matches(contents, pattern, RegexOptions.Multiline).Count != 0;
        }
    }
}
