using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// F8 — Resultados del partido (réplica de Tactical5 adaptada al fútbol):
/// resto de la jornada, MVP y líderes de goleadores/asistentes/notas.
/// </summary>
public class GameResultsController : UIScreenController
{
    public override GameScreen ScreenId => GameScreen.GameResults;

    private VisualElement _headerLogo;
    private Label _headerTeam, _headerSubtitle, _headerManager;
    private Label _headerSeason, _headerDate, _headerGameDay;
    private Button _btnDashboard;
    private VisualElement _loadingSpinner;

    private VisualElement _gamesBody;
    private VisualElement _mvpPanel;
    private VisualElement _mvpLogo;
    private Label _mvpName, _mvpTeam, _mvpGoals, _mvpAssists, _mvpVal;

    private VisualElement _lbScorersBody, _lbAssistersBody, _lbRatedBody;

    private GameDataDB _game;
    private List<GameDataDB> _matchdayGames = new();
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

        _gamesBody = Q<VisualElement>("GamesBody");
        _mvpPanel = Q<VisualElement>("MvpPanel");
        _mvpLogo = Q<VisualElement>("MvpLogo");
        _mvpName = Q<Label>("MvpName");
        _mvpTeam = Q<Label>("MvpTeam");
        _mvpGoals = Q<Label>("MvpGoals");
        _mvpAssists = Q<Label>("MvpAssists");
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

        _game = ResolveGame(db);
        if (_game == null) return;

        _matchdayGames = db.GetGamesBySeason(_season.Id)
            .Where(g => g.IsPlayed == 1
                        && g.MatchDay == _game.MatchDay
                        && g.GameType == _game.GameType)
            .OrderBy(g => g.Id)
            .ToList();
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

        RefreshHeader();
        RefreshGames();
        RefreshMvp();
        RefreshLeaderboards();
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
        if (_headerDate != null) _headerDate.text = GameDayRules.FormatLongDate(_game.MatchDate);
        if (_headerGameDay != null)
            _headerGameDay.text = _game.GameType == DatabaseManager.GameTypePreseason
                ? LocalizationService.T("ui.dashboard.amistoso")
                : LocalizationService.T("ui.dashboard.jornada", _game.MatchDay);
    }

    // ── RESULTADOS DE LA JORNADA ───────────────────────────────────────────

    private void RefreshGames()
    {
        if (_gamesBody == null) return;
        _gamesBody.Clear();

        foreach (var game in _matchdayGames)
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

        side.Add(logo);
        side.Add(name);
        return side;
    }

    // ── MVP ────────────────────────────────────────────────────────────────

    private void RefreshMvp()
    {
        var stats = DatabaseManager.Instance.GetPlayerMatchStats(_game.Id)
            .OrderByDescending(s => s.Rating)
            .ThenByDescending(s => s.Goals)
            .FirstOrDefault();

        if (stats == null)
        {
            if (_mvpPanel != null) _mvpPanel.style.display = DisplayStyle.None;
            return;
        }

        if (_mvpPanel != null) _mvpPanel.style.display = DisplayStyle.Flex;

        var team = TeamOf(stats.TeamId);
        SetLogo(_mvpLogo, team);

        if (_mvpName != null) _mvpName.text = PlayerName(stats.PlayerId);
        if (_mvpTeam != null) _mvpTeam.text = team?.Name ?? "";
        if (_mvpGoals != null) _mvpGoals.text = stats.Goals.ToString();
        if (_mvpAssists != null) _mvpAssists.text = stats.Assists.ToString();
        if (_mvpVal != null) _mvpVal.text = stats.Rating.ToString("F1");
    }

    // ── LÍDERES ────────────────────────────────────────────────────────────

    private void RefreshLeaderboards()
    {
        var all = _matchdayGames
            .SelectMany(g => DatabaseManager.Instance.GetPlayerMatchStats(g.Id))
            .ToList();

        var byPlayer = all
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

        FillLeaderboard(_lbScorersBody, byPlayer
            .OrderByDescending(e => e.Goals).ThenByDescending(e => e.Assists),
            e => e.Goals.ToString());

        FillLeaderboard(_lbAssistersBody, byPlayer
            .OrderByDescending(e => e.Assists).ThenByDescending(e => e.Goals),
            e => e.Assists.ToString());

        FillLeaderboard(_lbRatedBody, byPlayer
            .OrderByDescending(e => e.Rating),
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

        var top = entries.Take(5).ToList();
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
