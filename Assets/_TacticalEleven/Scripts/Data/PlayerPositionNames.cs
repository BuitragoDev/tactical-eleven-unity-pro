/// <summary>
/// Nombres de las demarcaciones de fútbol (código corto y nombre largo en español).
/// Fuente única para UI, plantillas y estadísticas.
/// </summary>
public static class PlayerPositionNames
{
    public static readonly string[] Short =
    {
        "POR", // 0 GK
        "DFC", // 1 CB
        "LI",  // 2 LB
        "LD",  // 3 RB
        "CAI", // 4 LWB
        "CAD", // 5 RWB
        "MCD", // 6 CDM
        "MC",  // 7 CM
        "MCO", // 8 CAM
        "MI",  // 9 LM
        "MD",  // 10 RM
        "EI",  // 11 LW
        "ED",  // 12 RW
        "DC",  // 13 ST
        "SD"   // 14 CF
    };

    public static readonly string[] Long =
    {
        "Portero",
        "Central",
        "Lateral izquierdo",
        "Lateral derecho",
        "Carrilero izquierdo",
        "Carrilero derecho",
        "Mediocentro defensivo",
        "Mediocentro",
        "Mediapunta",
        "Interior izquierdo",
        "Interior derecho",
        "Extremo izquierdo",
        "Extremo derecho",
        "Delantero centro",
        "Segundo delantero"
    };

    public static bool IsGoalkeeper(int position) => position == 0;

    /// <summary>Grupo de línea: 0 portero, 1 defensa, 2 centrocampista, 3 delantero.</summary>
    public static int LineGroup(int position)
    {
        if (position == 0) return 0;
        if (position >= 1 && position <= 5) return 1;
        if (position >= 6 && position <= 10) return 2;
        return 3;
    }

    public static string ShortName(int position)
        => position >= 0 && position < Short.Length ? Short[position] : "?";

    public static string LongName(int position)
        => position >= 0 && position < Long.Length ? Long[position] : "Desconocida";
}
