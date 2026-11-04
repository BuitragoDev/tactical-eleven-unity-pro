using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// F8 — Motor de partido: formaciones, alineación automática, determinismo,
/// medias realistas, pronóstico y persistencia de resultados/estadísticas.
/// </summary>
public class FootballSimulatorTests
{
    private string _root;
    private GameObject _go;
    private DatabaseManager _db;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "te_match_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);

        GameSaveManager.BaseDirOverride = Path.Combine(_root, "saves");
        DatabaseManager.DataRootOverride = Path.Combine(_root, "data");

        _go = new GameObject("DatabaseManagerMatchTest");
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

    // ── HELPERS ────────────────────────────────────────────────────────────

    private static TeamDataDB MakeTeam(int id, int overall, int formationIndex)
        => new TeamDataDB
        {
            Id = id,
            Name = $"Equipo {id}",
            ShortName = $"E{id}",
            LeagueId = 1,
            Overall = overall,
            Formation = formationIndex,
            StadiumCapacity = 40_000
        };

    private static List<PlayerDataDB> MakeSquad(int teamId, int overall, string formation)
    {
        var squad = new List<PlayerDataDB>();
        int id = teamId * 1000;

        foreach (var slot in FormationRules.Slots(formation))
        {
            id++;
            squad.Add(new PlayerDataDB
            {
                Id = id,
                TeamId = teamId,
                FirstName = "Jugador",
                LastName = id.ToString(),
                MainPosition = (int)slot,
                Overall = overall,
                Potential = overall,
                Finishing = overall,
                OffTheBall = overall,
                Passing = overall,
                Vision = overall,
                Crossing = overall,
                Aggression = overall,
                WorkRate = overall
            });
        }

        for (int i = 0; i < LineupBuilder.BenchSize; i++)
        {
            id++;
            squad.Add(new PlayerDataDB
            {
                Id = id,
                TeamId = teamId,
                FirstName = "Suplente",
                LastName = id.ToString(),
                MainPosition = (int)PlayerPosition.CM,
                Overall = Math.Max(40, overall - 5),
                Potential = overall,
                Finishing = overall - 5,
                OffTheBall = overall - 5,
                Passing = overall - 5,
                Vision = overall - 5,
                Crossing = overall - 5
            });
        }

        return squad;
    }

    private static MatchTeamSetup MakeSetup(int id, int overall, bool home, int formationIndex = 1)
    {
        var team = MakeTeam(id, overall, formationIndex);
        string formation = FormationRules.FromIndex(formationIndex);
        var squad = MakeSquad(id, overall, formation);
        var lineup = LineupBuilder.Build(team, squad, formation);

        return new MatchTeamSetup
        {
            Team = team,
            Starters = lineup.Starters,
            Bench = lineup.Bench,
            Formation = lineup.Formation,
            IsHome = home
        };
    }

    // ── FORMACIONES Y ALINEACIÓN ───────────────────────────────────────────

    [Test]
    public void Formations_HaveElevenSlotsAndExactlyOneGoalkeeper()
    {
        foreach (string formation in FormationRules.All)
        {
            var slots = FormationRules.Slots(formation);
            Assert.That(slots.Length, Is.EqualTo(11), formation);
            Assert.That(slots.Count(s => s == PlayerPosition.GK), Is.EqualTo(1), formation);
        }
    }

    [Test]
    public void Compatibility_IsHighestForNaturalPositionAndPenalizesOutOfPosition()
    {
        Assert.That(FormationRules.Compatibility(PlayerPosition.CB, PlayerPosition.CB),
            Is.EqualTo(1f));

        Assert.That(FormationRules.Compatibility(PlayerPosition.LB, PlayerPosition.CB),
            Is.GreaterThan(0.8f), "Misma línea defensiva");

        Assert.That(FormationRules.Compatibility(PlayerPosition.ST, PlayerPosition.CB),
            Is.LessThan(0.7f), "Delantero en el centro de la zaga");

        Assert.That(FormationRules.Compatibility(PlayerPosition.ST, PlayerPosition.GK),
            Is.LessThan(0.5f), "Jugador de campo en la portería");
    }

    [Test]
    public void LineupBuilder_FillsElevenPlusBench_AndPicksBestPlayers()
    {
        var team = MakeTeam(1, 80, 1);
        var squad = MakeSquad(1, 80, FormationRules.FromIndex(1));

        // Un delantero estrella debe entrar en el once.
        squad[9].Overall = 95;
        squad[9].Finishing = 95;

        var lineup = LineupBuilder.Build(team, squad, FormationRules.FromIndex(1));

        Assert.That(lineup.Starters.Count, Is.EqualTo(11));
        Assert.That(lineup.Bench.Count, Is.EqualTo(LineupBuilder.BenchSize));
        Assert.That(lineup.Starters.Any(p => p.Id == squad[9].Id), Is.True,
            "El mejor delantero debe estar en el once");
        Assert.That(lineup.Starters.Select(p => p.Id).Distinct().Count(), Is.EqualTo(11));
    }

