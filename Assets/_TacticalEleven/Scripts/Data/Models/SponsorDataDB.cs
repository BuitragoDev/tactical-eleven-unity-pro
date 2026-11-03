using SQLite;

[Table("sponsors")]
public class SponsorDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Logo { get; set; }
    public int SponsorType { get; set; }
    public long Value { get; set; }
    public long InitialIncome { get; set; }
    public int HomeGameIncome { get; set; }
    public int ContractYears { get; set; }
    public int IsActive { get; set; }
    public int TeamId { get; set; }
    public int SeasonId { get; set; }
    public string Description { get; set; }
    public int BonusPercent { get; set; }
}
