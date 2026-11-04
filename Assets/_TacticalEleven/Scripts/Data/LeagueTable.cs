using System.Collections.Generic;
using System.Linq;

/// <summary>Fila de clasificación calculada a partir de partidos jugados.</summary>
public sealed class LeagueTableRow
{
    public int TeamId { get; set; }
    public int Played { get; set; }
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }

    public int GoalDifference => GoalsFor - GoalsAgainst;
    public int Points => Won * 3 + Drawn;
}

/// <summary>
/// F7 — Clasificación de liga calculada desde `games`. Solo cuenta partidos
/// jugados (`IsPlayed == 1`); el llamador decide qué competición pasar.
/// Clase pura: no toca Unity ni SQLite.
/// </summary>
public static class LeagueTable
{
    public static List<LeagueTableRow> Build(IEnumerable<int> teamIds, IEnumerable<GameDataDB> games)
    {
        var rows = new Dictionary<int, LeagueTableRow>();
        foreach (int id in teamIds ?? Enumerable.Empty<int>())
            rows[id] = new LeagueTableRow { TeamId = id };

        foreach (var game in games ?? Enumerable.Empty<GameDataDB>())
        {
            if (game == null || game.IsPlayed != 1) continue;
            if (!rows.TryGetValue(game.HomeTeamId, out var home)) continue;
            if (!rows.TryGetValue(game.AwayTeamId, out var away)) continue;

            home.Played++;
            away.Played++;
            home.GoalsFor += game.HomeScore;
            home.GoalsAgainst += game.AwayScore;
            away.GoalsFor += game.AwayScore;
            away.GoalsAgainst += game.HomeScore;

            if (game.HomeScore > game.AwayScore)
            {
                home.Won++;
                away.Lost++;
            }
            else if (game.HomeScore < game.AwayScore)
            {
                away.Won++;
                home.Lost++;
            }
            else
            {
                home.Drawn++;
                away.Drawn++;
            }
        }

        return rows.Values
            .OrderByDescending(r => r.Points)
            .ThenByDescending(r => r.GoalDifference)
            .ThenByDescending(r => r.GoalsFor)
            .ThenBy(r => r.TeamId)
            .ToList();
    }

    /// <summary>Posición (1-based) del club; 0 si no aparece.</summary>
    public static int PositionOf(IReadOnlyList<LeagueTableRow> rows, int teamId)
    {
        if (rows == null) return 0;
        for (int i = 0; i < rows.Count; i++)
            if (rows[i].TeamId == teamId) return i + 1;
        return 0;
    }
}
