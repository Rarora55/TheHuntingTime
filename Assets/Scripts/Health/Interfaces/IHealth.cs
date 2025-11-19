using System;
using UnityEngine;

public interface IHealth
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    float HealthPercentage { get; }
    bool IsAlive { get; }
    bool IsDead { get; }
    bool IsInvulnerable { get; }
    
    event Action<float, float> OnHealthChanged;
    event Action<DamageData> OnDamaged;
    event Action<float> OnHealed;
    event Action OnDeath;
}
