using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// F8 — Día de partido (réplica de Tactical5 adaptada al fútbol): marcador,
/// estadísticas del equipo, cronología y estadísticas individuales.
/// </summary>
public class MatchDayController : UIScreenController
{
    public override GameScreen ScreenId => GameScreen.MatchDay;

    // Header
    private VisualElement _headerLogo;
    private Label _headerTeam, _headerSubtitle, _headerManager;
    private Label _headerSeason, _headerDate, _headerGameDay;
    private Button _btnContinue;

    // Banner
    private VisualElement _homeLogo, _awayLogo;
    private Label _homeName, _awayName;
    private Label _homeBadge, _awayBadge;
    private Label _homeScore, _awayScore;
    private Label _venue;
    private Label _status;

    // Stats
    private Label _posHome, _posAway;
    private Label _shotsHome, _shotsAway;
    private Label _onTargetHome, _onTargetAway;
    private Label _cornersHome, _cornersAway;
    private Label _foulsHome, _foulsAway;
    private Label _yellowHome, _yellowAway;

    // Timeline
    private VisualElement _timelineBody;

    // Box scores
    private VisualElement _homeBoxLogo, _awayBoxLogo;
    private Label _homeBoxName, _awayBoxName;
    private VisualElement _homeBoxBody, _awayBoxBody;
    private VisualElement _homeBoxFooter, _awayBoxFooter;

    // Datos
    private GameDataDB _game;
    private TeamDataDB _home, _away;
    private readonly Dictionary<string, Sprite> _logos = new();
    private readonly Dictionary<int, PlayerDataDB> _players = new();

    protected override void CacheReferences()
    {
        _headerLogo = Q<VisualElement>("HeaderTeamLogo");
        _headerTeam = Q<Label>("HeaderTeamName");
        _headerSubtitle = Q<Label>("HeaderSubtitle");
        _headerManager = Q<Label>("HeaderManagerName");
        _headerSeason = Q<Label>("HeaderSeason");
        _headerDate = Q<Label>("HeaderDate");
        _headerGameDay = Q<Label>("HeaderGameDay");
        _btnContinue = Q<Button>("BtnContinue");

        _homeLogo = Q<VisualElement>("HomeLogo");
        _awayLogo = Q<VisualElement>("AwayLogo");
        _homeName = Q<Label>("HomeName");
        _awayName = Q<Label>("AwayName");
        _homeBadge = Q<Label>("HomeMyTeamBadge");
        _awayBadge = Q<Label>("AwayMyTeamBadge");
        _homeScore = Q<Label>("HomeScore");
        _awayScore = Q<Label>("AwayScore");
        _venue = Q<Label>("VenueLabel");
        _status = Q<Label>("MatchStatus");

        _posHome = Q<Label>("StatPossessionHome"); _posAway = Q<Label>("StatPossessionAway");
        _shotsHome = Q<Label>("StatShotsHome"); _shotsAway = Q<Label>("StatShotsAway");
        _onTargetHome = Q<Label>("StatOnTargetHome"); _onTargetAway = Q<Label>("StatOnTargetAway");
        _cornersHome = Q<Label>("StatCornersHome"); _cornersAway = Q<Label>("StatCornersAway");
        _foulsHome = Q<Label>("StatFoulsHome"); _foulsAway = Q<Label>("StatFoulsAway");
        _yellowHome = Q<Label>("StatYellowHome"); _yellowAway = Q<Label>("StatYellowAway");

        _timelineBody = Q<VisualElement>("TimelineBody");

        _homeBoxLogo = Q<VisualElement>("HomeBoxLogo");
        _awayBoxLogo = Q<VisualElement>("AwayBoxLogo");
        _homeBoxName = Q<Label>("HomeBoxName");
        _awayBoxName = Q<Label>("AwayBoxName");
        _homeBoxBody = Q<VisualElement>("HomeBoxBody");
        _awayBoxBody = Q<VisualElement>("AwayBoxBody");
        _homeBoxFooter = Q<VisualElement>("HomeBoxFooter");
        _awayBoxFooter = Q<VisualElement>("AwayBoxFooter");
    }

