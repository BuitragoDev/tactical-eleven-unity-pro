/// <summary>
/// F7 — Objetivo de temporada según la posición del club en su competición.
///
/// Hay **seis objetivos** posibles: Campeonato, Europa, Media tabla, Permanencia,
/// Ascenso y Playoff de ascenso. La regla es pura (sin Unity ni base de datos) para
/// que la compartan la ficha de equipo y el Dashboard.
/// </summary>
public static class SeasonObjective
{
    public const string KeyCampeonato = "ui.select_team.objetivo_campeonato";
    public const string KeyEuropa = "ui.select_team.objetivo_europa";
    public const string KeyMedia = "ui.select_team.objetivo_media";
    public const string KeyPermanencia = "ui.select_team.objetivo_permanencia";
    public const string KeyAscenso = "ui.select_team.objetivo_ascenso";
    public const string KeyPlayoff = "ui.select_team.objetivo_playoff";

    /// <summary>
    /// Clave de localización del objetivo. `rank` empieza en 1 y `total` es el número
    /// de clubes de la competición (sin filiales).
    /// </summary>
    public static string KeyFor(int leagueId, int rank, int total)
    {
        if (rank <= 0 || total <= 0) return KeyMedia;

        if (leagueId == 1)
        {
            if (rank <= 4) return KeyCampeonato;
            if (rank <= 7) return KeyEuropa;
            if (rank <= total - 5) return KeyMedia;
            return KeyPermanencia;
        }

        if (leagueId == 2)
        {
            if (rank <= 2) return KeyAscenso;
            if (rank <= 6) return KeyPlayoff;
            if (rank <= total - 6) return KeyMedia;
            return KeyPermanencia;
        }

        // 1ª RFEF (grupos 3 y 4) y cualquier otra categoría con ascenso.
        if (rank <= 2) return KeyAscenso;
        if (rank <= 5) return KeyPlayoff;
        if (rank <= total - 6) return KeyMedia;
        return KeyPermanencia;
    }
}
