using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// F6 — Pretemporada: elige hasta cuatro amistosos contra rivales de menor nivel
/// y confirma el arranque de la temporada. El calendario de liga ya se generó al
/// crear la partida (F6); aquí solo se guardan los amistosos elegidos.
/// </summary>
public class PreseasonController : UIScreenController
{
    public override GameScreen ScreenId => GameScreen.Preseason;

    private const int MaxGames = 4;

    // Header
    private Button _btnBack;
    private Button _btnContinue;
    private Label _headerYear;

    // Info bar
    private VisualElement _infoBarLogo;
    private Label _infoBarTeam;
    private Label _infoBarCount;
    private Label _infoBarSeason;

    // Slots
    private VisualElement _slotsRow;

    // Bottom
    private Button _btnHome;
    private Button _btnAway;
    private VisualElement _teamsGrid;

    // Filtros por competición
    private Button _filterLaliga;
    private Button _filterLaliga2;
    private Button _filterRfef;
    private Button _filterChampions;
    private Button _filterEuropa;
    private Button _filterConference;
    private int _filterLeagueId = PreseasonFilter.LaLiga;

    private const int FilterLaLiga = PreseasonFilter.LaLiga;
    private const int FilterLaLiga2 = PreseasonFilter.LaLiga2;
    private const int FilterRfef = PreseasonFilter.Rfef;
    private const int FilterChampions = PreseasonFilter.Champions;
    private const int FilterEuropa = PreseasonFilter.Europa;
    private const int FilterConference = PreseasonFilter.Conference;

    // Estado
    private List<TeamDataDB> _allTeams = new();
    private readonly List<GameDataDB> _games = new();
    private bool _isHome = true;

    private readonly Dictionary<string, Sprite> _logoSprites = new();
    private readonly Dictionary<string, Sprite> _logoSprites120 = new();
    private CustomScrollbar.Handle _teamsScrollBar;

    private string[] _dates = Array.Empty<string>();
    private string[] _datesDb = Array.Empty<string>();

    // ── PIPELINE ───────────────────────────────────────────────────────────

    protected override void CacheReferences()
    {
        _btnBack = _root.Q<Button>("BtnBack");
        _btnContinue = _root.Q<Button>("BtnContinue");
        _headerYear = _root.Q<Label>("HeaderYear");
        _infoBarLogo = _root.Q<VisualElement>("InfoBarLogo");
        _infoBarTeam = _root.Q<Label>("InfoBarTeam");
        _infoBarCount = _root.Q<Label>("InfoBarCount");
        _infoBarSeason = _root.Q<Label>("InfoBarSeason");
        _slotsRow = _root.Q<VisualElement>("SlotsRow");
        _btnHome = _root.Q<Button>("BtnHome");
        _btnAway = _root.Q<Button>("BtnAway");
        _teamsGrid = _root.Q<VisualElement>("TeamsGrid");

        _filterLaliga = _root.Q<Button>("FilterLaliga");
        _filterLaliga2 = _root.Q<Button>("FilterLaliga2");
        _filterRfef = _root.Q<Button>("FilterRfef");
        _filterChampions = _root.Q<Button>("FilterChampions");
        _filterEuropa = _root.Q<Button>("FilterEuropa");
        _filterConference = _root.Q<Button>("FilterConference");
    }

    protected override void LoadData()
    {
        base.LoadData();
        if (_myTeam == null || _season == null) return;

        LoadLogos();

        _allTeams = DatabaseManager.Instance.GetAllTeams();

        int yearStart = _season.YearStart;
        if (_headerYear != null) _headerYear.text = yearStart.ToString();
        if (_infoBarSeason != null) _infoBarSeason.text = LocalizationService.T("ui.preseason.temporada", yearStart);

        var preseasonDates = FootballCalendar.PreseasonDates(yearStart);
        _dates = new string[MaxGames];
        _datesDb = new string[MaxGames];
        for (int i = 0; i < MaxGames; i++)
        {
            _datesDb[i] = CalendarHelper.ToDb(preseasonDates[i]);
            _dates[i] = preseasonDates[i].ToString("dd MMM yyyy").ToUpperInvariant();
        }

        // La pretemporada empieza vacía: el usuario elige sus amistosos.
        _games.Clear();
    }

