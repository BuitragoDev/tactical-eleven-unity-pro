# AGENTS.md — Tactical Eleven

> Instrucciones de contexto para agentes IA y desarrolladores que trabajen en este proyecto.
> Se carga automáticamente en cada sesión de opencode (raíz del repo).
> **La fuente de verdad del alcance y del orden de trabajo es `Docs/PLAN_REPLICA.md`.**
> Léelo antes de tocar nada: contiene las 18 fases, las pantallas, el modelo de datos y las
> reglas de negocio (ligas, copa, Europa, ascensos/descensos, mercado, contratos).

---

## 1. Qué es este proyecto (30 segundos)

**Tactical Eleven** — simulador de gestión de fútbol español, single-player, en español.
Réplica de la arquitectura y la UI de **Tactical Five** (baloncesto), cambiando el dominio:
fútbol en lugar de baloncesto.

- **Motor:** Unity **6000.3.15f1**, proyecto URP 2D.
- **Arquitectura:** **una única escena** (`Assets/_TacticalEleven/Scenes/MainMenu.unity`) con
  pantallas UI Toolkit como GameObjects `UIDocument`; **todo el estado vive en SQLite**.
- **Ligas:** 1ª División (20), 2ª División (22) y 1ª RFEF (2 grupos × 20). **No hay 2ª RFEF**:
  los descendidos de 1ª RFEF salen a un pool de Tercera no simulado.
- **Competiciones:** LaLiga, LaLiga Hypermotion, 1ª RFEF, Copa del Rey, Supercopa y las tres
  europeas con fase de liga de 36 (Champions, Europa y Conference).
- **Plazas europeas:** los **7 primeros** de LaLiga → **4 Champions + 2 Europa + 1 Conference**.
- **Sin draft ni salary cap NBA:** cláusulas de rescisión, ventanas de fichajes, cesiones,
  cantera, fair play financiero.
- **Multiplataforma:** **Windows x64 y Linux x64** (SQLite nativo incluido para ambas).

---

## 2. Base de conocimiento

| Doc / archivo | Contenido | Cuándo leerlo |
|---|---|---|
| **`Docs/PLAN_REPLICA.md`** | **Plan completo por fases (F0–F18)**, pantallas, modelo de datos, reglas de competición, decisiones y deudas | **Siempre, antes de empezar** |
| `AGENTS.md` (este) | Reglas rápidas, arquitectura e invariantes | Cada sesión (auto) |
| `Assets/_TacticalEleven/Scripts/Core/UIScreenController.cs` | Pipeline base de todas las pantallas | Tareas de UI |
| `Assets/_TacticalEleven/Scripts/Core/ScreenManager.cs` | Registro de pantallas y navegación | Añadir/cambiar pantallas |
| `Assets/_TacticalEleven/Scripts/Data/DatabaseManager*.cs` | Único acceso a la base de datos | Datos, migraciones, seed |
| `Assets/_TacticalEleven/Scripts/Data/SeedBaseline.cs` / `SeedAudit.cs` | Huella y auditoría del seed | Cambiar seed/template |
| `Assets/_TacticalEleven/Resources/Localization/es.json` | Tabla de textos | Cualquier texto visible |

> Estrategia de carga: lee primero `Docs/PLAN_REPLICA.md` y luego solo el archivo que toque.
> No es necesario releer todo el código ni todos los docs en cada tarea.

---

## 3. Hechos arquitectónicos esenciales (NO contradecir)

| Hecho | Consecuencia |
|---|---|
| **Escena única** `MainMenu.unity`; pantallas como `UIDocument` | No cargues escenas. Navega siempre con `ScreenManager.Instance.GoTo(GameScreen.X)`. |
| **`ScreenManager` con un campo `[SerializeField] UIDocument` por pantalla** (51 campos, agrupados por `Header`) | Las pantallas se crean en el Editor y se **arrastran al Inspector**. Si el campo está vacío, `GoTo` avisa y no cambia. |
| **Todas las pantallas heredan `UIScreenController`** | Pipeline en `OnEnable`: fullscreen → `CacheReferences` → `InitConfigModal` → `LoadData` → `RegisterCallbacks` → `Refresh` → localización. Sobrescribe solo lo que difiera. **Nunca inicialices en `Start`.** |
| **Persistencia única en SQLite** vía `DatabaseManager.Instance` (`partial` por dominio) | Nadie abre su propia conexión. Los lotes pesados usan `RunInBackground*` (conexión ambient `AsyncLocal` + WAL). |
| **`template.db` + slots** (`save_{n}.db` + `saves.json`) | La huella del seed (`SeedBaseline.Fingerprint`) decide si el template se reconstruye. |
| **Sin event bus** | Comunicación: `MessageData`, estado estático (`GameResultCache`…), `PlayerPrefs` (solo ajustes) y callbacks de UI. |
| **Sin namespaces**; clases globales | Referencia por nombre (`ScreenManager`, `DatabaseManager`). |
| **Reglas puras en clases static** | `PlayerRating`, `FilialRules`, `CalendarHelper`, `SeedBaseline`, `PlayerPositionNames`… Los controllers quedan delgados. |
| **Sin prefabs ni ScriptableObjects de juego** | Contenido = seeders + SQLite; vistas = UXML + USS. |

