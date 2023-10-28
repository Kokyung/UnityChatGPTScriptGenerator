using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BKKChatGPT
{
    public static string GPT35Turbo = "gpt-3.5-turbo";
    public static string GPT4 = "gpt-4";
    public static string TextDavinci003 = "text-davinci-003";
    
    public static readonly string[] modelOptions = new string[]
    {
        TextDavinci003, GPT35Turbo, GPT4
    };
    
    public enum ModelEnum
    {
        TextDavinci003,
        GPT35Turbo,
        GPT4,
    }
}
