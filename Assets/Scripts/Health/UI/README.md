# Sistema de UI de Salud - Guía Completa

## Resumen

Sistema de barra de salud animada que se conecta automáticamente al `HealthController` y proporciona feedback visual rico sobre el estado de salud del jugador.

## Características Implementadas

### 🎯 Animaciones Suaves
- **Barra principal**: Se anima suavemente hacia el nuevo valor de salud
- **Efecto de daño retrasado**: Muestra una barra roja secundaria que se reduce gradualmente después de recibir daño
- **Curvas de animación configurables**: Para ajustar la "sensación" de las animaciones

### 🎨 Feedback Visual Dinámico
- **Verde** (>50% HP): Salud saludable
- **Amarillo** (25-50% HP): Salud en advertencia
- **Rojo** (<25% HP): Salud crítica
- **Transiciones de color suaves**: Los colores se interpolan gradualmente

### ⚡ Efectos Especiales
- **Efecto de pulso**: Cuando la salud está crítica, la barra pulsa para llamar la atención
- **Animación de daño**: La barra roja "persigue" a la barra principal creando un efecto de retraso

### 📊 Información Textual
- Muestra "HP Actual / HP Máximo"
- Se actualiza automáticamente con cada cambio

## Configuración Paso a Paso

### Paso 1: Crear la Estructura UI

1. **Abre la escena** `Character.unity`
2. **En el Hierarchy**, haz clic derecho en `InventoryCanvas`
3. Selecciona `UI > Panel` y renómbralo a **HealthPanel**
4. Configura el **RectTransform** de HealthPanel:
   ```
   Anchors: Top-Left
   Pivot: (0, 1)
   Pos X: 20
   Pos Y: -20
   Width: 250
   Height: 40
   ```

### Paso 2: Crear el Fondo

1. Haz clic derecho en **HealthPanel** > `UI > Image` y renómbralo a **Background**
2. Configura su **RectTransform**:
   ```
   Anchors: Stretch (todos)
   Left: 5, Right: -5, Top: -5, Bottom: 5
   ```
3. En el componente **Image**:
   ```
   Color: (50, 50, 50, 255) - Gris oscuro
   ```

### Paso 3: Crear el Contenedor de Barras

1. Haz clic derecho en **HealthPanel** > `Create Empty` y renómbralo a **BarContainer**
2. Configura su **RectTransform**:
   ```
   Anchors: Stretch
   Left: 10, Right: -10, Top: 5, Bottom: 20
   ```

### Paso 4: Crear la Barra de Daño (Fondo Rojo)

1. Haz clic derecho en **BarContainer** > `UI > Image` y renómbralo a **DamageFillBar**
2. Configura su **RectTransform**:
   ```
   Anchors: Stretch (todos)
   Left: 0, Right: 0, Top: 0, Bottom: 0
   ```
3. En el componente **Image**:
   ```
   Image Type: Filled
   Fill Method: Horizontal
   Fill Origin: Left
   Fill Amount: 1
   Color: (204, 51, 51, 128) - Rojo semi-transparente
   ```

### Paso 5: Crear la Barra de Salud Principal

1. Haz clic derecho en **BarContainer** > `UI > Image` y renómbralo a **HealthFillBar**
2. Configura su **RectTransform** (igual que DamageFillBar):
   ```
   Anchors: Stretch (todos)
   Left: 0, Right: 0, Top: 0, Bottom: 0
   ```
3. En el componente **Image**:
   ```
   Image Type: Filled
   Fill Method: Horizontal
   Fill Origin: Left
   Fill Amount: 1
   Color: (51, 204, 51, 255) - Verde
   ```

### Paso 6: Crear el Texto de Salud

1. Haz clic derecho en **HealthPanel** > `UI > Text - TextMeshPro` y renómbralo a **HealthText**
2. Configura su **RectTransform**:
   ```
   Anchors: Bottom Stretch
   Left: 0, Right: 0
   Height: 15
   Pos Y: 8
   ```
3. En el componente **TextMeshProUGUI**:
   ```
   Text: "100 / 100"
   Font Size: 12
   Alignment: Center (horizontal y vertical)
   Color: White
   ```

### Paso 7: Añadir el Script HealthBarUI

1. Selecciona **HealthPanel** en el Hierarchy
2. En el Inspector, haz clic en **Add Component**
3. Busca y añade **HealthBarUI**
4. Configura las referencias arrastrando:
   ```
   Health Controller: (Arrastra el GameObject Player que tiene HealthController)
   Fill Image: HealthFillBar
   Damage Fill Image: DamageFillBar
   Health Text: HealthText
   ```

