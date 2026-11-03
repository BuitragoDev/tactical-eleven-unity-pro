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
        FilialRules.Apply(teams);
        Db.InsertAll(teams);
        Debug.Log($"[DB] {teams.Count} clubes sembrados ({teams.Count(t => t.IsFilial == 1)} filiales).");
    }

    void SeedPlayers()
    {
        PlayerSeeder.Seed(Db);
        EnsurePlayerContracts();
    }

    /// <summary>
    /// Red de seguridad para datos incompletos del seed: todo profesional debe tener
    /// valor, salario y cláusula. (El seed tenía 3 llamadas con los campos
    /// desplazados; ya están corregidas, esto cubre futuros seeds.)
    /// </summary>
    void EnsurePlayerContracts()
    {
        var pending = Db.Table<PlayerDataDB>()
            .Where(p => p.ReleaseClause == 0 || p.Wage <= 0 || p.Value <= 1)
            .ToList();

        foreach (var player in pending)
        {
            if (player.Value <= 1) player.Value = 100_000L;
            if (player.Wage <= 0) player.Wage = 1_000L;
            if (player.ReleaseClause == 0)
                player.ReleaseClause = Math.Max(player.Value * 2, 300_000L);
            Db.Update(player);
        }

        if (pending.Count > 0)
            Debug.Log($"[DB] Contrato normalizado en {pending.Count} jugadores del seed.");
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
