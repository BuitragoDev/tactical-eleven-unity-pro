using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// F7 — Dashboard: hub del juego con header (club, fecha, presupuesto, manager),
/// sidebar de navegación, tarjetas de resumen y avance de día.
///
/// El motor de partido llega en F8: si el día tiene partido, el avance se detiene
/// y el botón invita a jugarlo (todavía sin simulación).
/// </summary>
public class DashboardController : UIScreenController
{
    public override GameScreen ScreenId => GameScreen.Dashboard;

    // ── Header ─────────────────────────────────────────────────────────────
    private VisualElement _headerLogo;
    private Label _headerTeam;
    private Label _headerLeague;
    private Label _headerDate;
    private Label _headerBudget;
    private Label _headerManager;
    private Button _btnSettings;

    // ── Sidebar ────────────────────────────────────────────────────────────
    private VisualElement _sidebarViewport;
    private VisualElement _sidebarContent;
    private CustomScrollbar.Handle _sidebarScrollbar;
    private readonly Dictionary<string, GameScreen> _navTargets = new();

    // ── Próximo partido ────────────────────────────────────────────────────
    private VisualElement _nextBody;
    private VisualElement _nextLogo;
    private Label _nextRival;
    private Label _nextVenue;
    private Label _nextDate;
    private Label _nextComp;
    private Label _nextEmpty;

    // ── Último resultado ───────────────────────────────────────────────────
    private Label _lastScore;
    private Label _lastRival;
    private Label _lastDate;
    private Label _lastEmpty;

    // ── Clasificación ──────────────────────────────────────────────────────
    private VisualElement _standingsBody;

    // ── Objetivo y finanzas ────────────────────────────────────────────────
    private Label _objectiveValue;
    private Label _objectiveHint;
    private Label _financeBudget;
    private Label _financeWage;

    // ── Plantilla y noticias ───────────────────────────────────────────────
    private Label _squadCount;
    private Label _squadAverage;
    private Label _squadMorale;
    private Label _squadInjured;
    private Label _newsText;

    // ── Avance de día ──────────────────────────────────────────────────────
    private Label _dayStatus;
    private Button _btnAdvance;
    private Label _toast;
    private IVisualElementScheduledItem _toastTimer;

    // ── Datos ──────────────────────────────────────────────────────────────
    private LeagueDataDB _league;
    private List<TeamDataDB> _teams = new();
    private readonly Dictionary<int, TeamDataDB> _teamsById = new();
    private List<GameDataDB> _teamGames = new();
    private List<GameDataDB> _regularGames = new();
    private List<PlayerDataDB> _squad = new();
    private GameDataDB _matchToday;
    private GameDataDB _nextGame;

    private static Dictionary<string, Sprite> _logoCache;

    // ── PIPELINE ───────────────────────────────────────────────────────────

    protected override void CacheReferences()
    {
        _headerLogo = Q<VisualElement>("HeaderLogo");
        _headerTeam = Q<Label>("HeaderTeam");
        _headerLeague = Q<Label>("HeaderLeague");
        _headerDate = Q<Label>("HeaderDate");
        _headerBudget = Q<Label>("HeaderBudget");
        _headerManager = Q<Label>("HeaderManager");
        _btnSettings = Q<Button>("BtnSettings");

        _sidebarViewport = Q<VisualElement>("SidebarViewport");
        _sidebarContent = Q<VisualElement>("SidebarContent");

        _nextBody = Q<VisualElement>("NextBody");
        _nextLogo = Q<VisualElement>("NextLogo");
        _nextRival = Q<Label>("NextRival");
        _nextVenue = Q<Label>("NextVenue");
        _nextDate = Q<Label>("NextDate");
        _nextComp = Q<Label>("NextComp");
        _nextEmpty = Q<Label>("NextEmpty");

        _lastScore = Q<Label>("LastScore");
        _lastRival = Q<Label>("LastRival");
        _lastDate = Q<Label>("LastDate");
        _lastEmpty = Q<Label>("LastEmpty");

        _standingsBody = Q<VisualElement>("StandingsBody");

        _objectiveValue = Q<Label>("ObjectiveValue");
        _objectiveHint = Q<Label>("ObjectiveHint");
        _financeBudget = Q<Label>("FinanceBudget");
        _financeWage = Q<Label>("FinanceWage");

        _squadCount = Q<Label>("SquadCount");
        _squadAverage = Q<Label>("SquadAverage");
        _squadMorale = Q<Label>("SquadMorale");
        _squadInjured = Q<Label>("SquadInjured");
        _newsText = Q<Label>("NewsText");

        _dayStatus = Q<Label>("DayStatus");
        _btnAdvance = Q<Button>("BtnAdvance");
        _toast = Q<Label>("DashToast");

        if (_sidebarViewport != null && _sidebarContent != null)
            _sidebarScrollbar = CustomScrollbar.Bind(_sidebarViewport, _sidebarContent,
                Q<VisualElement>("SidebarScrollTrack"), Q<VisualElement>("SidebarScrollThumb"));
    }

