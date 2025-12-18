# 🏗️ Refactorización: Separación de Responsabilidades

Documentación sobre la refactorización del sistema de inventario para separar la lógica de llaves.

---

## 🎯 Problema Original

El `InventorySystem` estaba gestionando **demasiadas responsabilidades**:

```
InventorySystem.cs (364 líneas)
  ├─ Gestión de items generales
  ├─ Gestión de armas
  ├─ Gestión de munición
  ├─ Gestión de llaves  ← Extraído a KeyInventoryManager
  ├─ Selección de slots
  ├─ Uso/Drop/Examine items
  └─ Eventos
```

**Problemas:**
- ❌ Violación del **Single Responsibility Principle**
- ❌ Código largo y difícil de mantener
- ❌ Mezcla de diferentes conceptos
- ❌ Difícil de testear individualmente

---

## ✅ Solución Implementada

Separar la lógica de llaves en un componente dedicado:

```
ANTES:
  Player
    └─ InventorySystem (todo en uno)

DESPUÉS:
  Player
    ├─ InventorySystem (gestión general)
    └─ KeyInventoryManager (gestión de llaves)
```

---

## 📦 Arquitectura Refactorizada

### Componente 1: InventorySystem

**Responsabilidades:**
- ✅ Gestionar slots de items
- ✅ Añadir/remover items
- ✅ Equipar armas
- ✅ Gestionar munición
- ✅ Usar/examinar/drop items
- ✅ Selección de slots
- ✅ Eventos de inventario

**Líneas de código:** ~310 (reducido desde 364)

**NO gestiona:**
- ❌ Lógica específica de llaves
- ❌ Verificación de IDs de llaves
- ❌ Consumo de llaves

---

### Componente 2: KeyInventoryManager (NUEVO)

**Responsabilidades:**
- ✅ Verificar si tiene una llave (HasKeyItem)
- ✅ Consumir llaves (ConsumeKeyItem)
- ✅ Obtener datos de llaves (GetKeyData)
- ✅ Contar llaves en inventario (GetKeyCount)
- ✅ Eventos específicos de llaves

**Líneas de código:** ~140

**Depende de:**
- ✅ InventorySystem (composición)

---

## 🔄 Cambios Realizados

### 1. Creado `KeyInventoryManager.cs`

```csharp
namespace TheHunt.Inventory
{
    public class KeyInventoryManager : MonoBehaviour
    {
        private InventorySystem inventorySystem;
        
        public event Action<string, KeyItemData> OnKeyFound;
        public event Action<string, KeyItemData> OnKeyConsumed;
        
        public bool HasKeyItem(string keyID) { ... }
        public bool ConsumeKeyItem(string keyID) { ... }
        public KeyItemData GetKeyData(string keyID) { ... }
        public int GetKeyCount() { ... }
    }
}
```

**Ubicación:** `/Assets/Scripts/Inventory/Core/KeyInventoryManager.cs`

---

### 2. Modificado `InventorySystem.cs`

**Eliminado:**
```csharp
// ❌ ELIMINADO
public bool HasKeyItem(string keyID) { ... }
public bool ConsumeKeyItem(string keyID) { ... }
```

**Resultado:**
- InventorySystem ahora es más simple
- 54 líneas menos
- Más fácil de leer y mantener

---

### 3. Actualizado `LockedDoorInteractable.cs`

**ANTES:**
```csharp
Inventory.InventorySystem inventory = interactor.GetComponent<Inventory.InventorySystem>();
bool hasKey = inventory.HasKeyItem(requiredKeyID);
inventory.ConsumeKeyItem(requiredKeyID);
```

**DESPUÉS:**
```csharp
Inventory.KeyInventoryManager keyManager = interactor.GetComponent<Inventory.KeyInventoryManager>();
bool hasKey = keyManager.HasKeyItem(requiredKeyID);
keyManager.ConsumeKeyItem(requiredKeyID);
```

**Ventajas:**
- ✅ Más semántico (keyManager vs inventory)
- ✅ Separación clara de responsabilidades
- ✅ Fácil de extender con funcionalidad específica de llaves

---

## 🎨 Beneficios de la Refactorización

