using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// S1 — Fase de liga europea: 36 equipos, 8 jornadas, cada equipo contra 8 rivales
/// distintos. Champions en martes, Europa en miércoles y Conference en jueves.
/// Clase pura.
/// </summary>
public static class EuropeScheduleGenerator
{
    public static List<ScheduledFixture> Generate(IList<int> teamIds, int competitionId,
                                                  int yearStart, IRandomSource random = null)
    {
        var ids = (teamIds ?? new List<int>())
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        if (ids.Count < 4) return new List<ScheduledFixture>();

        Shuffle(ids, random);
        if (ids.Count % 2 != 0) ids.Add(-1);

        int n = ids.Count;
        var dates = CompetitionCalendarRules.EuropeRoundDates(yearStart, competitionId);

        int rounds = Math.Min(CompetitionCalendarRules.EuropeRounds, Math.Min(n - 1, dates.Count));
        var fixtures = new List<ScheduledFixture>();

        for (int round = 0; round < rounds; round++)
        {
            for (int slot = 0; slot < n / 2; slot++)
            {
                int a = ids[slot];
                int b = ids[n - 1 - slot];
                if (a == -1 || b == -1) continue;

                bool aHome = (round + slot) % 2 == 0;

                fixtures.Add(new ScheduledFixture
                {
                    MatchDay = round + 1,
                    HomeTeamId = aHome ? a : b,
                    AwayTeamId = aHome ? b : a,
                    Date = dates[round],
                    GameType = DatabaseManager.GameTypeEurope,
                    CompetitionId = competitionId
                });
            }

            Rotate(ids);
        }

        return fixtures;
    }

    private static void Rotate(List<int> ids)
    {
        int last = ids[ids.Count - 1];
        for (int i = ids.Count - 1; i > 1; i--)
            ids[i] = ids[i - 1];
        ids[1] = last;
    }

    private static void Shuffle(IList<int> list, IRandomSource random)
    {
        if (random == null) return;

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Range(0, i + 1);
            int tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }
}
