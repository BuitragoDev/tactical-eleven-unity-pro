using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Elige los rivales de la pretemporada: clubes de menor categoría (o, si no
/// existe una liga inferior jugable, de menor nivel) y sin filiales.
///
/// Clase pura: no toca la base de datos.
/// </summary>
public static class PreseasonPlanner
{
    /// <summary>
    /// Devuelve hasta <paramref name="count"/> rivales candidatos, ordenados de
    /// menor a mayor nivel para que los amistosos sean asequibles.
    /// </summary>
    public static List<int> CandidateOpponents(
        TeamDataDB myTeam,
        IEnumerable<TeamDataDB> allTeams,
        int count = 12)
    {
        if (myTeam == null || allTeams == null) return new List<int>();

        var teams = allTeams
            .Where(t => t != null
                        && t.Id != myTeam.Id
                        && t.LeagueId > 0
                        && t.IsFilial == 0)
            .ToList();

        // 1) Ligas inferiores a la mía (1ª → 2ª/RFEF; 2ª → RFEF).
        var lowerDivision = teams
            .Where(t => t.LeagueId > myTeam.LeagueId)
            .OrderBy(t => t.Overall)
            .ThenBy(t => t.Reputation)
            .ToList();

        // 2) Si no hay liga inferior (1ª RFEF), rivales de nivel inferior al mío.
        var lowerLevel = teams
            .Where(t => t.LeagueId == myTeam.LeagueId && t.Overall < myTeam.Overall)
            .OrderBy(t => t.Overall)
            .ThenBy(t => t.Reputation)
            .ToList();

        // 3) Último recurso: cualquier club con menor reputación.
        var lowerReputation = teams
            .Where(t => t.Reputation < myTeam.Reputation)
            .OrderBy(t => t.Overall)
            .ThenBy(t => t.Reputation)
            .ToList();

        var ordered = new List<int>();
        foreach (var group in new[] { lowerDivision, lowerLevel, lowerReputation })
        {
            foreach (var team in group)
            {
                if (!ordered.Contains(team.Id)) ordered.Add(team.Id);
                if (ordered.Count >= count) return ordered;
            }
        }

        return ordered;
    }
}
