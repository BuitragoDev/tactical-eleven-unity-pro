using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Selección de equipo: rejilla por competición (1ª, 2ª, 1ª RFEF G1/G2), ficha del
/// club, plantilla y nombre del manager. Al continuar crea manager + temporada y
/// abre la pretemporada.
/// </summary>
public class SelectTeamController : UIScreenController
{
    private Button _btnBack;
    private Button _btnContinue;
    private Label _headerMode;
    private TextField _managerInput;

    private Button _tabLiga;
    private Button _tabLiga2;
    private Button _tabRfef1;
    private Button _tabRfef2;

    private VisualElement _teamsGrid;
    private VisualElement _detailPlaceholder;
    private VisualElement _detailScrollRow;
    private VisualElement _detailPanel;
    private VisualElement _detailLogo;
    private Label _detailTeamName;
    private Label _detailCity;
    private Label _detailCompetition;
    private Label _detailFoundation;
    private Label _detailCountry;
    private Label _detailStadium;
    private Label _detailCapacity;
    private VisualElement _detailReputation;
    private VisualElement _detailFacilities;
    private Label _detailObjective;
    private Label _detailBudget;
    private Label _detailWageBill;
    private Label _detailAttack;
    private Label _detailDefense;
    private Label _detailOverall;
    private Button _btnShowSquad;

    private VisualElement _squadOverlay;
    private VisualElement _squadLogo;
    private Label _squadTeamName;
    private Label _squadAvg;
    private VisualElement _squadLeft;
    private VisualElement _squadRight;
    private Button _btnCloseSquad;

    private CustomScrollbar.Handle _gridBar;
    private CustomScrollbar.Handle _detailBar;
    private CustomScrollbar.Handle _squadBar;

    private List<TeamDataDB> _allTeams = new();
    private Dictionary<int, List<PlayerDataDB>> _playersByTeam = new();
    private Dictionary<string, Sprite> _logoSprites = new();
    private readonly Dictionary<string, Sprite> _flagSprites = new();
    private TeamDataDB _selectedTeam;
    private string _filter = "all";

    public override GameScreen ScreenId => GameScreen.SelectTeam;

    protected override void CacheReferences()
    {
        _btnBack = _root.Q<Button>("BtnBack");
        _btnContinue = _root.Q<Button>("BtnContinue");
        _headerMode = _root.Q<Label>("HeaderMode");
        _managerInput = _root.Q<TextField>("ManagerNameInput");

        _tabLiga = _root.Q<Button>("TabLiga");
        _tabLiga2 = _root.Q<Button>("TabLiga2");
        _tabRfef1 = _root.Q<Button>("TabRfef1");
        _tabRfef2 = _root.Q<Button>("TabRfef2");

        _teamsGrid = _root.Q<VisualElement>("TeamsGrid");
        _detailPlaceholder = _root.Q<VisualElement>("DetailPlaceholder");
        _detailScrollRow = _root.Q<VisualElement>("DetailScrollRow");
        _detailPanel = _root.Q<VisualElement>("DetailPanel");

        _detailLogo = _root.Q<VisualElement>("DetailLogo");
        _detailTeamName = _root.Q<Label>("DetailTeamName");
        _detailCity = _root.Q<Label>("DetailCity");
        _detailCompetition = _root.Q<Label>("DetailConferenceVal");
        _detailFoundation = _root.Q<Label>("DetailDivision");
        _detailCountry = _root.Q<Label>("DetailOwner");
        _detailStadium = _root.Q<Label>("DetailArena");
        _detailCapacity = _root.Q<Label>("DetailCapacity");
        _detailReputation = _root.Q<VisualElement>("DetailReputation");
        _detailFacilities = _root.Q<VisualElement>("DetailFacilities");
        _detailObjective = _root.Q<Label>("DetailObjective");
        _detailBudget = _root.Q<Label>("DetailBudget");
        _detailWageBill = _root.Q<Label>("DetailSalaryMargin");
        _detailAttack = _root.Q<Label>("DetailAttack");
        _detailDefense = _root.Q<Label>("DetailDefense");
        _detailOverall = _root.Q<Label>("DetailOverall");
        _btnShowSquad = _root.Q<Button>("BtnShowSquad");

        _squadOverlay = _root.Q<VisualElement>("SquadModalOverlay");
        _squadLogo = _root.Q<VisualElement>("SquadModalLogo");
        _squadTeamName = _root.Q<Label>("SquadModalTeamName");
        _squadAvg = _root.Q<Label>("SquadModalAvg");
        _squadLeft = _root.Q<VisualElement>("SquadColumnLeft");
        _squadRight = _root.Q<VisualElement>("SquadColumnRight");
        _btnCloseSquad = _root.Q<Button>("BtnCloseSquad");

        foreach (var b in new[] { _btnBack, _btnContinue, _tabLiga, _tabLiga2,
                                  _tabRfef1, _tabRfef2, _btnShowSquad, _btnCloseSquad })
            RegisterHandCursor(b);
    }