### 1. **Single Responsibility Principle**

```
ANTES (InventorySystem):
  - Gestiona items ✓
  - Gestiona armas ✓
  - Gestiona munición ✓
  - Gestiona llaves ✓  ← Demasiadas responsabilidades

DESPUÉS:
  InventorySystem:
    - Gestiona items ✓
    - Gestiona armas ✓
    - Gestiona munición ✓
  
  KeyInventoryManager:
    - Gestiona llaves ✓  ← Responsabilidad única
```

---

### 2. **Mantenibilidad**

```
Añadir nueva funcionalidad de llaves:
  
ANTES:
  - Buscar en InventorySystem (364 líneas)
  - Mezclar con lógica de items/armas/munición
  - Riesgo de romper otras funcionalidades

DESPUÉS:
  - Abrir KeyInventoryManager (140 líneas)
  - Lógica aislada y clara
  - Menor riesgo de efectos secundarios
```

---

### 3. **Extensibilidad**

```
Ejemplos de extensiones fáciles:

KeyInventoryManager:
  ✅ Añadir sistema de conteo (Zelda style)
  ✅ Añadir master keys
  ✅ Añadir llaves temporales (expiran)
  ✅ Añadir llaves compartidas (multiplayer)
  ✅ Añadir crafting de llaves
  ✅ Añadir jerarquía de llaves
```

---

### 4. **Testabilidad**

```
Testing:

ANTES:
  - Testear InventorySystem completo
  - Difícil aislar lógica de llaves
  - Tests lentos y complejos

DESPUÉS:
  - Testear KeyInventoryManager independientemente
  - Mock de InventorySystem simple
  - Tests rápidos y específicos
```

---

## 🔧 Cómo Usar

### Setup en Player GameObject

```
Player
  Components:
    1. InventorySystem ✓
    2. KeyInventoryManager ✓  ← AÑADIR ESTE
    3. PlayerInteractionController ✓
```

**Inspector:**
```
Player GameObject
  ├─ InventorySystem (script)
  │    - (configuración normal)
  │
  └─ KeyInventoryManager (script)
       - (no necesita configuración, auto-detecta InventorySystem)
```

---

### Uso desde Otros Scripts

**Verificar si tiene llave:**
```csharp
KeyInventoryManager keyManager = player.GetComponent<KeyInventoryManager>();

if (keyManager.HasKeyItem("rusty_key"))
{
    Debug.Log("Player has the rusty key!");
}
```

**Consumir llave:**
```csharp
KeyInventoryManager keyManager = player.GetComponent<KeyInventoryManager>();

if (keyManager.ConsumeKeyItem("boss_key"))
{
    Debug.Log("Boss key consumed!");
}
```

**Obtener datos de llave:**
```csharp
KeyInventoryManager keyManager = player.GetComponent<KeyInventoryManager>();

KeyItemData keyData = keyManager.GetKeyData("master_key");

if (keyData != null)
{
    Debug.Log($"Found key: {keyData.ItemName}");
    Debug.Log($"Description: {keyData.ItemDescription}");
}
```

**Contar llaves:**
```csharp
KeyInventoryManager keyManager = player.GetComponent<KeyInventoryManager>();

int totalKeys = keyManager.GetKeyCount();
Debug.Log($"Player has {totalKeys} keys");
```

---

### Escuchar Eventos

```csharp
public class DoorManager : MonoBehaviour
{
    void Start()
    {
        KeyInventoryManager keyManager = GetComponent<KeyInventoryManager>();
        
        keyManager.OnKeyFound += HandleKeyFound;
        keyManager.OnKeyConsumed += HandleKeyConsumed;
    }
    
    void HandleKeyFound(string keyID, KeyItemData keyData)
    {
        Debug.Log($"Key found: {keyData.ItemName} for {keyID}");
        // Actualizar UI, reproducir sonido, etc.
    }
    
    void HandleKeyConsumed(string keyID, KeyItemData keyData)
    {
        Debug.Log($"Key consumed: {keyData.ItemName}");
        // Actualizar UI, mostrar notificación, etc.
    }
}
```

---

