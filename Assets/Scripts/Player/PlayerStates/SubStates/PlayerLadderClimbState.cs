using UnityEngine;

public class PlayerLadderClimbState : PlayerAbilityState
{
    private int yInput;
    private bool jumpInput;
    private bool grabInput;
    private bool isOnLadder;
    
    private Collider2D currentLadder;
    
    private const string LADDER_ANIM_NAME = "LadderClimbFront";
    private int ladderAnimHash;
    
    private bool isTransitioningToLedge;
    private bool hasTriggeredLedge;
    private float startYPosition;
    private const float MIN_CLIMB_DISTANCE = 0.3f;
    private const float MIN_LEDGE_HEIGHT = 0.15f;
    
    public PlayerLadderClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) 
        : base(player, stateMachine, playerData, animBoolName)
    {
        ladderAnimHash = Animator.StringToHash(LADDER_ANIM_NAME);
    }

    public override void Enter()
    {
        base.Enter();
        
        player.SetVelocityZero();
        player.RB.gravityScale = 0f;
        
        player.anim.Play(ladderAnimHash, 0, 0f);
        player.anim.speed = 1f;
        
        isTransitioningToLedge = false;
        hasTriggeredLedge = false;
        startYPosition = player.transform.position.y;
    }

    public override void Exit()
    {
        base.Exit();
        
        player.RB.gravityScale = playerData.fallGravityScale;
        player.anim.speed = 1f;
    }

    public override void LogicUpdate()
    {
        if (isTransitioningToLedge)
        {
            return;
        }
        
        yInput = player.InputHandler.NormInputY;
        jumpInput = player.InputHandler.JumpInput;
        grabInput = player.InputHandler.GrabInput;
        
        AnimatorStateInfo currentState = player.anim.GetCurrentAnimatorStateInfo(0);
        
        if (currentState.shortNameHash != ladderAnimHash)
        {
            player.anim.Play(ladderAnimHash, 0, 0f);
        }
        else if (currentState.normalizedTime >= 0.85f)
        {
            player.anim.Play(ladderAnimHash, 0, 0f);
        }
        
        if (IsClimbingRope() && !hasTriggeredLedge)
        {
            float distanceClimbed = player.transform.position.y - startYPosition;
            bool canTriggerLedge = distanceClimbed >= MIN_CLIMB_DISTANCE;
            bool isValidLedge = player.Collision != null && player.Collision.IsValidLedge(MIN_LEDGE_HEIGHT);
            
            if (yInput == 1 && grabInput && canTriggerLedge && isValidLedge)
            {
                if (CheckLedgeIsNearAndValid())
                {
                    hasTriggeredLedge = true;
                    isTransitioningToLedge = true;
                    
                    player.SetVelocityZero();
                    player.RB.gravityScale = 0f;
                    
                    if (player.FacingRight == 1)
                    {
                        player.Flip();
                    }
                    
                    stateMachine.ChangeState(player.WallLedgeState);
                    return;
                }
            }
        }
        
        if (!IsOnLadder())
        {
            isAbilityDone = true;
            base.LogicUpdate();
            return;
        }
        
        if (!grabInput)
        {
            isAbilityDone = true;
            base.LogicUpdate();
            return;
        }
        
        if (jumpInput)
        {
            player.InputHandler.JumpEnded();
            isAbilityDone = true;
            base.LogicUpdate();
            return;
        }
        
        if (yInput == 1)
        {
            player.SetVelocityY(playerData.ladderClimbSpeed);
            player.anim.speed = 1.2f;
        }
        else if (yInput == -1)
        {
            player.SetVelocityY(-playerData.ladderSlideSpeed);
            player.anim.speed = 1.2f;
        }
        else
        {
            player.SetVelocityY(0f);
            player.anim.speed = 0.3f;
        }
        
        player.SetVelocityX(0f);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    private bool IsOnLadder()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.transform.position, 0.5f);
        
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("FrontLadder"))
            {
                currentLadder = collider;
                return true;
            }
        }
        
        return false;
    }

    private bool IsClimbingRope()
    {
        if (currentLadder == null)
        {
            return false;
        }
        
        RopeClimbable rope = currentLadder.GetComponent<RopeClimbable>();
        return rope != null;
    }

    private bool CheckLedgeIsNearAndValid()
    {
        Vector2 playerPos = player.transform.position;
        float maxHorizontalDistance = 1.5f;
        float minVerticalDistance = 0.2f;
        float maxVerticalDistance = 1.5f;
        
        RaycastHit2D hitRight = Physics2D.Raycast(
            playerPos,
            Vector2.right,
            maxHorizontalDistance,
            playerData.WhatIsGround
        );
        
        RaycastHit2D hitLeft = Physics2D.Raycast(
            playerPos,
            Vector2.left,
            maxHorizontalDistance,
            playerData.WhatIsGround
        );
        
        RaycastHit2D wallHit = hitRight ? hitRight : hitLeft;
        
        if (!wallHit)
        {
            return false;
        }
        
        float horizontalDist = wallHit.distance;
        
        Vector2 topCheckPos = playerPos;
        topCheckPos.x += (hitRight ? 1 : -1) * (horizontalDist + 0.1f);
        
        RaycastHit2D topHit = Physics2D.Raycast(
            topCheckPos,
            Vector2.down,
            maxVerticalDistance,
            playerData.WhatIsGround
        );
        
        if (!topHit)
        {
            return false;
        }
        
        float verticalDist = topHit.point.y - playerPos.y;
        
        bool isInValidRange = verticalDist >= minVerticalDistance && verticalDist <= maxVerticalDistance;
        
        Color debugColor = isInValidRange ? Color.green : Color.yellow;
        Debug.DrawRay(playerPos, (hitRight ? Vector2.right : Vector2.left) * horizontalDist, debugColor, 1f);
        Debug.DrawRay(topCheckPos, Vector2.down * topHit.distance, debugColor, 1f);
        
        return isInValidRange;
    }

    public void SetLadder(Collider2D ladder)
    {
        currentLadder = ladder;
        isOnLadder = true;
    }

    public void ClearLadder()
    {
        currentLadder = null;
        isOnLadder = false;
    }
}