    protected override void LoadData()
    {
        base.LoadData();
        var db = DatabaseManager.Instance;

        // Todos los clubes de 1ª, 2ª y 1ª RFEF (los filiales se muestran deshabilitados).
        _allTeams = db.GetAllTeams()
            .Where(t => t.LeagueId >= 1 && t.LeagueId <= 4)
            .OrderBy(t => t.Name)
            .ToList();

        foreach (var sprite in Resources.LoadAll<Sprite>("Teams/Logos/120x120"))
            _logoSprites[sprite.name] = sprite;

        _playersByTeam = db.Db.Table<PlayerDataDB>()
            .Where(p => p.TeamId != 0)
            .ToList()
            .GroupBy(p => p.TeamId)
            .ToDictionary(g => g.Key, g => g.ToList());

        SetupScrollbars();
    }

    private void SetupScrollbars()
    {
        var viewport = _root.Q<VisualElement>("GridViewport");
        if (viewport != null)
            _gridBar = CustomScrollbar.Bind(viewport, _teamsGrid,
                _root.Q<VisualElement>("GridScrollTrack"),
                _root.Q<VisualElement>("GridScrollThumb"));

        var detailViewport = _root.Q<VisualElement>("DetailViewport");
        if (detailViewport != null)
            _detailBar = CustomScrollbar.Bind(detailViewport, _detailPanel,
                _root.Q<VisualElement>("DetailScrollTrack"),
                _root.Q<VisualElement>("DetailScrollThumb"));

        var squadViewport = _root.Q<VisualElement>("SquadModalViewport");
        if (squadViewport != null)
            _squadBar = CustomScrollbar.Bind(squadViewport,
                _root.Q<VisualElement>("SquadColumnLeft")?.parent,
                _root.Q<VisualElement>("SquadModalScrollTrack"),
                _root.Q<VisualElement>("SquadModalScrollThumb"));
    }

    protected override void RegisterCallbacks()
    {
        _btnBack?.RegisterCallback<ClickEvent>(_ => { PlayClick(); DiscardPendingGame(); ScreenManager.Instance.GoTo(GameScreen.MainMenu); });
        _btnContinue?.RegisterCallback<ClickEvent>(_ => { PlayClick(); OnContinue(); });

        _tabLiga?.RegisterCallback<ClickEvent>(_ => { PlayClick(); ShowFilter("liga"); });
        _tabLiga2?.RegisterCallback<ClickEvent>(_ => { PlayClick(); ShowFilter("liga2"); });
        _tabRfef1?.RegisterCallback<ClickEvent>(_ => { PlayClick(); ShowFilter("rfef1"); });
        _tabRfef2?.RegisterCallback<ClickEvent>(_ => { PlayClick(); ShowFilter("rfef2"); });

        _managerInput?.RegisterValueChangedCallback(_ => ValidateContinue());
        _btnShowSquad?.RegisterCallback<ClickEvent>(_ => { PlayClick(); ShowSquadModal(); });
        _btnCloseSquad?.RegisterCallback<ClickEvent>(_ => { PlayClick(); CloseSquadModal(); });
    }

