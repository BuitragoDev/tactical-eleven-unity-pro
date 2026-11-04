using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// S1 — Calendario: días reales por competición, generación completa de la
/// temporada (ligas, Europa y Copa) y pantalla de calendario.
/// </summary>
public class CalendarScreenTests
{
    private string _root;
    private GameObject _go;
    private DatabaseManager _db;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "te_calendar_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);

        GameSaveManager.BaseDirOverride = Path.Combine(_root, "saves");
        DatabaseManager.DataRootOverride = Path.Combine(_root, "data");

        _go = new GameObject("DatabaseManagerCalendarTest");
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

    // ── DÍAS REALES ────────────────────────────────────────────────────────

    [Test]
    public void EuropeRoundDates_UseTheRealWeekday()
    {
        foreach (int competitionId in new[] { 5, 6, 7 })
        {
            var dates = CompetitionCalendarRules.EuropeRoundDates(2026, competitionId);
            Assert.That(dates.Count, Is.EqualTo(CompetitionCalendarRules.EuropeRounds));

            int weekday = CompetitionCalendarRules.EuropeWeekday(competitionId);
            foreach (var date in dates)
            {
                Assert.That((int)date.DayOfWeek, Is.EqualTo(weekday),
                    $"Competición {competitionId} en {date:yyyy-MM-dd}");
                Assert.That(FootballCalendar.IsBreak(date), Is.False,
                    $"Fecha europea en parón: {date:yyyy-MM-dd}");
            }
        }

        Assert.That(CompetitionCalendarRules.EuropeWeekday(5), Is.EqualTo((int)DayOfWeek.Tuesday));
        Assert.That(CompetitionCalendarRules.EuropeWeekday(6), Is.EqualTo((int)DayOfWeek.Wednesday));
        Assert.That(CompetitionCalendarRules.EuropeWeekday(7), Is.EqualTo((int)DayOfWeek.Thursday));
    }

    [Test]
    public void CupRoundDates_AreTuesdayOrWednesdayAndAvoidBreaks()
    {
        var dates = CompetitionCalendarRules.CupRoundDates(2026);
        Assert.That(dates.Count, Is.GreaterThanOrEqualTo(7));

        foreach (var date in dates)
        {
            Assert.That(date.DayOfWeek == DayOfWeek.Tuesday
                        || date.DayOfWeek == DayOfWeek.Wednesday, Is.True,
                $"Copa en día no permitido: {date:yyyy-MM-dd}");
            Assert.That(FootballCalendar.IsBreak(date), Is.False);
        }
    }

    // ── GENERADORES ────────────────────────────────────────────────────────

    [Test]
    public void EuropeSchedule_36Teams_PlayEightDifferentOpponents()
    {
        var ids = Enumerable.Range(1, 36).ToList();
        var fixtures = EuropeScheduleGenerator.Generate(ids, 5, 2026, new SeededRandomSource(3));

        Assert.That(fixtures.Count, Is.EqualTo(144), "8 jornadas × 18 partidos");
        Assert.That(fixtures.Max(f => f.MatchDay), Is.EqualTo(8));

        foreach (int id in ids)
        {
            var games = fixtures.Where(f => f.HomeTeamId == id || f.AwayTeamId == id).ToList();
            Assert.That(games.Count, Is.EqualTo(8), $"Equipo {id}");

            var opponents = games
                .Select(f => f.HomeTeamId == id ? f.AwayTeamId : f.HomeTeamId)
                .Distinct()
                .Count();
            Assert.That(opponents, Is.EqualTo(8), $"Rivales repetidos del equipo {id}");
        }
    }

    [Test]
    public void CupPreliminary_82SpanishTeams_Produces18Ties()
    {
        var teams = Enumerable.Range(1, 82)
            .Select(i => new TeamDataDB { Id = i, Overall = 100 - i })
            .ToList();

        var fixtures = CupScheduleGenerator.GeneratePreliminary(
            teams, 2026, null, new SeededRandomSource(1));

        Assert.That(fixtures.Count, Is.EqualTo(18), "82 − 64 = 18 eliminatorias");
        var playing = fixtures.SelectMany(f => new[] { f.HomeTeamId, f.AwayTeamId }).ToList();
        Assert.That(playing.Count, Is.EqualTo(36));
        Assert.That(playing.Distinct().Count(), Is.EqualTo(36));
    }

    // ── TEMPORADA COMPLETA ─────────────────────────────────────────────────

