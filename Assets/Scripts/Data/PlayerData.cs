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

    [Header("Knockback System")]
    public float shootKnockbackForce = 3f;
    public float shootKnockbackDelay = 0.1f;
    public float wallCollisionKnockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    public float minRunSpeedForWallKnockback = 10f;

    [Header("Push & Pull System")]
    [Tooltip("Multiplicador de velocidad base al empujar/tirar (0.75 = 75% de velocidad)")]
    public float basePushPullSpeedMultiplier = 0.75f;
    
    [Tooltip("Peso mínimo sin penalización extra")]
    public float minimumWeight = 1.0f;
    
    [Tooltip("Penalización de velocidad por cada unidad de peso extra")]
    public float weightPenaltyPerUnit = 0.3f;
    
    [Tooltip("Radio de detección de objetos empujables")]
    public float pushPullDetectionRadius = 1.5f;
    
    [Tooltip("Fuerza aplicada al empujar/tirar objetos")]
    public float pushPullForce = 8f;
    
    [Tooltip("Layer de objetos empujables")]
    public LayerMask pushableObjectLayer;
    
}
