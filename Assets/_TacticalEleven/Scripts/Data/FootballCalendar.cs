using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Calendario puro de la temporada de fútbol: decide **qué fechas** ocupan las
/// jornadas de liga y los amistosos de pretemporada.
///
/// Reglas (F6):
/// - La liga arranca el primer sábado a partir del 15 de agosto.
/// - Las jornadas caen en fin de semana y se reparten sábado/domingo.
/// - Se respetan los parones FIFA (septiembre, octubre, noviembre y marzo) y el
///   parón navideño (22/12 – 04/01).
/// - Si los fines de semana disponibles no llegan para todas las jornadas, se
///   añaden jornadas entre semana (martes/miércoles) repartidas por la temporada.
/// - La temporada termina como muy tarde el 31 de mayo.
///
/// No toca Unity ni la base de datos: es determinista y testeable.
/// </summary>
public static class FootballCalendar
{
    public const int FirstRoundMonth = 8;
    public const int FirstRoundDay = 15;
    public const int LastRoundMonth = 5;
    public const int LastRoundDay = 31;

    /// <summary>Ventanas sin partidos de liga (mes/día inicial y final, ambos incluidos).</summary>
    private static readonly (int StartMonth, int StartDay, int EndMonth, int EndDay)[] BreakWindows =
    {
        (9, 1, 9, 10),    // parón FIFA de septiembre
        (10, 6, 10, 14),  // parón FIFA de octubre
        (11, 10, 11, 18), // parón FIFA de noviembre
        (12, 22, 1, 4),   // parón navideño
        (3, 23, 3, 31),   // parón FIFA de marzo
    };

    /// <summary>Fechas de los cuatro amistosos de pretemporada (julio/agosto).</summary>
    public static List<DateTime> PreseasonDates(int yearStart)
    {
        return new List<DateTime>
        {
            new DateTime(yearStart, 7, 18),
            new DateTime(yearStart, 7, 22),
            new DateTime(yearStart, 7, 26),
            new DateTime(yearStart, 8, 1),
        };
    }

    /// <summary>Primera jornada de liga: primer sábado desde el 15 de agosto.</summary>
    public static DateTime FirstLeagueRound(int yearStart)
    {
        var start = new DateTime(yearStart, FirstRoundMonth, FirstRoundDay);
        int offset = ((int)DayOfWeek.Saturday - (int)start.DayOfWeek + 7) % 7;
        return start.AddDays(offset);
    }

    /// <summary>Fin de la temporada: 31 de mayo del año siguiente.</summary>
    public static DateTime SeasonEnd(int yearStart)
        => new DateTime(yearStart + 1, LastRoundMonth, LastRoundDay);

    /// <summary>¿La fecha cae dentro de un parón (FIFA o navideño)?</summary>
    public static bool IsBreak(DateTime date)
    {
        foreach (var window in BreakWindows)
        {
            if (IsInside(date, window.StartMonth, window.StartDay, window.EndMonth, window.EndDay))
                return true;
        }
        return false;
    }

    private static bool IsInside(DateTime date, int startMonth, int startDay, int endMonth, int endDay)
    {
        var value = date.Month * 100 + date.Day;
        int start = startMonth * 100 + startDay;
        int end = endMonth * 100 + endDay;

        if (start <= end) return value >= start && value <= end;
        // Ventana que cruza el año (diciembre → enero).
        return value >= start || value <= end;
    }

    /// <summary>
    /// Fechas ancla de todas las jornadas de liga (una por jornada), en orden.
    /// Los fines de semana son sábados; las jornadas entre semana, miércoles.
    /// </summary>
    public static List<DateTime> LeagueRoundDates(int yearStart, int matchDays)
    {
        if (matchDays <= 0) return new List<DateTime>();

        var start = FirstLeagueRound(yearStart);
        var end = SeasonEnd(yearStart);

        var weekends = new List<DateTime>();
        for (var date = start; date <= end; date = date.AddDays(7))
        {
            if (!IsBreak(date)) weekends.Add(date);
        }

        int missing = matchDays - weekends.Count;
        if (missing > 0)
        {
            var midweek = new List<DateTime>();
            for (var date = new DateTime(yearStart, 8, 18); date <= end; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Wednesday) continue;
                if (IsBreak(date)) continue;
                // Sin pegarse a un fin de semana ya ocupado (mínimo 48 h de descanso).
                if (weekends.Any(s => Math.Abs((s - date).TotalDays) < 3)) continue;
                midweek.Add(date);
            }

            // Reparte las jornadas entre semana a lo largo de la temporada.
            for (int i = 0; i < missing && midweek.Count > 0; i++)
            {
                int index = (int)Math.Round((double)i * (midweek.Count - 1) / Math.Max(1, missing - 1));
                var picked = midweek[Math.Min(index, midweek.Count - 1)];
                if (!weekends.Contains(picked)) weekends.Add(picked);
            }
        }

        weekends.Sort();
        return weekends.Take(matchDays).ToList();
    }

    /// <summary>
    /// Reparte los partidos de una jornada entre dos días consecutivos:
    /// sábado/domingo si el ancla es fin de semana, martes/miércoles si es entre semana.
    /// </summary>
    public static void SplitRoundDates(DateTime anchor, int games, out DateTime firstDay, out DateTime secondDay)
    {
        if (anchor.DayOfWeek == DayOfWeek.Wednesday)
            firstDay = anchor.AddDays(-1);
        else
            firstDay = anchor;

        secondDay = firstDay.AddDays(1);
    }

    /// <summary>Fecha de un partido concreto dentro de la jornada (alterna los dos días).</summary>
    public static DateTime MatchDate(DateTime anchor, int indexInRound, int gamesInRound)
    {
        SplitRoundDates(anchor, gamesInRound, out var firstDay, out var secondDay);
        return indexInRound < gamesInRound / 2 ? firstDay : secondDay;
    }

    /// <summary>Comprueba que dos partidos del mismo equipo dejan al menos 48 h.</summary>
    public static bool HasMinimumRest(DateTime previous, DateTime next, int minimumHours = 48)
        => (next - previous).TotalHours >= minimumHours;
}
