using UnityEngine;

public class PlayerJumpCooldown : MonoBehaviour
{
    private float lastJumpTime = -999f;
    
    [SerializeField] private float jumpCooldown = 0.2f;
    
    public bool CanJump()
    {
        return Time.time >= lastJumpTime + jumpCooldown;
    }
    
    public void RegisterJump()
    {
        lastJumpTime = Time.time;
        Debug.Log($"<color=yellow>[JUMP COOLDOWN] Salto registrado. Próximo salto disponible en: {jumpCooldown}s</color>");
    }
    
    public float GetTimeSinceLastJump()
    {
        return Time.time - lastJumpTime;
    }
}
