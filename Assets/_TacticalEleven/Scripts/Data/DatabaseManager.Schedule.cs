using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Calendario persistido: generación de la liga y de la pretemporada, guardado de
/// amistosos y consultas de partidos (próximo partido, jornada, equipo).
///
/// Las reglas puras viven en <see cref="FootballScheduleGenerator"/> y
/// <see cref="FootballCalendar"/>; este partial solo traduce a filas de `games`.
/// </summary>
public partial class DatabaseManager
{
    public const string GameTypeRegular = "regular";
    public const string GameTypePreseason = "preseason";
    public const string GameTypeCup = "cup";
    public const string GameTypeEurope = "europe";

    // ── GENERACIÓN ─────────────────────────────────────────────────────────

    /// <summary>
    /// Genera y guarda el calendario de liga de la temporada (doble vuelta).
    /// Devuelve el número de partidos o -1 si el calendario no es válido.
    /// Es idempotente: reemplaza los partidos `regular` anteriores.
    /// </summary>
    public int GenerateLeagueSchedule(SeasonDataDB season, IRandomSource random = null)
    {
        if (!EnsureDb() || season == null) return -1;

        var teams = GetTeamsByLeague(season.LeagueId);
        if (teams.Count < 2)
        {
            Debug.LogError($"[Schedule] La liga {season.LeagueId} no tiene equipos suficientes.");
            return -1;
        }

        var teamIds = teams.Select(t => t.Id).ToList();
        List<ScheduledFixture> fixtures;
        try
        {
            fixtures = FootballScheduleGenerator.GenerateLeague(
                teamIds, season.YearStart, season.LeagueId, random);
        }
        catch (Exception e)
        {
            Debug.LogError($"[Schedule] No se pudo generar el calendario: {e.Message}");
            return -1;
        }

        var validation = FootballScheduleGenerator.Validate(fixtures, teamIds);
        if (!validation.IsValid)
        {
            Debug.LogError($"[Schedule] Calendario inválido: {validation}");
            return -1;
        }

        var games = fixtures.Select(f => ToGame(season, f)).ToList();
        ReplaceGames(season.Id, GameTypeRegular, games);

        season.TotalMatchDays = fixtures.Max(f => f.MatchDay);
        UpdateSeason(season);

        Debug.Log($"[Schedule] Liga {season.LeagueId}: {games.Count} partidos en {season.TotalMatchDays} jornadas.");
        return games.Count;
    }

    /// <summary>
    /// Genera y guarda los amistosos de pretemporada por defecto (4, contra
    /// rivales de menor nivel). Devuelve cuántos se guardaron.
    /// </summary>
    public int GeneratePreseasonSchedule(SeasonDataDB season, int myTeamId, IRandomSource random = null)
    {
        if (!EnsureDb() || season == null) return 0;

        var myTeam = GetTeamById(myTeamId);
        if (myTeam == null) return 0;

        var candidates = PreseasonPlanner.CandidateOpponents(myTeam, GetAllTeams());
        var fixtures = FootballScheduleGenerator.GeneratePreseason(
            myTeamId, candidates, season.YearStart, random);

        var games = fixtures.Select(f => ToGame(season, f)).ToList();
        ReplaceGames(season.Id, GameTypePreseason, games);
        return games.Count;
    }

    /// <summary>
    /// Sustituye los amistosos de pretemporada por los elegidos en la pantalla.
    /// Devuelve cuántos quedaron guardados.
    /// </summary>
    public int ReplacePreseasonGames(SeasonDataDB season, IList<GameDataDB> games)
    {
        if (!EnsureDb() || season == null) return 0;

        var rows = (games ?? new List<GameDataDB>())
            .Where(g => g != null)
            .Select(g =>
            {
                g.SeasonId = season.Id;
                g.ManagerId = season.ManagerId;
                g.GameType = GameTypePreseason;
                g.CompetitionId = 0;
                if (string.IsNullOrEmpty(g.MatchDate))
                    g.MatchDate = CalendarHelper.ToDb(CalendarHelper.GameDayToDate(season.YearStart, g.MatchDay));
                return g;
            })
            .ToList();

        ReplaceGames(season.Id, GameTypePreseason, rows);
        return rows.Count;
    }

