using UnityEngine;

public class FireTransitionDebugger : MonoBehaviour
{
    private Animator animator;
    private PlayerInputHandler inputHandler;
    private Player player;

    private void Start()
    {
        animator = GetComponent<Animator>();
        inputHandler = GetComponent<PlayerInputHandler>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (animator == null || inputHandler == null || player == null)
            return;

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        if (stateInfo.IsName("Aim"))
        {
            bool aimParam = animator.GetBool("aim");
            bool fireParam = animator.GetBool("fire");
            
            if (inputHandler.FireInput)
            {
                Debug.Log($"<color=yellow>[FIRE DEBUG] FireInput=TRUE | aim={aimParam} fire={fireParam} | State: {player.StateMachine.CurrentState.GetType().Name}</color>");
                
                if (!fireParam)
                {
                    Debug.LogError("<color=red>[FIRE DEBUG] ⚠️ FireInput is TRUE but 'fire' animator param is FALSE!</color>");
                }
                
                if (animator.IsInTransition(0))
                {
                    var transInfo = animator.GetAnimatorTransitionInfo(0);
                    Debug.Log($"<color=green>[FIRE DEBUG] ✓ In transition! Duration: {transInfo.duration} Progress: {transInfo.normalizedTime}</color>");
                }
                else
                {
                    Debug.LogWarning("<color=orange>[FIRE DEBUG] ⚠️ NOT transitioning despite fire param!</color>");
                }
            }
        }
        
        if (stateInfo.IsName("Fire"))
        {
            Debug.Log($"<color=cyan>[FIRE DEBUG] ✓ IN FIRE STATE! | NormalizedTime: {stateInfo.normalizedTime} | Speed: {stateInfo.speed} | Animator.speed: {animator.speed} | Hash: {stateInfo.fullPathHash}</color>");
        }
    }
}
