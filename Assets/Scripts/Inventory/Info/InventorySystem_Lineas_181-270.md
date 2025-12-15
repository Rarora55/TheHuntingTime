# 🎒 InventorySystem - Línea por Línea (181-270)

**Archivo:** `/Assets/Scripts/Inventory/Core/InventorySystem.cs`

---

## ⚔️ Líneas 189-228: Método EquipWeapon

```csharp
189:         public void EquipWeapon(WeaponItemData weapon, EquipSlot slot)
```
- `public void`: Método público sin retorno
- `WeaponItemData weapon`: Arma a equipar
- `EquipSlot slot`: Primary o Secondary (enum)

```csharp
190:         {
```

```csharp
191:             if (weapon == null)
```
- Validación: ¿Arma es null?

```csharp
192:                 return;
```
- Sale sin hacer nada

```csharp
194:             bool hasWeapon = false;
```
- Flag para verificar si el arma está en inventario
- Inicializado en `false` (asume que no está)

```csharp
195:             for (int i = 0; i < MAX_SLOTS; i++)
```
- Loop por todos los slots (0-5)

```csharp
196:             {
```

```csharp
197:                 if (items[i] != null && items[i].itemData == weapon)
```
- ¿El slot tiene el arma que queremos equipar?
- Compara referencias de ScriptableObject

```csharp
198:                 {
```

```csharp
199:                     hasWeapon = true;
```
- Marcamos que SÍ está en inventario

```csharp
200:                     break;
```
- Sale del loop (ya no necesita seguir buscando)

```csharp
201:                 }
```

```csharp
202:             }
```

```csharp
204:             if (!hasWeapon)
```
- ¿El arma NO está en ningún slot?

```csharp
205:             {
```

```csharp
206:                 Debug.LogWarning("[INVENTORY] Cannot equip weapon not in inventory");
```
- **Restricción importante:** Solo se pueden equipar armas que estén en slots
- Previene equipar armas "de la nada"

```csharp
207:                 return;
```

```csharp
208:             }
```

```csharp
210:             if (slot == EquipSlot.Primary)
```
- ¿Queremos equipar en slot primario?

```csharp
211:             {
```

```csharp
212:                 if (primaryWeapon != null)
```
- ¿Ya hay un arma equipada en Primary?

```csharp
213:                     UnequipWeapon(EquipSlot.Primary);
```
- Desequipa el arma actual primero
- Llama método `UnequipWeapon()` (línea 230)

```csharp
215:                 primaryWeapon = weapon;
```
- Asigna la nueva arma al slot primario

```csharp
216:             }
```

```csharp
217:             else
```
- Si es slot secundario

```csharp
218:             {
```

```csharp
219:                 if (secondaryWeapon != null)
```
- ¿Ya hay arma en Secondary?

```csharp
220:                     UnequipWeapon(EquipSlot.Secondary);
```
- Desequipa arma secundaria

```csharp
222:                 secondaryWeapon = weapon;
```
- Asigna nueva arma al slot secundario

```csharp
223:             }
```

```csharp
225:             weapon.Equip(gameObject);
```
- Llama método `Equip()` del ScriptableObject
- `gameObject`: El jugador
- `WeaponItemData.Equip()` puede:
  - Instanciar prefab del arma
  - Adjuntar a la mano del jugador
  - Activar animaciones

```csharp
226:             OnWeaponEquipped?.Invoke(slot, weapon);
```
- Dispara evento con:
  - Slot donde se equipó
  - Arma equipada

```csharp
227:             Debug.Log($"<color=green>[INVENTORY] Equipped {weapon.ItemName} in {slot} slot</color>");
```
- Log verde confirmando equipamiento

```csharp
228:         }
```

---

## 🗡️ Líneas 230-246: Método UnequipWeapon

```csharp
230:         public void UnequipWeapon(EquipSlot slot)
```
- Desequipa arma de un slot

```csharp
231:         {
```

```csharp
232:             WeaponItemData weapon = slot == EquipSlot.Primary ? primaryWeapon : secondaryWeapon;
```
- **Operador ternario** (`? :`)
- Si `slot == Primary` → `weapon = primaryWeapon`
- Si no → `weapon = secondaryWeapon`
- Equivalente a:
  ```csharp
  WeaponItemData weapon;
  if (slot == EquipSlot.Primary)
      weapon = primaryWeapon;
  else
      weapon = secondaryWeapon;
  ```

```csharp
234:             if (weapon == null)
```
- ¿El slot ya está vacío?

```csharp
235:                 return;
```
- No hay nada que desequipar

```csharp
237:             weapon.Unequip(gameObject);
```
- Llama método `Unequip()` del ScriptableObject
- Puede destruir el prefab instanciado del arma

```csharp
239:             if (slot == EquipSlot.Primary)
```

```csharp
240:                 primaryWeapon = null;
```
- Vacía el slot primario

```csharp
241:             else
```

```csharp
242:                 secondaryWeapon = null;
```
- Vacía el slot secundario

```csharp
244:             OnWeaponUnequipped?.Invoke(slot);
```
- Dispara evento con el slot que se vació

```csharp
245:             Debug.Log($"<color=orange>[INVENTORY] Unequipped weapon from {slot} slot</color>");
```

```csharp
246:         }
```

---

## 🔄 Líneas 248-260: Método SwapWeapons

```csharp
248:         public void SwapWeapons()
```
- Intercambia Primary ↔ Secondary
- Usado típicamente con tecla "Q"

```csharp
249:         {
```

```csharp
250:             WeaponItemData temp = primaryWeapon;
```
- Guarda Primary en variable temporal
- Técnica clásica de swap

```csharp
251:             primaryWeapon = secondaryWeapon;
```
- Primary ahora tiene lo que estaba en Secondary

```csharp
252:             secondaryWeapon = temp;
```
- Secondary ahora tiene lo que estaba en Primary (guardado en temp)
- **Swap completo**

```csharp
254:             if (primaryWeapon != null)
```
- ¿Hay arma en Primary después del swap?

```csharp
255:                 OnWeaponEquipped?.Invoke(EquipSlot.Primary, primaryWeapon);
```
- Dispara evento para actualizar UI

```csharp
256:             if (secondaryWeapon != null)
```

```csharp
257:                 OnWeaponEquipped?.Invoke(EquipSlot.Secondary, secondaryWeapon);
```

```csharp
259:             Debug.Log("<color=cyan>[INVENTORY] Swapped weapons</color>");
```

```csharp
260:         }
```

---

## 🔍 Líneas 262-265: Método GetEquippedWeapon

```csharp
262:         public WeaponItemData GetEquippedWeapon(EquipSlot slot)
```
- Getter para obtener arma equipada
- Retorna `WeaponItemData` o `null`

```csharp
263:         {
```

```csharp
264:             return slot == EquipSlot.Primary ? primaryWeapon : secondaryWeapon;
```
- Operador ternario de una línea
- Retorna directamente

```csharp
265:         }
```

---

## 🔫 Líneas 267-275: Método AddAmmo

```csharp
267:         public void AddAmmo(AmmoType type, int amount)
```
- Añade munición al inventario
- `AmmoType type`: Pistol_9mm, Shotgun_Shell, etc.
- `int amount`: Cantidad a añadir

```csharp
268:         {
```

```csharp
269:             if (type == AmmoType.None)
```
- `AmmoType.None`: Tipo especial para armas sin munición
- Ejemplo: Armas cuerpo a cuerpo

```csharp
270:                 return;
```
- No hace nada si es `None`

---

**Continúa en:** InventorySystem_Lineas_271-311.md
