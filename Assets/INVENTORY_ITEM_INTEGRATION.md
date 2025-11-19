# 🎒 SISTEMA DE ITEMS - INTEGRACIÓN CON INVENTARIO

## 🎯 Arquitectura Corregida

El sistema de items ahora está diseñado para trabajar **CON un sistema de inventario**, en lugar de auto-curarse al tocar.

---

## 🏗️ Flujo Correcto

### **❌ ANTES (Incorrecto - Auto-healing)**
```
Player toca item → Item cura inmediatamente → Item desaparece
```

### **✅ AHORA (Correcto - Inventory-based)**
```
1. Player toca item → Item va al inventario → Item desaparece del mundo
2. Player abre inventario → Selecciona item → Presiona "Use"
3. Inventario llama item.Use(player) → Item cura → Item se consume
```

---

## 📦 Estructura de Archivos

### **Interfaces**
```
/Assets/Scripts/Items/Interfaces/
├── IItem.cs            → Interface base para todos los items
├── IUsableItem.cs      → Items que se pueden usar
└── IPickupable.cs      → Items que se pueden recoger
```

### **Data (ScriptableObjects)**
```
/Assets/Scripts/Items/Data/
└── ConsumableItemData.cs  → Config de consumibles (medkit, potion, etc)
```

### **Components**
```
/Assets/Scripts/Items/
├── WorldItemPickup.cs          → Pickup en el mundo (solo recoge, NO usa)
├── ConsumableEffectHandler.cs  → Maneja efectos over-time
└── INVENTORY_INTEGRATION_EXAMPLE.cs  → Ejemplo de cómo el inventario usa items
```

### **Types**
```
/Assets/Scripts/Items/
└── ItemType.cs  → Enum de tipos (Consumable, Weapon, Armor, etc)
```

---

## 🔧 Interfaces Clave

### **IItem** - Todo item debe implementar esto
```csharp
public interface IItem
{
    string ItemName { get; }
    string Description { get; }
    Sprite Icon { get; }
    ItemType ItemType { get; }
    bool IsStackable { get; }
    int MaxStackSize { get; }
}
```

### **IUsableItem** - Items que se pueden usar
```csharp
public interface IUsableItem : IItem
{
    bool CanUse(GameObject user);      // ¿Se puede usar ahora?
    void Use(GameObject user);          // Usar el item
    bool IsConsumedOnUse { get; }       // ¿Se consume al usar?
}
```

### **IPickupable** - Items en el mundo
```csharp
public interface IPickupable
{
    IItem ItemData { get; }
    void OnPickedUp(GameObject picker);
}
```

---

## 🎮 Cómo Funciona

### **1. Crear Consumable (Medkit)**

**A) Crear ScriptableObject:**
```
Right-click Project:
Create > Items > Consumable Item

Configure:
├── Item Name: "Medkit"
├── Description: "Restores 50 HP"
├── Icon: [sprite del medkit]
├── Item Type: Consumable
├── Is Stackable: true
├── Max Stack Size: 5
│
├── Effect Type: RestoreHealth
├── Effect Value: 50
├── Is Over Time: false
```

**B) Crear GameObject en el mundo:**
```
GameObject: Medkit_Pickup
├── SpriteRenderer (visual)
├── BoxCollider2D (trigger = true)
└── WorldItemPickup.cs
    ├── Item Data: [Medkit ScriptableObject]
    └── Quantity: 1
```

---

### **2. Pickup Flow**

```csharp
// WorldItemPickup.cs - OnTriggerEnter2D
void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        // 🎒 AQUÍ el inventario debería recibir el item
        // Por ejemplo:
        // InventorySystem.AddItem(itemData, quantity);
        
        Debug.Log($"Picked up {itemData.ItemName}");
        OnPickedUp(other.gameObject);  // Destroy object
    }
}
```

**⚠️ Nota:** El `WorldItemPickup` solo recoge y destruye el objeto. **TÚ debes conectarlo con tu sistema de inventario** cuando lo implementes.

---

### **3. Use Flow (Desde Inventario)**

```csharp
// En tu futuro InventorySystem
public class InventorySystem
{
    public void UseItem(int slotIndex)
    {
        IUsableItem item = GetItemInSlot(slotIndex);
        
        if (item == null)
        {
            Debug.Log("No item in slot");
            return;
        }
        
        Player player = GetPlayer();
        
        // ✅ Verificar si se puede usar
        if (!item.CanUse(player.gameObject))
        {
            Debug.Log($"Cannot use {item.ItemName}");
            PlayErrorSound();
            return;
        }
        
        // ✅ Usar el item
        item.Use(player.gameObject);
        
        // ✅ Consumir si es necesario
        if (item.IsConsumedOnUse)
        {
            RemoveItemFromSlot(slotIndex, 1);
        }
    }
}
```

