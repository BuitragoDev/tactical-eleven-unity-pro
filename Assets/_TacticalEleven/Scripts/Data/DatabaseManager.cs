using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SQLite;
using UnityEngine;

/// <summary>
/// Único punto de acceso a SQLite. Un archivo por slot (`save_{n}.db`) y un
/// `template.db` con los datos estáticos que se clonan al crear una partida.
///
/// Los lotes pesados pueden ejecutarse en segundo plano con
/// <see cref="RunInBackground"/> / <see cref="RunInBackgroundAsync"/>: cada hilo
/// abre su propia conexión y la expone por <see cref="Db"/> (ambient AsyncLocal).
/// Nunca se toca `_dbField` fuera del hilo principal.
/// </summary>
public partial class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    // Conexión "ambient": en el hilo principal es la conexión abierta del slot;
    // dentro de un lote de fondo es la conexión de ese hilo.
    private static readonly AsyncLocal<SQLiteConnection> _ambientDb = new();

    private SQLiteConnection _dbField;
    public SQLiteConnection Db => _ambientDb.Value ?? _dbField;

    private static string _activeDbPath;
    private int _activeSaveSlot;
    public int ActiveSaveSlot => _activeSaveSlot;
    public bool HasOpenDatabase => _dbField != null;

    public string LastInitError { get; private set; }
    public string LastMigrationError { get; private set; }

    /// <summary>Solo tests: raíz de datos alternativa (template y partidas).</summary>
    public static string DataRootOverride { get; set; }

    public static string DataRoot => string.IsNullOrEmpty(DataRootOverride)
        ? Path.Combine(Application.persistentDataPath, "TacticalEleven")
        : DataRootOverride;

    /// <summary>Ruta del template que se clona en las partidas nuevas.</summary>
    public string TemplateDbPath => Path.Combine(DataRoot, "template.db");

    private string DbPath => GameSaveManager.GetSaveDbPath(_activeSaveSlot);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── CICLO DE VIDA DEL SLOT ─────────────────────────────────────────────

    /// <summary>
    /// Abre (o crea) el slot indicado. Devuelve false y deja el motivo en
    /// <see cref="LastInitError"/> si algo falla; nunca deja la base a medias.
    /// </summary>
    public bool InitSaveSlot(int slotNumber)
    {
        LastInitError = null;
        LastMigrationError = null;

        CloseActiveSlot();

        try
        {
            _activeSaveSlot = slotNumber;
            _activeDbPath = DbPath;

            string dir = Path.GetDirectoryName(_activeDbPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            _dbField = new SQLiteConnection(_activeDbPath);
            _dbField.ExecuteScalar<string>("PRAGMA journal_mode=WAL");
            _dbField.ExecuteScalar<int>("PRAGMA busy_timeout=5000");

            CreateTables();
            RunMigrations();

            bool hasData = _dbField.Table<TeamDataDB>().Count() > 0;
            if (!hasData)
            {
                if (!EnsureTemplateDb())
                    throw new InvalidOperationException("No se pudo preparar template.db.");

                CloneFromTemplate();
                SeedStaticDataIfNeeded();

                // El slot nuevo se audita completo (F1): un ERROR revierte.
                var issues = SeedAudit.Validate(_dbField);
                var errors = issues.Where(i => i.Severity == "ERROR").ToList();
                if (errors.Count > 0)
                    throw new InvalidOperationException(
                        "Seed inválido en el slot: " + string.Join(" | ", errors.Select(e => e.Message)));
            }

            return true;
        }
        catch (Exception e)
        {
            LastInitError = e.Message;
            Debug.LogError($"[DB] InitSaveSlot({slotNumber}) falló: {e}");
            CloseActiveSlot();
            return false;
        }
    }

    public void CloseActiveSlot()
    {
        if (_dbField != null)
        {
            try { _dbField.Close(); } catch { /* ya cerrada */ }
            _dbField = null;
        }
        _ambientDb.Value = null;
        _activeSaveSlot = 0;
        _activeDbPath = null;
    }

    bool EnsureDb()
    {
        if (Db == null)
        {
            Debug.LogError("[DB] No hay base de datos activa. Llama InitSaveSlot() primero.");
            return false;
        }
        return true;
    }

    // ── ESQUEMA ────────────────────────────────────────────────────────────

    /// <summary>Tablas e índices. Se llama también sobre el template.</summary>
    public void CreateTables()
    {
        var db = Db;

        db.CreateTable<LeagueDataDB>();
        db.CreateTable<TeamDataDB>();
        db.CreateTable<PlayerDataDB>();
        db.CreateTable<ManagerDataDB>();
        db.CreateTable<SeasonDataDB>();
        db.CreateTable<GameDataDB>();
        db.CreateTable<MatchEventDataDB>();
        db.CreateTable<PlayerMatchStatsDB>();
        db.CreateTable<PlayerSeasonStatsDB>();
        db.CreateTable<InjuryDataDB>();
        db.CreateTable<StaffDataDB>();
        db.CreateTable<EmployeeDataDB>();
        db.CreateTable<FinanceRecordDB>();
        db.CreateTable<MessageDataDB>();
        db.CreateTable<OfferDataDB>();
        db.CreateTable<StandingsDataDB>();
        db.CreateTable<TrainingDataDB>();
        db.CreateTable<TeamSettingsDataDB>();
        db.CreateTable<SponsorDataDB>();
        db.CreateTable<TvChannelDataDB>();
        db.CreateTable<LoanDataDB>();
        db.CreateTable<FinalsRecord>();
        db.CreateTable<AwardsRecord>();
        db.CreateTable<GoldenBootRecord>();
        db.CreateTable<LineupDataDB>();

        // Metadatos y migraciones
        db.CreateTable<DbMetaData>();
        db.CreateTable<SchemaMigrationData>();
        db.CreateTable<SchemaMigrationAttemptData>();

        ApplyIndexes(db);
    }

    /// <summary>Índices de las consultas calientes (lista única).</summary>
    public static readonly string[] IndexDefinitions =
    {
        "CREATE INDEX IF NOT EXISTS IX_Players_TeamId ON players(TeamId)",
        "CREATE INDEX IF NOT EXISTS IX_Players_Status ON players(Status)",
        "CREATE INDEX IF NOT EXISTS IX_Games_SeasonId ON games(SeasonId)",
        "CREATE INDEX IF NOT EXISTS IX_Games_MatchDay ON games(SeasonId, MatchDay)",
        "CREATE INDEX IF NOT EXISTS IX_Games_Teams ON games(HomeTeamId, AwayTeamId)",
        "CREATE INDEX IF NOT EXISTS IX_Standings_SeasonId ON standings(SeasonId)",
        "CREATE INDEX IF NOT EXISTS IX_PlayerStats_PlayerId ON player_season_stats(PlayerId, SeasonYear)",
        "CREATE INDEX IF NOT EXISTS IX_MatchStats_GameId ON player_match_stats(GameId)",
        "CREATE INDEX IF NOT EXISTS IX_Lineup_TeamId ON lineups(TeamId)",
        "CREATE INDEX IF NOT EXISTS IX_Offers_Status ON offers(Status)",
        "CREATE INDEX IF NOT EXISTS IX_Finals_Competition ON finals_records(CompetitionId, Season)",
    };

    static void ApplyIndexes(SQLiteConnection db)
    {
        foreach (var sql in IndexDefinitions)
        {
            try { db.Execute(sql); }
            catch (Exception e) { Debug.LogWarning($"[DB] Índice no aplicado: {sql} ({e.Message})"); }
        }
    }

    // ── ACCESO BÁSICO ──────────────────────────────────────────────────────

    public ManagerDataDB GetActiveManager()
    {
        if (!EnsureDb()) return null;
        return Db.Table<ManagerDataDB>().FirstOrDefault();
    }

    public TeamDataDB GetTeamById(int id)
    {
        if (!EnsureDb()) return null;
        return Db.Table<TeamDataDB>().FirstOrDefault(t => t.Id == id);
    }

    public LeagueDataDB GetLeagueById(int id)
    {
        if (!EnsureDb()) return null;
        return Db.Table<LeagueDataDB>().FirstOrDefault(l => l.Id == id);
    }

    public List<LeagueDataDB> GetAllLeagues()
    {
        if (!EnsureDb()) return new List<LeagueDataDB>();
        return Db.Table<LeagueDataDB>().ToList();
    }

    public List<TeamDataDB> GetAllTeams()
    {
        if (!EnsureDb()) return new List<TeamDataDB>();
        return Db.Table<TeamDataDB>().ToList();
    }

    public List<TeamDataDB> GetTeamsByLeague(int leagueId)
    {
        if (!EnsureDb()) return new List<TeamDataDB>();
        return Db.Table<TeamDataDB>().Where(t => t.LeagueId == leagueId).ToList();
    }

    public List<PlayerDataDB> GetPlayersByTeam(int teamId)
    {
        if (!EnsureDb()) return new List<PlayerDataDB>();
        return Db.Table<PlayerDataDB>().Where(p => p.TeamId == teamId).ToList();
    }

    public PlayerDataDB GetPlayerById(int id)
    {
        if (!EnsureDb()) return null;
        return Db.Table<PlayerDataDB>().FirstOrDefault(p => p.Id == id);
    }

    public SeasonDataDB GetActiveSeason(int managerId)
    {
        if (!EnsureDb()) return null;
        return Db.Table<SeasonDataDB>()
                 .Where(s => s.ManagerId == managerId && s.IsActive == 1)
                 .FirstOrDefault();
    }

    public void UpdateTeam(TeamDataDB team)
    {
        if (!EnsureDb() || team == null) return;
        Db.Update(team);
    }

    public void UpdatePlayer(PlayerDataDB player)
    {
        if (!EnsureDb() || player == null) return;
        Db.Update(player);
    }

    // ── TRABAJO EN SEGUNDO PLANO ───────────────────────────────────────────

    /// <summary>
    /// Ejecuta un lote en un hilo de fondo con su propia conexión WAL. Los helpers
    /// que usan <see cref="Db"/> dentro del delegado escriben en esa conexión.
    /// </summary>
    public static void RunInBackground(Action<SQLiteConnection> work)
    {
        if (work == null || string.IsNullOrEmpty(_activeDbPath)) return;

        using var conn = new SQLiteConnection(_activeDbPath);
        conn.ExecuteScalar<string>("PRAGMA journal_mode=WAL");
        var previous = _ambientDb.Value;
        _ambientDb.Value = conn;
        try
        {
            work(conn);
        }
        finally
        {
            _ambientDb.Value = previous;
        }
    }

    public static Task RunInBackgroundAsync(Action<SQLiteConnection> work)
    {
        if (work == null || string.IsNullOrEmpty(_activeDbPath))
            return Task.CompletedTask;

        string path = _activeDbPath;
        return Task.Run(() =>
        {
            using var conn = new SQLiteConnection(path);
            conn.ExecuteScalar<string>("PRAGMA journal_mode=WAL");
            var previous = _ambientDb.Value;
            _ambientDb.Value = conn;
            try
            {
                work(conn);
            }
            finally
            {
                _ambientDb.Value = previous;
            }
        });
    }

    // ── TEMPLATE ───────────────────────────────────────────────────────────

    /// <summary>
    /// Construye `template.db` si no existe. Se genera en un archivo temporal y se
    /// publica al final para no dejar un template a medias.
    /// </summary>
    public bool EnsureTemplateDb()
    {
        string templatePath = TemplateDbPath;
        if (File.Exists(templatePath))
        {
            // La huella es frescura, no validez: si no coincide se reconstruye.
            try
            {
                using var existing = new SQLiteConnection(templatePath);
                var meta = existing.Table<DbMetaData>().FirstOrDefault();
                if (meta != null && meta.SeedFingerprint == SeedBaseline.Fingerprint)
                    return true;

                Debug.LogWarning("[DB] Template obsoleto (huella distinta): se reconstruye.");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[DB] Template ilegible ({e.Message}): se reconstruye.");
            }
        }

        try
        {
            string dir = Path.GetDirectoryName(templatePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string tempPath = templatePath + ".tmp";
            if (File.Exists(tempPath)) File.Delete(tempPath);

            using (var conn = new SQLiteConnection(tempPath))
            {
                conn.ExecuteScalar<string>("PRAGMA journal_mode=WAL");
                var previous = _ambientDb.Value;
                _ambientDb.Value = conn;
                try
                {
                    CreateTables();
                    RunMigrations();
                    SeedStaticDataIfNeeded();
                    WriteSeedFingerprint(SeedBaseline.Fingerprint);
                }
                finally
                {
                    _ambientDb.Value = previous;
                }
            }

            if (File.Exists(templatePath)) File.Delete(templatePath + ".bak");
            if (File.Exists(templatePath)) File.Move(templatePath, templatePath + ".bak");

            File.Move(tempPath, templatePath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[DB] EnsureTemplateDb falló: {e}");
            return false;
        }
    }

    /// <summary>Copia las tablas estáticas del template al slot abierto.</summary>
    void CloneFromTemplate()
    {
        string templatePath = TemplateDbPath;
        if (!File.Exists(templatePath)) return;

        using var template = new SQLiteConnection(templatePath);
        template.ExecuteScalar<string>("PRAGMA journal_mode=WAL");

        var db = Db;
        bool own = !db.IsInTransaction;
        if (own) db.BeginTransaction();
        try
        {
            var leagues = template.Table<LeagueDataDB>().ToList();
            if (leagues.Count > 0) db.InsertAll(leagues);

            var teams = template.Table<TeamDataDB>().ToList();
            if (teams.Count > 0) db.InsertAll(teams);

            var players = template.Table<PlayerDataDB>().ToList();
            if (players.Count > 0) db.InsertAll(players);

            var meta = template.Table<DbMetaData>().FirstOrDefault();
            if (meta != null)
            {
                meta.Id = 1;
                db.InsertOrReplace(meta);
            }

            if (own) db.Commit();
        }
        catch
        {
            if (own) { try { db.Rollback(); } catch { } }
            throw;
        }
    }

    public void WriteSeedFingerprint(string fingerprint)
    {
        if (!EnsureDb()) return;
        var db = Db;
        var meta = new DbMetaData
        {
            Id = 1,
            SeedFingerprint = fingerprint,
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            GameVersion = Application.version
        };
        db.InsertOrReplace(meta);
    }

    public string ReadSeedFingerprint()
    {
        if (!EnsureDb()) return null;
        return Db.Table<DbMetaData>().FirstOrDefault()?.SeedFingerprint;
    }
}
