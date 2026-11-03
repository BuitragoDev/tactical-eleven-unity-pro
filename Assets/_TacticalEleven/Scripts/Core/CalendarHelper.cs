using System;

/// <summary>
/// Mapeo entre la fecha del calendario y el índice de día de temporada (`game_day`).
///
/// Regla: el día 1 es el **15 de agosto** del año de inicio (arranque de la
/// pretemporada de fútbol); los días anteriores son negativos y el 0 es el
/// centinela "temporada sin empezar". Clase estática pura para poder testearla.
/// </summary>
public static class CalendarHelper
{
    public const int SeasonStartMonth = 8;
    public const int SeasonStartDay = 15;

    /// <summary>Fecha de inicio de la temporada (15 de agosto del año indicado).</summary>
    public static DateTime SeasonStart(int yearStart)
        => new DateTime(yearStart, SeasonStartMonth, SeasonStartDay);

    /// <summary>Índice de día de temporada: 15/08 → 1, 16/08 → 2, 14/08 → −1.</summary>
    public static int DateToGameDay(int yearStart, DateTime date)
    {
        var start = SeasonStart(yearStart);
        if (date >= start) return (int)(date - start).TotalDays + 1;
        return -(int)(start - date).TotalDays;
    }

    /// <summary>Inverso de <see cref="DateToGameDay"/>; el 0 se resuelve como el día 1.</summary>
    public static DateTime GameDayToDate(int yearStart, int gameDay)
    {
        var start = SeasonStart(yearStart);
        if (gameDay >= 1) return start.AddDays(gameDay - 1);
        if (gameDay == 0) return start;
        return start.AddDays(gameDay);
    }

    /// <summary>Formato de fecha usado en la base de datos.</summary>
    public static string ToDb(DateTime date) => date.ToString("yyyy-MM-dd");

    /// <summary>Convierte una fecha de la base de datos; devuelve false si no es válida.</summary>
    public static bool TryParse(string dbDate, out DateTime date)
        => DateTime.TryParse(dbDate, out date);
}
