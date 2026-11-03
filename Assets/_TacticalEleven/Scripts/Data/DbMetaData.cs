using System;
using System.Collections.Generic;
using SQLite;

/// <summary>Metadatos de la base de datos (huella del seed y versión del juego).</summary>
[Table("db_meta")]
public class DbMetaData
{
    [PrimaryKey] public int Id { get; set; }
    public string SeedFingerprint { get; set; }
    public string CreatedAt { get; set; }
    public string GameVersion { get; set; }
}
