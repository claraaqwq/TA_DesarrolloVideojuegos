# Descent to Erebus — Contexto completo para Codex

> Fuente: GDD del proyecto académico. Este archivo resume **todo el contenido funcional, narrativo y técnico** del documento original para que Codex pueda revisar el repositorio y determinar qué falta para la entrega final.

---



## Estado conocido antes de la auditoría

El equipo confirma que **el movimiento del jugador ya está implementado**.

Codex no debe rehacerlo desde cero. Primero debe revisar:

- Qué scripts controlan el movimiento.
- Si incluye solo desplazamiento y salto o también Dash, Blink, caída rápida y Hilo.
- Si está conectado al prefab y a las escenas.
- Si las referencias del Inspector están asignadas.
- Si funciona sin errores.
- Si usa correctamente Rigidbody2D, Collider2D e Input System.

El movimiento base debe registrarse inicialmente como **existente, pendiente de verificación**, no como ausente.

La prioridad inmediata comienza después del movimiento:

1. Verificar y estabilizar el controlador existente.
2. Salud, daño, muerte y reinicio.
3. Dos mecánicas adicionales funcionales.
4. HUD.
5. Nivel corto jugable.
6. Hades como jefe final.
7. Victoria y derrota.
8. Animator, partículas, audio y presentación.
9. Build final probado.

No modificar el controlador del jugador salvo que exista un error concreto o falte integración necesaria para la rúbrica.

---

## 0. Prioridad real de la entrega final: cumplir la rúbrica

El GDD contiene la visión completa del juego, pero **la implementación debe priorizar la rúbrica del examen final**. El contenido del GDD que no contribuya directamente a la evaluación se considera alcance secundario.

La rúbrica suma **20 puntos**, distribuidos en cinco categorías de 4 puntos:

| Categoría | Requisito principal | Puntaje |
|---|---|---:|
| A. Funcionalidad general | Juego ejecutable de inicio a fin, estable y con objetivo claro | 4 |
| B. Jugabilidad | Dos o más mecánicas adicionales al movimiento que afecten la experiencia | 4 |
| C. Temática y jefe final | Uso correcto de al menos tres sistemas de Unity y jefe final completo e integrado | 4 |
| D. Diseño visual | Estética coherente, UI legible, ambientación y efectos claros | 4 |
| E. Integración de sistemas | Varios sistemas conectados: salud, enemigos/IA, nivel, UI, etc. | 4 |

### Interpretación de alcance

La rúbrica **no exige explícitamente implementar los tres niveles descritos en el GDD**. Sí exige una experiencia completa y estable, al menos dos mecánicas adicionales, tres sistemas de Unity, integración técnica y un jefe final funcional.

Por eso, si actualmente no existen niveles ni enemigos, la estrategia recomendada es construir primero **un nivel final corto pero completo**, ambientado en el Trono del Érebo, que termine con el combate contra Hades.

Un solo nivel bien terminado puede demostrar mejor la rúbrica que tres niveles incompletos. Los niveles de Creta y Aqueronte, junto con Asterión y Caronte, pasan a ser contenido de expansión si queda tiempo.

### Alcance mínimo recomendado para apuntar a la rúbrica completa

El flujo mínimo jugable debe ser:

`Menú o inicio → sección corta de plataformas → interacción/recolección → arena de Hades → victoria o derrota → reinicio/salida`

Debe incluir:

1. Un objetivo comprensible: llegar hasta Hades y entregarle el mensaje.
2. Movimiento y salto estables.
3. Al menos dos mecánicas adicionales que realmente se usen:
   - Dash;
   - Blink;
   - Hilo de Ariadna.
4. Salud del jugador, daño, muerte y respawn o reinicio.
5. Hades como jefe final con comportamiento completo.
6. Condición de victoria al derrotar o superar a Hades.
7. Condición de derrota cuando el jugador pierde toda su vida.
8. UI de vida y estado del jefe.
9. Física 2D.
10. Animator en el jugador, jefe o ambos.
11. Partículas o efectos visuales.
12. Una presentación visual coherente.
13. Build de PC probado de principio a fin.

### Qué significa “jefe final completo”

Hades no cuenta como completo solo por existir como sprite o GameObject. Debe tener:

