using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// F6 — Pretemporada y calendario: reglas puras del generador (doble vuelta,
/// fechas con parones y descanso) y persistencia en `games`.
/// </summary>
public class ScheduleTests
{
    private string _root;
    private GameObject _go;
    private DatabaseManager _db;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "te_schedule_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);

        GameSaveManager.BaseDirOverride = Path.Combine(_root, "saves");
        DatabaseManager.DataRootOverride = Path.Combine(_root, "data");

        _go = new GameObject("DatabaseManagerScheduleTest");
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

    private static List<int> Ids(int count) => Enumerable.Range(1, count).ToList();

    // ── GENERADOR PURO ─────────────────────────────────────────────────────

    [Test]
    public void GenerateLeague_20Teams_Produces38RoundsAnd380Games()
    {
        var fixtures = FootballScheduleGenerator.GenerateLeague(Ids(20), 2026, 1, new SeededRandomSource(7));

        Assert.That(fixtures.Count, Is.EqualTo(380), "20 equipos × 19 rivales × 2");
        Assert.That(fixtures.Max(f => f.MatchDay), Is.EqualTo(38));
        Assert.That(fixtures.All(f => f.GameType == "regular" && f.CompetitionId == 1), Is.True);
    }

    [Test]
    public void GenerateLeague_22Teams_Produces42RoundsAnd462Games()
    {
        var ids = Ids(22);
        var fixtures = FootballScheduleGenerator.GenerateLeague(ids, 2026, 2, new SeededRandomSource(7));

        Assert.That(fixtures.Count, Is.EqualTo(462), "22 equipos × 21 rivales × 2");
        Assert.That(fixtures.Max(f => f.MatchDay), Is.EqualTo(42));

        var validation = FootballScheduleGenerator.Validate(fixtures, ids);
        Assert.That(validation.IsValid, Is.True, validation.ToString());
    }

    [Test]
    public void Validate_EachPairPlaysTwice_OneHomeEach()
    {
        var ids = Ids(20);
        var fixtures = FootballScheduleGenerator.GenerateLeague(ids, 2026, 1, new SeededRandomSource(11));

        var validation = FootballScheduleGenerator.Validate(fixtures, ids);
        Assert.That(validation.IsValid, Is.True, validation.ToString());

        // Comprobación explícita e independiente del validador.
        foreach (var a in ids)
        {
            foreach (var b in ids.Where(x => x != a))
            {
                int home = fixtures.Count(f => f.HomeTeamId == a && f.AwayTeamId == b);
                int away = fixtures.Count(f => f.HomeTeamId == b && f.AwayTeamId == a);
                Assert.That(home, Is.EqualTo(1), $"{a} debe recibir a {b} una vez");
                Assert.That(away, Is.EqualTo(1), $"{a} debe visitar a {b} una vez");
            }
        }
    }

    [Test]
    public void SameSeed_ProducesIdenticalCalendar()
    {
        var first = FootballScheduleGenerator.GenerateLeague(Ids(20), 2026, 1, new SeededRandomSource(123));
        var second = FootballScheduleGenerator.GenerateLeague(Ids(20), 2026, 1, new SeededRandomSource(123));

        Assert.That(first.Count, Is.EqualTo(second.Count));
        for (int i = 0; i < first.Count; i++)
        {
            Assert.That(first[i].MatchDay, Is.EqualTo(second[i].MatchDay));
            Assert.That(first[i].HomeTeamId, Is.EqualTo(second[i].HomeTeamId));
            Assert.That(first[i].AwayTeamId, Is.EqualTo(second[i].AwayTeamId));
            Assert.That(first[i].Date, Is.EqualTo(second[i].Date));
        }
    }

    [Test]
    public void LeagueDates_StartInAugust_RespectBreaksAnd48Hours()
    {
        var ids = Ids(20);
        var fixtures = FootballScheduleGenerator.GenerateLeague(ids, 2026, 1, new SeededRandomSource(5));

        Assert.That(fixtures.Min(f => f.Date), Is.GreaterThanOrEqualTo(new DateTime(2026, 8, 15)));
        Assert.That(fixtures.Max(f => f.Date), Is.LessThanOrEqualTo(new DateTime(2027, 5, 31)));
        Assert.That(fixtures.Any(f => FootballCalendar.IsBreak(f.Date)), Is.False,
            "Ninguna jornada puede caer en parón FIFA o navideño");

        foreach (var team in ids)
        {
            var dates = fixtures
                .Where(f => f.HomeTeamId == team || f.AwayTeamId == team)
                .Select(f => f.Date)
                .OrderBy(d => d)
                .ToList();

            for (int i = 1; i < dates.Count; i++)
                Assert.That((dates[i] - dates[i - 1]).TotalHours, Is.GreaterThanOrEqualTo(48),
                    $"Equipo {team}: {dates[i - 1]:yyyy-MM-dd} → {dates[i]:yyyy-MM-dd}");
        }
    }

