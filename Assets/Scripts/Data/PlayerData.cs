using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData: ScriptableObject
{

    [Header("Move State")]
    public float movementVelocity = 10;
    
}
