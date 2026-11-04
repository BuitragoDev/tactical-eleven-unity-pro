using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// F7 — Dashboard (réplica de la pantalla de Tactical5 adaptada al fútbol).
///
/// Header con presupuesto, masa salarial, reputación y química; paneles de último
/// y próximo partido, estadísticas del equipo, clasificación, estadísticas de
/// jugadores (goles, asistencias, valoración, MVP, partidos y amarillas),
/// relaciones y noticias. El sidebar se inyecta en runtime.
/// </summary>
public class DashboardController : UIScreenController
{
    public override GameScreen ScreenId => GameScreen.Dashboard;

    // ── Header ─────────────────────────────────────────────────────────────
    private VisualElement _headerTeamLogo;
    private Label _headerTeamName;
    private Label _headerManagerName;
    private Label _headerBudget;
    private Label _headerChemistry;
    private Label _headerSeason;
    private Label _headerDate;
    private Button _btnAction;
    private VisualElement _configIcon;
    private VisualElement _loadingSpinner;
    private VisualElement _matchTransitionOverlay;

    // ── Último partido ─────────────────────────────────────────────────────
    private Label _noLastGame;
    private VisualElement _lastGameResult;
    private VisualElement _lastHomeLog;
    private VisualElement _lastAwayLog;
    private Label _lastHomeName;
    private Label _lastHomeRecord;
    private Label _lastAwayName;
    private Label _lastAwayRecord;
    private Label _lastHomeScore;
    private Label _lastAwayScore;
    private Label _lastResultBadge;
    private Label _lastGameDate;
    private Label _lastGameArena;
    private VisualElement _lastGameMeta;

    // ── Próximo partido ────────────────────────────────────────────────────
    private Label _noNextGame;
    private VisualElement _nextGameResult;
    private VisualElement _nextHomeLog;
    private VisualElement _nextAwayLog;
    private Label _nextHomeName;
    private Label _nextHomeRecord;
    private Label _nextAwayName;
    private Label _nextAwayRecord;
    private Label _nextGameDate;
    private Label _nextGameArena;
    private VisualElement _nextGameMeta;

    // ── Clasificación ──────────────────────────────────────────────────────
    private VisualElement _standingsBody;
    private Label _standingsLeague;

    // ── Estadísticas del equipo ────────────────────────────────────────────
    private Label _teamObjective;
    private VisualElement _teamObjectiveStatus;
    private VisualElement _teamStatsLogo;
    private Label _teamOverallRingVal;
    private VisualElement _teamChemistryRing;
    private Label _teamChemistryRingVal;
    private VisualElement _teamChemistryIcon;
    private Label _teamArenaName;
    private Label _teamArenaCapacity;
    private VisualElement _teamReputationStars;

    // ── Relaciones ─────────────────────────────────────────────────────────
    private VisualElement _circleTrust;
    private VisualElement _circleMorale;
    private VisualElement _circleFanConfidence;
    private Label _valTrust;
    private Label _valMorale;
    private Label _valFanConfidence;

    // ── Noticias ───────────────────────────────────────────────────────────
    private VisualElement _messagesBody;

    // ── Modales ────────────────────────────────────────────────────────────
    private VisualElement _mainMenuConfirmOverlay;
    private VisualElement _mainMenuConfirmBox;
    private VisualElement _exitConfirmOverlay;
    private VisualElement _exitConfirmBox;

    // ── Datos ──────────────────────────────────────────────────────────────
    private LeagueDataDB _league;
    private List<TeamDataDB> _teams = new();
    private readonly Dictionary<int, TeamDataDB> _teamsById = new();
    private List<PlayerDataDB> _players = new();
    private List<GameDataDB> _regularGames = new();
    private List<GameDataDB> _teamGames = new();
    private GameDataDB _matchToday;
    private GameDataDB _nextGame;
    private GameDataDB _lastGame;
    private List<PlayerSeasonStatsDB> _seasonStats = new();

    private Label _toast;
    private IVisualElementScheduledItem _toastTimer;

    private static Dictionary<string, Sprite> _logoCache;

    // ── PIPELINE ───────────────────────────────────────────────────────────