    // ── SIMULACIÓN ─────────────────────────────────────────────────────────

    [Test]
    public void Simulate_IsDeterministicWithTheSameSeed()
    {
        var home = MakeSetup(1, 82, true);
        var away = MakeSetup(2, 78, false);

        var a = FootballSimulator.Simulate(home, away, new SeededRandomSource(12345));
        var b = FootballSimulator.Simulate(home, away, new SeededRandomSource(12345));

        Assert.That(a.HomeScore, Is.EqualTo(b.HomeScore));
        Assert.That(a.AwayScore, Is.EqualTo(b.AwayScore));
        Assert.That(a.Events.Count, Is.EqualTo(b.Events.Count));
        Assert.That(a.HomeShots, Is.EqualTo(b.HomeShots));
        Assert.That(a.MvpPlayerId, Is.EqualTo(b.MvpPlayerId));
    }

    [Test]
    public void Simulate_1000Matches_HasRealisticGoalsAndHomeAdvantage()
    {
        var home = MakeSetup(1, 80, true);
        var away = MakeSetup(2, 80, false);

        int totalGoals = 0, homeGoals = 0, awayGoals = 0;

        for (int i = 0; i < 1000; i++)
        {
            var result = FootballSimulator.Simulate(home, away, new SeededRandomSource((ulong)(i + 1)));
            totalGoals += result.HomeScore + result.AwayScore;
            homeGoals += result.HomeScore;
            awayGoals += result.AwayScore;

            Assert.That(result.HomeScore, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.AwayScore, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.HomePossession + result.AwayPossession, Is.EqualTo(100));
            Assert.That(result.Players.Count, Is.GreaterThanOrEqualTo(22));
        }

        double average = totalGoals / 1000.0;
        Assert.That(average, Is.InRange(2.0, 3.4), $"Media de goles: {average:F2}");
        Assert.That(homeGoals, Is.GreaterThan(awayGoals), "Ventaja de campo");
    }

    [Test]
    public void Simulate_ProducesEventsAndRatings()
    {
        var home = MakeSetup(1, 84, true);
        var away = MakeSetup(2, 76, false);

        var result = FootballSimulator.Simulate(home, away, new SeededRandomSource(7));

        Assert.That(result.Events.Any(e => e.Type == MatchEventType.KickOff), Is.True);
        Assert.That(result.Events.Any(e => e.Type == MatchEventType.FullTime), Is.True);
        Assert.That(result.Events.Count(e => e.Type == MatchEventType.Goal),
            Is.EqualTo(result.HomeScore + result.AwayScore));

        Assert.That(result.Players.All(p => p.Rating >= 1 && p.Rating <= 10), Is.True);
        Assert.That(result.MvpPlayerId, Is.GreaterThan(0));
        Assert.That(result.Players.First(p => p.PlayerId == result.MvpPlayerId).Rating,
            Is.GreaterThanOrEqualTo(result.Players.Max(p => p.Rating) - 0.001));
    }

    // ── PRONÓSTICO ─────────────────────────────────────────────────────────

    [Test]
    public void MatchupPreview_StrongerTeamIsFavoriteAndProbabilitiesAddUp()
    {
        var strong = MakeTeam(1, 88, 1);
        var weak = MakeTeam(2, 68, 1);

        var preview = MatchupPreview.For(strong, weak);

        Assert.That(preview.HomeWin, Is.GreaterThan(preview.AwayWin));
        Assert.That(preview.HomeWin + preview.Draw + preview.AwayWin, Is.EqualTo(1f).Within(0.001f));
    }

    // ── PANTALLAS DE PARTIDO ───────────────────────────────────────────────

    [Test]
    public void MatchContext_KeepsGameAndResultBetweenScreens()
    {
        MatchContext.Clear();
        Assert.That(MatchContext.HasGame, Is.False);

        var game = new GameDataDB { Id = 42, SeasonId = 7 };
        MatchContext.Set(game);

        Assert.That(MatchContext.HasGame, Is.True);
        Assert.That(MatchContext.GameId, Is.EqualTo(42));
        Assert.That(MatchContext.SeasonId, Is.EqualTo(7));

        MatchContext.Result = new MatchSimulationResult { HomeScore = 2, AwayScore = 1 };
        Assert.That(MatchContext.Result.HomeScore, Is.EqualTo(2));

        MatchContext.Clear();
        Assert.That(MatchContext.HasGame, Is.False);
        Assert.That(MatchContext.Result, Is.Null);
    }

