using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// F8 — Resultados del día (réplica de Tactical5 adaptada al fútbol): partidos
/// jugados esa fecha, filtros por las competiciones presentes, MVP del día y
/// líderes de goleadores/asistentes/notas.
/// </summary>
public class GameResultsController : UIScreenController
{
    public override GameScreen ScreenId => GameScreen.GameResults;

    private VisualElement _headerLogo;
    private Label _headerTeam, _headerSubtitle, _headerManager;
    private Label _headerSeason, _headerDate, _headerGameDay;
    private Button _btnDashboard;
    private VisualElement _loadingSpinner;

    private VisualElement _filtersBar;
    private VisualElement _gamesBody;
    private VisualElement _mvpPanel;
    private VisualElement _mvpLogo;
    private Label _mvpName, _mvpTeam, _mvpVal;

    private VisualElement _lbScorersBody, _lbAssistersBody, _lbRatedBody;

    private GameDataDB _game;
    private string _dayDate;
    private List<GameDataDB> _seasonGames = new();
    private List<GameDataDB> _dayGames = new();
    private List<PlayerMatchStatsDB> _dayStats = new();
    private List<LeaderEntry> _leaderEntries = new();
    private int _filterCompetition; // 0 = todos los partidos del día

    private readonly List<(Button Button, int CompetitionId)> _filterButtons = new();
    private readonly Dictionary<int, TeamDataDB> _teams = new();
    private readonly Dictionary<int, PlayerDataDB> _players = new();
    private readonly Dictionary<string, Sprite> _logos = new();

    protected override void CacheReferences()
    {
        _headerLogo = Q<VisualElement>("HeaderTeamLogo");
        _headerTeam = Q<Label>("HeaderTeamName");
        _headerSubtitle = Q<Label>("HeaderSubtitle");
        _headerManager = Q<Label>("HeaderManagerName");
        _headerSeason = Q<Label>("HeaderSeason");
        _headerDate = Q<Label>("HeaderDate");
        _headerGameDay = Q<Label>("HeaderGameDay");
        _btnDashboard = Q<Button>("BtnDashboard");
        _loadingSpinner = Q<VisualElement>("LoadingSpinner");

        _filtersBar = Q<VisualElement>("FiltersBar");
        _gamesBody = Q<VisualElement>("GamesBody");
        _mvpPanel = Q<VisualElement>("MvpPanel");
        _mvpLogo = Q<VisualElement>("MvpLogo");
        _mvpName = Q<Label>("MvpName");
        _mvpTeam = Q<Label>("MvpTeam");
        _mvpVal = Q<Label>("MvpVal");

        _lbScorersBody = Q<VisualElement>("LbScorersBody");
        _lbAssistersBody = Q<VisualElement>("LbAssistersBody");
        _lbRatedBody = Q<VisualElement>("LbRatedBody");

        if (_loadingSpinner != null) _loadingSpinner.style.display = DisplayStyle.None;
    }

    protected override void LoadData()
    {
        base.LoadData();
        if (_myTeam == null || _season == null) return;

        var db = DatabaseManager.Instance;

        foreach (var sprite in Resources.LoadAll<Sprite>("Teams/Logos/120x120"))
            _logos[sprite.name] = sprite;

        foreach (var team in db.GetAllTeams())
            _teams[team.Id] = team;

        foreach (var player in db.Db.Table<PlayerDataDB>().ToList())
            _players[player.Id] = player;

        _seasonGames = db.GetGamesBySeason(_season.Id);

        _game = ResolveGame(db);
        if (_game == null) return;

        // Los resultados y los filtros son del día en que se jugó el partido.
        _dayDate = _game.MatchDate;
        _dayGames = _seasonGames
            .Where(g => g.IsPlayed == 1 && g.MatchDate == _dayDate)
            .OrderBy(g => g.CompetitionId)
            .ThenBy(g => g.Id)
            .ToList();

        _filterCompetition = 0;
        BuildFilterButtons();
    }