    protected override void CacheReferences()
    {
        _headerTeamLogo = Q<VisualElement>("HeaderTeamLogo");
        _headerTeamName = Q<Label>("HeaderTeamName");
        _headerManagerName = Q<Label>("HeaderManagerName");
        _headerBudget = Q<Label>("HeaderBudget");
        _headerChemistry = Q<Label>("HeaderChemistry");
        _headerSeason = Q<Label>("HeaderSeason");
        _headerDate = Q<Label>("HeaderDate");
        _btnAction = Q<Button>("BtnAction");
        _configIcon = Q<VisualElement>("ConfigIcon");

        // El spinner solo se muestra durante operaciones largas (F8+).
        _loadingSpinner = Q<VisualElement>("LoadingSpinner");
        if (_loadingSpinner != null) _loadingSpinner.style.display = DisplayStyle.None;

        _matchTransitionOverlay = Q<VisualElement>("MatchTransitionOverlay");
        if (_matchTransitionOverlay != null) _matchTransitionOverlay.style.display = DisplayStyle.None;

        _noLastGame = Q<Label>("NoLastGame");
        _lastGameResult = Q<VisualElement>("LastGameResult");
        _lastHomeLog = Q<VisualElement>("LastHomeLog");
        _lastAwayLog = Q<VisualElement>("LastAwayLog");
        _lastHomeName = Q<Label>("LastHomeName");
        _lastHomeRecord = Q<Label>("LastHomeRecord");
        _lastAwayName = Q<Label>("LastAwayName");
        _lastAwayRecord = Q<Label>("LastAwayRecord");
        _lastHomeScore = Q<Label>("LastHomeScore");
        _lastAwayScore = Q<Label>("LastAwayScore");
        _lastResultBadge = Q<Label>("LastResultBadge");
        _lastGameDate = Q<Label>("LastGameDate");
        _lastGameArena = Q<Label>("LastGameArena");
        _lastGameMeta = Q<VisualElement>("LastGameMeta");

        _noNextGame = Q<Label>("NoNextGame");
        _nextGameResult = Q<VisualElement>("NextGameResult");
        _nextHomeLog = Q<VisualElement>("NextHomeLog");
        _nextAwayLog = Q<VisualElement>("NextAwayLog");
        _nextHomeName = Q<Label>("NextHomeName");
        _nextHomeRecord = Q<Label>("NextHomeRecord");
        _nextAwayName = Q<Label>("NextAwayName");
        _nextAwayRecord = Q<Label>("NextAwayRecord");
        _nextGameDate = Q<Label>("NextGameDate");
        _nextGameArena = Q<Label>("NextGameArena");
        _nextGameMeta = Q<VisualElement>("NextGameMeta");

        _standingsBody = Q<VisualElement>("StandingsBody");
        _standingsLeague = Q<Label>("StandingsLeague");

        _teamObjective = Q<Label>("TeamObjective");
        _teamObjectiveStatus = Q<VisualElement>("TeamObjectiveStatus");
        _teamStatsLogo = Q<VisualElement>("TeamStatsLogo");
        _teamOverallRingVal = Q<Label>("TeamOverallRingVal");
        _teamChemistryRing = Q<VisualElement>("TeamChemistryRing");
        _teamChemistryRingVal = Q<Label>("TeamChemistryRingVal");
        _teamChemistryIcon = Q<VisualElement>("TeamChemistryIcon");
        _teamArenaName = Q<Label>("TeamArenaName");
        _teamArenaCapacity = Q<Label>("TeamArenaCapacity");
        _teamReputationStars = Q<VisualElement>("TeamReputationStars");

        _circleTrust = Q<VisualElement>("CircleTrust");
        _circleMorale = Q<VisualElement>("CircleMorale");
        _circleFanConfidence = Q<VisualElement>("CircleFanConfidence");
        _valTrust = Q<Label>("ValTrust");
        _valMorale = Q<Label>("ValMorale");
        _valFanConfidence = Q<Label>("ValFanConfidence");

        _messagesBody = Q<VisualElement>("MessagesBody");

        _mainMenuConfirmOverlay = Q<VisualElement>("ConfigMainMenuConfirmOverlay");
        _mainMenuConfirmBox = Q<VisualElement>("ConfigMainMenuConfirmBox");
        _exitConfirmOverlay = Q<VisualElement>("ConfigExitConfirmOverlay");
        _exitConfirmBox = Q<VisualElement>("ConfigExitConfirmBox");

        CreateToast();
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

        _players = db.GetPlayersByTeam(_myTeam.Id);
        ReloadGames();
        ReloadSeasonStats();
    }

    private void ReloadGames()
    {
        var db = DatabaseManager.Instance;
        _teamGames = db.GetGamesForTeam(_season.Id, _myTeam.Id);
        _regularGames = db.GetGamesByType(_season.Id, DatabaseManager.GameTypeRegular);
        _matchToday = GameDayRules.FindMatch(_teamGames, _myTeam.Id, _season.CurrentDate);
        _nextGame = db.GetNextGame(_season.Id, _myTeam.Id);
        _lastGame = db.GetLastPlayedGame(_season.Id, _myTeam.Id);
    }