    protected override void RegisterCallbacks()
    {
        _btnBack?.RegisterCallback<ClickEvent>(_ => { PlayClick(); ScreenManager.Instance.GoTo(GameScreen.SelectTeam); });
        _btnContinue?.RegisterCallback<ClickEvent>(_ => { PlayClick(); OnContinue(); });
        _btnHome?.RegisterCallback<ClickEvent>(_ => { PlayClick(); SetLocation(true); });
        _btnAway?.RegisterCallback<ClickEvent>(_ => { PlayClick(); SetLocation(false); });

        _filterLaliga?.RegisterCallback<ClickEvent>(_ => { PlayClick(); SelectFilter(FilterLaLiga); });
        _filterLaliga2?.RegisterCallback<ClickEvent>(_ => { PlayClick(); SelectFilter(FilterLaLiga2); });
        _filterRfef?.RegisterCallback<ClickEvent>(_ => { PlayClick(); SelectFilter(FilterRfef); });
        _filterChampions?.RegisterCallback<ClickEvent>(_ => { PlayClick(); SelectFilter(FilterChampions); });
        _filterEuropa?.RegisterCallback<ClickEvent>(_ => { PlayClick(); SelectFilter(FilterEuropa); });
        _filterConference?.RegisterCallback<ClickEvent>(_ => { PlayClick(); SelectFilter(FilterConference); });

        RegisterHandCursor(_btnBack);
        RegisterHandCursor(_btnContinue);
        RegisterHandCursor(_btnHome);
        RegisterHandCursor(_btnAway);
        foreach (var filter in new[] { _filterLaliga, _filterLaliga2, _filterRfef,
                                       _filterChampions, _filterEuropa, _filterConference })
            RegisterHandCursor(filter);
    }

    protected override void Refresh()
    {
        if (_myTeam == null) return;

        SetLocation(true);
        RefreshSlots();
        SelectFilter(FilterLaLiga);
        UpdateInfoBar();
    }

    // ── LOGOS ──────────────────────────────────────────────────────────────

    private void LoadLogos()
    {
        foreach (var sprite in Resources.LoadAll<Sprite>("Teams/Logos/64x64"))
            _logoSprites[sprite.name] = sprite;
        foreach (var sprite in Resources.LoadAll<Sprite>("Teams/Logos/120x120"))
            _logoSprites120[sprite.name] = sprite;
    }

    private static string LogoKey(TeamDataDB team) => team.Logo ?? team.Id.ToString();

    // ── INFO BAR ───────────────────────────────────────────────────────────

    private void UpdateInfoBar()
    {
        if (_myTeam == null) return;

        if (_infoBarLogo != null && _logoSprites.TryGetValue(LogoKey(_myTeam), out var sprite))
            _infoBarLogo.style.backgroundImage = new StyleBackground(sprite);
        if (_infoBarTeam != null) _infoBarTeam.text = _myTeam.Name.ToUpperInvariant();
        if (_infoBarCount != null) _infoBarCount.text = $"{_games.Count} / {MaxGames}";
    }

    // ── LOCAL / VISITANTE ──────────────────────────────────────────────────

    private void SetLocation(bool isHome)
    {
        _isHome = isHome;
        if (isHome)
        {
            _btnHome?.AddToClassList("toggle-btn--active");
            _btnAway?.RemoveFromClassList("toggle-btn--active");
        }
        else
        {
            _btnAway?.AddToClassList("toggle-btn--active");
            _btnHome?.RemoveFromClassList("toggle-btn--active");
        }
    }

    // ── SLOTS ──────────────────────────────────────────────────────────────

    private void RefreshSlots()
    {
        if (CursorManager.Instance != null) CursorManager.Instance.SetDefaultCursor();
        _slotsRow?.Clear();

        for (int i = 0; i < MaxGames; i++)
        {
            bool filled = i < _games.Count;
            _slotsRow?.Add(filled ? CreateFilledSlot(i, _games[i]) : CreateEmptySlot(i));
        }
    }

    private VisualElement CreateEmptySlot(int index)
    {
        var slot = new VisualElement();
        slot.AddToClassList("game-slot");
        slot.AddToClassList("game-slot--empty");

        var num = new Label { text = LocalizationService.T("ui.preseason.partido", index + 1) };
        num.AddToClassList("game-slot-number");

        var empty = new Label { text = LocalizationService.T("ui.preseason.sin_rival") };
        empty.AddToClassList("game-slot-empty-text");

        slot.Add(num);
        slot.Add(empty);
        return slot;
    }

