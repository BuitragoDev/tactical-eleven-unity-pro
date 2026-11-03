using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Navegación del juego: una sola escena y un <see cref="UIDocument"/> por pantalla.
/// Las pantallas se registran en una lista serializada (pantalla → documento) y
/// <see cref="GoTo"/> activa una y desactiva el resto.
/// </summary>
public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    /// <summary>
    /// Bloquea las salidas "duras" (al menú) mientras corre una operación crítica
    /// (simulación de día, rollover de temporada…).
    /// </summary>
    public static bool NavigationLocked { get; set; }

    /// <summary>Jugador seleccionado en una lista (contexto entre pantallas).</summary>
    public static int SelectedPlayerId { get; set; }

    [Serializable]
    public class ScreenEntry
    {
        public GameScreen screen;
        public UIDocument document;
    }

    [SerializeField] private List<ScreenEntry> screens = new();
    [SerializeField] private UIDocument initialDocument;

    private readonly Dictionary<GameScreen, UIDocument> _map = new();

    public GameMode CurrentMode { get; private set; } = GameMode.None;
    public GameScreen CurrentScreen { get; private set; } = GameScreen.Loading;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _map.Clear();
        foreach (var entry in screens)
        {
            if (entry == null || entry.document == null) continue;
            _map[entry.screen] = entry.document;
        }

        if (initialDocument != null)
            ShowOnly(initialDocument);
    }

    /// <summary>
    /// Navega a una pantalla. Si el documento no está registrado, mantiene la
    /// pantalla actual y avisa (nunca deja la escena en negro).
    /// </summary>
    public void GoTo(GameScreen screen, GameMode mode = GameMode.None)
    {
        if (mode != GameMode.None)
            CurrentMode = mode;

        if (!_map.TryGetValue(screen, out var target) || target == null)
        {
            Debug.LogWarning($"[ScreenManager] No hay UIDocument registrado para '{screen}'.");
            return;
        }

        CurrentScreen = screen;
        ShowOnly(target);
    }

    /// <summary>Registra una pantalla en tiempo de ejecución (útil para pruebas).</summary>
    public void Register(GameScreen screen, UIDocument document)
    {
        if (document == null) return;
        _map[screen] = document;
    }

    void ShowOnly(UIDocument target)
    {
        if (target == null) return;

        foreach (var doc in _map.Values)
        {
            if (doc == null || doc == target) continue;
            doc.gameObject.SetActive(false);
        }

        target.gameObject.SetActive(true);
    }
}
