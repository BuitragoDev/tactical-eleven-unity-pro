using System;
using System.Linq;
using SQLite;
using UnityEngine;

/// <summary>
/// Seed de los datos estáticos. Cada bloque solo se ejecuta si su tabla está
/// vacía, de modo que el template y las partidas nuevas se puedan rellenar en
/// fases distintas sin duplicar filas.
/// </summary>
public partial class DatabaseManager
{
    /// <summary>Siembra lo que falte. Idempotente: se puede llamar varias veces.</summary>
    public void SeedStaticDataIfNeeded()
    {
        if (!EnsureDb()) return;

        var db = Db;
        bool own = !db.IsInTransaction;
        if (own) db.BeginTransaction();

        try
        {
            if (db.Table<LeagueDataDB>().Count() == 0) SeedCompetitions();
            if (db.Table<TeamDataDB>().Count() == 0) SeedTeams();
            if (db.Table<PlayerDataDB>().Count() == 0) SeedPlayers();
            if (db.Table<FinalsRecord>().Count() == 0) SeedPalmares();
            if (db.Table<GoldenBootRecord>().Count() == 0) SeedGoldenBoot();
            if (db.Table<ManagerDataDB>().Count() == 0) SeedManagerDefaults();
            if (db.Table<TeamSettingsDataDB>().Count() == 0) EnsureTeamSettingsForAllTeams();

            if (own) db.Commit();
        }
        catch
        {
            if (own) { try { db.Rollback(); } catch { } }
            throw;
        }
    }

    // ── Bloques de seed (se completan en F2 y F3) ──────────────────────────

    void SeedCompetitions()
    {
        var leagues = CompetitionSeeder.Build();
        Db.InsertAll(leagues);
        Debug.Log($"[DB] {leagues.Count} competiciones sembradas.");
    }

    void SeedTeams()
    {
        var teams = TeamSeeder.Build();
        Db.InsertAll(teams);
        Debug.Log($"[DB] {teams.Count} clubes sembrados.");
    }

    void SeedPlayers()
    {
        PlayerSeeder.Seed(Db);
    }

    void SeedPalmares()
    {
        Db.InsertAll(PalmaresSeeder.FinalsData);
        if (Db.Table<AwardsRecord>().Count() == 0)
            Db.InsertAll(PalmaresSeeder.AwardsData);
        Debug.Log($"[DB] {PalmaresSeeder.FinalsData.Count} finales históricas sembradas.");
    }

    void SeedGoldenBoot()
    {
        Db.InsertAll(GoldenBootSeeder.Data);
        Debug.Log($"[DB] {GoldenBootSeeder.Data.Count} Pichichis históricos sembrados.");
    }

    void SeedManagerDefaults()
    {
        // F4: el manager se crea al elegir equipo; aquí no hay nada que sembrar.
    }

    /// <summary>Crea `team_settings` para todos los clubes (F12 las usa).</summary>
    void EnsureTeamSettingsForAllTeams()
    {
        var db = Db;
        foreach (var team in db.Table<TeamDataDB>().ToList())
        {
            if (db.Table<TeamSettingsDataDB>().Any(s => s.TeamId == team.Id)) continue;
            db.Insert(new TeamSettingsDataDB { TeamId = team.Id });
        }
    }
}
