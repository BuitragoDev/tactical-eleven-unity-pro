/// <summary>Demarcación del jugador. Se llama <c>PlayerPosition</c> para no
/// chocar con <c>UnityEngine.UIElements.Position</c> (el enum global ocultaba el de UI Toolkit).</summary>
public enum PlayerPosition
{
    GK,
    CB,
    LB,
    RB,
    LWB,
    RWB,
    CDM,
    CM,
    CAM,
    LM,
    RM,
    LW,
    RW,
    ST,
    CF
}

public enum PreferredFoot
{
    Left,
    Right,
    Both
}

public enum PlayerStatus
{
    Active,
    Injured,
    Loaned,
    Suspended,
    Retired
}

public enum InjurySeverity
{
    Minor,
    Moderate,
    Severe,
    CareerEnding
}

public enum MatchRating
{
    Terrible = 1,
    Poor = 2,
    BelowAverage = 3,
    Average = 4,
    Decent = 5,
    Good = 6,
    VeryGood = 7,
    Excellent = 8,
    Outstanding = 9,
    Perfect = 10
}

public enum PersonalityType
{
    Balanced,
    Professional,
    Determined,
    Resilient,
    Spirited,
    Ambitious,
    Loyal,
    Fair,
    Controversial,
    Mercenary,
    Temperamental,
    Unprofessional,
    Perfectionist,
    ModelCitizen,
    Slacker,
    Charismatic,
    Reserved
}

public enum Morale
{
    VeryPoor,
    Poor,
    Low,
    Okay,
    Good,
    High,
    VeryHigh,
    Perfect
}

public enum FitnessLevel
{
    Exhausted,
    VeryTired,
    Tired,
    Moderate,
    Fit,
    VeryFit,
    Peak
}

public enum MatchEventType
{
    Goal,
    Assist,
    YellowCard,
    RedCard,
    SubstitutionOn,
    SubstitutionOff,
    Injury,
    PenaltyScored,
    PenaltyMissed,
    OwnGoal,
    Save,
    CleanSheet,
    // F8 — eventos de crónica (se añaden al final para no alterar los valores previos).
    Chance,
    Woodwork,
    Corner,
    Foul,
    Offside,
    AddedTime,
    KickOff,
    FullTime
}

[System.Flags]
public enum PlayerPositionFlags
{
    None = 0,
    GK = 1 << 0,
    CB = 1 << 1,
    LB = 1 << 2,
    RB = 1 << 3,
    LWB = 1 << 4,
    RWB = 1 << 5,
    CDM = 1 << 6,
    CM = 1 << 7,
    CAM = 1 << 8,
    LM = 1 << 9,
    RM = 1 << 10,
    LW = 1 << 11,
    RW = 1 << 12,
    ST = 1 << 13,
    CF = 1 << 14
}
