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
        bool isGrounded = Physics2D.Raycast(
            groundCheck.position, 
            Vector2.down, 
            playerData.GroundCheckRadius, 
            playerData.WhatIsGround);
        
        if (wasGrounded != isGrounded)
        {
            events?.InvokeGroundedChanged(new PlayerCollisionData
            {
                WasGrounded = wasGrounded,
                IsGrounded = isGrounded
            });
            wasGrounded = isGrounded;
        }
        
        return isGrounded;
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
        
        if (shouldGrab)
        {
            Debug.Log($"<color=magenta>[AUTO LEDGE] ¡Detectado! NoGroundAhead={noGroundAhead}, WallBelow={wallHit.collider.name} at {wallHit.point}</color>");
        }
        
        return shouldGrab;
    }
    
    public bool CheckCanGrabLedgeFromAbove()
    {
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
        
        Debug.Log($"<color=cyan>[CORNER] xRaycast desde WallCheck.pos: {wallCheck.position} → Hit: {xHit.point} | Dist: {xDist:F3}</color>");
        
        workSpace.Set((xDist + 0.015f) * orientation.FacingDirection, 0f);
        
        Vector3 yRayStart = ledgeCheck.position + (Vector3)workSpace;
        float yRayMaxDist = ledgeCheck.position.y - wallCheck.position.y + 0.015f;
        
        RaycastHit2D yHit = Physics2D.Raycast(yRayStart, Vector2.down, yRayMaxDist, playerData.WhatIsGround);
        float yDist = yHit.distance;
        
        Debug.Log($"<color=cyan>[CORNER] yRaycast desde LedgeCheck offset: {yRayStart} → Hit: {yHit.point} | Dist: {yDist:F3} | MaxDist: {yRayMaxDist:F3}</color>");
        
        Vector2 calculatedCorner = new Vector2(
            wallCheck.position.x + (xDist * orientation.FacingDirection), 
            ledgeCheck.position.y - yDist
        );
        
        Debug.Log($"<color=green>[CORNER] Resultado final: {calculatedCorner}</color>");
        
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
            Debug.Log("<color=red>[CORNER FROM ABOVE] No se detectó suelo adelante</color>");
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
            Debug.Log("<color=red>[CORNER FROM ABOVE] No se detectó pared debajo</color>");
            return Vector2.zero;
        }
        
        Vector2 cornerPos = new Vector2(
            wallHit.point.x + (orientation.FacingDirection * 0.05f),
            wallHit.point.y + 0.05f
        );
        
        Debug.Log($"<color=magenta>[CORNER FROM ABOVE] Corner calculado: {cornerPos} | WallHit: {wallHit.point} | GroundCheck: {groundCheck.position}</color>");
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
    
    public void SetColliderHeight(float height)
    {
        if (collider == null) return;
        
        if (!colliderInitialized)
        {
            originalColliderOffset = collider.offset;
            originalColliderHeight = collider.size.y;
            colliderInitialized = true;
            Debug.Log($"<color=cyan>[COLLIDER INIT] Offset original: {originalColliderOffset} | Height original: {originalColliderHeight}</color>");
        }
        
        Vector2 size = collider.size;
        float heightDifference = height - originalColliderHeight;
        
        Vector2 newOffset = originalColliderOffset;
        newOffset.y += heightDifference / 2f;
        
        size.y = height;
        
        Debug.Log($"<color=yellow>[COLLIDER] Cambiando altura de {collider.size.y:F2} a {height:F2}</color>");
        Debug.Log($"<color=yellow>[COLLIDER] HeightDiff: {heightDifference:F3} | Offset anterior: {collider.offset} → Nuevo: {newOffset}</color>");
        Debug.Log($"<color=yellow>[COLLIDER] GroundCheck.localPos ANTES: {groundCheck.localPosition}</color>");
        
        collider.size = size;
        collider.offset = newOffset;
        
        Debug.Log($"<color=yellow>[COLLIDER] GroundCheck.localPos DESPUÉS: {groundCheck.localPosition}</color>");
    }
}