- Aparición o inicio claro del combate.
- Vida o condición de progreso.
- Al menos dos patrones de ataque distinguibles.
- Detección de daño.
- Daño al jugador.
- Estados básicos: espera/inicio, ataque, daño y derrota.
- UI o feedback que comunique su estado.
- Integración con las mecánicas del jugador.
- Condición de victoria.
- Transición a una pantalla o estado final.
- Funcionamiento sin errores críticos.

Una implementación razonable y viable puede usar una máquina de estados simple:

`Intro → Ataque 1 → Ataque 2 → Vulnerable → repetir → Derrotado`

El jefe no necesita reproducir toda la secuencia ambiciosa del GDD si el tiempo es limitado. Es preferible un Hades simplificado, estable y bien integrado.

### Diseño sugerido del combate para aprovechar las mecánicas

- **Ataque 1: proyectiles o sombras horizontales.** El jugador usa salto o Blink.
- **Ataque 2: zonas de lava o pilares de fuego.** El jugador usa Dash para cruzarlas.
- **Fase vulnerable:** Hades queda expuesto después de atacar.
- **Hilo de Ariadna:** puede actuar como retorno/checkpoint dentro de la arena o como mecánica para evitar una caída.
- Después de reducir la vida del jefe, se activa la entrega del pergamino y la victoria.

Esto permite que el jefe demuestre jugabilidad, física, UI, IA/estados, salud, partículas y temática en una sola escena.

### Tres sistemas de Unity que deben quedar evidentes

Usar como mínimo tres, aunque es recomendable mostrar cuatro:

1. **Físicas 2D:** Rigidbody2D, Collider2D, triggers y detección de suelo/daño.
2. **UI:** vida del jugador, barra de Hades, cooldowns y pantallas de victoria/derrota.
3. **Animator:** animaciones del jugador o del jefe y transición entre estados.
4. **Partículas:** Dash, Blink, daño, ataques de Hades o derrota.

Los sistemas deben estar conectados al gameplay. No basta con agregarlos sin uso visible.

### Integración técnica mínima

Para la categoría de complejidad, los sistemas deben relacionarse entre sí:

- El jugador recibe daño de un ataque del jefe.
- `PlayerHealth` actualiza el HUD.
- Al llegar a cero se activa la derrota o el respawn.
- El jugador daña a Hades durante una ventana vulnerable.
- `BossHealth` actualiza la barra del jefe.
- Al llegar a cero se detiene la IA, se reproduce la derrota y se activa la victoria.
- Dash/Blink/Hilo tienen cooldown o estado visible.
- El nivel bloquea la salida mientras el combate está activo.
- La puerta o transición se habilita al terminar el combate.

### Prioridades según la rúbrica

#### P0 — Bloqueantes

- Verificar que el movimiento existente funciona y está correctamente conectado.
- El proyecto abre y compila sin errores.
- Existe una escena configurada en Build Profiles.
- Se puede iniciar y finalizar una partida.
- No hay referencias faltantes que rompan el flujo.
- El build de Windows funciona.

#### P1 — Mayor retorno de puntaje

- Un nivel corto y jugable.
- Hades funcional e integrado.
- Vida, daño, derrota y victoria.
- Dos mecánicas adicionales al movimiento.
- Física 2D, UI y Animator.
- Integración entre jugador, jefe y HUD.

#### P2 — Presentación

- Sprites y escenario coherentes.
- Interfaz legible.
- Partículas.
- Sonidos.
- Música.
- Pantallas de inicio, victoria y derrota.
- Feedback claro al recibir daño, usar habilidades y golpear al jefe.

#### P3 — Expansión del GDD

Solo después de asegurar lo anterior:

- Enemigos comunes.
- Nivel 1 completo.
- Asterión.
- Nivel 2 completo.
- Caronte.
- Nivel 3 más extenso.
- Medusa.
- Consecuencias del Tártaro.
- Secretos y coleccionables adicionales.

### Matriz de verificación para Codex

Codex debe auditar el repositorio usando esta tabla como criterio principal:

