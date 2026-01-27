using UnityEngine;

public class DeathAnimatorDebugger : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Sprite lastSprite;
    private bool inDeathState = false;
    private int frameInDeath = 0;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (animator == null)
            Debug.LogError("[DEATH ANIM DEBUG] No Animator found!");
        if (spriteRenderer == null)
            Debug.LogError("[DEATH ANIM DEBUG] No SpriteRenderer found!");
    }
    
    void Update()
    {
        if (animator == null || spriteRenderer == null)
            return;
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        bool isInDeathAnim = stateInfo.IsName("death") || stateInfo.IsName("Death") || stateInfo.IsName("deathAnim");
        
        if (isInDeathAnim)
        {
            if (!inDeathState)
            {
                inDeathState = true;
                frameInDeath = 0;
                lastSprite = spriteRenderer.sprite;
                Debug.Log("<color=red>═══════════════════════════════════════</color>");
                Debug.Log("<color=red>   DEATH STATE ENTERED - DEBUG START</color>");
                Debug.Log("<color=red>═══════════════════════════════════════</color>");
            }
            
            frameInDeath++;
            
            if (frameInDeath <= 20)
            {
                string spriteStatus = (spriteRenderer.sprite != lastSprite) ? "CHANGED ✅" : "SAME ⚠️";
                
                Debug.Log($"<color=cyan>[F{frameInDeath}] NormTime:{stateInfo.normalizedTime:F4} | StateSpd:{stateInfo.speed:F2} | AnimSpd:{animator.speed:F2} | Mult:{stateInfo.speedMultiplier:F2} | Sprite:{spriteStatus}</color>");
                
                if (spriteRenderer.sprite != lastSprite)
                {
                    lastSprite = spriteRenderer.sprite;
                }
                
                if (stateInfo.speed == 0f)
                {
                    Debug.LogError($"<color=red>[FRAME {frameInDeath}] ❌ STATE SPEED IS ZERO!</color>");
                }
                
                if (animator.speed == 0f)
                {
                    Debug.LogError($"<color=red>[FRAME {frameInDeath}] ❌ ANIMATOR.SPEED IS ZERO!</color>");
                }
                
                if (stateInfo.speedMultiplier == 0f)
                {
                    Debug.LogError($"<color=red>[FRAME {frameInDeath}] ❌ SPEED MULTIPLIER IS ZERO!</color>");
                }
                
                if (stateInfo.normalizedTime == 0f && frameInDeath > 5)
                {
                    Debug.LogError($"<color=red>[FRAME {frameInDeath}] ❌ NORMALIZED TIME STUCK AT ZERO!</color>");
                }
            }
            
            if (frameInDeath == 21)
            {
                Debug.Log("<color=yellow>[DEATH ANIM DEBUG] Stopping verbose logging (20 frames captured)</color>");
            }
        }
        else
        {
            if (inDeathState)
            {
                Debug.Log("<color=green>═══════════════════════════════════════</color>");
                Debug.Log($"<color=green>   EXITED DEATH STATE AFTER {frameInDeath} FRAMES</color>");
                Debug.Log("<color=green>═══════════════════════════════════════</color>");
                inDeathState = false;
            }
        }
    }
}
