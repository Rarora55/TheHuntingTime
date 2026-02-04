using UnityEngine;
using TheHunt.Environment;

public class PlayerPullState : PlayerGroundState
{
    private IPushable currentPushable;
    private PlayerPushPullController pushPullController;
    private float modifiedSpeed;
    private bool interactInput;
    private const float MIN_DISTANCE = 0.8f;

    public PlayerPullState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        if (pushPullController == null)
        {
            pushPullController = player.GetComponent<PlayerPushPullController>();
        }

        currentPushable = pushPullController?.DetectNearbyPushable();
        
        if (currentPushable != null && currentPushable.CanBePulled)
        {
            pushPullController.StartPulling(currentPushable);
            modifiedSpeed = pushPullController.CalculateSpeedPenalty(currentPushable);
            
            Transform pullPos = player.GetPullPos();
            if (pullPos != null && currentPushable is PushableObject pushableObj)
            {
                pushableObj.AttachToPlayer(pullPos);
            }
            player.anim.SetFloat("pushPullBlend", -1.0f);
        }
        else
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        player.anim.SetFloat("pushPullBlend", 0f);
        
        if (currentPushable is PushableObject pushableObj)
        {
            pushableObj.DetachFromPlayer();
        }
        
        if (pushPullController != null)
        {
            pushPullController.StopPulling();
        }
        
        currentPushable = null;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        xInput = player.InputHandler.NormInputX;
        interactInput = player.InputHandler.PushPullInput;

        if (currentPushable == null || !interactInput)
        {
            stateMachine.ChangeState(player.IdleState);
            return;
        }

        float distanceToObject = Vector2.Distance(
            player.transform.position, 
            currentPushable.GetObjectTransform().position
        );

        if (xInput == 0)
        {
            player.SetVelocityX(0);
        }
        else if (xInput == -player.FacingRight)
        {
            if (distanceToObject > MIN_DISTANCE)
            {
                player.SetVelocityX(modifiedSpeed * xInput);
            }
            else
            {
                player.SetVelocityX(0);
            }
        }
        else
        {
            stateMachine.ChangeState(player.PushState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
