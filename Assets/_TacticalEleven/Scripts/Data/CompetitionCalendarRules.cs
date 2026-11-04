using System;
using System.Collections.Generic;

/// <summary>
/// S1 — Días reales de cada competición:
/// ligas en fin de semana; Champions martes; Europa miércoles; Conference jueves;
/// Copa del Rey en martes/miércoles libres. Clase pura.
/// </summary>
public static class CompetitionCalendarRules
{
    public const int Champions = 5;
    public const int Europa = 6;
    public const int Conference = 7;
    public const int Copa = 8;
    public const int Supercopa = 9;

    public const int EuropeRounds = 8;

    /// <summary>Día de la semana de una competición europea (Tuesday/Wednesday/Thursday).</summary>
    public static int EuropeWeekday(int competitionId) => competitionId switch
    {
        Europa => (int)DayOfWeek.Wednesday,
        Conference => (int)DayOfWeek.Thursday,
        _ => (int)DayOfWeek.Tuesday
    };

    /// <summary>8 fechas de la fase de liga europea, separadas y fuera de parones.</summary>
    public static List<DateTime> EuropeRoundDates(int yearStart, int competitionId)
    {
        var anchors = new (int Month, int Day)[]
        {
            (9, 15), (10, 20), (11, 3), (11, 24),
            (12, 8), (1, 19), (1, 26), (2, 2)
        };

        int weekday = EuropeWeekday(competitionId);
        var dates = new List<DateTime>();

        foreach (var anchor in anchors)
        {
            var date = Resolve(yearStart, anchor.Month, anchor.Day);
            while ((int)date.DayOfWeek != weekday) date = date.AddDays(1);
            while (FootballCalendar.IsBreak(date)) date = date.AddDays(7);
            dates.Add(date);
        }

        return dates;
    }

    /// <summary>Fechas de las rondas de Copa: martes o miércoles, evitando parones.</summary>
    public static List<DateTime> CupRoundDates(int yearStart)
    {
        var anchors = new (int Month, int Day)[]
        {
            (10, 27), (12, 15), (1, 12), (2, 9),
            (3, 2), (4, 13), (4, 27), (5, 11)
        };

        var dates = new List<DateTime>();
        foreach (var anchor in anchors)
        {
            var date = Resolve(yearStart, anchor.Month, anchor.Day);
            while (date.DayOfWeek != DayOfWeek.Tuesday && date.DayOfWeek != DayOfWeek.Wednesday)
                date = date.AddDays(1);
            while (FootballCalendar.IsBreak(date)) date = date.AddDays(1);
            dates.Add(date);
        }
        return dates;
    }

    public static bool IsWeekend(DateTime date)
        => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

    private static DateTime Resolve(int yearStart, int month, int day)
        => new DateTime(month >= 7 ? yearStart : yearStart + 1, month, day);
}
