using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amount);
    void TakeDamage(DamageData damageData);
    bool IsAlive { get; }
}
