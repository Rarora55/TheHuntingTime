# ⚠️ ACLARACIÓN IMPORTANTE: Sistema de Llaves

## 🚨 El Problema que Detectaste

Tienes **completamente razón**. Si reutilizas un SO con un solo ID, se crea este problema:

```
❌ PROBLEMA:

KeyItemData: "SmallKeyData"
  - Unlocks: ["common_door"]

Si recoges 1 llave...

LockedDoor_Habitacion101
  requiredKeyID: "common_door" ← Se abre ✓

LockedDoor_Habitacion102
  requiredKeyID: "common_door" ← También se abre ✓

LockedDoor_Cofre
  requiredKeyID: "common_door" ← También se abre ✓

Resultado: ¡1 LLAVE ABRE TODO!
```

---

## ✅ Soluciones Reales

Hay **3 estrategias diferentes** según el comportamiento que quieras:

---

### 🎮 CASO 1: Cada Llave Abre Solo UNA Puerta Específica

**Ejemplo:** Llaves de habitaciones de hotel (llave 101 solo abre habitación 101)

**Solución:** Necesitas **SOs únicos por llave** con **IDs únicos**

```
KeyItemData: "Key_Room101"
  - Unlocks: ["room_101"]
    
KeyItemData: "Key_Room102"
  - Unlocks: ["room_102"]
    
KeyItemData: "Key_Room103"
  - Unlocks: ["room_103"]

Puertas:
  Room101_Door → requiredKeyID: "room_101"
  Room102_Door → requiredKeyID: "room_102"
  Room103_Door → requiredKeyID: "room_103"

Resultado:
  - Llave 101 solo abre puerta 101 ✓
  - Llave 102 solo abre puerta 102 ✓
  - Llave 103 solo abre puerta 103 ✓
  
SOs necesarios: 3 (uno por llave)
```

**Conclusión:** Para este caso **SÍ necesitas un SO por llave**.

---

### 🗝️ CASO 2: Una Llave Abre Múltiples Puertas del Mismo Tipo

**Ejemplo:** Keycard de seguridad roja abre todas las puertas rojas

**Solución:** Reutilizar SO con **un ID compartido** es correcto

```
KeyItemData: "RedKeycardData"
  - Unlocks: ["red_security"]

Puertas:
  Red_Door_A → requiredKeyID: "red_security"
  Red_Door_B → requiredKeyID: "red_security"
  Red_Door_C → requiredKeyID: "red_security"

Resultado:
  - 1 Keycard Roja abre todas las puertas rojas ✓
  
SOs necesarios: 1 (reutilizado intencionalmente)
```

**Conclusión:** Para este caso **1 SO es suficiente** (comportamiento deseado).

---

### 🔢 CASO 3: Sistema de Conteo (Zelda Style)

**Ejemplo:** Necesitas X llaves pequeñas para abrir X puertas (conteo, no IDs)

**Solución:** Modificar el sistema para usar **contador de llaves**

```
❌ Sistema ACTUAL (basado en IDs):
  - No soporta conteo
  - HasKeyItem() solo verifica presencia

✅ Sistema MODIFICADO (con conteo):
  - TryAddItem() incrementa contador
  - ConsumeKeyItem() decrementa contador
  - Puerta verifica: keyCount > 0
```

**Implementación necesaria:**

```csharp
// InventorySystem.cs - NUEVA funcionalidad
private Dictionary<string, int> keyCounters = new Dictionary<string, int>();

public bool HasKeyCount(string keyType, int required = 1)
{
    if (keyCounters.ContainsKey(keyType))
    {
        return keyCounters[keyType] >= required;
    }
    return false;
}

public void AddKeyCount(string keyType, int amount = 1)
{
    if (!keyCounters.ContainsKey(keyType))
    {
        keyCounters[keyType] = 0;
    }
    keyCounters[keyType] += amount;
}

public bool ConsumeKeyCount(string keyType, int amount = 1)
{
    if (!keyCounters.ContainsKey(keyType) || keyCounters[keyType] < amount)
    {
        return false;
    }
    
    keyCounters[keyType] -= amount;
    return true;
}
```

**Uso:**
```
SmallKeyData (mismo SO reutilizado)
  - keyType: "small_key"

Mundo:
  - SmallKey_1 → añade contador +1
  - SmallKey_2 → añade contador +1
  - SmallKey_3 → añade contador +1

Puertas:
  - Door_A → consume contador -1
  - Door_B → consume contador -1
  - Door_C → consume contador -1

Resultado:
  - Recoges 3 llaves (contador = 3)
  - Abres 3 puertas (consume 3 del contador)
  - 1 SO reutilizado ✓
```

---

## 📊 Comparación de Estrategias