    private GameDataDB ResolveGame(DatabaseManager db)
    {
        if (MatchContext.HasGame)
        {
            var target = db.GetGamesBySeason(_season.Id).FirstOrDefault(g => g.Id == MatchContext.GameId);
            if (target != null) return target;
        }

        return db.GetLastPlayedGame(_season.Id, _myTeam.Id);
    }

    protected override void RegisterCallbacks()
    {
        _btnDashboard?.RegisterCallback<ClickEvent>(_ =>
        {
            PlayClick();
            ScreenManager.Instance.GoTo(GameScreen.Dashboard);
        });

        RegisterHandCursor(_btnDashboard);
    }

    protected override void Refresh()
    {
        if (_game == null)
        {
            if (_gamesBody != null)
            {
                var empty = new Label(LocalizationService.T("ui.game_results.sin_resultados"));
                empty.AddToClassList("no-games-text");
                _gamesBody.Add(empty);
            }
            return;
        }

        LoadDayStats();
        BuildLeaderEntries();

        RefreshHeader();
        RefreshGames();
        RefreshMvp();
        RefreshLeaderboards();
    }

    // ── FILTROS DINÁMICOS ──────────────────────────────────────────────────

    /// <summary>Solo se ofrecen las competiciones que han jugado ese día.</summary>
    private void BuildFilterButtons()
    {
        if (_filtersBar == null) return;

        _filtersBar.Clear();
        _filterButtons.Clear();

        AddFilterButton(0, LocalizationService.T("ui.game_results.filter_all"));

        foreach (int competitionId in _dayGames
                     .Select(g => g.CompetitionId)
                     .Distinct()
                     .OrderBy(id => id))
        {
            AddFilterButton(competitionId, CompetitionName(competitionId));
        }
    }

    private void AddFilterButton(int competitionId, string label)
    {
        var button = new Button { text = label };
        button.AddToClassList("gr-filter-btn");

        if (_filterCompetition == competitionId)
            button.AddToClassList("gr-filter-btn--active");

        button.RegisterCallback<ClickEvent>(_ =>
        {
            PlayClick();
            _filterCompetition = competitionId;
            UpdateFilterButtons();
            RefreshGames();
        });

        RegisterHandCursor(button);
        _filtersBar.Add(button);
        _filterButtons.Add((button, competitionId));
    }

    private void UpdateFilterButtons()
    {
        foreach (var entry in _filterButtons)
        {
            if (entry.CompetitionId == _filterCompetition)
                entry.Button.AddToClassList("gr-filter-btn--active");
            else
                entry.Button.RemoveFromClassList("gr-filter-btn--active");
        }
    }

    private string CompetitionName(int competitionId)
    {
        if (competitionId <= 0)
            return LocalizationService.T("ui.game_results.filter_preseason");

        var league = DatabaseManager.Instance.GetLeagueById(competitionId);
        return league?.ShortName ?? $"#{competitionId}";
    }

    // ── DATOS DEL DÍA ──────────────────────────────────────────────────────

    private void LoadDayStats()
    {
        _dayStats = _dayGames
            .SelectMany(g => DatabaseManager.Instance.GetPlayerMatchStats(g.Id))
            .ToList();
    }

    private void BuildLeaderEntries()
    {
        _leaderEntries = _dayStats
            .GroupBy(s => s.PlayerId)
            .Select(g => new LeaderEntry
            {
                PlayerId = g.Key,
                TeamId = g.First().TeamId,
                Goals = g.Sum(s => s.Goals),
                Assists = g.Sum(s => s.Assists),
                Rating = g.Average(s => s.Rating)
            })
            .ToList();
    }

    // ── HEADER ─────────────────────────────────────────────────────────────