---

## 💻 Ejemplos de Uso

### **Ejemplo 1: Medkit (Instant Heal)**

**ConsumableItemData config:**
```
Item Name: Medkit
Effect Type: RestoreHealth
Effect Value: 50
Is Over Time: false
```

**Uso desde inventario:**
```csharp
ConsumableItemData medkit = GetMedkitFromInventory();

if (medkit.CanUse(player.gameObject))
{
    medkit.Use(player.gameObject);
    // Output: "[ITEM] Medkit restored 50 HP instantly"
}
```

---

### **Ejemplo 2: Bandage (Heal Over Time)**

**ConsumableItemData config:**
```
Item Name: Bandage
Effect Type: RestoreHealthOverTime
Effect Value: 30
Is Over Time: true
Duration: 5s
Tick Rate: 1s
```

**Uso desde inventario:**
```csharp
ConsumableItemData bandage = GetBandageFromInventory();

if (bandage.CanUse(player.gameObject))
{
    bandage.Use(player.gameObject);
    // Output: "[ITEM] Bandage will restore 30 HP over 5s"
    // Luego cada 1s: "[HOT] Healing 6 HP..."
}
```

---

### **Ejemplo 3: No se puede usar (Health full)**

```csharp
// Player tiene salud full
IHealth health = player.GetComponent<IHealth>();
// health.CurrentHealth == 100
// health.MaxHealth == 100

ConsumableItemData medkit = GetMedkitFromInventory();

if (medkit.CanUse(player.gameObject))
{
    // ❌ NO entra aquí
}
else
{
    Debug.Log("Cannot use Medkit - health already full!");
    // Mostrar mensaje en UI
}
```

---

## 🔌 Integración con tu Futuro Inventario

Cuando implementes tu sistema de inventario, necesitarás:

### **1. Modificar WorldItemPickup**

```csharp
// WorldItemPickup.cs
public void OnPickedUp(GameObject picker)
{
    // ✅ Agregar al inventario
    InventorySystem inventory = picker.GetComponent<InventorySystem>();
    if (inventory != null)
    {
        bool added = inventory.AddItem(itemData, quantity);
        
        if (added)
        {
            PlayFeedback();
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory full!");
        }
    }
}
```

### **2. Crear Inventory Slot**

```csharp
[System.Serializable]
public class InventorySlot
{
    public IItem item;
    public int quantity;
    
    public bool IsEmpty => item == null || quantity <= 0;
    
    public bool CanStack(IItem newItem)
    {
        return item != null && 
               item.ItemName == newItem.ItemName && 
               item.IsStackable && 
               quantity < item.MaxStackSize;
    }
}
```

### **3. UI - Item Slot**

```csharp
public class UIInventorySlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text quantityText;
    [SerializeField] private Button useButton;
    
    private IUsableItem currentItem;
    private int quantity;
    
    void Start()
    {
        useButton.onClick.AddListener(OnUseButtonClicked);
    }
    
    public void SetItem(IUsableItem item, int qty)
    {
        currentItem = item;
        quantity = qty;
        
        iconImage.sprite = item.Icon;
        iconImage.enabled = true;
        
        quantityText.text = qty > 1 ? qty.ToString() : "";
    }
    
    void OnUseButtonClicked()
    {
        if (currentItem == null) return;
        
        Player player = FindObjectOfType<Player>();
        
        if (currentItem.CanUse(player.gameObject))
        {
            currentItem.Use(player.gameObject);
            
            if (currentItem.IsConsumedOnUse)
            {
                quantity--;
                
                if (quantity <= 0)
                {
                    Clear();
                }
                else
                {
                    quantityText.text = quantity.ToString();
                }
            }
        }
        else
        {
            Debug.Log("Cannot use item!");
        }
    }
    
    void Clear()
    {
        currentItem = null;
        quantity = 0;
        iconImage.enabled = false;
        quantityText.text = "";
    }
}
```

---

## 🧪 Testing (SIN inventario por ahora)

Para testear el sistema **sin tener inventario implementado**, usá el script de ejemplo:

### **Setup de Test:**

1. **Crear Medkit ScriptableObject:**
   ```
   Create > Items > Consumable Item
   Name: Medkit
   Effect Type: RestoreHealth
   Effect Value: 50
   ```