| Rúbrica | Evidencia necesaria | Estado |
|---|---|---|
| A - Inicio a fin | Inicio, gameplay, victoria/derrota y build estable | Completo / Parcial / Ausente |
| B - Dos mecánicas | Dos de Dash, Blink, Hilo, recolección o puzzle conectadas al nivel | Completo / Parcial / Ausente |
| C - 3 sistemas Unity | Física + UI + Animator, preferiblemente partículas | Completo / Parcial / Ausente |
| C - Jefe final | Hades con patrones, daño, estados y final | Completo / Parcial / Ausente |
| D - Visual | Arte coherente, UI legible, ambientación y efectos | Completo / Parcial / Ausente |
| E - Integración | Salud, HUD, jefe/IA, nivel y habilidades conectados | Completo / Parcial / Ausente |

---

## 1. Datos generales

- **Nombre:** Descent to Erebus
- **Curso:** INF 309 — Desarrollo de Videojuegos
- **Motor:** Unity
- **Versión usada por el proyecto:** Unity 6, `6000.0.41f1`
- **Plataforma objetivo:** PC
- **Género:** Plataformas 2D / acción y exploración
- **Perspectiva:** 2D lateral, side-scroller
- **Temática:** Mitología griega
- **Duración mínima esperada:** 10 minutos por partida completa
- **Estructura:** 3 niveles principales + jefe final
- **Estado del GDD:** inicialmente preparado para el examen parcial, pero ahora debe servir como base para la entrega final

---

## 2. Visión general

El jugador controla a **Hermes**, mensajero de los dioses. Zeus le entrega un mensaje urgente que debe llevar hasta Hades para evitar una guerra entre el Olimpo y el Inframundo.

Durante el descenso, el pergamino se fragmenta en varios **Sellos Divinos**. Cada nivel contiene uno. Al recuperar cada sello, Hermes obtiene una nueva habilidad necesaria para superar el siguiente ecosistema y avanzar hasta el Trono del Érebo.

El juego combina:

- Plataformas de precisión.
- Exploración.
- Habilidades de movilidad.
- Interacciones con el entorno.
- Combates y jefes.
- Progresión de habilidades por nivel.
- Elementos narrativos inspirados en la mitología griega.

---

## 3. Referencias principales

### Hollow Knight
Referencia para:

- Dirección artística oscura.
- Exploración de zonas subterráneas.
- Sensación de mundo hostil.
- Movimiento y atmósfera general.

### Celeste
Referencia para:

- Fluidez del movimiento.
- Dash aéreo.
- Plataformas de precisión.
- Progresión de mecánicas introducidas una por una.

### Hades
Referencia para:

- Mitología griega.
- Narrativa.
- Personajes del Inframundo.
- Descenso hacia el reino de Hades.

### Dead Cells
Referencia para:

- Trampas.
- Aprendizaje mediante la muerte.
- Consecuencias al fallar.
- Retroalimentación tras morir.

---

## 4. Historia completa

Zeus le encarga a Hermes entregar un mensaje urgente a Hades. El objetivo es evitar una guerra inminente entre los dioses del Olimpo y las fuerzas del Inframundo.

Al comenzar el viaje, el pergamino se fragmenta en Sellos Divinos que quedan dispersos por el mundo. Hermes debe recuperar uno en cada nivel.

Cada sello desbloquea una habilidad nueva:

1. El jugador comienza con las Sandalias Aladas y el Dash aéreo.
2. Tras vencer a Asterión, obtiene el Hilo de Ariadna.
3. En Aqueronte obtiene el Blink o Manto de Sombras.
4. Puede encontrar de forma opcional la Mirada de Piedra de Medusa.
5. Finalmente combina todas las habilidades para llegar hasta Hades y entregarle el mensaje.

El enfrentamiento final no busca matar a Hades, sino atravesar sus defensas y hacerle llegar el mensaje de Zeus, lo que detiene el conflicto.

---

## 5. Controles

| Acción | Entrada | Descripción |
|---|---|---|
| Mover izquierda/derecha | `A / D` o `← / →` | Movimiento horizontal fluido |
| Saltar | `Espacio`, `W` o `↑` | Salto estándar con altura variable |
| Caída rápida | `S` o `↓` en el aire | Aumenta la velocidad de caída |
| Dash aéreo | `C` | Impulso horizontal rápido en el aire |
| Blink | `E` | Teletransporte corto que atraviesa obstáculos permitidos |
| Hilo de Ariadna | `Q` | Coloca un anclaje o regresa al anclaje activo |

---

## 6. Mecánicas principales

