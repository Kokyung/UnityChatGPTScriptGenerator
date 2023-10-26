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

        private GUIStyle warnLabelStyle;
        
        private void OnEnable()
        {
            chatGptSettings = target as ChatGPTSettings;

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
            EditorGUILayout.PropertyField(m_Model);
            EditorGUILayout.LabelField("Warning: Current text-davinci-003 Only", warnLabelStyle);
            EditorGUILayout.PropertyField(m_CreateAssetPath);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
