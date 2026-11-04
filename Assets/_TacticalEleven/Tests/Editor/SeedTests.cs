using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// F2 — Seed de competiciones y clubes: conteos, distribución por competición,
/// huella determinista y assets (escudos/estadios) importados como sprite.
/// </summary>
public class SeedTests
{
    private string _root;
    private GameObject _go;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "te_seed_" + System.Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);

        GameSaveManager.BaseDirOverride = Path.Combine(_root, "saves");
        DatabaseManager.DataRootOverride = Path.Combine(_root, "data");

        _go = new GameObject("DatabaseManagerSeedTest");
        var db = _go.AddComponent<DatabaseManager>();
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
        typeof(DatabaseManager).GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)
            ?.SetValue(null, db);

    // ── SEED PURO ──────────────────────────────────────────────────────────

    [Test]
    public void Competitions_HaveExpectedShape()
    {
        var leagues = CompetitionSeeder.Build();

        Assert.That(leagues.Count, Is.EqualTo(9));
        Assert.That(leagues.Select(l => l.Id), Is.EquivalentTo(Enumerable.Range(1, 9)));

        Assert.That(leagues.Single(l => l.Id == 1).TeamCount, Is.EqualTo(20));
        Assert.That(leagues.Single(l => l.Id == 2).TeamCount, Is.EqualTo(22));
        Assert.That(leagues.Single(l => l.Id == 3).TeamCount, Is.EqualTo(20));
        Assert.That(leagues.Single(l => l.Id == 4).TeamCount, Is.EqualTo(20));

        foreach (int europeId in new[] { 5, 6, 7 })
            Assert.That(leagues.Single(l => l.Id == europeId).TeamCount, Is.EqualTo(36));

        Assert.That(leagues.Count(l => l.Type == CompetitionSeeder.TypeLeague), Is.EqualTo(4));
        Assert.That(leagues.Count(l => l.Type == CompetitionSeeder.TypeEurope), Is.EqualTo(3));
        Assert.That(leagues.Single(l => l.Id == 1).EuropeSlots, Is.EqualTo(7));
    }

    [Test]
    public void Teams_MatchSeedDistribution()
    {
        var teams = TeamSeeder.Build();

        Assert.That(teams.Count, Is.EqualTo(236));
        Assert.That(teams.Select(t => t.Id).Distinct().Count(), Is.EqualTo(236), "Ids únicos");
        Assert.That(teams.Count(t => t.LeagueId == 1), Is.EqualTo(20));
        Assert.That(teams.Count(t => t.LeagueId == 2), Is.EqualTo(22));
        Assert.That(teams.Count(t => t.LeagueId == 3), Is.EqualTo(20));
        Assert.That(teams.Count(t => t.LeagueId == 4), Is.EqualTo(20));
        Assert.That(teams.Count(t => t.LeagueId == 5), Is.EqualTo(32));
        Assert.That(teams.Count(t => t.LeagueId == 6), Is.EqualTo(34));
        Assert.That(teams.Count(t => t.LeagueId == 7), Is.EqualTo(35));
        Assert.That(teams.Count(t => t.LeagueId == 0), Is.EqualTo(53), "Históricos del palmarés");
    }

    [Test]
    public void Teams_WithLeague_HaveNameAndStadium()
    {
        foreach (var team in TeamSeeder.Build().Where(t => t.LeagueId != 0))
        {
            Assert.That(team.Name, Is.Not.Empty, $"Equipo {team.Id} sin nombre");
            Assert.That(team.StadiumName, Is.Not.Empty, $"{team.Name} sin estadio");
            Assert.That(team.StadiumCapacity, Is.GreaterThan(0), $"{team.Name} sin aforo");
            Assert.That(team.Logo, Is.Not.Empty, $"{team.Name} sin logo asignado");
            Assert.That(team.StadiumImage, Is.Not.Empty, $"{team.Name} sin imagen de estadio");
        }
    }

    [Test]
    public void Fingerprint_IsDeterministic()
    {
        string a = SeedBaseline.Fingerprint;
        string b = SeedBaseline.Fingerprint;

        Assert.That(a, Is.EqualTo(b));
        Assert.That(a.Length, Is.EqualTo(32), "MD5 en hex");
    }

    [Test]
    public void InitSaveSlot_SeedsCompetitionsTeamsAndFingerprint()
    {
        var db = DatabaseManager.Instance;
        Assert.That(db.InitSaveSlot(1), Is.True, db.LastInitError);

        Assert.That(db.Db.Table<LeagueDataDB>().Count(), Is.EqualTo(9));
        Assert.That(db.Db.Table<TeamDataDB>().Count(), Is.EqualTo(236));
        Assert.That(db.GetTeamsByLeague(1).Count, Is.EqualTo(20));
        Assert.That(db.ReadSeedFingerprint(), Is.EqualTo(SeedBaseline.Fingerprint));

        var issues = SeedAudit.Validate(db.Db);
        Assert.That(issues.Any(i => i.Severity == "ERROR"), Is.False,
            string.Join(" | ", issues.Where(i => i.Severity == "ERROR").Select(i => i.Message)));
    }

    // ── PRESUPUESTOS ───────────────────────────────────────────────────────

    [Test]
    public void Budgets_OfFirstAndSecondDivision_AreRealisticAndRounded()
    {
        var db = DatabaseManager.Instance;
        Assert.That(db.InitSaveSlot(1), Is.True, db.LastInitError);

        var teams = db.GetTeamsByLeague(1).Concat(db.GetTeamsByLeague(2)).ToList();
        Assert.That(teams.Count, Is.EqualTo(42));

        foreach (var team in teams)
        {
            Assert.That(team.TransferBudget % 500_000, Is.EqualTo(0),
                $"{team.Name}: el presupuesto debe ser múltiplo de 500.000 €");
            Assert.That(team.TransferBudget, Is.GreaterThan(0));
        }

        // Escala real 2026: el Madrid supera los 1.000 M€ y el último de 2ª no llega a 20 M€.
        Assert.That(db.GetTeamsByLeague(1).Max(t => t.TransferBudget),
            Is.GreaterThanOrEqualTo(1_000_000_000));
        Assert.That(db.GetTeamsByLeague(2).Min(t => t.TransferBudget),
            Is.LessThan(20_000_000));
    }

    [Test]
    public void LogoAndStadiumSprites_AreImportable()
    {
        Assert.That(Resources.Load<Sprite>("Teams/Logos/64x64/1"), Is.Not.Null,
            "Escudo del club 1 (Real Madrid) no encontrado como Sprite");
        Assert.That(Resources.Load<Sprite>("Teams/Logos/64x64/default"), Is.Not.Null,
            "Falta el escudo genérico");
        Assert.That(Resources.Load<Sprite>("Stadiums/large"), Is.Not.Null,
            "Imagen de estadio 'large' no encontrada");
    }
}