    [Test]
    public void Preseason_FourDistinctOpponents_AlternatingVenue()
    {
        var fixtures = FootballScheduleGenerator.GeneratePreseason(
            1, Ids(30).Where(id => id != 1).ToList(), 2026, new SeededRandomSource(3));

        Assert.That(fixtures.Count, Is.EqualTo(4));
        Assert.That(fixtures.All(f => f.GameType == "preseason"), Is.True);
        Assert.That(fixtures.Select(f => f.HomeTeamId == 1 ? f.AwayTeamId : f.HomeTeamId).Distinct().Count(),
            Is.EqualTo(4), "Los cuatro rivales deben ser distintos");
        Assert.That(fixtures.All(f => f.HomeTeamId == 1 || f.AwayTeamId == 1), Is.True);

        // Alternancia local/visitante.
        for (int i = 0; i < fixtures.Count; i++)
            Assert.That(fixtures[i].HomeTeamId == 1, Is.EqualTo(i % 2 == 0));

        Assert.That(fixtures.All(f => f.Date < FootballCalendar.FirstLeagueRound(2026)), Is.True,
            "Los amistosos deben jugarse antes de la jornada 1");
    }

    [Test]
    public void PreseasonPlanner_ExcludesSelfFilialesAndHistoricalClubs()
    {
        var teams = new List<TeamDataDB>
        {
            new TeamDataDB { Id = 1, LeagueId = 1, Overall = 85, Reputation = 90 },
            new TeamDataDB { Id = 2, LeagueId = 2, Overall = 70, Reputation = 70 },
            new TeamDataDB { Id = 3, LeagueId = 2, Overall = 60, Reputation = 60, IsFilial = 1 },
            new TeamDataDB { Id = 4, LeagueId = 0, Overall = 99, Reputation = 99 },
            new TeamDataDB { Id = 5, LeagueId = 3, Overall = 55, Reputation = 50 },
        };

        var candidates = PreseasonPlanner.CandidateOpponents(teams[0], teams, 10);

        Assert.That(candidates, Has.No.Member(1), "No puede jugar contra sí mismo");
        Assert.That(candidates, Has.No.Member(3), "Los filiales no son rivales de pretemporada");
        Assert.That(candidates, Has.No.Member(4), "Los clubes históricos no están en liga");
        Assert.That(candidates, Has.Member(2));
        Assert.That(candidates, Has.Member(5));
    }

    // ── PERSISTENCIA ───────────────────────────────────────────────────────

    [Test]
    public void PreseasonFilters_MatchThreeSpanishAndThreeEuropeanCompetitions()
    {
        // Tres españolas: LaLiga (1), Hypermotion (2) y 1ª RFEF (3+4).
        Assert.That(PreseasonFilter.Matches(1, PreseasonFilter.LaLiga), Is.True);
        Assert.That(PreseasonFilter.Matches(2, PreseasonFilter.LaLiga2), Is.True);
        Assert.That(PreseasonFilter.Matches(3, PreseasonFilter.Rfef), Is.True);
        Assert.That(PreseasonFilter.Matches(4, PreseasonFilter.Rfef), Is.True);
        Assert.That(PreseasonFilter.Matches(1, PreseasonFilter.Rfef), Is.False);

        // Tres europeas: Champions (5), Europa (6) y Conference (7).
        Assert.That(PreseasonFilter.Matches(5, PreseasonFilter.Champions), Is.True);
        Assert.That(PreseasonFilter.Matches(6, PreseasonFilter.Europa), Is.True);
        Assert.That(PreseasonFilter.Matches(7, PreseasonFilter.Conference), Is.True);
        Assert.That(PreseasonFilter.Matches(5, PreseasonFilter.Europa), Is.False);

        // Cada filtro tiene clubes reales en el seed.
        Assert.That(_db.GetTeamsByLeague(1).Count, Is.EqualTo(20));
        Assert.That(_db.GetTeamsByLeague(2).Count, Is.EqualTo(22));
        Assert.That(_db.GetTeamsByLeague(3).Count + _db.GetTeamsByLeague(4).Count, Is.EqualTo(40));
        Assert.That(_db.GetTeamsByLeague(5).Count, Is.EqualTo(32));
        Assert.That(_db.GetTeamsByLeague(6).Count, Is.EqualTo(34));
        Assert.That(_db.GetTeamsByLeague(7).Count, Is.EqualTo(35));
    }

