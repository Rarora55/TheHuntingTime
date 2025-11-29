# 🏥 GUÍA DEL SISTEMA DE SALUD

## 📋 Resumen

Sistema modular de salud basado en **interfaces** y **componentes reusables** que puede usarse en Player, enemigos, NPCs, y cualquier entidad que necesite health tracking.

---

## 🎯 Características Implementadas

✅ **Modular y Reusable** - Funciona en cualquier GameObject  
✅ **Interface-driven** - IDamageable, IHealable, IHealth  
✅ **Event-based** - Comunicación desacoplada vía eventos  
✅ **Fall Damage** - Sistema automático de daño por caída  
✅ **Healing Items** - Instantáneo y Over Time  
✅ **Invulnerability Frames** - i-frames configurables  
✅ **Health Regeneration** - Opcional, configurable  
✅ **ScriptableObject Config** - Fácil balanceo sin código  

---

## 🏗️ Arquitectura

```
HealthController (Component)
├── Implements: IHealth, IDamageable, IHealable
├── Uses: HealthData (ScriptableObject)
└── Fires Events: OnHealthChanged, OnDamaged, OnHealed, OnDeath

FallDamageCalculator (Component)
├── Tracks falling state
├── Calculates fall distance
└── Triggers TakeFallDamage() on landing

HealingItem (Component)
├── Collision detection
├── Uses: HealingItemData (ScriptableObject)
└── Applies instant or over-time healing
```

---

## 🚀 Setup Rápido

### **1. Setup en Player**

```
GameObject: Player
├── Player.cs (existing)
├── HealthController.cs          ← ADD
├── FallDamageCalculator.cs      ← ADD
└── PlayerHealthIntegration.cs   ← ADD
```

**En Inspector:**
1. Asignar `HealthData` ScriptableObject al `HealthController`
2. El resto se auto-configura

---

### **2. Crear HealthData**

**Right-click en Project:**
```
Create > Health System > Health Data
```

**Configuración recomendada para Player:**
```
Max Health: 100
Starting Health: 100
Can Regenerate: false (o true si querés regen)
Regeneration Rate: 5 HP/s
Regeneration Delay: 3s
Invulnerability Duration: 1s
Can Take Fall Damage: true
Fall Damage Threshold: 5m
Fall Damage Multiplier: 10
Max Fall Damage: 50
```

---

### **3. Crear Healing Item**

**A) Crear HealingItemData:**
```
Create > Health System > Healing Item
```

**Ejemplos:**

**Medkit (Instant):**
```
Item Name: Medkit
Healing Type: Instant
Heal Amount: 50
```

**Bandage (Over Time):**
```
Item Name: Bandage
Healing Type: OverTime
Heal Amount: 30
Duration: 5s
Tick Rate: 1s
```

**B) Crear GameObject en escena:**
```
GameObject: Medkit
├── SpriteRenderer (visual del item)
├── BoxCollider2D (trigger = true)
└── HealingItem.cs
    └── Assign HealingItemData
```

---

## 💻 Uso desde Código

### **Aplicar Daño**

```csharp
// Opción 1: Daño simple
IDamageable target = enemy.GetComponent<IDamageable>();
target.TakeDamage(25f);

// Opción 2: Daño con metadata
DamageData damageData = new DamageData(
    amount: 30f,
    damageType: DamageType.Fire,
    direction: (target.position - attacker.position).normalized,
    source: attacker.gameObject
);
target.TakeDamage(damageData);
```

---

### **Curar**

```csharp
IHealable target = player.GetComponent<IHealable>();

if (target.CanHeal)
{
    target.Heal(25f);
}

// Curar al máximo
target.HealToFull();
```

---

### **Leer Estado de Salud**

```csharp
IHealth health = player.GetComponent<IHealth>();

float currentHP = health.CurrentHealth;
float maxHP = health.MaxHealth;
float percentage = health.HealthPercentage; // 0.0 - 1.0

if (health.IsAlive)
{
    // Player vivo
}

if (health.IsDead)
{
    // Player muerto
}
```

---

### **Subscribirse a Eventos**

