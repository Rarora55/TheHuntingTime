# 📖 Ejemplos de Recetas de Combinación

Colección de recetas listas para usar inspiradas en Resident Evil y Silent Hill.

---

## 🧪 Munición y Pólvora

### Recipe 1: Handgun Bullets
```
Recipe Name: "Handgun Bullets"
Description: "Combine gunpowder to create pistol ammunition."

Required Items:
  Item A: GunpowderA
  Item B: EmptyCasing
  Bidirectional: ✓

Result:
  Result Item: HandgunBullets
  Result Quantity: 15

Consumption:
  Consume Amount A: 1
  Consume Amount B: 1

Success Message: "Created 15 handgun bullets!"
```

### Recipe 2: Shotgun Shells
```
Recipe Name: "Shotgun Shells"
Description: "Create shotgun ammunition."

Required Items:
  Item A: GunpowderB
  Item B: EmptyCasing
  Bidirectional: ✓

Result:
  Result Item: ShotgunShells
  Result Quantity: 8

Consumption:
  Consume Amount A: 1
  Consume Amount B: 1

Success Message: "Created 8 shotgun shells!"
```

### Recipe 3: High-Grade Powder
```
Recipe Name: "High-Grade Powder"
Description: "Mix two types of gunpowder for enhanced ammunition."

Required Items:
  Item A: GunpowderA
  Item B: GunpowderB
  Bidirectional: ✓

Result:
  Result Item: HighGradePowder
  Result Quantity: 1

Consumption:
  Consume Amount A: 1
  Consume Amount B: 1

Success Message: "Mixed high-grade gunpowder!"
```

---

## 💊 Curativas y Medicinas

### Recipe 4: First Aid Spray
```
Recipe Name: "First Aid Spray"
Description: "Create a powerful healing spray."

Required Items:
  Item A: GreenHerb
  Item B: ChemicalFluid
  Bidirectional: ☐ (orden específico)

Result:
  Result Item: FirstAidSpray
  Result Quantity: 1

Consumption:
  Consume Amount A: 3  (3 hierbas)
  Consume Amount B: 1

Success Message: "Created First Aid Spray!"
```

### Recipe 5: Mixed Herbs (Green + Red)
```
Recipe Name: "Mixed Herbs G+R"
Description: "Enhance healing with red herb."

Required Items:
  Item A: GreenHerb
  Item B: RedHerb
  Bidirectional: ✓

Result:
  Result Item: MixedHerbsGR
  Result Quantity: 1

Consumption:
  Consume Amount A: 1
  Consume Amount B: 1

Success Message: "Mixed green and red herbs!"
```

### Recipe 6: Mixed Herbs (Green + Green + Green)
```
Recipe Name: "Triple Green Herb"
Description: "Maximum green herb potency."

Required Items:
  Item A: GreenHerb
  Item B: MixedHerbsGG  (2 verdes ya mezcladas)
  Bidirectional: ✓

Result:
  Result Item: MixedHerbsGGG
  Result Quantity: 1

Consumption:
  Consume Amount A: 1
  Consume Amount B: 1

Success Message: "Mixed three green herbs!"
```

---

## 🔧 Mejoras de Armas

### Recipe 7: Silenced Pistol
```
Recipe Name: "Silenced Pistol"
Description: "Attach a silencer to your pistol."

Required Items:
  Item A: Pistol9mm
  Item B: SilencerAttachment
  Bidirectional: ✓

Result:
  Result Item: SilencedPistol
  Result Quantity: 1

Consumption:
  Consume Amount A: 1
  Consume Amount B: 1

Success Message: "Attached silencer to pistol!"
```

### Recipe 8: Tactical Shotgun
```
Recipe Name: "Tactical Shotgun"
Description: "Upgrade shotgun with laser sight."

Required Items:
  Item A: Shotgun
  Item B: LaserSight
  Bidirectional: ✓

Result:
  Result Item: TacticalShotgun
  Result Quantity: 1

Consumption:
  Consume Amount A: 1
  Consume Amount B: 1

Success Message: "Upgraded shotgun with laser sight!"
```

### Recipe 9: Magnum Scope
```
Recipe Name: "Scoped Magnum"
Description: "Add precision scope to magnum."

Required Items:
  Item A: Magnum
  Item B: Scope
  Bidirectional: ✓

Result:
  Result Item: ScopedMagnum
  Result Quantity: 1

Consumption:
  Consume Amount A: 1
  Consume Amount B: 1

Success Message: "Attached scope to magnum!"
```

---

## 🗝️ Llaves y Puzzles

### Recipe 10: Ornate Key
```
Recipe Name: "Ornate Key"
Description: "Combine key parts to form complete key."

Required Items:
  Item A: KeyPart1
  Item B: KeyPart2
  Bidirectional: ✓

Result:
  Result Item: OrnateKey
  Result Quantity: 1

Consumption:
  Consume Amount A: 1
  Consume Amount B: 1

Success Message: "Assembled the ornate key!"
```

### Recipe 11: Ancient Seal
```
Recipe Name: "Ancient Seal"
Description: "Combine medallions to form the seal."

Required Items:
  Item A: GoldMedallion
  Item B: SilverMedallion
  Bidirectional: ☐

Result:
  Result Item: AncientSeal
  Result Quantity: 1

Consumption:
  Consume Amount A: 1
  Consume Amount B: 1

Success Message: "Formed the ancient seal!"
```

---

## 🧰 Herramientas y Utilidades

