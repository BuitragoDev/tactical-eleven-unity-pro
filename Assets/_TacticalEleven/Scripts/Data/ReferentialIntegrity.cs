using System.Collections.Generic;
using SQLite;
using UnityEngine;

/// <summary>
/// Relaciones declaradas entre tablas y validador de huérfanos. Se usa para
/// detectar datos incoherentes sin meter claves foráneas físicas en un esquema
/// legado (misma decisión que Tactical Five).
/// </summary>
public static class ReferentialIntegrity
{
    public class Rule
    {
        public string Name;
        public string Child;
        public string ChildColumn;
        public string Parent;
        public string ParentColumn;
        /// <summary>Valor centinela permitido en la columna hija (p. ej. 0 = agente libre).</summary>
        public int Sentinel;
    }

    public static readonly Rule[] Rules =
    {
        new Rule { Name = "players.team", Child = "players", ChildColumn = "TeamId",
                   Parent = "teams", ParentColumn = "Id", Sentinel = 0 },
        new Rule { Name = "teams.league", Child = "teams", ChildColumn = "LeagueId",
                   Parent = "leagues", ParentColumn = "Id", Sentinel = 0 },
        new Rule { Name = "games.home", Child = "games", ChildColumn = "HomeTeamId",
                   Parent = "teams", ParentColumn = "Id", Sentinel = 0 },
        new Rule { Name = "games.away", Child = "games", ChildColumn = "AwayTeamId",
                   Parent = "teams", ParentColumn = "Id", Sentinel = 0 },
        new Rule { Name = "standings.team", Child = "standings", ChildColumn = "TeamId",
                   Parent = "teams", ParentColumn = "Id", Sentinel = 0 },
        new Rule { Name = "lineups.team", Child = "lineups", ChildColumn = "TeamId",
                   Parent = "teams", ParentColumn = "Id", Sentinel = 0 },
        new Rule { Name = "lineups.player", Child = "lineups", ChildColumn = "PlayerId",
                   Parent = "players", ParentColumn = "Id", Sentinel = 0 },
        new Rule { Name = "match_stats.game", Child = "player_match_stats", ChildColumn = "GameId",
                   Parent = "games", ParentColumn = "Id", Sentinel = 0 },
    };

    /// <summary>Lista de infracciones (vacía si todo está bien).</summary>
    public static List<string> Validate(SQLiteConnection db)
    {
        var problems = new List<string>();
        if (db == null) return problems;

        foreach (var rule in Rules)
        {
            if (!TableExists(db, rule.Child) || !TableExists(db, rule.Parent)) continue;

            string sql =
                $"SELECT COUNT(*) FROM {rule.Child} c " +
                $"LEFT JOIN {rule.Parent} p ON p.{rule.ParentColumn} = c.{rule.ChildColumn} " +
                $"WHERE c.{rule.ChildColumn} <> {rule.Sentinel} AND p.{rule.ParentColumn} IS NULL";

            try
            {
                int count = db.ExecuteScalar<int>(sql);
                if (count > 0)
                    problems.Add($"{rule.Name}: {count} fila(s) huérfana(s).");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[ReferentialIntegrity] Regla '{rule.Name}' no evaluada: {e.Message}");
            }
        }

        return problems;
    }

    static bool TableExists(SQLiteConnection db, string table)
    {
        var rows = db.Query<TableRow>(
            "SELECT name FROM sqlite_master WHERE type='table' AND name=?", table);
        return rows.Count > 0;
    }

    class TableRow
    {
        public string name { get; set; }
    }
}