    private void RefreshHeader()
    {
        SetLogo(_headerLogo, _myTeam);
        if (_headerTeam != null) _headerTeam.text = _myTeam.Name.ToUpperInvariant();
        if (_headerSubtitle != null)
            _headerSubtitle.text = LocalizationService.T("ui.game_results.header_subtitle");
        if (_headerManager != null)
            _headerManager.text = LocalizationService.T("ui.common.manager", _manager?.Name ?? "");
        if (_headerSeason != null)
            _headerSeason.text = LocalizationService.T("ui.dashboard.temporada",
                _season.YearStart, _season.YearEnd);

        var reference = _dayGames.FirstOrDefault() ?? _game;
        if (_headerDate != null) _headerDate.text = GameDayRules.FormatLongDate(reference.MatchDate);
        if (_headerGameDay != null)
            _headerGameDay.text = reference.GameType == DatabaseManager.GameTypePreseason
                ? LocalizationService.T("ui.dashboard.amistoso")
                : LocalizationService.T("ui.dashboard.jornada", reference.MatchDay);
    }

    // ── RESULTADOS DEL DÍA ─────────────────────────────────────────────────

    private void RefreshGames()
    {
        if (_gamesBody == null) return;
        _gamesBody.Clear();

        var visible = _filterCompetition == 0
            ? _dayGames
            : _dayGames.Where(g => g.CompetitionId == _filterCompetition).ToList();

        if (visible.Count == 0)
        {
            var empty = new Label(LocalizationService.T("ui.game_results.sin_competicion"));
            empty.AddToClassList("no-games-text");
            _gamesBody.Add(empty);
            return;
        }

        foreach (var game in visible)
            _gamesBody.Add(BuildGameCard(game));
    }

    private VisualElement BuildGameCard(GameDataDB game)
    {
        var home = TeamOf(game.HomeTeamId);
        var away = TeamOf(game.AwayTeamId);

        var card = new VisualElement();
        card.AddToClassList("game-card");
        if (game.Id == _game.Id) card.AddToClassList("my-game");

        var content = new VisualElement();
        content.AddToClassList("game-card-content");

        content.Add(BuildTeamSide(home, true));

        var score = new VisualElement();
        score.AddToClassList("game-card-score");

        var homeScore = new Label(game.HomeScore.ToString());
        homeScore.AddToClassList("game-card-score-home");
        if (game.HomeScore > game.AwayScore) homeScore.AddToClassList("winner");

        var separator = new Label("-");
        separator.AddToClassList("game-card-score-sep");

        var awayScore = new Label(game.AwayScore.ToString());
        awayScore.AddToClassList("game-card-score-away");
        if (game.AwayScore > game.HomeScore) awayScore.AddToClassList("winner");

        score.Add(homeScore);
        score.Add(separator);
        score.Add(awayScore);
        content.Add(score);

        content.Add(BuildTeamSide(away, false));

        card.Add(content);
        return card;
    }

    private VisualElement BuildTeamSide(TeamDataDB team, bool first)
    {
        var side = new VisualElement();
        side.AddToClassList("game-card-team-side");
        if (first) side.AddToClassList("team-side-first");

        var logo = new VisualElement();
        logo.AddToClassList("game-card-logo");
        SetLogo(logo, team);

        var name = new Label(team?.Name ?? "???");
        name.AddToClassList("game-card-team-name");
        if (team != null && team.Id == _myTeam.Id) name.AddToClassList("my-team");

        // Local: nombre → logo (pegado al marcador). Visitante: logo → nombre.
        if (first)
        {
            side.Add(name);
            side.Add(logo);
        }
        else
        {
            side.Add(logo);
            side.Add(name);
        }

        return side;
    }

    // ── MVP DEL DÍA ────────────────────────────────────────────────────────

