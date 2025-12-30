using UnityEngine;

public class DebugSettings : MonoBehaviour
{
    [Header("Debug Categories")]
    [SerializeField] private bool enablePlayerStates = false;
    [SerializeField] private bool enableWallInteraction = false;
    [SerializeField] private bool enableClimbing = false;
    [SerializeField] private bool enableJumping = false;
    [SerializeField] private bool enableMovement = false;
    [SerializeField] private bool enableStamina = false;
    [SerializeField] private bool enableHealth = false;
    [SerializeField] private bool enableCombat = false;
    [SerializeField] private bool enableInteraction = false;
    [SerializeField] private bool enableEnvironment = false;
    
    [Header("Quick Presets")]
    [SerializeField] private bool enableAll = false;
    [SerializeField] private bool disableAll = true;
    
    void Awake()
    {
        ApplySettings();
    }
    
    void OnValidate()
    {
        if (Application.isPlaying)
        {
            ApplySettings();
        }
    }
    
    private void ApplySettings()
    {
        DebugManager.DebugCategory categories = DebugManager.DebugCategory.None;
        
        if (enableAll)
        {
            categories = DebugManager.DebugCategory.All;
            disableAll = false;
        }
        else if (disableAll)
        {
            categories = DebugManager.DebugCategory.None;
        }
        else
        {
            if (enablePlayerStates) categories |= DebugManager.DebugCategory.PlayerStates;
            if (enableWallInteraction) categories |= DebugManager.DebugCategory.WallInteraction;
            if (enableClimbing) categories |= DebugManager.DebugCategory.Climbing;
            if (enableJumping) categories |= DebugManager.DebugCategory.Jumping;
            if (enableMovement) categories |= DebugManager.DebugCategory.Movement;
            if (enableStamina) categories |= DebugManager.DebugCategory.Stamina;
            if (enableHealth) categories |= DebugManager.DebugCategory.Health;
            if (enableCombat) categories |= DebugManager.DebugCategory.Combat;
            if (enableInteraction) categories |= DebugManager.DebugCategory.Interaction;
            if (enableEnvironment) categories |= DebugManager.DebugCategory.Environment;
        }
        
        DebugManager.SetCategories(categories);
    }
}
