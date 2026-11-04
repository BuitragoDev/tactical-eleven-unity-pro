using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// F7 — Dashboard y avance de día: reglas puras (clasificación, objetivo, fechas),
/// bucle diario persistido y carga del UXML.
/// </summary>
public class DashboardTests
{
    private string _root;
    private GameObject _go;
    private DatabaseManager _db;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "te_dashboard_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);

        GameSaveManager.BaseDirOverride = Path.Combine(_root, "saves");
        DatabaseManager.DataRootOverride = Path.Combine(_root, "data");

        _go = new GameObject("DatabaseManagerDashboardTest");
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

    // ── CLASIFICACIÓN ──────────────────────────────────────────────────────

    [Test]
    public void LeagueTable_CountsPointsAndOrdersByGoalDifference()
    {
        var games = new List<GameDataDB>
        {
            new GameDataDB { HomeTeamId = 1, AwayTeamId = 2, HomeScore = 2, AwayScore = 0, IsPlayed = 1 },
            new GameDataDB { HomeTeamId = 3, AwayTeamId = 4, HomeScore = 1, AwayScore = 1, IsPlayed = 1 },
            new GameDataDB { HomeTeamId = 1, AwayTeamId = 3, HomeScore = 0, AwayScore = 1, IsPlayed = 1 },
            new GameDataDB { HomeTeamId = 2, AwayTeamId = 4, HomeScore = 3, AwayScore = 0, IsPlayed = 0 }
        };

        var table = LeagueTable.Build(new[] { 1, 2, 3, 4 }, games);

        Assert.That(table.Select(r => r.TeamId), Is.EqualTo(new[] { 3, 1, 2, 4 }),
            "3 (4 pts) > 1 (3) > 2 (0, -2) > 4 (0, -3)");
        Assert.That(table[0].Points, Is.EqualTo(4));
        Assert.That(table[0].Played, Is.EqualTo(2));
        Assert.That(LeagueTable.PositionOf(table, 1), Is.EqualTo(2));
        Assert.That(LeagueTable.PositionOf(table, 99), Is.EqualTo(0));

        // Los partidos no jugados no puntúan.
        Assert.That(table.First(r => r.TeamId == 2).Played, Is.EqualTo(1));
    }

    // ── OBJETIVO ───────────────────────────────────────────────────────────

    [Test]
    public void SeasonObjective_HasSixOptionsAndMapsEachLeague()
    {
        var keys = new HashSet<string>
        {
            SeasonObjective.KeyFor(1, 1, 20),
            SeasonObjective.KeyFor(1, 6, 20),
            SeasonObjective.KeyFor(1, 12, 20),
            SeasonObjective.KeyFor(1, 20, 20),
            SeasonObjective.KeyFor(2, 1, 22),
            SeasonObjective.KeyFor(2, 4, 22)
        };
        Assert.That(keys.Count, Is.EqualTo(6), "Campeonato, Europa, Media, Permanencia, Ascenso y Playoff");

        Assert.That(SeasonObjective.KeyFor(1, 1, 20), Is.EqualTo(SeasonObjective.KeyCampeonato));
        Assert.That(SeasonObjective.KeyFor(1, 5, 20), Is.EqualTo(SeasonObjective.KeyEuropa));
        Assert.That(SeasonObjective.KeyFor(1, 15, 20), Is.EqualTo(SeasonObjective.KeyMedia));
        Assert.That(SeasonObjective.KeyFor(1, 16, 20), Is.EqualTo(SeasonObjective.KeyPermanencia));

        Assert.That(SeasonObjective.KeyFor(2, 2, 22), Is.EqualTo(SeasonObjective.KeyAscenso));
        Assert.That(SeasonObjective.KeyFor(2, 3, 22), Is.EqualTo(SeasonObjective.KeyPlayoff));
        Assert.That(SeasonObjective.KeyFor(2, 16, 22), Is.EqualTo(SeasonObjective.KeyMedia));
        Assert.That(SeasonObjective.KeyFor(2, 17, 22), Is.EqualTo(SeasonObjective.KeyPermanencia));

        Assert.That(SeasonObjective.KeyFor(3, 2, 20), Is.EqualTo(SeasonObjective.KeyAscenso));
        Assert.That(SeasonObjective.KeyFor(4, 4, 20), Is.EqualTo(SeasonObjective.KeyPlayoff));
    }

    // ── FECHAS Y FASE ──────────────────────────────────────────────────────

    [Test]
    public void GameDayRules_NextDatePhaseAndFormat()
    {
        Assert.That(GameDayRules.TryNextDate("2026-07-31", out var next), Is.True);
        Assert.That(next, Is.EqualTo("2026-08-01"));
        Assert.That(GameDayRules.TryNextDate("", out _), Is.False);

        Assert.That(GameDayRules.PhaseFor("2026-08-14", "2026-08-15"), Is.EqualTo("preseason"));
        Assert.That(GameDayRules.PhaseFor("2026-08-15", "2026-08-15"), Is.EqualTo("regular"));
        Assert.That(GameDayRules.PhaseFor("2026-10-01", "2026-08-15"), Is.EqualTo("regular"));
        Assert.That(GameDayRules.PhaseFor("2026-08-01", null), Is.EqualTo("preseason"));

        Assert.That(GameDayRules.FormatShortDate("2026-07-18"), Is.EqualTo("18 JUL 2026"));
        Assert.That(GameDayRules.FormatLongDate("2026-07-18"), Is.EqualTo("SÁB 18 JUL 2026"));
        Assert.That(GameDayRules.FormatShortDate(""), Is.EqualTo(""));
    }

