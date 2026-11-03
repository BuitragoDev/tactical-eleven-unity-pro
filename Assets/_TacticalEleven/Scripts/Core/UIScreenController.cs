using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Base de todos los controllers de pantalla. Pipeline único:
/// fullscreen → CacheReferences → LoadData → RegisterCallbacks → Refresh → localización.
/// Las pantallas sobreescriben solo lo que difiere y **nunca** inicializan en Start.
/// </summary>
public abstract class UIScreenController : MonoBehaviour
{
    protected UIDocument _document;
    protected VisualElement _root;

    protected ManagerDataDB _manager;
    protected TeamDataDB _myTeam;
    protected SeasonDataDB _season;

    /// <summary>Pantalla que representa este controller (para navegación y chrome).</summary>
    public virtual GameScreen ScreenId => GameScreen.Loading;

    protected virtual void Awake()
    {
        _document = GetComponent<UIDocument>();
    }

    protected virtual void OnEnable()
    {
        if (_document == null) _document = GetComponent<UIDocument>();
        _root = _document != null ? _document.rootVisualElement : null;
        if (_root == null) return;

        MakeFullscreen();
        CacheReferences();
        LoadData();
        RegisterCallbacks();
        Refresh();

        // Último paso: resolver tokens de localización (también los del chrome inyectado).
        LocalizationService.Apply(_root);

        if (CursorManager.Instance != null)
            CursorManager.Instance.SetDefaultCursor();
    }

    // ── Hooks ──────────────────────────────────────────────

    protected virtual void MakeFullscreen()
    {
        _root.style.position = Position.Absolute;
        _root.style.left = 0;
        _root.style.right = 0;
        _root.style.top = 0;
        _root.style.bottom = 0;
        _root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        _root.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
    }

    protected virtual void CacheReferences() { }

    /// <summary>
    /// Carga el contexto común (manager, equipo y temporada activos). Las pantallas
    /// que necesiten más datos lo amplían después de llamar a la base.
    /// </summary>
    protected virtual void LoadData()
    {
        var db = DatabaseManager.Instance;
        if (db == null || db.Db == null) return;

        _manager = db.GetActiveManager();
        if (_manager == null) return;

        _myTeam = db.GetTeamById(_manager.TeamId);
        _season = db.GetActiveSeason(_manager.Id);
    }

    /// <summary>Wiring de botones y eventos. La navegación se registra aquí.</summary>
    protected virtual void RegisterCallbacks() { }

    protected virtual void Refresh() { }

    // ── Utilidades comunes ─────────────────────────────────

    protected void PlayClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("click");
    }

    protected void RegisterHandCursor(VisualElement element)
    {
        if (CursorManager.Instance != null)
            CursorManager.Instance.RegisterHandCursor(element);
    }

    protected T Q<T>(string name) where T : VisualElement => _root?.Q<T>(name);

    protected void SetVisible(VisualElement element, bool visible)
    {
        if (element != null) element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
