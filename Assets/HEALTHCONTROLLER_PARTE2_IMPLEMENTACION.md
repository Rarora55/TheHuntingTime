# HealthController - Parte 2: Implementación Detallada

**Continuación de Parte 1**

---

## 7. TakeDamage - Sistema de Daño

### Sobrecarga de Métodos

```csharp
// Versión simple
public void TakeDamage(float amount)
{
    TakeDamage(new DamageData(amount, DamageType.Physical));
}

// Versión completa
public void TakeDamage(DamageData damageData) { ... }
```

**Simple:** Llamadas rápidas sin metadata  
**Completa:** Con tipo, dirección, fuente

---

### Implementación Línea por Línea

#### Guard Clauses (Líneas 50-51)

```csharp
if (IsDead || isInvulnerable)
    return;
```

**Early return** si:
- Ya muerto → No puede dañarse
- En i-frames → Ignora daño

**Patrón:** Validaciones primero, evita anidamiento.

---

#### Guardar Estado (Línea 53)

```csharp
float previousHealth = currentHealth;
```

Para:
- Evento `OnHealthChanged(new, old)`
- Calcular delta
- Animaciones de transición

---

#### Aplicar Daño (Línea 54)

```csharp
currentHealth = Mathf.Max(0, currentHealth - damageData.amount);
```

**Desglose:**
```csharp
currentHealth -= damageData.amount;       // Restar
currentHealth = Mathf.Max(0, currentHealth);  // Clamp a 0
```

**Previene valores negativos:**
```
HP=10, Damage=25
Sin clamp: -15 ❌
Con clamp: 0 ✅
```

---

#### Timestamp (Línea 55)

```csharp
lastDamageTime = Time.time;
```

Para regeneración:
```csharp
if (Time.time - lastDamageTime >= delay)
    StartHealing();
```

---

#### Debug Log (Líneas 57-58)

```csharp
Debug.Log($"<color=orange>[HEALTH] {gameObject.name} " +
          $"took {damageData.amount:F1} {damageData.damageType} " +
          $"damage. Health: {currentHealth:F1}/{MaxHealth}</color>");
```

**Features:**
- String interpolation: `$"{var}"`
- Format: `:F1` (1 decimal)
- Rich text: `<color=orange>`

**Output:**
```
[HEALTH] Player took 25.0 Physical damage. Health: 75.0/100
```

---

#### Disparar Eventos (Líneas 60-61)

```csharp
OnHealthChanged?.Invoke(currentHealth, previousHealth);
OnDamaged?.Invoke(damageData);
```

**`?.` Null-conditional:**
- Con subscribers → invoca
- Sin subscribers → no hace nada (no crash)

---

#### Activar i-frames (Líneas 63-66)

```csharp
if (healthData.invulnerabilityDuration > 0)
{
    StartCoroutine(InvulnerabilityRoutine());
}
```

**Timeline:**
```
T=0: Daño recibido → isInvulnerable = true
T=0-1s: WaitForSeconds → TakeDamage ignora daño
T=1s: isInvulnerable = false
```

---

#### Verificar Muerte (Líneas 68-71)

```csharp
if (IsDead)
{
    Die();
}
```

Usa property `IsDead` para consistencia.

---

### Flujo Completo de Daño

```
Enemy ataca
    ↓
IDamageable.TakeDamage(DamageData)
    ↓
HealthController.TakeDamage()
    ├─ Guard: IsDead || invulnerable → return
    ├─ currentHealth -= amount
    ├─ OnHealthChanged?.Invoke()
    ├─ OnDamaged?.Invoke()
    ├─ Activar i-frames
    └─ if (IsDead) Die()
    ↓
Eventos disparados:
    ├─ UI → Health bar
    ├─ Integration → Animation + knockback
    ├─ VFX → Particles
    └─ Audio → Sound
```

---

## 8. Heal - Sistema de Curación

### Implementación

```csharp
public void Heal(float amount)
{
    if (!CanHeal)
        return;
    
    float previousHealth = currentHealth;
    currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
    
    Debug.Log($"<color=green>[HEALTH] {gameObject.name} " +
              $"healed {amount:F1}. Health: {currentHealth:F1}/" +
              $"{MaxHealth}</color>");
    
    OnHealthChanged?.Invoke(currentHealth, previousHealth);
    OnHealed?.Invoke(amount);
}
```

---

### Guard Clause

```csharp
if (!CanHeal)
    return;
```

**CanHeal = IsAlive && currentHealth < MaxHealth**

Previene:
- Curar muertos
- Exceder max HP

---

### Clamp a MaxHealth

```csharp
currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
```

**Ejemplo:**
```
HP=90, Heal=30
90 + 30 = 120
Min(100, 120) = 100 ✅
```

---

### HealToFull()

```csharp
public void HealToFull()
{
    Heal(MaxHealth - currentHealth);
}
```

**Calcula diferencia:**
```
HP=60, Max=100
Heal(100 - 60) = Heal(40)
→ HP = 100
```

**Delega a `Heal()`** - Reutiliza lógica.

---

### Flujo de Curación

```
Checkpoint/Item
    ↓
IHealable.Heal(amount)
    ↓
HealthController.Heal()
    ├─ Guard: !CanHeal → return
    ├─ currentHealth += amount (clamped)
    ├─ OnHealthChanged?.Invoke()
    └─ OnHealed?.Invoke()
    ↓
Eventos:
    ├─ UI → Anima bar
    ├─ VFX → Heal particles
    └─ Audio → Heal sound
```

---

## 9. Fall Damage

### TakeFallDamage()

