using UnityEngine;

public class PlayerCollisionController : IPlayerCollision
{
    private readonly Transform groundCheck;
    private readonly Transform wallCheck;
    private readonly Transform ledgeCheck;
    private readonly Transform ceilingCheck;
    private readonly BoxCollider2D collider;
    private readonly PlayerData playerData;
    private readonly IPlayerOrientation orientation;
    private readonly PlayerEvents events;
    
    private Vector2 workSpace;
    private bool wasGrounded;
    private Vector2 originalColliderOffset;
    private float originalColliderHeight;
    private bool colliderInitialized;
    
    public PlayerCollisionController(
        Transform ground, 
        Transform wall, 
        Transform ledge, 
        Transform ceiling,
        BoxCollider2D boxCollider,
        PlayerData data,
        IPlayerOrientation playerOrientation,
        PlayerEvents playerEvents)
    {
        groundCheck = ground;
        wallCheck = wall;
        ledgeCheck = ledge;
        ceilingCheck = ceiling;
        collider = boxCollider;
        playerData = data;
        orientation = playerOrientation;
        events = playerEvents;
    }
    
    public bool CheckIsGrounded()
    {
        bool centerGrounded = Physics2D.Raycast(
            groundCheck.position, 
            Vector2.down, 
            playerData.GroundCheckRadius, 
            playerData.WhatIsGround);
        
        float horizontalOffset = 0.3f;
        
        Vector2 leftCheckPos = groundCheck.position;
        leftCheckPos.x -= horizontalOffset;
        bool leftGrounded = Physics2D.Raycast(
            leftCheckPos,
            Vector2.down,
            playerData.GroundCheckRadius,
            playerData.WhatIsGround);
        
        Vector2 rightCheckPos = groundCheck.position;
        rightCheckPos.x += horizontalOffset;
        bool rightGrounded = Physics2D.Raycast(
            rightCheckPos,
            Vector2.down,
            playerData.GroundCheckRadius,
            playerData.WhatIsGround);
        
        int groundedCount = (centerGrounded ? 1 : 0) + (leftGrounded ? 1 : 0) + (rightGrounded ? 1 : 0);
        bool isFullyGrounded = groundedCount >= 1;
        
        Color centerColor = centerGrounded ? Color.green : Color.red;
        Color leftColor = leftGrounded ? Color.green : Color.red;
        Color rightColor = rightGrounded ? Color.green : Color.red;
        
        Debug.DrawRay(groundCheck.position, Vector2.down * playerData.GroundCheckRadius, centerColor);
        Debug.DrawRay(leftCheckPos, Vector2.down * playerData.GroundCheckRadius, leftColor);
        Debug.DrawRay(rightCheckPos, Vector2.down * playerData.GroundCheckRadius, rightColor);
        
        if (wasGrounded != isFullyGrounded)
        {
            events?.InvokeGroundedChanged(new PlayerCollisionData
            {
                WasGrounded = wasGrounded,
                IsGrounded = isFullyGrounded
            });
            wasGrounded = isFullyGrounded;
        }
        
        return isFullyGrounded;
    }
    