    protected override void LoadData()
    {
        base.LoadData();
        if (_myTeam == null || _season == null) return;

        var db = DatabaseManager.Instance;

        foreach (var sprite in Resources.LoadAll<Sprite>("Teams/Logos/120x120"))
            _logos[sprite.name] = sprite;

        foreach (var player in db.Db.Table<PlayerDataDB>().ToList())
            _players[player.Id] = player;

        _game = ResolveGame(db);
        if (_game == null) return;

        _home = db.GetTeamById(_game.HomeTeamId);
        _away = db.GetTeamById(_game.AwayTeamId);

        if (_game.IsPlayed == 0)
            db.SimulateGame(_season, _game);
    }

    private GameDataDB ResolveGame(DatabaseManager db)
    {
        if (MatchContext.HasGame)
        {
            var target = db.GetGamesBySeason(_season.Id).FirstOrDefault(g => g.Id == MatchContext.GameId);
            if (target != null) return target;
        }

        return db.GetLastPlayedGame(_season.Id, _myTeam.Id)
               ?? db.GetMatchOnDate(_season.Id, _myTeam.Id, _season.CurrentDate);
    }

    protected override void RegisterCallbacks()
    {
        _btnContinue?.RegisterCallback<ClickEvent>(_ =>
        {
            PlayClick();
            if (ScreenManager.Instance.IsRegistered(GameScreen.GameResults))
                ScreenManager.Instance.GoTo(GameScreen.GameResults);
            else
                ScreenManager.Instance.GoTo(GameScreen.Dashboard);
        });

        RegisterHandCursor(_btnContinue);
    }

    protected override void Refresh()
    {
        if (_game == null || _home == null || _away == null)
        {
            if (_status != null)
                _status.text = LocalizationService.T("ui.match_day.sin_partido");
            return;
        }

        RefreshHeader();
        RefreshBanner();
        RefreshStats();
        RefreshTimeline();
        RefreshBoxScores();
    }

    // ── HEADER Y BANNER ────────────────────────────────────────────────────

    private void RefreshHeader()
    {
        SetLogo(_headerLogo, _myTeam);
        if (_headerTeam != null) _headerTeam.text = _myTeam.Name.ToUpperInvariant();
        if (_headerSubtitle != null) _headerSubtitle.text = LocalizationService.T("ui.match_day.header_subtitle");
        if (_headerManager != null)
            _headerManager.text = LocalizationService.T("ui.common.manager", _manager?.Name ?? "");
        if (_headerSeason != null)
            _headerSeason.text = LocalizationService.T("ui.dashboard.temporada",
                _season.YearStart, _season.YearEnd);
        if (_headerDate != null) _headerDate.text = GameDayRules.FormatLongDate(_game.MatchDate);
        if (_headerGameDay != null)
            _headerGameDay.text = _game.GameType == DatabaseManager.GameTypePreseason
                ? LocalizationService.T("ui.dashboard.amistoso")
                : LocalizationService.T("ui.dashboard.jornada", _game.MatchDay);
    }

    private void RefreshBanner()
    {
        SetLogo(_homeLogo, _home);
        SetLogo(_awayLogo, _away);
        SetLogo(_homeBoxLogo, _home);
        SetLogo(_awayBoxLogo, _away);

        if (_homeName != null) _homeName.text = _home.Name.ToUpperInvariant();
        if (_awayName != null) _awayName.text = _away.Name.ToUpperInvariant();
        if (_homeBoxName != null) _homeBoxName.text = _home.Name.ToUpperInvariant();
        if (_awayBoxName != null) _awayBoxName.text = _away.Name.ToUpperInvariant();

        if (_homeBadge != null)
            SetVisible(_homeBadge, _game.HomeTeamId == _myTeam.Id);
        if (_awayBadge != null)
            SetVisible(_awayBadge, _game.AwayTeamId == _myTeam.Id);

        if (_homeScore != null) _homeScore.text = _game.HomeScore.ToString();
        if (_awayScore != null) _awayScore.text = _game.AwayScore.ToString();
        if (_venue != null)
            _venue.text = LocalizationService.T("ui.match_day.venue",
                _home.StadiumName, _game.Attendance);
        if (_status != null)
            _status.text = _game.IsPlayed == 1
                ? LocalizationService.T("ui.match_day.finalizado")
                : LocalizationService.T("ui.match_day.pendiente");
    }