    [Test]
    public void MatchScreens_HaveTheirCoreElements()
    {
        var preGame = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/_TacticalEleven/UI/Screens/PreGame/PreGame.uxml");
        Assert.That(preGame, Is.Not.Null);
        var preRoot = preGame.CloneTree();
        Assert.That(preRoot.Q<Button>("BtnGoMatch"), Is.Not.Null);
        Assert.That(preRoot.Q<VisualElement>("MPVHomeStarters"), Is.Not.Null);
        Assert.That(preRoot.Q<VisualElement>("MPVAwayStarters"), Is.Not.Null);
        Assert.That(preRoot.Q<VisualElement>("MPVHomeBajas"), Is.Not.Null);
        Assert.That(preRoot.Q<VisualElement>("MPVHomeBench"), Is.Null, "El banquillo ya no se muestra");
        Assert.That(preRoot.Q<VisualElement>("MPVAwayBench"), Is.Null, "El banquillo ya no se muestra");

        var matchDay = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/_TacticalEleven/UI/Screens/MatchDay/MatchDay.uxml");
        Assert.That(matchDay, Is.Not.Null);
        var mdRoot = matchDay.CloneTree();
        Assert.That(mdRoot.Q<Label>("HomeScore"), Is.Not.Null);
        Assert.That(mdRoot.Q<Label>("AwayScore"), Is.Not.Null);
        Assert.That(mdRoot.Q<VisualElement>("TimelineBody"), Is.Not.Null);
        Assert.That(mdRoot.Q<VisualElement>("HomeBoxBody"), Is.Not.Null);
        Assert.That(mdRoot.Q<Button>("BtnContinue"), Is.Not.Null);

        var results = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/_TacticalEleven/UI/Screens/GameResults/GameResults.uxml");
        Assert.That(results, Is.Not.Null);
        var grRoot = results.CloneTree();
        Assert.That(grRoot.Q<VisualElement>("GamesBody"), Is.Not.Null);
        Assert.That(grRoot.Q<VisualElement>("FiltersBar"), Is.Not.Null);
        Assert.That(grRoot.Q<Button>("FilterLiga"), Is.Null,
            "Los filtros se generan dinámicamente con las competiciones del día");
        Assert.That(grRoot.Q<VisualElement>("MvpPanel"), Is.Not.Null);
        Assert.That(grRoot.Q<VisualElement>("LbScorersBody"), Is.Not.Null);
        Assert.That(grRoot.Q<Label>("MvpVal"), Is.Not.Null);
        Assert.That(grRoot.Q<Label>("MvpGoals"), Is.Null, "El MVP solo muestra la nota");
        Assert.That(grRoot.Q<Label>("MvpAssists"), Is.Null, "El MVP solo muestra la nota");
        Assert.That(grRoot.Q<Button>("BtnDashboard"), Is.Not.Null);
    }

    [Test]
    public void Scene_HasTheThreeMatchScreensWired()
    {
        string scene = System.IO.File.ReadAllText("Assets/_TacticalEleven/Scenes/MainMenu.unity");

        Assert.That(scene.Contains("m_Name: PreGameDocument"), Is.True);
        Assert.That(scene.Contains("m_Name: MatchDayDocument"), Is.True);
        Assert.That(scene.Contains("m_Name: GameResultsDocument"), Is.True);

        Assert.That(System.Text.RegularExpressions.Regex.IsMatch(
            scene, @"preGameDocument: \{fileID: (?!0\})"), Is.True);
        Assert.That(System.Text.RegularExpressions.Regex.IsMatch(
            scene, @"matchDayDocument: \{fileID: (?!0\})"), Is.True);
        Assert.That(System.Text.RegularExpressions.Regex.IsMatch(
            scene, @"gameResultsDocument: \{fileID: (?!0\})"), Is.True);
    }

    // ── PERSISTENCIA ───────────────────────────────────────────────────────

    [Test]
    public void PlayToday_PersistsResultEventsPlayerStatsAndSeasonTotals()
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
        Assert.That(_db.GenerateLeagueSchedule(season), Is.GreaterThan(0));

        var myFirstGame = _db.GetGamesForTeam(season.Id, 1)
            .Where(g => g.GameType == DatabaseManager.GameTypeRegular)
            .OrderBy(g => g.MatchDate)
            .First();

        season.CurrentDate = myFirstGame.MatchDate;
        _db.UpdateSeason(season);

        Assert.That(_db.PlayToday(season, 1, out var result), Is.True);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.HomeTeamId == 1 || result.AwayTeamId == 1, Is.True);

        var played = _db.GetGamesBySeason(season.Id)
            .First(g => g.Id == myFirstGame.Id);
        Assert.That(played.IsPlayed, Is.EqualTo(1));
        Assert.That(played.HomeScore, Is.EqualTo(result.HomeScore));
        Assert.That(played.AwayScore, Is.EqualTo(result.AwayScore));

        Assert.That(_db.GetMatchEvents(played.Id).Count, Is.GreaterThan(0));
        Assert.That(_db.GetPlayerMatchStats(played.Id).Count, Is.GreaterThanOrEqualTo(22));

        var seasonStats = _db.GetPlayerSeasonStats(1, season.YearStart);
        Assert.That(seasonStats.Count, Is.GreaterThanOrEqualTo(11));

        int myGoals = result.HomeTeamId == 1 ? result.HomeScore : result.AwayScore;
        Assert.That(seasonStats.Sum(s => s.Goals), Is.EqualTo(myGoals));

        // Ya no quedan partidos pendientes en esa fecha.
        Assert.That(_db.PlayToday(season, 1, out _), Is.False);
    }
}