### 6.1 Movimiento horizontal

- Desplazamiento lateral.
- Velocidad inicial propuesta: **6 unidades por segundo**.
- Debe sentirse fluido y controlable.
- El valor debe ser configurable desde el Inspector.

### 6.2 Salto

- Altura inicial propuesta: **5 unidades**.
- Salto variable:
  - mantener el botón produce un salto más alto;
  - soltarlo antes reduce la altura.
- Debe poder combinarse con Dash y Blink.
- Debe detectar correctamente si el jugador está en el suelo.

### 6.3 Caída rápida

- Solo funciona mientras el personaje está en el aire.
- Se activa con `S` o flecha abajo.
- Aumenta la velocidad vertical hacia abajo.
- Debe ayudar al jugador a corregir la trayectoria.

### 6.4 Dash aéreo

- Habilidad inicial del jugador.
- Representa las Sandalias Aladas.
- Impulso horizontal rápido.
- Distancia inicial: **3 unidades**.
- Cooldown inicial: **0.8 segundos**.
- Se recarga al tocar el suelo.
- Debe tener feedback visual.
- Debe poder calibrarse desde el Inspector.
- Se utiliza especialmente en el Nivel 1.

### 6.5 Blink

- Teletransporte corto.
- Distancia inicial: **2.5 unidades**.
- Cooldown inicial: **1.5 segundos**.
- Permite atravesar ciertos obstáculos físicos.
- No debe ignorar todas las colisiones: solo las capas u objetos marcados como atravesables.
- Debe validar que el destino sea seguro.
- Debe incluir un efecto visual de distorsión o sombra.
- Se utiliza principalmente en el Nivel 2.

### 6.6 Hilo de Ariadna

Es la mecánica diferencial del juego.

#### Modo checkpoint

- El jugador coloca un marcador luminoso.
- Guarda la posición del jugador.
- Puede guardar la vida actual.
- Al morir, el jugador regresa al último anclaje válido.

#### Modo rebobinado

- Permite regresar al anclaje antes de morir.
- Puede utilizarse cuando el jugador falla un salto o cae.
- El retorno puede consumir una carga.
- La cantidad de cargas debe poder configurarse.

#### Modo combate

- Durante el combate contra Caronte, el Hilo puede generar un anclaje holográfico.
- El jefe ataca la posición falsa.
- Esto permite engañarlo y avanzar.

#### Implementación esperada

- `Raycast2D` o validación equivalente para comprobar que el anclaje sea válido.
- `LineRenderer` para visualizar el hilo.
- Guardar al menos:
  - posición;
  - vida;
  - estado del anclaje.
- Mostrar visualmente si el anclaje está activo.
- Evitar colocar el anclaje en paredes, trampas o espacios inválidos.

### 6.7 Mirada de Piedra de Medusa

- Habilidad opcional.
- Se encuentra en una gruta secreta.
- No es obligatoria para completar el juego.
- Puede congelar o petrificar temporalmente ciertos elementos.
- En el diseño final se usa para congelar una plataforma o petrificar la guadaña de Hades.
- Es un elemento secundario respecto al MVP, pero forma parte del diseño final.

---

## 7. Progresión de habilidades

| Momento | Habilidad | Obtención |
|---|---|---|
| Tutorial / inicio | Dash aéreo | Disponible desde el comienzo |
| Final del Nivel 1 | Hilo de Ariadna | Al derrotar a Asterión y absorber el Primer Sello |
| Inicio o desarrollo del Nivel 2 | Blink | Segundo Sello Divino / Manto de Sombras |
| Secreto del Nivel 2 o Nivel 3 | Mirada de Piedra | Reliquia oculta de Medusa |
| Nivel 3 | Sinergia total | Combinación de todas las habilidades |

La progresión debe enseñar cada habilidad antes de exigir su uso en situaciones difíciles.

---

## 8. Sistema de salud

### 8.1 Funcionamiento

- La salud se representa mediante corazones o una barra.
- El jugador pierde vida por:
  - enemigos;
  - trampas;
  - precipicios;
  - peligros del entorno.
- La vida no se regenera automáticamente.
- Al llegar a cero, el jugador muere.
- Después de morir, reaparece en el último checkpoint o anclaje válido.

### 8.2 Objetos de curación

