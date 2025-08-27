using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Player: MonoBehaviour
{
    public PlayerStateMachine StateMachine {  get; private set; }

    private void Awake()
    {
        StateMachine = GetComponent<PlayerStateMachine>();
    }

    private void Start()
    {
        //TODO; Initialize statemachine
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
