using UnityEngine;

public interface IHealable
{
    void Heal(float amount);
    void HealToFull();
    bool CanHeal { get; }
}