    private void ReloadSeasonStats()
    {
        _seasonStats = new List<PlayerSeasonStatsDB>();
        if (_myTeam == null || _season == null) return;

        var db = DatabaseManager.Instance;
        if (db?.Db == null) return;

        var rows = db.Db.Table<PlayerSeasonStatsDB>()
                     .Where(s => s.TeamId == _myTeam.Id && s.SeasonYear == _season.YearStart)
                     .ToList();

        if (rows.Count > 0) _seasonStats = rows;
    }

    protected override void RegisterCallbacks()
    {
        SidebarController.ScreenUnavailable = _ => ShowToast(
            LocalizationService.T("ui.dashboard.proximamente"));
        SidebarController.Attach(_root, ScreenId);
        HeaderController.Attach(_root, this);

        _btnAction?.RegisterCallback<ClickEvent>(_ => { PlayClick(); OnActionClicked(); });
        _configIcon?.RegisterCallback<ClickEvent>(_ => { PlayClick(); OpenConfigModal(); });

        RegisterHandCursor(_btnAction);
        RegisterHandCursor(_configIcon);

        RegisterFooterLink(Q<Label>("LastGameFooterLink"), GameScreen.Results);
        RegisterFooterLink(Q<Label>("NextGameFooterLink"), GameScreen.PreGame);
        RegisterFooterLink(Q<Label>("StandingsFooterLink"), GameScreen.Standings);
        RegisterFooterLink(Q<Label>("MessagesFooterLink"), GameScreen.Messages);
        RegisterPanelLink(Q<VisualElement>("PanelLastGame"), GameScreen.Results);
        RegisterPanelLink(Q<VisualElement>("PanelNextGame"), GameScreen.PreGame);

        WireConfigExtras();
    }

    protected override void Refresh()
    {
        if (_myTeam == null || _season == null) return;

        RefreshHeader();
        RefreshLastGame();
        RefreshNextGame();
        RefreshTeamStats();
        RefreshStandings();
        RefreshPlayerStats();
        RefreshBoard();
        RefreshMessages();
        RefreshActionButton();
    }

    // ── HEADER ─────────────────────────────────────────────────────────────

    private void RefreshHeader()
    {
        if (_headerTeamLogo != null && Logos().TryGetValue(LogoKey(_myTeam), out var logo))
            _headerTeamLogo.style.backgroundImage = new StyleBackground(logo);

        if (_headerTeamName != null) _headerTeamName.text = _myTeam.Name.ToUpperInvariant();
        if (_headerManagerName != null)
            _headerManagerName.text = LocalizationService.T("ui.common.manager", _manager?.Name ?? "");

        if (_headerBudget != null) _headerBudget.text = $"{_myTeam.TransferBudget:N0} €";

        if (_headerChemistry != null)
        {
            int chemistry = _myTeam.Chemistry;
            _headerChemistry.text = $"{chemistry}%";
            _headerChemistry.RemoveFromClassList("header-stat-value--negative");
            _headerChemistry.RemoveFromClassList("header-stat-value--gold");
            if (chemistry < 40) _headerChemistry.AddToClassList("header-stat-value--negative");
            else if (chemistry < 70) _headerChemistry.AddToClassList("header-stat-value--gold");
        }

        if (_headerSeason != null)
            _headerSeason.text = LocalizationService.T("ui.dashboard.temporada",
                _season.YearStart, _season.YearEnd);

        if (_headerDate != null) _headerDate.text = GameDayRules.FormatFullDateEs(_season.CurrentDate);
    }

    // ── ÚLTIMO PARTIDO ─────────────────────────────────────────────────────

