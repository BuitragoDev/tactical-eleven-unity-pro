using SQLite;

[Table("awards_records")]
public class AwardsRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Season { get; set; }
    public string Winner { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
}
