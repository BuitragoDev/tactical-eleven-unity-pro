using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Menú principal. F0 deja la pantalla y la navegación base; los flujos de
/// crear/cargar partida, el modo ProManager, el editor y el aviso legal llegan en F4.
/// </summary>
public class MainMenuController : UIScreenController
{
    private Button _btnManager;
    private Button _btnProManager;
    private Button _btnLoadGame;
    private Button _btnEditor;
    private Button _btnExit;
    private Label _notice;

    public override GameScreen ScreenId => GameScreen.MainMenu;

    protected override void OnEnable()
    {
        base.OnEnable();

        // Música del menú (TryPlayMusic no la reinicia si ya suena) y cursor normal.
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

        foreach (var btn in new[] { _btnManager, _btnProManager, _btnLoadGame, _btnEditor, _btnExit })
            RegisterHandCursor(btn);
    }

    protected override void RegisterCallbacks()
    {
        if (_btnManager != null) _btnManager.clicked += () => Pending("Liga Manager");
        if (_btnProManager != null) _btnProManager.clicked += () => Pending("ProManager");
        if (_btnLoadGame != null) _btnLoadGame.clicked += () => Pending("Cargar partida");
        if (_btnEditor != null) _btnEditor.clicked += () => Pending("Editor");
        if (_btnExit != null) _btnExit.clicked += OnExit;
    }

    void OnExit()
    {
        PlayClick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Pending(string action)
    {
        PlayClick();
        Debug.Log($"[MainMenu] '{action}' se implementa en F4/F5.");
        ShowNotice($"«{action}» disponible en la siguiente fase");
    }

    private void ShowNotice(string message)
    {
        if (_root == null) return;
        if (_notice == null)
        {
            _notice = new Label();
            _notice.AddToClassList("menu-notice");
            _root.Add(_notice);
        }
        _notice.text = message;
        _notice.style.display = DisplayStyle.Flex;
    }
}
