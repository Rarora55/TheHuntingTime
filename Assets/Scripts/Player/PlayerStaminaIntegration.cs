using UnityEngine;

public class PlayerStaminaIntegration : MonoBehaviour
{
    private Player player;
    private StaminaController staminaController;
    private StaminaData staminaData;
    
    private bool isRunning;
    private bool isClimbing;
    private bool isGrappingWall;

    // Tracks the previous CanUseStamina value to detect the exact frame it becomes false.
    private bool prevCanUseStamina = true;
    
    void Awake()
    {
        player = GetComponent<Player>();
        staminaController = GetComponent<StaminaController>();
        
        SubscribeToEvents();
    }
    
    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    void SubscribeToEvents()
    {
        if (staminaController != null)
        {
            staminaController.OnStaminaDepleted += HandleStaminaDepleted;
            staminaController.OnStaminaRecovered += HandleStaminaRecovered;
            staminaController.OnCooldownStarted += HandleCooldownStarted;
            staminaController.OnCooldownEnded += HandleCooldownEnded;
        }
    }
    
    void UnsubscribeFromEvents()
    {
        if (staminaController != null)
        {
            staminaController.OnStaminaDepleted -= HandleStaminaDepleted;
            staminaController.OnStaminaRecovered -= HandleStaminaRecovered;
            staminaController.OnCooldownStarted -= HandleCooldownStarted;
            staminaController.OnCooldownEnded -= HandleCooldownEnded;
        }
    }
    
    void HandleStaminaDepleted()
    {
        // Safety net for when stamina reaches exactly 0.
        // The primary detection is in Update() via CanUseStamina tracking.
        TriggerTiredState();
    }
    
    void HandleStaminaRecovered()
    {
        player.anim.SetBool("isExhausted", false);
    }

    /// <summary>Transitions to TiredState if not already in it.</summary>
    private void TriggerTiredState()
    {
        if (player.TiredState == null) return;
        if (player.StateMachine.CurrentState == player.TiredState) return;

        player.anim.SetBool("isExhausted", true);
        player.StateMachine.ChangeState(player.TiredState);
        Debug.Log("<color=magenta>[STAMINA] Entering TiredState — stamina exhausted.</color>");
    }
    
    void HandleCooldownStarted() { }
    void HandleCooldownEnded() { }
    
    public bool CanRun()
    {
        return staminaController != null && staminaController.CanUseStamina;
    }
    
    public bool CanJump()
    {
        return staminaController != null && staminaController.CanUseStamina;
    }
    
    public bool CanClimb()
    {
        return staminaController != null && staminaController.CanUseStamina;
    }
    
    public bool CanGrapWall()
    {
        return staminaController != null && staminaController.CanUseStamina;
    }
    
    public bool TryConsumeJumpStamina(StaminaData data)
    {
        if (staminaController == null || data == null)
            return true;
        
        return staminaController.ConsumeStamina(data.jumpCost);
    }
    
    public void StartRunning(StaminaData data)
    {
        if (isRunning)
            return;
        
        isRunning = true;
        staminaData = data;
    }
    
    public void StopRunning()
    {
        isRunning = false;
    }
    
    public void StartClimbing(StaminaData data)
    {
        if (isClimbing)
            return;
        
        isClimbing = true;
        staminaData = data;
    }
    
    public void StopClimbing()
    {
        isClimbing = false;
    }
    
    public void StartGrappingWall(StaminaData data)
    {
        if (isGrappingWall)
            return;
        
        isGrappingWall = true;
        staminaData = data;
    }
    
    public void StopGrappingWall()
    {
        isGrappingWall = false;
    }
    
    void Update()
    {
        if (staminaController == null) return;

        // ── Exhaustion detection ─────────────────────────────────────────────
        // Monitor the exact frame CanUseStamina flips from true → false.
        // This catches all cases: run cost stuck above residual stamina,
        // jump cost depleting below threshold, etc.
        bool canUseStamina = staminaController.CanUseStamina;
        if (prevCanUseStamina && !canUseStamina)
        {
            TriggerTiredState();
        }
        prevCanUseStamina = canUseStamina;

        // ── Stamina consumption ──────────────────────────────────────────────
        if (staminaData == null) return;

        if (isRunning && canUseStamina)
        {
            float runCost = staminaData.runningCostPerSecond * Time.deltaTime;
            if (!staminaController.ConsumeStamina(runCost))
                StopRunning();
        }
        
        if (isClimbing && canUseStamina)
        {
            float climbCost = staminaData.climbingCostPerSecond * Time.deltaTime;
            if (!staminaController.ConsumeStamina(climbCost))
                StopClimbing();
        }

        if (isGrappingWall && canUseStamina)
        {
            float grapCost = staminaData.wallGrapCostPerSecond * Time.deltaTime;
            if (!staminaController.ConsumeStamina(grapCost))
                StopGrappingWall();
        }
    }
}