    private void RefreshLastGame()
    {
        if (_lastGame == null)
        {
            SetVisible(_noLastGame, true);
            SetVisible(_lastGameResult, false);
            SetVisible(_lastGameMeta, false);
            if (_lastGameDate != null) _lastGameDate.text = "";
            return;
        }

        SetVisible(_noLastGame, false);
        SetVisible(_lastGameResult, true);
        SetVisible(_lastGameMeta, true);

        var home = TeamOf(_lastGame.HomeTeamId);
        var away = TeamOf(_lastGame.AwayTeamId);

        SetLogo(_lastHomeLog, home);
        SetLogo(_lastAwayLog, away);

        if (_lastHomeName != null) _lastHomeName.text = home?.Name.ToUpperInvariant() ?? "";
        if (_lastAwayName != null) _lastAwayName.text = away?.Name.ToUpperInvariant() ?? "";
        if (_lastHomeScore != null) _lastHomeScore.text = _lastGame.HomeScore.ToString();
        if (_lastAwayScore != null) _lastAwayScore.text = _lastGame.AwayScore.ToString();

        if (_lastHomeRecord != null) _lastHomeRecord.text = RecordOf(_lastGame.HomeTeamId);
        if (_lastAwayRecord != null) _lastAwayRecord.text = RecordOf(_lastGame.AwayTeamId);

        if (_lastGameDate != null) _lastGameDate.text = GameDayRules.FormatShortDate(_lastGame.MatchDate);

        bool homeIsMine = _lastGame.HomeTeamId == _myTeam.Id;
        int mine = homeIsMine ? _lastGame.HomeScore : _lastGame.AwayScore;
        int theirs = homeIsMine ? _lastGame.AwayScore : _lastGame.HomeScore;

        if (_lastResultBadge != null)
        {
            bool won = mine > theirs;
            bool drawn = mine == theirs;
            _lastResultBadge.text = LocalizationService.T(
                won ? "ui.dashboard.victoria" : drawn ? "ui.dashboard.empate" : "ui.dashboard.derrota");

            _lastResultBadge.RemoveFromClassList("badge-win");
            _lastResultBadge.RemoveFromClassList("badge-loss");
            if (won) _lastResultBadge.AddToClassList("badge-win");
            else if (!drawn) _lastResultBadge.AddToClassList("badge-loss");
        }

        if (_lastGameArena != null) _lastGameArena.text = home?.StadiumName ?? "";
    }

    // ── PRÓXIMO PARTIDO ────────────────────────────────────────────────────

    private void RefreshNextGame()
    {
        if (_nextGame == null)
        {
            SetVisible(_noNextGame, true);
            SetVisible(_nextGameResult, false);
            SetVisible(_nextGameMeta, false);
            if (_nextGameDate != null) _nextGameDate.text = "";
            return;
        }

        SetVisible(_noNextGame, false);
        SetVisible(_nextGameResult, true);
        SetVisible(_nextGameMeta, true);

        var home = TeamOf(_nextGame.HomeTeamId);
        var away = TeamOf(_nextGame.AwayTeamId);

        SetLogo(_nextHomeLog, home);
        SetLogo(_nextAwayLog, away);

        if (_nextHomeName != null) _nextHomeName.text = home?.Name.ToUpperInvariant() ?? "";
        if (_nextAwayName != null) _nextAwayName.text = away?.Name.ToUpperInvariant() ?? "";
        if (_nextHomeRecord != null) _nextHomeRecord.text = RecordOf(_nextGame.HomeTeamId);
        if (_nextAwayRecord != null) _nextAwayRecord.text = RecordOf(_nextGame.AwayTeamId);
        if (_nextGameDate != null) _nextGameDate.text = GameDayRules.FormatShortDate(_nextGame.MatchDate);

        if (_nextGameArena != null)
        {
            string competition = _nextGame.GameType == DatabaseManager.GameTypePreseason
                ? LocalizationService.T("ui.dashboard.amistoso")
                : $"{_league?.Name ?? ""} · {LocalizationService.T("ui.dashboard.jornada", _nextGame.MatchDay)}";
            _nextGameArena.text = $"{home?.StadiumName ?? ""} · {competition}";
        }
    }

    // ── ESTADÍSTICAS DEL EQUIPO ────────────────────────────────────────────

