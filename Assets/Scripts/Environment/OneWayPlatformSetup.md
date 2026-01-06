# 🎮 Plataformas One-Way (Pasar por Debajo)

## 📋 Guía Completa de Implementación

Esta guía te mostrará cómo crear plataformas por las que puedes pasar desde abajo y aterrizar encima, usando el **Platform Effector 2D** de Unity.

---

## 🎯 Método 1: Platform Effector 2D (Recomendado)

### ✅ Ventajas
- Solución nativa de Unity
- Configuración simple
- Funciona con cualquier collider 2D
- Soporte para Tilemaps
- Control fino de dirección

---

## 🔧 Implementación Paso a Paso

### Opción A: GameObject Individual

#### **Paso 1: Crear la Plataforma**

1. Crea un GameObject vacío (`GameObject > Create Empty`)
2. Nómbralo `OneWayPlatform`
3. Añade un **Sprite Renderer**:
   - Asigna tu sprite de plataforma
   - Layer: `Ground` (para que el player lo detecte)

#### **Paso 2: Añadir Collider**

1. Añade un **Box Collider 2D**:
   - Ajusta el tamaño para que coincida con el sprite
   - ✅ **IMPORTANTE**: Marca `Used By Effector`

#### **Paso 3: Añadir Platform Effector 2D**

1. Añade el componente **Platform Effector 2D**
2. Configura:
   - ✅ `Use One Way`: **Activado**
   - `Surface Arc`: **180** (permite pasar desde abajo)
   - `Side Arc`: **0** (opcional, para controlar lados)
   - ✅ `Use One Way Grouping`: **Activado** (para múltiples plataformas)

#### **Paso 4: Configurar Rotación**

Si tu plataforma mira hacia abajo, ajusta:
- `Rotation Offset`: **0** (flecha verde apunta arriba en Scene)

---

### Opción B: Con Tilemap (Para Niveles Grandes)

#### **Paso 1: Configurar Tilemap**

1. Crea o selecciona tu **Tilemap**
2. Añade **Tilemap Collider 2D**
3. ✅ **Marca** `Used By Effector`

#### **Paso 2: Composite Collider (Opcional pero Recomendado)**

1. Añade **Composite Collider 2D** al Tilemap
2. Configuración:
   - `Geometry Type`: **Outlines**
   - ✅ `Used By Effector`: **Activado**
3. El **Tilemap Collider 2D** automáticamente marcará `Used By Composite`

#### **Paso 3: Añadir Platform Effector 2D**

1. Añade **Platform Effector 2D** al Tilemap
2. Configuración:
   - ✅ `Use One Way`: **Activado**
   - `Surface Arc`: **180**
   - ✅ `Use One Way Grouping`: **Activado**

---

## ⚙️ Configuración Detallada del Platform Effector 2D

### Parámetros Principales

```
┌─────────────────────────────────────────────────────┐
│ Platform Effector 2D                                │
├─────────────────────────────────────────────────────┤
│ ✅ Use One Way                    (CRÍTICO)         │
│    └─ Permite pasar desde direcciones específicas  │
│                                                     │
│ Surface Arc: 180                  (DEFAULT)         │
│    └─ Ángulo donde el collider es sólido           │
│       • 180° = Solo superficie superior sólida      │
│       • 360° = Sólido desde todos los ángulos       │
│                                                     │
│ Side Arc: 0                       (OPCIONAL)        │
│    └─ Control de los lados de la plataforma        │
│                                                     │
│ ✅ Use One Way Grouping           (RECOMENDADO)     │
│    └─ Evita glitches al pasar entre plataformas    │
│                                                     │
│ Rotation Offset: 0                (AJUSTAR)         │
│    └─ Rota la dirección "arriba" del effector      │
│       • 0° = Arriba es +Y                           │
│       • 180° = Arriba es -Y                         │
└─────────────────────────────────────────────────────┘
```

---

## 🎨 Ejemplo Visual

### Configuración Correcta:

```
     👤 Player saltando
     ↑
     │  (Puede pasar)
─────┴───────  ← Plataforma con Platform Effector 2D
              Surface Arc: 180°
              ↑ Flecha verde apunta arriba

👤 Player en el suelo
```

### Dirección del Effector:

