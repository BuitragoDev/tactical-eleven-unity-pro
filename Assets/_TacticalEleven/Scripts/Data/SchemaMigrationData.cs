using SQLite;

/// <summary>Migración de datos ya aplicada en este slot.</summary>
[Table("schema_migrations")]
public class SchemaMigrationData
{
    [PrimaryKey] public string Name { get; set; }
    public string AppliedAt { get; set; }
}
