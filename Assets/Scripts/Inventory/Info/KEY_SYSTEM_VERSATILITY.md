# 🔑 Versatilidad del Sistema de Llaves

Guía completa sobre las diferentes estrategias para configurar llaves y puertas según tus necesidades.

---

## 📊 Resumen Rápido

```
┌─────────────────────────────────────────────────────────────┐
│ ¿Necesito un SO diferente por cada llave física?           │
│                                                             │
│ RESPUESTA: NO necesariamente                               │
│                                                             │
│ Tienes 3 estrategias principales:                          │
│   1. Una llave = Un SO (más flexible)                      │
│   2. Reutilizar SOs para llaves idénticas (más eficiente)  │
│   3. Master Keys con múltiples IDs (híbrido)               │
└─────────────────────────────────────────────────────────────┘
```

---

## 🎨 Estrategia 1: Una Llave Física = Un SO

**Cuándo usarla:** Cada llave tiene propiedades únicas (nombre, icono, descripción)

### Ejemplo: RPG con Llaves Únicas

```
KeyItemData: "RustyKeyData"
  - Item Name: "Rusty Key"
  - Item Icon: rusty_key_icon 🔑
  - Unlocks: ["basement_door"]
  
KeyItemData: "GoldenKeyData"
  - Item Name: "Golden Key"
  - Item Icon: golden_key_icon 🗝️
  - Unlocks: ["treasure_room"]
  
KeyItemData: "SilverKeyData"
  - Item Name: "Silver Key"
  - Item Icon: silver_key_icon 🔐
  - Unlocks: ["armory"]

Resultado:
  - 3 llaves físicas en el mundo
  - 3 ScriptableObjects
  - 3 iconos diferentes en inventario
  - 3 descripciones únicas
```

### Ventajas
- ✅ Cada llave es única visualmente
- ✅ Diferentes descripciones para storytelling
- ✅ Fácil de rastrear en inventario
- ✅ Mejor para juegos con pocas llaves especiales

### Desventajas
- ❌ Más ScriptableObjects que gestionar
- ❌ Más trabajo inicial de configuración

---

## 🔄 Estrategia 2: Reutilizar SOs para Llaves Idénticas

**Cuándo usarla:** Múltiples llaves físicas que hacen lo mismo (ej: llaves de habitaciones de hotel)

### Ejemplo: Llaves de Habitaciones de Hotel

```
KeyItemData: "HotelRoomKeyData" (UN SOLO SO)
  - Item Name: "Hotel Room Key"
  - Item Icon: generic_key_icon 🔑
  - Unlocks: ["hotel_room"]

Mundo:
  Habitación 101 - Llave en mesa
    └─ PickupInteractable → itemData: HotelRoomKeyData
  
  Habitación 102 - Llave en armario
    └─ PickupInteractable → itemData: HotelRoomKeyData
  
  Habitación 103 - Llave en cajón
    └─ PickupInteractable → itemData: HotelRoomKeyData

Puertas:
  Puerta 101
    └─ LockedDoorInteractable → requiredKeyID: "hotel_room"
  
  Puerta 102
    └─ LockedDoorInteractable → requiredKeyID: "hotel_room"
  
  Puerta 103
    └─ LockedDoorInteractable → requiredKeyID: "hotel_room"

Resultado:
  - 3 llaves físicas en el mundo
  - 1 ScriptableObject (reutilizado)
  - Todas tienen el mismo nombre/icono
  - Todas abren puertas con requiredKeyID="hotel_room"
  - Solo necesitas 1 llave para abrir TODAS las puertas
```

### Ventajas
- ✅ Menos ScriptableObjects que crear
- ✅ Perfecto para llaves genéricas
- ✅ Fácil de configurar en masa

### Desventajas
- ❌ Todas las llaves se ven iguales en inventario
- ❌ Menos distintivo narrativamente
- ❌ Una llave abre TODAS las puertas con ese ID

---

## 🎭 Estrategia 3: Master Keys (Una Llave → Múltiples Puertas)

**Cuándo usarla:** Llaves especiales que abren varias puertas diferentes

