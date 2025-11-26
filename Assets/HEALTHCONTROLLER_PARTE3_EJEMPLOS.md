# HealthController - Parte 3: Ejemplos y Patrones

**Continuación de Parte 2**

---

## 14. Ejemplos Completos

### Ejemplo 1: Enemy Ataca Player

```csharp
public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float damage = 15f;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable target = collision.gameObject.GetComponent<IDamageable>();
        
        if (target != null && target.IsAlive)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            
            DamageData data = new DamageData(
                amount: damage,
                damageType: DamageType.Physical,
                direction: direction,
                source: gameObject
            );
            
            target.TakeDamage(data);
        }
    }
}
```

**Resultado:**
```
Console: [HEALTH] Player took 15.0 Physical damage. Health: 85.0/100
Eventos: OnHealthChanged(85, 100) → UI
         OnDamaged(data) → Animation + knockback
```

---

### Ejemplo 2: Checkpoint Cura

```csharp
public class CheckpointZone : MonoBehaviour
{
    [SerializeField] private GameObject healVFX;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IHealable healable = other.GetComponent<IHealable>();
            
            if (healable != null && healable.CanHeal)
            {
                healable.HealToFull();
                
                if (healVFX != null)
                    Instantiate(healVFX, other.transform.position, Quaternion.identity);
            }
        }
    }
}
```

**Resultado:**
```
Console: [HEALTH] Player healed 15.0. Health: 100.0/100
Eventos: OnHealthChanged(100, 85) → UI anima
         OnHealed(15) → VFX + Sound
```

---

### Ejemplo 3: UI Health Bar

```csharp
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Text healthText;
    
    private IHealth health;
    
    void Start()
    {
        health = player.GetComponent<IHealth>();
        
        if (health != null)
        {
            health.OnHealthChanged += UpdateHealthBar;
            health.OnDeath += ShowGameOver;
            
            UpdateHealthBar(health.CurrentHealth, health.MaxHealth);
        }
    }
    
    void UpdateHealthBar(float newHP, float oldHP)
    {
        healthBarFill.fillAmount = health.HealthPercentage;
        healthText.text = $"{newHP:F0} / {health.MaxHealth:F0}";
        
        if (health.HealthPercentage < 0.25f)
            healthBarFill.color = Color.red;
        else if (health.HealthPercentage < 0.5f)
            healthBarFill.color = Color.yellow;
        else
            healthBarFill.color = Color.green;
    }
    
    void ShowGameOver()
    {
        Debug.Log("GAME OVER!");
    }
    
    void OnDestroy()
    {
        if (health != null)
        {
            health.OnHealthChanged -= UpdateHealthBar;
            health.OnDeath -= ShowGameOver;
        }
    }
}
```

---

### Ejemplo 4: Caída + Curación (Escenario Completo)

```
FASE 1: CAÍDA (20m)
───────────────────
T=0s: Player Y=22m
T=1s: Velocity < -5 → StartFalling()
      fallStartHeight = 22m
T=3.2s: Aterriza Y=2m
        OnLanded() → fallDistance = 20m

FASE 2: FALL DAMAGE
───────────────────
TakeFallDamage(20m)
→ Threshold: 5m
→ Excess: 15m
→ Damage: Min(150, 50) = 50
→ HP: 100 → 50

FASE 3: USAR POTION
───────────────────
Player presiona tecla H
→ ConsumableItemData.Use(player)
→ CanUse? IsAlive && 50<100 ✅
→ Heal(50)
→ HP: 50 → 100 ✅
```

---

## 15. Patrones de Diseño

### 1. Interface Segregation

```csharp
IHealth      → Solo lectura
IDamageable  → Solo daño
IHealable    → Solo curación
```

**Ventaja:** Scripts usan solo lo que necesitan.

```csharp
// UI solo IHealth
IHealth h = player.GetComponent<IHealth>();
bar.fillAmount = h.HealthPercentage;

// Enemy solo IDamageable
IDamageable d = player.GetComponent<IDamageable>();
d.TakeDamage(15f);
```

---

### 2. Event-Driven Architecture

**Desacoplamiento total:**

```csharp
// HealthController NO conoce UI/Audio/VFX
// Solo dispara eventos
OnHealthChanged?.Invoke(newHP, oldHP);

// Otros escuchan
UIController → Actualiza UI
AudioManager → Play sound
VFXManager → Particles
```

**Ventaja:** Agregar sistemas sin modificar HealthController.

---

### 3. Guard Clauses

```csharp
// ✅ Bueno
public void TakeDamage(DamageData data)
{
    if (IsDead || isInvulnerable)
        return;
    
    // ... código
}

// ❌ Malo - anidamiento
public void TakeDamage(DamageData data)
{
    if (!IsDead)
    {
        if (!isInvulnerable)
        {
            // ... código
        }
    }
}
```

---

### 4. DRY (Don't Repeat Yourself)

```csharp
// Métodos simples delegan
public void TakeDamage(float amount)
{
    TakeDamage(new DamageData(amount, DamageType.Physical));
}

public void HealToFull()
{
    Heal(MaxHealth - currentHealth);
}
```

