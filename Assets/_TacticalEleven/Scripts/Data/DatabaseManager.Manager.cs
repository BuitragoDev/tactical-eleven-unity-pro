using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manager y temporada activa: alta al elegir equipo, consultas y utilidades de
/// arranque (temporada actual, peores clubes para ProManager).
/// </summary>
public partial class DatabaseManager
{
    /// <summary>Crea o actualiza el manager (una sola fila por partida).</summary>
    public void SaveManager(ManagerDataDB manager)
    {
        if (!EnsureDb() || manager == null) return;
        if (manager.Id > 0) Db.Update(manager);
        else Db.Insert(manager);
    }

    /// <summary>Elimina managers de intentos abandonados antes de crear la partida.</summary>
    public void ClearAllManagers()
    {
        if (!EnsureDb()) return;
        Db.Execute("DELETE FROM games");
        Db.Execute("DELETE FROM seasons");
        Db.Execute("DELETE FROM managers");
    }

    /// <summary>
    /// Descarta una partida a medias (sin metadatos de slot): borra manager,
    /// temporada y partidos. No toca `saves.json`, así que nunca aparece en
    /// Cargar partida. Se usa al abandonar la creación antes del Dashboard.
    /// `closeSlot` se usa al volver al menú principal para no dejar la conexión abierta.
    /// </summary>
    public void DiscardPendingGame(bool closeSlot = false)
    {
        ClearAllManagers();
        if (closeSlot) CloseActiveSlot();
    }

    /// <summary>
    /// Temporada de arranque (año de inicio): desde julio cuenta el año en curso.
    /// </summary>
    public static int CurrentSeasonStartYear()
    {
        var now = DateTime.Now;
        return now.Month >= 7 ? now.Year : now.Year - 1;
    }

    /// <summary>Crea la temporada activa en fase de pretemporada.</summary>
    public SeasonDataDB CreateSeason(int managerId, int leagueId, int yearStart)
    {
        if (!EnsureDb()) return null;

        var league = GetLeagueById(leagueId);
        int teams = league != null && league.TeamCount > 0 ? league.TeamCount : 20;
        int totalMatchDays = Math.Max(0, (teams - 1) * 2);

        var preseason = new DateTime(yearStart, 7, 15);

        var season = new SeasonDataDB
        {
            YearStart = yearStart,
            YearEnd = yearStart + 1,
            IsActive = 1,
            CurrentMatchDay = CalendarHelper.DateToGameDay(yearStart, preseason),
            TotalMatchDays = totalMatchDays,
            Phase = "preseason",
            ManagerId = managerId,
            LeagueId = leagueId,
            CurrentDate = CalendarHelper.ToDb(preseason)
        };

        Db.Insert(season);
        return season;
    }

    public void UpdateSeason(SeasonDataDB season)
    {
        if (!EnsureDb() || season == null) return;
        Db.Update(season);
    }

    /// <summary>
    /// Clubes con menor media de las competiciones indicadas (por defecto 1ª y 2ª).
    /// Se usa para restringir la elección en ProManager.
    /// </summary>
    public List<TeamDataDB> GetWorstTeams(int count, params int[] leagueIds)
    {
        if (!EnsureDb()) return new List<TeamDataDB>();

        var ids = (leagueIds == null || leagueIds.Length == 0)
            ? new[] { 1, 2, 3, 4 }
            : leagueIds;

        // Filtrado en memoria: `Contains` sobre un array no siempre se traduce a SQL
        // en sqlite-net y aquí solo recorremos 236 clubes.
        return Db.Table<TeamDataDB>()
                 .ToList()
                 .Where(t => ids.Contains(t.LeagueId) && t.IsFilial == 0)
                 .OrderBy(t => t.Overall)
                 .Take(Mathf.Max(0, count))
                 .ToList();
    }
}
