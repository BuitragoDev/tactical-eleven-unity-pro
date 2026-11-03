using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// F4 — Ciclo de vida de slots que usa Cargar partida: alta, metadatos
/// (equipo/manager/temporada), listado y borrado.
/// </summary>
public class SaveSlotLifecycleTests
{
    private string _root;
    private GameObject _go;
    private DatabaseManager _db;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "te_slots_" + System.Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);

        GameSaveManager.BaseDirOverride = Path.Combine(_root, "saves");
        DatabaseManager.DataRootOverride = Path.Combine(_root, "data");

        _go = new GameObject("DatabaseManagerSlotTest");
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

    [Test]
    public void UpdateSlotFromDatabase_StoresTeamManagerAndSeason()
    {
        _db.Db.Insert(new ManagerDataDB
        {
            Name = "Buitrago",
            TeamId = 1,
            LeagueId = 1,
            GameMode = "manager",
            Trust = 100,
            Morale = 50,
            FanConfidence = 50
        });
        _db.Db.Insert(new SeasonDataDB
        {
            YearStart = 2026,
            YearEnd = 2027,
            IsActive = 1,
            CurrentMatchDay = 3,
            TotalMatchDays = 38,
            Phase = "regular",
            ManagerId = 1,
            LeagueId = 1,
            CurrentDate = "2026-08-30"
        });

        GameSaveManager.UpdateSlotFromDatabase(1);

        var slot = GameSaveManager.GetSlot(1);
        Assert.That(slot, Is.Not.Null);
        Assert.That(slot.exists, Is.True);
        Assert.That(slot.teamName, Is.EqualTo("Real Madrid CF"));
        Assert.That(slot.managerName, Is.EqualTo("Buitrago"));
        Assert.That(slot.teamLogo, Is.EqualTo("1"), "El logo del slot es el id/nombre del archivo");
        Assert.That(slot.seasonYear, Is.EqualTo("2026-2027"));
        Assert.That(slot.currentMatchDay, Is.EqualTo(3));
        Assert.That(slot.gameMode, Is.EqualTo("manager"));
        Assert.That(slot.leagueName, Is.EqualTo("LaLiga EA Sports"));
    }

    [Test]
    public void GetAllSlots_ThenDelete_RemovesSlotAndFreesNumber()
    {
        GameSaveManager.UpdateSlotFromDatabase(1);
        Assert.That(GameSaveManager.GetAllSlots().Count(s => s.exists), Is.EqualTo(1));
        Assert.That(GameSaveManager.HasFreeSlot(), Is.True);

        GameSaveManager.DeleteSave(1);

        var slots = GameSaveManager.GetAllSlots();
        Assert.That(slots.Any(s => s.exists && s.slotNumber == 1), Is.False);
        Assert.That(File.Exists(GameSaveManager.GetSaveDbPath(1)), Is.False);
        Assert.That(GameSaveManager.FindNextAvailableSlot(), Is.EqualTo(1));
    }

    [Test]
    public void FindNextAvailableSlot_SkipsUsedOnes()
    {
        GameSaveManager.UpdateSlotFromDatabase(1);
        Assert.That(GameSaveManager.FindNextAvailableSlot(), Is.EqualTo(2), "El 1 ya está usado");

        _db.CloseActiveSlot();
        Assert.That(_db.InitSaveSlot(2), Is.True);
        GameSaveManager.UpdateSlotFromDatabase(2);
        Assert.That(GameSaveManager.FindNextAvailableSlot(), Is.EqualTo(3));
    }

    [Test]
    public void ConfigSimMode_UsesPlayerPrefs()
    {
        PlayerPrefs.DeleteKey("TF_SimMode");
        Assert.That(UIScreenController.GetSimMode(), Is.EqualTo((int)SimMode.Direct));

        PlayerPrefs.SetInt("TF_SimMode", (int)SimMode.PlayByPlay);
        Assert.That(UIScreenController.GetSimMode(), Is.EqualTo((int)SimMode.PlayByPlay));

        PlayerPrefs.DeleteKey("TF_SimMode");
    }
}
