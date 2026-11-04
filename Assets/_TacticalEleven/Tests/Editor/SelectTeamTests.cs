using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// F5 — Selección de equipo: creación de manager/temporada, peores clubes para
/// ProManager y estructura de la pantalla (pestañas por competición y ficha).
/// </summary>
public class SelectTeamTests
{
    private string _root;
    private GameObject _go;
    private DatabaseManager _db;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "te_select_" + System.Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);

        GameSaveManager.BaseDirOverride = Path.Combine(_root, "saves");
        DatabaseManager.DataRootOverride = Path.Combine(_root, "data");

        _go = new GameObject("DatabaseManagerSelectTest");
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

    // ── MANAGER + TEMPORADA ────────────────────────────────────────────────

    [Test]
    public void CreateManagerAndSeason_StartsInPreseason()
    {
        var manager = new ManagerDataDB
        {
            Name = "Buitrago", TeamId = 1, LeagueId = 1, GameMode = "manager",
            Trust = 50, Morale = 50, FanConfidence = 50
        };
        _db.SaveManager(manager);
        Assert.That(manager.Id, Is.GreaterThan(0));

        var season = _db.CreateSeason(manager.Id, 1, 2026);

        Assert.That(season, Is.Not.Null);
        Assert.That(season.Phase, Is.EqualTo("preseason"));
        Assert.That(season.IsActive, Is.EqualTo(1));
        Assert.That(season.YearStart, Is.EqualTo(2026));
        Assert.That(season.YearEnd, Is.EqualTo(2027));
        Assert.That(season.TotalMatchDays, Is.EqualTo(38), "1ª: 20 equipos, doble vuelta");
        Assert.That(season.CurrentDate, Is.EqualTo("2026-07-15"));
        Assert.That(season.CurrentMatchDay, Is.LessThan(0), "Pretemporada = días negativos");
        Assert.That(_db.GetActiveSeason(manager.Id), Is.Not.Null);
    }

    [Test]
    public void SeasonMatchDays_MatchCompetitionSize()
    {
        int CreateForLeague(int leagueId)
        {
            _db.ClearAllManagers();
            var manager = new ManagerDataDB { Name = "Test", TeamId = 1, LeagueId = leagueId, GameMode = "manager" };
            _db.SaveManager(manager);
            return _db.CreateSeason(manager.Id, leagueId, 2026).TotalMatchDays;
        }

        Assert.That(CreateForLeague(1), Is.EqualTo(38), "1ª: 20 equipos");
        Assert.That(CreateForLeague(2), Is.EqualTo(42), "2ª: 22 equipos");
        Assert.That(CreateForLeague(3), Is.EqualTo(38), "1ª RFEF: 20 equipos");
    }

    [Test]
    public void ClearAllManagers_RemovesManagerAndSeason()
    {
        var manager = new ManagerDataDB { Name = "Test", TeamId = 1, LeagueId = 1, GameMode = "manager" };
        _db.SaveManager(manager);
        _db.CreateSeason(manager.Id, 1, 2026);

        _db.ClearAllManagers();

        Assert.That(_db.GetActiveManager(), Is.Null);
        Assert.That(_db.Db.Table<SeasonDataDB>().Count(), Is.EqualTo(0));
    }

    [Test]
    public void GetWorstTeams_ReturnsLowestRated()
    {
        var worst = _db.GetWorstTeams(5, 1);
        Assert.That(worst.Count, Is.EqualTo(5));
        Assert.That(worst.All(t => t.LeagueId == 1), Is.True);

        var best = _db.GetTeamsByLeague(1).OrderByDescending(t => t.Overall).First();
        Assert.That(worst.Any(t => t.Id == best.Id), Is.False, "El mejor club no puede estar entre los peores");
        Assert.That(worst[0].Overall, Is.LessThanOrEqualTo(worst[worst.Count - 1].Overall));
    }

    [Test]
    public void CurrentSeasonStartYear_UsesJulyAsCutoff()
    {
        int year = DatabaseManager.CurrentSeasonStartYear();
        Assert.That(year, Is.EqualTo(DateTime.Now.Month >= 7 ? DateTime.Now.Year : DateTime.Now.Year - 1));
    }

    // ── FILIALES ───────────────────────────────────────────────────────────

    [Test]
    public void FilialRules_MarksKnownReserveTeams()
    {
        var teams = TeamSeeder.Build();
        FilialRules.Apply(teams);

        var byId = teams.ToDictionary(t => t.Id);
        foreach (var id in new[] { 41, 42, 59, 62, 68, 75, 76 })
        {
            Assert.That(byId[id].IsFilial, Is.EqualTo(1), $"{byId[id].Name} debe ser filial");
            Assert.That(byId[id].ParentClubId, Is.GreaterThan(0),
                $"{byId[id].Name} debe tener club matriz");
        }

        // El primer equipo del filial no se marca.
        Assert.That(byId[4].IsFilial, Is.EqualTo(0), "Real Sociedad no es filial");
        Assert.That(byId[41].ParentClubId, Is.EqualTo(4));
        Assert.That(byId[68].ParentClubId, Is.EqualTo(1));

        var selectable = FilialRules.Selectable(teams).ToList();
        Assert.That(selectable.Any(t => t.IsFilial == 1), Is.False);
        Assert.That(selectable.Any(t => t.Name == "Real Sociedad B"), Is.False);
        Assert.That(selectable.Any(t => t.Name == "Real Madrid Castilla"), Is.False);
        Assert.That(selectable.Count, Is.EqualTo(75), "82 clubes españoles menos 7 filiales");
    }

    [Test]
    public void CanSelect_FilialesAreShownButDisabled()
    {
        var teams = TeamSeeder.Build();
        FilialRules.Apply(teams);
        var byId = teams.ToDictionary(t => t.Id);

        Assert.That(FilialRules.CanSelect(byId[41], false), Is.False,
            "Real Sociedad B no es jugable");
        Assert.That(FilialRules.CanSelect(byId[1], false), Is.True,
            "Real Madrid CF sí es jugable en Manager");
        Assert.That(FilialRules.CanSelect(byId[198], false), Is.False,
            "Los históricos (LeagueId 0) no son jugables");

        // ProManager: solo Primera Federación (grupos 3 y 4) y sin filiales.
        Assert.That(FilialRules.CanSelect(byId[1], true), Is.False, "1ª no vale en ProManager");
        Assert.That(FilialRules.CanSelect(byId[21], true), Is.False, "2ª no vale en ProManager");

        var rfef1 = teams.First(t => t.LeagueId == 3 && t.IsFilial == 0);
        var rfef2 = teams.First(t => t.LeagueId == 4 && t.IsFilial == 0);
        Assert.That(FilialRules.CanSelect(rfef1, true), Is.True, "1ª RFEF G1 sí");
        Assert.That(FilialRules.CanSelect(rfef2, true), Is.True, "1ª RFEF G2 sí");
        Assert.That(FilialRules.CanSelect(byId[59], true), Is.False,
            "Bilbao Athletic (filial de 1ª RFEF) no vale");
    }

    [Test]
    public void SeededSlot_MarksReserveTeamsAsFilial()
    {
        var rsb = _db.Db.Table<TeamDataDB>().FirstOrDefault(t => t.Name == "Real Sociedad B");
        Assert.That(rsb, Is.Not.Null);
        Assert.That(rsb.IsFilial, Is.EqualTo(1));
        Assert.That(rsb.ParentClubId, Is.EqualTo(4));
    }

    [Test]
    public void ProManagerSelectable_AreOnlyNonFilialPrimeraFederacion()
    {
        var teams = TeamSeeder.Build();
        FilialRules.Apply(teams);

        var allowed = teams.Where(t => FilialRules.CanSelect(t, true)).ToList();

        Assert.That(allowed.Count, Is.EqualTo(35), "40 clubes de 1ª RFEF menos los 5 filiales de esa categoría");
        Assert.That(allowed.All(t => t.LeagueId == 3 || t.LeagueId == 4), Is.True);
        Assert.That(allowed.All(t => t.IsFilial == 0), Is.True);
    }

    [Test]
    public void GetWorstTeams_ExcludesReserveTeams()
    {
        var worst = _db.GetWorstTeams(20, 1, 2, 3, 4);
        Assert.That(worst.Any(t => t.IsFilial == 1), Is.False);
    }

    // ── BANDERAS ───────────────────────────────────────────────────────────

    [Test]
    public void Flags_LoadForIso3AndFifaAliases()
    {
        Assert.That(Resources.Load<Sprite>(CountryCodes.GetFlagPath("ESP")), Is.Not.Null,
            "Bandera de España (ISO3)");
        Assert.That(Resources.Load<Sprite>(CountryCodes.GetFlagPath("ARG")), Is.Not.Null);
        Assert.That(Resources.Load<Sprite>(CountryCodes.GetFlagPath("GER")), Is.Not.Null,
            "GER debe resolver al alias DEU");
        Assert.That(Resources.Load<Sprite>(CountryCodes.GetFlagPath("NED")), Is.Not.Null,
            "NED debe resolver al alias NLD");
        Assert.That(Resources.Load<Sprite>(CountryCodes.GetFlagPath("default")), Is.Not.Null,
            "Bandera genérica de reserva");
        Assert.That(CountryCodes.GetName("GER"), Is.EqualTo("Germany"));

        // F6: el panel de detalle muestra el país en español (el seed guarda "Spain").
        Assert.That(CountryCodes.GetNameEs("ESP"), Is.EqualTo("España"));
        Assert.That(CountryCodes.GetNameEs("Spain"), Is.EqualTo("España"));
        Assert.That(CountryCodes.GetNameEs("GER"), Is.EqualTo("Alemania"));
        Assert.That(CountryCodes.GetNameEs("England"), Is.EqualTo("Inglaterra"));
    }

    // ── PANTALLA ───────────────────────────────────────────────────────────

    [Test]
    public void SelectTeamUxml_HasCompetitionTabsAndDetail()
    {
        var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/_TacticalEleven/UI/Screens/SelectTeam/SelectTeam.uxml");
        Assert.That(uxml, Is.Not.Null);

        var root = uxml.Instantiate();
        Assert.That(root.Q<Button>("TabAll"), Is.Null, "La pestaña TODOS ya no existe");
        foreach (var tab in new[] { "TabLiga", "TabLiga2", "TabRfef1", "TabRfef2" })
            Assert.That(root.Q<Button>(tab), Is.Not.Null, $"Falta la pestaña {tab}");

        Assert.That(root.Q<VisualElement>("TeamsGrid"), Is.Not.Null);
        Assert.That(root.Q<TextField>("ManagerNameInput"), Is.Not.Null);
        Assert.That(root.Q<Button>("BtnContinue"), Is.Not.Null);
        Assert.That(root.Q<Button>("BtnShowSquad"), Is.Not.Null);
        Assert.That(root.Q<VisualElement>("SquadModalOverlay"), Is.Not.Null);
        Assert.That(root.Q<Label>("DetailTeamName"), Is.Not.Null);
        Assert.That(root.Q<Label>("DetailConferenceVal"), Is.Not.Null);
        Assert.That(root.Q<Label>("DetailArena"), Is.Not.Null);
    }
}
