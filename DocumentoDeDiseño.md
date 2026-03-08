# FruitWood - Documento de Diseño

## 1. Concepto General

**FruitWood** (Bosque de Frutas) es un juego de plataformas 2D en el que el jugador debe atravesar un bosque lleno de peligros, recoger frutas y llegar al portal de salida. El nivel está diseñado para completarse en 1-3 minutos.

- **Género**: Plataformas 2D
- **Motor**: Unity 6 (6000.0.69f1)
- **Condición de victoria**: Llegar al portal del final del nivel
- **Condición de derrota**: Perder las 3 vidas

---

## 2. Mecánicas del Jugador

| Mecánica | Descripción |
|----------|-------------|
| **Movimiento** | Horizontal con A/D o flechas, velocidad 7 |
| **Salto** | Tecla Espacio, fuerza 14, con fall multiplier para mejor sensación |
| **Salto variable** | Soltar Espacio corta el salto (low jump multiplier = 2) |
| **Vidas** | 3 vidas, con invencibilidad temporal (1.5s) tras recibir daño |
| **Knockback** | El jugador es empujado hacia atrás al recibir daño |
| **Stomp** | Saltar encima de enemigos los derrota, con rebote automático |
| **Detección de suelo** | BoxCast desde la parte inferior del collider, ignora al propio jugador |
| **Flip** | El sprite se voltea automáticamente según la dirección de movimiento |

---

## 3. Sistema de Animaciones

El juego utiliza un **sistema de animación basado en sprites** (sprite-swapping), sin el sistema Animator de Unity.

### 3.1 Animaciones del Jugador (`PlayerAnimator`)

| Estado | Sprites | FPS |
|--------|---------|-----|
| **Idle** | 6 frames (índices 0-5) | 6 |
| **Run** | 8 frames (índices 12-19) | 10 |
| **Jump** | 3 frames (índices 20-22) | 8 |
| **Fall** | 3 frames (índices 23-25) | 8 |
| **Attack** | 6 frames (índices 6-11) | 12 |
| **Hit** | 3 frames (índices 38-40) | 8 |
| **Die** | 7 frames (índices 41-47) | 8 |

- Los sprites se asignan manualmente en el Inspector desde el spritesheet `char_blue`.
- Las animaciones de Hit y Die tienen **lock temporal** (no se interrumpen).
- Al morir, se queda en el último frame de la animación.

### 3.2 Animaciones de Enemigos (`EnemyAnimator`)

| Estado | Sprites | FPS |
|--------|---------|-----|
| **Idle** | Variable | 6 |
| **Run** | Variable | 10 |
| **Hit** | Variable | 8 |
| **Die** | Variable | 10 |

- Sprites del **Mushroom** cargados desde la carpeta `Sprites/Mushroom without VFX/`.
- El estado se determina automáticamente por la velocidad del Rigidbody2D.
- Animaciones Hit y Die con lock temporal, igual que el jugador.

---

## 4. Enemigos y Obstáculos

| Elemento | Comportamiento |
|----------|---------------|
| **Mushroom (Patrullero)** | Se mueve entre dos puntos con Rigidbody2D. Detecta bordes y paredes para girar. Daña por contacto lateral. Muere si el jugador salta encima (+100 pts). Animaciones: Idle, Run, Hit, Die |
| **Pinchos** | Zona de daño estática. Daña al jugador por contacto |
| **Plataformas móviles** | Se mueven entre dos puntos. El jugador se mueve con ellas |
| **KillZone (Vacío)** | Caer fuera del nivel = perder vida y respawn |

---

## 5. Sistema de Puntuación

- **Frutas**: +10 puntos cada una (con efecto de recolección)
- **Enemigos**: +100 puntos por stomp
- **Temporizador**: Se muestra el tiempo total al completar

---

## 6. Escenas del Juego

| Escena | Contenido |
|--------|-----------|
| **MainMenu** | Título del juego, botón Jugar, botón Salir |
| **GameLevel** | Nivel jugable completo con HUD |
| **GameOver** | Texto "Game Over", puntuación final, botones Reintentar/Menú |
| **WinScreen** | Texto "¡Nivel Completado!", puntuación y tiempo, botones Menú/Rejugar |

---

## 7. Interfaz de Usuario (HUD)

