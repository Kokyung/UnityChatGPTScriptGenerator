using UnityEditor;
using UnityEngine;

namespace BKK.ChatGPTEditor
{
    public class ChatGPTSettingsWindow : EditorWindow
    {
        private ChatGPTSettings settings;

        private Editor settingsEditor;

        [MenuItem("Window/BKK/Chat GPT Settings")]
        private static void OpenChatGPTSettingsWindow()
        {
            ChatGPTSettingsWindow window =
                GetWindow<ChatGPTSettingsWindow>(false, "Chat GPT Settings", true);
            window.titleContent = new GUIContent("Chat GPT Settings");
        }

        private void OnEnable()
        {
            settings = Resources.Load<ChatGPTSettings>("ChatGPTSettings");
            settingsEditor = Editor.CreateEditor(settings);
        }

        private void OnGUI()
        {
            settingsEditor.DrawDefaultInspector();
            EditorUtility.SetDirty(settingsEditor.target);
        }
    }
}
