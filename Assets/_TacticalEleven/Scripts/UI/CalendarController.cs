using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// S1 — Calendario de la temporada (réplica de Tactical5 adaptada al fútbol):
/// rejilla mensual con todos los partidos (ligas, Europa y Copa) y detalle del
/// día seleccionado.
/// </summary>
public class CalendarController : UIScreenController
{
    public override GameScreen ScreenId => GameScreen.Calendar;

    private static readonly string[] Months =
    {
        "", "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO",
        "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"
    };

    private Button _btnPrevMonth;
    private Button _btnNextMonth;
    private Label _currentMonth;
    private Label _subtitle;
    private VisualElement _calendarDays;
    private Label _selectedDayTitle;
    private VisualElement _selectedDayGames;
    private VisualElement _gamesViewport;
    private VisualElement _scrollTrack;
    private VisualElement _scrollThumb;
    private CustomScrollbar.Handle _scrollbar;
    private Label _noGamesText;

    private readonly List<GameDataDB> _games = new();
    private readonly Dictionary<string, List<GameDataDB>> _gamesByDate = new();
    private readonly Dictionary<int, TeamDataDB> _teams = new();
    private readonly Dictionary<string, Sprite> _logos = new();
    private readonly Dictionary<string, Sprite> _logosLarge = new();
    private readonly Dictionary<DateTime, VisualElement> _dayCells = new();

    private DateTime _currentMonthDate;
    private DateTime? _selectedDate;

    protected override void CacheReferences()
    {
        _btnPrevMonth = Q<Button>("BtnPrevMonth");
        _btnNextMonth = Q<Button>("BtnNextMonth");
        _currentMonth = Q<Label>("CurrentMonth");
        _subtitle = Q<Label>("CalendarSubtitle");
        _calendarDays = Q<VisualElement>("CalendarDays");
        _selectedDayTitle = Q<Label>("SelectedDayTitle");
        _selectedDayGames = Q<VisualElement>("SelectedDayGames");
        _gamesViewport = Q<VisualElement>("SelectedDayGamesViewport");
        _scrollTrack = Q<VisualElement>("SelectedDayScrollTrack");
        _scrollThumb = Q<VisualElement>("SelectedDayScrollThumb");
        _noGamesText = Q<Label>("NoGamesText");
    }

    protected override void LoadData()
    {
        base.LoadData();
        if (_myTeam == null || _season == null) return;

        var db = DatabaseManager.Instance;

        _games.Clear();
        _games.AddRange(db.GetGamesBySeason(_season.Id));

        _gamesByDate.Clear();
        foreach (var game in _games)
        {
            if (!_gamesByDate.TryGetValue(game.MatchDate, out var list))
                _gamesByDate[game.MatchDate] = list = new List<GameDataDB>();
            list.Add(game);
        }

        _teams.Clear();
        foreach (var team in db.GetAllTeams())
            _teams[team.Id] = team;

        foreach (var sprite in Resources.LoadAll<Sprite>("Teams/Logos/64x64"))
            _logos[sprite.name] = sprite;

        foreach (var sprite in Resources.LoadAll<Sprite>("Teams/Logos/120x120"))
            _logosLarge[sprite.name] = sprite;

        var current = CalendarHelper.TryParse(_season.CurrentDate, out var date)
            ? date
            : new DateTime(_season.YearStart, 7, 15);

        _currentMonthDate = new DateTime(current.Year, current.Month, 1);
        _selectedDate = current.Date;

        var league = db.GetLeagueById(_myTeam.LeagueId);
        if (_subtitle != null)
            _subtitle.text = LocalizationService.T("ui.calendar.subtitle",
                _season.YearStart, _season.YearEnd, league?.ShortName ?? "");
    }