    private void RefreshTeamStats()
    {
        int total = _teams.Count;
        int position = CurrentPosition(total);

        if (_teamObjective != null)
            _teamObjective.text = LocalizationService.T(
                SeasonObjective.KeyFor(_myTeam.LeagueId, position, total));

        if (_teamObjectiveStatus != null)
        {
            string icon = IsObjectiveOnTrack(position, total) ? "boton-v-64px" : "boton-x-64px";
            var texture = Resources.Load<Texture2D>($"Icons/{icon}");
            if (texture != null) _teamObjectiveStatus.style.backgroundImage = new StyleBackground(texture);
        }

        SetLogo(_teamStatsLogo, _myTeam);

        int average = _players.Count > 0
            ? Mathf.RoundToInt((float)_players.Average(p => p.Overall))
            : _myTeam.Overall;

        if (_teamOverallRingVal != null) _teamOverallRingVal.text = average.ToString();

        int chemistry = _myTeam.Chemistry;
        if (_teamChemistryRingVal != null) _teamChemistryRingVal.text = chemistry.ToString();
        if (_teamChemistryIcon != null)
        {
            var texture = Resources.Load<Texture2D>("Icons/quimica");
            if (texture != null) _teamChemistryIcon.style.backgroundImage = new StyleBackground(texture);
        }

        if (_teamChemistryRing != null)
        {
            Color ringColor = chemistry >= 70
                ? new Color32(39, 174, 96, 255)
                : chemistry >= 40 ? new Color32(212, 160, 23, 255) : new Color32(192, 57, 43, 255);
            _teamChemistryRing.style.borderTopColor = new StyleColor(ringColor);
            _teamChemistryRing.style.borderBottomColor = new StyleColor(ringColor);
            _teamChemistryRing.style.borderLeftColor = new StyleColor(ringColor);
            _teamChemistryRing.style.borderRightColor = new StyleColor(ringColor);
        }

        if (_teamArenaName != null) _teamArenaName.text = _myTeam.StadiumName;
        if (_teamArenaCapacity != null)
            _teamArenaCapacity.text = LocalizationService.T("ui.dashboard.espectadores", _myTeam.StadiumCapacity);

        if (_teamReputationStars != null)
        {
            _teamReputationStars.Clear();
            int stars = Mathf.Clamp(Mathf.RoundToInt((float)_myTeam.Reputation / 20f), 1, 5);
            for (int i = 1; i <= 5; i++)
            {
                var star = new Label { text = i <= stars ? "★" : "☆" };
                star.AddToClassList("team-stat-star");
                star.style.color = i <= stars
                    ? new Color(0.84f, 0.63f, 0.09f)
                    : new Color(0.2f, 0.25f, 0.35f);
                _teamReputationStars.Add(star);
            }
        }
    }

    private bool IsObjectiveOnTrack(int position, int total)
    {
        string key = SeasonObjective.KeyFor(_myTeam.LeagueId, position, total);
        switch (key)
        {
            case SeasonObjective.KeyCampeonato: return position <= 4;
            case SeasonObjective.KeyEuropa: return position <= 7;
            case SeasonObjective.KeyAscenso: return position <= 2;
            case SeasonObjective.KeyPlayoff: return position <= 6;
            case SeasonObjective.KeyMedia: return position <= total - 5;
            default: return position <= total - 5;
        }
    }

    // ── CLASIFICACIÓN ──────────────────────────────────────────────────────

    private void RefreshStandings()
    {
        if (_standingsLeague != null && _league != null) _standingsLeague.text = _league.Name;
        if (_standingsBody == null) return;

        _standingsBody.Clear();

        var table = LeagueTable.Build(_teams.Select(t => t.Id), _regularGames);
        foreach (var row in table.Take(10))
            _standingsBody.Add(BuildStandingsRow(row, table.IndexOf(row) + 1));
    }

    private VisualElement BuildStandingsRow(LeagueTableRow row, int position)
    {
        var element = new VisualElement();
        element.AddToClassList("standings-row");

        if (row.TeamId == _myTeam.Id) element.AddToClassList("standings-row--my-team");
        else if (position <= 6) element.AddToClassList("standings-row--playoff");
        else if (position <= 10) element.AddToClassList("standings-row--playin");

        element.Add(Cell(position.ToString(), "col-rank"));

        var logo = new VisualElement();
        logo.AddToClassList("col-team-logo");
        SetLogo(logo, TeamOf(row.TeamId));
        element.Add(logo);

        element.Add(Cell(TeamOf(row.TeamId)?.Name ?? "???", "col-team-name"));
        element.Add(Cell(row.Played.ToString(), "col-stat"));
        element.Add(Cell(row.Won.ToString(), "col-stat", "col-wins"));
        element.Add(Cell(row.Drawn.ToString(), "col-stat"));
        element.Add(Cell(row.Lost.ToString(), "col-stat", "col-losses"));
        element.Add(Cell(row.GoalDifference > 0 ? $"+{row.GoalDifference}" : row.GoalDifference.ToString(), "col-stat"));
        element.Add(Cell(row.Points.ToString(), "col-stat", "col-points"));

        return element;
    }

    private static Label Cell(string text, params string[] classes)
    {
        var label = new Label(text);
        foreach (string css in classes) label.AddToClassList(css);
        return label;
    }

    // ── ESTADÍSTICAS DE JUGADORES (FÚTBOL) ─────────────────────────────────

    private static readonly string[] StatCardPrefixes =
    {
        "StatGoals", "StatAssists", "StatYellow", "StatRed", "StatMvp", "StatMinutes"
    };

    private void RefreshPlayerStats()
    {
        if (_seasonStats.Count == 0)
        {
            foreach (string prefix in StatCardPrefixes)
                SetStatCard(prefix, "--", "", LocalizationService.T("ui.dashboard.sin_datos"));
            return;
        }

        SetLeader("StatGoals", s => s.Goals);
        SetLeader("StatAssists", s => s.Assists);
        SetLeader("StatYellow", s => s.YellowCards);
        SetLeader("StatRed", s => s.RedCards);
        SetLeader("StatMvp", s => s.ManOfTheMatch);
        SetLeader("StatMinutes", s => s.MinutesPlayed);
    }

