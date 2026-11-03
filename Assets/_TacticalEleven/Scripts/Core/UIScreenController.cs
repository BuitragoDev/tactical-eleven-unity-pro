using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Base de todos los controllers de pantalla. Pipeline único:
/// fullscreen → CacheReferences → InitConfigModal → LoadData → RegisterCallbacks
/// → Refresh → localización. Las pantallas sobreescriben solo lo que difiere y
/// **nunca** inicializan en Start.
/// </summary>
public abstract class UIScreenController : MonoBehaviour
{
    protected const string SimModePrefKey = "TF_SimMode";

    protected UIDocument _document;
    protected VisualElement _root;

    protected ManagerDataDB _manager;
    protected TeamDataDB _myTeam;
    protected SeasonDataDB _season;

    // ── Modal de configuración (solo si la pantalla lo incluye) ────────────
    protected VisualElement _configModalOverlay;
    protected VisualElement _configModalBox;
    protected CustomSlider _configSliderMaster;
    protected CustomSlider _configSliderMusic;
    protected CustomSlider _configSliderSFX;
    protected Label _configLabelMaster;
    protected Label _configLabelMusic;
    protected Label _configLabelSFX;
    protected Button _configBtnQualityLow;
    protected Button _configBtnQualityMedium;
    protected Button _configBtnQualityHigh;
    protected Button _configBtnQualityUltra;
    protected Button _configBtnSimDirect;
    protected Button _configBtnSimPlay;
    protected Button _configBtnSimVisual;
    protected Button _btnConfigCerrar;

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
        InitConfigModal();
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

    // ── Modal de configuración ─────────────────────────────

    /// <summary>
    /// Inicializa el modal compartido de configuración si la pantalla lo incluye.
    /// Audio, calidad gráfica y modo de simulación.
    /// </summary>
    protected virtual void InitConfigModal()
    {
        _configModalOverlay = _root.Q<VisualElement>("ConfigModalOverlay");
        if (_configModalOverlay == null) return;

        _configModalBox = _root.Q<VisualElement>("ConfigModalBox");
        _btnConfigCerrar = _root.Q<Button>("ConfigBtnCerrar");

        var sliderMaster = _root.Q<VisualElement>("ConfigSliderMaster");
        if (sliderMaster != null)
        {
            _configSliderMaster = new CustomSlider(sliderMaster,
                _root.Q<VisualElement>("ConfigFillMaster"),
                _root.Q<VisualElement>("ConfigDraggerMaster"));
            _configSliderMusic = new CustomSlider(
                _root.Q<VisualElement>("ConfigSliderMusic"),
                _root.Q<VisualElement>("ConfigFillMusic"),
                _root.Q<VisualElement>("ConfigDraggerMusic"));
            _configSliderSFX = new CustomSlider(
                _root.Q<VisualElement>("ConfigSliderSFX"),
                _root.Q<VisualElement>("ConfigFillSFX"),
                _root.Q<VisualElement>("ConfigDraggerSFX"));

            _configSliderMaster.OnValueChanged = v =>
            {
                AudioManager.Instance?.SetMasterVolume(v);
                UpdateConfigLabels();
            };
            _configSliderMusic.OnValueChanged = v =>
            {
                AudioManager.Instance?.SetMusicVolume(v);
                UpdateConfigLabels();
            };
            _configSliderSFX.OnValueChanged = v =>
            {
                AudioManager.Instance?.SetSFXVolume(v);
                UpdateConfigLabels();
            };

            RegisterHandCursor(_configSliderMaster.Container);
            RegisterHandCursor(_configSliderMusic.Container);
            RegisterHandCursor(_configSliderSFX.Container);
        }

        _configLabelMaster = _root.Q<Label>("ConfigLabelMaster");
        _configLabelMusic = _root.Q<Label>("ConfigLabelMusic");
        _configLabelSFX = _root.Q<Label>("ConfigLabelSFX");

        _configBtnQualityLow = _root.Q<Button>("ConfigBtnQualityLow");
        _configBtnQualityMedium = _root.Q<Button>("ConfigBtnQualityMedium");
        _configBtnQualityHigh = _root.Q<Button>("ConfigBtnQualityHigh");
        _configBtnQualityUltra = _root.Q<Button>("ConfigBtnQualityUltra");

        _configBtnQualityLow?.RegisterCallback<ClickEvent>(_ => { PlayClick(); SelectConfigQuality(0); });
        _configBtnQualityMedium?.RegisterCallback<ClickEvent>(_ => { PlayClick(); SelectConfigQuality(1); });
        _configBtnQualityHigh?.RegisterCallback<ClickEvent>(_ => { PlayClick(); SelectConfigQuality(2); });
        _configBtnQualityUltra?.RegisterCallback<ClickEvent>(_ => { PlayClick(); SelectConfigQuality(3); });

        _configBtnSimDirect = _root.Q<Button>("ConfigBtnSimDirect");
        _configBtnSimPlay = _root.Q<Button>("ConfigBtnSimPlay");
        _configBtnSimVisual = _root.Q<Button>("ConfigBtnSimVisual");

        _configBtnSimDirect?.RegisterCallback<ClickEvent>(_ => { PlayClick(); SelectConfigSimMode((int)SimMode.Direct); });
        _configBtnSimPlay?.RegisterCallback<ClickEvent>(_ => { PlayClick(); SelectConfigSimMode((int)SimMode.PlayByPlay); });
        _configBtnSimVisual?.RegisterCallback<ClickEvent>(_ => { PlayClick(); SelectConfigSimMode((int)SimMode.Visual2D); });

        _btnConfigCerrar?.RegisterCallback<ClickEvent>(_ => { PlayClick(); CloseConfigModal(); });

        foreach (var b in new[] { _configBtnQualityLow, _configBtnQualityMedium, _configBtnQualityHigh,
                                  _configBtnQualityUltra, _configBtnSimDirect, _configBtnSimPlay,
                                  _configBtnSimVisual, _btnConfigCerrar })
            RegisterHandCursor(b);

        UpdateConfigLabels();
        UpdateConfigQualityButtons(Mathf.Clamp(QualitySettings.GetQualityLevel(), 0, 3));
        UpdateConfigSimButtons(GetSimMode());
    }

