using UnityEngine;

public class PlayerGroundedState : PlayerState
{

    protected int xInput;
    private bool jumpInput;
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {


    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormalizeInputX;
        jumpInput = player.InputHandler.JumpInput;

        if (jumpInput)
        {
            player.InputHandler.UseJumpInput();
            stateMachine.ChangeState(player.jumpState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
