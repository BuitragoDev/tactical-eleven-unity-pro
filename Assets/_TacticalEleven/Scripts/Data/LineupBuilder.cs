using System.Collections.Generic;
using System.Linq;

/// <summary>Alineación de un equipo para un partido (once + banquillo).</summary>
public sealed class MatchLineup
{
    public TeamDataDB Team;
    public string Formation = FormationRules.Default;
    public List<PlayerPosition> Slots = new();
    public List<PlayerDataDB> Starters = new();
    public List<PlayerDataDB> Bench = new();

    public bool IsValid => Starters.Count == 11;
}

/// <summary>
/// F8 — Construye el once y el banquillo cuando el club no tiene alineación
/// guardada. En F10 la pantalla de alineación sustituirá esta elección automática.
/// </summary>
public static class LineupBuilder
{
    public const int BenchSize = 7;

    public static MatchLineup Build(TeamDataDB team, IEnumerable<PlayerDataDB> squad, string formation = null)
    {
        var lineup = new MatchLineup
        {
            Team = team,
            Formation = string.IsNullOrEmpty(formation) ? FormationRules.FromIndex(team?.Formation ?? 0) : formation
        };
        lineup.Slots.AddRange(FormationRules.Slots(lineup.Formation));

        var players = (squad ?? Enumerable.Empty<PlayerDataDB>())
            .Where(p => p != null)
            .ToList();

        var available = players.Where(p => p.InjuryDays <= 0).ToList();
        if (available.Count < 11) available = players;

        var used = new HashSet<int>();

        foreach (var slot in lineup.Slots)
        {
            var best = available
                .Where(p => !used.Contains(p.Id))
                .OrderByDescending(p => Score(p, slot))
                .ThenByDescending(p => p.Overall)
                .ThenBy(p => p.Id)
                .FirstOrDefault();

            if (best == null) break;

            used.Add(best.Id);
            lineup.Starters.Add(best);
        }

        lineup.Bench = available
            .Where(p => !used.Contains(p.Id))
            .OrderByDescending(p => p.Overall)
            .ThenBy(p => p.Id)
            .Take(BenchSize)
            .ToList();

        return lineup;
    }

    /// <summary>Rendimiento esperado del jugador en ese hueco (overall × encaje).</summary>
    public static float Score(PlayerDataDB player, PlayerPosition slot)
    {
        if (player == null) return 0f;

        var natural = (PlayerPosition)player.MainPosition;
        float compatibility = FormationRules.Compatibility(natural, slot);

        // Puesto secundario: pequeño plus sobre el encaje por línea.
        if (player.SecondaryPositions > 0)
        {
            int bit = 1 << (int)slot;
            if ((player.SecondaryPositions & bit) != 0)
                compatibility = System.Math.Min(1f, compatibility + 0.05f);
        }

        return player.Overall * compatibility;
    }
}
