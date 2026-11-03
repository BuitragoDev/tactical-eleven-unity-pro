using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public static class GameSaveManager
{
    public const int MAX_SLOTS = 5;

    /// <summary>Solo tests: raíz de guardado alternativa (no toca las partidas reales).</summary>
    public static string BaseDirOverride { get; set; }

    private static string BaseDir => string.IsNullOrEmpty(BaseDirOverride)
        ? Path.Combine(Application.persistentDataPath, "TacticalEleven", "saves")
        : BaseDirOverride;
    private static string MetaPath => Path.Combine(BaseDir, "saves.json");

    private static void EnsureBaseDir()
    {
        if (!Directory.Exists(BaseDir))
            Directory.CreateDirectory(BaseDir);
    }

    public static int CountExistingSlots()
    {
        return GetAllSlots().Count(s => s.exists && !string.IsNullOrEmpty(s.managerName) && !string.IsNullOrEmpty(s.teamName));
    }

    public static bool HasFreeSlot()
    {
        return CountExistingSlots() < MAX_SLOTS;
    }

    public static int FindNextAvailableSlot()
    {
        var existing = GetAllSlots();
        int slot = 1;
        while (existing.Any(s => s.slotNumber == slot && s.exists))
            slot++;
        return slot;
    }

    public static void CleanupOrphanDb(int slotNumber)
    {
        string dbPath = GetSaveDbPath(slotNumber);
        var slot = GetSlot(slotNumber);
        bool hasValidMeta = slot != null && slot.exists;
        if (!hasValidMeta && File.Exists(dbPath))
        {
            try { File.Delete(dbPath); } catch { }
            Debug.Log($"[GameSaveManager] DB huérfana eliminada: slot {slotNumber}");
        }
    }

    public static string GetSaveDbPath(int slotNumber)
    {
        return Path.Combine(BaseDir, $"save_{slotNumber}.db");
    }

    public static void CleanupAllOrphanDbs()
    {
        if (!Directory.Exists(BaseDir)) return;
        var metaSlots = GetAllSlots();
        var dbFiles = Directory.GetFiles(BaseDir, "save_*.db");
        foreach (var dbPath in dbFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(dbPath);
            if (!fileName.StartsWith("save_")) continue;
            if (!int.TryParse(fileName.Substring(5), out int slotNum)) continue;

            var slot = metaSlots.FirstOrDefault(s => s.slotNumber == slotNum);
            bool hasValidMeta = slot != null && slot.exists && !string.IsNullOrEmpty(slot.managerName);
            if (!hasValidMeta)
            {
                try { File.Delete(dbPath); } catch { }
                Debug.Log($"[GameSaveManager] DB huérfana eliminada: slot {slotNum}");
            }
        }
    }

    public static List<SaveSlotInfo> GetAllSlots()
    {
        EnsureBaseDir();
        var slots = new List<SaveSlotInfo>();

        if (File.Exists(MetaPath))
        {
            try
            {
                string json = File.ReadAllText(MetaPath);
                var meta = JsonUtility.FromJson<SaveMeta>(json);
                if (meta?.slots != null)
                    slots = meta.slots.ToList();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GameSaveManager] Error leyendo metadatos: {e.Message}");
            }
        }

        var result = new List<SaveSlotInfo>();
        foreach (var slot in slots)
        {
            string dbPath = GetSaveDbPath(slot.slotNumber);
            bool fileExists = File.Exists(dbPath);
            if (slot.exists && !fileExists)
                slot.exists = false;
            result.Add(slot);
        }

        return result.OrderBy(s => s.slotNumber).ToList();
    }

    public static SaveSlotInfo GetSlot(int slotNumber)
    {
        return GetAllSlots().FirstOrDefault(s => s.slotNumber == slotNumber);
    }

    public static void SaveSlotInfo(SaveSlotInfo slot)
    {
        EnsureBaseDir();
        var all = GetAllSlots();
        int idx = all.FindIndex(s => s.slotNumber == slot.slotNumber);
        if (idx >= 0) all[idx] = slot;
        else all.Add(slot);

        var meta = new SaveMeta { slots = all.ToArray() };
        string json = JsonUtility.ToJson(meta, true);
        File.WriteAllText(MetaPath, json);
    }

    public static void DeleteSave(int slotNumber)
    {
        string dbPath = GetSaveDbPath(slotNumber);
        if (File.Exists(dbPath))
        {
            try { File.Delete(dbPath); } catch { }
        }

        var slot = GetSlot(slotNumber);
        if (slot != null)
        {
            slot.exists = false;
            slot.managerName = null;
            slot.teamName = null;
            slot.teamId = 0;
            slot.teamLogo = null;
            slot.seasonYear = null;
            slot.currentDate = null;
            slot.lastPlayedRealDate = null;
            slot.currentMatchDay = 0;
            slot.gameMode = null;
            slot.leagueName = null;
            SaveSlotInfo(slot);
        }
    }

    public static void UpdateSlotFromDatabase(int slotNumber)
    {
        var manager = DatabaseManager.Instance.GetActiveManager();
        var team = manager != null ? DatabaseManager.Instance.GetTeamById(manager.TeamId) : null;
        var season = manager != null ? DatabaseManager.Instance.GetActiveSeason(manager.Id) : null;

        var slot = GetSlot(slotNumber) ?? new SaveSlotInfo { slotNumber = slotNumber };
        slot.exists = true;
        slot.managerName = manager?.Name ?? "Manager";
        slot.teamName = team?.Name ?? "Sin equipo";
        slot.teamId = team?.Id ?? 0;
        slot.teamLogo = team?.ShortName ?? "";
        slot.seasonYear = season != null ? $"{season.YearStart}-{season.YearEnd}" : "2026-2027";
        slot.currentMatchDay = season?.CurrentMatchDay ?? 0;
        slot.gameMode = manager?.GameMode ?? "manager";
        slot.lastPlayedRealDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        if (season != null && !string.IsNullOrEmpty(season.CurrentDate))
            slot.currentDate = DateTime.Parse(season.CurrentDate).ToString("dd/MM/yyyy");
        else
            slot.currentDate = "";

        if (team != null)
        {
            var league = DatabaseManager.Instance.GetLeagueById(team.LeagueId);
            slot.leagueName = league?.Name ?? "";
        }

        SaveSlotInfo(slot);
    }

    [Serializable]
    private class SaveMeta
    {
        public SaveSlotInfo[] slots;
    }
}
