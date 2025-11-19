using UnityEngine;

public enum DamageType
{
    Physical,
    Fall,
    Fire,
    Poison,
    Environmental
}

[System.Serializable]
public struct DamageData
{
    public float amount;
    public DamageType damageType;
    public Vector2 damageDirection;
    public GameObject source;
    
    public DamageData(float amount, DamageType type = DamageType.Physical, GameObject source = null)
    {
        this.amount = amount;
        this.damageType = type;
        this.damageDirection = Vector2.zero;
        this.source = source;
    }
    
    public DamageData(float amount, DamageType type, Vector2 direction, GameObject source = null)
    {
        this.amount = amount;
        this.damageType = type;
        this.damageDirection = direction;
        this.source = source;
    }
}
