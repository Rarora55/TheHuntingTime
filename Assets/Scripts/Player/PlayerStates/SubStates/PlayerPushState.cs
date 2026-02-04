using UnityEngine;
using TheHunt.Environment;

public class PlayerPushState : PlayerGroundState
{
    private IPushable currentPushable;
    private PlayerPushPullController pushPullController;
    private float modifiedSpeed;
    private bool interactInput;

    public PlayerPushState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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
        
        if (currentPushable != null && currentPushable.CanBePushed)
        {
            pushPullController.StartPushing(currentPushable);
            modifiedSpeed = pushPullController.CalculateSpeedPenalty(currentPushable);
            player.anim.SetFloat("pushPullBlend", 1.0f);
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
        
        if (pushPullController != null)
        {
            pushPullController.StopPushing();
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

        if (xInput == 0)
        {
            player.SetVelocityX(0);
        }
        else if (xInput == player.FacingRight)
        {
            player.SetVelocityX(modifiedSpeed * xInput);
            ApplyPushForce();
        }
        else
        {
            stateMachine.ChangeState(player.PullState);
        }
    }

    private void ApplyPushForce()
    {
        if (currentPushable == null)
            return;

        Rigidbody2D objectRb = currentPushable.GetRigidbody();
        
        if (objectRb != null)
        {
            objectRb.linearVelocity = new Vector2(player.RB.linearVelocity.x, objectRb.linearVelocity.y);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