    protected override void RegisterCallbacks()
    {
        SidebarController.Attach(_root, ScreenId);
        HeaderController.Attach(_root);

        _scrollbar = CustomScrollbar.Bind(_gamesViewport, _selectedDayGames,
            _scrollTrack, _scrollThumb);

        _btnPrevMonth?.RegisterCallback<ClickEvent>(_ =>
        {
            PlayClick();
            _currentMonthDate = _currentMonthDate.AddMonths(-1);
            BuildCalendar();
        });

        _btnNextMonth?.RegisterCallback<ClickEvent>(_ =>
        {
            PlayClick();
            _currentMonthDate = _currentMonthDate.AddMonths(1);
            BuildCalendar();
        });

        RegisterHandCursor(_btnPrevMonth);
        RegisterHandCursor(_btnNextMonth);

        var icon = Q<VisualElement>("CalendarScreenIcon");
        if (icon != null)
        {
            var texture = Resources.Load<Texture2D>("Icons/calendario");
            if (texture != null) icon.style.backgroundImage = new StyleBackground(texture);
        }
    }

    protected override void Refresh()
    {
        if (_gameMissing()) return;

        BuildCalendar();
        if (_selectedDate.HasValue) ShowDay(_selectedDate.Value);
    }

    private bool _gameMissing() => _myTeam == null || _season == null || _calendarDays == null;

    // ── REJILLA MENSUAL ────────────────────────────────────────────────────

    private void BuildCalendar()
    {
        if (_calendarDays == null) return;

        _calendarDays.Clear();
        _dayCells.Clear();

        int year = _currentMonthDate.Year;
        int month = _currentMonthDate.Month;
        if (_currentMonth != null) _currentMonth.text = $"{Months[month]} {year}";

        int daysInMonth = DateTime.DaysInMonth(year, month);
        int firstDay = (int)new DateTime(year, month, 1).DayOfWeek;
        if (firstDay == 0) firstDay = 7;
        firstDay--; // lunes = 0

        int totalCells = ((firstDay + daysInMonth + 6) / 7) * 7;

        for (int i = 0; i < totalCells; i++)
        {
            var cell = new VisualElement();
            cell.AddToClassList("calendar-day-cell");

            int dayNumber = i - firstDay + 1;
            if (dayNumber < 1 || dayNumber > daysInMonth)
            {
                cell.AddToClassList("calendar-day-cell--empty");
                _calendarDays.Add(cell);
                continue;
            }

            var date = new DateTime(year, month, dayNumber);
            string dbDate = CalendarHelper.ToDb(date);
            _gamesByDate.TryGetValue(dbDate, out var dayGames);
            dayGames ??= new List<GameDataDB>();

            bool isToday = _season != null && _season.CurrentDate == dbDate;
            bool isSelected = _selectedDate.HasValue && _selectedDate.Value.Date == date;
            bool isMyGame = dayGames.Any(g => g.HomeTeamId == _myTeam.Id || g.AwayTeamId == _myTeam.Id);

            if (isToday) cell.AddToClassList("calendar-day-cell--today");
            if (isMyGame) cell.AddToClassList("calendar-day-cell--my-game");
            if (isSelected) cell.AddToClassList("calendar-day-cell--selected");

            var number = new Label(dayNumber.ToString());
            number.AddToClassList("calendar-day-number");
            number.AddToClassList("calendar-day-number--top-right");
            cell.Add(number);

            if (isMyGame)
            {
                var myGame = dayGames.First(g =>
                    g.HomeTeamId == _myTeam.Id || g.AwayTeamId == _myTeam.Id);
                int rivalId = myGame.HomeTeamId == _myTeam.Id
                    ? myGame.AwayTeamId
                    : myGame.HomeTeamId;

                var crest = new VisualElement();
                crest.AddToClassList("calendar-day-logo");
                SetLargeLogo(crest, TeamOf(rivalId));
                cell.Add(crest);
            }

            _dayCells[date] = cell;

            var captured = date;
            cell.RegisterCallback<ClickEvent>(_ =>
            {
                PlayClick();

                if (_selectedDate.HasValue
                    && _dayCells.TryGetValue(_selectedDate.Value.Date, out var previous))
                    previous.RemoveFromClassList("calendar-day-cell--selected");

                _selectedDate = captured;
                cell.AddToClassList("calendar-day-cell--selected");
                ShowDay(captured);
            });
            RegisterHandCursor(cell);

            _calendarDays.Add(cell);
        }
    }

