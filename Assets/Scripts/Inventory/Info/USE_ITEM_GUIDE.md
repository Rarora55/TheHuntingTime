# 🔥 Guía: Use Item (Usar Items)

## ✅ Estado: Sistema Completamente Implementado

La acción **"Use Item"** ya está **completamente funcional** en tu inventario. Esta guía explica cómo funciona y cómo crear items usables.

---

## 🎮 Cómo Usar Items en el Juego

### Flujo de Usuario

```
1. Recoge un item consumible (ej. Health Potion)
2. Abre inventario → Tab
3. Navega al item → Arrow Left/Right
4. Abre menú contextual → E
5. Selecciona "Use" → (aparece solo si es usable)
6. Confirma → E
7. El item se consume y aplica su efecto
```

### Condiciones para que Aparezca "Use"

La opción **"Use"** solo aparece en el menú contextual si:

1. ✅ El item implementa la interfaz `IUsable`
2. ✅ El método `CanUse()` retorna `true`

**Ejemplo:** Un `ConsumableItemData` (health potion) solo muestra "Use" si tu salud **no está llena**.

---

## 📋 Items Usables Actuales

### ConsumableItemData (Consumibles de Curación)

**Ubicación:** `/Assets/Scripts/Inventory/Data/ConsumableItemData.cs`

**Qué hace:**
- Cura al jugador por una cantidad específica de HP
- Se elimina del inventario después de usarse (opcional)
- Reproduce sonido y efecto visual (opcional)

**Propiedades Configurables:**

```
Heal Amount: 50         (cantidad de HP que cura)
Remove On Use: ✓        (eliminar item después de usar)
Use Sound: AudioClip    (sonido al usar)
Use Effect: GameObject  (efecto visual al usar)
```

**Ejemplo de Item Existente:**
- **Nombre:** Health Recover
- **Cura:** 50 HP
- **Se elimina:** Sí
- **Ubicación:** `/Assets/Assets/Data/Items/TestHeltth1.asset`

---

## 🛠️ Crear un Nuevo Item Usable

### Opción 1: Crear un Consumible de Curación

**Paso 1:** Crear el Asset

```
1. En Project, click derecho en /Assets/Assets/Data/Items
2. Create → Inventory → Consumable Item
3. Nombra el asset (ej. "MedKit")
```

**Paso 2:** Configurar Propiedades

Selecciona el asset creado y configura en Inspector:

```
ITEM DATA:
  Item Name: MedKit
  Item ID: (se genera automáticamente)
  Description: Restores 100 HP
  Item Icon: (arrastra tu sprite)
  Item Detail Image: (imagen para UI detallada)
  Item Type: Consumable
  Stackable: ✓
  Can Be Examined: ✓
  Examination Text: A powerful medical kit that fully heals you

CONSUMABLE SETTINGS:
  Heal Amount: 100
  Remove On Use: ✓
  Use Sound: (opcional, sonido de curación)
  Use Effect: (opcional, partículas de curación)
```

**Paso 3:** Probar

```
1. Coloca el item en la escena con ItemPickup component
2. Play → Recoge el item
3. Tab → E (sobre el item) → Selecciona "Use" → E
4. Verifica que cure 100 HP
```

---

### Opción 2: Crear un Tipo de Item Usable Personalizado

Si quieres crear items con efectos diferentes (ej. aumentar stamina, dar velocidad temporal, etc.), necesitas crear una nueva clase que implemente `IUsable`.

**Ejemplo: Item de Stamina**

```csharp
using UnityEngine;

namespace TheHunt.Inventory
{
    [CreateAssetMenu(fileName = "New Stamina Item", menuName = "Inventory/Stamina Item")]
    public class StaminaItemData : ItemData, IUsable
    {
        [Header("Stamina Settings")]
        [SerializeField] private float staminaAmount = 50f;
        [SerializeField] private bool removeOnUse = true;

        public bool RemoveOnUse => removeOnUse;

        public bool CanUse(GameObject user)
        {
            StaminaController stamina = user.GetComponent<StaminaController>();
            if (stamina == null)
                return false;

            return stamina.CurrentStamina < stamina.MaxStamina;
        }

        public void Use(GameObject user)
        {
            if (!CanUse(user))
            {
                Debug.Log($"<color=yellow>[INVENTORY] Cannot use {ItemName} - Stamina is full</color>");
                return;
            }

            StaminaController stamina = user.GetComponent<StaminaController>();
            if (stamina != null)
            {
                stamina.RestoreStamina(staminaAmount);
                Debug.Log($"<color=green>[INVENTORY] Used {ItemName} - Restored {staminaAmount} Stamina</color>");
            }
        }
    }
}
```