    [Test]
    public void NewSeason_StartsWithNoPreseasonGames()
    {
        var manager = new ManagerDataDB { Name = "Test", TeamId = 1, LeagueId = 1, GameMode = "manager" };
        _db.SaveManager(manager);
        var season = _db.CreateSeason(manager.Id, 1, 2026);

        Assert.That(_db.GetGamesByType(season.Id, DatabaseManager.GameTypePreseason), Is.Empty,
            "La pretemporada debe empezar vacía: el usuario elige los amistosos");
    }

    [Test]
    public void GenerateLeagueSchedule_PersistsGames_AndIsIdempotent()
    {
        var manager = new ManagerDataDB { Name = "Test", TeamId = 1, LeagueId = 1, GameMode = "manager" };
        _db.SaveManager(manager);
        var season = _db.CreateSeason(manager.Id, 1, 2026);

        int count = _db.GenerateLeagueSchedule(season, new SeededRandomSource(9));
        Assert.That(count, Is.EqualTo(380));
        Assert.That(season.TotalMatchDays, Is.EqualTo(38));

        var games = _db.GetGamesBySeason(season.Id);
        Assert.That(games.Count, Is.EqualTo(380));
        Assert.That(games.All(g => g.GameType == DatabaseManager.GameTypeRegular), Is.True);
        Assert.That(games.All(g => g.CompetitionId == 1), Is.True);
        Assert.That(games.All(g => !string.IsNullOrEmpty(g.MatchDate)), Is.True);

        // Regenerar no duplica.
        Assert.That(_db.GenerateLeagueSchedule(season, new SeededRandomSource(9)), Is.EqualTo(380));
        Assert.That(_db.GetGamesBySeason(season.Id).Count, Is.EqualTo(380));
    }

    [Test]
    public void GeneratePreseasonSchedule_PersistsFourFriendlies()
    {
        var manager = new ManagerDataDB { Name = "Test", TeamId = 1, LeagueId = 1, GameMode = "manager" };
        _db.SaveManager(manager);
        var season = _db.CreateSeason(manager.Id, 1, 2026);

        int count = _db.GeneratePreseasonSchedule(season, manager.TeamId, new SeededRandomSource(2));

        Assert.That(count, Is.EqualTo(4));
        var friendlies = _db.GetGamesByType(season.Id, DatabaseManager.GameTypePreseason);
        Assert.That(friendlies.Count, Is.EqualTo(4));
        Assert.That(friendlies.All(g => g.HomeTeamId == manager.TeamId || g.AwayTeamId == manager.TeamId), Is.True);
        Assert.That(friendlies.All(g => g.IsPlayed == 0), Is.True);
        Assert.That(friendlies.Select(g => g.HomeTeamId == manager.TeamId ? g.AwayTeamId : g.HomeTeamId).Distinct().Count(),
            Is.EqualTo(4));
    }

    [Test]
    public void GetNextGame_ReturnsFirstUpcomingFixture()
    {
        var manager = new ManagerDataDB { Name = "Test", TeamId = 1, LeagueId = 1, GameMode = "manager" };
        _db.SaveManager(manager);
        var season = _db.CreateSeason(manager.Id, 1, 2026);
        _db.GenerateLeagueSchedule(season, new SeededRandomSource(4));

        var next = _db.GetNextGame(season.Id, manager.TeamId, "2026-07-01");

        Assert.That(next, Is.Not.Null);
        Assert.That(next.GameType, Is.EqualTo(DatabaseManager.GameTypeRegular));
        Assert.That(next.HomeTeamId == manager.TeamId || next.AwayTeamId == manager.TeamId, Is.True);
        Assert.That(next.MatchDay, Is.EqualTo(1));
    }
}
