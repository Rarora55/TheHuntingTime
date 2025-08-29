using UnityEngine;


[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData: ScriptableObject
{
    [Header("Move State")]
    public float movementVelocity = 10f;
    [Header("Jump State")]
    public float jumpVelocity = 15f;

    [Header("Check variables")]
    public float checkIfGrounded = 0.03f;
    public LayerMask whatIsGround;
}
