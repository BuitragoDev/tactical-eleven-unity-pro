using SQLite;

[Table("managers")]
public class ManagerDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public int TeamId { get; set; }
    public int LeagueId { get; set; }
    public string GameMode { get; set; }
    public int Trust { get; set; }
    public int Morale { get; set; }
    public int FanConfidence { get; set; }
}

[Table("seasons")]
public class SeasonDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int YearStart { get; set; }
    public int YearEnd { get; set; }
    public int IsActive { get; set; }
    public int CurrentMatchDay { get; set; }
    public int TotalMatchDays { get; set; }
    public string Phase { get; set; }
    public int ManagerId { get; set; }
    public int LeagueId { get; set; }
    public string CurrentDate { get; set; }
}

[Table("games")]
public class GameDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public int MatchDay { get; set; }
    public string MatchDate { get; set; }
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public int IsPlayed { get; set; }
    public string GameType { get; set; }
    public int ManagerId { get; set; }
    public int Attendance { get; set; }

    // Competición y eliminatorias (F13/F14)
    public int CompetitionId { get; set; }
    public string Round { get; set; }
    public string SeriesLabel { get; set; }
    public int NeutralVenue { get; set; }

    public int HomePossession { get; set; }
    public int AwayPossession { get; set; }
    public int HomeShots { get; set; }
    public int AwayShots { get; set; }
    public int HomeShotsOnTarget { get; set; }
    public int AwayShotsOnTarget { get; set; }
    public int HomeCorners { get; set; }
    public int AwayCorners { get; set; }
    public int HomeFouls { get; set; }
    public int AwayFouls { get; set; }
    public int HomeYellowCards { get; set; }
    public int AwayYellowCards { get; set; }
    public int HomeRedCards { get; set; }
    public int AwayRedCards { get; set; }
}

[Table("match_events")]
public class MatchEventDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int GameId { get; set; }
    public int Minute { get; set; }
    public int EventType { get; set; }
    public int PlayerId { get; set; }
    public int SecondaryPlayerId { get; set; }
    public int TeamId { get; set; }
    public string Detail { get; set; }
}

[Table("player_match_stats")]
public class PlayerMatchStatsDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int GameId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public int MinutesPlayed { get; set; }
    public double Rating { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int Shots { get; set; }
    public int ShotsOnTarget { get; set; }
    public int Passes { get; set; }
    public int PassAccuracy { get; set; }
    public int Tackles { get; set; }
    public int Interceptions { get; set; }
    public int Fouls { get; set; }
    public int YellowCard { get; set; }
    public int RedCard { get; set; }
    public int Saves { get; set; }
    public int GoalsConceded { get; set; }
}