| Caso | Comportamiento | SOs Necesarios | Sistema |
|------|----------------|----------------|---------|
| **Llave Individual** | 1 llave → 1 puerta específica | Muchos (1 por llave) | Actual ✓ |
| **Master Key** | 1 llave → N puertas del mismo tipo | Pocos (1 SO, múltiples IDs) | Actual ✓ |
| **Conteo (Zelda)** | N llaves → N puertas genéricas | Muy pocos (1 SO) | Requiere modificación |

---

## 🎯 Respuesta Directa a Tu Pregunta

> "¿Cómo las diferentes llaves comunes entienden el ID? ¿No podrían abrir tanto cofre como habitaciones?"

**Respuesta:** Sí, tienes razón. Con el sistema actual basado en IDs:

### ❌ SI reutilizas un SO con 1 ID:

```
KeyItemData: "CommonKeyData"
  - Unlocks: ["locked"]

Resultado:
  - 1 llave abre TODAS las puertas con ID "locked"
  - Cofre, habitación, puerta trasera, etc.
  - NO ES LO QUE QUIERES para llaves individuales
```

### ✅ SI quieres llaves individuales:

```
Opción A: IDs únicos (sistema actual)
  - KeyItemData: "Key_Chest" → Unlocks: ["chest_1"]
  - KeyItemData: "Key_Room" → Unlocks: ["room_1"]
  - Necesitas 1 SO por llave

Opción B: Sistema de conteo (modificación)
  - KeyItemData: "SmallKey" → keyType: "small_key"
  - Contador: recoges 5, usas 5
  - 1 SO reutilizado
```

---

## 💡 Qué Sistema Usar Según Tu Juego

### 🏰 Resident Evil / Horror (Llaves con Formas)

```
Llave Corazón → Solo abre Puerta Corazón
Llave Diamante → Solo abre Puerta Diamante
Llave Trébol → Solo abre Puerta Trébol

Sistema: IDs únicos
SOs: 1 por llave (3 total)

KeyItemData: "HeartKeyData"
  - Unlocks: ["heart_door"]
  
KeyItemData: "DiamondKeyData"
  - Unlocks: ["diamond_door"]
```

**Código actual funciona perfectamente ✓**

---

### 🗝️ Zelda / Dungeon Crawler (Small Keys)

```
Small Key 1 → Puede abrir cualquier puerta
Small Key 2 → Puede abrir cualquier puerta
Small Key 3 → Puede abrir cualquier puerta

Sistema: Conteo
SOs: 1 reutilizado

KeyItemData: "SmallKeyData"
  - keyType: "small_key"
  - NO usa Unlocks[] (usa contador)
```

**Requiere modificación del sistema (sistema de conteo)**

---

### 🎨 Metroidvania (Keycards de Colores)

```
Red Keycard → Abre todas las puertas rojas
Blue Keycard → Abre todas las puertas azules
Green Keycard → Abre todas las puertas verdes

Sistema: IDs compartidos (master keys)
SOs: 1 por color (3 total)

KeyItemData: "RedKeycardData"
  - Unlocks: ["red_door"]
  
(Todas las puertas rojas usan requiredKeyID: "red_door")
```

**Código actual funciona perfectamente ✓**

---

### 🏨 Múltiples Habitaciones Individuales

```
Llave Habitación 101 → Solo abre Habitación 101
Llave Habitación 102 → Solo abre Habitación 102
Llave Habitación 103 → Solo abre Habitación 103

Sistema: IDs únicos
SOs: 1 por habitación (100+ para un hotel)

KeyItemData: "Key_Room101"
  - Unlocks: ["room_101"]
  
KeyItemData: "Key_Room102"
  - Unlocks: ["room_102"]
```

**Código actual funciona, pero necesitas MUCHOS SOs**

**Alternativa:** Sistema de conteo con tipos
```
KeyItemData: "HotelKeyData"
  - keyType: "hotel_key"
  
LockedDoorInteractable modificado:
  - roomNumber: 101
  - Verifica: ¿tienes hotel_key? + ¿es habitación correcta?
```

---

## 🛠️ Modificación para Sistema de Conteo

Si quieres implementar el estilo Zelda (muchas llaves genéricas), aquí está la modificación:

### 1. Modificar KeyItemData

```csharp
[CreateAssetMenu(fileName = "New Key Item", menuName = "Inventory/Key Item")]
public class KeyItemData : ItemData
{
    [Header("Key Item Settings")]
    [SerializeField] private string[] unlocks;  // Para IDs específicos
    [SerializeField] private string keyType;    // NUEVO: Para conteo
    [SerializeField] private bool useCountSystem = false;  // NUEVO
    [SerializeField] private bool isQuestItem;
    [SerializeField] private bool canBeDiscarded = false;

    public string[] Unlocks => unlocks;
    public string KeyType => keyType;  // NUEVO
    public bool UseCountSystem => useCountSystem;  // NUEVO
    public bool IsQuestItem => isQuestItem;
    public bool CanBeDiscarded => canBeDiscarded;
}
```

