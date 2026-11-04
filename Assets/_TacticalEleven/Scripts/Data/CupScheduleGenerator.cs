using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// S1 — Copa del Rey: ronda preliminar con los clubes españoles que no entran
/// directamente en el cuadro de 64. Los cruces se juegan en martes/miércoles
/// libres (sin chocar con liga ni Europa). Clase pura.
///
/// Las rondas posteriores se generan en F13 cuando se conocen los clasificados.
/// </summary>
public static class CupScheduleGenerator
{
    public const int BracketSize = 64;

    public static List<ScheduledFixture> GeneratePreliminary(
        IList<TeamDataDB> teams,
        int yearStart,
        IDictionary<int, List<DateTime>> availability = null,
        IRandomSource random = null)
    {
        var ordered = (teams ?? new List<TeamDataDB>())
            .Where(t => t != null)
            .OrderByDescending(t => t.Overall)
            .ThenBy(t => t.Id)
            .ToList();

        int matches = Math.Max(0, ordered.Count - BracketSize);
        if (matches == 0 || ordered.Count < 2) return new List<ScheduledFixture>();

        // Los mejores entran directos; los 2*matches peores juegan la preliminar.
        var playing = ordered.Skip(ordered.Count - matches * 2).ToList();
        Shuffle(playing, random);

        var cupDates = CompetitionCalendarRules.CupRoundDates(yearStart);
        var candidates = new List<DateTime>();

        foreach (var date in cupDates)
        {
            candidates.Add(date);
            candidates.Add(date.DayOfWeek == DayOfWeek.Tuesday
                ? date.AddDays(1)
                : date.AddDays(-1));
        }

        candidates = candidates
            .Where(d => !FootballCalendar.IsBreak(d))
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        var fixtures = new List<ScheduledFixture>();

        for (int i = 0; i < matches; i++)
        {
            var home = playing[i];
            var away = playing[playing.Count - 1 - i];

            DateTime date = candidates.FirstOrDefault(d => IsFree(availability, home.Id, away.Id, d));
            if (date == default) date = cupDates[0];

            fixtures.Add(new ScheduledFixture
            {
                MatchDay = 1,
                HomeTeamId = home.Id,
                AwayTeamId = away.Id,
                Date = date,
                GameType = DatabaseManager.GameTypeCup,
                CompetitionId = CompetitionCalendarRules.Copa
            });

            Mark(availability, home.Id, date);
            Mark(availability, away.Id, date);
        }

        return fixtures;
    }

    private static bool IsFree(IDictionary<int, List<DateTime>> availability,
                               int homeId, int awayId, DateTime date)
        => availability == null
           || (IsTeamFree(availability, homeId, date)
               && IsTeamFree(availability, awayId, date));

    private static bool IsTeamFree(IDictionary<int, List<DateTime>> availability,
                                   int teamId, DateTime date)
    {
        if (!availability.TryGetValue(teamId, out var dates)) return true;
        return dates.All(d => Math.Abs((d - date).TotalHours) >= 48);
    }

    private static void Mark(IDictionary<int, List<DateTime>> availability,
                             int teamId, DateTime date)
    {
        if (availability == null) return;

        if (!availability.TryGetValue(teamId, out var dates))
            availability[teamId] = dates = new List<DateTime>();

        dates.Add(date);
    }

    private static void Shuffle(IList<TeamDataDB> list, IRandomSource random)
    {
        if (random == null) return;

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Range(0, i + 1);
            var tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }
}
