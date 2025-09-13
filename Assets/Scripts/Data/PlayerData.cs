using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData: ScriptableObject
{

    [Header("Move State")]
    public float movementVelocity = 10f;

    [Header("Jump State")]
    public float JumpVelocity = 15f;

    [Header("Check Ground")]
    public float GroundCheckRadius = 0.3f;
    public LayerMask WhatIsGround;


     
    
}
