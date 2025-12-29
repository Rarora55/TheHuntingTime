using UnityEngine;

[CreateAssetMenu(fileName = "newStaminaData", menuName = "Data/Stamina Data")]
public class StaminaData : ScriptableObject
{
    [Header("Stamina Settings")]
    [Tooltip("Maximum stamina capacity")]
    public float maxStamina = 100f;
    
    [Tooltip("Starting stamina when initialized")]
    public float startingStamina = 100f;
    
    [Header("Recovery Settings")]
    [Tooltip("Cooldown duration in seconds before stamina starts recovering")]
    public float cooldownDuration = 30f;
    
    [Tooltip("Recovery rate per second after cooldown")]
    public float recoveryRate = 10f;
    
    [Tooltip("Delay before recovery starts after last consumption (in seconds)")]
    public float recoveryDelay = 1f;
    
    [Header("Ability Costs")]
    [Tooltip("Stamina cost per second while running")]
    public float runningCostPerSecond = 10f;
    
    [Tooltip("Stamina cost per jump")]
    public float jumpCost = 15f;
    
    [Tooltip("Stamina cost per second while climbing")]
    public float climbingCostPerSecond = 8f;
    
    [Tooltip("Stamina cost per second while holding on wall (WallGrap)")]
    public float wallGrapCostPerSecond = 5f;
    
    [Header("Thresholds")]
    [Tooltip("Minimum stamina required to perform actions")]
    public float minimumStaminaThreshold = 5f;
}
