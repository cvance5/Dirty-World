using UnityEngine;

public static class Log
{
    public static void Error(string message)
    {
#if UNITY_EDITOR
        Debug.LogError(message);
#endif 
    }
    public static void Warning(string message)
    {
#if UNITY_EDITOR
        Debug.LogWarning(message);
#endif
    }
    public static void Info(string message, string colorName = "black")
    {
#if UNITY_EDITOR
        Debug.Log($"<color={colorName}>" + message + "</color>");
#endif
    }

    public static void ErrorIfNull(object obj, string message)
    {
        if (obj == null) Error(message);
    }
}
