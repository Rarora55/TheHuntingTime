# 🏗️ Arquitectura del Sistema de Combinación

## ✅ Diseño Sin Dependencias Circulares

Esta arquitectura sigue principios de **separación de responsabilidades** y **bajo acoplamiento**:

---

## 📐 Capas de Responsabilidad

### **1. Capa de Datos (ScriptableObjects) - SIN DEPENDENCIAS DE ESCENA**

```
ItemData (ScriptableObject)
├── Implementa ICombinable
├── NO depende de CombinationManager
├── NO usa FindObjectOfType
├── Solo expone propiedades de datos
└── Métodos son solo validaciones simples
```

**Responsabilidades:**
- ✅ Exponer datos de configuración (`CanBeCombined`, `CombinationHint`)
- ✅ Validaciones básicas (¿puede combinarse?, ¿no es null?, ¿no es el mismo item?)
- ❌ NO busca recetas
- ❌ NO ejecuta lógica de combinación

---

### **2. Capa de Recetas (ScriptableObjects)**

```
CombinationRecipe (ScriptableObject)
├── Define qué items se combinan
├── Define qué se produce
├── Validaciones en OnValidate()
└── NO ejecuta combinaciones
```

**Responsabilidades:**
- ✅ Definir relaciones entre items
- ✅ Validar configuración en editor
- ❌ NO ejecuta lógica de juego

---

### **3. Capa de Lógica (MonoBehaviours en escena)**

```
CombinationManager (MonoBehaviour)
├── Gestiona lista de recetas
├── Busca recetas compatibles
├── Valida cantidades en inventario
├── Ejecuta combinaciones
├── Consume/añade items
└── Dispara eventos
```

**Responsabilidades:**
- ✅ Buscar recetas válidas
- ✅ Validar inventario
- ✅ Ejecutar combinaciones
- ✅ Gestionar estado en runtime

---

### **4. Capa de UI (MonoBehaviours en escena)**

```
InventoryUIController (MonoBehaviour)
├── Referencia al CombinationManager
├── Gestiona modo combine
├── Maneja input del jugador
└── Dispara eventos de UI
```

**Responsabilidades:**
- ✅ Gestionar estados de UI
- ✅ Capturar input
- ✅ Iniciar combinaciones
- ✅ Feedback visual

---

## 🔄 Flujo de Datos (Sin Dependencias Circulares)

### **Flujo Correcto:**

```
1. ItemData (SO)
   ├── CanBeCombined: true
   └── CombinationHint: "Can mix with powder"
   
2. CombinationRecipe (SO)
   ├── ItemA: GunpowderA
   ├── ItemB: GunpowderB
   └── Result: HighGradePowder
   
3. CombinationManager (Scene)
   ├── Tiene lista de recetas
   ├── Busca: FindRecipe(ItemA, ItemB)
   └── Ejecuta: TryCombine()
   
4. InventoryUIController (Scene)
   ├── Referencia: CombinationManager
   ├── Llama: manager.TryCombine()
   └── Escucha: manager.OnCombinationSuccess
```

### **Dirección de Dependencias:**

```
ItemData (SO)     ← NO depende de nada
    ↑
    |
CombinationRecipe (SO)  ← Referencia ItemData (SO)
    ↑
    |
CombinationManager (Scene)  ← Referencia Recetas (SO)
    ↑
    |
InventoryUIController (Scene)  ← Referencia Manager (Scene)
```

**✅ Correcto:** ScriptableObjects NO dependen de MonoBehaviours  
**✅ Correcto:** Datos fluyen desde ScriptableObjects hacia la escena  
**❌ Incorrecto:** ScriptableObjects que llaman a FindObjectOfType  

---

## 💡 Por Qué Este Diseño

### **Problema: Dependencias Circulares**

```csharp
// ❌ MAL DISEÑO
public class ItemData : ScriptableObject
{
    public bool CanCombineWith(ItemData other)
    {
        // ScriptableObject depende de MonoBehaviour en escena
        var manager = FindObjectOfType<CombinationManager>();
        return manager.CanCombineWith(this, other);
    }
}
```

**Problemas:**
- ❌ `FindObjectOfType` es costoso (busca en toda la escena)
- ❌ ScriptableObject depende de que haya un manager en la escena
- ❌ No funciona en modo editor sin escena cargada
- ❌ Rompe el principio de separación de responsabilidades
- ❌ Dificulta testing unitario

