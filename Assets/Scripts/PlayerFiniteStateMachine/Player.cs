using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Player: MonoBehaviour
{
    #region States
    public PlayerStateMachine StateMachine {  get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    #endregion

    #region Components
    public Animator anim {  get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }
    #endregion

    #region Movements Vectors
    public Vector2 workSpace {  get; private set; }
    public Vector2 CurrentVelocity { get; private set; }
    #endregion

    [SerializeField] private PlayerData PlayerData;

    private void Awake()
    {
        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, StateMachine, PlayerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, PlayerData, "move");
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        RB = GetComponent<Rigidbody2D>();
        InputHandler = GetComponent<PlayerInputHandler>();
        StateMachine.Initialize(IdleState);

    }

    private void Update()
    {
        
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        CurrentVelocity = RB.linearVelocity;
        StateMachine.CurrentState.PhysicsUpdate();
    }

    public void SetVelocityX (float velocity)
    {

        workSpace.Set(velocity, CurrentVelocity.y);
        RB.linearVelocity = workSpace;
        CurrentVelocity = workSpace;
        Debug.Log($"Moving - Setting velocity to: {workSpace}, RB velocity is now: {RB.linearVelocity}");
    }
}