    /// <summary>Líder del equipo en una estadística (goles, asistencias, tarjetas…).</summary>
    private void SetLeader(string prefix, System.Func<PlayerSeasonStatsDB, int> selector)
    {
        var leader = _seasonStats
            .OrderByDescending(selector)
            .ThenByDescending(s => s.Appearances)
            .First();

        int value = selector(leader);
        if (value <= 0)
        {
            SetStatCard(prefix, "--", "", LocalizationService.T("ui.dashboard.sin_datos"));
            return;
        }

        SetStatCard(prefix, value.ToString(), PlayerName(leader.PlayerId), GamesText(leader.Appearances));
    }

    private void SetStatCard(string prefix, string value, string playerName, string games)
    {
        var valueLabel = Q<Label>(prefix);
        var nameLabel = Q<Label>(prefix + "Name");
        var gamesLabel = Q<Label>(prefix + "Games");

        if (valueLabel != null) valueLabel.text = value;
        if (nameLabel != null) nameLabel.text = playerName;
        if (gamesLabel != null) gamesLabel.text = games;
    }

    private string PlayerName(int playerId)
    {
        var player = _players.FirstOrDefault(p => p.Id == playerId)
                     ?? DatabaseManager.Instance.GetPlayerById(playerId);
        if (player == null) return "";

        string name = $"{player.FirstName} {player.LastName}".Trim();
        return string.IsNullOrEmpty(name) ? player.CommonName ?? "" : name;
    }

    private static string GamesText(int appearances)
        => LocalizationService.T("ui.dashboard.partidos_jugados", appearances);

    // ── RELACIONES ─────────────────────────────────────────────────────────

    private void RefreshBoard()
    {
        if (_manager == null) return;

        SetCircle(_circleTrust, _valTrust, _manager.Trust);
        SetCircle(_circleMorale, _valMorale, _manager.Morale);
        SetCircle(_circleFanConfidence, _valFanConfidence, _manager.FanConfidence);
    }

    private static void SetCircle(VisualElement circle, Label label, int value)
    {
        if (circle == null || label == null) return;

        Color background, border;
        if (value >= 70)
        {
            background = new Color32(39, 174, 96, 40);
            border = new Color32(39, 174, 96, 255);
        }
        else if (value >= 40)
        {
            background = new Color32(212, 160, 23, 40);
            border = new Color32(212, 160, 23, 255);
        }
        else
        {
            background = new Color32(192, 57, 43, 40);
            border = new Color32(192, 57, 43, 255);
        }

        circle.style.backgroundColor = new StyleColor(background);
        circle.style.borderTopColor = new StyleColor(border);
        circle.style.borderBottomColor = new StyleColor(border);
        circle.style.borderLeftColor = new StyleColor(border);
        circle.style.borderRightColor = new StyleColor(border);
        label.text = $"{value}%";
    }

    // ── NOTICIAS ───────────────────────────────────────────────────────────

    private void RefreshMessages()
    {
        if (_messagesBody == null) return;
        _messagesBody.Clear();

        if (_nextGame != null)
        {
            bool home = _nextGame.HomeTeamId == _myTeam.Id;
            var rival = TeamOf(home ? _nextGame.AwayTeamId : _nextGame.HomeTeamId);
            AddNews(
                LocalizationService.T("ui.dashboard.noticia_proximo_titulo", rival?.Name ?? ""),
                LocalizationService.T("ui.dashboard.noticia_proximo_cuerpo",
                    GameDayRules.FormatShortDate(_nextGame.MatchDate),
                    LocalizationService.T(home ? "ui.dashboard.local" : "ui.dashboard.visitante")));
        }
        else
        {
            AddNews(
                LocalizationService.T("ui.dashboard.noticia_sin_partido_titulo"),
                LocalizationService.T("ui.dashboard.noticia_sin_partido_cuerpo"));
        }

        int injured = _players.Count(p => p.InjuryDays > 0);
        AddNews(
            LocalizationService.T("ui.dashboard.noticia_objetivo_titulo"),
            LocalizationService.T("ui.dashboard.noticia_objetivo_cuerpo",
                LocalizationService.T(SeasonObjective.KeyFor(_myTeam.LeagueId, CurrentPosition(_teams.Count), _teams.Count))));

        AddNews(
            LocalizationService.T("ui.dashboard.noticia_plantilla_titulo"),
            injured > 0
                ? LocalizationService.T("ui.dashboard.noticia_plantilla_lesiones", _players.Count, injured)
                : LocalizationService.T("ui.dashboard.noticia_plantilla_sana", _players.Count));
    }

