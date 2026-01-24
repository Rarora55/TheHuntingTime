using UnityEngine;

public class PlayerLedgeJumpState : PlayerAbilityState
{
    private const float LEDGE_DETECTION_DISTANCE = 1.5f;
    private const float LEDGE_HEIGHT_RANGE_MIN = 0.3f;
    private const float LEDGE_HEIGHT_RANGE_MAX = 1.2f;
    private const float LEDGE_JUMP_FORCE_X = 3f;
    private const float LEDGE_JUMP_FORCE_Y = 8f;

    private bool isJumping = false;
    
    public PlayerLedgeJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        isJumping = false;

        if (IsNearLedge())
        {
            PerformLedgeJump();
        }
        else
        {
            isAbilityDone = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isJumping && Time.time >= startTime + 0.3f)
        {
            isAbilityDone = true;
        }
    }

    private bool IsNearLedge()
    {
        Vector2 playerPos = player.transform.position;
        int facingDir = player.FacingRight;

        RaycastHit2D wallHit = Physics2D.Raycast(
            playerPos,
            Vector2.right * facingDir,
            LEDGE_DETECTION_DISTANCE,
            playerData.WhatIsGround
        );

        if (!wallHit) return false;

        float horizontalDist = wallHit.distance;
        Vector2 topCheckPos = playerPos;
        topCheckPos.x += facingDir * (horizontalDist + 0.1f);

        RaycastHit2D topHit = Physics2D.Raycast(
            topCheckPos,
            Vector2.down,
            LEDGE_HEIGHT_RANGE_MAX,
            playerData.WhatIsGround
        );

        if (!topHit) return false;

        float verticalDist = topHit.point.y - playerPos.y;

        bool isValidLedge = verticalDist >= LEDGE_HEIGHT_RANGE_MIN && verticalDist <= LEDGE_HEIGHT_RANGE_MAX;

        Debug.DrawRay(playerPos, Vector2.right * facingDir * horizontalDist, isValidLedge ? Color.green : Color.red, 1f);
        Debug.DrawRay(topCheckPos, Vector2.down * topHit.distance, isValidLedge ? Color.green : Color.red, 1f);

        return isValidLedge;
    }

    private void PerformLedgeJump()
    {
        int facingDir = player.FacingRight;
        
        player.SetVelocityX(LEDGE_JUMP_FORCE_X * facingDir);
        player.SetVelocityY(LEDGE_JUMP_FORCE_Y);
        
        player.RB.gravityScale = playerData.jumpGravityScale;
        
        isJumping = true;
        
        Debug.Log("<color=green>[LEDGE JUMP] Performed ledge jump!</color>");
    }
}
