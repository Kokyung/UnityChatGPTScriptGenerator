using System;
using UnityEditor;
using UnityEngine;

namespace BKK.ChatGPTEditor
{
    [CustomEditor(typeof(ChatGPTSettings))]
    public class ChatGPTSettingsEditor : Editor
    {
        private ChatGPTSettings chatGptSettings;
        private SerializedProperty m_OpenAiApiKey;
        private SerializedProperty m_Model;
        private SerializedProperty m_CreateAssetPath;
        
        private void OnEnable()
        {
            chatGptSettings = target as ChatGPTSettings;

            m_OpenAiApiKey = serializedObject.FindProperty("openAiApiKey");
            m_Model = serializedObject.FindProperty("model");
            m_CreateAssetPath = serializedObject.FindProperty("createAssetPath");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_OpenAiApiKey);
            EditorGUILayout.PropertyField(m_Model, new GUIContent("Model( Current text-davinci-003 Only )"));
            EditorGUILayout.PropertyField(m_CreateAssetPath);
        }
    }
}
