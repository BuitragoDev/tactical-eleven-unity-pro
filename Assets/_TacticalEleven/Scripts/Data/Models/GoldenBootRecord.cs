using SQLite;

[Table("golden_boot_records")]
public class GoldenBootRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Season { get; set; }
    public string Winner { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public int Goals { get; set; }
}
