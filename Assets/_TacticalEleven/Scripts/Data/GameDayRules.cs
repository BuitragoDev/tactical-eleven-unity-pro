using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// F7 — Reglas puras del avance de día: fecha siguiente, fase de la temporada,
/// día de partido y formato de fecha. Sin Unity ni base de datos.
/// </summary>
public static class GameDayRules
{
    public const string PhasePreseason = "preseason";
    public const string PhaseRegular = "regular";

    private static readonly string[] Months =
    {
        "ENE", "FEB", "MAR", "ABR", "MAY", "JUN",
        "JUL", "AGO", "SEP", "OCT", "NOV", "DIC"
    };

    /// <summary>Suma un día a una fecha `yyyy-MM-dd`.</summary>
    public static bool TryNextDate(string currentDate, out string nextDate)
    {
        nextDate = null;
        if (!CalendarHelper.TryParse(currentDate, out var date)) return false;
        nextDate = CalendarHelper.ToDb(date.AddDays(1));
        return true;
    }

    /// <summary>
    /// Fase según la fecha: pretemporada hasta el primer partido de liga; después,
    /// fase regular. Las competiciones paralelas no cambian la fase (F9+).
    /// </summary>
    public static string PhaseFor(string currentDate, string firstLeagueDate)
    {
        if (string.IsNullOrEmpty(currentDate)) return PhasePreseason;
        if (string.IsNullOrEmpty(firstLeagueDate)) return PhasePreseason;
        return string.CompareOrdinal(currentDate, firstLeagueDate) >= 0
            ? PhaseRegular
            : PhasePreseason;
    }

    /// <summary>¿El equipo tiene un partido sin jugar en esa fecha?</summary>
    public static bool IsMatchDay(IEnumerable<GameDataDB> games, int teamId, string date)
        => FindMatch(games, teamId, date) != null;

    /// <summary>Partido sin jugar del equipo en la fecha (o null). Prioriza amistosos.</summary>
    public static GameDataDB FindMatch(IEnumerable<GameDataDB> games, int teamId, string date)
    {
        if (string.IsNullOrEmpty(date)) return null;

        return (games ?? Enumerable.Empty<GameDataDB>())
            .Where(g => g != null
                        && g.IsPlayed == 0
                        && g.MatchDate == date
                        && (g.HomeTeamId == teamId || g.AwayTeamId == teamId))
            .OrderBy(g => g.GameType == DatabaseManager.GameTypePreseason ? 0 : 1)
            .ThenBy(g => g.Id)
            .FirstOrDefault();
    }

    /// <summary>Fecha corta en español y mayúsculas: `18 JUL 2026`.</summary>
    public static string FormatShortDate(string dbDate)
    {
        if (!CalendarHelper.TryParse(dbDate, out var date)) return "";
        return $"{date.Day:00} {Months[date.Month - 1]} {date.Year}";
    }

    /// <summary>Fecha corta con día de la semana: `SÁB 18 JUL 2026`.</summary>
    public static string FormatLongDate(string dbDate)
    {
        if (!CalendarHelper.TryParse(dbDate, out var date)) return "";
        string[] days = { "DOM", "LUN", "MAR", "MIÉ", "JUE", "VIE", "SÁB" };
        return $"{days[(int)date.DayOfWeek]} {date.Day:00} {Months[date.Month - 1]} {date.Year}";
    }
}
