using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// F8 — Previa del partido (réplica de Tactical5 adaptada al fútbol): onces,
/// banquillos, bajas, jugadores clave, forma reciente y pronóstico.
/// </summary>
public class PreGameController : UIScreenController
{
    public override GameScreen ScreenId => GameScreen.PreGame;

    private Label _favorite;
    private Label _rivalry;
    private VisualElement _homeLogo;
    private VisualElement _awayLogo;
    private Label _homeName, _homeRecord, _homeConf;
    private Label _awayName, _awayRecord, _awayConf;
    private VisualElement _homeStreak, _awayStreak;
    private Label _date, _arena;
    private Label _homeAttack, _homeAttackRank, _homeDefense, _homeDefenseRank;
    private Label _awayAttack, _awayAttackRank, _awayDefense, _awayDefenseRank;
    private VisualElement _homeAbsences, _awayAbsences;
    private Label _homeAbsencesTitle, _awayAbsencesTitle;
    private VisualElement _homeStarters, _awayStarters;
    private VisualElement _keyRow1, _keyRow2, _keyRow3, _keyRow4;
    private Button _btnBack, _btnGoMatch;
    private Label _headerTitleType;

    private GameDataDB _game;
    private TeamDataDB _home, _away;
    private LeagueDataDB _league;
    private List<GameDataDB> _regularGames = new();
    private List<TeamDataDB> _leagueTeams = new();
    private readonly Dictionary<string, Sprite> _logos = new();
    private readonly Dictionary<int, (int TeamId, int Goals, int Assists)> _seasonStats = new();

    protected override void CacheReferences()
    {
        _favorite = Q<Label>("MPVFavorite");
        _rivalry = Q<Label>("MPVRivalry");
        _homeLogo = Q<VisualElement>("MPVHomeLogo");
        _awayLogo = Q<VisualElement>("MPVAwayLogo");
        _homeName = Q<Label>("MPVHomeName");
        _homeRecord = Q<Label>("MPVHomeRecord");
        _homeConf = Q<Label>("MPVHomeConf");
        _awayName = Q<Label>("MPVAwayName");
        _awayRecord = Q<Label>("MPVAwayRecord");
        _awayConf = Q<Label>("MPVAwayConf");
        _homeStreak = Q<VisualElement>("MPVHomeLast10");
        _awayStreak = Q<VisualElement>("MPVAwayLast10");
        _date = Q<Label>("MPVDate");
        _arena = Q<Label>("MPVArena");
        _homeAttack = Q<Label>("MPVHomeOff"); _homeAttackRank = Q<Label>("MPVHomeOffRank");
        _homeDefense = Q<Label>("MPVHomeDef"); _homeDefenseRank = Q<Label>("MPVHomeDefRank");
        _awayAttack = Q<Label>("MPVAwayOff"); _awayAttackRank = Q<Label>("MPVAwayOffRank");
        _awayDefense = Q<Label>("MPVAwayDef"); _awayDefenseRank = Q<Label>("MPVAwayDefRank");
        _homeAbsences = Q<VisualElement>("MPVHomeBajas");
        _awayAbsences = Q<VisualElement>("MPVAwayBajas");
        _homeAbsencesTitle = Q<Label>("MPVHomeBajasTitle");
        _awayAbsencesTitle = Q<Label>("MPVAwayBajasTitle");
        _homeStarters = Q<VisualElement>("MPVHomeStarters");
        _awayStarters = Q<VisualElement>("MPVAwayStarters");
        _keyRow1 = Q<VisualElement>("MPVKeyRow1");
        _keyRow2 = Q<VisualElement>("MPVKeyRow2");
        _keyRow3 = Q<VisualElement>("MPVKeyRow3");
        _keyRow4 = Q<VisualElement>("MPVKeyRow4");
        _btnBack = Q<Button>("BtnBack");
        _btnGoMatch = Q<Button>("BtnGoMatch");
        _headerTitleType = Q<Label>("HeaderTitleType");
    }