**Ejemplo: Item de Buff Temporal**

```csharp
using UnityEngine;

namespace TheHunt.Inventory
{
    [CreateAssetMenu(fileName = "New Buff Item", menuName = "Inventory/Buff Item")]
    public class BuffItemData : ItemData, IUsable
    {
        [Header("Buff Settings")]
        [SerializeField] private float speedMultiplier = 1.5f;
        [SerializeField] private float duration = 10f;
        [SerializeField] private bool removeOnUse = true;

        public bool RemoveOnUse => removeOnUse;

        public bool CanUse(GameObject user)
        {
            return true;
        }

        public void Use(GameObject user)
        {
            PlayerMovementController movement = user.GetComponent<PlayerMovementController>();
            if (movement != null)
            {
                movement.ApplySpeedBuff(speedMultiplier, duration);
                Debug.Log($"<color=green>[INVENTORY] Used {ItemName} - Speed x{speedMultiplier} for {duration}s</color>");
            }
        }
    }
}
```

**Ejemplo: Item de Munición**

```csharp
using UnityEngine;

namespace TheHunt.Inventory
{
    [CreateAssetMenu(fileName = "New Ammo Box", menuName = "Inventory/Ammo Item")]
    public class AmmoItemData : ItemData, IUsable
    {
        [Header("Ammo Settings")]
        [SerializeField] private AmmoType ammoType;
        [SerializeField] private int ammoAmount = 30;
        [SerializeField] private bool removeOnUse = true;

        public bool RemoveOnUse => removeOnUse;

        public bool CanUse(GameObject user)
        {
            return true;
        }

        public void Use(GameObject user)
        {
            InventorySystem inventory = user.GetComponent<InventorySystem>();
            if (inventory != null)
            {
                inventory.AddAmmo(ammoType, ammoAmount);
                Debug.Log($"<color=green>[INVENTORY] Used {ItemName} - Added {ammoAmount} {ammoType} ammo</color>");
            }
        }
    }
}
```

---

## 🔍 Cómo Funciona Internamente

### 1. Detección de Items Usables

Cuando abres el menú contextual, el sistema verifica:

```csharp
// En InventoryUIController.OpenContextMenu()
if (currentItem.itemData is IUsable usable)
{
    if (usable.CanUse(gameObject))
    {
        availableActions.Add(ItemContextAction.Use);
    }
}
```

### 2. Ejecución de la Acción

Cuando seleccionas "Use" y presionas E:

```csharp
// En InventoryUIController.ExecuteContextAction()
case ItemContextAction.Use:
    inventorySystem.UseCurrentItem();
    CloseContextMenu();
    break;
```

### 3. Uso del Item

El `InventorySystem` delega al item:

```csharp
// En InventorySystem.UseCurrentItem()
public void UseCurrentItem()
{
    ItemInstance item = slots[selectedIndex];
    if (item != null && item.itemData is IUsable usable)
    {
        usable.Use(gameObject);
        
        if (usable.RemoveOnUse)
        {
            RemoveItem(item.itemData, 1);
        }
    }
}
```

### 4. Efecto del Item

El item aplica su efecto específico:

```csharp
// En ConsumableItemData.Use()
public override void Use(GameObject user)
{
    HealthController health = user.GetComponent<HealthController>();
    if (health != null)
    {
        health.Heal(healAmount);
        // Sonido y efectos visuales...
    }
}
```

---

## 🎨 Interfaz IUsable

### Definición

```csharp
public interface IUsable
{
    bool CanUse(GameObject user);
    void Use(GameObject user);
    bool RemoveOnUse { get; }
}
```

### Métodos

#### `bool CanUse(GameObject user)`

Determina si el item **puede** ser usado en este momento.

**Ejemplos:**
- Health Potion → Retorna `true` solo si salud < max salud
- Ammo Box → Siempre retorna `true`
- Key → Retorna `true` solo si estás cerca de una puerta cerrada

#### `void Use(GameObject user)`

Ejecuta el efecto del item.

**Parámetro:**
- `user` → El GameObject que usa el item (normalmente el Player)

**Responsabilidades:**
- Aplicar el efecto (curar, buff, abrir puerta, etc.)
- Reproducir sonido/efectos visuales
- Loggear información de debug

#### `bool RemoveOnUse { get; }`

Indica si el item debe eliminarse del inventario después de usarse.

