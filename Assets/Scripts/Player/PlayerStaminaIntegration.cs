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

    // Grace period state: when stamina depletes, we wait this many seconds before
    // actually entering TiredState, allowing the current action to finish naturally.
    private bool isGracePeriodActive = false;
    private float gracePeriodEndTime = 0f;
    
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
        StartGracePeriod();
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

        isGracePeriodActive = false;
        player.anim.SetBool("isExhausted", true);
        player.StateMachine.ChangeState(player.TiredState);
        Debug.Log("<color=magenta>[STAMINA] Entering TiredState — stamina exhausted.</color>");
    }

    /// <summary>
    /// Starts the grace period countdown. TiredState will only trigger once
    /// the grace period expires, giving the current action time to complete.
    /// </summary>
    private void StartGracePeriod()
    {
        if (isGracePeriodActive) return;
        if (player.StateMachine.CurrentState == player.TiredState) return;

        PlayerData playerData = player.GetPlayerData();
        float gracePeriod = playerData != null ? playerData.tiredGracePeriod : 3f;

        isGracePeriodActive = true;
        gracePeriodEndTime = Time.time + gracePeriod;
        Debug.Log($"<color=yellow>[STAMINA] Grace period started — TiredState in {gracePeriod}s.</color>");
    }
    
    void HandleCooldownStarted() { }
    void HandleCooldownEnded() { }
    
    public bool CanRun()
    {
        return staminaController != null && staminaController.CanUseStamina;
    }

    /// <summary>
    /// Returns a speed multiplier based on the current stamina percentage.
    /// Thresholds and multipliers are configured in StaminaData.
    /// </summary>
    public float GetRunSpeedMultiplier()
    {
        if (staminaController == null || staminaData == null) return 1f;

        float percentage = staminaController.StaminaPercentage * 100f;

        if (percentage > staminaData.fatigueThreshold1) return staminaData.speedMultiplierFresh;
        if (percentage > staminaData.fatigueThreshold2) return staminaData.speedMultiplierTired;
        if (percentage > staminaData.fatigueThreshold3) return staminaData.speedMultiplierVeryTired;
        return staminaData.speedMultiplierExhausted;
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
        // Instead of triggering TiredState immediately, start a grace period
        // so the current action (e.g. a jump) can finish naturally.
        bool canUseStamina = staminaController.CanUseStamina;
        if (prevCanUseStamina && !canUseStamina)
        {
            StartGracePeriod();
        }
        prevCanUseStamina = canUseStamina;

        // ── Grace period countdown ───────────────────────────────────────────
        if (isGracePeriodActive && Time.time >= gracePeriodEndTime)
        {
            TriggerTiredState();
        }

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