    // ── AVANCE DE DÍA ──────────────────────────────────────────────────────

    [Test]
    public void AdvanceDay_StopsOnMatchDay_AndUpdatesDateAndPhase()
    {
        var manager = new ManagerDataDB { Name = "Test", TeamId = 1, LeagueId = 1, GameMode = "manager" };
        _db.SaveManager(manager);
        var season = _db.CreateSeason(manager.Id, 1, 2026);
        Assert.That(_db.GenerateLeagueSchedule(season), Is.GreaterThan(0));

        var first = _db.GetGamesForTeam(season.Id, 1)
                        .Where(g => g.GameType == DatabaseManager.GameTypeRegular)
                        .OrderBy(g => g.MatchDate)
                        .ThenBy(g => g.MatchDay)
                        .First();
        var firstDate = DateTime.Parse(first.MatchDate);
        var firstLeagueDate = _db.GetFirstLeagueGame(season.Id).MatchDate;

        // Dos días antes del primer partido: el primer avance cae en un día libre.
        season.CurrentDate = CalendarHelper.ToDb(firstDate.AddDays(-2));
        _db.UpdateSeason(season);

        var free = _db.AdvanceDay(season, 1, out var noMatch);
        Assert.That(free, Is.EqualTo(DayAdvanceResult.Advanced));
        Assert.That(noMatch, Is.Null);
        Assert.That(season.CurrentDate, Is.EqualTo(CalendarHelper.ToDb(firstDate.AddDays(-1))));
        Assert.That(season.Phase, Is.EqualTo(GameDayRules.PhaseFor(season.CurrentDate, firstLeagueDate)));

        // El siguiente avance llega al partido y se detiene.
        var matchDay = _db.AdvanceDay(season, 1, out var match);
        Assert.That(matchDay, Is.EqualTo(DayAdvanceResult.MatchDay));
        Assert.That(match, Is.Not.Null);
        Assert.That(match.MatchDate, Is.EqualTo(first.MatchDate));
        Assert.That(season.CurrentDate, Is.EqualTo(first.MatchDate));
        Assert.That(season.Phase, Is.EqualTo(GameDayRules.PhaseFor(season.CurrentDate, firstLeagueDate)));

        // No se puede avanzar mientras el partido siga pendiente.
        Assert.That(_db.AdvanceDay(season, 1, out _), Is.EqualTo(DayAdvanceResult.MatchDay));
        Assert.That(season.CurrentDate, Is.EqualTo(first.MatchDate));
    }

    [Test]
    public void AdvanceDay_BeyondLastGame_ReportsSeasonOver()
    {
        var manager = new ManagerDataDB { Name = "Test", TeamId = 1, LeagueId = 1, GameMode = "manager" };
        _db.SaveManager(manager);
        var season = _db.CreateSeason(manager.Id, 1, 2026);
        _db.GenerateLeagueSchedule(season);

        // Coloca la temporada justo después del último partido y avanza un día.
        var last = _db.GetGamesForTeam(season.Id, 1).Max(g => g.MatchDate);
        season.CurrentDate = CalendarHelper.ToDb(DateTime.Parse(last).AddDays(1));
        _db.UpdateSeason(season);

        var result = _db.AdvanceDay(season, 1, out _);
        Assert.That(result, Is.EqualTo(DayAdvanceResult.SeasonOver));
    }

    // ── PANTALLA ───────────────────────────────────────────────────────────

    [Test]
    public void DashboardUxml_HasHeaderSidebarCardsAndDayButton()
    {
        var uxml = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/_TacticalEleven/UI/Screens/Dashboard/Dashboard.uxml");
        Assert.That(uxml, Is.Not.Null, "Falta Dashboard.uxml o no importa como VisualTreeAsset");

        var root = uxml.CloneTree();
        Assert.That(root.Q<VisualElement>("HeaderLogo"), Is.Not.Null);
        Assert.That(root.Q<Label>("HeaderDate"), Is.Not.Null);
        Assert.That(root.Q<Label>("HeaderBudget"), Is.Not.Null);
        Assert.That(root.Q<VisualElement>("SidebarContent"), Is.Not.Null);
        Assert.That(root.Q<VisualElement>("StandingsBody"), Is.Not.Null);
        Assert.That(root.Q<Label>("ObjectiveValue"), Is.Not.Null);
        Assert.That(root.Q<Button>("BtnAdvance"), Is.Not.Null);
        Assert.That(root.Q<VisualElement>("ConfigModalOverlay"), Is.Not.Null);
    }
}