    protected override void LoadData()
    {
        base.LoadData();
        if (_myTeam == null || _season == null) return;

        var db = DatabaseManager.Instance;

        foreach (var sprite in Resources.LoadAll<Sprite>("Teams/Logos/120x120"))
            _logos[sprite.name] = sprite;

        _league = db.GetLeagueById(_myTeam.LeagueId);
        _regularGames = db.GetGamesByType(_season.Id, DatabaseManager.GameTypeRegular);
        _leagueTeams = db.GetTeamsByLeague(_myTeam.LeagueId)
                         .Where(t => t.IsFilial == 0)
                         .ToList();

        _seasonStats.Clear();
        foreach (var stat in db.Db.Table<PlayerSeasonStatsDB>()
                     .Where(st => st.SeasonYear == _season.YearStart).ToList())
            _seasonStats[stat.PlayerId] = (stat.TeamId, stat.Goals, stat.Assists);

        _game = ResolveGame(db);
        if (_game == null) return;

        _home = db.GetTeamById(_game.HomeTeamId);
        _away = db.GetTeamById(_game.AwayTeamId);
    }

    private GameDataDB ResolveGame(DatabaseManager db)
    {
        // Prioridad: el partido pendiente de hoy; si no, el seleccionado por el
        // contexto (entrada directa) o el siguiente del calendario.
        var today = db.GetMatchOnDate(_season.Id, _myTeam.Id, _season.CurrentDate);
        if (today != null) return today;

        if (MatchContext.HasGame)
        {
            var target = db.GetGamesBySeason(_season.Id).FirstOrDefault(g => g.Id == MatchContext.GameId);
            if (target != null) return target;
        }

        return db.GetNextGame(_season.Id, _myTeam.Id);
    }

    protected override void RegisterCallbacks()
    {
        _btnBack?.RegisterCallback<ClickEvent>(_ =>
        {
            PlayClick();
            ScreenManager.Instance.GoTo(GameScreen.Dashboard);
        });

        _btnGoMatch?.RegisterCallback<ClickEvent>(_ =>
        {
            PlayClick();
            if (!DashboardController.RequestPlayMatch() && _favorite != null)
                _favorite.text = LocalizationService.T("ui.pre_game.error_simular");
        });

        RegisterHandCursor(_btnBack);
        RegisterHandCursor(_btnGoMatch);
    }

    protected override void Refresh()
    {
        if (_game == null || _home == null || _away == null)
        {
            if (_btnGoMatch != null) _btnGoMatch.SetEnabled(false);
            if (_favorite != null) _favorite.text = LocalizationService.T("ui.pre_game.sin_partido");
            return;
        }

        if (_btnGoMatch != null) _btnGoMatch.SetEnabled(true);
        if (_headerTitleType != null)
            _headerTitleType.text = CompetitionLabel();

        FillTeam(_home, _homeLogo, _homeName, _homeRecord, _homeConf, _homeStreak,
            _homeAttack, _homeAttackRank, _homeDefense, _homeDefenseRank,
            _homeAbsences, _homeAbsencesTitle, _homeStarters);

        FillTeam(_away, _awayLogo, _awayName, _awayRecord, _awayConf, _awayStreak,
            _awayAttack, _awayAttackRank, _awayDefense, _awayDefenseRank,
            _awayAbsences, _awayAbsencesTitle, _awayStarters);

        if (_date != null) _date.text = GameDayRules.FormatLongDate(_game.MatchDate);
        if (_arena != null) _arena.text = _home.StadiumName;

        FillForecast();
        FillKeyPlayers();
        if (_rivalry != null) _rivalry.style.display = DisplayStyle.None;
    }

    // ── EQUIPOS ────────────────────────────────────────────────────────────

    private void FillTeam(TeamDataDB team, VisualElement logo, Label name, Label record,
                          Label position, VisualElement streak,
                          Label attack, Label attackRank, Label defense, Label defenseRank,
                          VisualElement absences, Label absencesTitle,
                          VisualElement starters)
    {
        SetLogo(logo, team);
        if (name != null) name.text = team.Name.ToUpperInvariant();
        if (record != null) record.text = RecordText(team.Id);
        if (position != null) position.text = PositionText(team.Id);

        if (attack != null) attack.text = team.Attack.ToString();
        if (defense != null) defense.text = team.Defense.ToString();
        if (attackRank != null)
            attackRank.text = RankText(t => t.Attack, team.Id);
        if (defenseRank != null)
            defenseRank.text = RankText(t => t.Defense, team.Id);

        FillStreak(streak, team.Id);
        FillAbsences(absences, absencesTitle, team.Id);
        FillLineup(team, starters);
    }