    protected override void LoadData()
    {
        base.LoadData();
        if (_myTeam == null || _season == null) return;

        var db = DatabaseManager.Instance;
        _league = db.GetLeagueById(_myTeam.LeagueId);

        _teams = db.GetTeamsByLeague(_myTeam.LeagueId)
                    .Where(t => t.IsFilial == 0)
                    .ToList();

        _teamsById.Clear();
        foreach (var team in db.GetAllTeams())
            _teamsById[team.Id] = team;

        _squad = db.GetPlayersByTeam(_myTeam.Id);

        ReloadGames();
    }

    private void ReloadGames()
    {
        var db = DatabaseManager.Instance;
        _teamGames = db.GetGamesForTeam(_season.Id, _myTeam.Id);
        _regularGames = db.GetGamesByType(_season.Id, DatabaseManager.GameTypeRegular);
        _matchToday = GameDayRules.FindMatch(_teamGames, _myTeam.Id, _season.CurrentDate);
        _nextGame = db.GetNextGame(_season.Id, _myTeam.Id);
    }

    protected override void RegisterCallbacks()
    {
        _btnSettings?.RegisterCallback<ClickEvent>(_ => { PlayClick(); OpenConfigModal(); });
        _btnAdvance?.RegisterCallback<ClickEvent>(_ => { PlayClick(); OnAdvanceDay(); });

        BuildNavTargets();
        foreach (var pair in _navTargets)
        {
            var button = Q<Button>(pair.Key);
            if (button == null) continue;
            var target = pair.Value;
            button.RegisterCallback<ClickEvent>(_ => { PlayClick(); Navigate(target); });
            RegisterHandCursor(button);
        }

        RegisterHandCursor(_btnSettings);
        RegisterHandCursor(_btnAdvance);
    }

    protected override void Refresh()
    {
        if (_myTeam == null || _season == null) return;

        RefreshHeader();
        RefreshNextMatch();
        RefreshLastResult();
        RefreshStandings();
        RefreshObjective();
        RefreshFinances();
        RefreshSquad();
        RefreshNews();
        RefreshDayBar();
    }

    // ── HEADER ─────────────────────────────────────────────────────────────

    private void RefreshHeader()
    {
        if (_headerLogo != null && Logos().TryGetValue(LogoKey(_myTeam), out var logo))
            _headerLogo.style.backgroundImage = new StyleBackground(logo);

        if (_headerTeam != null) _headerTeam.text = _myTeam.Name;
        if (_headerLeague != null) _headerLeague.text = _league?.Name ?? "";
        if (_headerDate != null) _headerDate.text = GameDayRules.FormatLongDate(_season.CurrentDate);
        if (_headerBudget != null) _headerBudget.text = $"{_myTeam.TransferBudget:N0} €";
        if (_headerManager != null) _headerManager.text = _manager?.Name ?? "-";
    }

    // ── PRÓXIMO PARTIDO ────────────────────────────────────────────────────

