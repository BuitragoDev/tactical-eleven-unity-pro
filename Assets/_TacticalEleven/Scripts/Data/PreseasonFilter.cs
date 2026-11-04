/// <summary>
/// Filtros de competición de la pretemporada: tres ligas españolas (LaLiga,
/// LaLiga Hypermotion y 1ª RFEF) y tres competiciones europeas (Champions,
/// Europa y Conference).
///
/// La 1ª RFEF agrupa sus dos grupos (`LeagueId` 3 y 4) en un único filtro.
/// Clase pura: no toca Unity ni la base de datos.
/// </summary>
public static class PreseasonFilter
{
    public const int LaLiga = 1;
    public const int LaLiga2 = 2;
    public const int Rfef = -1;
    public const int Champions = 5;
    public const int Europa = 6;
    public const int Conference = 7;

    /// <summary>Orden de los botones en la pantalla (3 españolas + 3 europeas).</summary>
    public static readonly int[] All = { LaLiga, LaLiga2, Rfef, Champions, Europa, Conference };

    /// <summary>¿El club de esta liga entra en el filtro indicado?</summary>
    public static bool Matches(int teamLeagueId, int filterLeagueId)
        => filterLeagueId == Rfef
            ? teamLeagueId == 3 || teamLeagueId == 4
            : teamLeagueId == filterLeagueId;

    /// <summary>Clave de localización del botón.</summary>
    public static string LocalizationKey(int filterLeagueId)
    {
        switch (filterLeagueId)
        {
            case LaLiga: return "ui.preseason.filter_laliga";
            case LaLiga2: return "ui.preseason.filter_laliga2";
            case Rfef: return "ui.preseason.filter_rfef";
            case Champions: return "ui.preseason.filter_champions";
            case Europa: return "ui.preseason.filter_europa";
            case Conference: return "ui.preseason.filter_conference";
            default: return "ui.preseason.filter_laliga";
        }
    }
}
