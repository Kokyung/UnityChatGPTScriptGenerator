using System;
using UnityEditor;
using UnityEngine;

namespace BKK.ChatGPTEditor
{
    [CustomEditor(typeof(ChatGPTSettings))]
    public class ChatGPTSettingsEditor : Editor
    {
        private SerializedProperty m_OpenAiApiKey;
        private SerializedProperty m_Model;
        private SerializedProperty m_CreateAssetPath;

        private GUIStyle warnLabelStyle;

        private readonly string choosePathButtonText = "Choose";
        private readonly string choosePathTitleText = "Choose Asset Path";

        private readonly string modelFieldLabel = "Model";

        private readonly string modelPrefsName = "UnityChatGPTScriptGenerator_Model";

        private void OnEnable()
        {
            m_OpenAiApiKey = serializedObject.FindProperty("openAiApiKey");
            m_Model = serializedObject.FindProperty("model");
            m_CreateAssetPath = serializedObject.FindProperty("createAssetPath");

            warnLabelStyle = new GUIStyle
            {
                normal = new GUIStyleState
                {
                    textColor = Color.cyan
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(m_OpenAiApiKey);

            int modelIndex = EditorPrefs.GetInt("UnityChatGPTScriptGenerator_Model", 0);

            modelIndex = EditorGUILayout.Popup(modelFieldLabel, modelIndex, BKKChatGPT.modelOptions);

            EditorPrefs.SetInt(modelPrefsName, modelIndex);
            
            BKKChatGPT.ModelEnum modelEnum = (BKKChatGPT.ModelEnum) modelIndex;
            
            switch (modelEnum)
            {
                case BKKChatGPT.ModelEnum.TextDavinci003:
                    m_Model.stringValue = BKKChatGPT.TextDavinci003;
                    break;
                case BKKChatGPT.ModelEnum.GPT35Turbo:
                    m_Model.stringValue = BKKChatGPT.GPT35Turbo;
                    break;
                case BKKChatGPT.ModelEnum.GPT4:
                    m_Model.stringValue = BKKChatGPT.GPT4;
                    break;
                default:
                    break;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_CreateAssetPath);
            if (GUILayout.Button(choosePathButtonText, GUILayout.Width(100)))
            {
                string absolutePath = EditorUtility.OpenFolderPanel(choosePathTitleText, Application.dataPath, "");

                if (!string.IsNullOrEmpty(absolutePath))
                {
                    string relativePath = "Assets" + absolutePath.Substring(Application.dataPath.Length);
                    m_CreateAssetPath.stringValue = relativePath;
                }
            }
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
