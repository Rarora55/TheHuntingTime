using UnityEngine;

public class PlayerStaminaIntegration : MonoBehaviour
{
    private Player player;
    private StaminaController staminaController;
    private StaminaData staminaData;
    
    private bool isRunning;
    private bool isClimbing;
    private bool isGrappingWall;
    
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
        Debug.Log("<color=red>[PLAYER STAMINA] Player exhausted! No more stamina actions allowed.</color>");
        
        player.anim.SetBool("isExhausted", true);
    }
    
    void HandleStaminaRecovered()
    {
        Debug.Log("<color=green>[PLAYER STAMINA] Player stamina fully recovered!</color>");
        
        player.anim.SetBool("isExhausted", false);
    }
    
    void HandleCooldownStarted()
    {
        Debug.Log("<color=yellow>[PLAYER STAMINA] Cooldown started. Player cannot use stamina abilities.</color>");
    }
    
    void HandleCooldownEnded()
    {
        Debug.Log("<color=cyan>[PLAYER STAMINA] Cooldown ended. Stamina abilities available again.</color>");
    }
    
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
        
        if (staminaController.ConsumeStamina(data.jumpCost))
        {
            Debug.Log($"<color=cyan>[STAMINA] Jump cost: {data.jumpCost}. Remaining: {staminaController.CurrentStamina:F1}</color>");
            return true;
        }
        
        Debug.Log("<color=red>[STAMINA] Not enough stamina to jump!</color>");
        return false;
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
        if (staminaController == null || staminaData == null)
            return;
        
        if (isRunning && staminaController.CanUseStamina)
        {
            float runCost = staminaData.runningCostPerSecond * Time.deltaTime;
            if (!staminaController.ConsumeStamina(runCost))
            {
                StopRunning();
                Debug.Log("<color=orange>[STAMINA] Not enough stamina to continue running!</color>");
            }
        }
        
        if (isClimbing && staminaController.CanUseStamina)
        {
            float climbCost = staminaData.climbingCostPerSecond * Time.deltaTime;
            if (!staminaController.ConsumeStamina(climbCost))
            {
                StopClimbing();
                Debug.Log("<color=orange>[STAMINA] Not enough stamina to continue climbing!</color>");
            }
        }
        
        if (isGrappingWall && staminaController.CanUseStamina)
        {
            float grapCost = staminaData.wallGrapCostPerSecond * Time.deltaTime;
            if (!staminaController.ConsumeStamina(grapCost))
            {
                StopGrappingWall();
                Debug.Log("<color=orange>[STAMINA] Not enough stamina to continue grapping wall!</color>");
            }
        }
    }
}
