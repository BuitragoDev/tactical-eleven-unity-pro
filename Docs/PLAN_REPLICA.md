# PLAN DE RÉPLICA — **Tactical Eleven** sobre la base de **Tactical Five**

> Documento de trabajo. Define **fase por fase**, en orden de ejecución, qué archivos crear,
> qué pantallas replicar y qué cambia respecto al juego de baloncesto.
> La UI (estructura, estilos, fuentes, modales, botones, tamaños) es **copia literal** de Tactical Five.
> Lo que cambia es el **dominio**: baloncesto → fútbol.

- Proyecto destino: `/home/abuitrago/ProyectosUnity/TacticalEleven`
- Proyecto origen (referencia viva): `/home/abuitrago/ProyectosUnity/Tactical5`
- Recursos ya preparados: `/home/abuitrago/Escritorio/Recursos TacticalEleven`
- Unity: **6000.3.15f1** (misma versión que Tactical Five)
- Temporada base del seed: **2026-27** (actualizar cada verano)

---

## 0. Cómo leer este documento

1. Las fases se ejecutan **en orden**: cada una compila y deja el juego arrancable (nunca se rompe la base).
2. Cada fase indica **qué se copia tal cual** de Tactical Five, **qué se adapta** y **qué es nuevo**.
3. Reglas de oro heredadas de Tactical Five (no negociables):
   - **Una sola escena** (`MainMenu.unity`); las pantallas son `UIDocument` en GameObjects y se navega con `ScreenManager.GoTo`.
   - **Cero prefabs y cero ScriptableObjects de juego.** Contenido = seeders + SQLite; vistas = UXML+USS.
   - **Toda la persistencia por `DatabaseManager`** (partial classes por dominio). Nadie abre su propia conexión.
   - **Sin bus de eventos**: mensajes en la tabla `messages`, estado estático (`GameResultCache`, `ScreenManager.*`), `PlayerPrefs` (solo ajustes) y callbacks de UI.
   - **Todo controller hereda `UIScreenController`**; la re-inicialización vive en `OnEnable`, nunca en `Start`.
   - **Textos por claves** (`{loc:clave}` en UXML, `LocalizationService.T("clave")` en C#), tabla única `Resources/Localization/es.json`.
   - **Lógica pura en clases static** (`TradeHelper`-style); controllers delgados.
   - **Idioma**: strings de juego y comentarios en español; estructura de código en inglés cuando sea natural.
   - **Dinero** `$"{valor:N0}"`, **fechas** `"yyyy-MM-dd"`.

---

## 1. Estado de partida

### 1.1 Proyecto `TacticalEleven` (hoy)

- Proyecto Universal **2D** vacío: `Assets/Default/{InputSystem_Actions, Settings, Scenes/SampleScene}`, `Packages/manifest.json` (URP 17, Input System 1.19, Test Framework 1.6, uGUI, módulo `uielements`).
- Git inicializado, primer commit `fbda58c`.
- **No hay** UI Toolkit montado, ni escena de menú, ni scripts, ni base de datos.

### 1.2 Recursos ya disponibles (`Recursos TacticalEleven`)

| Recurso | Contenido | Uso |
|---|---|---|
| `Data/` (14 archivos + `Models/`) | Capa de datos del intento anterior: **25 tablas** en `Models/`, `DatabaseManager.cs`, `GameSaveManager.cs`, `ScheduleGenerator.cs`, `Constants.cs`, `PlayerEnums.cs`, `SQLite.cs`, seeders y utilidades | **Base principal a rescatar** (ver §1.2.1) |
| `PlayerSeeder.cs` (5.320 ln) | Seed de jugadores con ~45 atributos estilo FM (físico, técnico, mental, portero), `MainPosition`/`SecondaryPositions`, `ReleaseClause`, `Wage`, `Value`, `Contract`, `Number` | **Base a rescatar** (adaptar modelos) |
| `PalmaresSeeder.cs` (554 ln) | Histórico de finales por `CompetitionId` (LaLiga, Copa, Europa) | **Base a rescatar** |
| `Teams/{32x32,64x64,120x120}` | **233 escudos** (nombrados por id de equipo) | Escudos de equipos |
| `Stadiums` | 10 imágenes de estadio (small/medium/large + deluxe + construcción) | Pantalla Estadio / Pabellón |
| `Icons` | 44 iconos (incluye `campo_futbol`, `gol_icon`, `yellowCard_icon`, `redCard_icon`, `secondYellowCard_icon`, `european_comp`, `trofeo`, `contrato`, `intercambio`, `star_24px`, cursores) | Sidebar, ticker y modales |
| `Flags` | 464 banderas por código ISO (`Default/` y `Large (2×)/`) | Nacionalidades |
| `Audio` | 7 WAV (menús, click, transición, día de partido, entrenamiento) | `AudioManager` |

### 1.2.1 Inventario de `Data/` (lo que ya existe)

| Archivo | Contenido | Veredicto |
|---|---|---|
| `Models/*.cs` (15 archivos) | 25 tablas: `players`, `teams`, `leagues`, `managers`, `seasons`, `games`, `match_events`, `player_match_stats`, `player_season_stats`, `injuries`, `lineups`, `offers`, `standings`, `staff`, `employees`, `team_settings`, `player_training`, `sponsors`, `tv_channels`, `loans`, `finance_records`, `messages`, `finals_records`, `awards_records`, `golden_boot_records` | ✅ **Copiar y adaptar** |
| `PlayerSeeder.cs`, `PalmaresSeeder.cs`, `GoldenBootSeeder.cs` | Seed de jugadores, palmarés y Pichichis | ✅ Rescatar |
| `ScheduleGenerator.cs` (8,1 KB) | Doble vuelta, calendario europeo (8 jornadas), Copa del Rey (borrador) y asignación de fechas | ⚠️ **Rescatar y endurecer** (ver F6) |
| `GameSaveManager.cs` | Slots `save_{n}.db`, `saves.json`, limpieza de huérfanos | ✅ Rescatar |
| `Constants.cs` | `CountryCodes` (nombres y ruta de bandera) | ✅ Rescatar |
| `PlayerEnums.cs` | `Position` (GK…CF), `PreferredFoot`, `PlayerStatus`, `InjurySeverity`, `MatchRating`, `PersonalityType`, `Morale`, `FitnessLevel`, `MatchEventType`, `PlayerPositionFlags` | ✅ Rescatar |
| `SQLite.cs` | Wrapper sqlite-net (MIT) | ✅ Ya copiado al proyecto |
| `DatabaseManager.cs` (2.028 ln) | Monolito con CRUD + seed de equipos/empleados/patrocinadores/TV | ⚠️ **Reescribir como partials**, reutilizando consultas |
| `SQLiteAsync.cs` | Wrapper async antiguo (**muerto**: T5 lo eliminó; el async va por `RunInBackground`) | ❌ **No copiar** |
| `PlayerData.cs`, `TeamData.cs` | POCOs legacy (`PlayerAttributes`, `ContractData`, `TeamData`, `LeagueData`, `MatchData`…) que **duplican** los modelos de `Models/` | ❌ **No copiar** (código muerto) |
| `SaveSlotInfo.cs` | Metadatos de slot | ✅ Rescatar |

> **Ojo:** `PlayerData.cs` y `TeamData.cs` declaran clases con nombres parecidos a los modelos reales (`TeamData`, `LeagueData`, `MatchData`) pero **no** son tablas y no se usan. Copiarlos solo añade confusión y riesgo de colisión.


### 1.3 Qué falta (lo cubre este plan)

- Toda la UI Toolkit (tema, panel settings, fuentes, estilos globales, header, sidebar).
- La política de **descensos desde 1ª RFEF** (no hay 2ª RFEF en el juego): pool de clubes no simulados que cubre las 10 plazas (§10).
- **Completar** el modelo de datos existente: tipo de competición, grupos, participación europea por temporada, contratos/cesiones/inscripción, cantera, copa por rondas y tabla de migraciones.
- Motor de partido de 90', copa completa, fase de liga europea con bombos, rollover con ascensos/descensos y toda la capa UI.

---

## 2. Arquitectura objetivo (idéntica a Tactical Five)

```
UI Toolkit (UIDocument por pantalla)  ← MainMenu.unity (escena única)
        ↑ ScreenManager.GoTo / UIScreenController (base)
LÓGICA  ← clases static puras (simulador, calendario, copa, europa, mercado, reglas)
        ↓
DATOS   ← DatabaseManager (partials) + SQLite por slot + template.db
```

Rendimiento y proceso:
- Lotes pesados (día, mercado IA, rollover, scouting) en **background** con `RunInBackground`/`RunInBackgroundAsync` (WAL + conexión ambient `AsyncLocal`).
- **Partido y sorteo/draft de cantera en main thread** (como la simulación de partido de T5).
- Transacciones atómicas en el día de partido y en el rollover de temporada.

**Multiplataforma (requisito):** el juego debe funcionar en **Windows x86_64** y **Linux x86_64**, tanto en el editor como en los builds. SQLite viaja con binarios nativos por plataforma (ver **§4.1**); nada de rutas con separadores fijos, ni dependencias externas al proyecto.

---

## 3. Mapa de conceptos baloncesto → fútbol

| Tactical Five (baloncesto) | Tactical Eleven (fútbol) |
|---|---|
| 30 franquicias, conferencias Este/Oeste | **82 clubes**: 20 (1ª) + 22 (2ª) + 40 (1ª RFEF, 2 grupos); sin conferencias |
| Calendario 82 partidos, 41 casa / 41 fuera | 38 (1ª) / 42 (2ª) / 38 (1ª RFEF), doble vuelta |
| Play-In + Playoffs 4×mejor de 7 | Sin playoffs de título; **playoff de ascenso** (2ª: 3º-6º); **Copa** y **Europa** a eliminatorias |
| NBA Cup (grupos + eliminatoria) | **Copa del Rey** (eliminatorias a partido único; semis ida/vuelta) y **Supercopa** (4 equipos) |
| Sin descensos | **3 descensos** (1ª), **4** (2ª) y **10** (1ª RFEF, 5 por grupo) que salen al pool de Tercera; ascensos simétricos |
| Salary cap, aprons, luxury tax, MLE | **Sin margen salarial NBA**: presupuesto de fichajes, masa salarial y **fair play financiero**; **cláusulas de rescisión** |
| Contratos con TO/PO, Bird rights | Contratos 1-5 años, **cláusula**, **renovación**, **cesiones**, **agentes libres**, **ventanas de fichajes** |
| Draft de rookies + lotería | **Cantera** (canteranos por temporada) + mercado de juveniles |
| G-League (30 filiales) | **Filiales** (equipos B dentro de 1ª/2ª RFEF) + **cantera** |
| Quinteto titular + rotación de minutos | **Once inicial** + banquillo de 7 + **formación y táctica** |
| All-Star, Quintos (quintetos del año) | **Onces ideales** (jornada/temporada) y premios individuales |
| Dorsales retirados | **Dorsales** (inscripción 1-25) y **camisetas retiradas** |
| MVPs, DPOY, 6º hombre | **MVP, Pichichi, Zamora, Miguel Muñoz, mejor joven, mejor jugador de la jornada** |
| Pabellón / aforo / abonos | **Estadio** / aforo / abonados y taquilla |
| TheCity (proyectos de ciudad) | **Ciudad** (estadio, ciudad deportiva, cantera, masa social) |
| Logros del GM (28) | Logros del mánager (misma mecánica) |
| Hilos narrativos / rivalidades | Igual (clásicos, crisis, rumores, vestuario) |
| Lesiones (27 tipos) | Catálogo de fútbol (muscular, tobillo, rodilla, ligamentos, fracturas, conmoción) |
| Fog of war de ojeadores | Igual (rango de atributos hasta ojear) |

---

## 4. Estructura de carpetas objetivo

```
Assets/_TacticalEleven/
  Scenes/
    MainMenu.unity                        ← única escena (renombrar SampleScene)
  Scripts/
    Core/      ScreenManager · UIScreenController · FullScreenUI · AudioManager · CursorManager
               GameEnums · LocalizationService · CustomScrollbar · CustomSlider · TeamImages
               CalendarHelper · IRandomSource · FileDialogHelper · NamePools · StatFormat
               FootballSimulator · FormationRules · FootballScheduleGenerator
               CopaDelReyGenerator · EuropeanCompetitionGenerator · EuropeanDraw
               PromotionRelegationRules · AscensoPlayoff · TransferRules · ContractRules
               RegistrationRules · YouthIntakeGenerator · PlayerRating · PlayerProgression
               ValueModel · WageModel · MarketEconomy · AttendanceModel · InjuryCatalog
               InjuryHelper · MatchupPreview · LineupBuilder · TacticsRules · RefereeRules
               ObjectiveHelper · AchievementCatalog · AchievementService · TeamStrategy
               StorylineCatalog/Service · MilestoneService · NewsTexts/NewsType
    Data/      DatabaseManager (partials) · GameSaveManager · SQLite.cs · modelos [Table] · seeders
    Stats/     AdvancedStatsHelper · StatsTrendHelper · RivalryHelper · DynastyHelper
               HallOfFameHelper · FogOfWarHelper
    UI/        <Pantalla>Controller.cs (+ partials del Dashboard)
  UI/
    Resources/ TacticalElevenPanelSettings.asset · TacticalElevenTheme.tss
               UI/Core/{Header,Sidebar}.uxml · {Header,Sidebar}.uss
    Screens/   <Pantalla>/<Pantalla>.uxml · <Pantalla>.uss   (≈50 carpetas)
    Styles/    GlobalVariables.uss · Typography.uss · Utilities.uss
  Art/
    Fonts/     BarlowCondensed-*.ttf · Saira_Condensed-*.ttf · Kindergarten.ttf
    Resources/ Audios/ · Flags/ · Icons/ · Teams/Logos/{32x32,64x64,120x120} · Stadiums/
               PlayerPhotos/ · Patrocinadores/ · Televisiones/ · Competiciones/
  Resources/
    Localization/es.json
Assets/Plugins/SQLite/                    ← binarios nativos (copiar de T5)
```

### 4.1 SQLite multiplataforma (Windows + Linux)

El juego debe compilar y **abrir la base de datos** en **StandaloneWindows64** y **StandaloneLinux64** (y en el editor en ambos SO). No se usan paquetes NuGet ni `SQLitePCL`: el proyecto lleva el wrapper sqlite-net y los binarios nativos.

> **Estado:** binarios y wrapper **ya copiados** en `TacticalEleven` (2026-11-03), con sus `.meta` originales:
> `Assets/Plugins/SQLite/...` y `Assets/_TacticalEleven/Scripts/Data/SQLite.cs`.

**Archivos a copiar tal cual (binario + `.meta`):**

```
Assets/Plugins/SQLite/x86_64/SQLite3.dll           → Windows x86_64
Assets/Plugins/SQLite/x86_64/SQLite3.dll.meta
Assets/Plugins/SQLite/Linux/x86_64/libsqlite3.so   → Linux x86_64
Assets/Plugins/SQLite/Linux/x86_64/libsqlite3.so.meta
Assets/_TacticalEleven/Scripts/Data/SQLite.cs      → wrapper sqlite-net (mismo archivo de T5)
```

**Configuración del importador (ya viene en los `.meta`; verificarla siempre):**

| Binario | Win64 | Linux64 | Editor | i386 / Any | Notas |
|---|---|---|---|---|---|
| `SQLite3.dll` | **x86_64, activado** | excluido | activado | desactivado | `isPreloaded: 1` |
| `libsqlite3.so` | excluido | **x86_64, activado** | excluido | desactivado | `isPreloaded: 1` |

- El `DllImport` del wrapper es `const string LibraryPath = "sqlite3"`. Por eso los nombres **deben** ser exactamente `SQLite3.dll` (Windows, sin distinguir mayúsculas) y `libsqlite3.so` (Linux, **sensible a mayúsculas**). No renombrar.
- **No reimportar sin el `.meta`**: si Unity importa los binarios como nuevos, los marca *Any Platform* y el build de la otra plataforma falla en tiempo de ejecución (`DllNotFoundException` en `sqlite3_open`).
- Ruta de guardado: siempre `Path.Combine(Application.persistentDataPath, "TacticalEleven", ...)`; **nunca** separadores `\` o `/` fijos (en Linux es `~/.config/unity3d/<company>/<product>` y en Windows `%USERPROFILE%\AppData\LocalLow\<company>\<product>`).

**Verificación obligatoria (se añade al checklist de F1 y F18):**

1. EditMode en **Linux** (suite completa) y en **Windows** si hay runner.
2. Build **StandaloneLinux64** y **StandaloneWindows64** con el mismo código.
3. *Smoke test* en cada build: crear slot → `InitSaveSlot` → insertar/leer un equipo → cerrar y reabrir.
4. Test de invariantes: `Db` no nulo, `PRAGMA user_version`, `journal_mode=WAL` y escritura en background (`RunInBackgroundAsync`).
5. Comprobar que un slot creado en Linux se abre en Windows (SQLite es portable) y viceversa.

**CI:** ampliar `Tools/ci_unity.sh` con `ContinuousIntegrationBuild.BuildWindowsPlayer` (`BuildTarget.StandaloneWindows64`) además del build Linux; el runner Linux necesita el módulo **Windows Build Support (Mono)** instalado. Ambos builds se publican como artifacts.

**Ampliación futura:** si se quiere Linux ARM64 (Steam Deck), añadir `Assets/Plugins/SQLite/Linux/arm64/libsqlite3.so` con su `.meta` (Linux64 ARM64). No es necesario ahora.

**Licencia:** `SQLite.cs` es el wrapper **sqlite-net (MIT, © Krueger Systems)**; conservar el aviso del encabezado al copiarlo o modificarlo y reflejarlo en `Docs/THIRD_PARTY_ASSETS.md` (F18). Los binarios nativos de SQLite son de dominio público (SQLite).

### 4.2 Alta manual de una pantalla (Editor)

Patrón idéntico a Tactical Five. Por cada pantalla nueva:

1. **UXML + USS** en `Assets/_TacticalEleven/UI/Screens/<Pantalla>/<Pantalla>.uxml` y `.uss`
   (reutilizar `GlobalVariables`/`Typography`/`Utilities`; textos con `{loc:clave}`).
2. **GameObject** en `MainMenu.unity` llamado `<Pantalla>Document`.
3. Añadir **`UIDocument`**:
   - `Panel Settings` → `TacticalElevenPanelSettings`
   - `Source Asset` → `<Pantalla>.uxml`
4. Añadir **`FullScreenUI`** (fuerza el root a pantalla completa).
5. Añadir **`<Pantalla>Controller`** heredando de `UIScreenController` y sobrescribiendo
   `ScreenId` (y solo los hooks que necesite).
6. **Arrastrar** el GameObject (o su componente `UIDocument`) al campo correspondiente
   de `ScreenManager` en el Inspector (`Loading`, `MainMenu`, `Dashboard`…).
7. Navegar con `ScreenManager.Instance.GoTo(GameScreen.<Pantalla>)`.

Notas:
- El árbol se reconstruye en `OnEnable` de cada `UIDocument`: todo el wiring va ahí
  (nunca en `Start`), y no se acumulan callbacks entre visitas.
- Si el campo del `ScreenManager` queda vacío, `GoTo` avisa por consola y mantiene la
  pantalla actual (no rompe la navegación).
- `ScreenId` es el valor que usan el header/sidebar para marcar la sección activa.

---

## 5. Identidad visual (copia literal de Tactical Five)

| Pieza T5 | Destino T11 | Nota |
|---|---|---|
| `Assets/_TacticalFive/Art/Fonts/*` | `Assets/_TacticalEleven/Art/Fonts/` | `BarlowCondensed-*`, `Saira_Condensed-*`, `Kindergarten` (+ SDF si se usan) |
| `UI/Resources/TacticalFiveTheme.tss` | `TacticalElevenTheme.tss` | mismo contenido; cambiar rutas de fuentes |
| `UI/Resources/TacticalFivePanelSettings.asset` | `TacticalElevenPanelSettings.asset` | `ScaleWithScreenSize`, ref. **1920×1080**, `MatchWidthOrHeight`, `match=0.5` |
| `UI/Styles/{GlobalVariables,Typography,Utilities}.uss` | mismos nombres | copia literal (colores, tipografías, utilidades) |
| `UI/Resources/UI/Core/Header.uxml/.uss` | igual | barra superior: equipo, fecha, presupuesto, manager, ajustes |
| `UI/Resources/UI/Core/Sidebar.uxml/.uss` | igual | mismas clases `nav-item`/`nav-submenu`; cambian etiquetas |
| Patrones de modal/botones | `Docs/CODE_GUIDELINES.md` de T5 | `.trade-modal-box`, `.modal-title`, `.btn-action`, `text-transform: uppercase`, **sin `rgba()`**, **sin `gap`** |
| Overlay compartido | `UIScreenController.InitConfigModal` | nunca color inline con alfa: usar `modal-overlay--visible` |

> Referencia obligatoria al implementar: `Tactical5/Docs/CODE_GUIDELINES.md` §2 y §2.1, `Docs/UI_TOOLKIT.md`, `Docs/LOCALIZATION.md`.

---

## 6. Resumen de fases

| Fase | Nombre | Tamaño | Deja jugable |
|---|---|---|---|
| F0 | Fundaciones del proyecto y tema | M | Arranca en pantalla vacía con estilos |
| F1 | Persistencia, slots y template | L | Abre/crea partida vacía |
| F2 | Competiciones, equipos y estadios | L | Datos de clubes cargados |
| F3 | Jugadores, palmarés y premios | XL | Plantillas pobladas |
| F4 | Arranque y menús | M | Menú → nueva partida → cargar → editor |
| F5 | Selección de equipo | M | Eliges club y se crea la temporada |
| F6 | Pretemporada y calendario de liga | L | Calendario completo de la temporada |
| F7 | Dashboard, header, sidebar y avance de día | XL | Puedes avanzar días |
| **Hito 1** | **Corte vertical jugable** | — | **Una temporada completa de liga sin copa ni Europa** |
| F8 | 🚧 En curso | Motor de partido y pantallas de partido | XL | Partidos simulados con crónica |
| F9 | Clasificaciones, resultados e info de liga | M | Competición visible y navegable |
| F10 | Plantilla, alineación, táctica y dorsales | L | Once y formación editables |
| F11 | Mercado: traspasos, cláusulas, cesiones, FA | XL | Fichar y vender |
| F12 | Finanzas, estadio, ciudad, patrocinio y TV | L | Economía del club |
| F13 | Copa del Rey y Supercopa | L | Copa completa |
| F14 | Competiciones europeas | XL | Champions/Europa/Conference |
| F15 | Fin de temporada, ascensos/descensos y rollover | XL | Temporadas infinitas |
| F16 | Staff, ojeadores, entrenamiento y cantera/filial | XL | Desarrollo de jugadores |
| F17 | Narrativa, logros, mensajes, buscador, perfil e historial | L | Profundidad y feedback |
| F18 | Calibración, tests, rendimiento, localización y build | L | Entrega |

---

### Estado de implementación

**Actualizado: 2026-11-04**

| Fase | Estado | Qué hay |
|---|---|---|
| **F0** | ✅ Hecha | Fuentes, tema, panel settings, estilos globales, `GameEnums`, `ScreenManager` (un campo por pantalla), `UIScreenController`, `FullScreenUI`, `AudioManager`, `CursorManager`, `LocalizationService`, `CustomScrollbar`, `CustomSlider`, `IRandomSource`, `CalendarHelper`; audio/iconos/banderas copiados. **Las pantallas las crea el usuario en el Editor** (ver «Alta manual de una pantalla») |
| **F1** | ✅ Hecha | 25 tablas (`Models/*.cs` + columnas nuevas), `DatabaseManager` (fachada + migraciones + seed + template), `GameSaveManager`, `DbMetaData`, `SchemaMigration*`, `SeedBaseline`, `SeedAudit`, `ReferentialIntegrity`; tests `PersistenceTests` + `UiSmokeTests` (**13/13**) |
| **F2** | ✅ Hecha | 9 competiciones, **236 clubes** (82 españoles, 101 europeos y 53 históricos), 233 escudos + genérico, 10 estadios, huella de seed calculada y auditoría; tests `SeedTests` |
| **F3** | ✅ Hecha | **4.184 jugadores** (~45 atributos, 19-29 por club), `PlayerRating` como fuente única del overall, **437 finales**, **69 Balones de Oro** y **32 Pichichis**; tests `PlayerSeedTests` |
| **F4** | ✅ Hecha | Menú completo (Liga Manager, ProManager con modal, Cargar, Editor pendiente, Salir, aviso legal), **modal de configuración** en la base (audio/calidad/vista de partido), pantalla **Cargar partida** con tarjetas y borrado; tests `SaveSlotLifecycleTests` + UI |
| **F5** | ✅ Hecha | Selección de equipo con **4 pestañas** (1ª, 2ª, 1ª RFEF G1/G2), ficha del club, plantilla y nombre del manager; crea manager + temporada (38/42/38 jornadas) y deja el slot listo |
| **F6** | ✅ Hecha | Calendario de liga completo (doble vuelta, 380/462/380 partidos), fechas con parones FIFA/navideño y descanso mínimo de 48 h, pretemporada de hasta 4 amistosos elegidos por el jugador y pantalla `Preseason` con filtros por competición; tests `ScheduleTests` (**57/57** en total) |
| **F7** | ✅ Hecha | Dashboard con header (club, fecha, presupuesto, manager), sidebar completo, tarjetas de resumen (próximo partido, último resultado, clasificación, objetivo, finanzas, plantilla, noticias), modal de ajustes y **avance de día** con parada en día de partido; tests `DashboardTests`

**F6 — decisiones tomadas:**

- **`FootballCalendar`** (puro) decide las fechas: la liga arranca el primer sábado desde el 15/08, las jornadas van en fin de semana (sábado/domingo) y se saltan los parones FIFA (sept., oct., nov., mar.), el navideño (22/12–04/01) y el final de temporada (31/05). Si faltan fines de semana, añade jornadas entre semana (martes/miércoles).
- **`FootballScheduleGenerator`** (puro) hace la doble vuelta por el método del círculo, baraja el orden con `IRandomSource` (misma semilla = mismo calendario) y valida: 38/42/38 jornadas, cada pareja 2 veces (1 casa/1 fuera), sin equipos repetidos por jornada y **48 h mínimas de descanso**.
- **`PreseasonPlanner`** elige rivales de pretemporada de menor categoría (o menor nivel si no hay liga inferior), excluyendo filiales e históricos.
- **`DatabaseManager.Schedule`** persiste en `games` (`GameType` = `regular`/`preseason`, `CompetitionId` = liga) y expone consultas (`GetGamesBySeason`, `GetNextGame`, `GetFirstLeagueGame`…). Regenerar **reemplaza** los partidos del mismo tipo (idempotente).
- **Flujo de creación (réplica de Tactical5)**: al confirmar equipo solo se crea el `manager` y se navega a `Preseason`. La temporada, el calendario (380/462/380 partidos), los amistosos y los **metadatos del slot** se generan al pulsar CONTINUAR en `Preseason`; solo entonces la partida aparece en Cargar partida y se navega al Dashboard. Si el jugador pulsa VOLVER antes, `DiscardPendingGame` borra manager/temporada/partidos y la partida nunca queda listada. La pantalla arranca sin amistosos y permite elegir hasta 4 con filtros de competición (LaLiga, LaLiga 2, 1ª RFEF / Champions, Europa, Conference).
- **Copa y Europa no se generan aquí**: sus generadores (F13/F14) reservarán los huecos de fecha.
- **`ClearAllManagers`** borra también `games` para no arrastrar calendarios de intentos abandonados.

**F5 — decisiones tomadas:**

- **Filtros por competición** (no por conferencia): `TabLiga`, `TabLiga2`, `TabRfef1`, `TabRfef2` (la pestaña TODOS se retiró), filtrando por `teams.LeagueId` 1-4.
- **Ficha del club** con datos reales del seed: competición, fundación, país, estadio, aforo, reputación e instalaciones (estrellas), presupuesto, masa salarial, ataque/defensa/media y **objetivo orientativo** según su puesto por media en la competición.
- **Presupuestos reales 2026**: los 42 clubes de 1ª y 2ª usan el presupuesto anual aproximado de 2026 (`TransferBudget`), redondeado a múltiplos de 500.000 € (Madrid 1.248 M€, Barça 1.075 M€, Atlético 554 M€; en 2ª, de 60 M€ del Girona a 7 M€ del Celta Fortuna). La huella del seed incluye `TransferBudget`, así que el template se reconstruye solo; las partidas ya creadas conservan sus cifras hasta empezar una nueva.
- **ProManager**: solo son seleccionables los clubes de **Primera Federación** (grupos 1 y 2) que no sean filiales (35 de 40). La pantalla abre en el grupo 1; el resto de clubes aparecen deshabilitados.
- **Filiales visibles pero deshabilitados**: aparecen en la rejilla atenuados (tooltip «filial · no seleccionable») y no se pueden elegir. `FilialRules` los marca en el seed (`IsFilial`, `ParentClubId`) por nombre y tabla de padres (Real Sociedad B, Celta Fortuna, Bilbao Athletic, Fabril, Castilla, Atlético Madrileño y Villarreal B); `CanSelect` centraliza la regla (en ProManager solo Primera Federación sin filiales). Quedan **75 clubes jugables** de los 82 españoles. La huella incluye la marca y `InitSaveSlot` repara plantillas/partidas antiguas.
- **Al continuar**: limpia managers/temporadas de intentos previos, crea el `manager` y navega a `Preseason`. **No** crea temporada, calendario ni `saves.json`: eso ocurre en el commit de Pretemporada (ver F6).
- **`PlayerPositionNames`**: códigos y nombres de demarcación en un único sitio (POR, DFC, LI, LD, MCD, MC, MCO, EI, ED, DC…).

**F4 — decisiones tomadas:**

- **Modal de configuración en `UIScreenController`**: audio (3 sliders), calidad gráfica y vista de partido (`TF_SimMode` 0/1/2). Se abre desde el engranaje; solo existe en las pantallas que incluyen el markup.
- **Nueva partida**: comprueba hueco → `CleanupOrphanDb` → `InitSaveSlot` → `SelectTeam`. Si la pantalla de selección no está creada, avisa y **no crea el slot** (evita partidas huérfanas).
- **Cargar partida**: tarjetas con escudo, manager, temporada, jornada, liga y fecha; cargar abre el slot y navega al Dashboard; borrar pide confirmación.
- **Editor**: el botón avisa de que llega en la siguiente fase (la pantalla necesita sesión `template` y edición de assets).
- **`ScreenManager.IsRegistered`**: permite comprobar si una pantalla está asignada antes de navegar.

**F3 — decisiones tomadas:**

- **Seed de jugadores**: `PlayerSeeder` del intento anterior, con atributos procedurales deterministas (`Random.InitState(teamId, dorsal, posición)`); 4.184 jugadores, todos los clubes de liga con 19-29.
- **Overall = `PlayerRating`**: media de los atributos relevantes por demarcación (campo: 8 físicos + 14 técnicos + 14 mentales; portero: 14 mentales + 8 específicos) capada por el potencial. `PlayerDataDB.GetCalculatedAverage()` delega en ella.
- **Palmarés**: 437 finales + 69 premios (`finals_records`, `awards_records`) y 32 Pichichis (`golden_boot_records`), referenciando los ids de club del seed (incluidos los 53 históricos).
- **Dorsales repetidos**: el seed repite dorsales en 65 clubes; la auditoría lo deja como **WARN** (no bloquea la partida). La inscripción única se gestiona en F10/F11.
- **Bug del seed corregido**: 3 llamadas (`Pipe` en Mérida; `Pereda` y `Mani López` en Zamora) tenían `valor/salario/cláusula` desplazados por `RIGHT, GK, 0` (`Value=1`, `Wage=0`, `Clause=0`). Ya tienen valores reales.
- **`SeedBaseline.PlayerSeedVersion`**: el seed de 4.184 jugadores no se hashea (coste); se sube a mano al tocar `PlayerSeeder` para invalidar el template.
- **Tests**: 25/25 en verde (F3 cerrada).

**F2 — decisiones tomadas:**

- **Seed de clubes**: se reutiliza el del intento anterior (236 filas). Los 53 clubes con `LeagueId = 0` son históricos para el palmarés y **no aparecen** en SelectTeam.
- **Huella del seed**: `SeedBaseline.Fingerprint` se **calcula** desde `CompetitionSeeder` + `TeamSeeder` (MD5); si cambia el seed, el template se reconstruye solo, sin tocar constantes.
- **Escudos**: 233 PNG por id + `default` genérico generado; se cargan como `Sprite` en `Teams/Logos/{32,64,120}`.
- **Estadio**: `StadiumImage` se asigna por aforo (`small` <30.000, `medium` <60.000, `large` ≥60.000).
- **2ª RFEF**: fuera del juego (decisión previa); no se siembra.

**Desviaciones respecto al plan (decididas al implementar):**

- `ScreenManager` sigue el patrón de Tactical Five: **un campo `[SerializeField] UIDocument` por pantalla** (agrupados por `Header`) y un `switch` en `GoTo`; las pantallas se arrastran en el Inspector. No hay generación automática de escena (se retiró `SceneBootstrap`).
- El enum de demarcaciones se llama **`PlayerPosition`**, no `Position`: el enum global `Position` ocultaba `UnityEngine.UIElements.Position` y rompía el fullscreen (CS0117).
- `GameSaveManager.BaseDirOverride` y `DatabaseManager.DataRootOverride` existen **solo para tests** (carpeta temporal, sin tocar las partidas reales).
- La escena y los GameObjects de pantalla los crea el usuario en el Editor (no hay bootstrap automático).
- `PRAGMA journal_mode` y `busy_timeout` se ejecutan con `ExecuteScalar`, no con `Execute` (sqlite-net lanza "not an error" al devolver fila).

---

## 7. Fases detalladas

### F0 — Fundaciones del proyecto y tema visual

**Objetivo:** dejar el proyecto listo para UI Toolkit con la misma identidad visual, sin lógica de juego.

**Origen:** `Tactical5/Assets/_TacticalFive/{UI,Scripts/Core}`.

**Archivos a crear:**
- `Assets/_TacticalEleven/Art/Fonts/*` (copiar fuentes + `.meta` nuevos).
- `Assets/_TacticalEleven/UI/Styles/{GlobalVariables,Typography,Utilities}.uss` (copia literal).
- `Assets/_TacticalEleven/UI/Resources/TacticalElevenTheme.tss` y `TacticalElevenPanelSettings.asset` (1920×1080, `match=0.5`).
- `Assets/_TacticalEleven/Scripts/Core/`:
  - `GameEnums.cs` (`GameScreen`, `GameMode`, `SimMode`).
  - `ScreenManager.cs` (singleton, registro `screen → UIDocument`, `GoTo`).
  - `UIScreenController.cs` (base: fullscreen, pipeline `CacheReferences → LoadData → RegisterCallbacks → Refresh`, cursores, click).
  - `FullScreenUI.cs`, `AudioManager.cs`, `CursorManager.cs`.
  - `LocalizationService.cs`, `CustomScrollbar.cs`, `CustomSlider.cs`, `IRandomSource.cs`, `CalendarHelper.cs`.
  - **Diferidos a F2/F4**: `TeamImages`, `CustomScopes`, `StatFormat`, `NamePools`, header/sidebar y modal de configuración.
- `Assets/_TacticalEleven/Scenes/MainMenu.unity`: escena única. El usuario crea en el Editor los GameObjects de pantalla (`UIDocument` + `FullScreenUI` + controller) y los arrastra a `ScreenManager`.
- `Assets/_TacticalEleven/Resources/Localization/es.json` (tabla vacía + claves comunes).
- `Tools/localization_extract.py` (copiar de T5 y adaptar rutas).

**Pantallas:** ninguna de juego; solo `Loading` mínimo para comprobar el pipeline.

**Aceptación:** la escena arranca, carga la fuente, muestra una pantalla de carga estilizada y `ScreenManager.GoTo` funciona entre dos documentos vacíos.

---

### F1 — Persistencia, slots y template

**Objetivo:** SQLite por slot, `template.db`, migraciones versionadas y auditoría de seed.

**Origen:** `Tactical5/Scripts/Data/{DatabaseManager*.cs, GameSaveManager.cs, SQLite.cs, DbMetaData.cs, SchemaMigrationAttemptData.cs, SeedBaseline.cs, SeedAudit.cs, SaveSlotInfo.cs, ReferentialIntegrity.cs}`.

**Archivos:**
- `Scripts/Data/DatabaseManager.cs` (fachada: `Instance`, `Db`, `RunInBackground*`, `CreateTables`, `RunMigrations`, `InitSaveSlot`, `CloseActiveSlot`, índices).
- `Scripts/Data/DatabaseManager.Migrations.cs` (columnas aditivas, `user_version`, intentos) y `DatabaseManager.Seeding.cs` (bloques idempotentes de seed).
- `Scripts/Data/DatabaseManager.Seeding.cs`, `.Teams.cs`, `.Players.cs`, `.Games.cs`, `.Manager.cs`, `.Staff.cs`, `.Contracts.cs`, `.Finances.cs`, `.Records.cs`, `.Awards.cs`, `.History.cs`, `.Rollover.cs`, `.Progression.cs`, `.Cup.cs`, `.Europe.cs`, `.Storylines.cs`.
- `Scripts/Data/GameSaveManager.cs` (slots `save_{n}.db`, `saves.json`, `template.db`, borrado con assets).
- `Scripts/Data/{SaveSlotInfo, DbMetaData, SchemaMigrationAttemptData, SeedBaseline, SeedAudit, ReferentialIntegrity}.cs`.
- `Assets/Plugins/SQLite/{x86_64/SQLite3.dll, Linux/x86_64/libsqlite3.so}` + sus `.meta` (ver **§4.1**, multiplataforma Windows/Linux).
- Modelos: **copiar `Models/*.cs` de `Recursos TacticalEleven/Data`** (ya existen las 25 tablas) y añadir las columnas/tablas que falten (F2/F11).

**Ya disponible:** `SQLite.cs` está copiado en el proyecto; `GameSaveManager.cs`, `SaveSlotInfo.cs`, `Constants.cs` y `PlayerEnums.cs` se rescatan tal cual. **No copiar** `SQLiteAsync.cs`, `PlayerData.cs` ni `TeamData.cs` (§1.2.1).

**Claves:**
- `schema_migrations` + `PRAGMA user_version = 2`; migraciones obligatorias bloqueantes (`DatabaseMigrationException`).
- `InitSaveSlot` siembra dentro de `RunInTransaction` y **nunca anida transacciones** (bug histórico de T5 documentado).
- `EnsureTemplateDb` genera el template al vuelo si falta; huella `seed_fingerprint`.

**Aceptación:** crear un slot nuevo desde código, cerrarlo y reabrirlo; `template.db` se genera y se clona; un fallo de migración bloquea la apertura; **el slot se abre igual en Linux y en Windows** (checklist de §4.1).

---

### F2 — Competiciones, equipos y estadios

**Objetivo:** cargar la pirámide española y las competiciones europeas, con escudos y estadios.

**Origen:** `Recursos TacticalEleven/Data/{Models/TeamDataDB.cs, DatabaseManager.cs}` (`SeedLeagues`, `SeedTeams`) + `Tactical5/Scripts/Data/{TeamData.cs, DatabaseManager.Teams.cs, DatabaseManager.Seeding.cs, TeamRecordSeeder.cs}`.

**Modelo: se parte del existente** (`LeagueDataDB`, `TeamDataDB`) y se **amplía**, para no reescribir el seed de 236 equipos:

```csharp
[Table("leagues")] LeagueDataDB          // ya existe: se amplía
  Id, Name, Country, Division, TeamCount, Promotions, Relegations   // actuales
+ ShortName, Type ("league"|"cup"|"supercup"|"europe"),
+ Tier (1..3 | 0), GroupIndex, EuropeSlots, Logo

[Table("teams")] TeamDataDB              // ya existe: se amplía
  Id, Name, ShortName, Nickname, City, Country, LeagueId,
  StadiumName, StadiumCapacity, YearFounded, PrimaryColor, SecondaryColor,
  BankBalance, TransferBudget, WageBudget, TotalWageBill,
  TrainingFacilities, YouthFacilities, YouthRecruitment, ScoutingRange,
  Reputation, Overall, Attack, Midfield, Defense, Formation, ParentClub,
  Objective, Chemistry, EuropeanCompetitionId,
  SeasonPoints/Position/GoalsFor/Against/Wins/Draws/Losses,
  DomesticLeagueTitles, DomesticCupTitles, ContinentalTitles
+ Logo, StadiumImage, IsFilial, ParentClubId, JerseyHome, JerseyAway
```

Reglas de compatibilidad:
- **Se mantiene `teams.LeagueId`** (no renombrar a `competition_id`): el seed, `ScheduleGenerator` y las consultas ya lo usan. `leagues` es la tabla de competiciones.
- `leagues.Type` distingue liga/copa/supercopa/europa; `GroupIndex` para 1ª RFEF G1/G2.
- `teams.EuropeanCompetitionId` del seed antiguo **queda como legado**: la participación se recalcula cada temporada en `competition_entries` (F14) desde la clasificación.
- `TeamDataDB` **no tiene logo**: los PNG vienen nombrados por `Id`, así que `Logo` puede ser el nombre del archivo (o resolverse por id con fallback genérico).
- `TeamDataDB.Formation` es un `int` (índice de formación); documentar el orden en `FormationRules`.

**Competiciones a sembrar (2026-27):**

| id | Competición | Tipo | Equipos |
|---|---|---|---|
| 1 | LaLiga EA Sports | league, tier 1 | 20 |
| 2 | LaLiga Hypermotion | league, tier 2 | 22 |
| 3 | Primera Federación — Grupo I | league, tier 3 | 20 |
| 4 | Primera Federación — Grupo II | league, tier 3 | 20 |
| 5 | UEFA Champions League | europe | 36 |
| 6 | UEFA Europa League | europe | 36 |
| 7 | UEFA Conference League | europe | 36 |
| 8 | Copa del Rey | cup | variable |
| 9 | Supercopa de España (nueva) | supercup | 4 |
| — | Tercera Federación | **no simulada** | pool de 10 ascensos/año (solo nombres) |

> **Ids alineados con `PalmaresSeeder.CompetitionId`** (1-8) para no tener que remapear el histórico; la Supercopa entra como 9.

**Composición europea (fase de liga de 36, formato actual):**

| Competición | Clubes extranjeros (seed) | Plazas españolas | Total |
|---|---|---|---|
| Champions League | 32 | **4** (1º-4º de LaLiga) | 36 |
| Europa League | 34 | **2** (5º-6º de LaLiga) | 36 |
| Conference League | 35 | **1** (7º de LaLiga) | 36 |

> Los **7 primeros** de LaLiga juegan Europa cada temporada (4 + 2 + 1). La asignación es **dinámica**: se calcula en el rollover desde la clasificación final, no desde `teams.european_league_id`. El seed solo aporta los clubes extranjeros de cada competición.

**Rescate del seed antiguo:** los 236 equipos de `SeedTeams` cubren 1ª, 2ª, 1ª RFEF y los clubes europeos (32 + 34 + 35 = 101). **No se añade 2ª RFEF.** Hay que **reclasificar** los clubes europeos: en el intento anterior se guardaban como `LeagueId=5/6/7`; aquí van en `competition_entries` (participación por temporada), no como liga del club, y las plazas españolas se recalculan cada temporada.

**Assets:** copiar `Teams/{32x32,64x64,120x120}` a `Art/Resources/Teams/Logos/...`; `Stadiums/*` a `Art/Resources/Stadiums/`; `Flags/*` a `Art/Resources/Flags/`; `Icons/*` a `Art/Resources/Icons/`. **Copiar siempre el `.meta` junto al PNG**: sin él Unity importa la imagen como *sprite múltiple* con recortes automáticos y la UI muestra solo una porción (pasó con el logo de Loading). Prever **escudo genérico** para los clubes del pool de ascenso que no tengan logo.

**Aceptación:** `GetAllTeams()` devuelve **82 clubes españoles** (20 + 22 + 40) **+ 101 europeos**; cada uno con competición, estadio y aforo; el auditor informa plantillas fuera de rango pero **no reasigna clubes**.

---

### F3 — Jugadores, palmarés y premios históricos

**Objetivo:** poblar plantillas reales y el histórico de competiciones.

**Origen:** `Recursos TacticalEleven/Data/{PlayerSeeder.cs, Models/PlayerDataDB.cs, PalmaresSeeder.cs, GoldenBootSeeder.cs}` + `Tactical5/Scripts/Data/{PlayerRating.cs, DatabaseManager.Seeding.cs, AwardsRecord.cs}`.

**Modelo de jugador: ya existe** (`Models/PlayerDataDB.cs`) con ~45 atributos y todo lo necesario:
```csharp
[Table("players")] PlayerDataDB
  Id, TeamId, FirstName, LastName, CommonName, Age, Nationality, SecondNationality,
  Height, Weight, Foot, MainPosition (enum Position), SecondaryPositions (flags),
  // físicos: Pace, Acceleration, Stamina, Strength, Agility, Balance, Jumping, NaturalFitness
  // técnicos: Passing, Crossing, Dribbling, Finishing, FirstTouch, Heading, LongShots,
  //           Marking, Penalties, FreeKicks, Corners, Tackling, Technique, Flair
  // mentales: Aggression, Anticipation, Bravery, Composure, Concentration, Decisions,
  //           Determination, Leadership, OffTheBall, Positioning, Teamwork, Vision, WorkRate, Consistency
  // portero: Handling, Kicking, Reflexes, GkPositioning, OneOnOnes, CommandOfArea, RushingOut, PenaltySaving
  Overall, Potential, Value, Wage, ReleaseClause, ContractYears, SquadNumber, Status,
  Morale, Fitness, Form, Personality, InjuryDays, InjuryType, Treated,
  TotalGoals, TotalAssists, TotalAppearances, TotalYellowCards, TotalRedCards, TotalManOfTheMatch
```
**Ajustes obligatorios al copiarlo:**
- `Wage` es `int`: pasar a **`long`** (evita desbordes al crecer salarios/años).
- Añadir `ContractEndSeason`, `IsOnLoan`, `LoanFromTeamId`, `LoanEndSeason`, `TransferListed`, `IsYouth`, `IsFilialEligible`, `SeasonsWithTeam`, `LastTeamId` (los necesita el mercado de F11).
- `GetCalculatedAverage()` usa una media simple de 24 atributos; **sustituirla por `PlayerRating`** con pesos por posición (fuente única) y no dejar dos fórmulas de media.
- `PlayerEnums.Position` (GK…CF) y `PlayerPositionFlags` ya están; usarlos como fuente única de posiciones.

**Otros modelos existentes y sus ajustes:**
- `AwardsRecord.Winner` es **string** (nombre) → añadir `PlayerId` y usarlo; el nombre solo para histórico importado.
- `GoldenBootRecord.Winner` igual.
- `PlayerSeasonStatsDB` ya trae `Appearances/Starts/Subs/Minutes/Goals/Assists/Cards/MOTM/Rating/CleanSheets/Conceded/Saves` → se amplía con `Shots/Passes/KeyPasses/Tackles/Interceptions/Dribbles` si se quieren stats avanzadas.
- `InjuryDataDB` ya existe (`Date/Description/Severity/DaysOut/DaysRemaining/IsRecurring`).

**Overall:** `PlayerRating.FromAttributes(...)` con **pesos por posición**; recalcular en seed, progresión, cantera y migraciones.

**Seed:**
- Adaptar `PlayerSeeder` (atributos, `ReleaseClause`, `Wage`, `Value`, `SquadNumber`, `ContractYears`).
- `PalmaresSeeder` → `finals_records` (ya existe la tabla y el `CompetitionId` 1-8).
- `GoldenBootSeeder` → `golden_boot_records`.
- Generar plantillas para los **clubes del pool de ascenso** (Tercera) solo cuando asciendan a 1ª RFEF, con `PlayerGenerator` a partir de una media por club.

**Aceptación:** 1ª y 2ª con 11-25 jugadores por club; el auditor valida duplicados, dorsales y `overall` derivado; el palmarés se muestra sin depender del seed de clubes actual.

---

### F4 — Arranque y menús

**Objetivo:** flujo de entrada completo.

**Origen:** `Tactical5/UI/Screens/{Loading, MainMenu, LoadGame, Editor}` + `Scripts/UI/{LoadingController, MainMenuController, LoadGameController, EditorController}.cs`.

**Pantallas:**
- `Loading` (consejo + temporizador/tecla, como T5).
- `MainMenu`: **LIGA MANAGER**, **PRO MANAGER**, **CARGAR PARTIDA**, **EDITOR**, **SALIR** + modal legal + modal de restricciones de ProManager + ajustes.
- `LoadGame`: rejilla de slots con escudo, equipo, manager, temporada, fecha y borrado.
- `Editor`: sesión `template` (equipos, jugadores, escudos, estadios, fotos).

**Diferencias con T5:** ninguna funcional; solo textos y logos.

**Aceptación:** menú → nueva partida (crea slot) → selección de equipo; cargar y borrar partida; el editor modifica el template sin tocar partidas existentes.

---

### F5 — Selección de equipo

**Objetivo:** elegir club mostrando la pirámide con filtros por competición.

**Origen:** `UI/Screens/SelectTeam/*` + `SelectTeamController` + `DatabaseManager.{GetWorstTeams, SaveManager, CreateSeason}`.

**Cambios:**
- Filtros: **1ª**, **2ª**, **1ª RFEF G1**, **1ª RFEF G2** (y "Todos").
- Panel de detalle: ciudad, estadio y aforo, fundación, reputación, presupuesto, masa salarial, objetivos, plantilla media, palmarés.
- **ProManager**: solo clubes de la mitad baja de su competición (adaptar `GetWorstTeams`).
- Al confirmar: crear `manager`, `season`, sembrar alineación y **generar calendario** (F6).

**Aceptación:** se puede empezar con cualquier club de las 4 competiciones (1ª, 2ª, 1ª RFEF G1 y G2); el modo ProManager restringe; el slot guarda equipo y manager.

---

### F6 — Pretemporada y calendario de liga

**Objetivo:** generar el calendario de la temporada (ligas) y la pretemporada.

**Origen:** `Recursos TacticalEleven/Data/ScheduleGenerator.cs` (**ya existe**) + `Tactical5/Scripts/Core/{TravelHelper.cs, CalendarHelper.cs}` + `UI/Screens/Preseason/*` + `PreseasonController`.

**Estado actual del generador (auditado):**
- ✅ `GenerateDoubleRoundRobin`: método del círculo, doble vuelta, alternancia casa/fuera; válido para 20/22 equipos.
- ✅ `GenerateAllDomesticSchedules`: agrupa por `LeagueId 1..4`.
- ⚠️ `GenerateEuropeanSchedule`: 8 jornadas aleatorias, **sin bombos, sin control de país/localía ni validación** de que cada equipo tenga 8 rivales distintos.
- ⚠️ `GenerateCopaDelRey`: **solo una ronda** (32 equipos emparejados al azar, ida/vuelta); no es un cuadro completo.
- ⚠️ `AssignLeagueDates`: arranca el 22/08 y va **una jornada por semana** sin parones FIFA, sin parón navideño ni jornadas entre semana.
- ⚠️ Usa `UnityEngine.Random` directamente y escribe `GameDataDB`; no es determinista ni testeable con semilla.
- ⚠️ No hay pretemporada ni `MatchDay` negativo.

**Plan de rescate (`FootballScheduleGenerator`):**
1. Mantener el algoritmo de doble vuelta y **añadir validación**: 38/42/38 jornadas, cada pareja 2 veces (1 casa/1 fuera), 41-41 localías en 1ª, 21-21 en 2ª, 19-19 en 1ª RFEF.
2. **Fechas reales**: arranque mediados de agosto, jornadas en fin de semana, **parones FIFA** (sept., oct., nov., mar.), **parón navideño**, y huecos para Copa (miércoles) y Europa (mar/mié/jue).
3. Inyectar `IRandomSource` (semilla) y separar el cálculo de fechas en un helper puro (`FootballCalendar`).
4. Restricción de descanso: mínimo **48 h** entre partidos del mismo equipo (incluye copa/Europa).
5. Copa y Europa **no** se generan aquí: cada una tiene su generador (F13/F14) y este solo reserva los huecos de fecha.

**Implementado (F6):**
- `FootballCalendar` + `FootballScheduleGenerator` + `PreseasonPlanner` (reglas puras, sin Unity ni SQLite).
- `DatabaseManager.Schedule`: generación/persistencia idempotente y consultas de calendario.
- Pantalla `Preseason` (`UI/Screens/Preseason/*` + `PreseasonController`) con 4 huecos, rejilla de rivales, alternancia local/visitante y guardado.
- Tests `ScheduleTests`: doble vuelta 20/22 equipos, validación, misma semilla, parones/48 h, amistosos y persistencia.

**Pretemporada:** hasta 4 amistosos (18/07, 22/07, 26/07 y 01/08) elegidos por el jugador con filtros de competición (3 españolas y 3 europeas), sin impacto en clasificación; se pueden quitar antes de continuar.

**Aceptación:** calendario válido (cada equipo juega 2× contra cada rival, 1 casa / 1 fuera), sin huecos ni solapamientos, con parones y 48 h de descanso; test puro del generador con semilla fija (mismo calendario en dos ejecuciones). ✅

---

### F7 — Dashboard, header, sidebar y avance de día

**Objetivo:** el hub del juego y el bucle diario.

**Origen:** `UI/Screens/Dashboard/*` + `DashboardController` + 9 partials (`GameDay`, `Match`, `FastSim`, `Offers`, `Modals`, `DaySupport`, `MarketUI`, `Results`, `Refresh`) + `HeaderController`, `SidebarController`, `GameDayPipeline`, `TeamEconomyService`.

**Sidebar (fútbol):**
```
INICIO (Dashboard)
PLANTILLA: Jugadores · Alineación · Táctica · Entrenamiento · Cuerpo técnico · Enfermería · Dorsales
CALENDARIO
COMPETICIONES: Resultados · Clasificación · Info competición · Filial/Cantera · Rivalidades
ESTADÍSTICAS · PALMARÉS · RÉCORDS · PREMIOS · ELIMINATORIAS
MERCADO: Ofertas · Cartera · Buscador · Historial
FINANZAS: Decisiones · Préstamos · Patrocinadores · Televisiones · La Ciudad · Estadio
MANAGER: Info · Logros del mánager
ACTUALIDAD: Noticias · Hilos
```

**Dashboard (cards):** próximo partido (local/visitante, hora, competición), último resultado, clasificación (posición + contexto), objetivo de temporada, presupuesto y masa salarial, moral/forma, lesiones, noticias, hilos activos, logros pendientes.

**Bucle diario (`GameDayPipeline`):** `PreBatch` (recuperación, entrenamiento, mercado IA, ojeadores) → `LineupValidation` (once de 11 + banquillo válido) → `PreGameGate` → `SimulationBookkeeping` (transacción de un frame) → `PostDay` (fases, premios del mes, calendario, finanzas).

**Extra:** simulación rápida hasta fecha (reutiliza `FastSimRoutine` y `CalendarController`).

**Aceptación:** avanzar un día no rompe nada, el header refresca, los modales bloquean el avance y no hay doble pre-lote.

**F7 — decisiones tomadas:**

- **Réplica de la pantalla de Tactical5**: mismo layout (header, sidebar con submenús, paneles de último/próximo partido, estadísticas del equipo, clasificación, estadísticas de jugadores, relaciones y noticias). La sidebar se inyecta en runtime desde `UI/Resources/UI/Core/Sidebar.uxml` (`SidebarController`), como en Tactical5.
- **Adaptación al fútbol**: las tarjetas de jugador pasan de Puntos/Rebotes/Asistencias/Robos/Tapones/Valoración a **Goles/Asistencias/Valoración/MVP/Partidos/Amarillas**; la clasificación usa PJ-G-E-P-DG-PTS; el sidebar sustituye Pabellón por **Estadio**, Quinteto por Alineación, Rotación por Táctica, Empleados por Cuerpo técnico, G-League por Filial/Cantera y Playoffs por Eliminatorias. Los destinos sin pantalla avisan con un toast.
- **Reglas puras**: `LeagueTable` calcula la clasificación desde `games` jugados; `SeasonObjective` centraliza los **6 objetivos** (Campeonato, Europa, Media tabla, Permanencia, Ascenso y Playoff) y lo comparten la ficha de equipo y el Dashboard; `GameDayRules` resuelve fecha siguiente, fase y detección de día de partido.
- **Avance de día** (`DatabaseManager.AdvanceDay`): mueve `seasons.current_date` y `current_match_day`, recalcula `phase` (`preseason` hasta el primer partido de liga; `regular` después) y **se detiene en el día de partido**. Sin motor (F8) el botón explica que la simulación llega en la fase siguiente; cuando F8 exista solo hay que enlazar el `PreGame`.
- **Clasificación**: se muestran los 6 primeros y, si el club queda fuera, una fila separada con su posición. En pretemporada el objetivo se calcula por media de plantilla (igual que la ficha de F5).
- **Estado en SQLite**: cada avance actualiza el slot (`saves.json`) para que Cargar partida muestre la fecha real.
- **Escena**: `DashboardDocument` (UIDocument + FullScreenUI + DashboardController) creado y enlazado a `ScreenManager.dashboardDocument`.

---

### HITO 1 — Corte vertical jugable

Con F0-F9 se puede **jugar una temporada completa de liga** (sin copa ni Europa), con partidos simulados, clasificación y fin de temporada básico. Es el primer punto en el que el juego "se siente" terminado.

---

### F8 — Motor de partido y pantallas de partido

**Objetivo:** simular fútbol de 90 minutos y mostrarlo.

**Origen:** `Scripts/GameSimulator.cs`, `GameResultCache.cs`, `PlayByPlayNarrator.cs`, `IRandomSource.cs`, `Scripts/Stats/MatchupPreview.cs`, `UI/Screens/{PreGame, MatchDay, GameResults}`, `MatchDayController(.Visual.cs)`.

**Nuevo: `FootballSimulator`**
- **Formaciones**: 4-4-2, 4-3-3, 4-2-3-1, 3-5-2, 5-3-2 (slot por posición; `FormationRules`).
- **Minutos**: 90 + descuento + prórroga y penaltis en eliminatorias.
- **Eventos**: gol, ocasión, parada, palo, córner, falta, amarilla, segunda amarilla, roja, penalti, fuera de juego, lesión, cambio, tiempo añadido.
- **Estadísticas**: posesión, tiros, tiros a puerta, paradas, córners, faltas, fuera de juego, tarjetas, pases, precisión de pase, xG (opcional), nota por jugador (1-10).
- **Determinismo**: `IRandomSource` inyectable (`SeededRandomSource`) para reproducir un partido.
- **IA de banquillo**: cambios por fatiga/resultado; `MatchupPreview` (probabilidad de victoria/empate, favorito).
- **Rendimiento**: simulación en main thread; captura de play-by-play solo si la vista lo necesita.

**Pantallas** (replicando estructura y estilos de T5):
- `PreGame`: previa con onces, bajas, racha, pronóstico y estadio.
- `MatchDay`: marcador, reloj, crónica (play-by-play), estadísticas en vivo y cambios.
- `GameResults`: resultado final, goleadores, notas, estadísticas y MVP.

**Fuera de alcance inicial:** vista 2D animada (campo con jugadores). Se deja como fase opcional posterior (F8b), con un `Match2DChoreographer` equivalente al `Sim2D` de T5.

**Aceptación:** 1.000 partidos sintéticos con medias realistas (≈2,6 goles/partido), sin empates imposibles en eliminatorias; partido reproducible con la misma semilla; suite de tests de reglas.

**F8 — estado actual (motor):**

- **`FootballSimulator`** (puro): simula 90 minutos desde los onces, con posesión, xG, goles, tiros, paradas, córners, faltas, tarjetas, cambios, notas 1–10 y MVP. Determinista con `IRandomSource`.
- **`FormationRules`**: 4-4-2, 4-3-3, 4-2-3-1, 3-5-2 y 5-3-2 con 11 huecos y compatibilidad por demarcación (natural, línea, contigua, portero).
- **`LineupBuilder`**: once y banquillo automáticos (el mejor jugador disponible por hueco). F10 lo sustituirá por la alineación guardada.
- **`MatchupPreview`**: probabilidades local/empate/visitante por Poisson con ventaja de campo.
- **`DatabaseManager.MatchEngine`**: `SimulateGame` persiste resultado, eventos, estadísticas individuales y acumulados de temporada; `PlayToday` simula la **jornada completa** (mismo tipo y número) para que la clasificación sea coherente.
- **Dashboard**: el botón JUGAR PARTIDO ya simula el encuentro y muestra el resultado; después CONTINUAR avanza el día.
- **Pantallas** (réplica de Tactical5 adaptada al fútbol): `PreGame` (onces, banquillos, bajas, forma, jugadores clave y pronóstico), `MatchDay` (marcador, estadísticas de equipo, cronología y estadísticas individuales) y `GameResults` (resto de la jornada, MVP y líderes de goleadores/asistentes/notas). Flujo: Dashboard → PreGame → MatchDay → GameResults → Dashboard.
- **Pendiente de F8**: play-by-play interactivo, cambios en directo y vista 2D (F8b).

> **F8b en pausa (decisión actual).** Antes de seguir con la simulación se implementan todas las
> pantallas del sidebar. El plan detallado, con fases S0–S9 y checklist, está en
> **`Docs/PLAN_PANTALLAS_SIDEBAR.md`**.

---

### F9 — Clasificaciones, resultados e info de competición

**Origen:** `UI/Screens/{Standings, Results, Calendar, InfoLeague}` + `StandingsController`, `ResultsController`, `CalendarController`, `InfoLeagueController`, `StandingsHelper`.

**Cambios:**
- Clasificación por competición y grupo; desempate **particular → diferencia de goles → goles a favor** (fútbol, no average de baloncesto).
- Zonas coloreadas: Champions, Europa, Conference, descenso (según competición).
- Resultados por jornada con todos los partidos y acceso al resumen.
- Info de competición: formato, plazas europeas, ascensos/descensos, palmarés.

**Aceptación:** clasificación correcta tras cualquier jornada; desempates probados; calendario navegable por meses.

---

### F10 — Plantilla, alineación, táctica y dorsales

**Origen:** `UI/Screens/{Roster, Quinteto, Rotation, Training, Employees, Injured, Dorsales, Cartera}` + controllers + `LineupBuilder`, `RotationPlanner`.

**Modelo existente:** `lineups` (`TeamId`, `PlayerId`, `FieldSlot`) sirve para el XI; **ampliar** con `Slot` (0 = once, 1 = banquillo, 2 = no convocado) y `Order`, para poder guardar el banquillo de 7 y la convocatoria.

**Nuevo:**
- **Alineación (`Lineup`)**: once por formación + **7 suplentes**; arrastrar/seleccionar; validación de posiciones (fuera de posición penaliza).
- **Táctica (`Tactics`)**: formación, estilo (posesión/contragolpe/presión), intensidad, línea defensiva, balón parado, marcaje; reemplaza la pantalla ROTACIÓN (no hay minutos por jugador en fútbol).
- **Plantilla (`Roster`)**: atributos, forma, moral, contrato, cláusula, valor, minutos, notas medias.
- **Entrenamiento (`Training`)**: foco semanal (físico, técnico, táctico, recuperación) con mejora acotada.
- **Cuerpo técnico (`Staff`)**: entrenador (bonus táctico), segundo, preparador físico, médico, ojeadores, cantera.
- **Enfermería (`Injured`)**: lesiones, evolución, alta forzada con riesgo de recaída.
- **Dorsales**: inscripción **1-25** (1/13/25 porteros); canteranos promocionables.

**Aceptación:** el once guardado es el que juega; las posiciones inválidas avisan; los dorsales no se duplican; lesiones y sanciones excluyen al jugador.

---

### F11 — Mercado: traspasos, cláusulas, cesiones y agentes libres

**Origen:** `UI/Screens/Market/*` + `MarketController` + `TradeHelper`, `TradeValueHelper`, `AiMarketService`, `NegotiationHelper`, `FreeAgencyRules`, `BuyoutMarketRules`, `ContractMarket`.

**Modelos ya existentes:** `offers` (`ManagerId`, `PlayerId`, `FromTeamId`, `ToTeamId`, `TransferFee`, `WeeklyWage`, `ContractYears`, `Status`, `MatchDay`) y `loans`. Se amplían con `OfferType` (traspaso/renovación/cesión), `ReleaseClause`, `SellOn`, `LoanOptionToBuy`, `LoanWageSplit` y `MaturesOnDate`.

**Nuevo modelo de negocio:**
- **Ventanas**: verano (1 jul – 1 sep) e invierno (1 – 31 ene). Fuera de ventana solo se puede negociar (acuerdo para la siguiente).
- **Traspaso**: oferta (importe + variables), negociación con club y agente, contrato (años, salario, prima, cláusula).
- **Cláusula de rescisión**: cualquier club puede pagarla si tiene presupuesto; el jugador debe aceptar el contrato. El club vendedor no puede negarse.
- **Cesión**: 1 temporada, % de salario, opción de compra, sin coste de traspaso.
- **Agente libre**: sin club, negociación directa.
- **Renovación**: el club ofrece; el jugador pide según rendimiento/edad/minutos.
- **IA de mercado**: estrategia por club (`Rebuild/Balanced/Contend` equivalentes: `Cantera/Consolidado/Aspirante`), cooldowns, mercado por demarcación y presupuesto.

**UI (mismo patrón que T5):** modal **PROPONER TRASPASO** con paneles por club. Campos por equipo:
`VALOR PLANTILLA · SALARIO · PRESUPUESTO · JUGADORES (25/25)` y, por jugador, `VALOR · SALARIO · CLAÚSULA · CONTRATO · EDAD`.

**Aceptación:** no se puede fichar fuera de ventana ni superar 25 fichas; una cláusula pagada ejecuta el traspaso; la IA no acepta ofertas absurdas; el estado se revierte si falla la transacción.

---

### F12 — Finanzas, estadio, ciudad, patrocinio y TV

**Origen:** `UI/Screens/{Finances, Loans, Sponsors, TV, Arena, TheCity}` + `FinancesController`, `LoansController`, `SponsorsController`, `TVController`, `ArenaController`, `TheCityController`, `TeamEconomyService`, `MarketEconomy`, `CityCatalog/CityService`.

**Cambios de dominio:**
- **Ingresos**: taquilla (aforo, precio, abonados), abonos, patrocinio (camiseta, estadio, naming), TV (reparto por competición), traspasos, europeo.
- **Gastos**: salarios, amortización de fichajes, primas, seguridad, mantenimiento del estadio, viajes, cantera.
- **Fair play financiero** (sustituye a cap/apron): límite de coste de plantilla calculado sobre ingresos; avisos y sanciones (inscripción restringida, límite de fichajes). **Sin margen salarial NBA.**
- **Estadio**: aforo, ampliaciones, obra (imágenes `Stadiums/*-construction`), abonos.
- **Ciudad**: proyectos (ciudad deportiva, cantera, tienda, museo) con efectos acotados (asistencia, moral, cantera).
- **Préstamos bancarios**: importe, interés, plazo.

**Aceptación:** cuentas cuadran por temporada; el balance mensual no duplica ingresos; el límite financiero bloquea fichajes coherentemente.

---

### F13 — Copa del Rey y Supercopa

**Origen:** `Scripts/Core/CupHelper.cs`, `DatabaseManager.Cup.cs`, `UI/Screens/Playoffs/*` (como base de bracket) y `TournamentData`.

**Nuevo: `CopaDelReyGenerator`**
- **82 participantes** (los 20 de 1ª + 22 de 2ª + 40 de 1ª RFEF).
- **Cuadro fijo:** ronda preliminar con **18 eliminatorias** entre clubes de 1ª RFEF (36 participantes; los otros 4 y los 42 de 1ª/2ª quedan exentos) → **64** → primera ronda (32) → dieciseisavos (16) → octavos (8) → cuartos (4) → **semifinales a ida/vuelta** → **final** a partido único en sede neutral.
- **Sorteos** con condicionantes: el club de menor categoría (o el peor clasificado) actúa de local; en la preliminar, los 4 exentos de 1ª RFEF rotan por temporada.
- Prórroga y penaltis en todas las eliminatorias.

**Supercopa:** 4 equipos (campeón y subcampeón de liga, campeón y subcampeón de copa), semifinales y final en enero.

**UI:** pantalla de eliminatorias con bracket y resultados; se puede reutilizar el contenedor de `Playoffs` con pestañas.

**Aceptación:** una copa completa sin equipos duplicados; el campeón se guarda en palmarés; la copa no altera la clasificación de liga.

---

### F14 — Competiciones europeas

**Origen:** `DatabaseManager.Cup.cs`/`.GLeague.cs` (patrón de competición paralela sin tocar `seasons.phase`), `GLeaguePostSeason`, `PlayoffsGenerator`.

**Nuevo:**
- **Participación**: **7 plazas españolas** cada temporada, calculadas desde la clasificación final de LaLiga (no desde un campo fijo del equipo):
  - **Champions League**: 1º, 2º, 3º y 4º → 32 clubes extranjeros sembrados + 4 españoles = **36**.
  - **Europa League**: 5º y 6º → 34 extranjeros + 2 españoles = **36**.
  - **Conference League**: 7º → 35 extranjeros + 1 español = **36**.
  - El campeón de Copa **no** da plaza europea (decisión de diseño: las 7 plazas son por liga).
- **Fase de liga única (formato 2024+)**: 36 equipos, 8 partidos (2 por bombo), sorteo con restricciones (no mismo país, no repetir rival), clasificación única.
- **Eliminatorias**: 1º-8º a octavos; 9º-24º playoff; 25º-36º eliminados.
- **Sorteo** reproducible por semilla; calendario martes/miércoles (Champions) y jueves (Europa/Conference), sin solapar con liga.
- **Coeficiente** por país: simplificado y fijo por seed (solo se usa para el reparto de bombos; las plazas españolas ya están fijadas en 4/2/1).

**UI:** pantalla `Europe` con fase de liga (tabla de 36), bracket y resultados; reutiliza estilos de `Standings` + `Playoffs`.

**Aceptación:** 8 partidos por equipo, sin repetir rival ni país (cuando sea posible); el campeón se guarda en palmarés; no toca `seasons.phase` de la liga.

---

### F15 — Fin de temporada, ascensos/descensos y rollover

**Origen:** `UI/Screens/{SeasonSummary, PlayerAwards, EndSeason, NewSeason, Premios, Palmares, Records, Historial}`, `DatabaseManager.Rollover.cs`, `ObjectiveHelper`, `RolloverRules`.

**Nuevo: `PromotionRelegationRules`**

| Competición | Ascensos | Descensos |
|---|---|---|
| LaLiga (1ª) | — | 3 (18º-20º) → 2ª |
| LaLiga Hypermotion (2ª) | 3 (1º, 2º y ganador del playoff 3º-6º) | 4 → 1ª RFEF |
| 1ª RFEF (2 grupos) | 4 (2 campeones + 2 del playoff) | 5 por grupo → **pool de Tercera** |
| Pool de Tercera (no simulada) | 10 (rellenan 1ª RFEF) | — |

- El **playoff de ascenso** a 2ª es a doble partido (3º-6º y 4º-5º; final a doble partido).
- **No hay 2ª RFEF**: la 1ª RFEF es la última categoría simulada. Los 10 descendidos (5 por grupo) salen del juego y entran 10 clubes del **pool de Tercera** (`NamePools` + escudo genérico, con plantilla generada al ascender). Así la 1ª RFEF se mantiene siempre en **40 clubes**.
- Cuadre anual: 1ª −3/+3 · 2ª −7/+7 (3 arriba, 4 abajo) · 1ª RFEF −14/+14 (4 arriba, 10 abajo; 10 del pool).
- **Rollover** (`StartNewSeason`): archivar stats, premios, palmarés, ascensos/descensos, recalcular presupuestos y objetivos, renovar patrocinios/TV, generar canteranos, nuevo calendario, reset de lesiones/físico.

**Pantallas:** resumen de temporada (campeón, ascensos, descensos, premios), fin de temporada (premios individuales), nueva temporada (ofertas de club, elección de equipo en ProManager), palmarés y récords actualizados.

**Aceptación:** 5 temporadas seguidas sin descuadres de plantilla; la pirámide mantiene exactamente **20/22/40**; nadie asciende dos veces ni desaparece; los clubes del pool reciben plantilla al entrar.

---

### F16 — Staff, ojeadores, entrenamiento y cantera/filial

**Origen:** `DatabaseManager.Staff.cs`, `ScoutingHelper`, `ScoutData`, `FogOfWarHelper`, `TrainingData`, `GLeagueHelper` (para el filial), `SummerLeagueHelper` (para cantera).

**Nuevo:**
- **Cantera**: cada temporada genera 3-5 canteranos por club (edad 16-19), calidad según `youth_facilities`/`youth_recruitment`; se suben al primer equipo o al filial.
- **Filial**: equipo B participando en 1ª/2ª RFEF; el primer equipo puede **ceder jugadores al filial** (edad ≤23 y ficha) y recuperarlos; desarrollo acelerado con tope en `potential`.
- **Ojeadores**: regiones, informes, niebla de guerra en atributos hasta completar el informe.
- **Empleados**: contratación, reputación, efecto en entrenamiento/táctica/lesiones.

**Aceptación:** los canteranos aparecen en el resumen de temporada; el filial no puede ascender por encima de la categoría del primer equipo (regla real) — o se documenta la simplificación; el desarrollo respeta `potential`.

---

### F17 — Narrativa, logros, mensajes, buscador, perfil e historial

**Origen:** `StorylineService/Catalog/Context`, `MilestoneService/Catalog`, `QuickNewsGenerator`, `NewsTexts`, `AchievementCatalog/Service`, `MessagesController`, `BuscadorController`, `PlayerProfileController`, `TrajectoryController`, `RecordsController`, `PalmaresController`, `HallOfFameData`, `RivalryHelper`, `DynastyHelper`.

**Cambios:** tipos de noticia de fútbol (fichaje, lesión, racha, clásico, crisis, cantera), hitos (500 partidos, 200 goles, debut, ascenso), premios y récords (máximo goleador histórico, más partidos, más títulos).

**Aceptación:** el buzón agrupa y filtra; los logros se desbloquean una vez; el perfil respeta la niebla de guerra; los récords se calculan desde datos persistidos.

---

### F18 — Calibración, tests, rendimiento, localización y build

**Origen:** `MonteCarloRunner/Calibration`, `Tools/localization_extract.py`, `Tools/ci_unity.sh`, `Docs/MONTE_CARLO_LAB.md`, `Docs/CI.md`.

**Trabajo:**
- `FootballMonteCarloRunner`: 100 temporadas sintéticas con semilla → goles/partido, victorias locales, dispersión, lesiones, tarjetas; perfil de aceptación versionado.
- Suite EditMode completa (reglas puras: calendario, copa, europa, ascensos, mercado, contratos, overall, progresión, lesiones).
- Presupuesto de rendimiento: día simulado < 50 ms sin UI; rollover < 2 s.
- Localización: `{loc:}` + `es.json` + test que impide literales.
- Build **StandaloneLinux64 y StandaloneWindows64** (mismo código, SQLite nativo incluido en ambos) + verificación de licencias (`THIRD_PARTY_ASSETS.md`). Ver §4.1 para el checklist de SQLite por plataforma.

**Aceptación:** suite verde, calibración dentro de bandas, build reproducible.

---

## 8. Inventario de pantallas (Tactical Five → Tactical Eleven)

| # | T5 (`Screens/`) | T11 (`Screens/`) | Cambios de dominio |
|---|---|---|---|
| 1 | Loading | Loading | textos |
| 2 | MainMenu | MainMenu | Liga Manager / ProManager / Cargar / Editor / Salir |
| 3 | LoadGame | LoadGame | escudo, club, temporada |
| 4 | Editor | Editor | estadios, escudos, jugadores |
| 5 | SelectTeam | SelectTeam | filtros por competición (1ª, 2ª, 1ª RFEF G1 y G2) |
| 6 | Preseason | Preseason | 3-5 amistosos |
| 7 | Dashboard | Dashboard | próximo partido, clasificación, objetivo |
| 8 | Roster | Roster | atributos FM, cláusula, valor |
| 9 | Quinteto | **Lineup** | once + banquillo de 7 |
| 10 | Rotation | **Tactics** | formación/instrucciones (sin minutos) |
| 11 | Training | Training | foco semanal |
| 12 | Employees | **Staff** | entrenador, preparador, médico, ojeadores |
| 13 | Injured | Injured | catálogo de fútbol |
| 14 | Dorsales | Dorsales | inscripción 1-25 |
| 15 | Calendar | Calendar | jornadas, copa, europa |
| 16 | Results | Results | por jornada/competición |
| 17 | Standings | Standings | desempate particular |
| 18 | InfoLeague | **CompetitionInfo** | formato y plazas |
| 19 | GLeague | **Filial** | cesiones al filial + cantera |
| 20 | Rivalidades | Rivalidades | clásicos |
| 21 | Logros | Logros | logros del mánager |
| 22 | Palmares | Palmares | ligas, copas, europa |
| 23 | Records | Records | goleadores, partidos, títulos |
| 24 | Premios | Premios | MVP, Pichichi, Zamora, Muñoz |
| 25 | Playoffs | **Knockouts** | copa, europa y playoff de ascenso |
| 26 | Stats | Stats | estadísticas de fútbol |
| 27 | Market | Market | traspasos, cláusulas, cesiones, FA |
| 28 | Cartera | Cartera | contratos, cláusulas, valor |
| 29 | Finances | Finances | P&L de club |
| 30 | Loans | Loans | préstamos bancarios |
| 31 | Sponsors | Sponsors | camiseta, estadio, naming |
| 32 | TV | TV | reparto por competición |
| 33 | Arena | **Stadium** | aforo, obra, abonos |
| 34 | TheCity | **City** | proyectos de club |
| 35 | Manager | Manager | perfil y confianza |
| 36 | Messages | Messages | noticias y ofertas |
| 37 | MatchDay | MatchDay | 90', crónica |
| 38 | GameResults | GameResults | goleadores, notas |
| 39 | PreGame | PreGame | onces, pronóstico |
| 40 | SeasonSummary | SeasonSummary | ascensos/descensos |
| 41 | PlayerAwards | PlayerAwards | premios de la temporada |
| 42 | EndSeason | EndSeason | campeones y premios |
| 43 | NewSeason | NewSeason | nuevo calendario, cantera |
| 44 | Historial | Historial | temporadas del mánager |
| 45 | PlayerProfile | PlayerProfile | atributos, contrato, cláusula |
| 46 | Trajectory | Trajectory | evolución |
| 47 | Buscador | Buscador | filtros de fútbol |
| 48 | Quintos | **BestXI** | onces ideales |
| 49 | Hilos | Hilos | narrativa |
| 50 | — | **Europe** (nueva) | fase de liga + bracket europeo |
| 51 | — | **Copa** (nueva) | bracket de Copa del Rey |

> `LegalNotice/` sigue siendo carpeta huérfana: el aviso legal va inline en `MainMenu.uxml` (como en T5).

---

## 9. Modelo de datos (bloques)

**Ya existe en `Data/Models`** (copiar y ampliar):

| Bloque | Tablas existentes |
|---|---|
| Competición | `leagues` (liga/copa/europa en la misma tabla), `standings` |
| Club | `teams`, `team_settings` |
| Jugador | `players`, `injuries`, `player_season_stats`, `player_match_stats` |
| Partido | `games`, `match_events`, `lineups` |
| Mercado | `offers`, `loans` |
| Staff | `staff`, `employees` |
| Economía | `finance_records`, `sponsors`, `tv_channels` |
| Histórico | `finals_records`, `awards_records`, `golden_boot_records` |
| Meta | `managers`, `seasons`, `messages` |

**Tablas nuevas a añadir** (por fase):

| Bloque | Tablas nuevas | Fase |
|---|---|---|
| Competición | `competition_entries` (participación por temporada), `competition_editions`, `standings_snapshots` | F2/F14 |
| Jugador | `player_contracts`, `injury_history`, `player_milestones`, `player_relationships`, `player_personality` | F3/F16/F17 |
| Partido | `game_lineups`, `game_attendance` | F8 |
| Mercado | `transfer_offers`, `transfers`, `release_clause_activations`, `transfer_windows` | F11 |
| Staff | `employee_candidates`, `scouts`, `scouted_players` | F16 |
| Cantera | `youth_intake`, `filial_assignments` | F16 |
| Economía | `bank_loans`, `tv_deals`, `season_attendance` | F12 |
| Copa/Europa | `copa_rounds`, `european_league_phase`, `european_draws`, `knockout_ties` | F13/F14 |
| Histórico | `season_records`, `hall_of_fame`, `retired_numbers` | F15/F17 |
| Meta | `storylines`, `gm_achievements`, `schema_migrations`, `schema_migration_attempts`, `db_meta` | F1/F17 |

**Nombres a respetar (ya usados por el seed y el generador):** `leagues`, `teams`, `players`, `games`, `match_events`, `player_match_stats`, `player_season_stats`, `lineups`, `offers`, `standings`, `finals_records`, `awards_records`, `golden_boot_records`.

**Columnas/enums existentes que hay que mapear, no duplicar:**
- `PlayerEnums.Position` (GK=0 … CF=14) y `PlayerPositionFlags` para `SecondaryPositions`.
- `PlayerEnums.MatchEventType` para `match_events.EventType`.
- `PlayerEnums.MatchRating` (1-10) para la nota del jugador.
- `PlayerEnums.PlayerStatus` (Active/Injured/Loaned/Suspended/Retired) para `players.Status`.
- `PlayerEnums.InjurySeverity` para `injuries.Severity`.
- `Constants.CountryCodes` + `GetFlagPath` para nacionalidades y banderas.

Reglas heredadas que **no** se deben romper:
- `player_season_stats` y las tablas con **doble vía de creación** (si existen): actualizar `CreateTable` **y** el SQL crudo.
- Migraciones aditivas con `PRAGMA table_info` + `ALTER TABLE`.
- `seasons.phase` conduce el día; las competiciones paralelas (copa/europa) **no lo tocan**.
- Claves de ids estables (`team_id`, `player_id` manuales en el seed).

---

## 10. Reglas de competición (resumen operativo)

- **Liga**: doble vuelta; 3 puntos victoria, 1 empate; desempate **particular → diferencia → goles a favor**.
- **Pirámide**: 20 (1ª) / 22 (2ª) / 40 (1ª RFEF, 2 grupos de 20). No hay 2ª RFEF; los 10 descendidos de 1ª RFEF salen a un **pool de Tercera** no simulado.
- **Ascensos/descensos**: 1ª −3; 2ª +3/−4; 1ª RFEF +4/−10. El playoff de ascenso a 2ª se juega en junio (3º-6º).
- **Copa**: eliminatorias a partido único hasta cuartos; semifinales ida/vuelta; final neutral. Prórroga y penaltis.
- **Europa**: fase de liga de 8 partidos por equipo; 1º-8º a octavos, 9º-24º playoff, resto eliminado; eliminatorias a doble partido y final única. **Plazas españolas: 4 Champions + 2 Europa + 1 Conference (top 7 de LaLiga).**
- **Supercopa**: 4 equipos, enero, eliminatorias a partido único.
- **Ventanas**: verano 1 jul-1 sep; invierno 1-31 ene.
- **Inscripción**: máximo 25 fichas (1/13/25 porteros) + canteranos sub-23 con ficha del filial.
- **Fair play financiero**: límite de coste de plantilla sobre ingresos; sanciones graduales. **No hay margen salarial NBA.**

---

### ProManager (reglas propias, sin equivalentes NBA)

Lo que anuncia el modal y hay que implementar en las fases indicadas:

- **Solo Primera Federación** (grupos 1 y 2) y **sin filiales** (35 clubes jugables de 40): F5, hecho. La pantalla abre en el grupo 1 y el resto de clubes aparecen deshabilitados.
- **Objetivo exigente**: si no se cumple, la directiva puede destituir al final de la temporada (F15).
- **Cese por presupuesto**: dos avisos en rojo; en Manager son tres (F12).
- **Cambio de equipo cada temporada**: al terminar, quedarse o aceptar ofertas de clubes de la zona baja (F15).
- **Sin excepciones económicas**: no hay margen salarial extra ni fichajes por encima del presupuesto del club (F11/F12). En fútbol no existen MLE ni aprons.

## 11. Calendario tipo de una temporada

| Periodo | Contenido |
|---|---|
| Julio | Pretemporada, mercado de verano, cantera |
| Agosto | Amistosos, inicio de liga (mediados), ventana de fichajes hasta el 1 sep |
| Septiembre | Liga + Europa (fase de liga) + parón FIFA |
| Octubre-Noviembre | Liga + Europa + Copa (rondas iniciales) + parones FIFA |
| Diciembre | Liga + Europa + Copa; parón navideño |
| Enero | Liga + Copa + **Supercopa** + ventana de invierno |
| Febrero-Marzo | Liga + Europa (eliminatorias) + Copa; parón FIFA en marzo |
| Abril-Mayo | Liga (final), Copa (final), Europa (final), playoff de ascenso |
| Junio | Ascensos/descensos, premios, rollover y nueva temporada |

---

## 12. Tests y criterios de aceptación (transversal)

- **Puros (EditMode)**: `FootballScheduleGenerator`, `PromotionRelegationRules`, `CopaDelReyGenerator`, `EuropeanCompetitionGenerator`, `TransferRules`, `ContractRules`, `PlayerRating`, `FootballSimulator`, `LineupBuilder`, `InjuryCatalog`, `ObjectiveHelper`.
- **De persistencia**: `SaveSlotInitTests`, `DatabaseMigrationFailureTests`, `DatabaseIndexPlanTests`, `ReferentialIntegrityTests`.
- **De UI**: `LocalizationTests`, `UiStyleTests` (sin `rgba()`, `text-transform` correcto, modales dentro de 1700×675, scroll propio idempotente).
- **De calibración**: `FootballMonteCarloRunner` + perfil versionado.
- Regla: **ninguna fase se cierra sin sus tests en verde** y sin actualizar el doc de sistema correspondiente.

---

## 13. Riesgos y decisiones abiertas

**Deuda conocida de la capa `Data/` existente (resolver al copiar):**

- `LeagueDataDB` no distingue tipo de competición (liga/copa/europa) ni grupo → añadir `Type`, `Tier`, `GroupIndex`, `EuropeSlots`.
- `TeamDataDB` no tiene `Logo`/`StadiumImage` ni `IsFilial`/`ParentClubId`; `EuropeanCompetitionId` debe quedar como legado.
- `PlayerDataDB.Wage` es `int` → `long`. `AwardsRecord.Winner` y `GoldenBootRecord.Winner` son `string` → añadir `PlayerId`.
- `LineupDataDB` solo tiene `FieldSlot` (11 titulares) → añadir `Slot` (XI/banquillo/inactivo) y `Order` para el banquillo de 7.
- `ScheduleGenerator.GenerateCopaDelRey` solo genera **una ronda**; la copa completa va en F13.
- `ScheduleGenerator` usa `UnityEngine.Random` y no valida el calendario → inyectar `IRandomSource` y añadir `ValidateSchedule`.
- `DatabaseManager.cs` no tiene `schema_migrations`, `db_meta` ni transacciones atómicas → F1.
- **Nacionalidades y banderas (resuelto)**: los PNG de `Flags/Default` y `Flags/Large (2×)` ya están en **ISO3** (`ARG`, `DEU`, `GBR`…); `Constants.NormalizeFlagCode` mapea los códigos FIFA del seeder (`GER`→`DEU`, `NED`→`NLD`, `ENG`→`GBR`…) y `GetFlagPath`/`GetFlagPathLarge` apuntan a las carpetas correctas. Queda como tarea de F3 revisar los códigos raros (`KVX`, `UN`).
- **Assets copiados sin `.meta`**: se importan como sprite múltiple y la UI recorta (logo de Loading). Copiar el `.meta` original o fijar `spriteMode: 1`.
- **Modales: nunca `style="display: none"` inline en `.modal-overlay`** (gana a `modal-overlay--visible` y el modal no se abre). La clase ya los oculta; los helpers ponen además `style.display` explícito. Cubierto por `UiSmokeTests.ModalOverlays_DoNotUseInlineDisplayNone`.

**Decisiones abiertas:**

1. **Política de descensos desde 1ª RFEF**: al no existir 2ª RFEF, los 10 descendidos salen a un **pool de Tercera** no simulado y entran 10 clubes de ese pool (con plantilla generada). Decidido; queda elegir nombres reales históricos o generados.
2. **Europa**: formato de liga único (36) es fiel pero costoso; alternativa de grupos de 4 como plan B si el calendario se complica.
3. **Filiales**: regla real (el filial no puede jugar en la misma categoría que el primer equipo). Simplificar o implementar.
4. **Cláusulas**: ¿siempre ejecutables o solo con ventana abierta? Decisión: ejecutables siempre que el jugador acepte y el club pagador tenga presupuesto.
5. **Motor de partido**: realismo vs. velocidad. Objetivo inicial 2,6 goles/partido y 90 eventos máx.
6. **Vista 2D del partido**: fase opcional posterior; el motor debe emitir metadatos desde el principio (`MatchEventData` con coordenadas/zonas) para no rehacerlo.
7. **Assets de los clubes del pool**: pipeline de escudo genérico y de fotos de jugador.
8. **7 plazas europeas**: no hay plaza para el campeón de Copa si queda fuera del top 7 (simplificación consciente).

---

## 14. Anexos

### 14.1 Qué reutilizar del intento anterior

Detalle completo en **§1.2.1**. Resumen:

| Archivo | Decisión |
|---|---|
| `Data/Models/*.cs` | ✅ Copiar las 25 tablas y ampliar columnas |
| `Data/PlayerSeeder.cs` | ✅ Rescatar; adaptar `Wage` a `long` y recalcular `overall` con `PlayerRating` |
| `Data/PalmaresSeeder.cs` | ✅ Rescatar → `finals_records` |
| `Data/GoldenBootSeeder.cs` | ✅ Rescatar → `golden_boot_records` |
| `Data/ScheduleGenerator.cs` | ⚠️ Rescatar y endurecer (validación, fechas, semilla, copa completa) |
| `Data/GameSaveManager.cs` | ✅ Rescatar |
| `Data/Constants.cs` / `PlayerEnums.cs` | ✅ Rescatar |
| `Data/SQLite.cs` | ✅ Ya copiado al proyecto |
| `Data/DatabaseManager.cs` | ⚠️ Reescribir como partials, reutilizando consultas |
| `Data/SQLiteAsync.cs` | ❌ No copiar (muerto) |
| `Data/PlayerData.cs` / `TeamData.cs` | ❌ No copiar (POCOs legacy que duplican los modelos) |
| `Data/SaveSlotInfo.cs` | ✅ Rescatar |
| `Teams/*`, `Stadiums/*`, `Icons/*`, `Flags/*`, `Audio/*` | ✅ Copiar |

### 14.2 Claves de localización

- Prefijo por pantalla: `ui.<pantalla>.<slug>` (generado por `Tools/localization_extract.py`).
- Prefijos de sidebar: `ui.core_sidebar.*`.
- Nuevas claves de fútbol: `ui.market.clausula`, `ui.market.cesion`, `ui.market.presupuesto`, `ui.lineup.once`, `ui.tactics.formacion`, `ui.standings.champions`, `ui.standings.descenso`…

### 14.3 Nombres de pantalla (`GameScreen`)

```
Loading, MainMenu, LoadGame, Editor, SelectTeam, Preseason, Dashboard, Roster, Lineup,
Tactics, Training, Staff, Injured, Dorsales, Calendar, Results, Standings, CompetitionInfo,
Filial, Rivalidades, Logros, Palmares, Records, Premios, Knockouts, Europe, Copa, Stats,
Market, Cartera, Finanzas, Loans, Sponsors, TV, Stadium, City, Manager, Messages,
MatchDay, GameResults, PreGame, SeasonSummary, PlayerAwards, EndSeason, NewSeason,
Historial, PlayerProfile, Trajectory, Buscador, BestXI, Hilos
```

---

**Siguiente paso recomendado:** ejecutar **F0 + F1** (fundaciones y persistencia). La pirámide queda cerrada en **1ª / 2ª / 1ª RFEF** y las plazas europeas en **4 + 2 + 1** para los 7 primeros de LaLiga.