    protected override void Refresh()
    {
        bool pro = ScreenManager.Instance.CurrentMode == GameMode.ProManager;
        if (_headerMode != null)
            _headerMode.text = pro ? "PROMANAGER" : "MANAGER";

        if (_detailScrollRow != null) _detailScrollRow.style.display = DisplayStyle.None;
        if (_detailPlaceholder != null) _detailPlaceholder.style.display = DisplayStyle.Flex;
        _selectedTeam = null;
        if (_managerInput != null && string.IsNullOrEmpty(_managerInput.value))
            _managerInput.value = "";

        CloseSquadModal();
        // En ProManager lo válido es 1ª RFEF, así que se abre en su primer grupo.
        ShowFilter(pro ? "rfef1" : "liga");
        ValidateContinue();
    }

    // ── Filtros y rejilla ─────────────────────────────────────────────────

    private void ShowFilter(string filter)
    {
        _filter = filter;
        SetTabActive(_tabLiga, filter == "liga");
        SetTabActive(_tabLiga2, filter == "liga2");
        SetTabActive(_tabRfef1, filter == "rfef1");
        SetTabActive(_tabRfef2, filter == "rfef2");

        int leagueId = filter switch
        {
            "liga2" => 2,
            "rfef1" => 3,
            "rfef2" => 4,
            _ => 1
        };

        var teams = _allTeams.Where(t => t.LeagueId == leagueId).ToList();

        BuildGrid(teams);
    }

    private void SetTabActive(Button tab, bool active)
    {
        if (tab == null) return;
        tab.EnableInClassList("tab-btn--active", active);
    }

    private void BuildGrid(List<TeamDataDB> teams)
    {
        if (_teamsGrid == null) return;
        _teamsGrid.Clear();

        bool pro = ScreenManager.Instance.CurrentMode == GameMode.ProManager;

        foreach (var team in teams)
        {
            bool disabled = !FilialRules.CanSelect(team, pro);
            _teamsGrid.Add(CreateTeamItem(team, disabled));
        }

        CustomScrollbar.Refresh(_gridBar);
    }

    private VisualElement CreateTeamItem(TeamDataDB team, bool disabled)
    {
        var item = new VisualElement();
        item.AddToClassList("team-item");
        if (disabled && team.IsFilial == 1)
            item.tooltip = $"{team.Name} · {LocalizationService.T("ui.select_team.filial_no_seleccionable")}";
        else
            item.tooltip = team.Name;

        if (disabled) item.AddToClassList("team-item--disabled");

        var logo = new VisualElement();
        logo.AddToClassList("team-logo");
        if (_logoSprites.TryGetValue(team.Logo ?? team.Id.ToString(), out var sprite))
            logo.style.backgroundImage = new StyleBackground(sprite);
        item.Add(logo);

        if (team.Id == _selectedTeam?.Id)
            item.AddToClassList("team-item--selected");

        if (!disabled)
        {
            item.RegisterCallback<ClickEvent>(_ => { PlayClick(); OnTeamSelected(team, item); });
            RegisterHandCursor(item);
        }

        return item;
    }

    private void OnTeamSelected(TeamDataDB team, VisualElement item)
    {
        _selectedTeam = team;

        foreach (var e in _teamsGrid.Query<VisualElement>(className: "team-item--selected").ToList())
            e.RemoveFromClassList("team-item--selected");
        item.AddToClassList("team-item--selected");

        ShowTeamDetail(team);
        ValidateContinue();
    }

    // ── Ficha del club ────────────────────────────────────────────────────