| Objeto | Curación | Uso |
|---|---:|---|
| Ambrosía Fragmentada | 25% | Enemigos o rincones del mapa |
| Néctar de los Dioses | 50% | Cofres ocultos o zonas difíciles |
| Llama del Olimpo | 100% | Una por nivel, en desafío opcional |

### 8.3 Feedback al curarse

Debe incluir, como mínimo:

- Actualización visible del HUD.
- Pulso dorado o cambio visual en la barra de vida.
- Partículas o destello.
- Sonido de recompensa, si existe audio implementado.

### 8.4 Muerte

Al morir:

1. Se desactiva temporalmente el control.
2. Se reproduce feedback visual o animación.
3. El jugador vuelve al checkpoint/anclaje.
4. La vida se restaura según las reglas del proyecto.
5. La escena no debe quedar bloqueada.

### 8.5 Consecuencias del Tártaro

Sistema opcional para el Nivel 3:

- Después de morir, el jugador puede recibir una pequeña mejora.
- Ejemplo: cooldown del Dash reducido.
- A cambio, el mapa agrega más trampas.
- La dificultad aumenta progresivamente.
- Debe tratarse como contenido avanzado, posterior a los sistemas base.

---

## 9. Estructura completa de niveles

### 9.1 Nivel 1 — La Caída a Creta

#### Ambiente

- Ruinas del mundo mortal.
- Precipicios.
- Laberinto del Minotauro.
- Tonos cálidos, marrones y rojizos.

#### Mecánica central

- Dash aéreo.
- El nivel enseña a cruzar precipicios.
- El jugador aprende distancia, cooldown y control del Dash.

#### Enemigos

- Guardianes del laberinto.
- Criaturas menores del mundo mortal.

#### Interacciones

- Palancas.
- Puertas.
- Columnas derrumbables.
- Plataformas.
- Obstáculos que obligan a usar el Dash.

#### Jefe: Asterión, el Minotauro

##### Fase 1

- Persigue al jugador.
- El laberinto cambia o se reconfigura.
- El jugador usa el Dash para escapar de las embestidas.

##### Fase 2

- Asterión destruye el suelo.
- El combate continúa en una arena.
- El jugador debe provocar que el jefe embista pilares de carga.

##### Fase 3

- Asterión queda aturdido.
- Hermes absorbe el Primer Sello Divino.
- Se desbloquea el Hilo de Ariadna.
- El jugador debe usarlo antes de que el suelo colapse.

---

### 9.2 Nivel 2 — Las Puertas de Aqueronte

#### Ambiente

- Cavernas oscuras.
- Cascadas de ácido.
- Barrotes de piedra.
- Transición hacia el Hades.

#### Mecánica central

- Blink.
- Combinación de Dash + Blink.
- Atravesar obstáculos.
- Resolver situaciones con más de una habilidad.

#### Enemigos

- Almas errantes.
- Guardianes del río Aqueronte.

#### Interacciones

- Barrotes atravesables.
- Palancas.
- Mecanismos.
- Uso combinado del Hilo de Ariadna.
- Plataformas o escombros móviles.

#### Secreto

- Gruta oculta.
- Reliquia de la Mirada de Piedra de Medusa.
- Contenido opcional.

#### Jefe: Caronte

##### Fase 1

- Lanza olas de almas.
- El jugador usa Blink para atravesarlas.

##### Fase 2

- Caronte rompe la balsa.
- Hermes debe usar Dash para moverse sobre escombros que se hunden.

##### Fase 3

- El jugador usa el Hilo de Ariadna.
- Crea un anclaje holográfico.
- Engaña a Caronte.
- Caronte queda atrapado o atascado.
- Se abre el acceso al Inframundo profundo.

---

### 9.3 Nivel 3 — El Trono del Érebo

#### Ambiente

- Lava.
- Oscuridad opresiva.
- Plataformas temporales.
- Elementos que aparecen y desaparecen.

#### Mecánica central

Sinergia de habilidades:

1. Salto.
2. Dash.
3. Mirada de Piedra opcional.
4. Blink.
5. Hilo de Ariadna.

Ejemplo planteado:

`salto → Dash → congelar plataforma → Blink a través de un muro`

#### Enemigos

- Sombras del Tártaro.
- Proyecciones espectrales.

#### Sistema especial

