# Player Integration Guide - Health & Interaction

**Proyecto:** TheHuntProject | **Unity:** 6000.3  
**Tema:** Cómo el Player usa HealthController e InteractionController

---

## 📋 Índice

1. [Arquitectura de Componentes](#1-arquitectura-de-componentes)
2. [HealthController - Integración con Player](#2-healthcontroller---integración-con-player)
3. [PlayerHealthIntegration - El Puente](#3-playerhealthintegration---el-puente)
4. [InteractionController - Integración con Player](#4-interactioncontroller---integración-con-player)
5. [Patrón: Event-Driven Integration](#5-patrón-event-driven-integration)
6. [Setup Completo en Player GameObject](#6-setup-completo-en-player-gameobject)
7. [Flujos Completos](#7-flujos-completos)

---

## 1. Arquitectura de Componentes

### Player GameObject - Estructura

```
Player (GameObject)
├─ Player.cs                          ← State Machine principal
├─ PlayerInputHandler.cs              ← Input del New Input System
├─ Rigidbody2D                        ← Física
├─ BoxCollider2D                      ← Colisión
├─ Animator                           ← Animaciones
│
├─ HealthController.cs                ← Sistema de salud
├─ PlayerHealthIntegration.cs         ← Conecta Health ↔ Player
├─ FallDamageCalculator.cs            ← Calcula daño por caída
│
└─ PlayerInteractionController.cs     ← Sistema de interacción
```

---

## 2. HealthController - Integración con Player

### ¿Qué hace HealthController?

`HealthController` es un **componente independiente** que:
- Gestiona HP (currentHealth, MaxHealth)
- Recibe daño via `TakeDamage()`
- Cura via `Heal()`
- Calcula fall damage via `TakeFallDamage()`
- Dispara eventos: `OnDamaged`, `OnHealed`, `OnDeath`

### ¿Cómo NO funciona?

❌ **HealthController NO conoce:**
- Clase `Player`
- Animaciones del player
- State machine del player
- Knockback del player

❌ **HealthController NO llama directamente a:**
```csharp
// ESTO NO EXISTE en HealthController ❌
player.anim.SetTrigger("damaged");
player.RB.AddForce(knockback);
```

### ¿Cómo SÍ funciona?

✅ **HealthController usa EVENTOS:**

```csharp
// En HealthController.cs
public void TakeDamage(DamageData damageData)
{
    currentHealth -= damageData.amount;
    
    // SOLO dispara eventos
    OnHealthChanged?.Invoke(currentHealth, previousHealth);
    OnDamaged?.Invoke(damageData);  // ← Otros escuchan esto
    
    if (IsDead)
        OnDeath?.Invoke();  // ← Otros escuchan esto
}
```

**Ventajas:**
- HealthController **desacoplado** del Player
- Funciona en Player, Enemy, NPC, Boss
- Fácil testear
- Fácil extender

---

## 3. PlayerHealthIntegration - El Puente

### Propósito

`PlayerHealthIntegration` es el **puente** entre:
- `HealthController` (genérico)
- `Player` (específico del jugador)

### Código Completo Comentado

```csharp
using UnityEngine;

public class PlayerHealthIntegration : MonoBehaviour
{
    // Referencias a componentes del mismo GameObject
    private Player player;
    private HealthController healthController;
    private FallDamageCalculator fallDamageCalculator;
    
    void Awake()
    {
        // Obtener referencias
        player = GetComponent<Player>();
        healthController = GetComponent<HealthController>();
        fallDamageCalculator = GetComponent<FallDamageCalculator>();
        
        // Subscribirse a eventos del HealthController
        SubscribeToEvents();
    }
    
    void OnDestroy()
    {
        // IMPORTANTE: Evitar memory leaks
        UnsubscribeFromEvents();
    }
    
    void SubscribeToEvents()
    {
        if (healthController != null)
        {
            // Escuchar eventos de HealthController
            healthController.OnDeath += HandleDeath;
            healthController.OnDamaged += HandleDamaged;
            healthController.OnHealed += HandleHealed;
        }
    }
    
    void UnsubscribeFromEvents()
    {
        if (healthController != null)
        {
            // Limpiar subscripciones
            healthController.OnDeath -= HandleDeath;
            healthController.OnDamaged -= HandleDamaged;
            healthController.OnHealed -= HandleHealed;
        }
    }
    
    // ═════════════════════════════════════════════════════
    // EVENT HANDLERS - Responden a eventos de HealthController
    // ═════════════════════════════════════════════════════
    
    void HandleDeath()
    {
        Debug.Log("<color=red>[PLAYER DEATH] Player has died!</color>");
        
        // Activar animación de muerte
        player.anim.SetBool("isDead", true);
        
        // Aquí podrías:
        // - Desactivar input
        // - Mostrar UI de Game Over
        // - Reproducir sonido de muerte
        // - Detener música
    }
    
    void HandleDamaged(DamageData damageData)
    {
        // Activar animación de daño
        player.anim.SetTrigger("damaged");
        
        // Si el daño tiene dirección, aplicar knockback
        if (damageData.damageDirection != Vector2.zero)
        {
            ApplyKnockback(damageData.damageDirection, damageData.amount);
        }
        
        // Aquí podrías:
        // - Reproducir sonido de dolor
        // - Hacer shake de cámara
        // - Mostrar VFX de sangre/chispa
        // - Interrumpir animaciones actuales
    }
    
    void HandleHealed(float amount)
    {
        Debug.Log($"<color=green>[PLAYER HEAL] Healed {amount:F1} HP</color>");
        
        // Aquí podrías:
        // - Reproducir sonido de curación
        // - Mostrar VFX de partículas verdes
        // - Animar health bar
    }
    
    void ApplyKnockback(Vector2 direction, float damageAmount)
    {
        // Fuerza proporcional al daño (cap en 10)
        float knockbackForce = Mathf.Min(damageAmount * 0.5f, 10f);
        Vector2 knockback = direction.normalized * knockbackForce;
        
        // Resetear velocidad y aplicar knockback
        player.RB.linearVelocity = Vector2.zero;
        player.RB.AddForce(knockback, ForceMode2D.Impulse);
    }
    
    // ═════════════════════════════════════════════════════
    // PUBLIC METHODS - Llamados desde otros sistemas
    // ═════════════════════════════════════════════════════
    
    public void OnPlayerLanded()
    {
        // Llamado desde PlayerLandState cuando el player aterriza
        if (fallDamageCalculator != null)
        {
            fallDamageCalculator.OnLanded();
        }
    }
}
```

---

### Flujo de Eventos: HealthController → PlayerHealthIntegration

```
Enemy golpea al Player
    ↓
IDamageable damageable = player.GetComponent<IDamageable>();
damageable.TakeDamage(new DamageData(25, Physical, direction, enemy));
    ↓
HealthController.TakeDamage(DamageData)
    ├─ currentHealth -= 25
    ├─ OnHealthChanged?.Invoke(75, 100)
    └─ OnDamaged?.Invoke(damageData)  ← EVENTO
    ↓
PlayerHealthIntegration.HandleDamaged(damageData)  ← ESCUCHA
    ├─ player.anim.SetTrigger("damaged")
    └─ ApplyKnockback(direction, 25)
        └─ player.RB.AddForce(knockback)
    ↓
Player vuela hacia atrás + animación de daño ✅
```

---

## 4. InteractionController - Integración con Player

### ¿Qué hace PlayerInteractionController?

`PlayerInteractionController` es un componente que:
- Detecta objetos cercanos (`IInteractable`)
- Escucha input de interacción (tecla E)
- Ejecuta `interactable.Interact(player)`
- Dispara eventos: `OnInteractableDetected`, `OnInteracted`

### ¿Cómo se integra con Player?

A diferencia de `HealthController`, `PlayerInteractionController` **NO necesita** un componente "bridge" como `PlayerHealthIntegration`.

**¿Por qué?**
- No modifica el estado del Player directamente
- Solo detecta y ejecuta interacciones
- El Player solo necesita **tener el componente**

### Setup Simple

```
Player GameObject
└─ PlayerInteractionController.cs
    ├─ Detection Radius: 2.0
    ├─ Interaction Layer: "Interactable"
    └─ Interact Action: Player/Interact (Input System)
```

### ¿Cuándo SÍ necesitarías integración?

Si quieres que el Player **reaccione** a interacciones:

```csharp
// OPCIONAL: PlayerInteractionFeedback.cs
using UnityEngine;
using TheHunt.Interaction;

public class PlayerInteractionFeedback : MonoBehaviour
{
    private IInteractor interactor;
    private AudioSource audioSource;
    
    void Awake()
    {
        interactor = GetComponent<IInteractor>();
        audioSource = GetComponent<AudioSource>();
        
        // Subscribirse a eventos
        interactor.OnInteracted += HandleInteracted;
    }
    
    void OnDestroy()
    {
        interactor.OnInteracted -= HandleInteracted;
    }
    
    void HandleInteracted(IInteractable interactable)
    {
        // Reproducir sonido de interacción
        audioSource.PlayOneShot(interactSound);
        
        // Animación de recoger
        player.anim.SetTrigger("pickup");
        
        // Incrementar contador de stats
        PlayerStats.itemsCollected++;
    }
}
```

---

## 5. Patrón: Event-Driven Integration

### Comparación de Arquitecturas

#### ❌ Acoplamiento Directo (MAL)

```csharp
// HealthController.cs
public class HealthController : MonoBehaviour
{
    private Player player;  // ❌ Conoce Player
    
    void Start()
    {
        player = GetComponent<Player>();
    }
    
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        
        // ❌ Directamente modifica Player
        player.anim.SetTrigger("damaged");
        player.RB.AddForce(knockback);
    }
}
```

**Problemas:**
- Solo funciona con `Player`
- No sirve para Enemy/NPC
- Difícil testear
- Acoplamiento alto

---

#### ✅ Event-Driven (BIEN)

```csharp
// HealthController.cs
public class HealthController : MonoBehaviour
{
    // ✅ NO conoce Player
    public event Action<DamageData> OnDamaged;
    
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        
        // ✅ Solo dispara evento
        OnDamaged?.Invoke(damageData);
    }
}

// PlayerHealthIntegration.cs
public class PlayerHealthIntegration : MonoBehaviour
{
    private Player player;
    private HealthController health;
    
    void Awake()
    {
        player = GetComponent<Player>();
        health = GetComponent<HealthController>();
        
        // ✅ Player escucha eventos
        health.OnDamaged += HandleDamaged;
    }
    
    void HandleDamaged(DamageData data)
    {
        // ✅ Integración específica del Player
        player.anim.SetTrigger("damaged");
        player.RB.AddForce(knockback);
    }
}
```

**Ventajas:**
- HealthController reutilizable
- Player/Enemy usan mismo HealthController
- Fácil testear
- Bajo acoplamiento

---

## 6. Setup Completo en Player GameObject

### Componentes Requeridos

```
Player
├─ Transform
├─ Rigidbody2D
├─ BoxCollider2D (layer: "Player")
├─ Animator
│
├─ Player.cs
├─ PlayerInputHandler.cs
│
├─ HealthController.cs
│   └─ Health Data: PlayerHealthData (ScriptableObject)
│
├─ PlayerHealthIntegration.cs  ← NO configuración necesaria
│
├─ FallDamageCalculator.cs
│
└─ PlayerInteractionController.cs
    ├─ Detection Radius: 2.0
    ├─ Interaction Layer: Interactable
    └─ Interact Action: Player/Interact
```

### PlayerHealthData (ScriptableObject)

```
Create > Data > Health Data > "PlayerHealthData"

Configuración:
├─ Max Health: 100
├─ Starting Health: 100
├─ Can Regenerate: ✅
├─ Regeneration Rate: 5 HP/s
├─ Regeneration Delay: 3s
├─ Invulnerability Duration: 1s
├─ Can Take Fall Damage: ✅
├─ Fall Damage Threshold: 5m
├─ Fall Damage Multiplier: 10
└─ Max Fall Damage: 50
```

---

## 7. Flujos Completos

### Flujo 1: Enemy Daña Player

```
═══════════════════════════════════════════════════════
ENEMY ATTACK SCRIPT
═══════════════════════════════════════════════════════

void OnCollisionEnter2D(Collision2D collision)
{
    IDamageable target = collision.gameObject.GetComponent<IDamageable>();
    
    if (target != null)
    {
        Vector2 dir = (collision.transform.position - transform.position).normalized;
        target.TakeDamage(new DamageData(25, Physical, dir, gameObject));
    }
}

    ↓

═══════════════════════════════════════════════════════
HEALTHCONTROLLER (PLAYER)
═══════════════════════════════════════════════════════

public void TakeDamage(DamageData data)
{
    currentHealth -= 25;  // 100 → 75
    OnDamaged?.Invoke(data);  ← EVENTO
}

    ↓

═══════════════════════════════════════════════════════
PLAYERHEALTHINTEGRATION
═══════════════════════════════════════════════════════

void HandleDamaged(DamageData data)
{
    player.anim.SetTrigger("damaged");  ← Animación
    ApplyKnockback(data.direction, 25); ← Física
}

    ↓

═══════════════════════════════════════════════════════
RESULTADO
═══════════════════════════════════════════════════════

Player:
├─ HP: 100 → 75
├─ Animación: "damaged" trigger
├─ Física: Knockback hacia atrás
└─ Invulnerabilidad: 1 segundo

UI (si está subscrita a OnHealthChanged):
└─ Health bar: Anima de 100% → 75%

Audio (si está subscrito a OnDamaged):
└─ PlayOneShot(damageSound)
```

---

### Flujo 2: Player Recoge Item de Curación

```
═══════════════════════════════════════════════════════
PLAYER SE ACERCA A POTION
═══════════════════════════════════════════════════════

PlayerInteractionController.Update()
    ↓
DetectNearbyInteractables()
    ↓
OverlapCircle encuentra Potion
    ↓
SetInteractable(potion)
    ↓
OnInteractableDetected?.Invoke(potion)
    ↓
UI: Muestra "Press E to use Potion"

    ↓

═══════════════════════════════════════════════════════
PLAYER PRESIONA E
═══════════════════════════════════════════════════════

Input System: "Interact" performed
    ↓
OnInteractPerformed(context)
    ↓
TryInteract()
    ↓
potion.Interact(player)  ← Llama método del item

    ↓

═══════════════════════════════════════════════════════
POTION INTERACTABLE
═══════════════════════════════════════════════════════

public void Interact(GameObject interactor)
{
    IHealable healable = interactor.GetComponent<IHealable>();
    healable.Heal(50);  ← Llama a HealthController
    Destroy(gameObject);
}

    ↓

═══════════════════════════════════════════════════════
HEALTHCONTROLLER (PLAYER)
═══════════════════════════════════════════════════════

public void Heal(float amount)
{
    currentHealth += 50;  // 75 → 100 (clamped)
    OnHealed?.Invoke(50);  ← EVENTO
}

    ↓

═══════════════════════════════════════════════════════
PLAYERHEALTHINTEGRATION
═══════════════════════════════════════════════════════

void HandleHealed(float amount)
{
    Debug.Log("[PLAYER HEAL] Healed 50 HP");
    // Aquí podrías:
    // - Reproducir sonido
    // - Mostrar VFX
}

    ↓

═══════════════════════════════════════════════════════
RESULTADO
═══════════════════════════════════════════════════════

Player:
└─ HP: 75 → 100

Potion:
└─ Destruido

UI:
└─ Health bar: Anima 75% → 100%

Audio:
└─ PlayOneShot(healSound)
```

---

### Flujo 3: Player Cae y Toma Fall Damage

```
═══════════════════════════════════════════════════════
PLAYER SALTA DESDE ALTURA (Y=20m)
═══════════════════════════════════════════════════════

FallDamageCalculator.Update()
    ├─ velocity.y < -5 → isFalling = true
    └─ fallStartHeight = 20

    ↓

═══════════════════════════════════════════════════════
PLAYER ATERRIZA (Y=0m)
═══════════════════════════════════════════════════════

PlayerLandState.Enter()
    ↓
PlayerHealthIntegration.OnPlayerLanded()
    ↓
FallDamageCalculator.OnLanded()
    ├─ fallDistance = 20 - 0 = 20m
    └─ healthController.TakeFallDamage(20)

    ↓

═══════════════════════════════════════════════════════
HEALTHCONTROLLER
═══════════════════════════════════════════════════════

public void TakeFallDamage(float height)
{
    if (height < 5) return;  // 20 > 5 ✅
    
    excess = 20 - 5 = 15m
    damage = Min(15 * 10, 50) = 50
    
    TakeDamage(new DamageData(50, Fall));
}

    ↓

═══════════════════════════════════════════════════════
PLAYERHEALTHINTEGRATION
═══════════════════════════════════════════════════════

HandleDamaged(DamageData{50, Fall})
    └─ player.anim.SetTrigger("damaged")

    ↓

═══════════════════════════════════════════════════════
RESULTADO
═══════════════════════════════════════════════════════

Player:
├─ HP: 100 → 50
└─ Animación: "damaged" trigger

Console:
└─ [FALL DAMAGE] Height: 20.0m | Excess: 15.0m | Damage: 50.0
```

---

## 8. Resumen de Integración

### HealthController + Player

| Componente | Responsabilidad |
|------------|-----------------|
| `HealthController` | Lógica de salud (HP, daño, curación) |
| `PlayerHealthIntegration` | Conecta salud → animaciones/física del Player |
| `FallDamageCalculator` | Detecta caídas, calcula altura |

**Comunicación:** Eventos (`OnDamaged`, `OnHealed`, `OnDeath`)

---

### InteractionController + Player

| Componente | Responsabilidad |
|------------|-----------------|
| `PlayerInteractionController` | Detecta objetos, ejecuta interacciones |
| `InteractableObject` (objetos) | Lógica específica (pickup, open, talk) |

**Comunicación:** 
- Player → Objeto: `interactable.Interact(player)`
- Objeto → Player: `IHealable.Heal()`, `IInventory.AddItem()`

---

### Patrón Común: Event-Driven

**Ventajas:**
✅ Desacoplamiento  
✅ Reutilización  
✅ Testeable  
✅ Extensible  
✅ Composición sobre herencia

---

**Siguiente paso:** Implementar UI que escuche los eventos de HealthController e InteractionController.