```
Scene View (al seleccionar plataforma):

──────────────
      ↑ ← Flecha verde (Surface Arc)
      │    Debe apuntar ARRIBA
      
Si no apunta arriba:
  → Ajusta "Rotation Offset"
```

---

## 🛠️ Bajar de la Plataforma (Opcional)

Si quieres que el jugador pueda bajar presionando ⬇️ + Salto:

### Script: OneWayPlatformController.cs

```csharp
using UnityEngine;

public class OneWayPlatformController : MonoBehaviour
{
    [SerializeField] private float disableTime = 0.5f;
    
    private PlatformEffector2D platformEffector;
    
    private void Awake()
    {
        platformEffector = GetComponent<PlatformEffector2D>();
    }
    
    private void Update()
    {
        if (Input.GetAxisRaw("Vertical") < 0 && Input.GetButtonDown("Jump"))
        {
            StartCoroutine(DisablePlatform());
        }
    }
    
    private System.Collections.IEnumerator DisablePlatform()
    {
        platformEffector.rotationalOffset = 180f;
        yield return new WaitForSeconds(disableTime);
        platformEffector.rotationalOffset = 0f;
    }
}
```

### Integración con tu Input System:

```csharp
using UnityEngine;

public class OneWayPlatformController : MonoBehaviour
{
    [SerializeField] private float disableTime = 0.5f;
    
    private PlatformEffector2D platformEffector;
    private PlayerInputHandler inputHandler;
    
    private void Start()
    {
        platformEffector = GetComponent<PlatformEffector2D>();
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            inputHandler = player.GetComponent<PlayerInputHandler>();
        }
    }
    
    private void Update()
    {
        if (inputHandler != null)
        {
            if (inputHandler.NormInputY < 0 && inputHandler.JumpInput)
            {
                StartCoroutine(DisablePlatform());
            }
        }
    }
    
    private System.Collections.IEnumerator DisablePlatform()
    {
        platformEffector.rotationalOffset = 180f;
        yield return new WaitForSeconds(disableTime);
        platformEffector.rotationalOffset = 0f;
    }
}
```

---

## 🔍 Troubleshooting

### ❌ Problema 1: El player no puede pasar

**Causa**: `Used By Effector` no está marcado en el collider

**Solución**:
1. Selecciona el GameObject con el collider
2. En Box Collider 2D (o el collider que uses)
3. ✅ Marca `Used By Effector`

---

### ❌ Problema 2: El player cae a través

**Causa**: Surface Arc mal configurado o dirección incorrecta

**Solución**:
1. `Surface Arc`: Debe ser **180** (no 360)
2. Verifica la flecha verde en Scene View
3. Ajusta `Rotation Offset` si la flecha no apunta arriba

---

### ❌ Problema 3: Glitches al pasar entre plataformas

**Causa**: `Use One Way Grouping` desactivado

**Solución**:
1. ✅ Activa `Use One Way Grouping` en Platform Effector 2D
2. Asegúrate de que todas las plataformas cercanas lo tengan activado

---

### ❌ Problema 4: El player rebota al pasar

**Causa**: Friction o Bounciness en Physics Material

**Solución**:
1. Crea un Physics Material 2D
2. `Friction`: **0**
3. `Bounciness`: **0**
4. Asígnalo al collider de la plataforma

---

## 🎯 Método 2: Layer Collision Matrix (Avanzado)

Si necesitas más control, puedes usar la **Physics Collision Matrix**:

### Paso 1: Crear Layers

1. `Edit > Project Settings > Tags and Layers`
2. Crea layer: `OneWayPlatform`
3. Tu Player ya tiene layer: `Player`

### Paso 2: Configurar Collision Matrix

1. `Edit > Project Settings > Physics 2D`
2. En **Layer Collision Matrix**:
   - ✅ `Player` ↔ `OneWayPlatform`: **Activado**

### Paso 3: Script de Control

```csharp
using UnityEngine;

public class OneWayPlatformScript : MonoBehaviour
{
    private BoxCollider2D platformCollider;
    
    private void Awake()
    {
        platformCollider = GetComponent<BoxCollider2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            
            // Solo colisiona si el player viene desde abajo
            if (playerRb.linearVelocity.y > 0)
            {
                Physics2D.IgnoreCollision(platformCollider, other, true);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(platformCollider, other, false);
        }
    }
}
```

