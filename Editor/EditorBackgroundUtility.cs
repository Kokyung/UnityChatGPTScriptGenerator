using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace BKK.ChatGPTEditor
{
    public static class EditorBackgroundUtility
    {
        public static void StartBackgroundTask(IEnumerator update, Action end = null)
        {
            void ClosureCallback()
            {
                try
                {
                    if (update.MoveNext() == false)
                    {
                        end?.Invoke();
                        EditorApplication.update -= ClosureCallback;
                    }
                }
                catch (Exception ex)
                {
                    end?.Invoke();
                    Debug.LogException(ex);
                    EditorApplication.update -= ClosureCallback;
                }
            }

            EditorApplication.update += ClosureCallback;
        }
    }
}
