# Plan de pantallas del sidebar (réplica de Tactical5)

> Documento de trabajo para implementar **todas las pantallas del sidebar** de Tactical Eleven,
> replicando las de `Tactical5` y adaptándolas al fútbol.
>
> **Estado:** la simulación de partido (F8b: play-by-play, cambios en vivo y 2D) queda en pausa.
> El foco pasa a completar la navegación y las pantallas del sidebar.
>
> Marca cada pantalla con `[x]` cuando cumpla el **Definition of Done**. Actualiza también la
> tabla de progreso.

---

## 1. Reglas del proyecto (no romper)

- **Una sola escena** (`MainMenu.unity`); cada pantalla es un `UIDocument` + `UIScreenController`.
- **Persistencia única en SQLite** vía `DatabaseManager.Instance`; nada de estado de juego en memoria.
- **Textos por claves** en `Resources/Localization/es.json`; nunca literales visibles.
- **Estilos**: reutilizar `UI/Styles/*.uss` y copiar el USS de Tactical5 cambiando:
  - `Assets/_TacticalFive/` → `Assets/_TacticalEleven/`.
  - `BarlowCondensed-*` → `Saira_Condensed-*` (si aparece).
- **Escena**: crear `<Pantalla>Document` con `UIDocument`, `FullScreenUI` y el controller, y
  enlazarlo al campo correspondiente de `ScreenManager`.
- **Navegación**: `ScreenManager.Instance.GoTo(GameScreen.X)`.
- **Sin namespaces**; reglas puras en clases `static` (`LeagueTable`, `SeasonObjective`, etc.).
- **Tests** EditMode en `Assets/_TacticalEleven/Tests/Editor/`.

### Definition of Done de una pantalla

- [ ] `UXML` copiado/adaptado de Tactical5 y con claves `{loc:...}`.
- [ ] `USS` copiado/adaptado (fuentes y rutas de TacticalEleven).
- [ ] `Controller : UIScreenController` (`ScreenId`, `CacheReferences`, `LoadData`, `RegisterCallbacks`, `Refresh`).
- [ ] `GameObject` en la escena y campo de `ScreenManager` enlazado.
- [ ] Textos en `es.json`; sin literales.
- [ ] Datos leídos de SQLite y acciones persistidas.
- [ ] Navegación desde/hacia el sidebar y botones de la pantalla.
- [ ] Test de humo del UXML (elementos + claves) y, si hay reglas, test puro.
- [ ] Pantalla marcada en este documento.

---

## 2. Progreso

| Fase | Contenido | Pantallas | Hechas |
|---|---|---|---:|---:|
| **S0** | Chasis común (header/sidebar, Dashboard) | 1 | 1 |
| **S1** | Competiciones: calendario, resultados, clasificación, info | 4 | 1 |
| **S2** | Plantilla: jugadores, alineación, táctica, dorsales | 4 | 0 |
| **S3** | Entrenamiento y salud: entrenamiento, cuerpo técnico, enfermería | 3 | 0 |
| **S4** | Cantera y rivalidades: filial/cantera, rivalidades | 2 | 0 |
| **S5** | Mercado: ofertas, cartera, buscador, historial | 4 | 0 |
| **S6** | Finanzas: finanzas, préstamos, patrocinadores, televisiones | 4 | 0 |
| **S7** | Club: ciudad, estadio | 2 | 0 |
| **S8** | Manager y actualidad: manager, logros, noticias, hilos | 4 | 0 |
| **S9** | Estadísticas y palmarés: stats, palmarés, récords, premios, eliminatorias | 5 | 0 |
| **Total** | | **33** | **2** |

---

## 3. S0 — Chasis común (✅ hecha)

- [x] **Dashboard** — réplica de `Tactical5/UI/Screens/Dashboard`; header, sidebar inyectada,
  tarjetas, avance de día y estadísticas de fútbol.
- [x] `SidebarController` + `UI/Resources/UI/Core/Sidebar.uxml`.
- [x] `HeaderController` + `UI/Resources/UI/Core/Header.uxml` (header común inyectado en las pantallas del sidebar).
- [x] `ScreenManager` con los 51 campos y navegación.
- [x] Textos base, tema y estilos compartidos.