```csharp
void Awake()
{
    IHealth health = GetComponent<IHealth>();
    
    health.OnHealthChanged += HandleHealthChanged;
    health.OnDamaged += HandleDamaged;
    health.OnHealed += HandleHealed;
    health.OnDeath += HandleDeath;
}

void HandleHealthChanged(float newHealth, float previousHealth)
{
    Debug.Log($"Health changed: {previousHealth} → {newHealth}");
    UpdateHealthBar(newHealth / health.MaxHealth);
}

void HandleDamaged(DamageData damageData)
{
    Debug.Log($"Took {damageData.amount} {damageData.damageType} damage");
    PlayDamageVFX();
    ScreenShake();
}

void HandleHealed(float amount)
{
    Debug.Log($"Healed {amount} HP");
    PlayHealVFX();
}

void HandleDeath()
{
    Debug.Log("Entity died!");
    PlayDeathAnimation();
    Respawn();
}
```

---

## 🎮 Integración con Player States

El sistema ya está integrado con `PlayerLandState` para detectar fall damage:

```csharp
// PlayerLandState.cs - Enter()
PlayerHealthIntegration healthIntegration = player.GetComponent<PlayerHealthIntegration>();
if (healthIntegration != null)
{
    healthIntegration.OnPlayerLanded(); // ← Calcula y aplica fall damage
}
```

**Flujo completo:**
```
1. Player salta/cae desde altura
2. FallDamageCalculator detecta velocidad negativa
3. Registra altura inicial
4. Player aterriza → PlayerLandState.Enter()
5. Llama OnPlayerLanded()
6. Calcula distancia caída
7. Si > threshold → aplica daño
8. HealthController dispara eventos
```

---

## 🔧 Configuración Avanzada

### **Daño por Tipo**

Podés extender el sistema para resistencias:

```csharp
// En HealthController.cs, modificar TakeDamage():
public void TakeDamage(DamageData damageData)
{
    float finalDamage = CalculateDamageWithResistances(damageData);
    
    // ... resto del código
}

float CalculateDamageWithResistances(DamageData damageData)
{
    float damage = damageData.amount;
    
    switch (damageData.damageType)
    {
        case DamageType.Fire:
            damage *= fireResistance; // 0.5 = 50% resistencia
            break;
        case DamageType.Poison:
            damage *= poisonResistance;
            break;
    }
    
    return damage;
}
```

---

### **Regeneración Condicional**

```csharp
// Regenerar solo fuera de combate
IEnumerator RegenerationRoutine()
{
    while (true)
    {
        yield return new WaitForSeconds(0.1f);
        
        bool isOutOfCombat = Time.time - lastDamageTime >= regenerationDelay;
        
        if (IsAlive && 
            currentHealth < MaxHealth && 
            isOutOfCombat &&
            !IsInCombat()) // ← Custom check
        {
            Heal(healthData.regenerationRate * 0.1f);
        }
    }
}
```

---

### **Max Health Upgrades**

```csharp
// En HealthController, agregar:
public void IncreaseMaxHealth(float amount)
{
    float previousMax = MaxHealth;
    healthData.maxHealth += amount;
    
    // Escalar current health proporcionalmente
    float ratio = currentHealth / previousMax;
    currentHealth = MaxHealth * ratio;
    
    Debug.Log($"Max health increased: {previousMax} → {MaxHealth}");
}
```

---

## 🧪 Testing Checklist

### Setup Inicial
- [ ] HealthController agregado al Player
- [ ] FallDamageCalculator agregado al Player
- [ ] PlayerHealthIntegration agregado al Player
- [ ] HealthData ScriptableObject creado y asignado
- [ ] Fall damage threshold configurado (ej: 5m)

### Tests de Daño
- [ ] TakeDamage(25) reduce salud correctamente
- [ ] Invulnerability frames funcionan
- [ ] OnDamaged event se dispara
- [ ] OnHealthChanged event se dispara
- [ ] OnDeath se dispara cuando salud = 0

### Tests de Fall Damage
- [ ] Caída < threshold → sin daño
- [ ] Caída > threshold → aplica daño proporcional
- [ ] Caída muy alta → capped a maxFallDamage
- [ ] Log muestra altura y daño correctos

### Tests de Curación
- [ ] Heal(25) aumenta salud
- [ ] Heal no excede MaxHealth
- [ ] OnHealed event se dispara
- [ ] HealToFull() llega a 100%