- Consecuencias del Tártaro.
- La muerte puede modificar el mapa y la dificultad.

#### Jefe final: Hades

##### Fase 1 — Prueba de movilidad

- Hades cubre el suelo con lava.
- El jugador debe mantenerse en movimiento.
- Usa Dash y Blink.
- Las plataformas aparecen por pocos segundos.

##### Fase 2 — El Castigo

- Hades invoca proyecciones de Cerbero y Asterión.
- El jugador usa el Hilo de Ariadna para crear barreras o señuelos temporales.

##### Resolución final

Hades crea un escudo y prepara un ataque de pantalla completa.

La secuencia planteada es:

1. Petrificar su guadaña con Medusa.
2. Subirse a la hoja usando Dash.
3. Dejar un anclaje con el Hilo.
4. Realizar un Blink a través del escudo.
5. Entregar el pergamino directamente a Hades.
6. Hades se pacifica.
7. Termina el juego.

---

## 10. Interacciones con el entorno

| Interacción | Implementación sugerida | Feedback |
|---|---|---|
| Recoger vida | `OnTriggerEnter2D` | Partículas, HUD, sonido |
| Activar palanca | `OnTriggerStay2D` + input | Animación, luz, sonido |
| Absorber sello | `OnTriggerEnter2D` | Resplandor, texto, animación |
| Plataforma derrumbable | Collider2D + temporizador | Vibración y cambio de color |
| Atravesar barrotes | Blink + capa específica | Distorsión o parpadeo |
| Anclaje del Hilo | Trigger/Raycast2D | Línea brillante y destello |

---

## 11. Implementación técnica general

### Física

- `Rigidbody2D`
- `Collider2D`
- `OnTriggerEnter2D`
- `OnTriggerStay2D`
- Raycasts 2D cuando sea necesario.

### Capas sugeridas

- `Player`
- `Enemy`
- `Ground`
- `Collectible`
- `Hazard`
- `Blinkable`

### Reglas técnicas

- No usar un sistema 3D para mecánicas 2D.
- No desactivar indiscriminadamente todas las colisiones durante Blink.
- Validar destinos de teletransporte.
- Evitar referencias nulas.
- Exponer variables de balance con `[SerializeField]`.
- Mantener scripts pequeños y con responsabilidades claras.

---

## 12. HUD

### Elementos

#### Vida

- Corazones, llamas griegas o maná.
- Esquina superior izquierda.
- Feedback al recibir daño o curarse.

#### Dash

- Ícono de sandalia alada.
- Indicador de disponibilidad.
- Cooldown circular o cambio visual.

#### Blink

- Ícono de sombra.
- Indicador de cooldown.

#### Hilo de Ariadna

- Ícono de hilo dorado.
- Brillante si existe un anclaje.
- Opaco si no existe anclaje.

#### Medusa

- Solo aparece si se obtiene la habilidad.
- Ícono de ojo.
- Debe mostrar cooldown o disponibilidad.

---

## 13. Vertical slice / escena de pruebas

Antes de construir todo el juego, el proyecto debe tener una escena técnica donde se puedan probar todos los sistemas.

### Contenido

1. Plataformas fijas de distintos tamaños.
2. Diferentes alturas.
3. Precipicios.
4. Muros.
5. Zona de Dash.
6. Zona de Blink.
7. Zona del Hilo.
8. Objetos de vida.
9. Palanca y puerta.
10. Zona de daño.
11. Punto de respawn.

### Valores iniciales

| Variable | Valor |
|---|---:|
| Velocidad | 6 unidades/s |
| Altura de salto | 5 unidades |
| Distancia de Dash | 3 unidades |
| Cooldown de Dash | 0.8 s |
| Distancia de Blink | 2.5 unidades |
| Cooldown de Blink | 1.5 s |

Todos los valores deben ser ajustables desde el Inspector.

---

## 14. Assets y referencias visuales

### Personajes y enemigos

- Medusa / Gorgona.
- Sátiro.
- Minotauro.
- Esqueletos.
- Zeus para diálogos.

### Efectos y UI

- Efectos mágicos.
- Íconos de habilidades.
- Objetos y loot.
- Pergamino.

### Fondos

- Olimpo.
- Nubes.
- Lava.
- Inframundo.
- Cueva de Creta.
- Exterior de Creta.