## 🚀 Próximas Refactorizaciones

Siguiendo este patrón, podemos separar otras responsabilidades:

### 1. **AmmoInventoryManager**

```
Extraer de InventorySystem:
  - AddAmmo()
  - RemoveAmmo()
  - GetAmmoCount()
  - HasAmmo()
```

**Beneficio:** Gestión independiente de munición

---

### 2. **WeaponInventoryManager**

```
Extraer de InventorySystem:
  - EquipWeapon()
  - UnequipWeapon()
  - SwapWeapons()
  - GetEquippedWeapon()
```

**Beneficio:** Gestión independiente de equipamiento

---

### 3. **ConsumableInventoryManager**

```
Nueva funcionalidad:
  - UseConsumable()
  - GetConsumablesByType()
  - HasConsumable()
```

**Beneficio:** Lógica específica para consumibles

---

## 📊 Comparación Antes/Después

| Aspecto | ANTES | DESPUÉS |
|---------|-------|---------|
| **Líneas InventorySystem** | 364 | 310 |
| **Responsabilidades** | 6+ | 4 |
| **Componentes** | 1 | 2 |
| **Mantenibilidad** | ⭐⭐ | ⭐⭐⭐⭐⭐ |
| **Testabilidad** | ⭐⭐ | ⭐⭐⭐⭐⭐ |
| **Extensibilidad** | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| **Legibilidad** | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ |

---

## 🎯 Principios Aplicados

### 1. Single Responsibility Principle (SRP)

```
Cada clase tiene UNA razón para cambiar:

InventorySystem:
  - Cambia si modificas cómo se gestionan items en general

KeyInventoryManager:
  - Cambia si modificas cómo funcionan las llaves
```

---

### 2. Composition Over Inheritance

```
EVITADO (Herencia):
  class InventorySystem { ... }
  class KeyInventorySystem : InventorySystem { ... }  ❌

USADO (Composición):
  class InventorySystem { ... }
  class KeyInventoryManager {
      private InventorySystem inventory;  ✅
  }
```

---

### 3. Separation of Concerns

```
Cada componente tiene su preocupación:

InventorySystem:
  → Preocupación: Gestión de slots y items

KeyInventoryManager:
  → Preocupación: Lógica específica de llaves
```

---

### 4. Open/Closed Principle

```
Abierto para extensión, cerrado para modificación:

Añadir funcionalidad de llaves:
  ✅ Extender KeyInventoryManager (abierto)
  ❌ No modificar InventorySystem (cerrado)
```

---

## 📝 Notas de Migración

### Si ya tienes un proyecto existente:

**Paso 1:** Añadir `KeyInventoryManager` al Player
```
Player GameObject
  Add Component → KeyInventoryManager
```

**Paso 2:** Actualizar scripts que usan llaves
```csharp
// ANTES
InventorySystem inventory = player.GetComponent<InventorySystem>();
inventory.HasKeyItem("key");

// DESPUÉS
KeyInventoryManager keyManager = player.GetComponent<KeyInventoryManager>();
keyManager.HasKeyItem("key");
```

**Paso 3:** Recompilar y testear
```
1. Build → Recompile
2. Play mode
3. Verificar que las llaves funcionan correctamente
```

---

## ✅ Checklist Post-Refactorización

- [x] `KeyInventoryManager.cs` creado
- [x] Métodos de llaves eliminados de `InventorySystem`
- [x] `LockedDoorInteractable` actualizado
- [x] Sin errores de compilación
- [x] Tests pasados
- [x] Documentación actualizada

---

## 🎓 Lecciones Aprendidas

1. **Detectar responsabilidades múltiples:**
   - Si una clase tiene más de 300 líneas
   - Si tiene métodos de dominios diferentes
   - Si es difícil darle un nombre descriptivo

2. **Refactorizar progresivamente:**
   - No refactorizar todo de golpe
   - Empezar con una responsabilidad
   - Testear después de cada cambio

3. **Mantener la API pública simple:**
   - Los métodos públicos deben ser claros
   - Evitar exponer implementación interna
   - Eventos para comunicación entre componentes

---

¡Refactorización completada! 🎉✨
