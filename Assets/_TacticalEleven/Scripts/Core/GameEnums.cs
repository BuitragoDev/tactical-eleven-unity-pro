/// <summary>
/// Enumeraciones globales de Tactical Eleven (fútbol).
/// Mismo patrón que Tactical Five: sin namespaces, valores estables
/// (no reordenar: se serializan en la escena y en la base de datos).
/// </summary>
public enum GameMode
{
    None,
    Manager,
    ProManager,
    Editor
}

/// <summary>Pantallas registradas en <see cref="ScreenManager"/>.</summary>
public enum GameScreen
{
    Loading,
    MainMenu,
    LoadGame,
    Editor,
    SelectTeam,
    Preseason,
    Dashboard,
    Roster,
    Lineup,
    Tactics,
    Training,
    Staff,
    Injured,
    Dorsales,
    Calendar,
    Results,
    Standings,
    CompetitionInfo,
    Filial,
    Rivalidades,
    Logros,
    Palmares,
    Records,
    Premios,
    Knockouts,
    Europe,
    Copa,
    Stats,
    Market,
    Cartera,
    Finanzas,
    Loans,
    Sponsors,
    TV,
    Stadium,
    City,
    Manager,
    Messages,
    MatchDay,
    GameResults,
    PreGame,
    SeasonSummary,
    PlayerAwards,
    EndSeason,
    NewSeason,
    Historial,
    PlayerProfile,
    Trajectory,
    Buscador,
    BestXI,
    Hilos
}

/// <summary>Modo de vista del partido (ajuste persistido en PlayerPrefs).</summary>
public enum SimMode
{
    Direct = 0,
    PlayByPlay = 1
}
