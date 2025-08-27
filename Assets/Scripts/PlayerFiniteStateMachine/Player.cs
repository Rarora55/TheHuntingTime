using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Player: MonoBehaviour
{
    #region States
    public PlayerStateMachine StateMachine {  get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    #endregion

    public Animator Anim {  get; private set; }

    [SerializeField] private PlayerData playerData;


    private void Awake()
    {
        StateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
        moveState = new PlayerMoveState(this, StateMachine, playerData, "move");
    }

    private void Start()
    {
        
        Anim = GetComponent<Animator>();

        StateMachine.Initialize(idleState);
    }

    private void Update()
    {
        StateMachine.currentState.LogicUpdate();

    }

    private void FixedUpdate()
    {
        StateMachine.currentState.PhysicsUpdate();
    }
}
