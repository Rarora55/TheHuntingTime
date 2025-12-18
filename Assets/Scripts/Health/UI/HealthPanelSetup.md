# Configuración del Panel de Salud

## Estructura del GameObject

Para crear el panel de salud en tu Canvas, sigue estos pasos:

### 1. Crear la Jerarquía

```
InventoryCanvas (Canvas existente)
└── HealthPanel (GameObject con Image)
    ├── Background (Image - fondo oscuro)
    ├── BarContainer (GameObject)
    │   ├── DamageFillBar (Image - barra roja que se anima tras recibir daño)
    │   └── HealthFillBar (Image - barra verde principal)
    └── HealthText (TextMeshProUGUI)
```

### 2. Configuración de Componentes

#### HealthPanel
- **RectTransform**:
  - Anchors: Top-Left
  - Position: X: 20, Y: -20
  - Size: Width: 250, Height: 40
- **Image**: Color negro semi-transparente (0, 0, 0, 0.7)
- **HealthBarUI** (script): Asigna referencias

#### Background
- **RectTransform**: Stretch (fill parent)
  - Margins: 5px en todos lados
- **Image**: Color gris oscuro (0.2, 0.2, 0.2, 1)

#### BarContainer
- **RectTransform**: 
  - Anchors: Stretch
  - Left: 10, Right: -10, Top: 5, Bottom: 20

#### DamageFillBar (hijo de BarContainer)
- **RectTransform**: Stretch (fill parent)
- **Image**: 
  - Type: **Filled**
  - Fill Method: **Horizontal**
  - Fill Origin: **Left**
  - Color: Rojo transparente (0.8, 0.2, 0.2, 0.5)

#### HealthFillBar (hijo de BarContainer)
- **RectTransform**: Stretch (fill parent)
- **Image**:
  - Type: **Filled**
  - Fill Method: **Horizontal**
  - Fill Origin: **Left**
  - Color: Verde (0.2, 0.8, 0.2, 1)

#### HealthText
- **RectTransform**:
  - Anchors: Bottom stretch
  - Height: 20
  - Position Y: 10
- **TextMeshProUGUI**:
  - Text: "100 / 100"
  - Font Size: 14
  - Alignment: Center
  - Color: Blanco

### 3. Configuración del Script HealthBarUI

En el Inspector del HealthPanel, configura:

**References:**
- Health Controller: Arrastra el Player con HealthController

**UI Elements:**
- Fill Image: HealthFillBar
- Damage Fill Image: DamageFillBar
- Health Text: HealthText

**Colors:**
- Healthy Color: RGB(51, 204, 51) - Verde
- Warning Color: RGB(230, 230, 51) - Amarillo
- Critical Color: RGB(230, 51, 51) - Rojo
- Damage Color: RGB(204, 51, 51, 128) - Rojo semi-transparente

**Animation Settings:**
- Fill Animation Speed: 5
- Damage Delay Duration: 0.3
- Damage Animation Speed: 2

**Thresholds:**
- Warning Threshold: 0.5 (50%)
- Critical Threshold: 0.25 (25%)

**Effects:**
- Enable Pulse Effect: ✓
- Pulse Speed: 2
- Pulse Intensity: 0.2

## Características

### 1. **Animación Suave**
- La barra principal se anima suavemente hacia el nuevo valor
- Curva de animación configurable

### 2. **Efecto de Daño Retrasado**
- Cuando recibes daño, aparece una barra roja detrás
- La barra roja se reduce lentamente después de un delay
- Efecto visual similar a juegos modernos

### 3. **Colores Dinámicos**
- Verde: Salud alta (>50%)
- Amarillo: Salud media (25-50%)
- Rojo: Salud crítica (<25%)
- Transición suave entre colores

### 4. **Efecto de Pulso**
- Cuando la salud está crítica (<25%), la barra pulsa
- Configurable con velocidad e intensidad

### 5. **Texto de Salud**
- Muestra "Actual / Máxima"
- Se actualiza automáticamente

## Testing

Para probar el panel de salud, usa estos comandos en un script de debug:

```csharp
// Quitar 20 de daño
healthController.TakeDamage(20f);

// Curar 15 puntos
healthController.Heal(15f);

// Curación completa
healthController.HealToFull();
```

## Notas

- El panel se auto-inicializa al empezar el juego
- Se conecta automáticamente al HealthController del jugador
- Los eventos del HealthController actualizan el UI automáticamente
- Compatible con regeneración de salud