    public bool CheckIfTouchingWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            wallCheck.position, 
            Vector2.right * orientation.FacingDirection, 
            playerData.WallCheckDistance, 
            playerData.WhatIsGround);
        
        Color debugColor = hit ? Color.green : Color.red;
        Debug.DrawRay(
            wallCheck.position, 
            Vector2.right * orientation.FacingDirection * playerData.WallCheckDistance, 
            debugColor);
        
        return hit;
    }
    
    public bool CheckTouchingLedge()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            ledgeCheck.position, 
            Vector2.right * orientation.FacingDirection, 
            playerData.LedgeCheckDistance, 
            playerData.WhatIsGround);
        
        Color debugColor = hit ? Color.cyan : Color.magenta;
        Debug.DrawRay(
            ledgeCheck.position, 
            Vector2.right * orientation.FacingDirection * playerData.LedgeCheckDistance, 
            debugColor);
        
        return hit;
    }
    
    public bool CheckForCeiling()
    {
        return Physics2D.OverlapCircle(
            ceilingCheck.position, 
            playerData.GroundCheckRadius, 
            playerData.WhatIsGround);
    }
    
    public bool CheckGroundEdgeAhead()
    {
        Vector2 edgeCheckPosition = groundCheck.position;
        edgeCheckPosition.x += orientation.FacingDirection * 0.6f;
        
        bool hasGroundAhead = Physics2D.Raycast(
            edgeCheckPosition,
            Vector2.down,
            playerData.GroundCheckRadius * 2f,
            playerData.WhatIsGround);
        
        Color debugColor = hasGroundAhead ? Color.green : Color.red;
        Debug.DrawRay(edgeCheckPosition, Vector2.down * playerData.GroundCheckRadius * 2f, debugColor);
        
        return !hasGroundAhead;
    }
    
    public bool ShouldAutoGrabLedge()
    {
        bool currentlyGrounded = CheckIsGrounded();
        if (currentlyGrounded)
        {
            return false;
        }
        
        Vector2 forwardCheckPos = groundCheck.position;
        forwardCheckPos.x += orientation.FacingDirection * 0.5f;
        
        bool noGroundAhead = !Physics2D.Raycast(
            forwardCheckPos,
            Vector2.down,
            playerData.GroundCheckRadius * 2.5f,
            playerData.WhatIsGround);
        
        if (!noGroundAhead) 
        {
            return false;
        }
        
        Vector2 wallCheckPos = wallCheck.position;
        wallCheckPos.x += orientation.FacingDirection * 0.3f;
        
        RaycastHit2D wallHit = Physics2D.Raycast(
            wallCheckPos,
            Vector2.down,
            2.5f,
            playerData.WhatIsGround);
        
        Color edgeColor = noGroundAhead ? Color.yellow : Color.green;
        Color wallColor = wallHit ? Color.cyan : Color.red;
        
        Debug.DrawRay(forwardCheckPos, Vector2.down * (playerData.GroundCheckRadius * 2.5f), edgeColor);
        Debug.DrawRay(wallCheckPos, Vector2.down * 2.5f, wallColor);
        
        bool shouldGrab = noGroundAhead && wallHit;
        
        return shouldGrab;
    }
    
    public bool CheckCanGrabLedgeFromAbove()
    {
        bool currentlyGrounded = CheckIsGrounded();
        if (currentlyGrounded)
        {
            return false;
        }
        
        Vector2 forwardCheckPos = groundCheck.position;
        forwardCheckPos.x += orientation.FacingDirection * 0.4f;
        
        bool noGroundAhead = !Physics2D.Raycast(
            forwardCheckPos,
            Vector2.down,
            playerData.GroundCheckRadius * 3f,
            playerData.WhatIsGround);
        
        if (!noGroundAhead) return false;
        
        Vector2 wallCheckPos = wallCheck.position;
        wallCheckPos.x += orientation.FacingDirection * 0.3f;
        
        RaycastHit2D wallHit = Physics2D.Raycast(
            wallCheckPos,
            Vector2.down,
            2.0f,
            playerData.WhatIsGround);
        
        Color edgeColor = noGroundAhead ? Color.yellow : Color.green;
        Color wallColor = wallHit ? Color.cyan : Color.red;
        
        Debug.DrawRay(forwardCheckPos, Vector2.down * (playerData.GroundCheckRadius * 3f), edgeColor);
        Debug.DrawRay(wallCheckPos, Vector2.down * 2.0f, wallColor);
        
        return noGroundAhead && wallHit;
    }
    
    public Vector2 DetermineCornerPosition()
    {
        RaycastHit2D xHit = Physics2D.Raycast(
            wallCheck.position, 
            Vector2.right * orientation.FacingDirection, 
            playerData.WallCheckDistance, 
            playerData.WhatIsGround);
        float xDist = xHit.distance;
        
        workSpace.Set((xDist + 0.015f) * orientation.FacingDirection, 0f);
        
        Vector3 yRayStart = ledgeCheck.position + (Vector3)workSpace;
        float yRayMaxDist = ledgeCheck.position.y - wallCheck.position.y + 0.015f;
        
        RaycastHit2D yHit = Physics2D.Raycast(yRayStart, Vector2.down, yRayMaxDist, playerData.WhatIsGround);
        float yDist = yHit.distance;
        
        Vector2 calculatedCorner = new Vector2(
            wallCheck.position.x + (xDist * orientation.FacingDirection), 
            ledgeCheck.position.y - yDist
        );
        
        Debug.DrawRay(wallCheck.position, Vector2.right * orientation.FacingDirection * xDist, Color.red, 3f);
        Debug.DrawRay(yRayStart, Vector2.down * yDist, Color.blue, 3f);
        Debug.DrawLine(calculatedCorner, calculatedCorner + Vector2.up * 0.5f, Color.green, 3f);
        
        return calculatedCorner;
    }
    
    public Vector2 DetermineCornerPositionFromAbove()
    {
        Vector2 forwardCheckPos = groundCheck.position;
        forwardCheckPos.x += orientation.FacingDirection * 0.5f;
        
        RaycastHit2D groundEdgeHit = Physics2D.Raycast(
            forwardCheckPos,
            Vector2.down,
            2.5f,
            playerData.WhatIsGround);
        
        if (!groundEdgeHit)
        {
            return Vector2.zero;
        }
        
        Vector2 wallCheckPos = wallCheck.position;
        wallCheckPos.x += orientation.FacingDirection * 0.3f;
        
        RaycastHit2D wallHit = Physics2D.Raycast(
            wallCheckPos,
            Vector2.down,
            2.5f,
            playerData.WhatIsGround);
        
        if (!wallHit)
        {
            return Vector2.zero;
        }
        
        Vector2 cornerPos = new Vector2(
            wallHit.point.x + (orientation.FacingDirection * 0.05f),
            wallHit.point.y + 0.05f
        );
        
        Debug.DrawLine(cornerPos, cornerPos + Vector2.up * 0.5f, Color.magenta, 2f);
        Debug.DrawLine(wallHit.point, wallHit.point + Vector2.right * orientation.FacingDirection * 0.3f, Color.cyan, 2f);
        Debug.DrawLine(cornerPos, cornerPos + Vector2.left * orientation.FacingDirection * 0.3f, Color.yellow, 2f);
        
        return cornerPos;
    }
    
    public bool IsValidLedge(float minHeight)
    {
        RaycastHit2D xHit = Physics2D.Raycast(
            wallCheck.position, 
            Vector2.right * orientation.FacingDirection, 
            playerData.WallCheckDistance, 
            playerData.WhatIsGround);
            
        if (!xHit) return false;
        
        float xDist = xHit.distance;
        workSpace.Set((xDist + 0.015f) * orientation.FacingDirection, 0f);
        Vector3 yRayStart = ledgeCheck.position + (Vector3)workSpace;
        float yRayMaxDist = ledgeCheck.position.y - wallCheck.position.y + 0.015f;
        
        RaycastHit2D yHit = Physics2D.Raycast(yRayStart, Vector2.down, yRayMaxDist, playerData.WhatIsGround);
        
        if (!yHit) return false;
        
        float yDist = yHit.distance;
        bool isValid = yDist >= minHeight;
        
        return isValid;
    }
    
    public LedgeType DetectLedgeType()
    {
        bool touchingWall = CheckIfTouchingWall();
        bool touchingLedge = CheckTouchingLedge();
        
        RaycastHit2D wallHit = Physics2D.Raycast(
            wallCheck.position, 
            Vector2.right * orientation.FacingDirection, 
            playerData.WallCheckDistance, 
            playerData.WhatIsGround);
            
        RaycastHit2D ledgeHit = Physics2D.Raycast(
            ledgeCheck.position, 
            Vector2.right * orientation.FacingDirection, 
            playerData.LedgeCheckDistance, 
            playerData.WhatIsGround);
        
        if (wallHit && ledgeHit)
        {
            float heightDifference = Mathf.Abs(wallHit.point.y - ledgeHit.point.y);
            
            if (heightDifference < 0.2f)
            {
                return LedgeType.Edge;
            }
            else
            {
                return LedgeType.Corner;
            }
        }
        
        if (!touchingWall && touchingLedge)
        {
            return LedgeType.Edge;
        }
        
        if (touchingWall && !touchingLedge)
        {
            return LedgeType.Corner;
        }
        
        return LedgeType.None;
    }
    
    public Vector2 DetermineEdgePosition()
    {
        RaycastHit2D ledgeHit = Physics2D.Raycast(
            ledgeCheck.position, 
            Vector2.right * orientation.FacingDirection, 
            playerData.LedgeCheckDistance, 
            playerData.WhatIsGround);
        
        if (!ledgeHit)
        {
            return Vector2.zero;
        }
        
        Vector2 edgePos = new Vector2(
            ledgeHit.point.x - (orientation.FacingDirection * 0.05f),
            ledgeHit.point.y - 0.05f
        );
        
        Debug.DrawLine(edgePos, edgePos + Vector2.up * 0.5f, Color.yellow, 2f);
        
        return edgePos;
    }
    
    public void SetColliderHeight(float height)
    {
        if (collider == null) return;
        
        if (!colliderInitialized)
        {
            originalColliderOffset = collider.offset;
            originalColliderHeight = collider.size.y;
            colliderInitialized = true;
        }
        
        Vector2 size = collider.size;
        float heightDifference = height - originalColliderHeight;
        
        Vector2 newOffset = originalColliderOffset;
        newOffset.y += heightDifference / 2f;
        
        size.y = height;
        
        collider.size = size;
        collider.offset = newOffset;
    }
}
