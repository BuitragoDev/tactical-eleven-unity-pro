using SQLite;

[Table("finals_records")]
public class FinalsRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Season { get; set; }
    public int CompetitionId { get; set; }
    public int ChampionTeamId { get; set; }
    public int FinalistTeamId { get; set; }
    public string Result { get; set; }
}