    [Test]
    public void GenerateFullSeason_PersistsLeaguesEuropeAndCup()
    {
        var manager = new ManagerDataDB
        {
            Name = "Test",
            TeamId = 1,
            LeagueId = 1,
            GameMode = "manager"
        };
        _db.SaveManager(manager);

        var season = _db.CreateSeason(manager.Id, 1, 2026);
        int total = _db.GenerateFullSeason(season, new SeededRandomSource(7));
        Assert.That(total, Is.GreaterThan(0));

        var regular = _db.GetGamesByType(season.Id, DatabaseManager.GameTypeRegular);
        var europe = _db.GetGamesByType(season.Id, DatabaseManager.GameTypeEurope);
        var cup = _db.GetGamesByType(season.Id, DatabaseManager.GameTypeCup);

        Assert.That(regular.Count, Is.EqualTo(1602), "380 + 462 + 380 + 380");
        Assert.That(europe.Count, Is.EqualTo(432), "3 competiciones × 8 jornadas × 18");
        Assert.That(cup.Count, Is.EqualTo(18));

        // Europa en su día real.
        foreach (int competitionId in new[] { 5, 6, 7 })
        {
            int weekday = CompetitionCalendarRules.EuropeWeekday(competitionId);
            var games = europe.Where(g => g.CompetitionId == competitionId).ToList();
            Assert.That(games.Count, Is.EqualTo(144));
            Assert.That(games.All(g => (int)DateTime.Parse(g.MatchDate).DayOfWeek == weekday), Is.True);
        }

        // Copa en martes/miércoles.
        Assert.That(cup.All(g =>
        {
            var day = DateTime.Parse(g.MatchDate).DayOfWeek;
            return day == DayOfWeek.Tuesday || day == DayOfWeek.Wednesday;
        }), Is.True);

        // Ningún equipo juega dos partidos con menos de 48 h.
        var all = _db.GetGamesBySeason(season.Id);
        var byTeam = new Dictionary<int, List<DateTime>>();

        foreach (var game in all)
        {
            AddDate(byTeam, game.HomeTeamId, game.MatchDate);
            AddDate(byTeam, game.AwayTeamId, game.MatchDate);
        }

        foreach (var pair in byTeam)
        {
            var dates = pair.Value.OrderBy(d => d).ToList();
            for (int i = 1; i < dates.Count; i++)
            {
                Assert.That((dates[i] - dates[i - 1]).TotalHours, Is.GreaterThanOrEqualTo(48),
                    $"Equipo {pair.Key}: {dates[i - 1]:yyyy-MM-dd} → {dates[i]:yyyy-MM-dd}");
            }
        }
    }

    private static void AddDate(Dictionary<int, List<DateTime>> map, int teamId, string dbDate)
    {
        if (!map.TryGetValue(teamId, out var list))
            map[teamId] = list = new List<DateTime>();
        list.Add(DateTime.Parse(dbDate));
    }

    // ── PANTALLA ───────────────────────────────────────────────────────────

    [Test]
    public void CalendarUxml_HasGridDayPanelAndLegend()
    {
        var uxml = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/_TacticalEleven/UI/Screens/Calendar/Calendar.uxml");
        Assert.That(uxml, Is.Not.Null);

        var root = uxml.CloneTree();
        Assert.That(root.Q<VisualElement>("CalendarDays"), Is.Not.Null);
        Assert.That(root.Q<VisualElement>("SelectedDayGames"), Is.Not.Null);
        Assert.That(root.Q<Label>("SelectedDayTitle"), Is.Not.Null);
        Assert.That(root.Q<Label>("NoGamesText"), Is.Not.Null);
        Assert.That(root.Q<Button>("BtnPrevMonth"), Is.Not.Null);
        Assert.That(root.Q<Button>("BtnNextMonth"), Is.Not.Null);
        Assert.That(root.Q<VisualElement>("CalendarScreenIcon"), Is.Not.Null);
        Assert.That(root.Q<Label>("CalendarSubtitle"), Is.Not.Null);
    }

    [Test]
    public void Scene_HasCalendarDocumentWired()
    {
        string scene = File.ReadAllText("Assets/_TacticalEleven/Scenes/MainMenu.unity");
        Assert.That(scene.Contains("m_Name: CalendarDocument"), Is.True);
        Assert.That(System.Text.RegularExpressions.Regex.IsMatch(
            scene, @"calendarDocument: \{fileID: (?!0\})"), Is.True);
    }
}