---

### **Solución: Separación Clara**

```csharp
// ✅ BUEN DISEÑO
public class ItemData : ScriptableObject, ICombinable
{
    [SerializeField] private bool canBeCombined;
    [SerializeField] private string combinationHint;
    
    // Solo validaciones simples, sin dependencias
    public virtual bool CanCombineWith(ItemData otherItem)
    {
        return canBeCombined && 
               otherItem != null && 
               otherItem.CanBeCombined && 
               this != otherItem;
    }
    
    // Solo retorna el hint configurado
    public virtual string GetCombinationHint(ItemData otherItem)
    {
        return combinationHint;
    }
}
```

**Ventajas:**
- ✅ No busca en la escena
- ✅ Funciona sin managers
- ✅ Testeable en modo editor
- ✅ Rápido (sin FindObjectOfType)
- ✅ Bajo acoplamiento

---

## 🎯 Responsabilidades Claramente Definidas

### **ItemData (ScriptableObject)**

```csharp
// ✅ Expone datos
public bool CanBeCombined => canBeCombined;

// ✅ Validación básica (sin lógica de negocio)
public bool CanCombineWith(ItemData other)
{
    return canBeCombined && other != null && other.CanBeCombined;
}

// ✅ Retorna hint configurado
public string GetCombinationHint(ItemData other)
{
    return combinationHint;
}

// ❌ NO busca recetas
// ❌ NO ejecuta combinaciones
// ❌ NO accede a managers
```

---

### **CombinationManager (MonoBehaviour)**

```csharp
// ✅ Busca recetas
public CombinationRecipe FindRecipe(ItemData a, ItemData b)
{
    return allRecipes.FirstOrDefault(r => r.CanCombine(a, b));
}

// ✅ Valida cantidades
private bool HasRequiredQuantities(ItemData a, ItemData b, recipe)
{
    return CountItemInInventory(a) >= recipe.ConsumeAmountA &&
           CountItemInInventory(b) >= recipe.ConsumeAmountB;
}

// ✅ Ejecuta combinación
public bool TryCombineItems(ItemData a, ItemData b)
{
    var recipe = FindRecipe(a, b);
    if (recipe == null) return false;
    
    if (!HasRequiredQuantities(a, b, recipe)) return false;
    
    ExecuteCombination(recipe);
    return true;
}
```

---

### **InventoryUIController (MonoBehaviour)**

```csharp
// ✅ Referencia explícita al manager
[SerializeField] private CombinationManager combinationManager;

// ✅ Delega al manager
private void StartCombineMode()
{
    isCombineMode = true;
    combineSourceSlot = inventorySystem.SelectedSlot;
}

public void TryCombineWithSelected()
{
    // ✅ Usa la referencia directa
    bool success = combinationManager.TryCombine(
        combineSourceSlot, 
        inventorySystem.SelectedSlot
    );
}
```

---

## 🔧 Cómo Usar la API

### **Desde UI o Gameplay**

```csharp
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CombinationManager combinationManager;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            // ✅ Usa la referencia del manager
            var available = combinationManager.GetAvailableCombinations();
            
            foreach (var recipe in available)
            {
                Debug.Log($"Can craft: {recipe.ResultItem.ItemName}");
            }
        }
    }
}
```

### **Verificar si Item puede Combinarse (Datos)**

```csharp
// ✅ Validación básica sin manager
if (itemA.CanCombineWith(itemB))
{
    Debug.Log($"{itemA.ItemName} can potentially combine with {itemB.ItemName}");
}

// ✅ Mostrar hint
string hint = itemA.GetCombinationHint(itemB);
Debug.Log(hint); // "Can be mixed with other powder"
```

### **Buscar y Ejecutar Receta (Lógica)**

```csharp
// ✅ El manager busca la receta específica
CombinationRecipe recipe = combinationManager.FindRecipe(itemA, itemB);

if (recipe != null)
{
    Debug.Log($"Recipe found: {recipe.RecipeName}");
    Debug.Log($"Result: {recipe.ResultItem.ItemName}");
    
    // ✅ Ejecutar combinación
    bool success = combinationManager.TryCombineItems(itemA, itemB);
}
```

---

## 🧪 Testing

