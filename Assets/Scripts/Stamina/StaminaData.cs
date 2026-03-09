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

    [Header("Run Speed by Fatigue")]
    [Tooltip("Stamina % below which the first speed penalty applies (default 50%)")]
    [Range(0f, 100f)] public float fatigueThreshold1 = 50f;
    [Tooltip("Stamina % below which the second speed penalty applies (default 30%)")]
    [Range(0f, 100f)] public float fatigueThreshold2 = 30f;
    [Tooltip("Stamina % below which the third speed penalty applies (default 10%)")]
    [Range(0f, 100f)] public float fatigueThreshold3 = 10f;

    [Tooltip("Speed multiplier when stamina is between 100%-fatigueThreshold1 (default 1.0)")]
    [Range(0f, 1f)] public float speedMultiplierFresh    = 1.000f;
    [Tooltip("Speed multiplier when stamina is between fatigueThreshold1-fatigueThreshold2 (default 0.60)")]
    [Range(0f, 1f)] public float speedMultiplierTired    = 0.600f;
    [Tooltip("Speed multiplier when stamina is between fatigueThreshold2-fatigueThreshold3 (default 0.48)")]
    [Range(0f, 1f)] public float speedMultiplierVeryTired = 0.480f;
    [Tooltip("Speed multiplier when stamina is below fatigueThreshold3 (default 0.432)")]
    [Range(0f, 1f)] public float speedMultiplierExhausted = 0.432f;
}
