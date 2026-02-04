using UnityEngine;
using System.Diagnostics;

public static class DebugLogger
{
    [Conditional("ENABLE_DEBUG_LOGS")]
    public static void Log(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    [Conditional("ENABLE_DEBUG_LOGS")]
    public static void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

    [Conditional("ENABLE_DEBUG_LOGS")]
    public static void LogError(object message)
    {
        UnityEngine.Debug.LogError(message);
    }

    [Conditional("ENABLE_DEBUG_LOGS")]
    public static void Log(object message, Object context)
    {
        UnityEngine.Debug.Log(message, context);
    }

    [Conditional("ENABLE_DEBUG_LOGS")]
    public static void LogWarning(object message, Object context)
    {
        UnityEngine.Debug.LogWarning(message, context);
    }

    [Conditional("ENABLE_DEBUG_LOGS")]
    public static void LogError(object message, Object context)
    {
        UnityEngine.Debug.LogError(message, context);
    }
}