- **Esquina superior izquierda**: Vidas (corazones o "x 3")
- **Esquina superior derecha**: Puntuación ("Score: 150")
- **Centro superior**: Temporizador ("01:23")

### Scripts de UI:

| Script | Función |
|--------|---------|
| `GameUI` | Muestra vidas, puntuación y temporizador durante el juego |
| `MainMenuUI` | Controla el menú principal (Jugar / Salir) |
| `GameOverUI` | Pantalla de Game Over con puntuación y botones |
| `WinScreenUI` | Pantalla de victoria con puntuación, tiempo y botones |

---

## 8. Audio

| Evento | Sonido |
|--------|--------|
| Salto | Tono ascendente rápido |
| Recoger cristal | Tono agudo brillante |
| Recibir daño | Tono grave descendente |
| Muerte | Tono largo descendente |
| Pisar enemigo | Tono medio rápido |
| Completar nivel | Tono ascendente largo |

Los sonidos se generan proceduralmente por código como barridos de frecuencia estilo retro (`AudioManager`). Se pueden reemplazar por archivos .wav/.ogg asignándolos en el Inspector.

---

## 9. Arquitectura de Scripts

### 9.1 Core (Gameplay)

| Script | Responsabilidad |
|--------|----------------|
| `PlayerController` | Movimiento, salto, detección de suelo, flip |
| `PlayerHealth` | Sistema de vidas, invencibilidad temporal, knockback, daño |
| `PlayerAnimator` | Animaciones del jugador por sprite-swapping con auto-carga |
| `EnemyPatrol` | IA de patrulla, detección de bordes/paredes, stomp, muerte |
| `EnemyAnimator` | Animaciones de enemigos por sprite-swapping |
| `GameManager` | Singleton. Gestión de estado, vidas, puntuación, escenas |
| `AudioManager` | Singleton. Audio procedural con DontDestroyOnLoad |
| `CameraFollow` | Seguimiento suave del jugador |

### 9.2 Elementos de Nivel

| Script | Responsabilidad |
|--------|----------------|
| `Collectible` | Frutas recogibles (+10 pts) |
| `Hazard` | Zonas de daño (pinchos) |
| `KillZone` | Vacío / zonas de muerte instantánea |
| `MovingPlatform` | Plataformas que se mueven entre dos puntos |
| `Checkpoint` | Puntos de respawn intermedios |
| `LevelEnd` | Portal de fin de nivel |


---

## 10. Assets Gráficos

| Carpeta | Contenido |
|---------|-----------|
| `Sprites/character/` | Spritesheet del personaje (`char_blue`) |
| `Sprites/Mushroom without VFX/` | Sprites del enemigo Mushroom (Idle, Run, Hit, Die) |
| `Sprites/Items/` | Sprites de frutas y objetos recogibles |
| `Sprites/HU/` | Elementos de interfaz (HUD) |
| `Sprites/background/` | Fondos del nivel |
| `Sprites/decorations/` | Decoraciones ambientales |
| `Tilemaps/` | Tiles y paletas para construcción de niveles |

---

## 11. Decisiones de Diseño

1. **Fall Multiplier**: Caída más rápida que la subida del salto para mejor game feel (estilo Mario/Celeste)
2. **Salto variable**: Soltar Espacio reduce la altura del salto, dando más control al jugador
3. **Invencibilidad temporal**: 1.5 segundos con parpadeo visual, evita muertes injustas por múltiples golpes
4. **Checkpoints**: Un checkpoint a mitad de nivel para reducir frustración
5. **Stomp mechanic**: Recompensar al jugador por engagement activo con enemigos (+100 pts) con rebote automático
6. **Audio procedural**: Sonidos generados por código aseguran que el proyecto es autocontenido sin archivos externos obligatorios
7. **Singleton managers**: GameManager y AudioManager persisten entre escenas con DontDestroyOnLoad
8. **Animaciones por sprite-swapping**: Sistema propio en lugar del Animator de Unity, más sencillo y sin archivos de animación externos
9. **Detección de suelo con BoxCast**: Más fiable que OverlapCircle, no requiere objetos hijos
10. **Movimiento de enemigos con Rigidbody2D**: Más estable que Transform.Translate, permite interacción física correcta