### 2. Modificar InventorySystem

```csharp
// AÑADIR al principio de la clase
private Dictionary<string, int> keyCounters = new Dictionary<string, int>();

// AÑADIR al final de la clase
public int GetKeyCount(string keyType)
{
    if (keyCounters.ContainsKey(keyType))
    {
        return keyCounters[keyType];
    }
    return 0;
}

public void AddKeyCount(string keyType, int amount = 1)
{
    if (!keyCounters.ContainsKey(keyType))
    {
        keyCounters[keyType] = 0;
    }
    
    keyCounters[keyType] += amount;
    Debug.Log($"<color=green>[INVENTORY] {keyType} count: {keyCounters[keyType]}</color>");
}

public bool ConsumeKeyCount(string keyType, int amount = 1)
{
    if (!keyCounters.ContainsKey(keyType) || keyCounters[keyType] < amount)
    {
        Debug.Log($"<color=red>[INVENTORY] Not enough {keyType} (need {amount}, have {GetKeyCount(keyType)})</color>");
        return false;
    }
    
    keyCounters[keyType] -= amount;
    Debug.Log($"<color=yellow>[INVENTORY] Used {amount} {keyType}. Remaining: {keyCounters[keyType]}</color>");
    return true;
}
```

### 3. Modificar PickupInteractable

```csharp
bool AddToInventory(GameObject interactor)
{
    Inventory.InventorySystem inventory = interactor.GetComponent<Inventory.InventorySystem>();
    
    if (inventory == null)
    {
        Debug.LogError($"<color=red>[PICKUP] {interactor.name} has no InventorySystem component!</color>");
        return false;
    }
    
    if (itemData == null)
    {
        Debug.LogError($"<color=red>[PICKUP] {gameObject.name} has no ItemData assigned!</color>");
        return false;
    }
    
    // NUEVO: Si es KeyItem con sistema de conteo
    if (itemData is Inventory.KeyItemData keyData && keyData.UseCountSystem)
    {
        inventory.AddKeyCount(keyData.KeyType);
        Debug.Log($"<color=green>[PICKUP] {interactor.name} picked up {itemName} (count)</color>");
        return true;
    }
    
    // Sistema normal (actual)
    bool added = inventory.TryAddItem(itemData);
    
    if (added)
    {
        Debug.Log($"<color=green>[PICKUP] {interactor.name} picked up {itemName}</color>");
    }
    else
    {
        Debug.Log($"<color=yellow>[PICKUP] Could not add {itemName} to inventory (full?)</color>");
    }
    
    return added;
}
```

### 4. Crear LockedDoorInteractable con Conteo

```csharp
[Header("Key Type")]
[SerializeField] private bool useKeyCount = false;
[SerializeField] private string requiredKeyType = "small_key";
[SerializeField] private int keysRequired = 1;

bool HasRequiredKey(GameObject interactor)
{
    Inventory.InventorySystem inventory = interactor.GetComponent<Inventory.InventorySystem>();
    
    if (inventory == null)
    {
        return false;
    }
    
    if (useKeyCount)
    {
        return inventory.GetKeyCount(requiredKeyType) >= keysRequired;
    }
    else
    {
        return inventory.HasKeyItem(requiredKeyID);
    }
}

protected override void OnInteract(GameObject interactor)
{
    // ... código existente ...
    
    if (useKeyCount)
    {
        inventory.ConsumeKeyCount(requiredKeyType, keysRequired);
    }
    else if (consumeKeyOnUnlock)
    {
        inventory.ConsumeKeyItem(requiredKeyID);
    }
    
    // ... código existente ...
}
```

---

## 🎯 Conclusión Final

### Para tu pregunta específica:

**❌ NO puedes reutilizar 1 SO para llaves individuales** con el sistema actual basado en IDs.

**✅ Opciones que SÍ funcionan:**

1. **Llaves Únicas (Resident Evil):**
   - 1 SO por llave
   - Cada SO con ID único
   - Perfecto para pocas llaves especiales

2. **Master Keys (Metroidvania):**
   - 1 SO reutilizado
   - Múltiples puertas comparten ID
   - Perfecto para keycards de colores

3. **Sistema de Conteo (Zelda):**
   - 1 SO reutilizado
   - Requiere modificación del código
   - Perfecto para muchas llaves genéricas

---

¿Cuál de estos 3 casos se ajusta más a lo que necesitas para tu juego? Te ayudo a implementar la solución específica. 🔑✨