    private void RefreshStats()
    {
        SetText(_posHome, $"{_game.HomePossession}%");
        SetText(_posAway, $"{_game.AwayPossession}%");
        SetText(_shotsHome, _game.HomeShots.ToString());
        SetText(_shotsAway, _game.AwayShots.ToString());
        SetText(_onTargetHome, _game.HomeShotsOnTarget.ToString());
        SetText(_onTargetAway, _game.AwayShotsOnTarget.ToString());
        SetText(_cornersHome, _game.HomeCorners.ToString());
        SetText(_cornersAway, _game.AwayCorners.ToString());
        SetText(_foulsHome, _game.HomeFouls.ToString());
        SetText(_foulsAway, _game.AwayFouls.ToString());
        SetText(_yellowHome, _game.HomeYellowCards.ToString());
        SetText(_yellowAway, _game.AwayYellowCards.ToString());
    }

    // ── CRONOLOGÍA ─────────────────────────────────────────────────────────

    private void RefreshTimeline()
    {
        if (_timelineBody == null) return;
        _timelineBody.Clear();

        var events = DatabaseManager.Instance.GetMatchEvents(_game.Id);
        if (events.Count == 0)
        {
            var empty = new Label(LocalizationService.T("ui.match_day.sin_eventos"));
            empty.AddToClassList("md-empty");
            _timelineBody.Add(empty);
            return;
        }

        foreach (var matchEvent in events)
        {
            var row = new VisualElement();
            row.AddToClassList("md-timeline-row");
            if (matchEvent.TeamId == _home.Id) row.AddToClassList("md-timeline-row--home");
            else if (matchEvent.TeamId == _away.Id) row.AddToClassList("md-timeline-row--away");

            var minute = new Label($"{matchEvent.Minute}'");
            minute.AddToClassList("md-minute");

            var text = new Label(Describe(matchEvent));
            text.AddToClassList("md-event-text");

            var team = new Label(TeamNameOf(matchEvent.TeamId));
            team.AddToClassList("md-event-team");

            row.Add(minute);
            row.Add(text);
            row.Add(team);
            _timelineBody.Add(row);
        }
    }

    private string Describe(MatchEventDataDB matchEvent)
    {
        var player = PlayerName(matchEvent.PlayerId);
        var secondary = PlayerName(matchEvent.SecondaryPlayerId);

        return (MatchEventType)matchEvent.EventType switch
        {
            MatchEventType.Goal => string.IsNullOrEmpty(secondary)
                ? LocalizationService.T("ui.match_day.event_goal", player)
                : LocalizationService.T("ui.match_day.event_goal_assist", player, secondary),
            MatchEventType.YellowCard => LocalizationService.T("ui.match_day.event_yellow", player),
            MatchEventType.RedCard => LocalizationService.T("ui.match_day.event_red", player),
            MatchEventType.SubstitutionOn => LocalizationService.T("ui.match_day.event_sub", player, secondary),
            MatchEventType.Save => LocalizationService.T("ui.match_day.event_save", player),
            MatchEventType.Chance => LocalizationService.T("ui.match_day.event_chance", player),
            MatchEventType.KickOff => LocalizationService.T("ui.match_day.event_kickoff"),
            MatchEventType.FullTime => LocalizationService.T("ui.match_day.event_fulltime"),
            MatchEventType.AddedTime => LocalizationService.T("ui.match_day.event_added", matchEvent.Detail),
            _ => LocalizationService.T("ui.match_day.event_default")
        };
    }

    // ── ESTADÍSTICAS INDIVIDUALES ──────────────────────────────────────────

