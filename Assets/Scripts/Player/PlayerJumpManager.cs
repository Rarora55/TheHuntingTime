using UnityEngine;

public static class PlayerJumpManager
{
    private static float lastJumpTime = -999f;
    public const float JUMP_COOLDOWN = 0.2f;
    
    public static bool CanJump()
    {
        float timeSinceLastJump = Time.time - lastJumpTime;
        return timeSinceLastJump >= JUMP_COOLDOWN;
    }
    
    public static void RegisterJump()
    {
        lastJumpTime = Time.time;
        Debug.Log($"<color=yellow>[JUMP MANAGER] Salto registrado en t={Time.time:F2}. Próximo salto disponible en {JUMP_COOLDOWN}s</color>");
    }
    
    public static float GetTimeSinceLastJump()
    {
        return Time.time - lastJumpTime;
    }
    
    public static float GetRemainingCooldown()
    {
        float remaining = JUMP_COOLDOWN - GetTimeSinceLastJump();
        return Mathf.Max(0, remaining);
    }
}
