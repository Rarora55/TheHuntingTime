using UnityEngine;

public static class DebugManager
{
    [System.Flags]
    public enum DebugCategory
    {
        None = 0,
        PlayerStates = 1 << 0,
        WallInteraction = 1 << 1,
        Climbing = 1 << 2,
        Jumping = 1 << 3,
        Movement = 1 << 4,
        Stamina = 1 << 5,
        Health = 1 << 6,
        Combat = 1 << 7,
        Interaction = 1 << 8,
        Environment = 1 << 9,
        All = ~0
    }
    
    private static DebugCategory enabledCategories = DebugCategory.None;
    
    public static void EnableCategory(DebugCategory category)
    {
        enabledCategories |= category;
    }
    
    public static void DisableCategory(DebugCategory category)
    {
        enabledCategories &= ~category;
    }
    
    public static void SetCategories(DebugCategory categories)
    {
        enabledCategories = categories;
    }
    
    public static bool IsEnabled(DebugCategory category)
    {
        return (enabledCategories & category) != 0;
    }
    
    public static void Log(DebugCategory category, string message)
    {
        if (IsEnabled(category))
        {
            Debug.Log($"[{category}] {message}");
        }
    }
    
    public static void LogWarning(DebugCategory category, string message)
    {
        if (IsEnabled(category))
        {
            Debug.LogWarning($"[{category}] {message}");
        }
    }
    
    public static void LogError(DebugCategory category, string message)
    {
        if (IsEnabled(category))
        {
            Debug.LogError($"[{category}] {message}");
        }
    }
}
