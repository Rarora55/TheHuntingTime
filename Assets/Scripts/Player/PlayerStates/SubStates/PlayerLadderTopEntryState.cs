using UnityEngine;
using TheHunt.Environment;

public class PlayerLadderTopEntryState : PlayerState
{
    private bool hasEnteredLadder;
    private Ladder detectedLadder;
    
    public PlayerLadderTopEntryState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        hasEnteredLadder = false;
        DetectLadderBelow();
    }

    public override void Exit()
    {
        base.Exit();
        detectedLadder = null;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (hasEnteredLadder)
        {
            return;
        }
        
        int yInput = player.InputHandler.NormInputY;
        bool grabInput = player.InputHandler.GrabInput;
        
        if (yInput == -1 && grabInput && detectedLadder != null)
        {
            EnterLadderFromTop();
        }
        else
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    void DetectLadderBelow()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.transform.position, 1.5f);
        
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("FrontLadder"))
            {
                Ladder ladder = collider.GetComponent<Ladder>();
                
                if (ladder != null && ladder.IsPlayerAtTop(player.transform.position))
                {
                    detectedLadder = ladder;
                    player.SetCurrentLadder(collider);
                    break;
                }
            }
        }
    }

    void EnterLadderFromTop()
    {
        hasEnteredLadder = true;
        
        if (player.FacingRight == 1)
        {
            player.Flip();
        }
        
        player.SetVelocityZero();
        player.RB.gravityScale = 0f;
        
        stateMachine.ChangeState(player.LadderClimbState);
    }
}