    private static GameDataDB ToGame(SeasonDataDB season, ScheduledFixture fixture)
    {
        return new GameDataDB
        {
            SeasonId = season.Id,
            MatchDay = fixture.MatchDay,
            MatchDate = CalendarHelper.ToDb(fixture.Date),
            HomeTeamId = fixture.HomeTeamId,
            AwayTeamId = fixture.AwayTeamId,
            HomeScore = 0,
            AwayScore = 0,
            IsPlayed = 0,
            GameType = fixture.GameType,
            ManagerId = season.ManagerId,
            Attendance = 0,
            CompetitionId = fixture.CompetitionId
        };
    }

    private void ReplaceGames(int seasonId, string gameType, List<GameDataDB> games)
    {
        var db = Db;
        bool own = !db.IsInTransaction;
        if (own) db.BeginTransaction();
        try
        {
            db.Execute("DELETE FROM games WHERE SeasonId = ? AND GameType = ?", seasonId, gameType);
            if (games.Count > 0) db.InsertAll(games);
            if (own) db.Commit();
        }
        catch
        {
            if (own) { try { db.Rollback(); } catch { } }
            throw;
        }
    }

    // ── CONSULTAS ──────────────────────────────────────────────────────────

    public List<GameDataDB> GetGamesBySeason(int seasonId)
    {
        if (!EnsureDb()) return new List<GameDataDB>();
        return Db.Table<GameDataDB>()
                 .Where(g => g.SeasonId == seasonId)
                 .OrderBy(g => g.MatchDay)
                 .ThenBy(g => g.MatchDate)
                 .ToList();
    }

    public List<GameDataDB> GetGamesByType(int seasonId, string gameType)
    {
        if (!EnsureDb()) return new List<GameDataDB>();
        return Db.Table<GameDataDB>()
                 .Where(g => g.SeasonId == seasonId && g.GameType == gameType)
                 .OrderBy(g => g.MatchDay)
                 .ThenBy(g => g.MatchDate)
                 .ToList();
    }

    public List<GameDataDB> GetGamesForTeam(int seasonId, int teamId)
    {
        if (!EnsureDb()) return new List<GameDataDB>();
        return Db.Table<GameDataDB>()
                 .Where(g => g.SeasonId == seasonId && (g.HomeTeamId == teamId || g.AwayTeamId == teamId))
                 .OrderBy(g => g.MatchDay)
                 .ThenBy(g => g.MatchDate)
                 .ToList();
    }

    public List<GameDataDB> GetGamesByMatchDay(int seasonId, int matchDay)
    {
        if (!EnsureDb()) return new List<GameDataDB>();
        return Db.Table<GameDataDB>()
                 .Where(g => g.SeasonId == seasonId && g.MatchDay == matchDay)
                 .OrderBy(g => g.MatchDate)
                 .ToList();
    }

    /// <summary>
    /// Próximo partido de un equipo desde una fecha (incluida). Si no se indica
    /// fecha, usa el `CurrentDate` de la temporada.
    /// </summary>
    public GameDataDB GetNextGame(int seasonId, int teamId, string fromDate = null)
    {
        if (!EnsureDb()) return null;

        var season = Db.Table<SeasonDataDB>().FirstOrDefault(s => s.Id == seasonId);
        string date = string.IsNullOrEmpty(fromDate)
            ? (season?.CurrentDate ?? "")
            : fromDate;

        var games = GetGamesForTeam(seasonId, teamId)
            .Where(g => g.IsPlayed == 0 && (string.IsNullOrEmpty(date) || string.CompareOrdinal(g.MatchDate, date) >= 0))
            .OrderBy(g => g.MatchDate)
            .ThenBy(g => g.MatchDay)
            .ToList();

        return games.FirstOrDefault();
    }

    /// <summary>Primer partido de liga (jornada 1) de la temporada.</summary>
    public GameDataDB GetFirstLeagueGame(int seasonId)
    {
        if (!EnsureDb()) return null;
        return Db.Table<GameDataDB>()
                 .Where(g => g.SeasonId == seasonId && g.GameType == GameTypeRegular && g.MatchDay == 1)
                 .OrderBy(g => g.MatchDate)
                 .FirstOrDefault();
    }
}
