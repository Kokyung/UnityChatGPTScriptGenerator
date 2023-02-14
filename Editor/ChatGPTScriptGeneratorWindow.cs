using System.IO;
using UnityEditor;
using UnityEngine;

namespace BKK.ChatGPTEditor
{
    public class ChatGPTScriptGeneratorWindow : EditorWindow
    {
        private GUIStyle titleStyle;

        private ChatGPTAnswer answerAsset;
        private Editor answerAssetEditor;

        private ChatGPTSettings settings;

        private const string assetFileName = "Answer Asset.asset";

        [MenuItem("Window/BKK/Chat GPT Script Generator")]
        private static void OpenChatGPTScriptGeneratorWindow()
        {
            ChatGPTScriptGeneratorWindow window =
                GetWindow<ChatGPTScriptGeneratorWindow>(false, "Chat GPT Script Generator", true);
            window.titleContent = new GUIContent("Chat GPT Script Generator");
        }

        private void OnEnable()
        {
            titleStyle = new GUIStyle
            {
                fontSize = 20,
                normal =
                {
                    textColor = Color.white
                }
            };

            EditorPrefs.GetString($"{this.GetType()}_CreatePath", "Assets/Editor/ChatGPTAnswers");

            settings = Resources.Load<ChatGPTSettings>("ChatGPTSettings");
        }

        private void OnDisable()
        {
            EditorPrefs.SetString($"{this.GetType()}_CreatePath", settings.createPath);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Chat GPT Script Generator", titleStyle);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            if (answerAsset && answerAsset.answerSent) GUI.enabled = false;

            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button("Create Answer"))
            {
                var instance = ScriptableObject.CreateInstance<ChatGPTAnswer>();
                var path = $"{settings.createPath}/{assetFileName}";

                if (!Directory.Exists(settings.createPath))
                {
                    Directory.CreateDirectory(settings.createPath);
                }
                
                var uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);
                AssetDatabase.CreateAsset(instance, uniquePath);

                AssetDatabase.SaveAssets();

                answerAsset = instance;

                EditorGUI.FocusTextInControl(null); // take focus from textArea
            }

            EditorGUILayout.Separator();

            answerAsset =
                (ChatGPTAnswer) EditorGUILayout.ObjectField("Answer Asset", answerAsset, typeof(ChatGPTAnswer), false);

            if (answerAsset && answerAsset.answerSent) GUI.enabled = true;

            if (answerAsset)
            {
                if (EditorGUI.EndChangeCheck())
                    answerAssetEditor =
                        Editor.CreateEditor(answerAsset); // Create Answer Asset Editor when Answer Asset changed.

                answerAssetEditor.OnInspectorGUI();
                EditorUtility.SetDirty(answerAssetEditor.target);

                if (answerAsset && answerAsset.answerSent) GUI.enabled = false;

                if (GUILayout.Button("Create Script And Compile"))
                {
                    if (ScriptGenerator.IsScript(answerAsset.answer))
                    {
                        ScriptGenerator.CreateCsFile(answerAsset.answer, answerAsset.fileName, answerAsset.savePath);
                    }
                    else
                    {
                        Debug.Log("Answer does not match class pattern.");
                    }
                }

                if (answerAsset && answerAsset.answerSent) GUI.enabled = true;
            }

            if (answerAsset && answerAsset.answerSent) GUI.enabled = false;

            if (GUILayout.Button("Settings"))
            {
                var asset = AssetDatabase.LoadMainAssetAtPath($"{ChatGPTSettings.settingPath}/ChatGPTSettings.asset");

                EditorGUIUtility.PingObject(asset);
            }

            EditorGUILayout.EndVertical();

            if (answerAsset && answerAsset.answerSent) GUI.enabled = true;
        }
    }
}
