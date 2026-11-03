using SQLite;

[Table("players")]
public class PlayerDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int TeamId { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CommonName { get; set; }
    public int Age { get; set; }
    public string Nationality { get; set; }
    public string SecondNationality { get; set; }

    public int Height { get; set; }
    public int Weight { get; set; }
    public int Foot { get; set; }
    public int MainPosition { get; set; }
    public int SecondaryPositions { get; set; }

    public int Pace { get; set; }
    public int Acceleration { get; set; }
    public int Stamina { get; set; }
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int Balance { get; set; }
    public int Jumping { get; set; }
    public int NaturalFitness { get; set; }

    public int Passing { get; set; }
    public int Crossing { get; set; }
    public int Dribbling { get; set; }
    public int Finishing { get; set; }
    public int FirstTouch { get; set; }
    public int Heading { get; set; }
    public int LongShots { get; set; }
    public int Marking { get; set; }
    public int Penalties { get; set; }
    public int FreeKicks { get; set; }
    public int Corners { get; set; }
    public int Tackling { get; set; }
    public int Technique { get; set; }
    public int Flair { get; set; }

    public int Aggression { get; set; }
    public int Anticipation { get; set; }
    public int Bravery { get; set; }
    public int Composure { get; set; }
    public int Concentration { get; set; }
    public int Decisions { get; set; }
    public int Determination { get; set; }
    public int Leadership { get; set; }
    public int OffTheBall { get; set; }
    public int Positioning { get; set; }
    public int Teamwork { get; set; }
    public int Vision { get; set; }
    public int WorkRate { get; set; }
    public int Consistency { get; set; }

    public int Handling { get; set; }
    public int Kicking { get; set; }
    public int Reflexes { get; set; }
    public int GkPositioning { get; set; }
    public int OneOnOnes { get; set; }
    public int CommandOfArea { get; set; }
    public int RushingOut { get; set; }
    public int PenaltySaving { get; set; }

    public int Overall { get; set; }
    public int Potential { get; set; }
    public long Value { get; set; }
    public long Wage { get; set; }
    public long ReleaseClause { get; set; }

    public int ContractYears { get; set; }
    public int ContractEndSeason { get; set; }
    public int SquadNumber { get; set; }
    public int Status { get; set; }

    // Mercado y cesiones (F11)
    public int IsOnLoan { get; set; }
    public int LoanFromTeamId { get; set; }
    public int LoanEndSeason { get; set; }
    public int TransferListed { get; set; }
    public int IsYouth { get; set; }
    public int IsFilialEligible { get; set; }
    public int SeasonsWithTeam { get; set; }
    public int LastTeamId { get; set; }

    public int Morale { get; set; }
    public int Fitness { get; set; }
    public double Form { get; set; }
    public int Personality { get; set; }

    public int TotalGoals { get; set; }
    public int TotalAssists { get; set; }
    public int TotalAppearances { get; set; }
    public int TotalYellowCards { get; set; }
    public int TotalRedCards { get; set; }
    public int TotalManOfTheMatch { get; set; }

    public int InjuryDays { get; set; }
    public string InjuryType { get; set; }
    public int Treated { get; set; }

    /// <summary>Media de atributos relevante para la posición (delega en PlayerRating).</summary>
    public double GetCalculatedAverage() => PlayerRating.FromAttributes(this);
}

[Table("player_season_stats")]
public class PlayerSeasonStatsDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int SeasonYear { get; set; }
    public int TeamId { get; set; }

    public int Appearances { get; set; }
    public int Starts { get; set; }
    public int Substitutions { get; set; }
    public int MinutesPlayed { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public int ManOfTheMatch { get; set; }
    public double AverageRating { get; set; }
    public int CleanSheets { get; set; }
    public int GoalsConceded { get; set; }
    public int Saves { get; set; }
}

[Table("injuries")]
public class InjuryDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string Date { get; set; }
    public string Description { get; set; }
    public int Severity { get; set; }
    public int DaysOut { get; set; }
    public int DaysRemaining { get; set; }
    public int IsRecurring { get; set; }
}