---

## 4. Convenciones que DEBES respetar

- **Idioma:** strings de juego y comentarios en **español**; estructura de código en inglés es aceptable.
- **Textos de UI por claves:** UXML con `text="{loc:clave}"`; C# con `LocalizationService.T("clave")`
  o `T("clave", args)` (plantillas `{0}`). La tabla base es `Resources/Localization/es.json`.
  **No añadas literales visibles** en UXML ni en controllers.
- **Fechas** `"yyyy-MM-dd"`. **Dinero** `$"{valor:N0} €"` (siempre con formato `N0` y sufijo `€`).
- **Modales:** overlay + caja con las clases compartidas `modal-overlay--visible` /
  `modal-box--visible` (estilos en `UI/Styles/Modals.uss`, importados por el tema).
  **Nunca pongas `style="display: none"` inline en un `.modal-overlay`**: gana a la clase y el
  modal no se abre. Los helpers ponen además `style.display`.
- **Assets:** copiar siempre el **`.meta`** junto al PNG (sprite simple). Si se importa como
  sprite múltiple, la UI muestra un recorte (pasó con el logo de Loading).
- **Aleatoriedad:** `UnityEngine.Random` en main thread; `System.Random` (thread-static) en
  background. El simulador de partido aceptará `IRandomSource` (F8) para reproducibilidad.
- **Singletons:** `public static X Instance { get; private set; }` + guard en `Awake` +
  `DontDestroyOnLoad` propio.
- **Estilo:** indentación 4 espacios; separadores `// ── NOMBRE ─────────────`.

---

## 5. Estado actual (resumen; detalle en `Docs/PLAN_REPLICA.md`)

| Fase | Estado | Contenido |
|---|---|---|
| F0 | ✅ | Fuentes, tema, panel settings, estilos, core (ScreenManager, UIScreenController, Audio, Cursor, Localización, scroll/slider, CalendarHelper) |
| F1 | ✅ | 25 tablas, `DatabaseManager` (fachada + migraciones + seed), slots, `template.db`, auditoría |
| F2 | ✅ | 9 competiciones, **236 clubes** (82 españoles + 101 europeos + 53 históricos), 233 escudos + genérico, 10 estadios |
| F3 | ✅ | **4.184 jugadores**, `PlayerRating`, 437 finales, 69 premios y 32 Pichichis |
| F4 | ✅ | Menú, modal de configuración, Cargar partida, aviso legal, salir |
| F5 | ✅ | Selección de equipo (4 pestañas), filiales deshabilitados, ProManager (solo 1ª RFEF), manager + temporada, plantilla con banderas |
| **F6** | ✅ | **Pretemporada y calendario de liga**: doble vuelta 38/42/38, parones FIFA/Navidad, 48 h de descanso, pretemporada vacía de hasta 4 amistosos con filtros y pantalla `Preseason` |
| **F7** | ⏭ **Siguiente** | **Dashboard, header, sidebar y avance de día** |
| F8…F18 | — | Motor de partido, competiciones, mercado, finanzas, copa, Europa, rollover, staff/cantera, narrativa y build |

**Tests:** `Assets/_TacticalEleven/Tests/Editor/` (7 archivos, sin `.asmdef`; compilan en
`Assembly-CSharp-Editor`). Comando (con **Unity cerrado**):

```bash
~/Unity/Hub/Editor/6000.3.15f1/Editor/Unity -batchmode -projectPath . \
  -runTests -testPlatform EditMode \
  -testResults /tmp/opencode/editmode.xml -logFile /tmp/opencode/editmode.log
```