---

## 4. S1 — Competiciones

**Objetivo:** calendario navegable, resultados por jornada, clasificación real y ficha de competición.

**Reglas puras nuevas/necesarias:** `LeagueTable` (ya existe), desempate `particular → DG → GF`,
zonas (Champions/Europa/Conference/descenso), formato por competición.

- [x] **Calendario** (`Calendar`)
  - Origen: `Tactical5/UI/Screens/Calendar`.
  - Implementado: rejilla mensual, navegación por meses, puntos por competición, detalle del día
    y partidos de todas las competiciones.
  - Generación realista al confirmar la pretemporada: ligas en fin de semana, **Champions martes**,
    **Europa miércoles**, **Conference jueves** y **Copa en martes/miércoles libres**.
  - Datos: `games` (`GetGamesBySeason`), `seasons`.
- [ ] **Resultados** (`Results`)
  - Origen: `Tactical5/UI/Screens/Results`.
  - Fútbol: resultados por jornada con todos los partidos; filtros por competición del día
    (mismo patrón que `GameResults`).
  - Datos: `games` jugados + `player_match_stats` para el detalle.
- [ ] **Clasificación** (`Standings`)
  - Origen: `Tactical5/UI/Screens/Standings`.
  - Fútbol: **PJ · G · E · P · GF · GC · DG · PTS**; desempate particular → DG → GF;
    zonas europeas y descenso.
  - Datos: `games` + `LeagueTable`.
- [ ] **Info competición** (`CompetitionInfo`)
  - Origen: `Tactical5/UI/Screens/InfoLeague`.
  - Fútbol: formato, plazas europeas (4 Champions + 2 Europa + 1 Conference), ascensos/descensos,
    palmarés de la competición.
  - Datos: `leagues`, `palmares`, reglas en `static`.

**Aceptación S1:** clasificación correcta tras cualquier jornada; desempates probados; calendario
navegable por meses; resultados filtrables por competición.

**Notas de calendario (S1):**
- La temporada se genera completa en `Preseason → CONTINUAR` con `DatabaseManager.GenerateFullSeason`.
- `CompetitionCalendarRules` fija los días: ligas en fin de semana; Champions martes, Europa
  miércoles y Conference jueves; Copa en martes/miércoles.
- `EuropeScheduleGenerator` genera 8 jornadas de 36 equipos (cada club, 8 rivales distintos).
- `CupScheduleGenerator` genera la **ronda preliminar** de Copa (82 clubes españoles → 64);
  las rondas siguientes se crean en F13 cuando se conocen los clasificados.
- El generador evita que un equipo juegue dos partidos con menos de **48 h** de descanso.

---

## 5. S2 — Plantilla

**Objetivo:** consultar la plantilla, elegir el once y la táctica, y gestionar dorsales.

- [ ] **Jugadores** (`Roster`)
  - Origen: `Tactical5/UI/Screens/Roster`.
  - Fútbol: atributos de fútbol (físicos/técnicos/mentales), demarcación, contrato, cláusula,
    valor, forma, moral, minutos y notas.
  - Datos: `players`, `player_season_stats`, `lineups`.
  - Incluye accesos a **Perfil de jugador** y **Trayectoria** (pantallas `PlayerProfile` /
    `Trajectory`, origen `Tactical5/UI/Screens/PlayerProfile|Trajectory`).
- [ ] **Alineación** (`Lineup`)
  - Origen: `Tactical5/UI/Screens/Quinteto`.
  - Fútbol: **once por formación** (4-4-2, 4-3-3, 4-2-3-1, 3-5-2, 5-3-2) + **7 suplentes**;
    validación de posiciones y penalización fuera de puesto.
  - Datos: `lineups` (`Slot`: 0 once, 1 banquillo, 2 no convocado), `FormationRules`.
- [ ] **Táctica** (`Tactics`)
  - Origen: `Tactical5/UI/Screens/Rotation`.
  - Fútbol: formación, estilo (posesión/contragolpe/presión), intensidad, línea defensiva,
    balón parado y marcaje. Sustituye la rotación de minutos de baloncesto.
  - Datos: `teams.Formation`, ajustes tácticos en `teams`/tabla nueva si hace falta.