### **Testing de ItemData (Sin Escena)**

```csharp
[Test]
public void ItemData_CanCombineWith_ReturnsTrueForValidItems()
{
    // ✅ No requiere escena ni managers
    ItemData itemA = CreateTestItem(canBeCombined: true);
    ItemData itemB = CreateTestItem(canBeCombined: true);
    
    Assert.IsTrue(itemA.CanCombineWith(itemB));
}

[Test]
public void ItemData_CanCombineWith_ReturnsFalseForSameItem()
{
    ItemData item = CreateTestItem(canBeCombined: true);
    
    // ✅ Validación básica funciona sin manager
    Assert.IsFalse(item.CanCombineWith(item));
}
```

### **Testing de CombinationManager (Con Escena)**

```csharp
[UnityTest]
public IEnumerator CombinationManager_FindsValidRecipe()
{
    // Setup
    var manager = CreateManagerInScene();
    var recipe = CreateTestRecipe(itemA, itemB, result);
    manager.AddRecipe(recipe);
    
    // ✅ Test lógica del manager
    CombinationRecipe found = manager.FindRecipe(itemA, itemB);
    
    Assert.AreEqual(recipe, found);
    yield return null;
}
```

---

## 📊 Comparación de Arquitecturas

### **❌ Con Singleton/FindObjectOfType**

```
ItemData (SO)
    ↓ FindObjectOfType<>()
CombinationManager (Scene)
    ↓ uses
ItemData (SO)
```

**Problemas:**
- Dependencia circular
- Costoso (FindObjectOfType)
- No funciona sin escena
- Difícil de testear

---

### **✅ Arquitectura Actual**

```
ItemData (SO) ← Datos puros
    ↑ referencia
CombinationRecipe (SO) ← Configuración
    ↑ referencia
CombinationManager (Scene) ← Lógica
    ↑ referencia
InventoryUIController (Scene) ← UI
```

**Ventajas:**
- Sin dependencias circulares
- Rápido (sin búsquedas)
- Funciona en editor
- Fácil de testear
- Bajo acoplamiento

---

## 🎓 Principios Aplicados

### **1. Separation of Concerns**
- ItemData = Datos
- CombinationRecipe = Configuración
- CombinationManager = Lógica
- InventoryUIController = Presentación

### **2. Dependency Inversion**
- Managers dependen de abstracciones (ICombinable)
- No hay dependencias de ScriptableObjects → MonoBehaviours

### **3. Single Responsibility**
- Cada clase tiene una responsabilidad clara
- ItemData NO ejecuta combinaciones
- Manager NO define datos de items

### **4. Open/Closed**
- Extender funcionalidad = Crear nuevas recetas
- No modificar código existente

---

## ✅ Checklist de Buenas Prácticas

Cuando añadas nueva funcionalidad:

- [ ] ScriptableObjects NO llaman a `FindObjectOfType`
- [ ] ScriptableObjects NO dependen de MonoBehaviours
- [ ] ScriptableObjects solo exponen datos y validaciones simples
- [ ] MonoBehaviours tienen referencias explícitas (`[SerializeField]`)
- [ ] Lógica de negocio está en Managers, no en ScriptableObjects
- [ ] Dependencias fluyen de arriba (SO) hacia abajo (Scene)
- [ ] Sistema testeable sin escena cargada

---

## 🚀 Resumen

**ItemData (ScriptableObject):**
- ✅ `CanBeCombined` - Propiedad de dato
- ✅ `CombinationHint` - Texto configurado
- ✅ `CanCombineWith()` - Validación básica (null check, mismo item)
- ❌ NO busca recetas
- ❌ NO ejecuta combinaciones

**CombinationManager (MonoBehaviour):**
- ✅ `FindRecipe()` - Busca receta válida
- ✅ `TryCombine()` - Ejecuta combinación
- ✅ `GetAvailableCombinations()` - Lista recetas disponibles
- ✅ Valida cantidades
- ✅ Consume/añade items

**InventoryUIController (MonoBehaviour):**
- ✅ Referencia directa a `CombinationManager`
- ✅ Gestiona estados de UI
- ✅ Captura input
- ✅ Dispara eventos

---

Esta arquitectura es **escalable**, **testeable** y **mantenible** sin dependencias circulares ni singletons. 🎯