### Ejemplo: Master Key del Conserje

```
KeyItemData: "MasterKeyData"
  - Item Name: "Master Key"
  - Item Icon: master_key_icon 🗝️✨
  - Unlocks: [
      "basement_door",
      "storage_room",
      "office_door",
      "rooftop_access"
    ]

Puertas:
  Basement Door
    └─ LockedDoorInteractable → requiredKeyID: "basement_door"
  
  Storage Room
    └─ LockedDoorInteractable → requiredKeyID: "storage_room"
  
  Office Door
    └─ LockedDoorInteractable → requiredKeyID: "office_door"
  
  Rooftop Access
    └─ LockedDoorInteractable → requiredKeyID: "rooftop_access"

Resultado:
  - 1 llave física
  - 1 ScriptableObject
  - Abre 4 puertas diferentes
  - Cada puerta tiene su propio ID único
```

### Ventajas
- ✅ Poderosa mecánica de juego (llave especial)
- ✅ Recompensa valiosa para el jugador
- ✅ Flexibilidad total (añade IDs al array)

### Desventajas
- ❌ Puede romper puzzles si se obtiene muy pronto
- ❌ Menos desafío si abre demasiadas puertas

---

## 🔀 Estrategia 4: Llaves Específicas + Llaves Compartidas

**Cuándo usarla:** Combinar unicidad con reutilización

### Ejemplo: Juego de Aventuras

```
LLAVES ÚNICAS (1 SO cada una):

KeyItemData: "BossKeyData"
  - Unlocks: ["boss_room"]
  
KeyItemData: "TreasureKeyData"
  - Unlocks: ["treasure_vault"]

LLAVES GENÉRICAS (reutilizar SOs):

KeyItemData: "SmallKeyData" (reutilizado x5)
  - Unlocks: ["locked_door"]
  
  Mundo:
    - SmallKey_1 → SmallKeyData
    - SmallKey_2 → SmallKeyData
    - SmallKey_3 → SmallKeyData
    - SmallKey_4 → SmallKeyData
    - SmallKey_5 → SmallKeyData
  
  Puertas:
    - LockedDoor_1 → requiredKeyID: "locked_door"
    - LockedDoor_2 → requiredKeyID: "locked_door"
    - LockedDoor_3 → requiredKeyID: "locked_door"

Resultado:
  - 2 llaves únicas (boss, treasure)
  - 5 llaves pequeñas genéricas
  - Solo 3 ScriptableObjects total
```

---

## 🎯 Comparación de Estrategias

| Estrategia | SOs Necesarios | Llaves Físicas | Flexibilidad | Complejidad |
|------------|----------------|----------------|--------------|-------------|
| 1 Llave = 1 SO | Muchos | Muchas | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ |
| Reutilizar SOs | Pocos | Muchas | ⭐⭐ | ⭐ |
| Master Keys | Muy pocos | Pocas | ⭐⭐⭐⭐ | ⭐⭐ |
| Híbrido | Moderado | Muchas | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ |

---

## 🛠️ Ejemplos Prácticos

### Caso 1: Resident Evil Style (Llaves con Forma)

```
SOs:
  - SpadeKeyData → Unlocks: ["spade_door"]
  - HeartKeyData → Unlocks: ["heart_door"]
  - DiamondKeyData → Unlocks: ["diamond_door"]
  - ClubKeyData → Unlocks: ["club_door"]

Características:
  - Cada llave tiene forma única
  - Cada puerta requiere su llave específica
  - No se reutilizan SOs
  - 4 llaves = 4 SOs
```

### Caso 2: Zelda Style (Small Keys + Boss Key)

```
SOs:
  - SmallKeyData → Unlocks: ["locked_door"] (reutilizado)
  - BossKeyData → Unlocks: ["boss_door"] (único)

Características:
  - Small Keys se reutilizan en todo el dungeon
  - Boss Key es única
  - 10 llaves físicas = 2 SOs
```

### Caso 3: Metroidvania Style (Colored Keycards)