    // ── DETALLE DEL DÍA ────────────────────────────────────────────────────

    private void ShowDay(DateTime date)
    {
        if (_selectedDayGames == null) return;

        _selectedDayGames.Clear();
        string dbDate = CalendarHelper.ToDb(date);
        _gamesByDate.TryGetValue(dbDate, out var dayGames);
        dayGames ??= new List<GameDataDB>();

        if (_selectedDayTitle != null)
            _selectedDayTitle.text = $"{date.Day:00} / {Months[date.Month]} / {date.Year}";

        bool hasGames = dayGames.Count > 0;
        SetVisible(_noGamesText, !hasGames);
        CustomScrollbar.Reset(_scrollbar);

        if (!hasGames) return;

        foreach (var game in dayGames
                     .OrderBy(g => g.CompetitionId)
                     .ThenBy(g => g.Id))
        {
            _selectedDayGames.Add(BuildGameRow(game));
        }

        CustomScrollbar.Refresh(_scrollbar);
    }

    private VisualElement BuildGameRow(GameDataDB game)
    {
        var row = new VisualElement();
        row.AddToClassList("selected-game-row");

        bool isMine = game.HomeTeamId == _myTeam.Id || game.AwayTeamId == _myTeam.Id;
        if (isMine) row.AddToClassList("selected-game-row--my-game");
        if (game.IsPlayed == 1) row.AddToClassList("selected-game-row--played");

        var home = TeamOf(game.HomeTeamId);
        var away = TeamOf(game.AwayTeamId);

        row.Add(BuildLogo(home, false));
        row.Add(BuildAbbreviation(home, false));

        var score = new Label(game.IsPlayed == 1
            ? $"{game.HomeScore} - {game.AwayScore}"
            : LocalizationService.T("ui.calendar.vs"));
        score.AddToClassList("selected-game-score");
        row.Add(score);

        row.Add(BuildAbbreviation(away, true));
        row.Add(BuildLogo(away, true));

        var type = new Label(CompetitionShort(game.CompetitionId));
        type.AddToClassList("selected-game-type");
        row.Add(type);

        return row;
    }

    private VisualElement BuildLogo(TeamDataDB team, bool away)
    {
        var logo = new VisualElement();
        logo.AddToClassList("selected-game-logo");
        if (away) logo.AddToClassList("selected-game-logo--away");
        SetLogo(logo, team);
        return logo;
    }

    private static Label BuildAbbreviation(TeamDataDB team, bool away)
    {
        var label = new Label(team?.ShortName ?? "???");
        label.AddToClassList("selected-game-abbr");
        if (away) label.AddToClassList("selected-game-abbr--away");
        return label;
    }

    // ── UTILIDADES ─────────────────────────────────────────────────────────

    private TeamDataDB TeamOf(int teamId)
        => _teams.TryGetValue(teamId, out var team) ? team : null;

    private string CompetitionShort(int competitionId)
    {
        if (competitionId <= 0) return LocalizationService.T("ui.calendar.amistoso");
        var league = DatabaseManager.Instance.GetLeagueById(competitionId);
        return league?.ShortName ?? competitionId.ToString();
    }

    private void SetLogo(VisualElement element, TeamDataDB team)
    {
        if (element == null || team == null) return;
        string key = string.IsNullOrEmpty(team.Logo) ? team.Id.ToString() : team.Logo;
        if (_logos.TryGetValue(key, out var sprite))
            element.style.backgroundImage = new StyleBackground(sprite);
    }

    private void SetLargeLogo(VisualElement element, TeamDataDB team)
    {
        if (element == null || team == null) return;
        string key = string.IsNullOrEmpty(team.Logo) ? team.Id.ToString() : team.Logo;
        if (_logosLarge.TryGetValue(key, out var sprite))
            element.style.backgroundImage = new StyleBackground(sprite);
        else if (_logos.TryGetValue(key, out var small))
            element.style.backgroundImage = new StyleBackground(small);
    }
}
