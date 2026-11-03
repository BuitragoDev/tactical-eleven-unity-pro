using SQLite;

[Table("lineups")]
public class LineupDataDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int PlayerId { get; set; }
    /// <summary>Puesto en el campo (0..10) para el once inicial.</summary>
    public int FieldSlot { get; set; }
    /// <summary>0 = once, 1 = banquillo, 2 = no convocado.</summary>
    public int Slot { get; set; }
    /// <summary>Orden dentro del banquillo/convocatoria.</summary>
    public int SortOrder { get; set; }
}
