using System.Collections.Generic;
using System.Linq;
using SQLite;

/// <summary>
/// Auditoría del contenido sembrado. Los **ERROR** bloquean la apertura del slot;
/// los **WARN** solo informan. No reasigna clubes ni jugadores: solo delata
/// incoherencias (equipos sin competición, jugadores huérfanos, dorsales repetidos).
/// </summary>
public static class SeedAudit
{
    public class Issue
    {
        public string Severity;   // ERROR | WARN
        public string Message;
    }

    public static List<Issue> Validate(SQLiteConnection db)
    {
        var issues = new List<Issue>();
        if (db == null) return issues;

        int teamCount = db.Table<TeamDataDB>().Count();
        if (teamCount == 0)
        {
            // F1: aún no hay contenido estático; no es un error.
            issues.Add(new Issue { Severity = "WARN", Message = "Sin equipos: seed pendiente (F2)." });
            return issues;
        }

        var leagues = db.Table<LeagueDataDB>().ToList();
        var leagueIds = new HashSet<int>(leagues.Select(l => l.Id));
        if (leagues.Count == 0)
            issues.Add(new Issue { Severity = "ERROR", Message = "Hay equipos pero ninguna competición." });

        foreach (var team in db.Table<TeamDataDB>().ToList())
        {
            if (team.LeagueId != 0 && !leagueIds.Contains(team.LeagueId))
                issues.Add(new Issue
                {
                    Severity = "ERROR",
                    Message = $"'{team.Name}' apunta a la competición inexistente {team.LeagueId}."
                });

            if (string.IsNullOrWhiteSpace(team.Name))
                issues.Add(new Issue { Severity = "ERROR", Message = $"Equipo {team.Id} sin nombre." });
        }

        var teamIds = new HashSet<int>(db.Table<TeamDataDB>().Select(t => t.Id));
        foreach (var player in db.Table<PlayerDataDB>().ToList())
        {
            if (player.TeamId != 0 && !teamIds.Contains(player.TeamId))
                issues.Add(new Issue
                {
                    Severity = "ERROR",
                    Message = $"Jugador {player.Id} ({player.LastName}) en equipo inexistente {player.TeamId}."
                });
        }

        // Dorsales repetidos dentro de un mismo club (solo equipos con plantilla).
        var duplicates = db.Table<PlayerDataDB>()
            .Where(p => p.TeamId != 0 && p.SquadNumber > 0)
            .GroupBy(p => new { p.TeamId, p.SquadNumber })
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var dup in duplicates)
            issues.Add(new Issue
            {
                Severity = "WARN",
                Message = $"Dorsal {dup.Key.SquadNumber} repetido en el equipo {dup.Key.TeamId}."
            });

        return issues;
    }
}
