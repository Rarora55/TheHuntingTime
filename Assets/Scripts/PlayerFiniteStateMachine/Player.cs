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

    [SerializeField] private PlayerData PlayerData;
    #endregion

    #region Components
    public Animator anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }
    #endregion

    #region Movements Vectors
    private Vector2 workSpace;
    public Vector2 CurrentVelocity { get; private set; }
    #endregion

    #region Check Transforms
    [SerializeField] private Transform GroundCheck;
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
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();
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
    public void CheckFlip(int xInput)
    {
        if(xInput != 0 && xInput != FacingRight)
        {
            Flip();
        }
    }
    #endregion

    #region Others

    public void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
    public void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
    public void Flip()
    {
        FacingRight *= -1;
        transform.Rotate(0.0f, 180f, 0.0f);
    }
    #endregion
}