### Tests de Items
- [ ] Medkit (instant) cura inmediatamente
- [ ] Bandage (over time) cura gradualmente
- [ ] Items se destruyen al recogerlos
- [ ] No se pueden recoger con salud full (opcional)

---

## 🎨 UI Integration (Próximo paso)

```csharp
public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Player player;
    
    void Start()
    {
        IHealth health = player.GetComponent<IHealth>();
        health.OnHealthChanged += UpdateBar;
        
        UpdateBar(health.CurrentHealth, health.MaxHealth);
    }
    
    void UpdateBar(float current, float previous)
    {
        IHealth health = player.GetComponent<IHealth>();
        fillImage.fillAmount = health.HealthPercentage;
    }
}
```

---

## 🔄 Migración Futura: Stamina System

El sistema de Stamina será **independiente** pero seguirá el mismo patrón:

```
StaminaController (Component)
├── Implements: IStamina
├── Uses: StaminaData (ScriptableObject)
└── Events: OnStaminaChanged, OnExhausted

Player
├── HealthController    (salud)
├── StaminaController   (stamina) ← Futuro
└── Integration scripts para coordinar ambos
```

**Interface ya creada:** `/Assets/Scripts/Stamina/IStamina.cs`

---

## ⚠️ Troubleshooting

### "Fall damage no se aplica"
- Verificar que `FallDamageCalculator` esté en Player
- Verificar que `canTakeFallDamage = true` en HealthData
- Verificar que `PlayerLandState` llama `OnPlayerLanded()`
- Ver logs en consola (filtrar `[FALL]`)

### "Items no curan"
- Verificar que Collider2D del item tiene `isTrigger = true`
- Verificar que Player tiene Rigidbody2D
- Verificar que `HealingItemData` está asignado
- Verificar que `CanHeal` es true (salud no está full)

### "Eventos no se disparan"
- Verificar que te subscribiste en `Awake()` o `Start()`
- Verificar que te des-subscribiste en `OnDestroy()`
- Usar `+=` para subscribir, no `=`

---

## 📊 Performance

**Optimizaciones incluidas:**
- ✅ Events en lugar de polling (`GetComponent` cada frame)
- ✅ Coroutines para regeneration (no Update)
- ✅ Cached references (no GetComponent repetidos)
- ✅ Struct para DamageData (no allocation)

**Overhead estimado:**
- HealthController: ~0.01ms/frame (solo si regenerando)
- FallDamageCalculator: ~0.001ms/frame
- HealingOverTime: ~0.01ms/tick (no por frame)

---

## 📚 Próximas Features Recomendadas

1. **Armor/Defense System** - Reducción de daño
2. **Damage Types & Resistances** - Fire, Ice, Poison, etc
3. **Status Effects** - Burn, Poison, Regen como efectos
4. **Shield System** - Capa de protección antes de health
5. **Stamina System** - Ya tiene interface preparada
6. **Health Pickups Pool** - Object pooling para items
7. **Save/Load Integration** - Persistir health entre escenas

---

## 🎯 Best Practices

### ✅ DO:
- Usar eventos para comunicación (OnDeath, OnDamaged)
- Configurar valores en ScriptableObjects
- Cachear referencias a IHealth, IDamageable
- Validar `CanHeal` antes de curar
- Usar DamageData para metadata

### ❌ DON'T:
- No llamar `GetComponent<HealthController>()` cada frame
- No modificar `currentHealth` directamente (usar TakeDamage/Heal)
- No hacer HealthController un Singleton
- No poner lógica de gameplay en HealthController
- No hardcodear valores de daño/curación

---

## 📞 Integración con GameManager (Futuro)

```csharp
// GameManager.cs
void SubscribeToPlayerHealth()
{
    Player player = FindObjectOfType<Player>();
    IHealth health = player.GetComponent<IHealth>();
    
    health.OnDeath += () => 
    {
        ChangeState(GameState.GameOver);
    };
}
```

---

**Sistema creado siguiendo los mismos principios de:**
- ✅ PlayerPhysicsController (IPlayerPhysics)
- ✅ PlayerCollisionController (IPlayerCollision)
- ✅ Modular, testeable, reusable

**Compatible con Unity 6.0 y arquitectura actual del proyecto.**
