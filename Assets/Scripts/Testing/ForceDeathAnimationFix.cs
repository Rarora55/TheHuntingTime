using UnityEngine;

public class ForceDeathAnimationFix : MonoBehaviour
{
    private Animator animator;
    private Player player;
    private bool wasInDeathState = false;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
    }
    
    void Update()
    {
        if (player == null || animator == null)
            return;
        
        bool isInDeathState = player.StateMachine?.CurrentState is PlayerDeathState;
        
        if (isInDeathState && !wasInDeathState)
        {
            Debug.Log("<color=magenta>[FORCE ANIM FIX] Death state detected! Forcing CrossFade to death animation</color>");
            
            animator.CrossFade("Death", 0f, 0, 0f);
            
            wasInDeathState = true;
        }
        else if (!isInDeathState && wasInDeathState)
        {
            wasInDeathState = false;
        }
    }
}
