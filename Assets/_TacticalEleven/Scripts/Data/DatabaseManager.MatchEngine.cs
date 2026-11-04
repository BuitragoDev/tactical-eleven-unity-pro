using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// F8 — Persistencia del motor de partido: construye los onces, simula, guarda el
/// resultado en `games`, los eventos en `match_events`, las estadísticas en
/// `player_match_stats` y acumula `player_season_stats`.
/// </summary>
public partial class DatabaseManager
{
    /// <summary>Prepara un equipo para simular (once y banquillo automáticos si no hay alineación).</summary>
    public MatchTeamSetup BuildMatchSetup(TeamDataDB team, bool isHome)
    {
        var players = GetPlayersByTeam(team.Id);
        var lineup = LineupBuilder.Build(team, players);

        return new MatchTeamSetup
        {
            Team = team,
            Starters = lineup.Starters,
            Bench = lineup.Bench,
            Formation = lineup.Formation,
            IsHome = isHome
        };
    }

    /// <summary>
    /// Simula un partido pendiente y lo persiste por completo. Devuelve null si el
    /// partido ya estaba jugado o faltan datos.
    /// </summary>
    public MatchSimulationResult SimulateGame(SeasonDataDB season, GameDataDB game)
    {
        if (!EnsureDb() || season == null || game == null) return null;
        if (game.IsPlayed == 1) return null;

        var homeTeam = GetTeamById(game.HomeTeamId);
        var awayTeam = GetTeamById(game.AwayTeamId);
        if (homeTeam == null || awayTeam == null) return null;

        var home = BuildMatchSetup(homeTeam, true);
        var away = BuildMatchSetup(awayTeam, false);

        if (home.Starters.Count < 11 || away.Starters.Count < 11) return null;

        ulong seed = (ulong)(game.Id * 397L + season.YearStart * 17L
                             + game.MatchDay * 31L + game.HomeTeamId * 7L);
        var random = new SeededRandomSource(seed);

        var result = FootballSimulator.Simulate(home, away, random);

        // ── Resultado ──────────────────────────────────────────────────────
        game.HomeScore = result.HomeScore;
        game.AwayScore = result.AwayScore;
        game.IsPlayed = 1;
        game.Attendance = homeTeam.StadiumCapacity > 0
            ? (int)(homeTeam.StadiumCapacity * (0.65f + random.Value * 0.3f))
            : 0;

        game.HomePossession = result.HomePossession;
        game.AwayPossession = result.AwayPossession;
        game.HomeShots = result.HomeShots;
        game.AwayShots = result.AwayShots;
        game.HomeShotsOnTarget = result.HomeShotsOnTarget;
        game.AwayShotsOnTarget = result.AwayShotsOnTarget;
        game.HomeCorners = result.HomeCorners;
        game.AwayCorners = result.AwayCorners;
        game.HomeFouls = result.HomeFouls;
        game.AwayFouls = result.AwayFouls;
        game.HomeYellowCards = result.HomeYellow;
        game.AwayYellowCards = result.AwayYellow;
        game.HomeRedCards = result.HomeRed;
        game.AwayRedCards = result.AwayRed;

        Db.Update(game);

        // ── Eventos ────────────────────────────────────────────────────────
        var events = result.Events
            .Select(e => new MatchEventDataDB
            {
                GameId = game.Id,
                Minute = e.Minute,
                EventType = (int)e.Type,
                PlayerId = e.PlayerId,
                SecondaryPlayerId = e.SecondaryPlayerId,
                TeamId = e.TeamId,
                Detail = e.Detail ?? ""
            })
            .ToList();

        if (events.Count > 0) Db.InsertAll(events);

        // ── Estadísticas individuales ──────────────────────────────────────
        var goalkeeperIds = new HashSet<int>(
            home.Starters.Concat(away.Starters)
                .Where(p => p.MainPosition == (int)PlayerPosition.GK)
                .Select(p => p.Id));

        var matchStats = result.Players
            .Where(p => p.Minutes > 0)
            .Select(p => new PlayerMatchStatsDB
            {
                GameId = game.Id,
                PlayerId = p.PlayerId,
                TeamId = p.TeamId,
                MinutesPlayed = p.Minutes,
                Rating = p.Rating,
                Goals = p.Goals,
                Assists = p.Assists,
                Shots = p.Shots,
                ShotsOnTarget = p.ShotsOnTarget,
                Passes = p.Passes,
                PassAccuracy = p.PassAccuracy,
                Tackles = p.Tackles,
                Interceptions = p.Interceptions,
                Fouls = p.Fouls,
                YellowCard = p.YellowCard,
                RedCard = p.RedCard,
                Saves = p.Saves,
                GoalsConceded = p.GoalsConceded
            })
            .ToList();

        if (matchStats.Count > 0) Db.InsertAll(matchStats);

        // ── Acumulado de temporada ─────────────────────────────────────────
        foreach (var stat in result.Players)
        {
            if (stat.Minutes <= 0) continue;
            UpsertSeasonStats(season, stat, result.MvpPlayerId,
                goalkeeperIds.Contains(stat.PlayerId));
        }

        return result;
    }