```csharp
public void TakeFallDamage(float fallHeight)
{
    if (!healthData.canTakeFallDamage)
        return;
    
    if (fallHeight < healthData.fallDamageThreshold)
        return;
    
    float excessHeight = fallHeight - healthData.fallDamageThreshold;
    
    float damage = Mathf.Min(
        excessHeight * healthData.fallDamageMultiplier,
        healthData.maxFallDamage
    );
    
    Debug.Log($"<color=yellow>[FALL DAMAGE] Height: {fallHeight:F1}m | " +
              $"Excess: {excessHeight:F1}m | Damage: {damage:F1}</color>");
    
    TakeDamage(new DamageData(damage, DamageType.Fall));
}
```

---

### Cálculo de Daño

**Fórmula:**
```
if (height < threshold) → 0 damage
else:
    excess = height - threshold
    damage = Min(excess × multiplier, maxDamage)
```

**Ejemplo (threshold=5m, mult=10, cap=50):**

| Altura | Excess | Calc | Cap | Final |
|--------|--------|------|-----|-------|
| 3m     | 0      | 0    | -   | 0     |
| 7m     | 2m     | 20   | 50  | 20    |
| 10m    | 5m     | 50   | 50  | 50    |
| 20m    | 15m    | 150  | 50  | 50    |

---

### Flujo Fall Damage

```
Player cae desde Y=22m
    ↓
FallDamageCalculator
    - Detecta velocity < -5
    - StartFalling() → fallStartHeight = 22m
    ↓
Player aterriza en Y=2m
    ↓
PlayerLandState → OnPlayerLanded()
    ↓
FallDamageCalculator.OnLanded()
    - fallDistance = 22 - 2 = 20m
    ↓
HealthController.TakeFallDamage(20m)
    - Threshold: 5m
    - Excess: 15m
    - Damage: Min(150, 50) = 50
    ↓
TakeDamage(DamageData{50, Fall})
    - HP: 100 → 50
```

---

## 10. Regeneración

### RegenerationRoutine()

```csharp
IEnumerator RegenerationRoutine()
{
    while (true)
    {
        yield return new WaitForSeconds(0.1f);
        
        if (IsAlive && 
            currentHealth < MaxHealth && 
            Time.time - lastDamageTime >= healthData.regenerationDelay)
        {
            float regenAmount = healthData.regenerationRate * 0.1f;
            Heal(regenAmount);
        }
    }
}
```

---

### Condiciones

1. **IsAlive** - No regenerar muerto
2. **currentHealth < MaxHealth** - No si full
3. **Time.time - lastDamageTime >= delay** - Delay después daño

---

### Ejemplo Timeline

**Config: rate=5HP/s, delay=3s**

```
T=0s: HP=60, recibe daño → lastDamageTime=0

T=1s: 1-0=1s < 3s → NO regenera (delay)
T=2s: 2-0=2s < 3s → NO regenera
T=3s: 3-0=3s → EMPIEZA ✅
      → HP: 60.5 (regen = 5×0.1 = 0.5/tick)

T=3.1s: HP: 61.0
T=3.2s: HP: 61.5
...
T=11s: HP: 100 → PARA (full)

T=15s: Daño → lastDamageTime=15, HP=75
T=16s: 16-15=1s < 3s → NO regenera
T=18s: 18-15=3s → RESUME ✅
```

---

### ¿Por qué tick 0.1s?

**Update() (❌):**
```csharp
void Update()
{
    Heal(rate * Time.deltaTime);
}
// - 60+ veces/segundo
// - Overhead innecesario
```

**Coroutine (✅):**
```csharp
yield return new WaitForSeconds(0.1f);
// - 10 veces/segundo
// - Suficiente precisión
// - Mejor performance
```

---

### Conversión HP/s a HP/tick

```csharp
float regenAmount = regenerationRate * 0.1f;
```

**Verificación:**
```
rate = 5 HP/s
regenAmount = 5 × 0.1 = 0.5 HP/tick

1s = 10 ticks
10 ticks × 0.5 HP = 5 HP/s ✅
```

---

## 11. Die() y ResetHealth()

### Die()

```csharp
void Die()
{
    Debug.Log($"<color=red>[HEALTH] {gameObject.name} has died!</color>");
    
    if (regenerationCoroutine != null)
    {
        StopCoroutine(regenerationCoroutine);
        regenerationCoroutine = null;
    }
    
    OnDeath?.Invoke();
}
```

**Detiene regeneración** para evitar memory leak.

---

### ResetHealth()

```csharp
public void ResetHealth()
{
    currentHealth = healthData.startingHealth;
    isInvulnerable = false;
    OnHealthChanged?.Invoke(currentHealth, currentHealth);
}
```

**Para respawns/checkpoints.**

**Nota:** Pasa `(current, current)` - UI puede detectar reset vs cambio gradual.

---

## 12. Invulnerability Routine

```csharp
IEnumerator InvulnerabilityRoutine()
{
    isInvulnerable = true;
    yield return new WaitForSeconds(healthData.invulnerabilityDuration);
    isInvulnerable = false;
}
```

**3 pasos simples:**
1. Activar flag
2. Esperar
3. Desactivar flag

**Durante espera:** `TakeDamage()` retorna sin procesar.

---

## 13. OnValidate()

```csharp
void OnValidate()
{
    if (healthData != null && currentHealth > healthData.maxHealth)
    {
        currentHealth = healthData.maxHealth;
    }
}
```

**Solo en Editor** - Ajusta HP si cambias max en Inspector.

**Escenario:**
```
1. currentHealth=100, maxHealth=100
2. Cambias maxHealth=50 en Inspector
3. OnValidate() → currentHealth=50 ✅
```

---

**Continúa en:** HEALTHCONTROLLER_PARTE3_EJEMPLOS.md
