using System;
using System.Collections;
using UnityEngine;

public class StaminaController : MonoBehaviour, IStamina, IStaminaConsumer
{
    [SerializeField] private StaminaData staminaData;
    
    private float currentStamina;
    private bool isExhausted;
    private bool isRecovering;
    private bool isInCooldown;
    private float lastConsumptionTime;
    private Coroutine cooldownCoroutine;
    private Coroutine recoveryCoroutine;
    
    public float CurrentStamina => currentStamina;
    public float MaxStamina => staminaData.maxStamina;
    public float StaminaPercentage => currentStamina / MaxStamina;
    public bool HasStamina => currentStamina > staminaData.minimumStaminaThreshold;
    public bool IsExhausted => isExhausted;
    public bool IsRecovering => isRecovering;
    public bool CanUseStamina => !isInCooldown && HasStamina;
    
    public event Action<float, float> OnStaminaChanged;
    public event Action OnStaminaDepleted;
    public event Action OnStaminaRecovered;
    public event Action OnCooldownStarted;
    public event Action OnCooldownEnded;
    
    void Awake()
    {
        InitializeStamina();
    }
    
    void InitializeStamina()
    {
        currentStamina = staminaData.startingStamina;
        isExhausted = false;
        isRecovering = false;
        isInCooldown = false;
        lastConsumptionTime = -staminaData.recoveryDelay;
    }
    
    public bool ConsumeStamina(float amount)
    {
        if (!CanConsumeStamina(amount))
            return false;
        
        float previousStamina = currentStamina;
        currentStamina = Mathf.Max(0, currentStamina - amount);
        lastConsumptionTime = Time.time;
        
        OnStaminaChanged?.Invoke(currentStamina, previousStamina);
        
        if (currentStamina <= 0 && !isExhausted)
        {
            HandleStaminaDepleted();
        }
        
        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
            isRecovering = false;
        }
        
        return true;
    }
    
    public bool CanConsumeStamina(float amount)
    {
        return !isInCooldown && currentStamina >= amount;
    }
    
    void HandleStaminaDepleted()
    {
        isExhausted = true;
        Debug.Log("<color=red>[STAMINA] Stamina depleted! Entering cooldown...</color>");
        
        OnStaminaDepleted?.Invoke();
        
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
        cooldownCoroutine = StartCoroutine(CooldownRoutine());
    }
    
    IEnumerator CooldownRoutine()
    {
        isInCooldown = true;
        OnCooldownStarted?.Invoke();
        
        Debug.Log($"<color=yellow>[STAMINA] Cooldown started: {staminaData.cooldownDuration}s</color>");
        
        yield return new WaitForSeconds(staminaData.cooldownDuration);
        
        isInCooldown = false;
        isExhausted = false;
        OnCooldownEnded?.Invoke();
        
        Debug.Log("<color=green>[STAMINA] Cooldown ended! Starting recovery...</color>");
        
        StartRecovery();
    }
    
    void Update()
    {
        if (!isInCooldown && !isRecovering && currentStamina < MaxStamina)
        {
            if (Time.time - lastConsumptionTime >= staminaData.recoveryDelay)
            {
                StartRecovery();
            }
        }
    }
    
    void StartRecovery()
    {
        if (recoveryCoroutine != null)
            return;
        
        recoveryCoroutine = StartCoroutine(RecoveryRoutine());
    }
    
    IEnumerator RecoveryRoutine()
    {
        isRecovering = true;
        
        while (currentStamina < MaxStamina && !isInCooldown)
        {
            yield return new WaitForSeconds(0.1f);
            
            float previousStamina = currentStamina;
            currentStamina = Mathf.Min(MaxStamina, currentStamina + staminaData.recoveryRate * 0.1f);
            
            OnStaminaChanged?.Invoke(currentStamina, previousStamina);
            
            if (currentStamina >= MaxStamina)
            {
                Debug.Log("<color=cyan>[STAMINA] Fully recovered!</color>");
                OnStaminaRecovered?.Invoke();
            }
        }
        
        isRecovering = false;
        recoveryCoroutine = null;
    }
    
    public void ResetStamina()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }
        
        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
        }
        
        currentStamina = staminaData.startingStamina;
        isExhausted = false;
        isRecovering = false;
        isInCooldown = false;
        
        OnStaminaChanged?.Invoke(currentStamina, currentStamina);
    }
    
    public void ForceRecovery()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }
        
        isInCooldown = false;
        isExhausted = false;
        
        float previousStamina = currentStamina;
        currentStamina = MaxStamina;
        
        OnStaminaChanged?.Invoke(currentStamina, previousStamina);
        OnStaminaRecovered?.Invoke();
        
        Debug.Log("<color=green>[STAMINA] Force recovery applied!</color>");
    }
}
