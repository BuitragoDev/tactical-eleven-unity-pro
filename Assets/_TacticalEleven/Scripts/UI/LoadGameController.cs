using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Cargar partida: rejilla de slots con equipo, manager, temporada y jornada.
/// Cargar abre el slot y navega al Dashboard; borrar pide confirmación.
/// </summary>
public class LoadGameController : UIScreenController
{
    private VisualElement _slotsGrid;
    private VisualElement _slotsArea;
    private VisualElement _emptyState;
    private VisualElement _paginator;
    private Button _btnBack;

    private VisualElement _deleteOverlay;
    private VisualElement _deleteBox;
    private Button _btnDeleteYes;
    private Button _btnDeleteNo;

    private int _pendingDeleteSlot = -1;
    private Texture2D _calendarTex;
    private Texture2D _empleadoTex;

    public override GameScreen ScreenId => GameScreen.LoadGame;

    protected override void CacheReferences()
    {
        _slotsGrid = _root.Q<VisualElement>("SlotsGrid");
        _slotsArea = _root.Q<VisualElement>("SlotsArea");
        _emptyState = _root.Q<VisualElement>("EmptyState");
        _paginator = _root.Q<VisualElement>("SlotsPaginator");
        _btnBack = _root.Q<Button>("BtnBack");

        _deleteOverlay = _root.Q<VisualElement>("DeleteModalOverlay");
        _deleteBox = _root.Q<VisualElement>("DeleteModalBox");
        _btnDeleteYes = _root.Q<Button>("BtnDeleteYes");
        _btnDeleteNo = _root.Q<Button>("BtnDeleteNo");

        RegisterHandCursor(_btnBack);
        RegisterHandCursor(_btnDeleteYes);
        RegisterHandCursor(_btnDeleteNo);
    }

    protected override void LoadData()
    {
        base.LoadData();
        _calendarTex = Resources.Load<Texture2D>("Icons/calendario");
        _empleadoTex = Resources.Load<Texture2D>("Icons/empleado");
        GameSaveManager.CleanupAllOrphanDbs();
    }

    protected override void RegisterCallbacks()
    {
        _btnBack?.RegisterCallback<ClickEvent>(_ => { PlayClick(); ScreenManager.Instance.GoTo(GameScreen.MainMenu); });
        _btnDeleteYes?.RegisterCallback<ClickEvent>(_ => { PlayClick(); ConfirmDelete(); });
        _btnDeleteNo?.RegisterCallback<ClickEvent>(_ => { PlayClick(); CloseDeleteModal(); });
    }

    protected override void Refresh()
    {
        if (_slotsGrid == null) return;
        _slotsGrid.Clear();
        SetVisible(_paginator, false);
        CloseDeleteModal();

        var slots = GameSaveManager.GetAllSlots()
            .Where(s => s.exists && !string.IsNullOrEmpty(s.managerName))
            .OrderByDescending(s => s.slotNumber)
            .ToList();

        bool any = slots.Count > 0;
        SetVisible(_slotsArea, any);
        SetVisible(_emptyState, !any);
        if (!any) return;

        foreach (var slot in slots)
            _slotsGrid.Add(BuildSlotCard(slot));
    }

