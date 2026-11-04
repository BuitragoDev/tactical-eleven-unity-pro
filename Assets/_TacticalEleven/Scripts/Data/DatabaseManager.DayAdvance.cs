using UnityEngine;

/// <summary>Resultado de avanzar un día.</summary>
public enum DayAdvanceResult
{
    /// <summary>No hay partida o temporada activa.</summary>
    NoSeason,
    /// <summary>Se avanzó a un día sin partido.</summary>
    Advanced,
    /// <summary>Se llegó a un día de partido: el avance se detiene.</summary>
    MatchDay,
    /// <summary>Se superó el último partido del calendario.</summary>
    SeasonOver
}

/// <summary>
/// F7 — Bucle diario mínimo: avanzar la fecha de la temporada y detectar el día
/// de partido. La simulación de partidos llega en F8; aquí solo se mueve el reloj.
/// </summary>
public partial class DatabaseManager
{
    /// <summary>Partido sin jugar del equipo en la fecha indicada (o null).</summary>
    public GameDataDB GetMatchOnDate(int seasonId, int teamId, string date)
        => GameDayRules.FindMatch(GetGamesForTeam(seasonId, teamId), teamId, date);

    /// <summary>
    /// Avanza un día la temporada. Si la fecha actual ya es día de partido, no
    /// avanza: devuelve <see cref="DayAdvanceResult.MatchDay"/> con el partido.
    /// Si el día siguiente tiene partido, se detiene en él.
    /// </summary>
    public DayAdvanceResult AdvanceDay(SeasonDataDB season, int teamId, out GameDataDB match)
    {
        match = null;
        if (!EnsureDb() || season == null) return DayAdvanceResult.NoSeason;

        var games = GetGamesForTeam(season.Id, teamId);

        // No se puede avanzar si hoy hay un partido pendiente: hay que jugarlo.
        match = GameDayRules.FindMatch(games, teamId, season.CurrentDate);
        if (match != null) return DayAdvanceResult.MatchDay;

        if (!GameDayRules.TryNextDate(season.CurrentDate, out string nextDate))
            return DayAdvanceResult.NoSeason;

        season.CurrentDate = nextDate;
        season.CurrentMatchDay = CalendarHelper.TryParse(nextDate, out var date)
            ? CalendarHelper.DateToGameDay(season.YearStart, date)
            : season.CurrentMatchDay;

        var firstLeague = GetFirstLeagueGame(season.Id);
        season.Phase = GameDayRules.PhaseFor(nextDate, firstLeague?.MatchDate);
        UpdateSeason(season);

        match = GameDayRules.FindMatch(games, teamId, nextDate);
        if (match != null) return DayAdvanceResult.MatchDay;

        string lastDate = null;
        foreach (var game in games)
            if (lastDate == null || string.CompareOrdinal(game.MatchDate, lastDate) > 0)
                lastDate = game.MatchDate;

        if (!string.IsNullOrEmpty(lastDate) && string.CompareOrdinal(nextDate, lastDate) > 0)
            return DayAdvanceResult.SeasonOver;

        return DayAdvanceResult.Advanced;
    }

    /// <summary>Último partido jugado del equipo (o null).</summary>
    public GameDataDB GetLastPlayedGame(int seasonId, int teamId)
    {
        GameDataDB last = null;
        foreach (var game in GetGamesForTeam(seasonId, teamId))
        {
            if (game.IsPlayed != 1) continue;
            if (last == null || string.CompareOrdinal(game.MatchDate, last.MatchDate) > 0)
                last = game;
        }
        return last;
    }
}
