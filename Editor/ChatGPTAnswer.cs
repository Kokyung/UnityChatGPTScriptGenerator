using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace BKK.ChatGPTEditor
{
    [CreateAssetMenu(fileName = "Answer Asset", menuName = "ChatGPT Script Compiler/Answer Asset", order = 0)]
    public class ChatGPTAnswer : ScriptableObject
    {
        public string question;
        public string answer;

        public string fileName;
        public string savePath;

        [HideInInspector] public bool answerSent;

        private readonly ChatGPT4Message systemMessage = new ChatGPT4Message()
        {
            role = "user",
            content =
                "You are Unity Engine Script Generator. You will be given script related unity. Respond with only the script without explanation. If user talk about not unity engine related, respond \"I can't Respond about that.\""
        };

        public void SendAnswer()
        {
            Debug.Log("Answer Sent");
            var settings = Resources.Load<ChatGPTSettings>("ChatGPTSettings");

            if (settings.aiModel.Contains(BKKChatGPT.TextDavinci003))
            {
                EditorBackgroundUtility.StartBackgroundTask(GenerateLegacyResponse(settings.apiKey, settings.aiModel,question, SetAnswer));
            }
            else
            {
                EditorBackgroundUtility.StartBackgroundTask(GenerateGPT4Response(settings.apiKey, settings.aiModel,question, SetAnswer));
            }
        }

        private void SetAnswer(string _answer)
        {
            answer = _answer.Trim();
        }

        private IEnumerator GenerateLegacyResponse(string apiKey, string model, string prompt, Action<string> resultAction)
        {
            ChatGPTCompletionData completionData = new ChatGPTCompletionData
            {
                model = model,
                prompt = prompt,
                max_tokens = 1000,
                temperature = 0,
            };

            string json = JsonUtility.ToJson(completionData);
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest request = UnityWebRequest.Post("https://api.openai.com/v1/completions", json))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
                request.uploadHandler = new UploadHandlerRaw(jsonBytes);
                request.downloadHandler = new DownloadHandlerBuffer();

                yield return request.SendWebRequest();

                while (request.isDone == false) yield return null;

                if (request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("ChatGPT Answered!");
                    var result = JsonUtility.FromJson<ChatGPTResult>(request.downloadHandler.text);
                    resultAction?.Invoke(result.choices[0].text);
                }
                answerSent = false;
            }
        }
        
        private IEnumerator GenerateGPT4Response(string apiKey, string model, string message, Action<string> resultAction)
        {
            ChatGPT4CompletionData completionData = new ChatGPT4CompletionData
            {
                model = model, 
                messages = new List<ChatGPT4Message>()
                {
                    systemMessage,
                    new ChatGPT4Message()
                    {
                        role = "user", 
                        content = message,
                    }
                },
                temperature = 0,
            };

            string json = JsonUtility.ToJson(completionData);
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest request = UnityWebRequest.Post("https://api.openai.com/v1/chat/completions", json))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
                request.uploadHandler = new UploadHandlerRaw(jsonBytes);
                request.downloadHandler = new DownloadHandlerBuffer();

                yield return request.SendWebRequest();

                while (request.isDone == false) yield return null;

                if (request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("ChatGPT Answered!");
                    ChatGPT4Result result = JsonUtility.FromJson<ChatGPT4Result>(request.downloadHandler.text);
                    resultAction?.Invoke(result.choices[0].message.content);
                }
                answerSent = false;
            }
        }

        [Serializable]
        public class ChatGPT4CompletionData
        {
            public string model;
            public List<ChatGPT4Message> messages = new List<ChatGPT4Message>();
            public int temperature;
        }
        
        [Serializable]
        public class ChatGPT4Result
        {
            public string id;
            public string @object;
            public int created;
            public string model;
            public ChatGPTUsage usage;
            public List<ChatGPT4Choice> choices = new List<ChatGPT4Choice>();
        }

        [Serializable]
        public class ChatGPT4Message
        {
            public string role;
            public string content;
        }
        
        [Serializable]
        public class ChatGPT4Choice
        {
            public ChatGPT4Message message;
            public string finish_reason;
            public int index;
        }

        [Serializable]
        public class ChatGPTCompletionData
        {
            public string model;
            public string prompt;
            public int max_tokens;
            public int temperature;
        }

        [Serializable]
        public class ChatGPTResult
        {
            public string id;
            public string @object;
            public int created;
            public string model;

            public List<ChatGPTChoice> choices;
            public ChatGPTUsage usage;
        }

        [Serializable]
        public class ChatGPTChoice
        {
            public string text;
            public int index;
            public string finish_reason;
        }

        [Serializable]
        public class ChatGPTUsage
        {
            public int prompt_tokens;
            public int completion_tokens;
            public int total_tokens;
        }
    }
}