**Ejemplos:**
- Health Potion → `true` (se consume)
- Key → `false` (se queda en el inventario)
- Torch → `false` (se puede usar múltiples veces)

---

## 🐛 Solución de Problemas

### "Use" no aparece en el menú

**Causas posibles:**

1. **El item no implementa IUsable**
   - Solución: Asegúrate de que el ScriptableObject del item implementa `IUsable`
   - Ejemplo: `ConsumableItemData : ItemData, IUsable`

2. **CanUse() retorna false**
   - Solución: Verifica la lógica en `CanUse()`
   - Ejemplo: Si es health potion, asegúrate de que tu salud no esté llena

3. **El item es de otro tipo**
   - Solución: Solo items que implementan `IUsable` muestran "Use"
   - Ejemplo: `WeaponItemData` no implementa `IUsable`, usa "Equip" en su lugar

### El item se usa pero no tiene efecto

**Causas posibles:**

1. **El método Use() está vacío**
   - Solución: Implementa la lógica en `Use()`

2. **El user no tiene el componente necesario**
   - Solución: Verifica que el Player tenga `HealthController`, `StaminaController`, etc.
   - Ejemplo: `user.GetComponent<HealthController>()` retorna `null`

3. **Los valores están en 0**
   - Solución: Verifica que `healAmount`, `staminaAmount`, etc. sean > 0

### El item no se elimina después de usarse

**Causas posibles:**

1. **RemoveOnUse está en false**
   - Solución: En Inspector del item → `Remove On Use: ✓`

2. **InventorySystem no elimina el item**
   - Solución: Verifica que `UseCurrentItem()` llame a `RemoveItem()`

---

## 📊 Tipos de Items Recomendados

### ✅ Items de Curación (Implementados)

```
ConsumableItemData
  → Health Potion (50 HP)
  → MedKit (100 HP)
  → Bandage (25 HP + regeneración)
```

### 🔮 Items de Buff (Por Implementar)

```
BuffItemData
  → Speed Boost (+50% velocidad, 10s)
  → Damage Boost (+30% daño, 15s)
  → Invisibility (invisible a enemigos, 5s)
```

### ⚡ Items de Stamina (Por Implementar)

```
StaminaItemData
  → Energy Drink (+50 stamina)
  → Coffee (+100 stamina + boost temporal)
```

### 🔑 Items Especiales (Por Implementar)

```
KeyItemData
  → Door Key (abre puerta específica)
  → Torch (ilumina área oscura)
  → Map (revela mapa)
```

---

## ✅ Checklist de Implementación

Para crear un nuevo tipo de item usable:

- [ ] Crear clase que herede de `ItemData` e implemente `IUsable`
- [ ] Implementar `CanUse()` con la lógica de validación
- [ ] Implementar `Use()` con el efecto del item
- [ ] Configurar `RemoveOnUse` (propiedad)
- [ ] Añadir `[CreateAssetMenu]` para crear assets
- [ ] Crear asset de prueba en `/Assets/Assets/Data/Items`
- [ ] Probar en Play mode: recoger → usar → verificar efecto
- [ ] Verificar que se elimine si `RemoveOnUse = true`

---

## 🎯 Resultado Actual

**Sistema Completo:**
✅ Interfaz `IUsable` definida  
✅ `ConsumableItemData` implementado (curación)  
✅ Menú contextual muestra "Use" dinámicamente  
✅ Ejecución de acción "Use" funcional  
✅ Eliminación automática de items consumibles  
✅ Sistema extensible para nuevos tipos de items  

**Item de Prueba Existente:**
✅ `Health Recover` (50 HP) en `/Assets/Assets/Data/Items/TestHeltth1.asset`

---

## 🚀 Próximos Pasos Sugeridos

1. **Crear más consumibles:**
   - MedKit (100 HP, caro, raro)
   - Bandage (25 HP, barato, común)
   - Emergency Kit (full HP, muy raro)

2. **Implementar items de Stamina:**
   - Energy Drink → `StaminaItemData`
   - Requiere `StaminaController` en el Player

3. **Añadir efectos visuales/sonoros:**
   - Asignar `Use Sound` a los consumibles
   - Crear `Use Effect` (partículas de curación)

4. **Crear items especiales:**
   - Keys → `KeyItemData` (abrir puertas)
   - Torches → `ToolItemData` (iluminación)
   - Maps → `QuestItemData` (progreso de historia)

---

¡El sistema de "Use Item" está listo para usar! 🎮✨