    protected virtual void OpenConfigModal()
    {
        if (_configModalOverlay == null) return;

        if (CursorManager.Instance != null)
            CursorManager.Instance.SetDefaultCursor();

        var am = AudioManager.Instance;
        if (am != null)
        {
            _configSliderMaster?.SetValueWithoutNotify(am.MasterVolume);
            _configSliderMusic?.SetValueWithoutNotify(am.MusicVolume);
            _configSliderSFX?.SetValueWithoutNotify(am.SFXVolume);
            UpdateConfigLabels();
        }

        UpdateConfigQualityButtons(Mathf.Clamp(QualitySettings.GetQualityLevel(), 0, 3));
        UpdateConfigSimButtons(GetSimMode());

        // El fondo opaco lo pone la clase compartida .modal-overlay--visible.
        _configModalOverlay.AddToClassList("modal-overlay--visible");
        _configModalBox?.AddToClassList("modal-box--visible");
    }

    protected virtual void CloseConfigModal()
    {
        if (_configModalOverlay == null) return;
        _configModalOverlay.RemoveFromClassList("modal-overlay--visible");
        _configModalBox?.RemoveFromClassList("modal-box--visible");
    }

    protected void UpdateConfigLabels()
    {
        var am = AudioManager.Instance;
        if (am == null) return;
        if (_configLabelMaster != null)
            _configLabelMaster.text = $"{Mathf.RoundToInt(am.MasterVolume * 100)}%";
        if (_configLabelMusic != null)
            _configLabelMusic.text = $"{Mathf.RoundToInt(am.MusicVolume * 100)}%";
        if (_configLabelSFX != null)
            _configLabelSFX.text = $"{Mathf.RoundToInt(am.SFXVolume * 100)}%";
    }

    private void SelectConfigQuality(int index)
    {
        AudioManager.Instance?.SetQualityLevel(index);
        UpdateConfigQualityButtons(index);
    }

    protected void UpdateConfigQualityButtons(int activeIndex)
    {
        var buttons = new[] { _configBtnQualityLow, _configBtnQualityMedium, _configBtnQualityHigh, _configBtnQualityUltra };
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;
            buttons[i].EnableInClassList("settings-quality-btn--active", i == activeIndex);
        }
    }

    protected void SelectConfigSimMode(int mode)
    {
        PlayerPrefs.SetInt(SimModePrefKey, mode);
        PlayerPrefs.Save();
        UpdateConfigSimButtons(mode);
    }

    protected void UpdateConfigSimButtons(int activeIndex)
    {
        _configBtnSimDirect?.EnableInClassList("settings-quality-btn--active", activeIndex == (int)SimMode.Direct);
        _configBtnSimPlay?.EnableInClassList("settings-quality-btn--active", activeIndex == (int)SimMode.PlayByPlay);
        _configBtnSimVisual?.EnableInClassList("settings-quality-btn--active", activeIndex == (int)SimMode.Visual2D);
    }

    /// <summary>Modo de simulación guardado (0 directa, 1 play-by-play, 2 visual 2D).</summary>
    public static int GetSimMode() => PlayerPrefs.GetInt(SimModePrefKey, (int)SimMode.Direct);

    // ── Utilidades comunes ─────────────────────────────────

    protected void PlayClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("click");
    }

    protected void RegisterHandCursor(VisualElement element)
    {
        if (element == null) return;
        if (CursorManager.Instance != null)
            CursorManager.Instance.RegisterHandCursor(element);
    }

    protected T Q<T>(string name) where T : VisualElement => _root?.Q<T>(name);

    protected void SetVisible(VisualElement element, bool visible)
    {
        if (element != null) element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
