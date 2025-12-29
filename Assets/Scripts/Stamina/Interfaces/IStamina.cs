using System;

public interface IStamina
{
    float CurrentStamina { get; }
    float MaxStamina { get; }
    float StaminaPercentage { get; }
    bool HasStamina { get; }
    bool IsExhausted { get; }
    bool IsRecovering { get; }
    bool CanUseStamina { get; }
    
    event Action<float, float> OnStaminaChanged;
    event Action OnStaminaDepleted;
    event Action OnStaminaRecovered;
    event Action OnCooldownStarted;
    event Action OnCooldownEnded;
}