    private void RefreshNextMatch()
    {
        SetVisible(_nextBody, _nextGame != null);
        SetVisible(_nextEmpty, _nextGame == null);
        SetVisible(_nextLogo, _nextGame != null);

        if (_nextGame == null)
        {
            if (_nextRival != null) _nextRival.text = "";
            if (_nextVenue != null) _nextVenue.text = "";
            if (_nextDate != null) _nextDate.text = "";
            if (_nextComp != null) _nextComp.text = "";
            return;
        }

        bool home = _nextGame.HomeTeamId == _myTeam.Id;
        int rivalId = home ? _nextGame.AwayTeamId : _nextGame.HomeTeamId;
        var rival = TeamOf(rivalId);

        if (_nextLogo != null && rival != null && Logos().TryGetValue(LogoKey(rival), out var sprite))
            _nextLogo.style.backgroundImage = new StyleBackground(sprite);

        if (_nextRival != null) _nextRival.text = rival?.Name ?? "";
        if (_nextVenue != null)
            _nextVenue.text = LocalizationService.T(home
                ? "ui.dashboard.local"
                : "ui.dashboard.visitante");
        if (_nextDate != null) _nextDate.text = GameDayRules.FormatLongDate(_nextGame.MatchDate);
        if (_nextComp != null) _nextComp.text = CompetitionLabel(_nextGame);
    }

    // ── ÚLTIMO RESULTADO ───────────────────────────────────────────────────

    private void RefreshLastResult()
    {
        var last = DatabaseManager.Instance.GetLastPlayedGame(_season.Id, _myTeam.Id);

        SetVisible(_lastScore, last != null);
        SetVisible(_lastRival, last != null);
        SetVisible(_lastDate, last != null);
        SetVisible(_lastEmpty, last == null);

        if (last == null) return;

        bool home = last.HomeTeamId == _myTeam.Id;
        int rivalId = home ? last.AwayTeamId : last.HomeTeamId;
        var rival = TeamOf(rivalId);

        int mine = home ? last.HomeScore : last.AwayScore;
        int theirs = home ? last.AwayScore : last.HomeScore;

        if (_lastScore != null) _lastScore.text = $"{mine} - {theirs}";
        if (_lastRival != null) _lastRival.text = rival?.Name ?? "";
        if (_lastDate != null) _lastDate.text = GameDayRules.FormatShortDate(last.MatchDate);
    }

    // ── CLASIFICACIÓN ──────────────────────────────────────────────────────

    private void RefreshStandings()
    {
        if (_standingsBody == null) return;
        _standingsBody.Clear();

        var table = LeagueTable.Build(_teams.Select(t => t.Id), _regularGames);
        if (table.Count == 0) return;

        int myPosition = LeagueTable.PositionOf(table, _myTeam.Id);
        int shown = 0;

        for (int i = 0; i < table.Count && shown < 6; i++, shown++)
            _standingsBody.Add(BuildTableRow(i + 1, table[i]));

        // Si el club queda fuera del top 6, se añade separado para no perder el contexto.
        if (myPosition > 6)
        {
            var separator = new Label("···");
            separator.AddToClassList("dash-cell");
            separator.AddToClassList("dash-cell--pos");
            _standingsBody.Add(separator);
            _standingsBody.Add(BuildTableRow(myPosition, table[myPosition - 1]));
        }
    }

    private VisualElement BuildTableRow(int position, LeagueTableRow row)
    {
        var line = new VisualElement();
        line.AddToClassList("dash-table-row");
        if (row.TeamId == _myTeam.Id) line.AddToClassList("dash-table-row--me");

        var team = TeamOf(row.TeamId);
        line.Add(Cell(position.ToString(), "dash-cell", "dash-cell--pos"));
        line.Add(Cell(team?.ShortName ?? row.TeamId.ToString(), "dash-cell", "dash-cell--team"));
        line.Add(Cell(row.Played.ToString(), "dash-cell", "dash-cell--num"));
        line.Add(Cell(row.GoalDifference > 0 ? $"+{row.GoalDifference}" : row.GoalDifference.ToString(),
            "dash-cell", "dash-cell--num"));
        line.Add(Cell(row.Points.ToString(), "dash-cell", "dash-cell--num"));
        return line;
    }

    private static Label Cell(string text, params string[] classes)
    {
        var label = new Label(text);
        foreach (var css in classes) label.AddToClassList(css);
        return label;
    }

    // ── OBJETIVO Y FINANZAS ────────────────────────────────────────────────