    private VisualElement CreateFilledSlot(int index, GameDataDB game)
    {
        int rivalId = game.HomeTeamId == _myTeam.Id ? game.AwayTeamId : game.HomeTeamId;
        var rival = _allTeams.FirstOrDefault(t => t.Id == rivalId);

        var slot = new VisualElement();
        slot.AddToClassList("game-slot");
        slot.AddToClassList("game-slot--filled");

        var num = new Label { text = LocalizationService.T("ui.preseason.partido", index + 1) };
        num.AddToClassList("game-slot-number");
        num.AddToClassList("game-slot-number--filled");

        var body = new VisualElement();
        body.AddToClassList("game-slot-body");

        // Columna izquierda: escudo + nombre
        var leftCol = new VisualElement();
        leftCol.AddToClassList("game-slot-left");

        var logo = new VisualElement();
        logo.AddToClassList("game-slot-logo");
        if (rival != null && _logoSprites120.TryGetValue(LogoKey(rival), out var sprite))
            logo.style.backgroundImage = new StyleBackground(sprite);

        var name = new Label { text = rival?.Name?.ToUpperInvariant() ?? "???" };
        name.AddToClassList("game-slot-rival");

        leftCol.Add(logo);
        leftCol.Add(name);

        // Columna derecha: fecha, local/visitante y borrar
        var rightCol = new VisualElement();
        rightCol.AddToClassList("game-slot-right");

        var date = new Label { text = index < _dates.Length ? _dates[index] : "" };
        date.AddToClassList("game-slot-date");

        bool isHome = game.HomeTeamId == _myTeam.Id;
        var location = new Label
        {
            text = LocalizationService.T(isHome ? "ui.preseason.local" : "ui.preseason.visitante")
        };
        location.AddToClassList("game-slot-location");
        location.AddToClassList(isHome ? "game-slot-location--home" : "game-slot-location--away");

        var btnRemove = new Button { text = LocalizationService.T("ui.preseason.borrar") };
        btnRemove.AddToClassList("btn-remove-slot");
        int captured = index;
        btnRemove.clicked += () => { PlayClick(); RemoveGame(captured); };
        RegisterHandCursor(btnRemove);

        rightCol.Add(date);
        rightCol.Add(location);
        rightCol.Add(btnRemove);

        body.Add(leftCol);
        body.Add(rightCol);

        slot.Add(num);
        slot.Add(body);
        return slot;
    }

    // ── GRID DE EQUIPOS ────────────────────────────────────────────────────

    /// <summary>Activa un filtro de competición y reconstruye la rejilla.</summary>
    private void SelectFilter(int leagueId)
    {
        _filterLeagueId = leagueId;

        SetFilterActive(_filterLaliga, leagueId == FilterLaLiga);
        SetFilterActive(_filterLaliga2, leagueId == FilterLaLiga2);
        SetFilterActive(_filterRfef, leagueId == FilterRfef);
        SetFilterActive(_filterChampions, leagueId == FilterChampions);
        SetFilterActive(_filterEuropa, leagueId == FilterEuropa);
        SetFilterActive(_filterConference, leagueId == FilterConference);

        BuildTeamsGrid(leagueId);
        UpdateTeamsGridAvailability();
    }

    private static void SetFilterActive(Button button, bool active)
    {
        if (button == null) return;
        if (active) button.AddToClassList("filter-btn--active");
        else button.RemoveFromClassList("filter-btn--active");
    }

    private void BuildTeamsGrid(int leagueId)
    {
        if (_teamsGrid == null || _myTeam == null) return;
        _teamsGrid.Clear();

        var viewport = _root.Q<VisualElement>("TeamsViewport");
        if (viewport != null && (_teamsScrollBar == null || _teamsScrollBar.Viewport != viewport))
            _teamsScrollBar = CustomScrollbar.Bind(viewport, _teamsGrid,
                _root.Q<VisualElement>("TeamsScrollTrack"), _root.Q<VisualElement>("TeamsScrollThumb"));

        var teams = _allTeams
            .Where(t => t != null
                        && t.Id != _myTeam.Id
                        && t.LeagueId > 0
                        && PreseasonFilter.Matches(t.LeagueId, leagueId))
            .OrderByDescending(t => t.Overall)
            .ToList();

        foreach (var team in teams)
            _teamsGrid.Add(CreateTeamItem(team));
    }