---

## 6. Cómo añadir una PANTALLA (Editor + código)

1. `UI/Screens/<Pantalla>/<Pantalla>.uxml` + `.uss` (reutiliza `GlobalVariables`, `Typography`,
   `Utilities`; textos con `{loc:clave}`).
2. En la escena `MainMenu.unity`: GameObject `<Pantalla>Document` con
   - `UIDocument` → Panel Settings `TacticalElevenPanelSettings`, Source Asset el UXML,
   - `FullScreenUI`,
   - `<Pantalla>Controller : UIScreenController` (sobrescribe `ScreenId`).
3. Arrastrar el GameObject al **campo del `ScreenManager`** correspondiente (ya existen los 51
   campos; si añades una pantalla nueva, añade el valor a `GameScreen`, el campo y el `case` de
   `ScreenManager.Resolve`).
4. Navegar con `ScreenManager.Instance.GoTo(GameScreen.<Pantalla>)`.

El controller re-inicializa todo en `OnEnable`; el árbol de UI se recrea en cada activación, así
que no se acumulan callbacks entre visitas.

---

## 7. Cómo añadir una MECÁNICA / REGLA

1. **Modelo** `[Table]` en `Scripts/Data/` (o `Models/`) + `CreateTable<T>()` en
   `DatabaseManager.CreateTables`.
2. **CRUD** en el partial correcto de `DatabaseManager`.
3. **Migración**: columna aditiva con `PRAGMA table_info` + `ALTER TABLE` en
   `DatabaseManager.Migrations.cs`, o migración de datos con nombre en `schema_migrations`
   (`IsMigrationApplied` / `MarkMigrationApplied`). `PRAGMA user_version` solo avanza si todo va bien.
4. **Reglas puras** en una clase static nueva (estilo `PlayerRating` / `FilialRules`).
5. **Engancha** en el punto adecuado: día de partido (F7+), `StartNewSeason` (F15) o acción de UI.
6. **Feedback** con `MessageData` y refresco de la pantalla.
7. **Tests** EditMode de las reglas puras y del flujo de datos.

---

## 8. Invariantes críticas (no romper)

1. **`overall` = `PlayerRating`** (fuente única): campo = 8 físicos + 14 técnicos + 14 mentales;
   portero = 14 mentales + 8 específicos; **capado por `Potential`**. Tras mutar atributos llama a
   `RecalculateOverall()`. No dupliques la fórmula (`GetCalculatedAverage` ya delega).
2. **Huella del seed calculada** (`SeedBaseline.Fingerprint` = MD5 de competiciones + equipos +
   palmarés + premios). Si cambia el seed, el template se reconstruye solo. El seed de 4.184
   jugadores **no** se hashea: **sube `SeedBaseline.PlayerSeedVersion`** al tocar `PlayerSeeder`.
3. **Filiales** (`FilialRules`): 7 filiales marcados (`IsFilial`, `ParentClubId`). Se muestran
   deshabilitados y **nunca** son seleccionables. En **ProManager solo 1ª RFEF** (grupos 1 y 2):
   35 clubes jugables de 40. `CanSelect` es la única regla.
4. **Banderas en ISO3** (`Flags/Default/{ISO3}`, `Flags/Large (2×)/{ISO3}`) con alias FIFA→ISO3 en
   `CountryCodes.NormalizeFlagCode` (`GER→DEU`, `NED→NLD`…). `default` va en minúsculas (Linux).
   Para mostrar países usa **`CountryCodes.GetNameEs`** (acepta ISO3, inglés o español).
5. **`PlayerPosition`**, no `Position`: un enum global `Position` ocultaba
   `UnityEngine.UIElements.Position` y rompía el fullscreen.
6. **`Constants.cs` declara la clase `CountryCodes`** (no `Constants`).
7. **SQLite:** `PRAGMA journal_mode` / `busy_timeout` con `ExecuteScalar` (con `Execute`,
   sqlite-net lanza "not an error" al devolver fila). Nunca anides transacciones en el seed.
8. **`seasons.phase`** gobernará el día (`preseason → regular → …`); las competiciones paralelas
   (Copa/Europa) **no** lo tocan.
9. **Europa:** 36 equipos por competición; plazas españolas 4/2/1 por clasificación final, no por
   campo fijo del club.