### Paso 8: Configurar Parámetros del Script

En el componente **HealthBarUI**:

**Colors:**
```
Healthy Color: RGB(51, 204, 51)
Warning Color: RGB(230, 230, 51)
Critical Color: RGB(230, 51, 51)
Damage Color: RGBA(204, 51, 51, 128)
```

**Animation Settings:**
```
Fill Animation Speed: 5
Damage Delay Duration: 0.3
Damage Animation Speed: 2
Fill Curve: (usar la curva por defecto EaseInOut)
```

**Thresholds:**
```
Warning Threshold: 0.5
Critical Threshold: 0.25
```

**Effects:**
```
✓ Enable Pulse Effect
Pulse Speed: 2
Pulse Intensity: 0.2
```

## Testing con HealthDebugger

### Añadir el Debugger (Opcional)

1. Selecciona el **Player** en el Hierarchy
2. Añade el componente **HealthDebugger**
3. Arrastra el **HealthController** a su campo de referencia

### Controles de Debug

Durante el Play Mode:

- **Tecla `-` (Minus)**: Quitar daño (10 HP por defecto)
- **Tecla `=` (Equals)**: Curar (15 HP por defecto)
- **Tecla `R`**: Resetear salud a máximo
- **GUI Buttons**: Aparecen en la esquina superior izquierda

### Verificación Visual

1. **Entra en Play Mode**
2. **Presiona `-`** varias veces para quitar salud
3. **Observa**:
   - La barra verde se reduce suavemente
   - Aparece una barra roja detrás que se reduce gradualmente
   - El color cambia de verde → amarillo → rojo según el porcentaje
   - Cuando llegas a <25%, la barra empieza a pulsar
4. **Presiona `=`** para curar
5. **Observa**:
   - La barra verde aumenta suavemente
   - La barra roja se ajusta inmediatamente al nuevo valor
   - El color vuelve a verde

## Estructura Final

```
InventoryCanvas
└── HealthPanel (Image + HealthBarUI)
    ├── Background (Image)
    ├── BarContainer (RectTransform)
    │   ├── DamageFillBar (Image - Filled Horizontal)
    │   └── HealthFillBar (Image - Filled Horizontal)
    └── HealthText (TextMeshProUGUI)
```

## Personalización

### Cambiar Colores

Ajusta los campos de color en el Inspector del **HealthBarUI** para cambiar el esquema de colores.

### Cambiar Velocidad de Animación

- **Fill Animation Speed**: Más alto = barra principal se mueve más rápido
- **Damage Animation Speed**: Más alto = barra de daño desaparece más rápido
- **Damage Delay Duration**: Tiempo antes de que la barra de daño empiece a reducirse

### Deshabilitar Efectos

- Desmarca **Enable Pulse Effect** para eliminar el pulso en salud crítica
- Ajusta **Pulse Speed** e **Pulse Intensity** para efectos más sutiles o dramáticos

### Cambiar Umbrales

- **Warning Threshold**: % de salud donde cambia a amarillo (por defecto 50%)
- **Critical Threshold**: % de salud donde cambia a rojo y empieza a pulsar (por defecto 25%)

## Integración con el Sistema de Salud

El `HealthBarUI` se conecta automáticamente a estos eventos del `HealthController`:

```csharp
healthController.OnHealthChanged += OnHealthChanged;
healthController.OnDeath += OnDeath;
```

No necesitas escribir código adicional. El sistema funciona automáticamente cuando:
- El jugador recibe daño
- El jugador se cura
- El jugador muere
- La salud se regenera

## Notas Técnicas

- **Performance**: Las animaciones usan `Lerp` para interpolación suave sin allocations
- **Eventos**: Se suscribe/desuscribe correctamente en `OnEnable`/`OnDisable`
- **Null Safety**: Verifica referencias antes de usarlas
- **Auto-inicialización**: Busca el HealthController automáticamente si no está asignado
- **Compatible con Time.timeScale = 0**: Usa `Time.deltaTime` para animaciones

## Troubleshooting

**La barra no se actualiza:**
- Verifica que el HealthController esté asignado
- Asegúrate de que el Player tenga el componente HealthController
- Revisa la consola por warnings

**Los colores no cambian:**
- Verifica que los umbrales estén configurados correctamente
- Asegúrate de que Fill Image esté asignada

**El efecto de daño no funciona:**
- Verifica que Damage Fill Image esté asignada
- Revisa que sea una Image de tipo Filled Horizontal

**El texto no aparece:**
- Asegúrate de tener TextMeshPro instalado
- Verifica que Health Text esté asignado