    private void FillLineup(TeamDataDB team, VisualElement starters)
    {
        if (starters == null) return;
        starters.Clear();

        var squad = DatabaseManager.Instance.GetPlayersByTeam(team.Id);
        var lineup = LineupBuilder.Build(team, squad);

        for (int i = 0; i < lineup.Starters.Count; i++)
        {
            var slot = i < lineup.Slots.Count
                ? lineup.Slots[i]
                : (PlayerPosition)lineup.Starters[i].MainPosition;
            starters.Add(PlayerRow(lineup.Starters[i], slot));
        }

        if (lineup.Starters.Count == 0)
        {
            var empty = new Label(LocalizationService.T("ui.pre_game.sin_jugadores"));
            empty.AddToClassList("mpv-starter-card--empty");
            starters.Add(empty);
        }
    }

    /// <summary>Fila de lista del once (mismo formato que el antiguo banquillo).</summary>
    private static VisualElement PlayerRow(PlayerDataDB player, PlayerPosition slot)
    {
        var card = new VisualElement();
        card.AddToClassList("mpv-bench-card");

        var pos = new Label(PlayerPositionNames.ShortName((int)slot));
        pos.AddToClassList("mpv-bench-pos");

        var name = new Label(ShortName(player));
        name.AddToClassList("mpv-bench-name");

        var overall = new Label(player.Overall.ToString());
        overall.AddToClassList("mpv-bench-ovr");

        card.Add(pos);
        card.Add(name);
        card.Add(overall);
        return card;
    }

    private void FillAbsences(VisualElement container, Label title, int teamId)
    {
        if (container == null) return;
        container.Clear();

        var injured = DatabaseManager.Instance.GetPlayersByTeam(teamId)
            .Where(p => p.InjuryDays > 0)
            .OrderByDescending(p => p.InjuryDays)
            .ToList();

        if (title != null)
            title.text = injured.Count == 0
                ? LocalizationService.T("ui.pre_game.sin_bajas")
                : LocalizationService.T("ui.pre_game.num_bajas", injured.Count);

        foreach (var player in injured)
        {
            var item = new VisualElement();
            item.AddToClassList("mpv-baja-item");

            var info = new VisualElement();
            info.AddToClassList("mpv-baja-info");

            var name = new Label(ShortName(player));
            name.AddToClassList("mpv-baja-name");

            var position = new Label(PlayerPositionNames.ShortName(player.MainPosition));
            position.AddToClassList("mpv-baja-pos");

            info.Add(name);
            info.Add(position);

            var status = new Label(LocalizationService.T("ui.pre_game.baja_dias", player.InjuryDays));
            status.AddToClassList("mpv-baja-status");
            status.AddToClassList("mpv-baja-status--out");

            item.Add(info);
            item.Add(status);
            container.Add(item);
        }
    }

    // ── FORMA Y ESTADÍSTICAS ───────────────────────────────────────────────

    private void FillStreak(VisualElement container, int teamId)
    {
        if (container == null) return;
        container.Clear();

        var last = _regularGames
            .Where(g => g.IsPlayed == 1 && (g.HomeTeamId == teamId || g.AwayTeamId == teamId))
            .OrderByDescending(g => g.MatchDate)
            .Take(5)
            .ToList();

        foreach (var game in last.Reverse<GameDataDB>())
        {
            bool home = game.HomeTeamId == teamId;
            int mine = home ? game.HomeScore : game.AwayScore;
            int theirs = home ? game.AwayScore : game.HomeScore;

            var dot = new VisualElement();
            dot.AddToClassList("mpv-streak-dot");
            if (mine > theirs) dot.AddToClassList("mpv-streak-dot--win");
            else if (mine < theirs) dot.AddToClassList("mpv-streak-dot--loss");
            else dot.AddToClassList("mpv-streak-dot--draw");
            container.Add(dot);
        }
    }