    private void RefreshObjective()
    {
        int total = _teams.Count;
        if (total == 0) return;

        bool hasResults = _regularGames.Any(g => g.IsPlayed == 1);
        int position = hasResults
            ? LeagueTable.PositionOf(LeagueTable.Build(_teams.Select(t => t.Id), _regularGames), _myTeam.Id)
            : OverallRank();

        if (position <= 0) position = total;

        if (_objectiveValue != null)
            _objectiveValue.text = LocalizationService.T(
                SeasonObjective.KeyFor(_myTeam.LeagueId, position, total));

        if (_objectiveHint != null)
            _objectiveHint.text = LocalizationService.T("ui.dashboard.objetivo_hint", position, total);
    }

    /// <summary>Puesto por media de plantilla (pretemporada, sin resultados).</summary>
    private int OverallRank()
    {
        var ordered = _teams.OrderByDescending(t => t.Overall).ThenBy(t => t.Id).ToList();
        int index = ordered.FindIndex(t => t.Id == _myTeam.Id);
        return index >= 0 ? index + 1 : 0;
    }

    private void RefreshFinances()
    {
        if (_financeBudget != null) _financeBudget.text = $"{_myTeam.TransferBudget:N0} €";
        if (_financeWage != null) _financeWage.text = $"{_myTeam.TotalWageBill:N0} €";
    }

    // ── PLANTILLA Y NOTICIAS ───────────────────────────────────────────────

    private void RefreshSquad()
    {
        int count = _squad?.Count ?? 0;
        if (_squadCount != null) _squadCount.text = count.ToString();

        if (_squadAverage != null)
            _squadAverage.text = count > 0 ? Mathf.RoundToInt((float)_squad.Average(p => p.Overall)).ToString() : "-";

        if (_squadMorale != null)
            _squadMorale.text = count > 0 ? Mathf.RoundToInt((float)_squad.Average(p => p.Morale)).ToString() : "-";

        if (_squadInjured != null)
            _squadInjured.text = _squad.Count(p => p.InjuryDays > 0).ToString();
    }

    private void RefreshNews()
    {
        if (_newsText == null) return;

        if (_nextGame != null)
        {
            bool home = _nextGame.HomeTeamId == _myTeam.Id;
            var rival = TeamOf(home ? _nextGame.AwayTeamId : _nextGame.HomeTeamId);
            _newsText.text = LocalizationService.T("ui.dashboard.noticia_proximo",
                rival?.Name ?? "", GameDayRules.FormatShortDate(_nextGame.MatchDate));
            return;
        }

        _newsText.text = LocalizationService.T("ui.dashboard.noticia_sin_partido");
    }

    // ── AVANCE DE DÍA ──────────────────────────────────────────────────────

    private void RefreshDayBar()
    {
        if (_dayStatus != null)
            _dayStatus.text = GameDayRules.FormatLongDate(_season.CurrentDate);

        if (_btnAdvance == null) return;

        if (_matchToday != null)
        {
            _btnAdvance.text = LocalizationService.T("ui.dashboard.jugar_partido");
            _btnAdvance.AddToClassList("dash-advance--match");
        }
        else
        {
            _btnAdvance.text = LocalizationService.T("ui.dashboard.avanzar_dia");
            _btnAdvance.RemoveFromClassList("dash-advance--match");
        }
    }

    private void OnAdvanceDay()
    {
        if (ScreenManager.NavigationLocked) return;

        if (_matchToday != null)
        {
            ShowToast(LocalizationService.T("ui.dashboard.motor_f8"));
            return;
        }

        var db = DatabaseManager.Instance;
        ScreenManager.NavigationLocked = true;
        try
        {
            var result = db.AdvanceDay(_season, _myTeam.Id, out _);
            GameSaveManager.UpdateSlotFromDatabase(db.ActiveSaveSlot);
            ReloadGames();
            Refresh();

            if (result == DayAdvanceResult.SeasonOver)
                ShowToast(LocalizationService.T("ui.dashboard.temporada_fin"));
            else if (result == DayAdvanceResult.MatchDay)
                ShowToast(LocalizationService.T("ui.dashboard.dia_partido"));
        }
        finally
        {
            ScreenManager.NavigationLocked = false;
        }
    }

