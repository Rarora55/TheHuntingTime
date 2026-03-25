##Reglas del proyecto: Tareas y errores 
##Rol/Perfil del Agente##

Actúa como un **Ingeniero Senior de Unity**. Tu objetivo arreglar y ajustar ciertas freatures que tenemos aplicadas actualmente el juego. .

## Stack Técnico
- **Engine:** Unity (2D Core).
- **Componentes Clave:** Stamine, Shoot anim
- **Estructura de Datos:** 

## Especificaciones ##

### 1. Tarea 1: Problemas con el estado de Tired. 

Actualmente tenemos el sistema de saltos ajustado a número de éstos. 
- **Dejar terminar la acción:** Actualmente cuando saltamos y agotamos nuestra estamina, la acción se corta automáticamente y pasa a estado de cansancio. Crea un gap al agotar la estamina para que pueda terminar la acción de forma completa; ejemplo "Cuando ejecuta un salto con los últimos porcentajes de estamina de uso, no pasar a animación de cansancio justo después y dejar 3 segundos de gap".

### 2. Tarea 2: Revisar sistema de disparo. 

Cuando el jugador dispara, 9 de cada 10 queda bloqueado en la animación de disparo en forma infinita hasta que pulsemos salto:
1. **Comportamiento deseado:**El jugador dispara mientras se cumpla el flujo aim-shot. Si el jugado deja de pulsar el input de aim, debe dejar de disparar. 
2. **Dejar de bloquear:** Analiza el sistema y encuentra el fallo de quedarse bloqueado en animación fire. 

### 3. Tarea 3: Dusk Light
Actualmente tenemos un sistema de luces en el atardecer que hace mover una luz de una posición A-B-C
- **Deshacer:** Elimina esa luz de movimiento y sus puntos A, center y B
 - **Nuevo Sistema:** He añadido una luz global "DuskLight". Actualmente tiene intensidad 0. Con el sistema de atardecer que tenemos, cuando entremos en estado Dusk, genera una degradado con la luz que cambie la intensidad de ésta de 0 a 4 durante su ejecución hasta el 20% del Dusk y de 4 a 0 a partir de la ejecución desde el 80% al 100% antes de pasar al ciclo de noche. 


## Prohibiciones y Restricciones
- **Sigue patrones de diseño SOLID :** Sigue como hasta ahora todos los patrones establecidos. 
- **No Singletons:** No podemos meter ningún singleton.
- **Seguir estructura de SO:** Todo se tiene que poder guardar en SO's y ser editables en unity por los diseñadores.
- **No cambios en el sistema actual:** Si debes hacer cambios que afecten a la arquitectura del juego, pregúntame primero.
