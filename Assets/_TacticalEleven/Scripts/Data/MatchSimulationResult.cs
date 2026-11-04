using System.Collections.Generic;

/// <summary>Equipo preparado para simular: club, once, banquillo y formación.</summary>
public sealed class MatchTeamSetup
{
    public TeamDataDB Team;
    public List<PlayerDataDB> Starters = new();
    public List<PlayerDataDB> Bench = new();
    public string Formation = FormationRules.Default;
    public bool IsHome;
}

/// <summary>Evento producido durante la simulación (se persiste en `match_events`).</summary>
public sealed class SimulatedEvent
{
    public int Minute;
    public MatchEventType Type;
    public int TeamId;
    public int PlayerId;
    public int SecondaryPlayerId;
    public string Detail;
}

/// <summary>Estadísticas individuales de un partido (se persisten en `player_match_stats`).</summary>
public sealed class SimulatedPlayerStats
{
    public int PlayerId;
    public int TeamId;
    public int Minutes;
    public double Rating = 6.2;
    public int Goals;
    public int Assists;
    public int Shots;
    public int ShotsOnTarget;
    public int Passes;
    public int PassAccuracy;
    public int Tackles;
    public int Interceptions;
    public int Fouls;
    public int YellowCard;
    public int RedCard;
    public int Saves;
    public int GoalsConceded;
    public bool Started;
}

/// <summary>Resultado completo de un partido simulado.</summary>
public sealed class MatchSimulationResult
{
    public int HomeTeamId;
    public int AwayTeamId;
    public int HomeScore;
    public int AwayScore;

    public int HomePossession = 50;
    public int AwayPossession = 50;
    public int HomeShots;
    public int AwayShots;
    public int HomeShotsOnTarget;
    public int AwayShotsOnTarget;
    public int HomeCorners;
    public int AwayCorners;
    public int HomeFouls;
    public int AwayFouls;
    public int HomeYellow;
    public int AwayYellow;
    public int HomeRed;
    public int AwayRed;

    public int MvpPlayerId;

    public readonly List<SimulatedEvent> Events = new();
    public readonly List<SimulatedPlayerStats> Players = new();

    public bool IsHomeWin => HomeScore > AwayScore;
    public bool IsDraw => HomeScore == AwayScore;
}
