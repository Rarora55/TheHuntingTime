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
    public PlayerLandState LandState { get; private set; }
    public PlayerWallClimbState WallClimbState { get; private set; }
    public PlayerWallGrapState WallGrapState { get; private set; }
    public PlayerWallSlicedState WallSlicedState { get; private set; }
    public PlayerLedgeClimbState WallLedgeState { get; private set; }
    public PlayerCrouchIdleState CrouchIdleState { get; private set; }
    public PlayerCrouchMoveState CrouchMoveState { get; private set; }



    [SerializeField] private PlayerData PlayerData;
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
    public int FacingRight { get; private set; }
    #endregion

    //Methods

    #region Unity CallBacks
    private void Awake()
    {
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
        StateMachine.Initialize(IdleState);

    }

    private void Update()
    {
        CurrentVelocity = RB.linearVelocity;
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {

        StateMachine.CurrentState.PhysicsUpdate();
    }

    #endregion

    #region Set Functions

    public void SetVelocityZero()
    {
        RB.linearVelocity = Vector2.zero;
        CurrentVelocity = Vector2.zero;
    }

    public void SetVelocityX(float velocity)
    {

        workSpace.Set(velocity, CurrentVelocity.y);
        RB.linearVelocity = workSpace;
        CurrentVelocity = workSpace;
        
    }

    public void SetVelocityY(float velocity)
    {
        workSpace.Set(CurrentVelocity.x, velocity);
        RB.linearVelocity = workSpace;
        CurrentVelocity = workSpace;
    }
    #endregion

    #region Checks

    public bool CheckIsGrounded()
    {
        return Physics2D.OverlapCircle(GroundCheck.position, PlayerData.GroundCheckRadius, PlayerData.WhatIsGround);
    }

    public bool CheckIfTouchingWall()
    {
        return Physics2D.Raycast(WallCheck.position, Vector2.right * FacingRight, PlayerData.WallCheckDistance, PlayerData.WhatIsGround);
    }

    public bool CheckTouchingLedge()
    {
        return Physics2D.Raycast(LedgeCheck.position , Vector2.right * FacingRight, PlayerData.WallCheckDistance, PlayerData.WhatIsGround);
    }
    public bool CheckForCeiling()
    {
        return Physics2D.OverlapCircle(ceilingCheck.position, PlayerData.GroundCheckRadius, PlayerData.WhatIsGround);
    }
    public void CheckFlip(int xInput)
    {
        if(xInput != 0 && xInput != FacingRight)
        {
            Flip();
        }
    }
    #endregion

    #region Others
    public void SetColliderHeight(float height)
    {
        Vector2 center = moveCollider.offset;
        workSpace.Set(moveCollider.size.x, height);
        center.y += height - (moveCollider.size.y) / 2;
        moveCollider.size = workSpace;
    }
    public Vector2 DeterminetCornerPos()
    {
        RaycastHit2D xHit = Physics2D.Raycast(WallCheck.position, Vector2.right * FacingRight, PlayerData.WallCheckDistance, PlayerData.WhatIsGround);
        float xDist  = xHit.distance;
        workSpace.Set((xDist + 0.015f) * FacingRight, 0f);
        RaycastHit2D yHit = Physics2D.Raycast(LedgeCheck.position + (Vector3)(workSpace), Vector2.down, LedgeCheck.position.y - WallCheck.position.y + 0.015f, PlayerData.WhatIsGround);
        float yDist = yHit.distance;
        workSpace.Set(WallCheck.position.x + (xDist * FacingRight), LedgeCheck.position.y - yDist);
        return workSpace;
    }

    public void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
    public void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();



    public void Flip()
    {
        FacingRight *= -1;
        transform.Rotate(0.0f, 180f, 0.0f);
    }
    #endregion
}