    private string RecordText(int teamId)
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
        return $"{won}V · {drawn}E · {lost}D";
    }

    private string PositionText(int teamId)
    {
        if (_leagueTeams.Count == 0) return "";

        var table = LeagueTable.Build(_leagueTeams.Select(t => t.Id), _regularGames);
        int position = LeagueTable.PositionOf(table, teamId);
        if (position <= 0) return "";

        return LocalizationService.T("ui.pre_game.posicion", position, _leagueTeams.Count);
    }

    private string RankText(System.Func<TeamDataDB, int> selector, int teamId)
    {
        var ordered = _leagueTeams.OrderByDescending(selector).ThenBy(t => t.Id).ToList();
        int index = ordered.FindIndex(t => t.Id == teamId);
        return index >= 0
            ? LocalizationService.T("ui.pre_game.rank", index + 1, ordered.Count)
            : "";
    }

    // ── PRONÓSTICO Y JUGADORES CLAVE ───────────────────────────────────────

    private void FillForecast()
    {
        if (_favorite == null) return;

        var preview = MatchupPreview.For(_home, _away);
        string favorite = preview.HomeWin >= preview.AwayWin && preview.HomeWin >= preview.Draw
            ? _home.Name
            : preview.AwayWin >= preview.Draw ? _away.Name : LocalizationService.T("ui.pre_game.empate");

        _favorite.text = LocalizationService.T("ui.pre_game.favorito",
            favorite,
            Mathf.RoundToInt(preview.HomeWin * 100f),
            Mathf.RoundToInt(preview.Draw * 100f),
            Mathf.RoundToInt(preview.AwayWin * 100f));
    }

    private void FillKeyPlayers()
    {
        var rows = new[] { _keyRow1, _keyRow2, _keyRow3, _keyRow4 };
        foreach (var row in rows) row?.Clear();

        // Dos filas: goles y asistencias. Cada una enfrenta al mejor de cada equipo.
        FillKeyRow(_keyRow1, "ui.pre_game.key_goals", stat => stat.Goals);
        FillKeyRow(_keyRow2, "ui.pre_game.key_assists", stat => stat.Assists);

        SetVisible(_keyRow3, false);
        SetVisible(_keyRow4, false);
    }

    private void FillKeyRow(VisualElement row, string labelKey,
                            System.Func<(int TeamId, int Goals, int Assists), int> selector)
    {
        if (row == null) return;

        var home = BestBy(_home, selector);
        var away = BestBy(_away, selector);

        AddKeySide(row, home.Player, _home, home.Value, false);

        var label = new Label(LocalizationService.T(labelKey));
        label.AddToClassList("mpv-key-label");
        row.Add(label);

        AddKeySide(row, away.Player, _away, away.Value, true);
    }

    private (PlayerDataDB Player, int Value) BestBy(
        TeamDataDB team, System.Func<(int TeamId, int Goals, int Assists), int> selector)
    {
        var players = DatabaseManager.Instance.GetPlayersByTeam(team.Id);
        if (players.Count == 0) return (null, 0);

        PlayerDataDB best = null;
        int bestValue = -1;

        foreach (var player in players)
        {
            if (!_seasonStats.TryGetValue(player.Id, out var stat)) continue;
            int value = selector(stat);
            if (value > bestValue)
            {
                bestValue = value;
                best = player;
            }
        }

        // Sin estadísticas todavía: se usa el mejor jugador de la plantilla con 0.
        if (best == null)
        {
            best = players.OrderByDescending(p => p.Overall).First();
            bestValue = 0;
        }

        return (best, bestValue);
    }

    private void AddKeySide(VisualElement row, PlayerDataDB player, TeamDataDB team,
                            int value, bool rightSide)
    {
        if (row == null || player == null) return;

        var block = new VisualElement();
        block.AddToClassList("mpv-key-player");
        if (rightSide) block.AddToClassList("mpv-key-player--right");

        var photo = new VisualElement();
        photo.AddToClassList("mpv-key-photo");
        SetLogo(photo, team);

        var name = new Label(ShortName(player));
        name.AddToClassList("mpv-key-name");

        var stat = new Label(value.ToString());
        stat.AddToClassList("mpv-key-stat");

        if (rightSide)
        {
            block.Add(stat);
            block.Add(name);
            block.Add(photo);
        }
        else
        {
            block.Add(photo);
            block.Add(name);
            block.Add(stat);
        }

        row.Add(block);
    }

    // ── UTILIDADES ─────────────────────────────────────────────────────────

    private string CompetitionLabel()
    {
        if (_game == null) return _league?.Name ?? "";
        if (_game.GameType == DatabaseManager.GameTypePreseason)
            return LocalizationService.T("ui.dashboard.amistoso");
        return _league?.Name ?? "";
    }

    private void SetLogo(VisualElement element, TeamDataDB team)
    {
        if (element == null || team == null) return;
        string key = string.IsNullOrEmpty(team.Logo) ? team.Id.ToString() : team.Logo;
        if (_logos.TryGetValue(key, out var sprite))
            element.style.backgroundImage = new StyleBackground(sprite);
    }

    private static string ShortName(PlayerDataDB player)
    {
        if (player == null) return "";
        if (!string.IsNullOrEmpty(player.CommonName)) return player.CommonName;
        return $"{player.FirstName} {player.LastName}".Trim();
    }
}
