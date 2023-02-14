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

        public void SendAnswer()
        {
            Debug.Log("Answer Sent");
            var settings = Resources.Load<ChatGPTSettings>("ChatGPTSettings");
            EditorBackgroundUtility.StartBackgroundTask(GenerateResponse(settings.apiKey, question, SetAnswer));
        }

        private void SetAnswer(string _answer)
        {
            answer = _answer.Trim();
        }

        private IEnumerator GenerateResponse(string apiKey, string prompt, Action<string> resultAction)
        {
            ChatGPTCompletionData completionData = new ChatGPTCompletionData
            {
                model = "text-davinci-003",
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

                    answerSent = false;
                }
            }
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