    private void RefreshBoxScores()
    {
        var allStats = DatabaseManager.Instance.GetPlayerMatchStats(_game.Id);

        FillBox(_homeBoxBody, _homeBoxFooter, allStats.Where(s => s.TeamId == _home.Id).ToList());
        FillBox(_awayBoxBody, _awayBoxFooter, allStats.Where(s => s.TeamId == _away.Id).ToList());
    }

    private void FillBox(VisualElement body, VisualElement footer, List<PlayerMatchStatsDB> stats)
    {
        if (body == null) return;
        body.Clear();

        stats = stats.OrderByDescending(s => s.MinutesPlayed)
                     .ThenByDescending(s => s.Rating)
                     .ToList();

        foreach (var stat in stats)
        {
            var row = new VisualElement();
            row.AddToClassList("box-row");

            row.Add(Cell(PlayerName(stat.PlayerId), "col-player", "player-name"));
            row.Add(Cell(stat.MinutesPlayed.ToString(), "col-min"));
            row.Add(Cell(stat.Goals.ToString(), "col-goals"));
            row.Add(Cell(stat.Assists.ToString(), "col-assists"));
            row.Add(Cell(stat.Shots.ToString(), "col-shots"));
            row.Add(Cell(stat.Saves.ToString(), "col-saves"));
            row.Add(Cell(stat.YellowCard.ToString(), "col-yellow"));
            row.Add(Cell(stat.RedCard.ToString(), "col-red"));
            row.Add(Cell(stat.Rating.ToString("F1", System.Globalization.CultureInfo.InvariantCulture), "col-rating"));

            body.Add(row);
        }

        if (footer == null) return;
        footer.Clear();

        var total = new VisualElement();
        total.AddToClassList("box-total-row");
        total.Add(Cell(LocalizationService.T("ui.match_day.total"), "col-player", "player-name"));
        total.Add(Cell(stats.Sum(s => s.MinutesPlayed).ToString(), "col-min"));
        total.Add(Cell(stats.Sum(s => s.Goals).ToString(), "col-goals"));
        total.Add(Cell(stats.Sum(s => s.Assists).ToString(), "col-assists"));
        total.Add(Cell(stats.Sum(s => s.Shots).ToString(), "col-shots"));
        total.Add(Cell(stats.Sum(s => s.Saves).ToString(), "col-saves"));
        total.Add(Cell(stats.Sum(s => s.YellowCard).ToString(), "col-yellow"));
        total.Add(Cell(stats.Sum(s => s.RedCard).ToString(), "col-red"));
        total.Add(Cell("", "col-rating"));
        footer.Add(total);
    }

    // ── UTILIDADES ─────────────────────────────────────────────────────────

    private static Label Cell(string text, params string[] classes)
    {
        var label = new Label(text);
        foreach (string css in classes) label.AddToClassList(css);
        return label;
    }

    private static void SetText(Label label, string text)
    {
        if (label != null) label.text = text;
    }

    private string PlayerName(int playerId)
    {
        if (playerId <= 0) return "";
        if (_players.TryGetValue(playerId, out var player))
            return string.IsNullOrEmpty(player.CommonName)
                ? $"{player.FirstName} {player.LastName}".Trim()
                : player.CommonName;

        var dbPlayer = DatabaseManager.Instance.GetPlayerById(playerId);
        return dbPlayer == null ? "" : $"{dbPlayer.FirstName} {dbPlayer.LastName}".Trim();
    }

    private string TeamNameOf(int teamId)
    {
        if (teamId == _home?.Id) return _home.Name;
        if (teamId == _away?.Id) return _away.Name;
        return "";
    }

    private void SetLogo(VisualElement element, TeamDataDB team)
    {
        if (element == null || team == null) return;
        string key = string.IsNullOrEmpty(team.Logo) ? team.Id.ToString() : team.Logo;
        if (_logos.TryGetValue(key, out var sprite))
            element.style.backgroundImage = new StyleBackground(sprite);
    }
}
