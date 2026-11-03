using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using UnityEngine;

/// <summary>
/// Migraciones del esquema. Son **aditivas** (columnas nuevas con
/// <c>ALTER TABLE</c>) o migraciones de datos registradas por nombre en
/// `schema_migrations`. `PRAGMA user_version` solo avanza si todo termina bien.
/// </summary>
public partial class DatabaseManager
{
    public const int SchemaVersion = 1;

    void RunMigrations()
    {
        var db = Db;
        bool own = !db.IsInTransaction;
        if (own) db.BeginTransaction();

        try
        {
            RunMigrationsCore(db);
            if (own) db.Commit();
            TryRecordMigrationAttempt("RunMigrations", true, null);
        }
        catch (Exception e)
        {
            if (own) { try { db.Rollback(); } catch { } }
            LastMigrationError = e.Message;
            TryRecordMigrationAttempt("RunMigrations", false, e.Message);
            Debug.LogError($"[DB] Migración fallida: {e}");
            throw new DatabaseMigrationException("RunMigrations", e);
        }
    }

    /// <summary>Registra el intento en `schema_migration_attempts` (nunca lanza).</summary>
    void TryRecordMigrationAttempt(string name, bool ok, string error)
    {
        try
        {
            var db = Db;
            if (db == null) return;
            db.Insert(new SchemaMigrationAttemptData
            {
                MigrationName = name,
                Result = ok ? "ok" : "failed",
                Error = error,
                AttemptedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[DB] No se pudo registrar el intento de migración: {e.Message}");
        }
    }

    void RunMigrationsCore(SQLiteConnection db)
    {
        // Columnas añadidas después del primer esquema (partidas antiguas).
        EnsureColumn(db, "teams", "Logo", "TEXT");
        EnsureColumn(db, "teams", "StadiumImage", "TEXT");
        EnsureColumn(db, "teams", "IsFilial", "INTEGER DEFAULT 0");
        EnsureColumn(db, "teams", "ParentClubId", "INTEGER DEFAULT 0");
        EnsureColumn(db, "teams", "JerseyHome", "TEXT");
        EnsureColumn(db, "teams", "JerseyAway", "TEXT");

        EnsureColumn(db, "leagues", "ShortName", "TEXT");
        EnsureColumn(db, "leagues", "Type", "TEXT");
        EnsureColumn(db, "leagues", "Tier", "INTEGER DEFAULT 0");
        EnsureColumn(db, "leagues", "GroupIndex", "INTEGER DEFAULT 0");
        EnsureColumn(db, "leagues", "EuropeSlots", "INTEGER DEFAULT 0");
        EnsureColumn(db, "leagues", "Logo", "TEXT");

        EnsureColumn(db, "players", "ContractEndSeason", "INTEGER DEFAULT 0");
        EnsureColumn(db, "players", "IsOnLoan", "INTEGER DEFAULT 0");
        EnsureColumn(db, "players", "LoanFromTeamId", "INTEGER DEFAULT 0");
        EnsureColumn(db, "players", "LoanEndSeason", "INTEGER DEFAULT 0");
        EnsureColumn(db, "players", "TransferListed", "INTEGER DEFAULT 0");
        EnsureColumn(db, "players", "IsYouth", "INTEGER DEFAULT 0");
        EnsureColumn(db, "players", "IsFilialEligible", "INTEGER DEFAULT 0");
        EnsureColumn(db, "players", "SeasonsWithTeam", "INTEGER DEFAULT 0");
        EnsureColumn(db, "players", "LastTeamId", "INTEGER DEFAULT 0");

        EnsureColumn(db, "lineups", "Slot", "INTEGER DEFAULT 0");
        EnsureColumn(db, "lineups", "SortOrder", "INTEGER DEFAULT 0");

        EnsureColumn(db, "games", "CompetitionId", "INTEGER DEFAULT 0");
        EnsureColumn(db, "games", "Round", "TEXT");
        EnsureColumn(db, "games", "SeriesLabel", "TEXT");
        EnsureColumn(db, "games", "NeutralVenue", "INTEGER DEFAULT 0");

        EnsureColumn(db, "awards_records", "PlayerId", "INTEGER DEFAULT 0");
        EnsureColumn(db, "golden_boot_records", "PlayerId", "INTEGER DEFAULT 0");

        // Migraciones de datos con nombre (se marcan cuando terminan).
        // Ejemplo de patrón:
        // if (!IsMigrationApplied(db, "normalize_team_names")) { ...; MarkMigrationApplied(db, "normalize_team_names"); }

        db.Execute($"PRAGMA user_version = {SchemaVersion}");
    }

    /// <summary>Añade una columna si no existe (idempotente).</summary>
    void EnsureColumn(SQLiteConnection db, string table, string column, string definition)
    {
        try
        {
            var columns = db.Query<ColumnInfo>($"PRAGMA table_info({table})");
            if (columns.Any(c => string.Equals(c.name, column, StringComparison.OrdinalIgnoreCase)))
                return;

            db.Execute($"ALTER TABLE {table} ADD COLUMN {column} {definition}");
            Debug.Log($"[DB] Migración: {table}.{column} añadida");
        }
        catch (Exception e)
        {
            throw new DatabaseMigrationException($"add_column:{table}.{column}", e);
        }
    }

    class ColumnInfo
    {
        public int cid { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int notnull { get; set; }
        public string dflt_value { get; set; }
        public int pk { get; set; }
    }

    public bool IsMigrationApplied(SQLiteConnection db, string name)
        => db.Table<SchemaMigrationData>().Any(m => m.Name == name);

    public void MarkMigrationApplied(SQLiteConnection db, string name)
        => db.InsertOrReplace(new SchemaMigrationData
        {
            Name = name,
            AppliedAt = DateTime.UtcNow.ToString("yyyy-MM-dd")
        });
}

/// <summary>Fallo de una migración obligatoria: bloquea la apertura del slot.</summary>
public class DatabaseMigrationException : Exception
{
    public string MigrationName { get; }

    public DatabaseMigrationException(string migrationName, Exception inner)
        : base($"Migración '{migrationName}' fallida: {inner?.Message}", inner)
    {
        MigrationName = migrationName;
    }
}