    /// <summary>
    /// Simula todos los partidos pendientes de la fecha actual de la temporada
    /// (incluidos los de otros equipos) y devuelve el resultado del club indicado.
    /// </summary>
    public bool PlayToday(SeasonDataDB season, int myTeamId, out MatchSimulationResult myResult)
    {
        myResult = null;
        if (!EnsureDb() || season == null) return false;

        var pending = GetGamesBySeason(season.Id)
            .Where(g => g.IsPlayed == 0)
            .OrderBy(g => g.Id)
            .ToList();

        var myGame = pending.FirstOrDefault(g =>
            g.MatchDate == season.CurrentDate
            && (g.HomeTeamId == myTeamId || g.AwayTeamId == myTeamId));

        if (myGame == null) return false;

        // Se simula la jornada completa (mismo tipo y número de jornada), no solo
        // el partido del jugador: así la clasificación queda coherente.
        var toPlay = pending
            .Where(g => g.Id == myGame.Id
                        || g.MatchDate == season.CurrentDate
                        || (g.GameType == myGame.GameType && g.MatchDay == myGame.MatchDay))
            .ToList();

        foreach (var game in toPlay)
        {
            var result = SimulateGame(season, game);
            if (result == null) continue;

            if (game.HomeTeamId == myTeamId || game.AwayTeamId == myTeamId)
                myResult = result;
        }

        return myResult != null;
    }

    // ── CONSULTAS ──────────────────────────────────────────────────────────

    public List<MatchEventDataDB> GetMatchEvents(int gameId)
    {
        if (!EnsureDb()) return new List<MatchEventDataDB>();
        return Db.Table<MatchEventDataDB>()
                 .Where(e => e.GameId == gameId)
                 .OrderBy(e => e.Minute)
                 .ThenBy(e => e.Id)
                 .ToList();
    }

    public List<PlayerMatchStatsDB> GetPlayerMatchStats(int gameId)
    {
        if (!EnsureDb()) return new List<PlayerMatchStatsDB>();
        return Db.Table<PlayerMatchStatsDB>()
                 .Where(s => s.GameId == gameId)
                 .ToList();
    }

    public List<PlayerSeasonStatsDB> GetPlayerSeasonStats(int teamId, int seasonYear)
    {
        if (!EnsureDb()) return new List<PlayerSeasonStatsDB>();
        return Db.Table<PlayerSeasonStatsDB>()
                 .Where(s => s.TeamId == teamId && s.SeasonYear == seasonYear)
                 .ToList();
    }

    // ── ACUMULADO ──────────────────────────────────────────────────────────

    private void UpsertSeasonStats(SeasonDataDB season, SimulatedPlayerStats stat,
                                   int mvpPlayerId, bool isGoalkeeper)
    {
        var row = Db.Table<PlayerSeasonStatsDB>()
                    .FirstOrDefault(s => s.PlayerId == stat.PlayerId
                                         && s.SeasonYear == season.YearStart);

        bool isNew = row == null;
        if (isNew)
        {
            row = new PlayerSeasonStatsDB
            {
                PlayerId = stat.PlayerId,
                SeasonYear = season.YearStart,
                TeamId = stat.TeamId
            };
        }

        int previousAppearances = row.Appearances;

        row.TeamId = stat.TeamId;
        row.Appearances = previousAppearances + 1;
        row.MinutesPlayed += stat.Minutes;
        row.Goals += stat.Goals;
        row.Assists += stat.Assists;
        row.YellowCards += stat.YellowCard;
        row.RedCards += stat.RedCard;
        row.Saves += stat.Saves;
        row.GoalsConceded += stat.GoalsConceded;

        if (stat.Started) row.Starts++;
        else row.Substitutions++;

        if (stat.PlayerId == mvpPlayerId) row.ManOfTheMatch++;

        if (isGoalkeeper && stat.Minutes >= 60 && stat.GoalsConceded == 0)
            row.CleanSheets++;

        row.AverageRating = previousAppearances == 0
            ? stat.Rating
            : (row.AverageRating * previousAppearances + stat.Rating) / (previousAppearances + 1);

        if (isNew) Db.Insert(row);
        else Db.Update(row);
    }
}