```
SOs:
  - RedKeycardData → Unlocks: ["red_door_1", "red_door_2", "red_door_3"]
  - BlueKeycardData → Unlocks: ["blue_door_1", "blue_door_2"]
  - GreenKeycardData → Unlocks: ["green_door_1", "green_door_2", "green_door_3", "green_door_4"]

Características:
  - Cada keycard abre múltiples puertas del mismo color
  - 1 keycard física de cada color en el mundo
  - 3 keycards = 3 SOs
  - Cada SO tiene múltiples IDs en Unlocks[]
```

### Caso 4: Horror Game (Numbered Keys)

```
SOs:
  - Key_Room101Data → Unlocks: ["room_101"]
  - Key_Room102Data → Unlocks: ["room_102"]
  - Key_Room103Data → Unlocks: ["room_103"]
  - MasterKeyData → Unlocks: ["room_101", "room_102", "room_103", "basement"]

Características:
  - Cada habitación tiene su llave numerada
  - Master Key abre todo
  - 4 llaves = 4 SOs
```

---

## 💡 Recomendaciones por Tipo de Juego

### 🎮 RPG / Adventure
```
Estrategia: 1 Llave = 1 SO

Razón:
  - Cada llave tiene historia
  - Nombres únicos importantes
  - Descripciones narrativas
  - Iconos distintivos
  - Quest items

Ejemplo:
  - "Ancient Temple Key" (naranja)
  - "Royal Treasury Key" (dorada)
  - "Crypt Key" (oscura)
```

### 🏚️ Horror / Survival
```
Estrategia: Híbrido (Únicas + Genéricas)

Razón:
  - Llaves especiales para áreas importantes
  - Llaves genéricas para habitaciones comunes
  - Balance entre tensión y progresión

Ejemplo:
  - "Rusty Key" x5 (genérica, reutilizada)
  - "Blood-Stained Key" (única, boss)
  - "Master Key" (única, final)
```

### 🗝️ Metroidvania
```
Estrategia: Master Keys (Colored/Tiered)

Razón:
  - Progresión por áreas de color
  - Una llave abre múltiples puertas
  - Gates de progresión claros

Ejemplo:
  - Red Keycard → 5 puertas rojas
  - Blue Keycard → 8 puertas azules
  - Green Keycard → 3 puertas verdes
```

### 🏰 Puzzle / Dungeon Crawler
```
Estrategia: Reutilizar SOs

Razón:
  - Muchas llaves pequeñas
  - Conteo más importante que individualidad
  - Simplicidad de gestión

Ejemplo:
  - "Small Key" x10 (mismo SO)
  - "Boss Key" x1 (SO único)
```

---

## 🔧 Configuración Avanzada

### Escenario: Múltiples Llaves para UNA Puerta

**Problema:** Quieres que una puerta requiera VARIAS llaves diferentes

**Solución Actual:** No soportado directamente

**Solución Modificada:** Extender `LockedDoorInteractable`

```csharp
// LockedDoorInteractable.cs
[Header("Door Settings")]
[SerializeField] private string[] requiredKeyIDs = { "red_key", "blue_key", "green_key" };
[SerializeField] private bool requiresAllKeys = true;

bool HasRequiredKeys(GameObject interactor)
{
    InventorySystem inventory = interactor.GetComponent<InventorySystem>();
    
    if (inventory == null) return false;
    
    if (requiresAllKeys)
    {
        // Necesita TODAS las llaves
        foreach (string keyID in requiredKeyIDs)
        {
            if (!inventory.HasKeyItem(keyID))
            {
                return false;
            }
        }
        return true;
    }
    else
    {
        // Necesita AL MENOS UNA
        foreach (string keyID in requiredKeyIDs)
        {
            if (inventory.HasKeyItem(keyID))
            {
                return true;
            }
        }
        return false;
    }
}
```

**Uso:**
```
Puerta Final:
  - Required Key IDs: ["fragment_1", "fragment_2", "fragment_3"]
  - Requires All Keys: ✓ true

Resultado: Necesitas las 3 llaves para abrir
```

---

