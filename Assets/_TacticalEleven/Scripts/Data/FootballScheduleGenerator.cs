using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>Partido generado por <see cref="FootballScheduleGenerator"/> (sin persistir).</summary>
public sealed class ScheduledFixture
{
    public int MatchDay { get; set; }
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public DateTime Date { get; set; }
    public string GameType { get; set; } = "regular";
    public int CompetitionId { get; set; }
}

/// <summary>Resultado de validar un calendario.</summary>
public sealed class ScheduleValidation
{
    public readonly List<string> Errors = new();

    public bool IsValid => Errors.Count == 0;

    public void Add(string error) => Errors.Add(error);

    public override string ToString() => IsValid ? "OK" : string.Join(" | ", Errors);
}

/// <summary>
/// Generador puro del calendario de liga y de la pretemporada.
///
/// - Doble vuelta por el método del círculo: cada pareja se enfrenta dos veces,
///   una en cada campo.
/// - El orden inicial se baraja con <see cref="IRandomSource"/>, de modo que una
///   semilla fija produce siempre el mismo calendario.
/// - Las fechas las decide <see cref="FootballCalendar"/> (parones FIFA, Navidad,
///   jornadas entre semana y reparto sábado/domingo).
///
/// No toca Unity ni la base de datos.
/// </summary>
public static class FootballScheduleGenerator
{
    public const int PreseasonGames = 4;