10. **No hay 2ª RFEF**; el pool de Tercera rellena las 10 plazas de descenso de 1ª RFEF.
11. **Multiplataforma:** cualquier ruta de guardado con `Path.Combine(Application.persistentDataPath, …)`
    y SQLite nativo para Windows y Linux (no tocar los `.meta` de `Assets/Plugins/SQLite`).
12. **`AudioListener`**: la escena lo necesita; `AudioManager` lo añade solo si falta.

---

## 9. Trampas y bugs ya resueltos (no reintroducir)

- **Modales con `display: none` inline** → no se abren (clase vs inline). Cubierto por
  `UiSmokeTests.ModalOverlays_DoNotUseInlineDisplayNone`.
- **PNG sin `.meta`** → sprite múltiple y recorte (logo de Loading).
- **`Position` global** → CS0117 en `Position.Absolute`.
- **`Constants`** no existe: es `CountryCodes`.
- **Seed con campos desplazados**: 3 jugadores tenían `value/wage/clause` ocupados por
  `RIGHT, GK, 0`; corregido y con `EnsurePlayerContracts()` como red de seguridad.
- **Dorsales repetidos en el seed** (65 clubes): la auditoría los deja como **WARN**, no bloquean.
- **`prefab`/escenas**: no hay; no añadas `SceneManager.LoadScene` ni prefabs.
- **`Assets/Plugins/SQLite`**: conservar los `.meta` (Win64 x64 / Linux64 x64, `isPreloaded`).

---

## 10. Mapa de archivos clave

| Archivo | Rol |
|---|---|
| `Scripts/Core/ScreenManager.cs` | Registro de pantallas (campos serializados) y `GoTo` |
| `Scripts/Core/UIScreenController.cs` | Base de controllers + modal de configuración |
| `Scripts/Core/{AudioManager,CursorManager,LocalizationService,CustomScrollbar,CustomSlider,CalendarHelper}` | Servicios y utilidades comunes |
| `Scripts/Data/DatabaseManager.cs` | Fachada: conexión, tablas, slots, template, índices |
| `Scripts/Data/DatabaseManager.{Migrations,Seeding,Manager}.cs` | Migraciones, seed idempotente y manager/temporada |
| `Scripts/Data/DatabaseManager.Schedule.cs` | Calendario: generación, persistencia y consultas de partidos |
| `Scripts/Data/{FootballCalendar,FootballScheduleGenerator,PreseasonPlanner}.cs` | Reglas puras de fechas, doble vuelta y amistosos |
| `Scripts/Data/GameSaveManager.cs` | Slots (`save_{n}.db`, `saves.json`) |
| `Scripts/Data/{TeamSeeder,CompetitionSeeder,PlayerSeeder,PalmaresSeeder,GoldenBootSeeder}.cs` | Seed estático |
| `Scripts/Data/{PlayerRating,FilialRules,PlayerPositionNames,SeedBaseline,SeedAudit,ReferentialIntegrity}.cs` | Reglas puras |
| `Scripts/UI/{LoadingController,MainMenuController,LoadGameController,SelectTeamController,PreseasonController}.cs` | Pantallas implementadas |
| `UI/Resources/TacticalElevenTheme.tss` / `TacticalElevenPanelSettings.asset` | Tema y escalado (1920×1080, match 0.5) |
| `UI/Styles/{GlobalVariables,Typography,Utilities,Modals}.uss` | Estilos globales |
| `Assets/Plugins/SQLite/` | Binarios nativos Windows/Linux |
| `Docs/PLAN_REPLICA.md` | **Plan por fases y reglas de negocio** |

---

## 11. Comandos

```bash
# Tests EditMode (Unity cerrado; el lock impide batchmode)
~/Unity/Hub/Editor/6000.3.15f1/Editor/Unity -batchmode -projectPath . \
  -runTests -testPlatform EditMode \
  -testResults /tmp/opencode/editmode.xml -logFile /tmp/opencode/editmode.log

# Estado del repo
git status -sb && git log --oneline -5

# Validar textos (cuando se añada la herramienta Tools/localization_extract.py)
python3 Tools/localization_extract.py --check
```

---

## 12. Regla de oro

**Antes de asumir cómo funciona algo, lee `Docs/PLAN_REPLICA.md` o el código real.**
Este proyecto replica decisiones de Tactical Five que no son obvias (huella de seed,
template auto-reparable, filiales, banderas ISO3, modal por clases). Si algo cambia,
**actualiza el plan** en el mismo commit.
