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
        LedgeMarker marker = TryGetLedgeMarker();
        if (marker != null && marker.HasValidCorners())
        {
            Vector2 markerCorner = marker.GetCornerPosition(orientation.FacingDirection);
            Debug.DrawLine(markerCorner, markerCorner + Vector2.up * 0.5f, Color.yellow, 3f);
            Debug.Log($"<color=cyan>[DetermineCornerPosition] ✅ Usando MARCADOR en {marker.name} | Corner: {markerCorner} | FacingDir: {orientation.FacingDirection}</color>");
            return markerCorner;
        }
        
        Debug.Log($"<color=orange>[DetermineCornerPosition] ⚠️ No hay marcador, usando RAYCAST | FacingDir: {orientation.FacingDirection}</color>");
        
        // Raycast horizontal para encontrar la pared MÁS CERCANA
        RaycastHit2D xHit = Physics2D.Raycast(
            wallCheck.position, 
            Vector2.right * orientation.FacingDirection, 
            playerData.WallCheckDistance, 
            playerData.WhatIsGround);
        
        // VALIDAR que el raycast golpeó algo
        if (!xHit)
        {
            Debug.LogWarning($"<color=red>[DetermineCornerPosition] ❌ No se detectó pared horizontal! Usando posición de wallCheck</color>");
            return wallCheck.position;
        }
        
        float xDist = xHit.distance;
        
        // Verificar que la distancia es razonable (no es una plataforma muy lejana)
        if (xDist > playerData.WallCheckDistance * 0.9f)
        {
            Debug.LogWarning($"<color=yellow>[DetermineCornerPosition] ⚠️ Pared muy lejana ({xDist}), posible detección incorrecta</color>");
        }
        
        workSpace.Set((xDist + 0.015f) * orientation.FacingDirection, 0f);
        
        Vector3 yRayStart = ledgeCheck.position + (Vector3)workSpace;
        float yRayMaxDist = ledgeCheck.position.y - wallCheck.position.y + 0.015f;
        
        // Raycast vertical para encontrar el TOP de la pared
        RaycastHit2D yHit = Physics2D.Raycast(yRayStart, Vector2.down, yRayMaxDist, playerData.WhatIsGround);
        
        // VALIDAR que el raycast vertical golpeó algo
        if (!yHit)
        {
            Debug.LogWarning($"<color=red>[DetermineCornerPosition] ❌ No se detectó superficie vertical! Usando ledgeCheck.y</color>");
            // Fallback: usar la posición horizontal encontrada pero mantener la altura del ledgeCheck
            Vector2 fallbackCorner = new Vector2(
                wallCheck.position.x + (xDist * orientation.FacingDirection),
                ledgeCheck.position.y
            );
            Debug.DrawLine(fallbackCorner, fallbackCorner + Vector2.up * 0.5f, Color.magenta, 3f);
            return fallbackCorner;
        }
        
        float yDist = yHit.distance;
        
        // Verificar que yDist es razonable
        if (yDist > yRayMaxDist * 0.95f)
        {
            Debug.LogWarning($"<color=yellow>[DetermineCornerPosition] ⚠️ Superficie muy baja, posible error de detección</color>");
        }
        
        Vector2 calculatedCorner = new Vector2(
            wallCheck.position.x + (xDist * orientation.FacingDirection), 
            ledgeCheck.position.y - yDist
        );
        
        // Debug visual mejorado
        Debug.DrawRay(wallCheck.position, Vector2.right * orientation.FacingDirection * xDist, Color.red, 3f);
        Debug.DrawRay(yRayStart, Vector2.down * yDist, Color.blue, 3f);
        Debug.DrawLine(calculatedCorner, calculatedCorner + Vector2.up * 0.5f, Color.green, 3f);
        
        // Log detallado
        Debug.Log($"<color=green>[DetermineCornerPosition] ✅ Calculado | xDist: {xDist:F3} | yDist: {yDist:F3} | Corner: {calculatedCorner} | xHit: {xHit.collider.name} | yHit: {yHit.collider.name}</color>");
        
        return calculatedCorner;
    }
    
    public Vector2 DetermineCornerPositionFromAbove()
    {
        LedgeMarker marker = TryGetLedgeMarker();
        if (marker != null && marker.HasValidCorners())
        {
            Vector2 markerCorner = marker.GetCornerPosition(orientation.FacingDirection);
            Debug.DrawLine(markerCorner, markerCorner + Vector2.up * 0.5f, Color.yellow, 3f);
            Debug.Log($"<color=green>[FROM ABOVE] ✅ Usando MARCADOR: {markerCorner}</color>");
            return markerCorner;
        }
        
        Vector2 forwardCheckPos = groundCheck.position;
        forwardCheckPos.x += orientation.FacingDirection * 0.5f;
        
        RaycastHit2D groundEdgeHit = Physics2D.Raycast(
            forwardCheckPos,
            Vector2.down,
            2.5f,
            playerData.WhatIsGround);
        
        if (!groundEdgeHit)
        {
            Debug.LogWarning($"<color=red>[FROM ABOVE] ❌ No se detectó borde del suelo</color>");
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
            Debug.LogWarning($"<color=red>[FROM ABOVE] ❌ No se detectó pared</color>");
            return Vector2.zero;
        }
        
        Vector2 cornerPos = new Vector2(
            wallHit.point.x + (orientation.FacingDirection * 0.05f),
            wallHit.point.y + 0.05f
        );
        
        Debug.DrawLine(cornerPos, cornerPos + Vector2.up * 0.5f, Color.magenta, 2f);
        Debug.DrawLine(wallHit.point, wallHit.point + Vector2.right * orientation.FacingDirection * 0.3f, Color.cyan, 2f);
        Debug.DrawLine(cornerPos, cornerPos + Vector2.left * orientation.FacingDirection * 0.3f, Color.yellow, 2f);
        
        Debug.Log($"<color=green>[FROM ABOVE] ✅ Calculado | Corner: {cornerPos} | wallHit: {wallHit.collider.name}</color>");
        
        return cornerPos;
    }
    
    public bool IsValidLedge(float minHeight)
    {
        // Raycast horizontal desde wallCheck para detectar pared
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
        
        // Raycast vertical hacia abajo desde ledgeCheck para encontrar el top de la pared
        RaycastHit2D yHit = Physics2D.Raycast(yRayStart, Vector2.down, yRayMaxDist, playerData.WhatIsGround);
        
        if (!yHit) return false;
        
        float yDist = yHit.distance;
        bool hasMinHeight = yDist >= minHeight;
        
        // NUEVA VALIDACIÓN: Verificar que NO hay suelo/pared arriba del ledge
        // Esto previene detectar esquinas internas como ledges válidos
        Vector3 aboveLedgeCheckPos = ledgeCheck.position + (Vector3)workSpace;
        RaycastHit2D aboveCheck = Physics2D.Raycast(
            aboveLedgeCheckPos,
            Vector2.up,
            0.5f, // Verificar 0.5 unidades arriba
            playerData.WhatIsGround);
        
        bool noGroundAbove = !aboveCheck;
        
        // Debug visual
        Color aboveColor = noGroundAbove ? Color.green : Color.red;
        Debug.DrawRay(aboveLedgeCheckPos, Vector2.up * 0.5f, aboveColor, 0.5f);
        
        bool isValid = hasMinHeight && noGroundAbove;
        
        if (!isValid && !noGroundAbove)
        {
            Debug.Log($"<color=yellow>[VALID LEDGE] Ledge rechazado - hay suelo arriba (esquina interna)</color>");
        }
        
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
        LedgeMarker marker = TryGetLedgeMarker();
        if (marker != null && marker.HasValidCorners())
        {
            Vector2 markerCorner = marker.GetCornerPosition(orientation.FacingDirection);
            Debug.DrawLine(markerCorner, markerCorner + Vector2.up * 0.5f, Color.yellow, 3f);
            Debug.Log($"<color=magenta>[LEDGE MARKER] Usando edge marcado: {markerCorner}</color>");
            return markerCorner;
        }
        
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
    
    private LedgeMarker TryGetLedgeMarker()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            wallCheck.position, 
            Vector2.right * orientation.FacingDirection, 
            playerData.WallCheckDistance, 
            playerData.WhatIsGround);
        
        if (hits.Length == 0)
        {
            return null;
        }
        
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;
            
            LedgeMarker marker = hit.collider.GetComponent<LedgeMarker>();
            if (marker != null && marker.HasValidCorners())
            {
                Debug.Log($"<color=yellow>[LEDGE MARKER] Detectado: {hit.collider.name} a distancia {hit.distance:F2}</color>");
                return marker;
            }
        }
        
        return null;
    }
}