- [ ] **Dorsales** (`Dorsales`)
  - Origen: `Tactical5/UI/Screens/Dorsales`.
  - Fútbol: inscripción **1–25** con porteros 1/13/25; avisos de duplicados.

**Aceptación S2:** el once guardado es el que usa el simulador; dorsales sin duplicados; ficha de
jugador completa.

---

## 6. S3 — Entrenamiento y salud

- [ ] **Entrenamiento** (`Training`)
  - Origen: `Tactical5/UI/Screens/Training`.
  - Fútbol: foco semanal (físico, técnico, táctico, recuperación) con mejora acotada y fatiga.
  - Datos: `players` (atributos, `Fitness`), plan semanal.
- [ ] **Cuerpo técnico** (`Staff`)
  - Origen: `Tactical5/UI/Screens/Employees`.
  - Fútbol: entrenador, segundo, preparador físico, médico, ojeadores y cantera; bonuses.
  - Datos: `staff` (modelo `StaffDataDB`).
- [ ] **Enfermería** (`Injured`)
  - Origen: `Tactical5/UI/Screens/Injured`.
  - Fútbol: lesiones, días de baja, evolución y alta forzada con riesgo.
  - Datos: `players.InjuryDays/InjuryType`, tabla `injuries`.

**Aceptación S3:** entrenamiento aplica mejoras y fatiga; lesiones descuentan días al avanzar.

---

## 7. S4 — Cantera y rivalidades

- [ ] **Filial / Cantera** (`Filial`)
  - Origen: `Tactical5/UI/Screens/GLeague`.
  - Fútbol: filial (Real Sociedad B, Celta Fortuna…), canteranos promocionables y cesiones.
  - Datos: `teams.IsFilial/ParentClubId`, `players.IsYouth`, `FilialRules`.
- [ ] **Rivalidades** (`Rivalidades`)
  - Origen: `Tactical5/UI/Screens/Rivalidades`.
  - Fútbol: derbis y rivalidad histórica; efecto en moral/ambiente.
  - Datos: tabla de rivalidades o seed.

---

## 8. S5 — Mercado

- [ ] **Ofertas** (`Market`)
  - Origen: `Tactical5/UI/Screens/Market`.
  - Fútbol: ofertas por tus jugadores, cláusulas de rescisión, cesiones y respuestas.
  - Datos: `offers`, `players.ReleaseClause`, `TransferBudget`.
- [ ] **Cartera** (`Cartera`)
  - Origen: `Tactical5/UI/Screens/Cartera`.
  - Fútbol: seguimiento de jugadores objetivo.
  - Datos: tabla `shortlist`/`cartera`.
- [ ] **Buscador** (`Buscador`)
  - Origen: `Tactical5/UI/Screens/Buscador`.
  - Fútbol: filtros por demarcación, edad, valor, cláusula, club y nacionalidad.
  - Datos: `players`, `teams`.
- [ ] **Historial** (`Historial`)
  - Origen: `Tactical5/UI/Screens/Historial`.
  - Fútbol: traspasos y cesiones realizados.
  - Datos: tabla `transfers`.

**Aceptación S5:** se puede fichar/vender respetando presupuesto, cláusula y ventanas.

---

## 9. S6 — Finanzas

- [ ] **Finanzas / Decisiones** (`Finanzas`)
  - Origen: `Tactical5/UI/Screens/Finances`.
  - Fútbol: ingresos/gastos, masa salarial, fair play financiero, decisiones de directiva.
  - Datos: `teams` (presupuesto, masa salarial), tabla de movimientos.
- [ ] **Préstamos** (`Loans`)
  - Origen: `Tactical5/UI/Screens/Loans`.
  - Fútbol: préstamos bancarios/deuda y su impacto en el límite salarial.
- [ ] **Patrocinadores** (`Sponsors`)
  - Origen: `Tactical5/UI/Screens/Sponsors`.
  - Fútbol: contratos de patrocinio y renovaciones.