2. **Guardar en Resources:**
   ```
   /Assets/Resources/Items/Medkit.asset
   ```

3. **Agregar script de test al Player:**
   ```
   Player GameObject:
   └── INVENTORY_INTEGRATION_EXAMPLE.cs
   ```

4. **Play mode:**
   - Presioná `1` para usar medkit
   - Verificá logs y que la salud suba

---

## 🎯 Tipos de Items Soportados

### **Consumables (Implementado)**
- ✅ Restore Health (instant)
- ✅ Restore Health Over Time
- 🔮 Restore Stamina (preparado, implementar cuando tengas stamina)
- 🔮 Buffs (preparado)
- 🔮 Antidote (preparado)

### **Otros tipos (Futuro)**
- 🔮 Weapons (IEquippable)
- 🔮 Armor (IEquippable)
- 🔮 Quest Items (no usables)
- 🔮 Keys (IUsableItem con target)

---

## ⚙️ Configuración Avanzada

### **Múltiples Efectos**

Si querés que un item tenga múltiples efectos (ej: heal + stamina):

```csharp
// Extender ConsumableItemData
[System.Serializable]
public class ItemEffect
{
    public ConsumableEffect effectType;
    public float value;
}

public class ConsumableItemData : ScriptableObject
{
    [SerializeField] private ItemEffect[] effects;  // Array de efectos
    
    public void Use(GameObject user)
    {
        foreach (var effect in effects)
        {
            ApplyEffect(user, effect);
        }
    }
}
```

### **Cooldowns**

```csharp
public class ConsumableItemData : ScriptableObject
{
    [SerializeField] private float cooldown = 0f;
    private float lastUseTime;
    
    public bool CanUse(GameObject user)
    {
        if (Time.time - lastUseTime < cooldown)
        {
            return false;
        }
        
        // ... resto de checks
    }
    
    public void Use(GameObject user)
    {
        lastUseTime = Time.time;
        // ... resto de lógica
    }
}
```

---

## 📋 Checklist de Integración

Cuando implementes tu inventario:

- [ ] Crear `InventorySystem` component
- [ ] Crear array de `InventorySlot[]`
- [ ] Implementar `AddItem(IItem, int quantity)`
- [ ] Implementar `RemoveItem(IItem, int quantity)`
- [ ] Implementar `UseItem(int slotIndex)`
- [ ] Conectar `WorldItemPickup` con `InventorySystem`
- [ ] Crear UI para slots de inventario
- [ ] Bind teclas/botones a `UseItem()`
- [ ] Implementar drag & drop (opcional)
- [ ] Implementar quickslots/hotbar (opcional)

---

## 🔄 Diferencias con el Sistema Anterior

| Aspecto | Sistema Anterior (❌) | Sistema Actual (✅) |
|---------|----------------------|---------------------|
| **Pickup** | Toca item → cura automáticamente | Toca item → va al inventario |
| **Uso** | Automático al tocar | Manual desde inventario |
| **Compatibilidad** | Solo healing automático | Compatible con inventory system |
| **Control** | Player no decide cuándo usar | Player decide cuándo usar |
| **Stackable** | No | Sí (configurable) |
| **Item types** | Solo healing | Consumable, Weapon, Armor, etc |

---

## 🎓 Best Practices

### ✅ DO:
- Separar "pickup" de "use"
- Usar `CanUse()` antes de `Use()`
- Verificar `IsConsumedOnUse` para decrementar stack
- Cachear referencia a Player/InventorySystem
- Usar eventos para feedback (OnItemUsed, OnItemAdded)

### ❌ DON'T:
- No auto-usar items al pickearlos
- No llamar `Use()` sin verificar `CanUse()`
- No hardcodear item references (usar ScriptableObjects)
- No modificar health directamente (usar IHealable)
- No duplicar lógica de efectos en múltiples lugares

---

## 🚀 Próximos Pasos

1. **Ahora:** Usar sistema de Health standalone (fall damage, etc)
2. **Pronto:** Implementar sistema de inventario básico
3. **Después:** Conectar items con inventario
4. **Futuro:** Expandir a weapons, armor, equipment

**El sistema de Health (HealthController, IHealth, etc) funciona INDEPENDIENTEMENTE del sistema de items.** Los consumibles son solo UNA forma de curación, pero podés curar por:
- Checkpoints
- Level-up
- Scripts de escena
- Zonas de curación
- NPCs healers

---

**El sistema está preparado para cuando implementes el inventario. Por ahora, el HealthController funciona perfectamente sin items.**