    // ── SIDEBAR ────────────────────────────────────────────────────────────

    private void BuildNavTargets()
    {
        if (_navTargets.Count > 0) return;

        _navTargets["NavDashboard"] = GameScreen.Dashboard;
        _navTargets["NavRoster"] = GameScreen.Roster;
        _navTargets["NavLineup"] = GameScreen.Lineup;
        _navTargets["NavTactics"] = GameScreen.Tactics;
        _navTargets["NavTraining"] = GameScreen.Training;
        _navTargets["NavStaff"] = GameScreen.Staff;
        _navTargets["NavInjured"] = GameScreen.Injured;
        _navTargets["NavDorsales"] = GameScreen.Dorsales;
        _navTargets["NavCalendar"] = GameScreen.Calendar;
        _navTargets["NavResults"] = GameScreen.Results;
        _navTargets["NavStandings"] = GameScreen.Standings;
        _navTargets["NavCompetition"] = GameScreen.CompetitionInfo;
        _navTargets["NavFilial"] = GameScreen.Filial;
        _navTargets["NavRivalidades"] = GameScreen.Rivalidades;
        _navTargets["NavStats"] = GameScreen.Stats;
        _navTargets["NavPalmares"] = GameScreen.Palmares;
        _navTargets["NavRecords"] = GameScreen.Records;
        _navTargets["NavPremios"] = GameScreen.Premios;
        _navTargets["NavKnockouts"] = GameScreen.Knockouts;
        _navTargets["NavMarket"] = GameScreen.Market;
        _navTargets["NavCartera"] = GameScreen.Cartera;
        _navTargets["NavBuscador"] = GameScreen.Buscador;
        _navTargets["NavHistorial"] = GameScreen.Historial;
        _navTargets["NavFinanzas"] = GameScreen.Finanzas;
        _navTargets["NavLoans"] = GameScreen.Loans;
        _navTargets["NavSponsors"] = GameScreen.Sponsors;
        _navTargets["NavTV"] = GameScreen.TV;
        _navTargets["NavCity"] = GameScreen.City;
        _navTargets["NavStadium"] = GameScreen.Stadium;
        _navTargets["NavManager"] = GameScreen.Manager;
        _navTargets["NavLogros"] = GameScreen.Logros;
        _navTargets["NavMessages"] = GameScreen.Messages;
        _navTargets["NavHilos"] = GameScreen.Hilos;
    }

    private void Navigate(GameScreen target)
    {
        if (target == ScreenId) return;

        var manager = ScreenManager.Instance;
        if (manager != null && manager.IsRegistered(target))
        {
            manager.GoTo(target);
            return;
        }

        ShowToast(LocalizationService.T("ui.dashboard.proximamente"));
    }

    // ── UTILIDADES ─────────────────────────────────────────────────────────

    private TeamDataDB TeamOf(int teamId)
        => _teamsById.TryGetValue(teamId, out var team) ? team : null;

    private string CompetitionLabel(GameDataDB game)
    {
        if (game.GameType == DatabaseManager.GameTypePreseason)
            return LocalizationService.T("ui.dashboard.amistoso");

        string competition = _league?.Name ?? "";
        return game.MatchDay > 0
            ? $"{competition} · {LocalizationService.T("ui.dashboard.jornada", game.MatchDay)}"
            : competition;
    }

    private static string LogoKey(TeamDataDB team)
        => string.IsNullOrEmpty(team.Logo) ? team.Id.ToString() : team.Logo;

    private static Dictionary<string, Sprite> Logos()
    {
        if (_logoCache != null) return _logoCache;

        _logoCache = new Dictionary<string, Sprite>();
        foreach (var sprite in Resources.LoadAll<Sprite>("Teams/Logos/64x64"))
            _logoCache[sprite.name] = sprite;
        return _logoCache;
    }

    private void ShowToast(string message)
    {
        if (_toast == null) return;

        _toast.text = message;
        _toast.style.display = DisplayStyle.Flex;

        _toastTimer?.Pause();
        _toastTimer = _toast.schedule
            .Execute(() => _toast.style.display = DisplayStyle.None)
            .StartingIn(2400);
    }
}