    /// <summary>
    /// Calendario completo de una liga (doble vuelta) con fechas asignadas.
    /// </summary>
    public static List<ScheduledFixture> GenerateLeague(
        IList<int> teamIds,
        int yearStart,
        int competitionId,
        IRandomSource random = null,
        ISet<DateTime> avoidMidweek = null)
    {
        var fixtures = GenerateDoubleRoundRobin(teamIds, random);

        int matchDays = fixtures.Count == 0 ? 0 : fixtures.Max(f => f.MatchDay);
        var roundDates = FootballCalendar.LeagueRoundDates(yearStart, matchDays, avoidMidweek);

        if (roundDates.Count < matchDays)
            throw new InvalidOperationException(
                $"No hay fechas suficientes para {matchDays} jornadas en {yearStart}-{yearStart + 1}.");

        foreach (var group in fixtures.GroupBy(f => f.MatchDay))
        {
            var anchor = roundDates[group.Key - 1];
            var ordered = group.OrderBy(f => f.HomeTeamId).ThenBy(f => f.AwayTeamId).ToList();
            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].Date = FootballCalendar.MatchDate(anchor, i, ordered.Count);
                ordered[i].CompetitionId = competitionId;
                ordered[i].GameType = "regular";
            }
        }

        return fixtures.OrderBy(f => f.MatchDay).ThenBy(f => f.Date).ToList();
    }

    /// <summary>Doble vuelta sin fechas (método del círculo).</summary>
    public static List<ScheduledFixture> GenerateDoubleRoundRobin(
        IList<int> teamIds,
        IRandomSource random = null)
    {
        if (teamIds == null || teamIds.Count < 2)
            return new List<ScheduledFixture>();

        var ids = teamIds.Where(id => id > 0).Distinct().ToList();
        Shuffle(ids, random);

        bool hasBye = ids.Count % 2 != 0;
        if (hasBye) ids.Add(-1);

        int n = ids.Count;
        int rounds = n - 1;
        int matchesPerRound = n / 2;
        var fixtures = new List<ScheduledFixture>();

        for (int round = 0; round < rounds; round++)
        {
            int firstLegDay = round + 1;
            int secondLegDay = firstLegDay + rounds;

            for (int m = 0; m < matchesPerRound; m++)
            {
                int a = ids[m];
                int b = ids[n - 1 - m];
                if (a == -1 || b == -1) continue;

                bool aHome = (round + m) % 2 == 0;
                int home = aHome ? a : b;
                int away = aHome ? b : a;

                fixtures.Add(new ScheduledFixture
                {
                    MatchDay = firstLegDay,
                    HomeTeamId = home,
                    AwayTeamId = away
                });
                fixtures.Add(new ScheduledFixture
                {
                    MatchDay = secondLegDay,
                    HomeTeamId = away,
                    AwayTeamId = home
                });
            }

            // Rotación: el primero queda fijo y el resto gira una posición.
            int last = ids[n - 1];
            for (int i = n - 1; i > 1; i--)
                ids[i] = ids[i - 1];
            ids[1] = last;
        }

        return fixtures;
    }

    /// <summary>
    /// Amistosos de pretemporada contra rivales de menor entidad. Los rivales se
    /// eligen sin repetir y con alternancia local/visitante.
    /// </summary>
    public static List<ScheduledFixture> GeneratePreseason(
        int myTeamId,
        IList<int> candidateOpponentIds,
        int yearStart,
        IRandomSource random = null,
        int count = PreseasonGames)
    {
        if (myTeamId <= 0 || candidateOpponentIds == null) return new List<ScheduledFixture>();

        var pool = candidateOpponentIds
            .Where(id => id > 0 && id != myTeamId)
            .Distinct()
            .ToList();
        Shuffle(pool, random);

        var dates = FootballCalendar.PreseasonDates(yearStart);
        int total = Math.Min(Math.Min(count, pool.Count), dates.Count);
        var fixtures = new List<ScheduledFixture>();

        for (int i = 0; i < total; i++)
        {
            bool isHome = i % 2 == 0;
            fixtures.Add(new ScheduledFixture
            {
                MatchDay = -(total - i),
                HomeTeamId = isHome ? myTeamId : pool[i],
                AwayTeamId = isHome ? pool[i] : myTeamId,
                Date = dates[i],
                GameType = "preseason",
                CompetitionId = 0
            });
        }

        return fixtures;
    }

    /// <summary>
    /// Valida un calendario de liga: jornadas esperadas, cada pareja dos veces
    /// (una en cada campo), sin equipos repetidos en la misma jornada y descanso
    /// mínimo de 48 h por equipo.
    /// </summary>
    public static ScheduleValidation Validate(
        IList<ScheduledFixture> fixtures,
        IList<int> teamIds,
        int minimumRestHours = 48)
    {
        var result = new ScheduleValidation();
        if (fixtures == null || fixtures.Count == 0)
        {
            result.Add("El calendario está vacío.");
            return result;
        }

        var teams = teamIds.Where(id => id > 0).Distinct().ToList();
        int expectedRounds = (teams.Count - 1) * 2;
        int actualRounds = fixtures.Max(f => f.MatchDay);

        if (actualRounds != expectedRounds)
            result.Add($"Jornadas esperadas {expectedRounds}, generadas {actualRounds}.");

        foreach (var f in fixtures)
        {
            if (f.HomeTeamId == f.AwayTeamId)
                result.Add($"Equipo contra sí mismo en la jornada {f.MatchDay} ({f.HomeTeamId}).");
            if (!teams.Contains(f.HomeTeamId) || !teams.Contains(f.AwayTeamId))
                result.Add($"Equipo fuera de la liga en la jornada {f.MatchDay}.");
        }

        var pairCounts = new Dictionary<(int, int), int>();
        var homeCount = new Dictionary<(int, int), int>();

        foreach (var f in fixtures)
        {
            var key = f.HomeTeamId < f.AwayTeamId
                ? (f.HomeTeamId, f.AwayTeamId)
                : (f.AwayTeamId, f.HomeTeamId);
            pairCounts[key] = pairCounts.GetValueOrDefault(key) + 1;
            homeCount[(f.HomeTeamId, f.AwayTeamId)] =
                homeCount.GetValueOrDefault((f.HomeTeamId, f.AwayTeamId)) + 1;
        }

        foreach (var team in teams)
        {
            foreach (var rival in teams.Where(t => t != team))
            {
                var key = team < rival ? (team, rival) : (rival, team);
                if (pairCounts.GetValueOrDefault(key) != 2)
                    result.Add($"La pareja {key.Item1}-{key.Item2} se enfrenta {pairCounts.GetValueOrDefault(key)} veces.");
                if (homeCount.GetValueOrDefault((team, rival)) != 1)
                    result.Add($"La pareja {team}-{rival} no tiene un partido en cada campo.");
            }
        }

        // Equipos repetidos en la misma jornada.
        foreach (var group in fixtures.GroupBy(f => f.MatchDay))
        {
            var seen = new HashSet<int>();
            foreach (var f in group)
            {
                if (!seen.Add(f.HomeTeamId) || !seen.Add(f.AwayTeamId))
                    result.Add($"Equipo repetido en la jornada {group.Key}.");
            }
        }

        // Descanso mínimo por equipo.
        if (minimumRestHours > 0)
        {
            foreach (var team in teams)
            {
                var dates = fixtures
                    .Where(f => f.HomeTeamId == team || f.AwayTeamId == team)
                    .OrderBy(f => f.Date)
                    .Select(f => f.Date)
                    .ToList();

                for (int i = 1; i < dates.Count; i++)
                {
                    if (!FootballCalendar.HasMinimumRest(dates[i - 1], dates[i], minimumRestHours))
                        result.Add($"El equipo {team} juega con menos de {minimumRestHours} h de descanso ({dates[i - 1]:yyyy-MM-dd} → {dates[i]:yyyy-MM-dd}).");
                }
            }
        }

        return result;
    }

    private static void Shuffle<T>(IList<T> list, IRandomSource random)
    {
        if (random == null) random = UnityRandomSource.Instance;
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
