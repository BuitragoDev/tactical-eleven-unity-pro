using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// F7 — Creación de partida (réplica de Tactical5): la temporada, el calendario y
/// los metadatos del slot solo se confirman al CONTINUAR en Pretemporada. Si se
/// abandona antes, no hay partida en Cargar partida.
/// </summary>
public class GameCreationFlowTests
{
    private string _root;
    private GameObject _go;
    private DatabaseManager _db;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "te_creation_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);

        GameSaveManager.BaseDirOverride = Path.Combine(_root, "saves");
        DatabaseManager.DataRootOverride = Path.Combine(_root, "data");

        _go = new GameObject("DatabaseManagerCreationTest");
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

    private ManagerDataDB CreateManager()
    {
        var manager = new ManagerDataDB
        {
            Name = "Ana",
            TeamId = 1,
            LeagueId = 1,
            GameMode = "manager",
            Trust = 50,
            Morale = 50,
            FanConfidence = 50
        };
        _db.SaveManager(manager);
        return manager;
    }

    // ── COMMIT ─────────────────────────────────────────────────────────────

    [Test]
    public void NewGame_IsNotVisibleInLoadGameUntilPreseasonCommit()
    {
        var manager = CreateManager();
        var season = _db.CreateSeason(manager.Id, 1, 2026);
        Assert.That(_db.GenerateLeagueSchedule(season), Is.GreaterThan(0));

        // Selección de equipo + pretemporada sin CONTINUAR: no hay slot visible.
        var slot = GameSaveManager.GetSlot(1);
        Assert.That(slot == null || !slot.exists, Is.True,
            "La partida no debe aparecer en Cargar partida antes del commit");
        Assert.That(GameSaveManager.CountExistingSlots(), Is.EqualTo(0));
        Assert.That(GameSaveManager.FindNextAvailableSlot(), Is.EqualTo(1));

        // CONTINUAR en Pretemporada: commit de los metadatos.
        GameSaveManager.UpdateSlotFromDatabase(1);

        slot = GameSaveManager.GetSlot(1);
        Assert.That(slot, Is.Not.Null);
        Assert.That(slot.exists, Is.True);
        Assert.That(slot.managerName, Is.EqualTo("Ana"));
        Assert.That(slot.teamName, Is.EqualTo("Real Madrid CF"));
        Assert.That(GameSaveManager.CountExistingSlots(), Is.EqualTo(1));
    }

    [Test]
    public void PreseasonCommit_CreatesMissingSeasonAndLeagueCalendar()
    {
        var manager = CreateManager();

        Assert.That(_db.GetActiveSeason(manager.Id), Is.Null,
            "La temporada no debe existir antes de CONTINUAR");

        // Lo que hace PreseasonController.OnContinue.
        var season = _db.CreateSeason(manager.Id, 1, 2026);
        Assert.That(season, Is.Not.Null);
        Assert.That(_db.GenerateLeagueSchedule(season), Is.EqualTo(380));
        Assert.That(_db.GetGamesByType(season.Id, DatabaseManager.GameTypeRegular).Count,
            Is.EqualTo(380));
        Assert.That(season.TotalMatchDays, Is.EqualTo(38));
    }

    [Test]
    public void DiscardPendingGame_RemovesManagerSeasonGamesAndLeavesNoSlot()
    {
        var manager = CreateManager();
        var season = _db.CreateSeason(manager.Id, 1, 2026);
        _db.GenerateLeagueSchedule(season);
        int seasonId = season.Id;

        _db.ReplacePreseasonGames(season, new List<GameDataDB>
        {
            new GameDataDB
            {
                MatchDay = -4,
                MatchDate = $"{season.YearStart}-07-18",
                HomeTeamId = 1,
                AwayTeamId = 2,
                GameType = DatabaseManager.GameTypePreseason
            }
        });
        Assert.That(_db.GetGamesByType(seasonId, DatabaseManager.GameTypePreseason).Count, Is.EqualTo(1));

        // VOLVER antes de CONTINUAR.
        _db.DiscardPendingGame();

        Assert.That(_db.GetActiveManager(), Is.Null);
        Assert.That(_db.Db.Table<SeasonDataDB>().Count(), Is.EqualTo(0));
        Assert.That(_db.GetGamesBySeason(seasonId), Is.Empty);

        var slot = GameSaveManager.GetSlot(1);
        Assert.That(slot == null || !slot.exists, Is.True,
            "Una partida abandonada no debe quedar listada");
    }

    [Test]
    public void StartingAgainAfterDiscard_ReusesTheSameSlot()
    {
        var manager = CreateManager();
        var season = _db.CreateSeason(manager.Id, 1, 2026);
        _db.GenerateLeagueSchedule(season);
        _db.DiscardPendingGame();

        // Nueva intentona en el mismo hueco.
        var manager2 = CreateManager();
        var season2 = _db.CreateSeason(manager2.Id, 1, 2026);

        Assert.That(season2.Id, Is.GreaterThan(0));
        Assert.That(GameSaveManager.FindNextAvailableSlot(), Is.EqualTo(1));
        Assert.That(_db.GetActiveSeason(manager2.Id), Is.Not.Null);
    }
}
