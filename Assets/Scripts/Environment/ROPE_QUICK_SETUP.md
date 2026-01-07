# 🪢 Rope System - Quick Setup

## ⚡ Setup Rápido en 5 Pasos

### 1️⃣ Crear RopeItem

```
Assets → Create → Inventory → Weapon Item Data
Nombre: RopeItem

Settings:
├── Weapon Type: Tool
├── Tool Type: Rope
└── Can Be Equipped: ✅
```

---

### 2️⃣ Crear Prefab RopeClimbable

```
Hierarchy → Create Empty → "RopeClimbable"

Components:
├── Tag: FrontLadder
├── BoxCollider2D (isTrigger: ✅, Size: 0.5 x 5.0)
└── RopeClimbable.cs (Rope Length: 5.0)

Guardar en: /Assets/Prefabs/Environment/RopeClimbable.prefab
```

---

### 3️⃣ Crear RopeAnchor en Escena

```
Hierarchy → Create Empty → "RopeAnchor_01"

Estructura:
RopeAnchor_01
├── AnchorVisual (Sprite opcional)
└── RopeSpawnPoint (Empty)

Components en RopeAnchor_01:
├── CircleCollider2D (isTrigger: ✅, Radius: 1.5)
├── RopeAnchorPoint.cs
│   ├── Rope Spawn Point: (RopeSpawnPoint Transform)
│   ├── Rope Length: 5.0
│   └── Rope Prefab: (RopeClimbable.prefab)
│
├── RopeAnchorInteraction.cs
└── InteractableObject.cs
```

---

### 4️⃣ Equipar Rope en Player

```
Runtime:
1. Abre inventario
2. Equipa RopeItem en Secondary Weapon Slot
```

---

### 5️⃣ Usar en Juego

```
1. Acércate al RopeAnchor
2. Presiona [E] → "Deploy Rope"
3. Presiona [W/S] para trepar/descender
```

---

## 🔍 Checklist Rápido

### RopeItem (WeaponItemData)
- [ ] WeaponType = **Tool**
- [ ] ToolType = **Rope**
- [ ] Can Be Equipped = **true**

### RopeClimbable (Prefab)
- [ ] Tag = **FrontLadder**
- [ ] BoxCollider2D → isTrigger = **true**
- [ ] RopeClimbable.cs añadido

### RopeAnchor (Scene)
- [ ] CircleCollider2D → isTrigger = **true**
- [ ] RopeAnchorPoint.cs → Rope Prefab asignado
- [ ] RopeAnchorInteraction.cs añadido
- [ ] InteractableObject.cs añadido

### Player
- [ ] WeaponInventoryManager presente
- [ ] RopeItem equipado en Secondary Slot

---

## 🐛 Errores Comunes

| Error | Solución |
|-------|----------|
| "No rope equipped" | Equipa RopeItem en Secondary Slot |
| Player no trepa | Verifica Tag `FrontLadder` |
| No aparece prompt | Añade `InteractableObject.cs` |
| Rope no se ve | LineRenderer es opcional, funciona sin él |

---

## 📊 Valores Recomendados

```
RopeAnchorPoint:
├── Rope Length: 5.0 (altura de descenso)
└── Interaction Range: 2.0

RopeClimbable:
├── BoxCollider Size: (0.5, 5.0)
├── Offset: (0, -2.5)
└── Rope Segments: 10 (para LineRenderer)

CircleCollider2D:
└── Radius: 1.5 (zona de interacción)
```

---

## 🎯 Estructura Final

```
PLAYER
└── Secondary Slot → 🪢 RopeItem

SCENE
└── 🪝 RopeAnchor_01
    └── Spawns → 🪢 RopeClimbable (cuando interactúas)
```

---

Para más detalles, revisa: `/Assets/Scripts/Environment/ROPE_SYSTEM_SETUP.md` 📖
