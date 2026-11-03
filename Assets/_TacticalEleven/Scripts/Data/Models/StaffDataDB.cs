using SQLite;

[Table("staff")]
public class StaffDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public string Nationality { get; set; }
    public string Role { get; set; }
    public int TacticalKnowledge { get; set; }
    public int ManManagement { get; set; }
    public int Motivating { get; set; }
    public int Coaching { get; set; }
    public int YouthCoaching { get; set; }
    public int WeeklyWage { get; set; }
}

[Table("messages")]
public class MessageDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int ManagerId { get; set; }
    public int MatchDay { get; set; }
    public string Date { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string Category { get; set; }
    public int IsRead { get; set; }
}

[Table("offers")]
public class OfferDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int ManagerId { get; set; }
    public int PlayerId { get; set; }
    public int FromTeamId { get; set; }
    public int ToTeamId { get; set; }
    public long TransferFee { get; set; }
    public int WeeklyWage { get; set; }
    public int ContractYears { get; set; }
    public string Status { get; set; }
    public int MatchDay { get; set; }
}

[Table("standings")]
public class StandingsDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public int TeamId { get; set; }
    public int LeagueId { get; set; }
    public int Position { get; set; }
    public int Played { get; set; }
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int Points { get; set; }
}
