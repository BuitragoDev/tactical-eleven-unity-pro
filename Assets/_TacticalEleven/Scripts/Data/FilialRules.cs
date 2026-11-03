using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Filiales (equipos B) del seed. No se pueden elegir para jugar, igual que en la
/// realidad no compiten en la misma categoría que su primer equipo.
///
/// El seed no traía la marca, así que se deriva del nombre y de una tabla de padres.
/// Fuente única: cualquier pantalla que liste clubes seleccionables debe excluirlos.
/// </summary>
public static class FilialRules
{
    /// <summary>Filiales conocidos y el id de su primer equipo en el seed.</summary>
    private static readonly Dictionary<string, int> ParentByName = new()
    {
        { "Real Sociedad B", 4 },
        { "RC Celta Fortuna", 11 },
        { "Bilbao Athletic", 5 },
        { "Fabril Deportivo", 19 },
        { "Real Madrid Castilla", 1 },
        { "Atlético Madrileño", 3 },
        { "CF Villarreal B", 7 },
        { "Atlético de Madrid B", 3 },
    };

    /// <summary>¿Es un filial? Lista conocida o sufijo "B"/"II".</summary>
    public static bool IsFilialName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        if (ParentByName.ContainsKey(name)) return true;

        string trimmed = name.TrimEnd();
        return trimmed.EndsWith(" B") || trimmed.EndsWith(" II");
    }

    /// <summary>
    /// Marca `IsFilial` y resuelve `ParentClubId` sobre la lista de clubes del seed.
    /// Idempotente.
    /// </summary>
    public static void Apply(IEnumerable<TeamDataDB> teams)
    {
        var list = teams?.ToList();
        if (list == null || list.Count == 0) return;

        var byId = list.ToDictionary(t => t.Id);

        foreach (var team in list)
        {
            if (!IsFilialName(team.Name))
            {
                team.IsFilial = 0;
                continue;
            }

            team.IsFilial = 1;

            if (ParentByName.TryGetValue(team.Name, out int parentId) &&
                byId.TryGetValue(parentId, out var parent))
            {
                team.ParentClubId = parent.Id;
                team.ParentClub = parent.Name;
            }
        }
    }

    /// <summary>Clubes que el jugador puede elegir (sin filiales ni históricos).</summary>
    public static IEnumerable<TeamDataDB> Selectable(IEnumerable<TeamDataDB> teams)
        => (teams ?? Enumerable.Empty<TeamDataDB>())
            .Where(t => t.LeagueId >= 1 && t.LeagueId <= 4 && t.IsFilial == 0);

    /// <summary>
    /// ¿Se puede elegir este club? Los filiales y los históricos nunca; en ProManager,
    /// solo la mitad baja de su competición (los filiales no cuentan como rivales).
    /// </summary>
    public static bool CanSelect(TeamDataDB team, bool proManager, IEnumerable<TeamDataDB> allTeams)
    {
        if (team == null) return false;
        if (team.LeagueId < 1 || team.LeagueId > 4) return false;
        if (team.IsFilial == 1) return false;
        if (!proManager) return true;

        var rivals = (allTeams ?? Enumerable.Empty<TeamDataDB>())
            .Where(t => t.LeagueId == team.LeagueId && t.IsFilial == 0)
            .OrderByDescending(t => t.Overall)
            .ToList();

        int index = rivals.FindIndex(t => t.Id == team.Id);
        return index >= rivals.Count / 2;
    }
}