    private void RefreshMvp()
    {
        var mvp = _leaderEntries
            .OrderByDescending(e => e.Rating)
            .ThenByDescending(e => e.Goals)
            .FirstOrDefault();

        if (mvp == null)
        {
            if (_mvpPanel != null) _mvpPanel.style.display = DisplayStyle.None;
            return;
        }

        if (_mvpPanel != null) _mvpPanel.style.display = DisplayStyle.Flex;

        var team = TeamOf(mvp.TeamId);
        SetLogo(_mvpLogo, team);

        if (_mvpName != null) _mvpName.text = PlayerName(mvp.PlayerId);
        if (_mvpTeam != null) _mvpTeam.text = team?.Name ?? "";
        if (_mvpVal != null) _mvpVal.text = mvp.Rating.ToString("F1");
    }

    // ── LÍDERES DEL DÍA ────────────────────────────────────────────────────

    private void RefreshLeaderboards()
    {
        FillLeaderboard(_lbScorersBody, _leaderEntries
            .OrderByDescending(e => e.Goals)
            .ThenByDescending(e => e.Assists)
            .ThenByDescending(e => e.Rating),
            e => e.Goals.ToString());

        FillLeaderboard(_lbAssistersBody, _leaderEntries
            .OrderByDescending(e => e.Assists)
            .ThenByDescending(e => e.Goals)
            .ThenByDescending(e => e.Rating),
            e => e.Assists.ToString());

        FillLeaderboard(_lbRatedBody, _leaderEntries
            .OrderByDescending(e => e.Rating)
            .ThenByDescending(e => e.Goals),
            e => e.Rating.ToString("F1"));
    }

    private sealed class LeaderEntry
    {
        public int PlayerId;
        public int TeamId;
        public int Goals;
        public int Assists;
        public double Rating;
    }

    private void FillLeaderboard(VisualElement body, IEnumerable<LeaderEntry> entries,
                                 System.Func<LeaderEntry, string> valueSelector)
    {
        if (body == null) return;
        body.Clear();

        var top = entries.Take(3).ToList();
        for (int i = 0; i < top.Count; i++)
        {
            var entry = top[i];
            var team = TeamOf(entry.TeamId);

            var row = new VisualElement();
            row.AddToClassList("lb-row");
            if (i == top.Count - 1) row.AddToClassList("lb-row--last");

            var rank = new Label((i + 1).ToString());
            rank.AddToClassList("lb-rank");
            if (i == 0) rank.AddToClassList("lb-first");

            var logo = new VisualElement();
            logo.AddToClassList("lb-logo");
            SetLogo(logo, team);

            var info = new VisualElement();
            info.AddToClassList("lb-info");

            var name = new Label(PlayerName(entry.PlayerId));
            name.AddToClassList("lb-name");

            var teamName = new Label(team?.Name ?? "");
            teamName.AddToClassList("lb-team");

            info.Add(name);
            info.Add(teamName);

            var value = new Label(valueSelector(entry));
            value.AddToClassList("lb-value");

            row.Add(rank);
            row.Add(logo);
            row.Add(info);
            row.Add(value);
            body.Add(row);
        }
    }

    // ── UTILIDADES ─────────────────────────────────────────────────────────

    private TeamDataDB TeamOf(int teamId)
        => _teams.TryGetValue(teamId, out var team) ? team : null;

    private string PlayerName(int playerId)
    {
        if (_players.TryGetValue(playerId, out var player))
            return string.IsNullOrEmpty(player.CommonName)
                ? $"{player.FirstName} {player.LastName}".Trim()
                : player.CommonName;

        var dbPlayer = DatabaseManager.Instance.GetPlayerById(playerId);
        return dbPlayer == null ? "" : $"{dbPlayer.FirstName} {dbPlayer.LastName}".Trim();
    }

    private void SetLogo(VisualElement element, TeamDataDB team)
    {
        if (element == null || team == null) return;
        string key = string.IsNullOrEmpty(team.Logo) ? team.Id.ToString() : team.Logo;
        if (_logos.TryGetValue(key, out var sprite))
            element.style.backgroundImage = new StyleBackground(sprite);
    }
}
