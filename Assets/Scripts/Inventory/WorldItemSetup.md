# Guía Completa: Crear Armas y Objetos en el Mundo

## Tabla de Contenidos
1. [Crear ScriptableObject de Arma](#1-crear-scriptableobject-de-arma)
2. [Crear GameObject de Arma en el Mundo](#2-crear-gameobject-de-arma-en-el-mundo)
3. [Configuración Completa](#3-configuración-completa)
4. [Ejemplos Prácticos](#4-ejemplos-prácticos)

---

## 1. Crear ScriptableObject de Arma

### Paso 1.1: Crear el Archivo de Datos

1. **En Project**, navega a donde quieras guardarlo:
   - Recomendado: `/Assets/Data/Items/Weapons/`
   - Si no existe, créala: `Create > Folder`

2. **Haz clic derecho** > `Create > Inventory > Weapon Item`

3. **Renombra** el archivo:
   - Ejemplo: `Pistol`, `CombatKnife`, `Shotgun`

### Paso 1.2: Configurar el Arma

Selecciona el arma creada y configura:

#### **Basic Info**
```
Item Name: "Pistol 9mm"
Item ID: "weapon_pistol_9mm"  (debe ser único)
Description: "A reliable semi-automatic pistol."
Item Icon: [Arrastra sprite pequeño 64x64]
```

#### **Item Type**
```
Item Type: Weapon
Stackable: ☐ (desactivado)
```

#### **Weapon Stats**
```
Damage: 20
Attack Range: 10
Attack Speed: 2
Weapon Type: Ranged
```

#### **Ammo Configuration**

**Para armas de fuego:**
```
Required Ammo: Pistol_9mm
Magazine Size: 15
Ammo Per Shot: 1
```

**Para armas cuerpo a cuerpo:**
```
Required Ammo: None
Magazine Size: 0
Ammo Per Shot: 0
```

#### **Equipment**
```
Equip Icon: [Sprite para mostrar en slots 128x128]
```

---

## 2. Crear GameObject de Arma en el Mundo

Ahora vamos a crear el objeto físico que el jugador puede recoger.

### Paso 2.1: Crear el GameObject

1. **En Hierarchy**, haz clic derecho > `Create Empty`
2. Renómbralo: **"Pistol_Pickup"** (o el nombre que quieras)
3. Posiciona donde quieras que aparezca el arma

### Paso 2.2: Añadir Componentes Necesarios

El GameObject necesita estos componentes (añádelos con `Add Component`):

#### 1. **SpriteRenderer**
```
Sprite: [Icono del arma]
Color: White
Sorting Layer: Items
Order in Layer: 0
```

#### 2. **Collider2D** (elige uno)

**Opción A - BoxCollider2D:**
```
Is Trigger: ✓
Size: Ajusta al sprite (ej: 1, 1)
```

**Opción B - CircleCollider2D:**
```
Is Trigger: ✓
Radius: 0.5
```

#### 3. **WorldItem** (nuestro script)
```
Item Data: [Arrastra el ScriptableObject del arma]
Quantity: 1
```

### Paso 2.3: Configurar WorldItem

En el componente **WorldItem**, configura:

#### **Item Data**
```
Item Data: [Arrastra tu Pistol.asset aquí]
Quantity: 1
```

#### **Visual Settings**
```
Use Item Icon: ✓
Sprite Size: (1, 1)
```

#### **Interaction Settings**
```
Interaction Radius: 2
Pickup Message: "Press E to pick up"
Auto Pickup: ☐ (desactivado)
```

#### **Animation**
```
Enable Floating: ✓
Float Speed: 1
Float Height: 0.3
Enable Rotation: ☐
```

---

## 3. Configuración Completa

### Estructura Final del GameObject

```
Pistol_Pickup
├── Transform (posición en el mundo)
├── SpriteRenderer (visual del arma)
├── BoxCollider2D (trigger para interacción)
└── WorldItem (script de pickup)
```

### Valores Recomendados por Tipo

#### Pistola
```
GameObject:
- Sprite Size: (0.8, 0.8)
- Float Height: 0.2
- Interaction Radius: 2

ScriptableObject:
- Damage: 20
- Attack Range: 10
- Attack Speed: 2
- Required Ammo: Pistol_9mm
- Magazine Size: 15
```

#### Escopeta
```
GameObject:
- Sprite Size: (1.2, 1.2)
- Float Height: 0.25
- Interaction Radius: 2.5

ScriptableObject:
- Damage: 50
- Attack Range: 5
- Attack Speed: 0.8
- Required Ammo: Shotgun_Shell
- Magazine Size: 8
```

#### Cuchillo
```
GameObject:
- Sprite Size: (0.6, 0.6)
- Float Height: 0.15
- Interaction Radius: 1.5

ScriptableObject:
- Damage: 25
- Attack Range: 1.5
- Attack Speed: 2.5
- Weapon Type: Melee
- Required Ammo: None
```

---

## 4. Ejemplos Prácticos

### Ejemplo 1: Crear una Pistola Completa

#### Paso A: ScriptableObject
```
ARCHIVO: /Assets/Data/Items/Weapons/Pistol.asset

Basic Info:
- Item Name: "9mm Pistol"
- Item ID: "weapon_pistol_9mm"
- Description: "Standard issue police pistol."
- Item Icon: pistol_icon.png

Weapon Stats:
- Damage: 20
- Attack Range: 10
- Attack Speed: 2
- Weapon Type: Ranged

Ammo:
- Required Ammo: Pistol_9mm
- Magazine Size: 15
- Ammo Per Shot: 1

Equipment:
- Equip Icon: pistol_equip.png
```

#### Paso B: GameObject en Escena
```
GameObject Name: Pistol_Pickup
Position: (5, 0, 0)

SpriteRenderer:
- Sprite: pistol_icon.png
- Sorting Layer: Items

BoxCollider2D:
- Is Trigger: ✓
- Size: (0.8, 0.8)

WorldItem:
- Item Data: Pistol.asset
- Quantity: 1
- Use Item Icon: ✓
- Sprite Size: (0.8, 0.8)
- Float Speed: 1
- Float Height: 0.2
```

### Ejemplo 2: Crear un Cuchillo

#### ScriptableObject
```
ARCHIVO: /Assets/Data/Items/Weapons/CombatKnife.asset

Basic Info:
- Item Name: "Combat Knife"
- Item ID: "weapon_knife_combat"
- Item Icon: knife_icon.png

Weapon Stats:
- Damage: 30
- Attack Range: 1.5
- Attack Speed: 2.5
- Weapon Type: Melee

Ammo:
- Required Ammo: None
- Magazine Size: 0
```

#### GameObject
```
GameObject Name: Knife_Pickup

WorldItem:
- Item Data: CombatKnife.asset
- Sprite Size: (0.6, 0.6)
- Float Height: 0.15
- Enable Rotation: ✓
- Rotation Speed: 50
```

---

## 5. Crear Prefab para Reutilizar

### Convertir a Prefab

1. Arrastra el GameObject desde **Hierarchy** a **Project**
2. Guárdalo en `/Assets/Prefabs/Items/`
3. Ahora puedes arrastrar el prefab varias veces a la escena

### Crear Variantes

Para crear diferentes armas a partir del mismo prefab:

1. Arrastra el prefab base a la escena
2. Cambia el **Item Data** en WorldItem
3. Haz clic derecho > `Create > Prefab Variant`
4. Renombra: `Pistol_Pickup_Variant`

---

## 6. Testing

### Probar en Play Mode

1. **Entra en Play Mode**
2. **Acércate** al objeto con el jugador
3. **Debería aparecer** el mensaje de interacción
4. **Presiona E** para recogerlo
5. **Abre el inventario** (Tab) - debería estar ahí
6. **Selecciona el arma** y equípala en un slot

### Debug

Si no funciona:

**El mensaje no aparece:**
- ✓ Verifica que el Collider tenga `Is Trigger` activado
- ✓ Revisa que el Player tenga el tag "Player"
- ✓ Comprueba que `Interaction Radius` sea suficiente

**No se recoge:**
- ✓ Verifica que Item Data esté asignado
- ✓ Revisa que el Player tenga `InventorySystem`
- ✓ Comprueba la consola por errores

**No aparece visualmente:**
- ✓ Asigna un Sprite en SpriteRenderer
- ✓ Verifica que la Sorting Layer sea visible
- ✓ Comprueba que `Use Item Icon` esté activado

---

## 7. Características del Sistema

### ✅ Animaciones Incluidas

**Floating (Flotación):**
- El objeto sube y baja suavemente
- Configurable con `Float Speed` y `Float Height`

**Rotation (Rotación):**
- El objeto gira sobre su eje
- Configurable con `Rotation Speed`

**Pickup Animation:**
- Al recoger, el objeto sube y desaparece
- Fade out automático

### ✅ Auto Pickup

Si activas `Auto Pickup`:
```
Auto Pickup: ✓
```
El jugador recogerá el item automáticamente al tocarlo (sin presionar E).

### ✅ Interacción con Sistema de Inventario

El WorldItem se integra con:
- ✓ `InventorySystem` - añade al inventario
- ✓ `WeaponInventoryManager` - permite equipar
- ✓ Sistema de interacción - muestra prompts
- ✓ Menú contextual - opciones "Equip", "Examine", etc.

---

## 8. Shortcuts y Tips

### Context Menu Actions

Con el GameObject seleccionado:
- `Right Click > Refresh Visuals` - actualiza el sprite

### Crear Rápidamente

1. **Duplica** un pickup existente (Ctrl+D)
2. **Cambia** solo el Item Data
3. **Mueve** a nueva posición
4. Listo!

### Sprites Placeholder

Si no tienes sprites:
```
1. Crea cuadrados de colores en Paint/Photoshop
2. Importa a Unity
3. Asígnalos temporalmente
4. Reemplaza después con arte final
```

### Organización de Assets

```
/Assets
  /Data
    /Items
      /Weapons      ← ScriptableObjects aquí
      /Consumables
  /Prefabs
    /Items         ← Prefabs de WorldItem aquí
  /Sprites
    /Items
      /Icons       ← Sprites pequeños (64x64)
      /Equipment   ← Sprites grandes (128x128)
```

---

## 9. Troubleshooting Común

| Problema | Solución |
|----------|----------|
| "Item Data not assigned" | Asigna el ScriptableObject en WorldItem |
| No aparece el sprite | Activa `Use Item Icon` en WorldItem |
| No se puede interactuar | Verifica que Collider tenga `Is Trigger` ✓ |
| Se recoge automáticamente | Desactiva `Auto Pickup` |
| No flota | Activa `Enable Floating` |
| Inventario lleno | El pickup no se destruirá hasta tener espacio |

---

## 10. Próximos Pasos

Ahora que tienes items en el mundo:

1. **Crea varias armas** diferentes
2. **Colócalas** estratégicamente en tu nivel
3. **Crea prefabs** para reutilizar
4. **Añade munición** usando el mismo sistema
5. **Integra** con el menú contextual para equipar

¿Necesitas crear items consumables, munición o llaves? ¡Usa el mismo proceso!
