/// <summary>
/// F8 — Contexto compartido entre las pantallas de partido (PreGame → MatchDay →
/// GameResults). Guarda el partido que se está jugando y el resultado simulado.
/// No sustituye a SQLite: es solo el hilo conductor de la visita actual.
/// </summary>
public static class MatchContext
{
    public static int GameId { get; private set; }
    public static int SeasonId { get; private set; }
    public static MatchSimulationResult Result { get; set; }

    public static bool HasGame => GameId > 0;

    public static void Set(GameDataDB game)
    {
        GameId = game?.Id ?? 0;
        SeasonId = game?.SeasonId ?? 0;
        Result = null;
    }

    public static void Clear()
    {
        GameId = 0;
        SeasonId = 0;
        Result = null;
    }
}