    private void ShowTeamDetail(TeamDataDB team)
    {
        if (_detailPlaceholder != null) _detailPlaceholder.style.display = DisplayStyle.None;
        if (_detailScrollRow != null) _detailScrollRow.style.display = DisplayStyle.Flex;

        if (_logoSprites.TryGetValue(team.Logo ?? team.Id.ToString(), out var sprite))
            _detailLogo.style.backgroundImage = new StyleBackground(sprite);

        var league = DatabaseManager.Instance.GetLeagueById(team.LeagueId);

        SetText(_detailTeamName, team.Name.ToUpperInvariant());
        SetText(_detailCity, team.City);
        SetText(_detailCompetition, league?.Name ?? "—");
        SetText(_detailFoundation, team.YearFounded > 0 ? team.YearFounded.ToString() : "—");
        SetText(_detailCountry, CountryCodes.GetNameEs(team.Country));
        SetText(_detailStadium, team.StadiumName);
        SetText(_detailCapacity, team.StadiumCapacity > 0 ? team.StadiumCapacity.ToString("N0") : "—");
        SetText(_detailBudget, $"{team.TransferBudget:N0} €");
        SetText(_detailWageBill, $"{team.TotalWageBill:N0} €");
        SetText(_detailAttack, team.Attack.ToString());
        SetText(_detailDefense, team.Defense.ToString());
        SetText(_detailOverall, team.Overall.ToString());
        SetText(_detailObjective, ObjectiveFor(team));

        BuildStars(_detailReputation, Mathf.Clamp(Mathf.RoundToInt((float)team.Reputation / 20f), 0, 5));
        BuildStars(_detailFacilities, Mathf.Clamp(team.TrainingFacilities, 0, 5));

        CustomScrollbar.Reset(_detailBar);
    }

    private static void SetText(Label label, string value)
    {
        if (label != null) label.text = value ?? "";
    }

    private void BuildStars(VisualElement container, int filled)
    {
        if (container == null) return;
        container.Clear();
        for (int i = 1; i <= 5; i++)
        {
            var star = new VisualElement();
            star.AddToClassList("star");
            if (i <= filled) star.AddToClassList("star--filled");
            container.Add(star);
        }
    }

    /// <summary>Objetivo orientativo según la posición del club en su competición.</summary>
    private string ObjectiveFor(TeamDataDB team)
    {
        var rivals = _allTeams.Where(t => t.LeagueId == team.LeagueId && t.IsFilial == 0)
                              .OrderByDescending(t => t.Overall)
                              .ToList();
        int rank = rivals.FindIndex(t => t.Id == team.Id) + 1;

        return LocalizationService.T(SeasonObjective.KeyFor(team.LeagueId, rank, rivals.Count));
    }

    // ── Plantilla ─────────────────────────────────────────────────────────

    private void ShowSquadModal()
    {
        if (_selectedTeam == null || _squadOverlay == null) return;

        _playersByTeam.TryGetValue(_selectedTeam.Id, out var players);
        players ??= new List<PlayerDataDB>();
        players = players.OrderBy(p => PlayerPositionNames.LineGroup(p.MainPosition))
                         .ThenByDescending(p => p.Overall)
                         .ToList();

        if (_logoSprites.TryGetValue(_selectedTeam.Logo ?? _selectedTeam.Id.ToString(), out var sprite))
            _squadLogo.style.backgroundImage = new StyleBackground(sprite);

        SetText(_squadTeamName, _selectedTeam.Name.ToUpperInvariant());
        int avg = players.Count > 0 ? Mathf.RoundToInt((float)players.Average(p => p.Overall)) : 0;
        SetText(_squadAvg, avg.ToString());

        _squadLeft?.Clear();
        _squadRight?.Clear();
        for (int i = 0; i < players.Count; i++)
        {
            var row = BuildSquadRow(players[i]);
            if (i < (players.Count + 1) / 2) _squadLeft?.Add(row);
            else _squadRight?.Add(row);
        }

        _squadOverlay.style.display = DisplayStyle.Flex;
        _squadOverlay.AddToClassList("modal-overlay--visible");
        var squadBox = _root.Q<VisualElement>("SquadModalBox");
        if (squadBox != null)
        {
            squadBox.style.display = DisplayStyle.Flex;
            squadBox.AddToClassList("modal-box--visible");
        }
        CustomScrollbar.Reset(_squadBar);
    }