    private VisualElement BuildSlotCard(SaveSlotInfo slot)
    {
        var card = new VisualElement();
        card.AddToClassList("slot-card");

        // Foto de fondo de la tarjeta (estadio genérico; el escudo va aparte).
        var arena = new VisualElement();
        arena.AddToClassList("slot-arena-photo");
        var stadiumSprite = Resources.Load<Sprite>("Stadiums/medium");
        if (stadiumSprite != null) arena.style.backgroundImage = new StyleBackground(stadiumSprite);
        card.Add(arena);

        // Manager
        var managerRow = new VisualElement();
        managerRow.AddToClassList("slot-manager-row");
        if (_empleadoTex != null)
        {
            var icon = new VisualElement();
            icon.AddToClassList("slot-manager-icon");
            icon.style.backgroundImage = new StyleBackground(_empleadoTex);
            managerRow.Add(icon);
        }
        var managerLbl = new Label(slot.managerName ?? "Manager");
        managerLbl.AddToClassList("slot-manager-name");
        managerRow.Add(managerLbl);
        card.Add(managerRow);

        // Equipo: escudo + nombre + temporada
        var teamSection = new VisualElement();
        teamSection.AddToClassList("slot-team-section");

        var logo = new VisualElement();
        logo.AddToClassList("slot-team-logo");
        var sprite = Resources.Load<Sprite>($"Teams/Logos/64x64/{(string.IsNullOrEmpty(slot.teamLogo) ? "default" : slot.teamLogo)}");
        if (sprite != null) logo.style.backgroundImage = new StyleBackground(sprite);
        teamSection.Add(logo);

        var info = new VisualElement();
        info.AddToClassList("slot-team-info");

        var teamName = new Label(slot.teamName ?? "Sin equipo");
        teamName.AddToClassList("slot-team-name");
        info.Add(teamName);

        var season = new Label(string.IsNullOrEmpty(slot.seasonYear)
            ? LocalizationService.T("ui.load_game.sin_temporada")
            : slot.seasonYear);
        season.AddToClassList("slot-team-season");
        info.Add(season);

        teamSection.Add(info);
        card.Add(teamSection);

        // Detalle: liga · jornada · fecha
        var stats = new VisualElement();
        stats.AddToClassList("slot-stats");
        stats.Add(BuildStatRow(_calendarTex, LocalizationService.T("ui.load_game.jornada"),
            slot.currentMatchDay > 0 ? slot.currentMatchDay.ToString() : "—"));
        if (!string.IsNullOrEmpty(slot.leagueName))
            stats.Add(BuildStatRow(null, LocalizationService.T("ui.load_game.liga"), slot.leagueName));
        if (!string.IsNullOrEmpty(slot.currentDate))
            stats.Add(BuildStatRow(null, LocalizationService.T("ui.load_game.fecha"), slot.currentDate));
        card.Add(stats);

        // Modo
        var modeBadge = new VisualElement();
        modeBadge.AddToClassList("slot-mode-badge");
        var modeLbl = new Label(slot.gameMode == "promanager" ? "PROMANAGER" : "MANAGER");
        modeLbl.AddToClassList("slot-mode-label");
        modeBadge.Add(modeLbl);
        card.Add(modeBadge);

        // Acciones
        var actions = new VisualElement();
        actions.AddToClassList("slot-actions");

        var loadBtn = new Button { text = LocalizationService.T("ui.load_game.cargar") };
        loadBtn.AddToClassList("btn-load");
        int slotNumber = slot.slotNumber;
        loadBtn.RegisterCallback<ClickEvent>(_ => { PlayClick(); OnLoadSlot(slotNumber); });
        RegisterHandCursor(loadBtn);
        actions.Add(loadBtn);

        var deleteBtn = new Button { text = LocalizationService.T("ui.load_game.borrar_partida") };
        deleteBtn.AddToClassList("btn-delete");
        deleteBtn.RegisterCallback<ClickEvent>(_ => { PlayClick(); OnDeleteSlot(slotNumber); });
        RegisterHandCursor(deleteBtn);
        actions.Add(deleteBtn);

        card.Add(actions);
        return card;
    }

    private VisualElement BuildStatRow(Texture2D icon, string label, string value)
    {
        var row = new VisualElement();
        row.AddToClassList("slot-stat-row");

        if (icon != null)
        {
            var iconElem = new VisualElement();
            iconElem.AddToClassList("slot-stat-icon");
            iconElem.style.backgroundImage = new StyleBackground(icon);
            row.Add(iconElem);
        }

        var labelElem = new Label(label);
        labelElem.AddToClassList("slot-stat-label");
        row.Add(labelElem);

        var valueElem = new Label(value);
        valueElem.AddToClassList("slot-stat-value");
        row.Add(valueElem);

        return row;
    }

    // ── Acciones ───────────────────────────────────────────────────────────

    private void OnLoadSlot(int slotNumber)
    {
        var db = DatabaseManager.Instance;
        if (db == null || !db.InitSaveSlot(slotNumber))
        {
            ShowError(db != null ? db.LastInitError : null);
            return;
        }

        GameSaveManager.UpdateSlotFromDatabase(slotNumber);

        var manager = db.GetActiveManager();
        var mode = manager != null && manager.GameMode == "promanager"
            ? GameMode.ProManager : GameMode.Manager;

        ScreenManager.Instance.GoTo(GameScreen.Dashboard, mode);
    }

    private void OnDeleteSlot(int slotNumber)
    {
        _pendingDeleteSlot = slotNumber;
        if (_deleteOverlay != null)
        {
            _deleteOverlay.AddToClassList("modal-overlay--visible");
            _deleteBox?.AddToClassList("modal-box--visible");
        }
    }

    private void ConfirmDelete()
    {
        if (_pendingDeleteSlot <= 0) return;
        GameSaveManager.DeleteSave(_pendingDeleteSlot);
        _pendingDeleteSlot = -1;
        CloseDeleteModal();
        Refresh();
    }

    private void CloseDeleteModal()
    {
        if (_deleteOverlay == null) return;
        _deleteOverlay.RemoveFromClassList("modal-overlay--visible");
        _deleteBox?.RemoveFromClassList("modal-box--visible");
    }

    private void ShowError(string detail)
    {
        Debug.LogError($"[LoadGame] No se pudo cargar la partida: {detail}");
    }
}
