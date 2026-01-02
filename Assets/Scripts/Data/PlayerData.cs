using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData: ScriptableObject
{

    [Header("Move State")]
    public float movementVelocity = 10f;
    public float runVelocity = 12f;

    [Header("Jump State")]
    public float JumpVelocity = 15f;
    public float jumpGravityScale = 3.5f;
    public float fallGravityScale = 8f;
    public float maxFallSpeed = 25f;

    [Header("Check Ground")]
    public float GroundCheckRadius = 0.3f;
    public LayerMask WhatIsGround;


    [Header("Wall Checks")]
    public float WallCheckDistance = 0.5f;
    public float LedgeCheckDistance = 0.4f;
    public float WallSlicedVelocity = 3f;

    [Header("Wall Climb State")]
    public float WallClimbVelocity = 3f;

    [Header("Ledge State")]
    public Vector2 startOffSet;
    public Vector2 stopOffSet;

    [Header("Couch")]
    public float crouchMovementVelocity = 5f;
    public float crouchColliderHeight = 0.8f;
    public float standColliderHeight = 1.6f;

    [Header("Ladder Climb")]
    public float ladderClimbSpeed = 4f;
    public float ladderSlideSpeed = 2f;
    
}