### Assets opcionales

- Armas.
- Cerbero o Hellhound.

Los assets pueden provenir de Craftpix, Unity Asset Store, Spriters Resource o sitios equivalentes, siempre respetando sus licencias.

---

## 15. Alcance inicial del parcial

El parcial priorizaba:

- Movimiento.
- Salto.
- Caída.
- Colisiones.
- Dash.
- Blink.
- Hilo de Ariadna.
- Una escena vertical slice.
- Dos interacciones básicas.
- Feedback visual mínimo.

---

## 16. Alcance esperado para la entrega final

Para considerar el proyecto cercano al diseño completo del GDD, debería existir:

### Sistemas principales

- Movimiento terminado.
- Salto variable.
- Caída rápida.
- Dash funcional y con feedback.
- Blink funcional y seguro.
- Hilo de Ariadna completo.
- Vida.
- Daño.
- Curación.
- Muerte.
- Respawn.
- Checkpoints.
- HUD completo.
- Guardado mínimo de progreso o reinicio coherente.

### Diseño de niveles

- Nivel 1 jugable.
- Nivel 2 jugable.
- Nivel 3 jugable.
- Transiciones entre niveles.
- Checkpoints.
- Objetivos claros.
- Plataformas, trampas y rutas.
- Zonas opcionales o secretos.

### Interacciones

- Recolectables.
- Palancas.
- Puertas.
- Plataformas derrumbables.
- Obstáculos atravesables.
- Sellos Divinos.
- Feedback audiovisual.

### Enemigos y jefes

- Enemigos básicos.
- Daño al jugador.
- IA o patrones.
- Asterión.
- Caronte.
- Hades.
- Fases de jefe o una versión simplificada coherente.

### Presentación

- Menú principal.
- Pausa.
- Reinicio.
- Victoria.
- Derrota.
- Créditos.
- Audio.
- Música.
- Efectos.
- UI legible.
- Build ejecutable.

### Contenido opcional

- Medusa.
- Consecuencias del Tártaro.
- Secretos.
- Contenido secundario.
- Más pulido audiovisual.

---

## 17. Arquitectura sugerida

Codex debe revisar primero qué ya existe y reutilizarlo.

Posibles responsabilidades:

- `PlayerMovement`
- `PlayerJump`
- `PlayerDash`
- `PlayerBlink`
- `AriadneThread`
- `PlayerHealth`
- `PlayerDamage`
- `PlayerRespawn`
- `Checkpoint`
- `HealthPickup`
- `Lever`
- `Door`
- `CrumblingPlatform`
- `Hazard`
- `DivineSeal`
- `PlayerHUD`
- `EnemyController`
- `BossController`
- `LevelManager`
- `GameManager`
- `PauseMenu`
- `MainMenu`

No es obligatorio usar esos nombres si el repositorio ya tiene otra estructura.

---

## 18. Reglas para Codex

Antes de cambiar código:

1. Inspeccionar escenas, scripts, prefabs, capas, tags, Input System y Project Settings.
2. Identificar qué ya funciona.
3. No crear sistemas duplicados.
4. No reemplazar una arquitectura funcional sin necesidad.
5. No modificar archivos no relacionados.
6. Mantener el código entendible para un proyecto académico.
7. Usar comentarios breves.
8. Evitar sobreingeniería.
9. Verificar referencias nulas.
10. Explicar la configuración manual requerida en Unity.

---

## 19. Tarea recomendada para detectar qué falta

Usar este prompt con Codex:

> Lee este archivo y revisa todo el repositorio de Unity.  
> Compara el estado real del proyecto con cada requisito del GDD.  
> No programes todavía.  
> Entrega una auditoría con:
>
> 1. Funcionalidades completas.
> 2. Funcionalidades parciales.
> 3. Funcionalidades ausentes.
> 4. Bugs o riesgos técnicos.
> 5. Escenas, prefabs y scripts relacionados.
> 6. Configuración faltante en Unity.
> 7. Prioridad sugerida: crítica, alta, media o baja.
> 8. Plan de trabajo por etapas hasta la entrega final.
>
> No marques algo como completo solo porque existe un script: comprueba que esté conectado en una escena o prefab y que tenga una forma clara de probarse.

---

## 20. Criterio para considerar una función completa

