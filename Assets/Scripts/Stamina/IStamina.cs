using System;

public interface IStamina
{
    float CurrentStamina { get; }
    float MaxStamina { get; }
    float StaminaPercentage { get; }
    bool IsExhausted { get; }
    bool CanConsumeStamina(float amount);
    
    void ConsumeStamina(float amount);
    void RegenerateStamina(float amount);
    
    event Action<float, float> OnStaminaChanged;
    event Action OnExhausted;
    event Action OnStaminaRecovered;
}
