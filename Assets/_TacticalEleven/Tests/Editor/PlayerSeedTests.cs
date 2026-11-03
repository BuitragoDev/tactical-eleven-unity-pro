using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// F3 — Jugadores (seed), palmarés y premios históricos.
/// Comprueba coherencia del overall, plantillas por club, referencias del palmarés
/// y que las tablas se siembren en un slot nuevo.
/// </summary>
public class PlayerSeedTests
{
    private string _root;
    private GameObject _go;
    private DatabaseManager _db;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "te_players_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);

        GameSaveManager.BaseDirOverride = Path.Combine(_root, "saves");
        DatabaseManager.DataRootOverride = Path.Combine(_root, "data");

        _go = new GameObject("DatabaseManagerPlayerTest");
        _db = _go.AddComponent<DatabaseManager>();
        typeof(DatabaseManager).GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)
            ?.SetValue(null, _db);

        Assert.That(_db.InitSaveSlot(1), Is.True, _db.LastInitError);
    }

    [TearDown]
    public void TearDown()
    {
        _db?.CloseActiveSlot();
        typeof(DatabaseManager).GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)
            ?.SetValue(null, null);
        if (_go != null) UnityEngine.Object.DestroyImmediate(_go);

        GameSaveManager.BaseDirOverride = null;
        DatabaseManager.DataRootOverride = null;
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); } catch { }
    }

    // ── PLANTILLAS ─────────────────────────────────────────────────────────

    [Test]
    public void EveryLeagueTeam_HasAtLeastElevenPlayers()
    {
        var players = _db.Db.Table<PlayerDataDB>().ToList();
        Assert.That(players.Count, Is.GreaterThan(1000), "El seed debe traer plantillas completas");

        foreach (var team in _db.GetAllTeams().Where(t => t.LeagueId >= 1 && t.LeagueId <= 4))
        {
            int count = players.Count(p => p.TeamId == team.Id);
            Assert.That(count, Is.GreaterThanOrEqualTo(11),
                $"{team.Name} solo tiene {count} jugadores");
        }
    }

    [Test]
    public void Players_HaveValidPositionAndNumber()
    {
        foreach (var p in _db.Db.Table<PlayerDataDB>().ToList())
        {
            Assert.That(p.MainPosition, Is.InRange(0, 14), $"{p.LastName} sin posición válida");
            Assert.That(p.SquadNumber, Is.GreaterThan(0), $"{p.LastName} sin dorsal");
            Assert.That(p.ReleaseClause, Is.GreaterThan(0), $"{p.LastName} sin cláusula");
            Assert.That(p.Wage, Is.GreaterThan(0), $"{p.LastName} sin salario");
        }
    }

    // ── OVERALL ────────────────────────────────────────────────────────────

    [Test]
    public void Overall_IsConsistentWithAttributes()
    {
        var players = _db.Db.Table<PlayerDataDB>().ToList();
        var diffs = players.Select(p => Math.Abs(p.Overall - PlayerRating.FromAttributes(p))).ToList();

        double mean = diffs.Average();
        int max = diffs.Max();
        int over5 = diffs.Count(d => d > 5);

        Debug.Log($"[F3] Overall vs atributos: media={mean:F2}, max={max}, >5={over5}/{diffs.Count}");

        Assert.That(mean, Is.LessThan(3.0), "La fórmula de overall debe parecerse al seed");
        Assert.That(max, Is.LessThanOrEqualTo(10), "Ningún jugador puede desviarse 10 puntos");
    }

    [Test]
    public void PlayerRating_CapsByPotential_AndIgnoresGkAttributes()
    {
        var striker = new PlayerDataDB
        {
            MainPosition = (int)PlayerPosition.ST,
            Potential = 0,
            Pace = 80, Acceleration = 80, Stamina = 80, Strength = 80,
            Agility = 80, Balance = 80, Jumping = 80, NaturalFitness = 80,
            Passing = 80, Crossing = 80, Dribbling = 80, Finishing = 80,
            FirstTouch = 80, Heading = 80, LongShots = 80, Marking = 80,
            Penalties = 80, FreeKicks = 80, Corners = 80, Tackling = 80,
            Technique = 80, Flair = 80,
            Aggression = 80, Anticipation = 80, Bravery = 80, Composure = 80,
            Concentration = 80, Decisions = 80, Determination = 80, Leadership = 80,
            OffTheBall = 80, Positioning = 80, Teamwork = 80, Vision = 80,
            WorkRate = 80, Consistency = 80,
            // Atributos de portero bajos: no deben penalizarle.
            Handling = 10, Kicking = 10, Reflexes = 10, GkPositioning = 10,
            OneOnOnes = 10, CommandOfArea = 10, RushingOut = 10, PenaltySaving = 10
        };

        Assert.That(PlayerRating.FromAttributes(striker), Is.EqualTo(80),
            "Un jugador de campo no debe verse penalizado por los atributos de portero");

        striker.Potential = 72;
        Assert.That(PlayerRating.FromAttributes(striker), Is.EqualTo(72), "El potencial es techo");

        var keeper = new PlayerDataDB
        {
            MainPosition = (int)PlayerPosition.GK,
            Potential = 0,
            Pace = 60, Acceleration = 60, Stamina = 60, Strength = 60,
            Agility = 60, Balance = 60, Jumping = 60, NaturalFitness = 60,
            Passing = 20, Crossing = 20, Dribbling = 20, Finishing = 20,
            FirstTouch = 20, Heading = 20, LongShots = 20, Marking = 20,
            Penalties = 20, FreeKicks = 20, Corners = 20, Tackling = 20,
            Technique = 20, Flair = 20,
            Aggression = 20, Anticipation = 20, Bravery = 20, Composure = 20,
            Concentration = 20, Decisions = 20, Determination = 20, Leadership = 20,
            OffTheBall = 20, Positioning = 20, Teamwork = 20, Vision = 20,
            WorkRate = 20, Consistency = 20,
            Handling = 80, Kicking = 80, Reflexes = 80, GkPositioning = 80,
            OneOnOnes = 80, CommandOfArea = 80, RushingOut = 80, PenaltySaving = 80
        };

        // 14 mentales (20) + 8 de portero (80) = 920/22 = 41,8 -> 42
        Assert.That(PlayerRating.FromAttributes(keeper), Is.EqualTo(42),
            "El portero se mide con mentales + atributos específicos (sin físicos)");
    }

    // ── PALMARÉS Y PREMIOS ─────────────────────────────────────────────────

    [Test]
    public void Palmares_And_Awards_AreSeededAndReferToExistingTeams()
    {
        var finals = _db.Db.Table<FinalsRecord>().ToList();
        var awards = _db.Db.Table<AwardsRecord>().ToList();
        var boots = _db.Db.Table<GoldenBootRecord>().ToList();

        Assert.That(finals.Count, Is.GreaterThan(400), "Finales históricas insuficientes");
        Assert.That(awards.Count, Is.GreaterThan(60), "Balones de Oro insuficientes");
        Assert.That(boots.Count, Is.GreaterThan(20), "Pichichis insuficientes");

        var teamIds = _db.GetAllTeams().Select(t => t.Id).ToHashSet();
        var missingFinals = finals.Where(f => !teamIds.Contains(f.ChampionTeamId) ||
                                              (f.FinalistTeamId != 0 && !teamIds.Contains(f.FinalistTeamId)))
                                  .Take(5).ToList();
        Assert.That(missingFinals, Is.Empty,
            "Palmarés con equipos inexistentes: " + string.Join(", ", missingFinals.Select(f => $"{f.Season}:{f.ChampionTeamId}")));

        var missingAwards = awards.Where(a => !teamIds.Contains(a.TeamId)).Take(5).ToList();
        Assert.That(missingAwards, Is.Empty,
            "Premios con equipos inexistentes: " + string.Join(", ", missingAwards.Select(a => $"{a.Season}:{a.TeamId}")));

        var missingBoots = boots.Where(b => !teamIds.Contains(b.TeamId)).Take(5).ToList();
        Assert.That(missingBoots, Is.Empty,
            "Pichichis con equipos inexistentes: " + string.Join(", ", missingBoots.Select(b => $"{b.Season}:{b.TeamId}")));
    }

    [Test]
    public void SeedFingerprint_ChangesWithPlayersHistory()
    {
        // La huella incluye palmarés y premios: no puede quedarse en el hash de F2.
        Assert.That(SeedBaseline.Fingerprint.Length, Is.EqualTo(32));
        Assert.That(_db.ReadSeedFingerprint(), Is.EqualTo(SeedBaseline.Fingerprint));
    }
}
