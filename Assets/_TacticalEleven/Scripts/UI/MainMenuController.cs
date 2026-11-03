using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Menú principal: Liga Manager, ProManager (con modal de restricciones), Cargar
/// partida, Editor, aviso legal, configuración y salida.
/// Crear partida = buscar hueco, limpiar restos, abrir el slot y elegir equipo (F5).
/// </summary>
public class MainMenuController : UIScreenController
{
    private Button _btnManager;
    private Button _btnProManager;
    private Button _btnLoadGame;
    private Button _btnEditor;
    private Button _btnExit;
    private Button _btnLegal;
    private Button _btnBugReport;

    private VisualElement _legalOverlay;
    private VisualElement _legalBox;
    private Button _btnCerrarLegal;

    private VisualElement _exitOverlay;
    private VisualElement _exitBox;
    private Button _btnExitYes;
    private Button _btnExitNo;

    private VisualElement _proOverlay;
    private VisualElement _proBox;
    private Button _btnProCancel;
    private Button _btnProConfirm;

    private VisualElement _errorOverlay;
    private VisualElement _errorBox;
    private Label _errorText;
    private Button _btnErrorCerrar;

    private VisualElement _configIcon;

    public override GameScreen ScreenId => GameScreen.MainMenu;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic("backgroundMenu");
        if (CursorManager.Instance != null)
            CursorManager.Instance.SetDefaultCursor();
    }

    protected override void CacheReferences()
    {
        _btnManager = _root.Q<Button>("BtnManager");
        _btnProManager = _root.Q<Button>("BtnProManager");
        _btnLoadGame = _root.Q<Button>("BtnLoadGame");
        _btnEditor = _root.Q<Button>("BtnEditor");
        _btnExit = _root.Q<Button>("BtnExit");
        _btnLegal = _root.Q<Button>("BtnLegal");
        _btnBugReport = _root.Q<Button>("BtnBugReport");
        _configIcon = _root.Q<VisualElement>("ConfigIcon");

        _legalOverlay = _root.Q<VisualElement>("ModalOverlay");
        _legalBox = _root.Q<VisualElement>("ModalBox");
        _btnCerrarLegal = _root.Q<Button>("BtnCerrar");

        _exitOverlay = _root.Q<VisualElement>("ExitModalOverlay");
        _exitBox = _root.Q<VisualElement>("ExitModalBox");
        _btnExitYes = _root.Q<Button>("BtnExitYes");
        _btnExitNo = _root.Q<Button>("BtnExitNo");

        _proOverlay = _root.Q<VisualElement>("ProModalOverlay");
        _proBox = _root.Q<VisualElement>("ProModalBox");
        _btnProCancel = _root.Q<Button>("BtnProCancel");
        _btnProConfirm = _root.Q<Button>("BtnProConfirm");

        _errorOverlay = _root.Q<VisualElement>("ErrorModalOverlay");
        _errorBox = _root.Q<VisualElement>("ErrorModalBox");
        _errorText = _root.Q<Label>("ErrorModalText");
        _btnErrorCerrar = _root.Q<Button>("ErrorModalCerrar");

        foreach (var b in new[] { _btnManager, _btnProManager, _btnLoadGame, _btnEditor, _btnExit,
                                  _btnLegal, _btnBugReport, _btnCerrarLegal, _btnExitYes, _btnExitNo,
                                  _btnProCancel, _btnProConfirm, _btnErrorCerrar })
            RegisterHandCursor(b);

        RegisterHandCursor(_configIcon);
    }

    protected override void RegisterCallbacks()
    {
        _btnManager?.RegisterCallback<ClickEvent>(_ => { PlayClick(); StartNewGame(GameMode.Manager); });
        _btnProManager?.RegisterCallback<ClickEvent>(_ => { PlayClick(); OpenModal(_proOverlay, _proBox); });
        _btnLoadGame?.RegisterCallback<ClickEvent>(_ => { PlayClick(); ScreenManager.Instance.GoTo(GameScreen.LoadGame); });
        _btnEditor?.RegisterCallback<ClickEvent>(_ => { PlayClick(); ShowNotice("ui.main_menu.editor_pendiente"); });
        _btnExit?.RegisterCallback<ClickEvent>(_ => { PlayClick(); OpenModal(_exitOverlay, _exitBox); });
        _btnLegal?.RegisterCallback<ClickEvent>(_ => { PlayClick(); OpenModal(_legalOverlay, _legalBox); });
        _btnBugReport?.RegisterCallback<ClickEvent>(_ => { PlayClick(); ShowNotice("ui.main_menu.bug_report_pendiente"); });

        _btnCerrarLegal?.RegisterCallback<ClickEvent>(_ => { PlayClick(); CloseModal(_legalOverlay, _legalBox); });
        _btnExitNo?.RegisterCallback<ClickEvent>(_ => { PlayClick(); CloseModal(_exitOverlay, _exitBox); });
        _btnExitYes?.RegisterCallback<ClickEvent>(_ => { PlayClick(); QuitGame(); });

        _btnProCancel?.RegisterCallback<ClickEvent>(_ => { PlayClick(); CloseModal(_proOverlay, _proBox); });
        _btnProConfirm?.RegisterCallback<ClickEvent>(_ => { PlayClick(); CloseModal(_proOverlay, _proBox); StartNewGame(GameMode.ProManager); });

        _btnErrorCerrar?.RegisterCallback<ClickEvent>(_ => { PlayClick(); CloseModal(_errorOverlay, _errorBox); });
        _configIcon?.RegisterCallback<ClickEvent>(_ => { PlayClick(); OpenConfigModal(); });

        // Aviso legal y modales ocultos al entrar (vuelven a mostrarse a demanda).
        CloseModal(_legalOverlay, _legalBox);
        CloseModal(_exitOverlay, _exitBox);
        CloseModal(_proOverlay, _proBox);
        CloseModal(_errorOverlay, _errorBox);
        CloseConfigModal();
    }

    // ── Nueva partida ──────────────────────────────────────────────────────

    private void StartNewGame(GameMode mode)
    {
        // Sin pantalla de selección de equipo el flujo no puede continuar (F5).
        if (!ScreenManager.Instance.IsRegistered(GameScreen.SelectTeam))
        {
            ShowNotice("ui.main_menu.seleccion_pendiente");
            return;
        }

        if (!GameSaveManager.HasFreeSlot())
        {
            ShowError("ui.main_menu.error_sin_huecos", GameSaveManager.MAX_SLOTS);
            return;
        }

        int slot = GameSaveManager.FindNextAvailableSlot();
        GameSaveManager.CleanupOrphanDb(slot);

        var db = DatabaseManager.Instance;
        if (db == null || !db.InitSaveSlot(slot))
        {
            ShowError("ui.main_menu.error_crear_partida",
                db != null && !string.IsNullOrEmpty(db.LastInitError) ? db.LastInitError : "desconocido");
            return;
        }

        ScreenManager.Instance.GoTo(GameScreen.SelectTeam, mode);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ── Modales ────────────────────────────────────────────────────────────

    private void OpenModal(VisualElement overlay, VisualElement box)
    {
        if (overlay == null) return;
        // style.display explícito: un display inline en el UXML ganaría a la clase.
        overlay.style.display = DisplayStyle.Flex;
        overlay.AddToClassList("modal-overlay--visible");
        if (box != null)
        {
            box.style.display = DisplayStyle.Flex;
            box.AddToClassList("modal-box--visible");
        }
    }

    private void CloseModal(VisualElement overlay, VisualElement box)
    {
        if (overlay == null) return;
        overlay.RemoveFromClassList("modal-overlay--visible");
        overlay.style.display = DisplayStyle.None;
        if (box != null)
        {
            box.RemoveFromClassList("modal-box--visible");
            box.style.display = DisplayStyle.None;
        }
    }

    private void ShowError(string key, params object[] args)
    {
        if (_errorText != null)
            _errorText.text = LocalizationService.T(key, args);
        OpenModal(_errorOverlay, _errorBox);
    }

    private void ShowNotice(string key)
    {
        if (_errorText != null)
            _errorText.text = LocalizationService.T(key);
        OpenModal(_errorOverlay, _errorBox);
    }
}
