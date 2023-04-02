using UnityEditor;
using UnityEngine;

namespace BKK.ChatGPTEditor
{
    [CustomEditor(typeof(ChatGPTAnswer))]
    public class ChatGPTAnswerEditor : Editor
    {
        private ChatGPTAnswer answerAsset;

        private ChatGPTSettings settings;

        private const string waitMessage = "Wait for Response...";

        private const string standbyMessage = "Ready to Answer.";

        private const string apiKeyErrorMessage = "API Key is Empty.";
        private const string modelErrorMessage = "Model Name is Empty.";
        private const string settingErrorMessage = "ChatGPTSettings not Exists.";

        private Vector2 scrollA;
        private Vector2 scrollB;

        private int typingAreaHeight = 15;
        private int fontSize = 12;

        private void OnEnable()
        {
            if (target is ChatGPTAnswer answer) answerAsset = answer;
            settings = Resources.Load<ChatGPTSettings>("ChatGPTSettings");
        }

        public override void OnInspectorGUI()
        {
            if (answerAsset.answerSent) GUI.enabled = false;

            TextScrollArea("[ Question ]", ref answerAsset.question, ref scrollA);
            TextScrollArea("[ Answer ]", ref answerAsset.answer, ref scrollB);

            EditorGUILayout.Separator();

            answerAsset.fileName = EditorGUILayout.TextField("File Name", answerAsset.fileName);

            answerAsset.savePath = EditorGUILayout.TextField("Save Path", answerAsset.savePath);

            EditorGUILayout.Separator();

            if (!settings || settings.ApiKeyIsEmpty() || settings.ModelIsEmpty()) GUI.enabled = false;
            if (GUILayout.Button("Send Answer"))
            {
                answerAsset.SendAnswer();
                answerAsset.answerSent = true;
            }

            if (!settings || settings.ApiKeyIsEmpty() || settings.ModelIsEmpty()) GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (answerAsset.answerSent)
            {
                GUILayout.Label(waitMessage);
            }
            else
            {
                if (!settings)
                {
                    GUILayout.Label(settingErrorMessage);
                }
                else
                {
                    if (settings.ApiKeyIsEmpty())
                    {
                        GUILayout.Label(apiKeyErrorMessage);
                    }
                    else if (settings.ModelIsEmpty())
                    {
                        GUILayout.Label(modelErrorMessage);
                    }
                    else
                    {
                        GUILayout.Label(standbyMessage);
                    }
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (answerAsset.answerSent) GUI.enabled = true;

            EditorUtility.SetDirty(answerAsset);
            Undo.RecordObject(answerAsset, "ChatGPTAnswer");
        }

        private void TextScrollArea(string label, ref string text, ref Vector2 scrollPos)
        {
            EditorGUILayout.LabelField(label);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos,
                GUILayout.MaxHeight(typingAreaHeight * EditorGUIUtility.singleLineHeight));

            text = EditorGUILayout.TextArea(text,
                EditorStyles.textArea,
                GUILayout.MaxHeight(typingAreaHeight *
                                    EditorGUIUtility.singleLineHeight));

            EditorStyles.textArea.fontSize = fontSize;
            EditorGUILayout.EndScrollView();
        }
    }
}
