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
    
    public void SetColliderHeight(float height)
    {
        if (collider == null) return;
        
        if (!colliderInitialized)
        {
            originalColliderOffset = collider.offset;
            originalColliderHeight = collider.size.y;
            colliderInitialized = true;
            Debug.Log($"[COLLIDER] Valores originales guardados - Offset: {originalColliderOffset}, Height: {originalColliderHeight}");
        }
        
        Vector2 size = collider.size;
        float heightDifference = height - originalColliderHeight;
        
        Vector2 newOffset = originalColliderOffset;
        newOffset.y += heightDifference / 2f;
        
        size.y = height;
        
        collider.size = size;
        collider.offset = newOffset;
        
        Debug.Log($"[COLLIDER] Altura cambiada a {height:F2} | Offset: {newOffset} | Size: {size}");
    }
}
