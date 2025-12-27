using UnityEngine;

public class AimSystemDebug : MonoBehaviour
{
    private PlayerInputHandler inputHandler;
    private Player player;
    private Animator animator;
    
    private void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        
        if (inputHandler == null)
            Debug.LogError("<color=red>[AIM DEBUG] PlayerInputHandler NOT FOUND!</color>");
        
        if (player == null)
            Debug.LogError("<color=red>[AIM DEBUG] Player NOT FOUND!</color>");
            
        if (animator == null)
            Debug.LogError("<color=red>[AIM DEBUG] Animator NOT FOUND!</color>");
        else
        {
            Debug.Log("<color=green>[AIM DEBUG] Checking Animator Parameters:</color>");
            foreach (var param in animator.parameters)
            {
                Debug.Log($"<color=cyan>[AIM DEBUG] Parameter: {param.name} | Type: {param.type}</color>");
            }
        }
    }
    
    private void Update()
    {
        if (inputHandler == null) return;
        
        if (inputHandler.AimInput)
        {
            Debug.Log($"<color=yellow>[AIM DEBUG] AimInput = TRUE | Current State: {player.StateMachine.CurrentState.GetType().Name}</color>");
            
            if (animator != null)
            {
                Debug.Log($"<color=magenta>[AIM DEBUG] Animator 'aim' param = {animator.GetBool("aim")}</color>");
            }
        }
        
        if (inputHandler.FireInput)
        {
            Debug.Log($"<color=orange>[AIM DEBUG] FireInput = TRUE</color>");
        }
        
        if (inputHandler.ReloadInput)
        {
            Debug.Log($"<color=green>[AIM DEBUG] ReloadInput = TRUE</color>");
        }
    }
}