### Recipe 12: Lockpick
```
Recipe Name: "Lockpick"
Description: "Craft a lockpick from metal scraps."

Required Items:
  Item A: MetalScrap
  Item B: WirePiece
  Bidirectional: ✓

Result:
  Result Item: Lockpick
  Result Quantity: 3

Consumption:
  Consume Amount A: 1
  Consume Amount B: 1

Success Message: "Crafted 3 lockpicks!"
```

### Recipe 13: Molotov Cocktail
```
Recipe Name: "Molotov Cocktail"
Description: "Create explosive from cloth and fuel."

Required Items:
  Item A: EmptyBottle
  Item B: Gasoline
  Bidirectional: ☐

Result:
  Result Item: MolotovCocktail
  Result Quantity: 1

Consumption:
  Consume Amount A: 1
  Consume Amount B: 1

Success Message: "Created molotov cocktail!"
```

### Recipe 14: Rope + Hook = Grappling Hook
```
Recipe Name: "Grappling Hook"
Description: "Combine rope with hook."

Required Items:
  Item A: Rope
  Item B: MetalHook
  Bidirectional: ✓

Result:
  Result Item: GrapplingHook
  Result Quantity: 1

Consumption:
  Consume Amount A: 1
  Consume Amount B: 1

Success Message: "Created grappling hook!"
```

---

## ⚗️ Químicos y Materiales

### Recipe 15: Acid Rounds
```
Recipe Name: "Acid Rounds"
Description: "Create corrosive ammunition."

Required Items:
  Item A: GrenadeLauncher
  Item B: AcidVial
  Bidirectional: ☐

Result:
  Result Item: AcidRounds
  Result Quantity: 6

Consumption:
  Consume Amount A: 0  (no consume launcher)
  Consume Amount B: 1

Success Message: "Loaded acid rounds!"
```

### Recipe 16: Antidote
```
Recipe Name: "Antidote"
Description: "Cure poison status."

Required Items:
  Item A: BlueHerb
  Item B: ChemicalBase
  Bidirectional: ✓

Result:
  Result Item: Antidote
  Result Quantity: 1

Consumption:
  Consume Amount A: 2
  Consume Amount B: 1

Success Message: "Created antidote!"
```

---

## 🎮 Recetas Avanzadas (Multi-Step)

### Recipe 17A: Step 1 - Mixed Herbs GG
```
Recipe Name: "Mixed Herbs G+G"

Required Items:
  Item A: GreenHerb
  Item B: GreenHerb

Result:
  Result Item: MixedHerbsGG
```

### Recipe 17B: Step 2 - Mixed Herbs GGG
```
Recipe Name: "Mixed Herbs G+G+G"

Required Items:
  Item A: MixedHerbsGG
  Item B: GreenHerb

Result:
  Result Item: MixedHerbsGGG
```

### Recipe 18A: Step 1 - Gun Parts
```
Recipe Name: "Gun Barrel Assembly"

Required Items:
  Item A: GunBarrel
  Item B: GunTrigger

Result:
  Result Item: GunParts1
```

### Recipe 18B: Step 2 - Complete Gun
```
Recipe Name: "Assemble Custom Gun"

Required Items:
  Item A: GunParts1
  Item B: GunHandle

Result:
  Result Item: CustomPistol
```

---

## 💡 Template Genérico

Usa este template para crear tus propias recetas:

```
Recipe Name: "[Nombre descriptivo]"
Description: "[Qué hace esta combinación]"

Required Items:
  Item A: [Primer item requerido]
  Item B: [Segundo item requerido]
  Bidirectional: [✓/☐]

Result:
  Result Item: [Item que se crea]
  Result Quantity: [Cantidad que se crea]

Consumption:
  Consume Amount A: [Cuántos de A se consumen]
  Consume Amount B: [Cuántos de B se consumen]

Feedback:
  Success Message: "[Mensaje de éxito]"
  Fail Message: "[Mensaje de fallo]"
  Combination Sound: [Clip de audio opcional]
```

---

## 🎯 Consejos de Diseño

### Balanceo

**Curativas:**
- 1 Herb = 25% salud
- 2 Herbs = 50% salud
- 3 Herbs = 100% salud
- Herb + Chemical = 100% salud + antídoto

**Munición:**
- Pólvora básica = 15 balas
- Pólvora mejorada = 25 balas
- Múltiples pólvoras = munición especial

**Armas:**
- Arma base + accesorio = versión mejorada
- Consumir accesorio permanentemente
- Arma mejorada = +20% stats

### Progresión

```
Temprano:
- Combinaciones simples (1+1=1)
- Recursos abundantes
- Recetas obvias

Medio:
- Combinaciones multi-paso
- Recursos escasos
- Recetas opcionales

Tardío:
- Combinaciones complejas
- Recursos raros
- Recetas poderosas
```

---

## ✅ Checklist de Implementación

Para cada receta:

- [ ] Crear ItemA ScriptableObject
- [ ] Crear ItemB ScriptableObject  
- [ ] Crear ResultItem ScriptableObject
- [ ] Marcar `Can Be Combined: ✓` en ItemA
- [ ] Marcar `Can Be Combined: ✓` en ItemB
- [ ] Crear CombinationRecipe ScriptableObject
- [ ] Asignar items a la receta
- [ ] Configurar cantidades
- [ ] Añadir receta al CombinationManager
- [ ] Testear en Play Mode
- [ ] Ajustar balanceo

---

¡Usa estos ejemplos como base para crear tu propio sistema de crafteo único! 🎨
