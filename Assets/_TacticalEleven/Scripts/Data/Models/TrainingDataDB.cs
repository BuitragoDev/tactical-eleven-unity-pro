using SQLite;

[Table("player_training")]
public class TrainingDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public string Attribute { get; set; }
    public int StartDay { get; set; }
    public int Duration { get; set; }
    public int Completed { get; set; }
}