Una función solo cuenta como completa si:

- Existe el código.
- Compila.
- Está conectada a un GameObject o prefab.
- Sus referencias del Inspector están asignadas.
- Puede probarse dentro de una escena.
- Tiene feedback suficiente para entender qué ocurrió.
- No depende de pasos manuales ocultos.
- No genera errores en consola durante el flujo normal.

---

## 21. Prioridad recomendada

La prioridad definitiva es la sección **0. Prioridad real de la entrega final: cumplir la rúbrica**.

### Crítica

- El proyecto abre, compila y genera un build funcional.
- Existe un flujo completo: inicio, gameplay y victoria/derrota.
- El movimiento y salto existentes están verificados y estables.
- Dos o más mecánicas adicionales funcionan dentro del nivel.
- Vida, daño, muerte y reinicio.
- Un nivel jugable.
- Hades como jefe final completo.
- Condición de victoria clara.
- Física 2D, UI y Animator integrados.

### Alta

- HUD del jugador y del jefe.
- Patrones y estados de Hades.
- Partículas y feedback.
- Checkpoint o Hilo de Ariadna.
- Puerta/arena bloqueada durante el combate.
- Menú, pausa y pantallas finales.
- Audio básico.
- Estética coherente.

### Media

- Enemigo común adicional.
- Nivel final más largo.
- Más patrones de Hades.
- Recolección o puzzle adicional.
- Nivel 1 y Nivel 2.
- Asterión y Caronte.

### Baja / expansión

- Tres niveles completos si el tiempo no alcanza.
- Medusa.
- Consecuencias del Tártaro.
- Secretos extensos.
- Efectos avanzados.
- Contenido no requerido por la rúbrica.

---


## 22. Prompt principal para la auditoría según la rúbrica

> Lee todo este archivo y revisa el repositorio completo de Unity.
>
> La prioridad es obtener el mayor cumplimiento posible de la rúbrica final, no implementar todo el GDD.
>
> Por ahora no programes. Primero:
>
> 1. Confirma si el proyecto compila.
> 2. Lista todas las escenas y cuáles están en Build Profiles.
> 3. Comprueba si existe un flujo jugable de inicio a fin.
> 4. Verifica cuáles mecánicas adicionales al movimiento funcionan realmente.
> 5. Comprueba si existen vida, daño, muerte, respawn, HUD y condición de victoria.
> 6. Busca enemigos o IA, pero prioriza comprobar si existe Hades como jefe final.
> 7. Revisa si el jefe está conectado a una escena, tiene ataques, recibe daño y activa el final.
> 8. Identifica el uso real de Physics2D, UI, Animator y ParticleSystem.
> 9. Evalúa presentación visual, legibilidad y feedback.
> 10. Señala errores de consola, referencias faltantes, prefabs rotos y configuración pendiente.
>
> Entrega:
>
> - Una tabla para las cinco categorías de la rúbrica.
> - Puntaje estimado actual sobre 20, justificando cada categoría.
> - Lista de funcionalidades completas, parciales y ausentes.
> - Archivos, prefabs y escenas que sirven como evidencia.
> - El camino mínimo para llegar a una entrega jugable.
> - Un backlog ordenado por P0, P1, P2 y P3.
> - Qué partes del GDD deben aplazarse para no poner en riesgo la rúbrica.
>
> No marques algo como completo solo porque existe un script. Comprueba que esté conectado, configurado y sea testeable en una escena.

---

## 23. Resultado esperado de la revisión

Codex debe terminar la auditoría con una tabla como esta:

| Área | Estado | Evidencia en el repo | Qué falta | Prioridad |
|---|---|---|---|---|
| Movimiento | Completo / Parcial / Ausente | Script, prefab, escena | Detalle concreto | Crítica/Alta/Media/Baja |
| Dash |  |  |  |  |
| Blink |  |  |  |  |
| Hilo |  |  |  |  |
| Vida |  |  |  |  |
| Nivel 1 |  |  |  |  |
| Nivel 2 |  |  |  |  |
| Nivel 3 |  |  |  |  |
| Asterión |  |  |  |  |
| Caronte |  |  |  |  |
| Hades |  |  |  |  |
| HUD |  |  |  |  |
| Menús |  |  |  |  |
| Build |  |  |  |  |