    private void AddNews(string subject, string body)
    {
        var item = new VisualElement();
        item.AddToClassList("message-item");

        var textBlock = new VisualElement();
        textBlock.AddToClassList("message-item-text");

        var subjectLabel = new Label(subject);
        subjectLabel.AddToClassList("message-item-subject");
        textBlock.Add(subjectLabel);

        var bodyLabel = new Label(body);
        bodyLabel.AddToClassList("message-item-body");
        textBlock.Add(bodyLabel);

        item.Add(textBlock);
        _messagesBody.Add(item);
    }

    // ── ACCIÓN PRINCIPAL / AVANCE DE DÍA ───────────────────────────────────

    private void RefreshActionButton()
    {
        if (_btnAction == null) return;

        if (_matchToday != null)
        {
            _btnAction.text = LocalizationService.T("ui.dashboard.btn_play");
            _btnAction.AddToClassList("btn-action--match");
        }
        else
        {
            _btnAction.text = LocalizationService.T("ui.dashboard.btn_action");
            _btnAction.RemoveFromClassList("btn-action--match");
        }
    }

    private void OnActionClicked()
    {
        if (ScreenManager.NavigationLocked) return;

        // Día de partido: primero la previa (PreGame), que lleva a MatchDay.
        if (_matchToday != null && ScreenManager.Instance.IsRegistered(GameScreen.PreGame))
        {
            MatchContext.Set(_matchToday);
            ScreenManager.Instance.GoTo(GameScreen.PreGame);
            return;
        }

        var db = DatabaseManager.Instance;
        ScreenManager.NavigationLocked = true;
        try
        {
            // Sin PreGame: simular directamente el encuentro.
            if (_matchToday != null)
            {
                if (db.PlayToday(_season, _myTeam.Id, out var match))
                {
                    GameSaveManager.UpdateSlotFromDatabase(db.ActiveSaveSlot);
                    ReloadGames();
                    Refresh();

                    ShowToast(LocalizationService.T("ui.dashboard.resultado_partido",
                        match.HomeScore, match.AwayScore));
                }
                else
                {
                    ShowToast(LocalizationService.T("ui.dashboard.error_partido"));
                }

                return;
            }

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

    // ── MODALES EXTRA (menú principal / salir) ─────────────────────────────

    private void WireConfigExtras()
    {
        Q<Button>("ConfigBtnMainMenu")?.RegisterCallback<ClickEvent>(_ =>
        {
            PlayClick();
            ShowModal(_mainMenuConfirmOverlay, _mainMenuConfirmBox);
        });
        Q<Button>("ConfigBtnMainMenuNo")?.RegisterCallback<ClickEvent>(_ =>
        {
            PlayClick();
            HideModal(_mainMenuConfirmOverlay, _mainMenuConfirmBox);
        });
        Q<Button>("ConfigBtnMainMenuYes")?.RegisterCallback<ClickEvent>(_ =>
        {
            PlayClick();
            HideModal(_mainMenuConfirmOverlay, _mainMenuConfirmBox);
            CloseConfigModal();
            GameSaveManager.UpdateSlotFromDatabase(DatabaseManager.Instance.ActiveSaveSlot);
            ScreenManager.Instance.GoTo(GameScreen.MainMenu);
        });

        Q<Button>("ConfigBtnExit")?.RegisterCallback<ClickEvent>(_ =>
        {
            PlayClick();
            ShowModal(_exitConfirmOverlay, _exitConfirmBox);
        });
        Q<Button>("ConfigBtnExitNo")?.RegisterCallback<ClickEvent>(_ =>
        {
            PlayClick();
            HideModal(_exitConfirmOverlay, _exitConfirmBox);
        });
        Q<Button>("ConfigBtnExitYes")?.RegisterCallback<ClickEvent>(_ => { PlayClick(); QuitGame(); });
    }

    private static void ShowModal(VisualElement overlay, VisualElement box)
    {
        if (overlay == null) return;
        overlay.style.display = DisplayStyle.Flex;
        overlay.AddToClassList("modal-overlay--visible");
        if (box != null)
        {
            box.style.display = DisplayStyle.Flex;
            box.AddToClassList("modal-box--visible");
        }
    }

    private static void HideModal(VisualElement overlay, VisualElement box)
    {
        if (overlay == null) return;
        overlay.RemoveFromClassList("modal-overlay--visible");
        overlay.style.display = DisplayStyle.None;
        if (box != null)
        {
            box.RemoveFromClassList("modal-box--visible");
            box.style.display = DisplayStyle.None;
        }
    }

    private static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Simula el partido de hoy (y el resto de la jornada) y navega a MatchDay.
    /// La usa PreGame desde su botón JUGAR PARTIDO.
    /// </summary>
    public static bool RequestPlayMatch()
    {
        var db = DatabaseManager.Instance;
        var manager = db?.GetActiveManager();
        if (manager == null) return false;

        var team = db.GetTeamById(manager.TeamId);
        var season = db.GetActiveSeason(manager.Id);
        if (team == null || season == null) return false;

        var game = db.GetMatchOnDate(season.Id, team.Id, season.CurrentDate);
        if (game == null) return false;

        MatchContext.Set(game);

        if (!db.PlayToday(season, team.Id, out var result) || result == null)
            return false;

        MatchContext.Result = result;
        GameSaveManager.UpdateSlotFromDatabase(db.ActiveSaveSlot);

        var screens = ScreenManager.Instance;
        if (screens.IsRegistered(GameScreen.MatchDay))
            screens.GoTo(GameScreen.MatchDay);
        else if (screens.IsRegistered(GameScreen.GameResults))
            screens.GoTo(GameScreen.GameResults);

        return true;
    }

    // ── NAVEGACIÓN Y UTILIDADES ────────────────────────────────────────────

    private void RegisterFooterLink(Label link, GameScreen target)
    {
        if (link == null) return;
        link.RegisterCallback<ClickEvent>(_ => { PlayClick(); Navigate(target); });
        RegisterHandCursor(link);
    }

    private void RegisterPanelLink(VisualElement panel, GameScreen target)
    {
        if (panel == null) return;
        panel.RegisterCallback<ClickEvent>(evt =>
        {
            // No navegar si el clic viene de un enlace del propio panel.
            if (evt.target is Label) return;
            PlayClick();
            Navigate(target);
        });
        RegisterHandCursor(panel);
    }

    private void Navigate(GameScreen target)
    {
        var manager = ScreenManager.Instance;
        if (manager != null && manager.IsRegistered(target))
        {
            manager.GoTo(target);
            return;
        }

        ShowToast(LocalizationService.T("ui.dashboard.proximamente"));
    }

    private int CurrentPosition(int total)
    {
        if (total <= 0) return 0;

        bool hasResults = _regularGames.Any(g => g.IsPlayed == 1);
        if (hasResults)
        {
            int position = LeagueTable.PositionOf(
                LeagueTable.Build(_teams.Select(t => t.Id), _regularGames), _myTeam.Id);
            if (position > 0) return position;
        }

        var ordered = _teams.OrderByDescending(t => t.Overall).ThenBy(t => t.Id).ToList();
        int index = ordered.FindIndex(t => t.Id == _myTeam.Id);
        return index >= 0 ? index + 1 : total;
    }

    private string RecordOf(int teamId)
    {
        int won = 0, drawn = 0, lost = 0;
        foreach (var game in _regularGames)
        {
            if (game.IsPlayed != 1) continue;
            if (game.HomeTeamId == teamId)
            {
                if (game.HomeScore > game.AwayScore) won++;
                else if (game.HomeScore < game.AwayScore) lost++;
                else drawn++;
            }
            else if (game.AwayTeamId == teamId)
            {
                if (game.AwayScore > game.HomeScore) won++;
                else if (game.AwayScore < game.HomeScore) lost++;
                else drawn++;
            }
        }
        return $"{won}-{drawn}-{lost}";
    }

    private TeamDataDB TeamOf(int teamId)
        => _teamsById.TryGetValue(teamId, out var team) ? team : null;

    private static string LogoKey(TeamDataDB team)
        => string.IsNullOrEmpty(team.Logo) ? team.Id.ToString() : team.Logo;

    private void SetLogo(VisualElement element, TeamDataDB team)
    {
        if (element == null || team == null) return;
        if (Logos().TryGetValue(LogoKey(team), out var sprite))
            element.style.backgroundImage = new StyleBackground(sprite);
    }

    private static Dictionary<string, Sprite> Logos()
    {
        if (_logoCache != null) return _logoCache;

        _logoCache = new Dictionary<string, Sprite>();
        foreach (var sprite in Resources.LoadAll<Sprite>("Teams/Logos/64x64"))
            _logoCache[sprite.name] = sprite;
        return _logoCache;
    }

    private void CreateToast()
    {
        if (_toast != null) return;

        _toast = new Label();
        _toast.AddToClassList("dashboard-toast");
        _toast.style.display = DisplayStyle.None;
        _root.Add(_toast);
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