- [ ] **Televisiones** (`TV`)
  - Origen: `Tactical5/UI/Screens/TV`.
  - Fútbol: reparto audiovisual por clasificación y competición.

---

## 10. S7 — Club

- [ ] **La Ciudad** (`City`)
  - Origen: `Tactical5/UI/Screens/TheCity`.
  - Fútbol: ciudad, afición, instalaciones y entorno.
- [ ] **Estadio** (`Stadium`)
  - Origen: `Tactical5/UI/Screens/Arena`.
  - Fútbol: **estadio** (no pabellón), aforo, obras y ampliación.
  - Datos: `teams.StadiumName/StadiumCapacity/ArenaRenovation*`.

---

## 11. S8 — Manager y actualidad

- [ ] **Info manager** (`Manager`)
  - Origen: `Tactical5/UI/Screens/Manager`.
  - Fútbol: perfil, reputación, confianza de la directiva y objetivos.
  - Datos: `managers` (`Trust`, `Morale`, `FanConfidence`).
- [ ] **Logros del mánager** (`Logros`)
  - Origen: `Tactical5/UI/Screens/Logros`.
  - Fútbol: hitos y trofeos conseguidos.
- [ ] **Noticias** (`Messages`)
  - Origen: `Tactical5/UI/Screens/Messages`.
  - Fútbol: bandeja de noticias (fichajes, resultados, objetivos, lesiones).
  - Datos: `messages` (`MessageDataDB`); generar noticias al avanzar el día.
- [ ] **Hilos** (`Hilos`)
  - Origen: `Tactical5/UI/Screens/Hilos`.
  - Fútbol: tramas narrativas (rachas, conflictos, rumores).

---

## 12. S9 — Estadísticas y palmarés

- [ ] **Estadísticas** (`Stats`)
  - Origen: `Tactical5/UI/Screens/Stats`.
  - Fútbol: goleadores, asistentes, porteros, tarjetas y notas por competición.
  - Datos: `player_season_stats`, `player_match_stats`.
- [ ] **Palmarés** (`Palmares`)
  - Origen: `Tactical5/UI/Screens/Palmares`.
  - Fútbol: Ligas, Copas, Supercopas y europeas; histórico del club.
  - Datos: `palmares` (437 finales sembradas).
- [ ] **Récords** (`Records`)
  - Origen: `Tactical5/UI/Screens/Records`.
  - Fútbol: récords de goles, puntos, rachas, etc.
- [ ] **Premios** (`Premios`)
  - Origen: `Tactical5/UI/Screens/Premios`.
  - Fútbol: Balón de Oro y Pichichi (seed de 69 premios y 32 Pichichis).
- [ ] **Eliminatorias** (`Knockouts`)
  - Origen: `Tactical5/UI/Screens/Playoffs`.
  - Fútbol: Copa del Rey, Supercopa y eliminatorias europeas; cuadros y resultados.

---

## 13. Tareas transversales

- [ ] Completar `ScreenManager` si falta algún campo (ya existen los 51).
- [ ] Revisar que `SidebarController` marque la pantalla activa y abra submenús.
- [ ] `Messages`: generar noticias al avanzar el día y marcar leídas.
- [ ] `Stats`/`Palmares`/`Records`: consultas agregadas reutilizables en `DatabaseManager`.
- [ ] Zonas de clasificación centralizadas (Champions/Europa/Conference/descenso).
- [ ] Pruebas de humo de todos los UXML (elementos + claves).
- [ ] Actualizar `Docs/PLAN_REPLICA.md` y `AGENTS.md` al cerrar cada fase.

---

## 14. Orden recomendado

1. **S1** (competiciones): desbloquea navegación y datos de clasificación.
2. **S2** (plantilla): el once y la táctica alimentan el simulador.
3. **S3** (entrenamiento/salud): da vida al avance de días.
4. **S9** (stats/palmarés): mucha lectura, poca lógica nueva.
5. **S5** (mercado) y **S6** (finanzas): dependen de presupuesto y ventanas.
6. **S4**, **S7**, **S8**: contenido de club y narrativa.