    private VisualElement BuildSquadRow(PlayerDataDB player)
    {
        var row = new VisualElement();
        row.AddToClassList("modal-player-row");

        var name = new Label($"{player.FirstName} {player.LastName}".Trim());
        name.AddToClassList("modal-player-name");
        row.Add(name);

        var pos = new Label(PlayerPositionNames.ShortName(player.MainPosition));
        pos.AddToClassList("modal-player-pos");
        row.Add(pos);

        var flag = new VisualElement();
        flag.AddToClassList("modal-player-flag");
        var flagSprite = FlagFor(player.Nationality);
        if (flagSprite != null) flag.style.backgroundImage = new StyleBackground(flagSprite);
        flag.tooltip = CountryCodes.GetNameEs(player.Nationality);
        row.Add(flag);

        var age = new Label(LocalizationService.T("ui.select_team.edad_anios", player.Age));
        age.AddToClassList("modal-player-age");
        row.Add(age);

        var ovr = new Label(player.Overall.ToString());
        ovr.AddToClassList("modal-player-avg");
        if (player.Overall >= 80) ovr.AddToClassList("modal-player-avg--high");
        else if (player.Overall >= 70) ovr.AddToClassList("modal-player-avg--mid");
        else ovr.AddToClassList("modal-player-avg--low");
        row.Add(ovr);

        return row;
    }

    /// <summary>Bandera de la nacionalidad (ISO3), con alias FIFA y fallback genérico.</summary>
    private Sprite FlagFor(string nationality)
    {
        string code = CountryCodes.NormalizeFlagCode(nationality);
        if (_flagSprites.TryGetValue(code, out var cached)) return cached;

        var sprite = Resources.Load<Sprite>(CountryCodes.GetFlagPath(code));
        if (sprite == null)
            sprite = Resources.Load<Sprite>(CountryCodes.GetFlagPath("default"));

        _flagSprites[code] = sprite;
        return sprite;
    }

    private void CloseSquadModal()
    {
        if (_squadOverlay == null) return;
        _squadOverlay.RemoveFromClassList("modal-overlay--visible");
        _squadOverlay.style.display = DisplayStyle.None;
        var squadBox = _root.Q<VisualElement>("SquadModalBox");
        if (squadBox != null)
        {
            squadBox.RemoveFromClassList("modal-box--visible");
            squadBox.style.display = DisplayStyle.None;
        }
    }

    // ── Continuar ─────────────────────────────────────────────────────────

    private void ValidateContinue()
    {
        bool hasTeam = _selectedTeam != null;
        bool hasName = _managerInput != null && !string.IsNullOrWhiteSpace(_managerInput.value);
        if (_btnContinue != null) _btnContinue.SetEnabled(hasTeam && hasName);
    }

    private void OnContinue()
    {
        if (_selectedTeam == null) return;

        string managerName = string.IsNullOrWhiteSpace(_managerInput?.value)
            ? "Manager" : _managerInput.value.Trim();

        var db = DatabaseManager.Instance;
        if (db == null || db.Db == null)
        {
            Debug.LogError("[SelectTeam] No hay base de datos abierta.");
            return;
        }

        db.ClearAllManagers();

        var mode = ScreenManager.Instance.CurrentMode;
        var manager = new ManagerDataDB
        {
            Name = managerName,
            TeamId = _selectedTeam.Id,
            LeagueId = _selectedTeam.LeagueId,
            GameMode = mode == GameMode.ProManager ? "promanager" : "manager",
            Trust = 50,
            Morale = 50,
            FanConfidence = 50
        };
        db.SaveManager(manager);

        // La partida NO se crea aquí: la temporada, el calendario y los metadatos
        // del slot se generan al pulsar CONTINUAR en Pretemporada. Si el jugador
        // abandona antes (VOLVER), no queda ninguna partida en Cargar partida.
        Debug.Log($"[SelectTeam] {managerName} → {_selectedTeam.Name} (pretemporada pendiente)");

        if (ScreenManager.Instance.IsRegistered(GameScreen.Preseason))
        {
            ScreenManager.Instance.GoTo(GameScreen.Preseason);
            return;
        }

        // Sin pantalla de Pretemporada no hay forma de completar el proceso.
        if (_detailObjective != null)
            _detailObjective.text = LocalizationService.T("ui.select_team.pretemporada_pendiente");
    }

    /// <summary>
    /// Borra la partida a medias (manager, temporada y partidos) sin escribir los
    /// metadatos del slot. Así no aparece en Cargar partida.
    /// </summary>
    private static void DiscardPendingGame()
    {
        DatabaseManager.Instance?.DiscardPendingGame();
    }
}