    private VisualElement CreateTeamItem(TeamDataDB team)
    {
        var item = new VisualElement();
        item.AddToClassList("team-item");
        item.name = $"team_{team.Id}";

        var logo = new VisualElement();
        logo.AddToClassList("team-logo");
        if (_logoSprites.TryGetValue(LogoKey(team), out var sprite))
            logo.style.backgroundImage = new StyleBackground(sprite);

        var overall = new Label { text = $"MED {team.Overall}" };
        overall.AddToClassList("team-overall");

        item.Add(logo);
        item.Add(overall);

        item.RegisterCallback<ClickEvent>(_ => { PlayClick(); OnTeamSelected(team); });
        RegisterHandCursor(item);
        return item;
    }

    private void UpdateTeamsGridAvailability()
    {
        if (_teamsGrid == null || _myTeam == null) return;

        var usedIds = _games
            .Select(g => g.HomeTeamId == _myTeam.Id ? g.AwayTeamId : g.HomeTeamId)
            .ToHashSet();

        foreach (var team in _allTeams)
        {
            var item = _teamsGrid.Q<VisualElement>($"team_{team.Id}");
            if (item == null) continue;

            if (usedIds.Contains(team.Id) || _games.Count >= MaxGames)
                item.AddToClassList("team-item--used");
            else
                item.RemoveFromClassList("team-item--used");
        }
    }

    // ── ACCIONES ───────────────────────────────────────────────────────────

    private void OnTeamSelected(TeamDataDB rival)
    {
        if (rival == null || _myTeam == null || _games.Count >= MaxGames) return;

        var usedIds = _games
            .Select(g => g.HomeTeamId == _myTeam.Id ? g.AwayTeamId : g.HomeTeamId)
            .ToHashSet();
        if (usedIds.Contains(rival.Id)) return;

        int index = _games.Count;
        var game = new GameDataDB
        {
            SeasonId = _season.Id,
            ManagerId = _manager?.Id ?? 0,
            MatchDay = -(MaxGames - index),
            MatchDate = _datesDb[index],
            HomeTeamId = _isHome ? _myTeam.Id : rival.Id,
            AwayTeamId = _isHome ? rival.Id : _myTeam.Id,
            IsPlayed = 0,
            GameType = DatabaseManager.GameTypePreseason,
            CompetitionId = 0
        };

        _games.Add(game);
        RefreshSlots();
        UpdateTeamsGridAvailability();
        UpdateInfoBar();
    }

    private void RemoveGame(int index)
    {
        if (index < 0 || index >= _games.Count) return;
        _games.RemoveAt(index);
        ReindexGames();
        RefreshSlots();
        UpdateTeamsGridAvailability();
        UpdateInfoBar();
    }

    /// <summary>Reasigna fechas y números de jornada tras quitar un amistoso.</summary>
    private void ReindexGames()
    {
        for (int i = 0; i < _games.Count; i++)
        {
            _games[i].MatchDate = _datesDb[i];
            _games[i].MatchDay = -(_games.Count - i);
        }
    }

    // ── CONTINUAR ──────────────────────────────────────────────────────────

    private void OnContinue()
    {
        if (_season == null || _myTeam == null) return;

        var db = DatabaseManager.Instance;
        ReindexGames();
        db.ReplacePreseasonGames(_season, _games);

        // La temporada arranca en el primer amistoso (o en la jornada 1 si no hay).
        var firstLeague = db.GetFirstLeagueGame(_season.Id);
        string startDate = _games.Count > 0
            ? _games[0].MatchDate
            : firstLeague?.MatchDate ?? CalendarHelper.ToDb(CalendarHelper.SeasonStart(_season.YearStart));

        _season.Phase = "preseason";
        _season.CurrentDate = startDate;
        _season.CurrentMatchDay = CalendarHelper.DateToGameDay(_season.YearStart, ParseDate(startDate));
        db.UpdateSeason(_season);

        GameSaveManager.UpdateSlotFromDatabase(db.ActiveSaveSlot);

        Debug.Log($"[Preseason] {_games.Count} amistosos guardados. Temporada {_season.YearStart}-{_season.YearEnd} lista.");

        if (ScreenManager.Instance.IsRegistered(GameScreen.Dashboard))
        {
            ScreenManager.Instance.GoTo(GameScreen.Dashboard);
            return;
        }

        // F7 aún no tiene Dashboard: la partida queda creada y guardada.
        if (_infoBarSeason != null)
            _infoBarSeason.text = LocalizationService.T("ui.preseason.partida_lista");
    }

    private static DateTime ParseDate(string dbDate)
        => CalendarHelper.TryParse(dbDate, out var date)
            ? date
            : CalendarHelper.SeasonStart(DatabaseManager.CurrentSeasonStartYear());
}
