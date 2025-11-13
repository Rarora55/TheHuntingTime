using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player : MonoBehaviour
{
    #region States
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerAirState AirState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    //public PlayerFallState FallState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerWallClimbState WallClimbState { get; private set; }
    public PlayerWallGrapState WallGrapState { get; private set; }
    public PlayerWallSlicedState WallSlicedState { get; private set; }
    public PlayerLedgeClimbState WallLedgeState { get; private set; }
    public PlayerCrouchIdleState CrouchIdleState { get; private set; }
    public PlayerCrouchMoveState CrouchMoveState { get; private set; }
   



    [SerializeField] private PlayerData PlayerData;
    #endregion

    #region Core Systems
    public IPlayerPhysics Physics { get; private set; }
    public IPlayerCollision Collision { get; private set; }
    public IPlayerAnimation Animation { get; private set; }
    public IPlayerOrientation Orientation { get; private set; }
    public PlayerEvents Events { get; private set; }
    #endregion

    #region Components
    public Animator anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public BoxCollider2D moveCollider { get; private set; }
    #endregion

    #region Movements Vectors
    private Vector2 workSpace;
    public Vector2 CurrentVelocity { get; private set; }
    #endregion

    #region Check Transforms
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private Transform WallCheck;
    [SerializeField] private Transform LedgeCheck;
    [SerializeField] private Transform ceilingCheck;
    #endregion

    #region Checks Var
    public int FacingRight 
    { 
        get 
        {
            if (Orientation != null)
                return Orientation.FacingDirection;
            return facingRightFallback;
        }
        private set
        {
            facingRightFallback = value;
        }
    }
    private int facingRightFallback = 1;
    
    public bool JustFinishedLedgeClimb { get; set; }
    #endregion

    //Methods

    #region Unity CallBacks
    private void Awake()
    {
        InitializeCoreSystems();
        
        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, StateMachine, PlayerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, PlayerData, "move");
        AirState = new PlayerAirState(this, StateMachine, PlayerData, "inAir");
        JumpState = new PlayerJumpState(this, StateMachine, PlayerData, "inAir");
        LandState = new PlayerLandState(this, StateMachine, PlayerData, "land");
        WallClimbState = new PlayerWallClimbState(this, StateMachine, PlayerData, "climbWall");
        WallGrapState = new PlayerWallGrapState(this, StateMachine, PlayerData, "grabWall");
        WallSlicedState = new PlayerWallSlicedState(this, StateMachine, PlayerData, "wallSlide");
        WallLedgeState = new PlayerLedgeClimbState(this, StateMachine, PlayerData, "ledge");
        CrouchIdleState = new PlayerCrouchIdleState(this, StateMachine, PlayerData, "crouchIdle");
        CrouchMoveState = new PlayerCrouchMoveState(this, StateMachine, PlayerData, "crouchMove");
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();
        moveCollider = GetComponent<BoxCollider2D>();
        FacingRight = 1;
        
        Collision = new PlayerCollisionController(
            GroundCheck, WallCheck, LedgeCheck, ceilingCheck,
            moveCollider, PlayerData, Orientation, Events);
        Animation = new PlayerAnimationController(anim, StateMachine, Events);
        
        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        if (Physics != null)
            Physics.UpdateVelocity();
        else
            CurrentVelocity = RB.linearVelocity;
            
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    #endregion

    #region Core System Initialization
    private void InitializeCoreSystems()
    {
        Events = new PlayerEvents();
        
        Orientation = new PlayerOrientationController(transform, Events, initialDirection: 1);
        Physics = new PlayerPhysicsController(GetComponent<Rigidbody2D>(), Events);
    }
    #endregion

    #region Set Functions

    public void SetVelocityZero()
    {
        if (Physics != null)
        {
            Physics.SetVelocityZero();
        }
        else
        {
            RB.linearVelocity = Vector2.zero;
            CurrentVelocity = Vector2.zero;
        }
    }

    public void SetVelocityX(float velocity)
    {
        if (Physics != null)
        {
            Physics.SetVelocityX(velocity);
        }
        else
        {
            workSpace.Set(velocity, RB.linearVelocity.y);
            RB.linearVelocity = workSpace;
            CurrentVelocity = workSpace;
        }
    }

    public void SetVelocityY(float velocity)
    {
        if (Physics != null)
        {
            Physics.SetVelocityY(velocity);
        }
        else
        {
            workSpace.Set(RB.linearVelocity.x, velocity);
            RB.linearVelocity = workSpace;
            CurrentVelocity = workSpace;
        }
    }
    #endregion

    #region Checks

    public bool CheckIsGrounded()
    {
        if (Collision != null)
            return Collision.CheckIsGrounded();
        return Physics2D.Raycast(GroundCheck.position, Vector2.down, PlayerData.GroundCheckRadius, PlayerData.WhatIsGround);
    }

    public bool CheckIfTouchingWall()
    {
        if (Collision != null)
            return Collision.CheckIfTouchingWall();
            
        RaycastHit2D hit = Physics2D.Raycast(WallCheck.position, Vector2.right * FacingRight, PlayerData.WallCheckDistance, PlayerData.WhatIsGround);
        
        Color debugColor = hit ? Color.green : Color.red;
        Debug.DrawRay(WallCheck.position, Vector2.right * FacingRight * PlayerData.WallCheckDistance, debugColor);
        
        return hit;
    }

    public bool CheckTouchingLedge()
    {
        if (Collision != null)
            return Collision.CheckTouchingLedge();
            
        RaycastHit2D hit = Physics2D.Raycast(LedgeCheck.position , Vector2.right * FacingRight, PlayerData.LedgeCheckDistance, PlayerData.WhatIsGround);
        
        Color debugColor = hit ? Color.cyan : Color.magenta;
        Debug.DrawRay(LedgeCheck.position, Vector2.right * FacingRight * PlayerData.LedgeCheckDistance, debugColor);
        
        return hit;
    }
   
    public bool CheckForCeiling()
    {
        if (Collision != null)
            return Collision.CheckForCeiling();
        return Physics2D.OverlapCircle(ceilingCheck.position, PlayerData.GroundCheckRadius, PlayerData.WhatIsGround);
    }
    
    public void CheckFlip(int xInput)
    {
        if (Orientation != null)
        {
            Orientation.CheckFlip(xInput);
        }
        else
        {
            if(xInput != 0 && xInput != FacingRight)
            {
                Flip();
            }
        }
    }
    #endregion

    #region Others
    public void SetColliderHeight(float height)
    {
        if (Collision != null)
        {
            Collision.SetColliderHeight(height);
        }
        else
        {
            Vector2 center = moveCollider.offset;
            workSpace.Set(moveCollider.size.x, height);
            center.y += (height - moveCollider.size.y) / 2;
            moveCollider.size = workSpace;
            moveCollider.offset = center;
        }
    }
    
    public Vector2 DeterminetCornerPos()
    {
        if (Collision != null)
            return Collision.DetermineCornerPosition();
        RaycastHit2D xHit = Physics2D.Raycast(WallCheck.position, Vector2.right * FacingRight, PlayerData.WallCheckDistance, PlayerData.WhatIsGround);
        float xDist = xHit.distance;
        
        Debug.Log($"<color=cyan>[CORNER] xRaycast desde WallCheck.pos: {WallCheck.position} → Hit: {xHit.point} | Dist: {xDist:F3}</color>");
        
        workSpace.Set((xDist + 0.015f) * FacingRight, 0f);
        
        Vector3 yRayStart = LedgeCheck.position + (Vector3)workSpace;
        float yRayMaxDist = LedgeCheck.position.y - WallCheck.position.y + 0.015f;
        
        RaycastHit2D yHit = Physics2D.Raycast(yRayStart, Vector2.down, yRayMaxDist, PlayerData.WhatIsGround);
        float yDist = yHit.distance;
        
        Debug.Log($"<color=cyan>[CORNER] yRaycast desde LedgeCheck offset: {yRayStart} → Hit: {yHit.point} | Dist: {yDist:F3} | MaxDist: {yRayMaxDist:F3}</color>");
        
        Vector2 calculatedCorner = new Vector2(
            WallCheck.position.x + (xDist * FacingRight), 
            LedgeCheck.position.y - yDist
        );
        
        Debug.Log($"<color=green>[CORNER] Resultado final: {calculatedCorner}</color>");
        
        Debug.DrawRay(WallCheck.position, Vector2.right * FacingRight * xDist, Color.red, 3f);
        Debug.DrawRay(yRayStart, Vector2.down * yDist, Color.blue, 3f);
        Debug.DrawLine(calculatedCorner, calculatedCorner + Vector2.up * 0.5f, Color.green, 3f);
        
        workSpace = calculatedCorner;
        return workSpace;
    }

    public void AnimationTrigger()
    {
        if (Animation != null)
            Animation.AnimationTrigger();
        else
            StateMachine.CurrentState.AnimationTrigger();
    }
    
    public void AnimationFinishTrigger()
    {
        if (Animation != null)
            Animation.AnimationFinishTrigger();
        else
            StateMachine.CurrentState.AnimationFinishTrigger();
    }

    public void Flip()
    {
        if (Orientation != null)
        {
            Orientation.Flip();
        }
        else
        {
            FacingRight *= -1;
            transform.Rotate(0.0f, 180f, 0.0f);
        }
    }

    private void OnDrawGizmos()
    {
        if (WallCheck == null || LedgeCheck == null || GroundCheck == null || PlayerData == null) 
            return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(WallCheck.position, WallCheck.position + Vector3.right * FacingRight * PlayerData.WallCheckDistance);
        Gizmos.DrawWireSphere(WallCheck.position, 0.05f);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(LedgeCheck.position, LedgeCheck.position + Vector3.right * FacingRight * PlayerData.LedgeCheckDistance);
        Gizmos.DrawWireSphere(LedgeCheck.position, 0.05f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GroundCheck.position, PlayerData.GroundCheckRadius);
    }
    #endregion
}
