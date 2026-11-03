using SQLite;

/// <summary>Resultado (`ok`/`failed`) de cada intento de migración.</summary>
[Table("schema_migration_attempts")]
public class SchemaMigrationAttemptData
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public string MigrationName { get; set; }
    public string Result { get; set; }
    public string Error { get; set; }
    public string AttemptedAt { get; set; }
}
