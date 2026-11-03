using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// F1 — Persistencia: alta de slot, template, migraciones y auditoría.
/// Todo el trabajo se hace en una carpeta temporal (overrides de test), así que no
/// toca las partidas reales del usuario.
/// </summary>
public class PersistenceTests
{
    private string _root;
    private GameObject _go;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "te_persist_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);

        GameSaveManager.BaseDirOverride = Path.Combine(_root, "saves");
        DatabaseManager.DataRootOverride = Path.Combine(_root, "data");

        _go = new GameObject("DatabaseManagerTest");
        var db = _go.AddComponent<DatabaseManager>();
        // En EditMode no se ejecuta Awake: el singleton se registra a mano.
        SetInstance(db);
    }

    [TearDown]
    public void TearDown()
    {
        DatabaseManager.Instance?.CloseActiveSlot();
        SetInstance(null);
        if (_go != null) UnityEngine.Object.DestroyImmediate(_go);

        GameSaveManager.BaseDirOverride = null;
        DatabaseManager.DataRootOverride = null;

        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); } catch { }
    }

    static void SetInstance(DatabaseManager db) =>
        typeof(DatabaseManager)
            .GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)
            ?.SetValue(null, db);

    // ── SLOT ───────────────────────────────────────────────────────────────

    [Test]
    public void InitSaveSlot_CreatesDatabaseAndTemplate_AndReopens()
    {
        var db = DatabaseManager.Instance;

        Assert.That(File.Exists(db.TemplateDbPath), Is.False, "Escenario: instalación nueva sin template");

        Assert.That(db.InitSaveSlot(1), Is.True, $"Alta fallida: {db.LastInitError}");
        Assert.That(db.Db, Is.Not.Null);
        Assert.That(db.LastMigrationError, Is.Null);
        Assert.That(File.Exists(GameSaveManager.GetSaveDbPath(1)), Is.True, "Debe existir save_1.db");
        Assert.That(File.Exists(db.TemplateDbPath), Is.True, "El template debe generarse al vuelo");

        db.CloseActiveSlot();
        Assert.That(db.InitSaveSlot(1), Is.True, "El slot debe poder reabrirse");
        Assert.That(db.Db, Is.Not.Null);

        // F1: aún no hay contenido estático, pero el esquema debe estar completo.
        Assert.That(db.Db.Table<TeamDataDB>().Count(), Is.EqualTo(0));
        Assert.That(db.Db.Table<LeagueDataDB>().Count(), Is.EqualTo(0));
    }

    [Test]
    public void InitSaveSlot_SetsSchemaVersionAndTables()
    {
        var db = DatabaseManager.Instance;
        Assert.That(db.InitSaveSlot(2), Is.True, db.LastInitError);

        int userVersion = db.Db.ExecuteScalar<int>("PRAGMA user_version");
        Assert.That(userVersion, Is.EqualTo(DatabaseManager.SchemaVersion));

        int tables = db.Db.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name IN " +
            "('players','teams','leagues','games','seasons','managers','lineups','offers','standings'," +
            "'finals_records','awards_records','golden_boot_records','db_meta','schema_migrations')");
        Assert.That(tables, Is.EqualTo(14));
    }

    [Test]
    public void InitSaveSlot_ClonesTemplateIntoNewSlot()
    {
        var db = DatabaseManager.Instance;
        Assert.That(db.InitSaveSlot(1), Is.True, db.LastInitError);
        db.CloseActiveSlot();

        // Segundo slot: el template ya existe y la huella coincide.
        Assert.That(db.InitSaveSlot(2), Is.True, db.LastInitError);
        Assert.That(db.ReadSeedFingerprint(), Is.EqualTo(SeedBaseline.Fingerprint));
    }

    [Test]
    public void Template_WithStaleFingerprint_IsRebuilt()
    {
        var db = DatabaseManager.Instance;
        Assert.That(db.InitSaveSlot(1), Is.True, db.LastInitError);
        db.CloseActiveSlot();

        // Estropeamos la huella del template.
        using (var conn = new SQLite.SQLiteConnection(db.TemplateDbPath))
        {
            conn.InsertOrReplace(new DbMetaData
            {
                Id = 1,
                SeedFingerprint = "huella-vieja",
                CreatedAt = "2020-01-01",
                GameVersion = "0"
            });
        }

        Assert.That(db.EnsureTemplateDb(), Is.True);
        using var check = new SQLite.SQLiteConnection(db.TemplateDbPath);
        Assert.That(check.Table<DbMetaData>().First().SeedFingerprint,
            Is.EqualTo(SeedBaseline.Fingerprint));
    }

    // ── AUDITORÍA / INTEGRIDAD ─────────────────────────────────────────────

    [Test]
    public void SeedAudit_WithoutData_ReturnsWarningNotError()
    {
        var db = DatabaseManager.Instance;
        Assert.That(db.InitSaveSlot(1), Is.True, db.LastInitError);

        var issues = SeedAudit.Validate(db.Db);
        Assert.That(issues.Any(i => i.Severity == "ERROR"), Is.False);
    }

    [Test]
    public void ReferentialIntegrity_DetectsOrphanPlayerTeam()
    {
        var db = DatabaseManager.Instance;
        Assert.That(db.InitSaveSlot(1), Is.True, db.LastInitError);

        db.Db.Insert(new PlayerDataDB
        {
            TeamId = 9999,
            FirstName = "Huerfano",
            LastName = "Test",
            MainPosition = 0,
            Overall = 50
        });

        var problems = ReferentialIntegrity.Validate(db.Db);
        Assert.That(problems.Any(p => p.StartsWith("players.team")), Is.True,
            "Debe detectar el jugador con equipo inexistente");
    }

    // ── CALENDARIO (puro) ──────────────────────────────────────────────────

    [Test]
    public void CalendarHelper_RoundTrip()
    {
        var start = CalendarHelper.SeasonStart(2026);
        Assert.That(start, Is.EqualTo(new DateTime(2026, 8, 15)));

        Assert.That(CalendarHelper.DateToGameDay(2026, new DateTime(2026, 8, 15)), Is.EqualTo(1));
        Assert.That(CalendarHelper.DateToGameDay(2026, new DateTime(2026, 8, 14)), Is.EqualTo(-1));

        int day = CalendarHelper.DateToGameDay(2026, new DateTime(2027, 5, 30));
        Assert.That(CalendarHelper.GameDayToDate(2026, day), Is.EqualTo(new DateTime(2027, 5, 30)));
    }
}
