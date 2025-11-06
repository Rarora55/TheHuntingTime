# PlayerCore - Arquitectura Refactorizada

## 📋 Índice
- [Visión General](#visión-general)
- [Arquitectura](#arquitectura)
- [Componentes](#componentes)
- [Migración desde Player.cs](#migración-desde-playercs)
- [Sistema de Eventos](#sistema-de-eventos)
- [Extensibilidad](#extensibilidad)

---

## 🎯 Visión General

`PlayerCore` es una refactorización completa del sistema de jugador original, diseñada siguiendo principios SOLID para mejorar:

- ✅ **Modularidad** - Cada sistema tiene una responsabilidad única
- ✅ **Testabilidad** - Interfaces permiten mocking y unit testing
- ✅ **Escalabilidad** - Fácil agregar nuevas funcionalidades
- ✅ **Mantenibilidad** - Código organizado y desacoplado
- ✅ **Extensibilidad** - Sistema de eventos para comunicación

---

## 🏗️ Arquitectura

```
PlayerCore (Orquestador)
    │
    ├─► IPlayerPhysics (PlayerPhysicsController)
    │   • Gestiona velocidad y movimiento del Rigidbody2D
    │   • Notifica cambios de velocidad mediante eventos
    │
    ├─► IPlayerCollision (PlayerCollisionController)
    │   • Detecta colisiones con suelo, paredes, ledges
    │   • Calcula posiciones de esquinas para ledge climbing
    │   • Notifica cambios de estado grounded
    │
    ├─► IPlayerAnimation (PlayerAnimationController)
    │   • Gestiona parámetros del Animator
    │   • Propaga triggers de animación al state machine
    │
    ├─► IPlayerOrientation (PlayerOrientationController)
    │   • Gestiona la dirección del jugador (FacingDirection)
    │   • Maneja rotación (Flip)
    │   • Notifica cambios de orientación
    │
    ├─► PlayerEvents
    │   • Sistema centralizado de eventos
    │   • Comunicación desacoplada entre sistemas
    │
    └─► PlayerStateMachine
        • Gestiona transiciones de estados
        • Actualiza estado actual
```

---

## 🔧 Componentes

### **1. Interfaces**

#### `IPlayerPhysics`
```csharp
public interface IPlayerPhysics
{
    Vector2 CurrentVelocity { get; }
    
    void SetVelocity(Vector2 velocity);
    void SetVelocityX(float velocityX);
    void SetVelocityY(float velocityY);
    void SetVelocityZero();
}
```

**Propósito:** Define el contrato para el control de física del jugador.

**Implementación:** `PlayerPhysicsController` - Manipula el Rigidbody2D.

---

#### `IPlayerCollision`
```csharp
public interface IPlayerCollision
{
    bool CheckIsGrounded();
    bool CheckIfTouchingWall();
    bool CheckTouchingLedge();
    bool CheckForCeiling();
    Vector2 DetermineCornerPosition();
    void SetColliderHeight(float height);
}
```

**Propósito:** Define el contrato para detección de colisiones.

**Implementación:** `PlayerCollisionController` - Usa Physics2D para raycasts.

---

#### `IPlayerAnimation`
```csharp
public interface IPlayerAnimation
{
    void SetBool(string parameterName, bool value);
    void SetFloat(string parameterName, float value);
    void SetTrigger(string parameterName);
    void AnimationTrigger();
    void AnimationFinishTrigger();
}
```

**Propósito:** Define el contrato para gestión de animaciones.

**Implementación:** `PlayerAnimationController` - Controla el Animator.

---

#### `IPlayerOrientation`
```csharp
public interface IPlayerOrientation
{
    int FacingDirection { get; }
    
    void Flip();
    void CheckFlip(int xInput);
}
```

**Propósito:** Define el contrato para orientación del jugador.

**Implementación:** `PlayerOrientationController` - Gestiona rotación del Transform.

---

### **2. Controladores**

Todos los controladores implementan sus interfaces correspondientes y reciben dependencias mediante **inyección de dependencias en el constructor**.

#### Ejemplo: `PlayerPhysicsController`
```csharp
public PlayerPhysicsController(Rigidbody2D rigidbody, PlayerEvents playerEvents)
{
    rb = rigidbody;
    events = playerEvents;
}
```

✅ **Ventajas:**
- Testeable (se puede pasar un mock Rigidbody2D)
- Desacoplado (no depende de MonoBehaviour)
- Reutilizable (puede usarse en otros contextos)

---

### **3. Sistema de Eventos**

#### `PlayerEvents`
```csharp
public class PlayerEvents
{
    public event Action<PlayerStateChangeData> OnStateChanged;
    public event Action<PlayerCollisionData> OnGroundedChanged;
    public event Action<int> OnFlipped;
    public event Action<Vector2> OnVelocityChanged;
    public event Action<PlayerAnimationEventData> OnAnimationTrigger;
}
```

#### Ejemplo de Uso:
```csharp
// Suscribirse a eventos
playerCore.Events.OnGroundedChanged += HandleGroundedChanged;
playerCore.Events.OnFlipped += HandleFlipped;

// Método handler
private void HandleGroundedChanged(PlayerCollisionData data)
{
    if (data.IsGrounded)
    {
        Debug.Log("Player landed!");
    }
}
```

---

## 🔄 Migración desde Player.cs

### **Método Automático (Recomendado)**

1. Selecciona el GameObject `Player` en la escena
2. En el Inspector, haz clic derecho en el componente `Player`
3. Selecciona **"Migrate to PlayerCore"** del menú contextual
4. Confirma la migración
5. Reasigna los campos serializados en el Inspector:
   - `PlayerData` (ScriptableObject)
   - `GroundCheck` (Transform)
   - `WallCheck` (Transform)
   - `LedgeCheck` (Transform)
   - `ceilingCheck` (Transform)

### **Método Manual**

1. Añade el componente `PlayerCore` al GameObject Player
2. Remueve el componente `Player` antiguo
3. Asigna todos los campos serializados
4. Guarda la escena

---

## 🎮 Compatibilidad con Estados Existentes

`PlayerCore` mantiene **compatibilidad total** con todos los estados existentes mediante métodos legacy:

```csharp
// En PlayerCore.cs - Legacy Compatibility Methods
public void SetVelocityZero() => Physics.SetVelocityZero();
public bool CheckIsGrounded() => Collision.CheckIsGrounded();
public void CheckFlip(int xInput) => Orientation.CheckFlip(xInput);
```

**Esto significa:**
- ✅ Todos los estados (`PlayerIdleState`, `PlayerAirState`, etc.) funcionan SIN cambios
- ✅ No es necesario refactorizar 11+ estados de golpe
- ✅ Migración gradual y segura

---

## 🚀 Extensibilidad

### **Agregar un Nuevo Sistema**

**Ejemplo: Sistema de Inventario**

#### 1. Crear interfaz:
```csharp
// /Assets/Scripts/Player/Core/Interfaces/IPlayerInventory.cs
public interface IPlayerInventory
{
    int ItemCount { get; }
    void AddItem(Item item);
    void RemoveItem(Item item);
}
```

#### 2. Crear controlador:
```csharp
// /Assets/Scripts/Player/Core/Controllers/PlayerInventoryController.cs
public class PlayerInventoryController : IPlayerInventory
{
    private List<Item> items = new List<Item>();
    private readonly PlayerEvents events;
    
    public int ItemCount => items.Count;
    
    public PlayerInventoryController(PlayerEvents playerEvents)
    {
        events = playerEvents;
    }
    
    public void AddItem(Item item)
    {
        items.Add(item);
        // Invocar evento si se añade al PlayerEvents
    }
}
```

#### 3. Integrar en PlayerCore:
```csharp
public class PlayerCore : MonoBehaviour
{
    public IPlayerInventory Inventory { get; private set; }
    
    private void InitializeCoreSystems()
    {
        // ... sistemas existentes
        Inventory = new PlayerInventoryController(Events);
    }
}
```

---

## 📊 Beneficios de la Nueva Arquitectura

### **1. Testabilidad**
```csharp
[Test]
public void TestPlayerJump()
{
    // Crear mocks
    var mockRB = new Mock<Rigidbody2D>();
    var mockEvents = new PlayerEvents();
    
    // Crear sistema
    var physics = new PlayerPhysicsController(mockRB.Object, mockEvents);
    
    // Test
    physics.SetVelocityY(15f);
    Assert.AreEqual(15f, physics.CurrentVelocity.y);
}
```

### **2. Desacoplamiento**
- Los controladores NO dependen de MonoBehaviour
- Pueden ser reutilizados en otros contextos (ej: NPCs)
- Fácil de extender sin romper código existente

### **3. Mantenibilidad**
- Cada archivo tiene ~50-150 líneas (vs 235 líneas en Player.cs original)
- Responsabilidades claras
- Fácil navegar y entender

### **4. Escalabilidad**
- Agregar sistemas (inventario, stats, habilidades) es trivial
- No contamina la clase principal
- Sistema de eventos permite comunicación flexible

---

## 📝 Próximos Pasos Sugeridos

### **Fase 2 - Refactorizar Estados**
- Crear clase base `PlayerStateNew` que use las interfaces
- Migrar estados uno por uno para usar las interfaces directamente
- Beneficio: Estados más limpios y testeables

### **Fase 3 - Sistemas de Juego**
- Inventario (IPlayerInventory)
- Stats (IPlayerStats - vida, stamina, etc.)
- Habilidades (IPlayerAbilities)
- Supervivencia (IPlayerSurvival - hambre, sed, etc.)

### **Fase 4 - Procedural/Generación**
- Sistema de generación de mundos
- Loot procedural
- Enemigos procedurales

---

## ⚠️ Notas Importantes

1. **PlayerCore es compatible con Player.cs** - Los estados antiguos siguen funcionando
2. **No se rompe nada** - Migración segura y gradual
3. **Sistema de eventos es opcional** - Puedes no usarlo inicialmente
4. **Legacy methods** - Se pueden eliminar gradualmente en el futuro

---

## 🤝 Contribuir

Para agregar nuevos sistemas:
1. Crear interfaz en `/Interfaces/`
2. Crear controlador en `/Controllers/`
3. Registrar en `PlayerCore.InitializeCoreSystems()`
4. Documentar en este README

---

**Creado:** 2024  
**Versión:** 1.0.0  
**Arquitectura:** SOLID + Event-Driven  
**Compatible con:** Unity 6000.3+