**Lógica centralizada.**

---

### 5. Single Responsibility

Cada componente una tarea:

```
HealthController        → Gestionar salud
FallDamageCalculator    → Detectar caídas
PlayerHealthIntegration → Conectar con Player
ConsumableItemData      → Efectos de items
```

---

### 6. Data-Driven Design

```csharp
// ✅ ScriptableObject
public HealthData healthData;

// ❌ Hardcoded
private float maxHealth = 100f;
```

**Ventajas:**
- Sin recompilar
- Múltiples configs
- Balanceo fácil

---

### 7. Coroutines para Timing

```csharp
// ✅ Coroutine limpia
IEnumerator InvulnerabilityRoutine()
{
    isInvulnerable = true;
    yield return new WaitForSeconds(1f);
    isInvulnerable = false;
}

// ❌ Update complejo
float timer = 0;
void Update()
{
    if (isInvulnerable)
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            isInvulnerable = false;
            timer = 0;
        }
    }
}
```

---

## 16. Preguntas Frecuentes

### ¿Por qué interfaces vs clases base?

**Interfaces:**
- Múltiple herencia (C# no soporta múltiple de clases)
- Más flexible
- Mejor para testing

---

### ¿Cómo prevenir memory leaks con eventos?

**SIEMPRE unsubscribe:**

```csharp
void Start()
{
    health.OnDeath += HandleDeath;
}

void OnDestroy()
{
    health.OnDeath -= HandleDeath;  // ✅
}
```

Sin esto, objeto se mantiene en memoria.

---

### ¿Puedo modificar HP directamente?

**NO:**
```csharp
health.currentHealth = 50;  // ❌ Private
```

**SÍ:**
```csharp
health.TakeDamage(50);  // ✅
health.Heal(50);        // ✅
health.ResetHealth();   // ✅
```

---

### ¿Cómo agregar armor?

**Opción 1: En HealthController**
```csharp
float finalDamage = damageData.amount - armorValue;
finalDamage = Mathf.Max(0, finalDamage);
currentHealth -= finalDamage;
```

**Opción 2: Componente separado**
```csharp
public class ArmorController : MonoBehaviour
{
    public float CalculateFinalDamage(DamageData data)
    {
        float damage = data.amount - armorValue;
        damage *= GetResistance(data.damageType);
        return Mathf.Max(0, damage);
    }
}
```

---

### ¿Cómo implementar God Mode?

```csharp
private bool godModeActive = false;

public void EnableGodMode(float duration)
{
    StartCoroutine(GodModeRoutine(duration));
}

IEnumerator GodModeRoutine(float duration)
{
    godModeActive = true;
    yield return new WaitForSeconds(duration);
    godModeActive = false;
}

public void TakeDamage(DamageData data)
{
    if (IsDead || isInvulnerable || godModeActive)
        return;
    
    // ... código
}
```

---

## 17. Resumen Final

El `HealthController` es:

✅ **Robusto** - Maneja edge cases  
✅ **Extensible** - Fácil agregar features  
✅ **Modular** - Componentes separados  
✅ **Testeable** - Interfaces + eventos  
✅ **Configurable** - ScriptableObjects  
✅ **Reutilizable** - Player/Enemy/NPC/Boss  
✅ **Performance** - Coroutines en vez de Update  
✅ **Maintainable** - Código limpio y documentado

---

## 18. Extensiones Futuras

Posibles mejoras:

- **Armor/Resistances** - Reducir daño por tipo
- **Shields** - Capa separada de HP
- **Damage Over Time** - Poison, burn
- **Critical Hits** - Daño aleatorio aumentado
- **Damage Numbers** - Floating text
- **Health Pickups** - Items en mundo
- **Multiplayer Sync** - Networked health

---

## 19. Convertir a PDF

**Opciones para exportar estas documentaciones a PDF:**

### Opción 1: VS Code
1. Instalar extensión "Markdown PDF"
2. Abrir archivo .md
3. Ctrl+Shift+P → "Markdown PDF: Export (pdf)"

### Opción 2: Pandoc (línea comandos)
```bash
pandoc HEALTHCONTROLLER_PARTE1_FUNDAMENTOS.md -o Parte1.pdf
pandoc HEALTHCONTROLLER_PARTE2_IMPLEMENTACION.md -o Parte2.pdf
pandoc HEALTHCONTROLLER_PARTE3_EJEMPLOS.md -o Parte3.pdf
```

### Opción 3: Online
- https://www.markdown-pdf.com
- https://md2pdf.netlify.app

### Opción 4: Combinar en un solo PDF
```bash
pandoc HEALTHCONTROLLER_PARTE1_FUNDAMENTOS.md \
       HEALTHCONTROLLER_PARTE2_IMPLEMENTACION.md \
       HEALTHCONTROLLER_PARTE3_EJEMPLOS.md \
       -o HealthController_Completo.pdf
```

---

**Documentación completa para:** TheHuntProject  
**Unity Version:** 6000.3  
**Autor:** Bezi AI Assistant  
**Fecha:** 2024