**Nota**: Este método es más complejo y propenso a bugs. **Usa Platform Effector 2D** en su lugar.

---

## 📦 Prefab Recomendado

### Estructura del Prefab:

```
OneWayPlatform (GameObject)
├── Sprite Renderer
│   └── Sprite: (Tu sprite de plataforma)
├── Box Collider 2D
│   ✅ Used By Effector: true
│   └── Physics Material 2D: (Friction 0, Bounciness 0)
├── Platform Effector 2D
│   ✅ Use One Way: true
│   └── Surface Arc: 180
│   ✅ Use One Way Grouping: true
│   └── Rotation Offset: 0
└── OneWayPlatformController (Opcional)
    └── Disable Time: 0.5
```

**Layer**: `Ground`  
**Tag**: `Ground` (opcional)

---

## ✅ Checklist de Configuración

### Para cada plataforma one-way:

- [ ] GameObject tiene **Sprite Renderer**
- [ ] GameObject tiene **Box Collider 2D** (o collider 2D)
- [ ] ✅ `Used By Effector` está **marcado** en el collider
- [ ] GameObject tiene **Platform Effector 2D**
- [ ] ✅ `Use One Way` está **activado**
- [ ] `Surface Arc` = **180**
- [ ] ✅ `Use One Way Grouping` está **activado**
- [ ] Flecha verde apunta **arriba** en Scene View
- [ ] Layer = **Ground** (para detección del player)
- [ ] (Opcional) Physics Material 2D con Friction 0

---

## 🎮 Testing

### Escenarios a Probar:

1. **Saltar desde abajo**:
   - Player pasa a través ✅
   - Player aterriza encima ✅

2. **Caminar encima**:
   - Player se mantiene arriba ✅
   - No cae a través ✅

3. **Múltiples plataformas**:
   - No hay glitches al pasar entre ellas ✅
   - `Use One Way Grouping` evita bugs ✅

4. **Bajar (si implementado)**:
   - ⬇️ + Salto → Atraviesa hacia abajo ✅

---

## 💡 Tips y Trucos

### 1. **Plataformas Inclinadas**

Para plataformas rotadas:
- Ajusta `Rotation Offset` para que la flecha verde apunte perpendicular a la superficie

### 2. **Plataformas Móviles**

Platform Effector 2D funciona con plataformas móviles:
- Añade el script a la plataforma padre
- El effector se mueve con ella

### 3. **Diferentes Alturas**

Puedes tener plataformas a diferentes alturas sin problemas:
- `Use One Way Grouping` las mantiene independientes

### 4. **Performance**

Para muchas plataformas:
- Usa **Tilemap + Composite Collider 2D**
- Mejor rendimiento que múltiples GameObjects

---

## 📊 Comparación de Métodos

| Método | Dificultad | Control | Performance | Bugs |
|--------|-----------|---------|-------------|------|
| **Platform Effector 2D** | ⭐ Fácil | ⭐⭐⭐ Alto | ⭐⭐⭐ Excelente | ⭐⭐⭐ Muy pocos |
| **Collision Matrix + Script** | ⭐⭐⭐ Difícil | ⭐⭐⭐⭐ Muy alto | ⭐⭐ Medio | ⭐ Muchos |

**Recomendación**: Usa **Platform Effector 2D** siempre que sea posible.

---

## 🚀 Quick Start

### Configuración rápida en 3 pasos:

1. **Selecciona tu plataforma** (GameObject con Sprite)
2. **Añade componentes**:
   - Box Collider 2D → ✅ `Used By Effector`
   - Platform Effector 2D → ✅ `Use One Way`
3. **Ajusta**:
   - `Surface Arc`: **180**
   - ✅ `Use One Way Grouping`

¡Listo! Ahora puedes pasar por debajo. 🎉

---

## 📁 Archivos de Referencia

**Ubicación sugerida para prefabs**:
- `/Assets/Prefabs/Environment/OneWayPlatform.prefab`

**Ubicación sugerida para scripts**:
- `/Assets/Scripts/Environment/OneWayPlatformController.cs`