### Escenario: Llave que se Consume vs Permanece

**Configuración en LockedDoorInteractable:**

```csharp
[SerializeField] private bool consumeKeyOnUnlock = true;
```

**Uso:**

```
LLAVE DE UN SOLO USO (Resident Evil style):
  consumeKeyOnUnlock = true
  Resultado: Llave desaparece al abrir puerta

LLAVE PERMANENTE (Master Key):
  consumeKeyOnUnlock = false
  Resultado: Llave permanece, puede abrir múltiples puertas
```

---

### Escenario: Llave Única para Múltiples Instancias

**Problema:** Tienes 3 puertas idénticas, quieres que 1 llave abra solo 1 de ellas

**Solución:** Usar IDs únicos por instancia

```
KeyItemData: "RedKeyData"
  - Unlocks: ["red_door_instance_1"]

Puertas:
  Red Door A
    └─ LockedDoorInteractable → requiredKeyID: "red_door_instance_1"
  
  Red Door B
    └─ LockedDoorInteractable → requiredKeyID: "red_door_instance_2"
  
  Red Door C
    └─ LockedDoorInteractable → requiredKeyID: "red_door_instance_3"

Resultado:
  - 1 llave solo abre 1 puerta específica
  - Aunque visualmente sean idénticas
```

---

## 📊 Flujo de Decisión: ¿Qué Estrategia Usar?

```
START
  │
  ├─ ¿Cada llave es narrativamente única?
  │    │
  │    ├─ SÍ → Estrategia 1 (1 Llave = 1 SO)
  │    │         ej: RPG, Adventure
  │    │
  │    └─ NO
  │         │
  │         ├─ ¿Tienes muchas llaves idénticas?
  │         │    │
  │         │    ├─ SÍ → Estrategia 2 (Reutilizar SOs)
  │         │    │         ej: Dungeon Crawler, Zelda
  │         │    │
  │         │    └─ NO
  │         │         │
  │         │         ├─ ¿Una llave abre múltiples puertas?
  │         │         │    │
  │         │         │    ├─ SÍ → Estrategia 3 (Master Keys)
  │         │         │    │         ej: Metroidvania, Keycards
  │         │         │    │
  │         │         │    └─ NO → Estrategia 4 (Híbrido)
  │         │         │              ej: Horror, Mixed
```

---

## 🎯 Respuesta Directa a tu Pregunta

**¿Para cada llave tengo que crear un SO KeyItem diferente?**

**RESPUESTA CORTA:** No necesariamente

**RESPUESTA LARGA:**

1. **SI quieres llaves VISUALMENTE diferentes:**
   - ✅ Sí, crea un SO por llave (nombre, icono, descripción únicos)
   
2. **SI las llaves son FUNCIONALMENTE idénticas:**
   - ❌ No, reutiliza el mismo SO en múltiples GameObjects
   
3. **SI quieres una MASTER KEY:**
   - ⭐ Crea 1 SO con múltiples IDs en el array `Unlocks[]`

**EJEMPLO PRÁCTICO:**

```
Tu juego tiene:
  - 10 "Small Keys" genéricas → 1 SO reutilizado
  - 1 "Boss Key" especial → 1 SO único
  - 1 "Master Key" final → 1 SO único con múltiples IDs

Total: 12 llaves físicas = 3 ScriptableObjects
```

---

## 🚀 Recomendación Final

Para la mayoría de juegos, usa **Estrategia 4 (Híbrido)**:

```
Llaves Especiales:
  ✅ 1 SO por llave (únicas, importantes, story-driven)
  
Llaves Genéricas:
  ✅ Reutilizar SOs (comunes, abundantes, funcionales)
  
Master Keys:
  ✅ 1 SO con array de IDs (poderosas, late-game)
```

**Ventajas:**
- Balance entre flexibilidad y simplicidad
- Menos SOs que gestionar
- Mantiene la narrativa de llaves especiales
- Escalable para juegos grandes

---

¿Necesitas ayuda implementando alguna estrategia específica o tienes dudas sobre cuál usar para tu juego? 🔑✨
