using SQLite;

[Table("teams")]
public class TeamDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Nickname { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public int LeagueId { get; set; }
    public string StadiumName { get; set; }
    public int StadiumCapacity { get; set; }

    public int YearFounded { get; set; }
    public string PrimaryColor { get; set; }
    public string SecondaryColor { get; set; }

    public long BankBalance { get; set; }
    public long TransferBudget { get; set; }
    public long WageBudget { get; set; }
    public long TotalWageBill { get; set; }

    public int TrainingFacilities { get; set; }
    public int YouthFacilities { get; set; }
    public int YouthRecruitment { get; set; }
    public int ScoutingRange { get; set; }

    public double Reputation { get; set; }
    public int Overall { get; set; }
    public int Attack { get; set; }
    public int Midfield { get; set; }
    public int Defense { get; set; }

    public int SeasonPoints { get; set; }
    public int SeasonPosition { get; set; }
    public int SeasonGoalsFor { get; set; }
    public int SeasonGoalsAgainst { get; set; }
    public int SeasonWins { get; set; }
    public int SeasonDraws { get; set; }
    public int SeasonLosses { get; set; }

    public int DomesticLeagueTitles { get; set; }
    public int DomesticCupTitles { get; set; }
    public int ContinentalTitles { get; set; }
    public int Formation { get; set; }
    public string ParentClub { get; set; }
    public string Objective { get; set; }
    public int Chemistry { get; set; }
    public int EuropeanCompetitionId { get; set; }
    public int ArenaRenovationEndDay { get; set; }
    public string ArenaRenovationType { get; set; }
    public long ArenaRenovationCost { get; set; }

    // Imágenes y filiales (F2/F16)
    public string Logo { get; set; }
    public string StadiumImage { get; set; }
    public int IsFilial { get; set; }
    public int ParentClubId { get; set; }
    public string JerseyHome { get; set; }
    public string JerseyAway { get; set; }
}

[Table("leagues")]
public class LeagueDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
    public int Division { get; set; }
    public int TeamCount { get; set; }
    public int Promotions { get; set; }
    public int Relegations { get; set; }

    // Competición (F2)
    public string ShortName { get; set; }
    public string Type { get; set; }        // league | cup | supercup | europe
    public int Tier { get; set; }           // 1..3 (0 en copa/europa)
    public int GroupIndex { get; set; }     // 0 = sin grupos
    public int EuropeSlots { get; set; }
    public string Logo { get; set; }
}
