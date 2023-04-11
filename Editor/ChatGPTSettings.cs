using System.IO;
using UnityEditor;
using UnityEngine;

namespace BKK.ChatGPTEditor
{
    public class ChatGPTSettings : ScriptableObject
    {
        [SerializeField] private string openAiApiKey;
        [SerializeField] private string model = "text-davinci-003";
        [SerializeField] private string createAssetPath = "Assets/Editor/ChatGPTAnswers";

        public string apiKey => openAiApiKey;
        public string aiModel => model;
        public string createPath => createAssetPath;

        public const string settingPath = "Assets/Editor/Resources";

        [InitializeOnLoadMethod]
        private static void Init()
        {
            var existAssets = AssetDatabase.FindAssets($"ChatGPTSettings t:scriptableobject");

            if (existAssets.Length == 0)
            {
                var instance = ScriptableObject.CreateInstance<ChatGPTSettings>();

                if (!Directory.Exists(settingPath))
                {
                    Directory.CreateDirectory(settingPath);
                }

                AssetDatabase.CreateAsset(instance, $"{settingPath}/ChatGPTSettings.asset");
                AssetDatabase.SaveAssets();
            }
        }

        public bool ApiKeyIsEmpty()
        {
            return string.IsNullOrEmpty(openAiApiKey) || string.IsNullOrWhiteSpace(openAiApiKey);
        }

        public bool ModelIsEmpty()
        {
            return string.IsNullOrEmpty(model) || string.IsNullOrWhiteSpace(model);
        }
    }
}
