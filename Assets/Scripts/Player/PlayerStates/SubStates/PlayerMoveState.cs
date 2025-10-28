using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoveState : PlayerGroundState
{
    private bool isTurning;
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

   

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        isTurning = false;
        player.anim.SetBool("turnIt", false);
    }

    public override void Exit()
    {
        base.Exit();
        player.anim.SetBool("turnIt", false);
        isTurning = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isTurning)
        {
            return;
        }

        

        //Logic of drifting here
        if(xInput == 0)
        {
            //
            if (xInput == 0 || xInput == player.FacingRight)
            {
                CancelTurn();
            }
            //
            stateMachine.ChangeState(player.IdleState);
        }
        else if (yInput == -1)
        {
            stateMachine.ChangeState(player.CrouchMoveState);
        }
        else if(xInput != 0 && xInput != player.FacingRight)
        {
            StartTurn();
        }else
        {
            player.SetVelocityX(playerData.movementVelocity * xInput);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        if (isTurning)
        {
            CompleteTurn();
        }

    }
    private void StartTurn()
    {
        isTurning = true;
        player.anim.SetBool("turnIt", true);
        player.SetVelocityX(0f);
    }

    private void CompleteTurn()
    {
        player.Flip();
        isTurning = false;
        player.anim.SetBool("turnIt", false);
    }

    private void CancelTurn()
    {
        isTurning = false;
        player.anim.SetBool("turnIt", false);
    }
}
