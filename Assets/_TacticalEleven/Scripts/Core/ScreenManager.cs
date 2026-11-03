using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Navegación del juego: una sola escena y un <see cref="UIDocument"/> por pantalla.
/// Cada pantalla se asigna **arrastrando su GameObject al campo del Inspector**.
/// <see cref="GoTo"/> activa el documento destino y desactiva el resto.
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

    // ── PANTALLAS ──────────────────────────────────────────────────────────
    [Header("Arranque")]
    [SerializeField] private UIDocument loadingDocument;

    [Header("Menús")]
    [SerializeField] private UIDocument mainMenuDocument;
    [SerializeField] private UIDocument loadGameDocument;
    [SerializeField] private UIDocument editorDocument;
    [SerializeField] private UIDocument selectTeamDocument;

    [Header("Temporada")]
    [SerializeField] private UIDocument preseasonDocument;
    [SerializeField] private UIDocument dashboardDocument;

    [Header("Plantilla")]
    [SerializeField] private UIDocument rosterDocument;
    [SerializeField] private UIDocument lineupDocument;
    [SerializeField] private UIDocument tacticsDocument;
    [SerializeField] private UIDocument trainingDocument;
    [SerializeField] private UIDocument staffDocument;
    [SerializeField] private UIDocument injuredDocument;
    [SerializeField] private UIDocument dorsalesDocument;

    [Header("Competiciones")]
    [SerializeField] private UIDocument calendarDocument;
    [SerializeField] private UIDocument resultsDocument;
    [SerializeField] private UIDocument standingsDocument;
    [SerializeField] private UIDocument competitionInfoDocument;
    [SerializeField] private UIDocument filialDocument;
    [SerializeField] private UIDocument rivalidadesDocument;
    [SerializeField] private UIDocument knockoutsDocument;
    [SerializeField] private UIDocument europeDocument;
    [SerializeField] private UIDocument copaDocument;

    [Header("Palmarés y estadísticas")]
    [SerializeField] private UIDocument logrosDocument;
    [SerializeField] private UIDocument palmaresDocument;
    [SerializeField] private UIDocument recordsDocument;
    [SerializeField] private UIDocument premiosDocument;
    [SerializeField] private UIDocument statsDocument;

    [Header("Mercado")]
    [SerializeField] private UIDocument marketDocument;
    [SerializeField] private UIDocument carteraDocument;
    [SerializeField] private UIDocument buscadorDocument;
    [SerializeField] private UIDocument historialDocument;

    [Header("Finanzas")]
    [SerializeField] private UIDocument finanzasDocument;
    [SerializeField] private UIDocument loansDocument;
    [SerializeField] private UIDocument sponsorsDocument;
    [SerializeField] private UIDocument tvDocument;
    [SerializeField] private UIDocument stadiumDocument;
    [SerializeField] private UIDocument cityDocument;

    [Header("Manager y actualidad")]
    [SerializeField] private UIDocument managerDocument;
    [SerializeField] private UIDocument messagesDocument;
    [SerializeField] private UIDocument hilosDocument;

    [Header("Partido")]
    [SerializeField] private UIDocument preGameDocument;
    [SerializeField] private UIDocument matchDayDocument;
    [SerializeField] private UIDocument gameResultsDocument;

    [Header("Fin de temporada")]
    [SerializeField] private UIDocument seasonSummaryDocument;
    [SerializeField] private UIDocument playerAwardsDocument;
    [SerializeField] private UIDocument endSeasonDocument;
    [SerializeField] private UIDocument newSeasonDocument;

    [Header("Jugador")]
    [SerializeField] private UIDocument playerProfileDocument;
    [SerializeField] private UIDocument trajectoryDocument;
    [SerializeField] private UIDocument bestXIDocument;

    // ── ESTADO ─────────────────────────────────────────────────────────────

    public GameMode CurrentMode { get; private set; } = GameMode.None;
    public GameScreen CurrentScreen { get; private set; } = GameScreen.Loading;

    private readonly List<UIDocument> _all = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        CacheDocuments();
        ShowOnly(loadingDocument);
    }

    /// <summary>Reúne todos los documentos asignados en el Inspector.</summary>
    private void CacheDocuments()
    {
        _all.Clear();
        AddIfPresent(loadingDocument);
        AddIfPresent(mainMenuDocument);
        AddIfPresent(loadGameDocument);
        AddIfPresent(editorDocument);
        AddIfPresent(selectTeamDocument);
        AddIfPresent(preseasonDocument);
        AddIfPresent(dashboardDocument);
        AddIfPresent(rosterDocument);
        AddIfPresent(lineupDocument);
        AddIfPresent(tacticsDocument);
        AddIfPresent(trainingDocument);
        AddIfPresent(staffDocument);
        AddIfPresent(injuredDocument);
        AddIfPresent(dorsalesDocument);
        AddIfPresent(calendarDocument);
        AddIfPresent(resultsDocument);
        AddIfPresent(standingsDocument);
        AddIfPresent(competitionInfoDocument);
        AddIfPresent(filialDocument);
        AddIfPresent(rivalidadesDocument);
        AddIfPresent(knockoutsDocument);
        AddIfPresent(europeDocument);
        AddIfPresent(copaDocument);
        AddIfPresent(logrosDocument);
        AddIfPresent(palmaresDocument);
        AddIfPresent(recordsDocument);
        AddIfPresent(premiosDocument);
        AddIfPresent(statsDocument);
        AddIfPresent(marketDocument);
        AddIfPresent(carteraDocument);
        AddIfPresent(buscadorDocument);
        AddIfPresent(historialDocument);
        AddIfPresent(finanzasDocument);
        AddIfPresent(loansDocument);
        AddIfPresent(sponsorsDocument);
        AddIfPresent(tvDocument);
        AddIfPresent(stadiumDocument);
        AddIfPresent(cityDocument);
        AddIfPresent(managerDocument);
        AddIfPresent(messagesDocument);
        AddIfPresent(hilosDocument);
        AddIfPresent(preGameDocument);
        AddIfPresent(matchDayDocument);
        AddIfPresent(gameResultsDocument);
        AddIfPresent(seasonSummaryDocument);
        AddIfPresent(playerAwardsDocument);
        AddIfPresent(endSeasonDocument);
        AddIfPresent(newSeasonDocument);
        AddIfPresent(playerProfileDocument);
        AddIfPresent(trajectoryDocument);
        AddIfPresent(bestXIDocument);
    }

    private void AddIfPresent(UIDocument document)
    {
        if (document != null && !_all.Contains(document))
            _all.Add(document);
    }

    // ── NAVEGACIÓN ─────────────────────────────────────────────────────────

    /// <summary>
    /// Navega a una pantalla. Si su documento no está asignado, mantiene la
    /// pantalla actual y avisa (nunca deja la escena en negro).
    /// </summary>
    public void GoTo(GameScreen screen, GameMode mode = GameMode.None)
    {
        if (mode != GameMode.None)
            CurrentMode = mode;

        UIDocument target = Resolve(screen);

        if (target == null)
        {
            Debug.LogWarning($"[ScreenManager] No hay UIDocument asignado para '{screen}'.");
            return;
        }

        CurrentScreen = screen;
        ShowOnly(target);
    }

    /// <summary>¿La pantalla tiene un UIDocument asignado en el Inspector?</summary>
    public bool IsRegistered(GameScreen screen) => Resolve(screen) != null;

    private UIDocument Resolve(GameScreen screen) => screen switch
    {
        GameScreen.Loading => loadingDocument,
        GameScreen.MainMenu => mainMenuDocument,
        GameScreen.LoadGame => loadGameDocument,
        GameScreen.Editor => editorDocument,
        GameScreen.SelectTeam => selectTeamDocument,
        GameScreen.Preseason => preseasonDocument,
        GameScreen.Dashboard => dashboardDocument,
        GameScreen.Roster => rosterDocument,
        GameScreen.Lineup => lineupDocument,
        GameScreen.Tactics => tacticsDocument,
        GameScreen.Training => trainingDocument,
        GameScreen.Staff => staffDocument,
        GameScreen.Injured => injuredDocument,
        GameScreen.Dorsales => dorsalesDocument,
        GameScreen.Calendar => calendarDocument,
        GameScreen.Results => resultsDocument,
        GameScreen.Standings => standingsDocument,
        GameScreen.CompetitionInfo => competitionInfoDocument,
        GameScreen.Filial => filialDocument,
        GameScreen.Rivalidades => rivalidadesDocument,
        GameScreen.Knockouts => knockoutsDocument,
        GameScreen.Europe => europeDocument,
        GameScreen.Copa => copaDocument,
        GameScreen.Logros => logrosDocument,
        GameScreen.Palmares => palmaresDocument,
        GameScreen.Records => recordsDocument,
        GameScreen.Premios => premiosDocument,
        GameScreen.Stats => statsDocument,
        GameScreen.Market => marketDocument,
        GameScreen.Cartera => carteraDocument,
        GameScreen.Buscador => buscadorDocument,
        GameScreen.Historial => historialDocument,
        GameScreen.Finanzas => finanzasDocument,
        GameScreen.Loans => loansDocument,
        GameScreen.Sponsors => sponsorsDocument,
        GameScreen.TV => tvDocument,
        GameScreen.Stadium => stadiumDocument,
        GameScreen.City => cityDocument,
        GameScreen.Manager => managerDocument,
        GameScreen.Messages => messagesDocument,
        GameScreen.Hilos => hilosDocument,
        GameScreen.PreGame => preGameDocument,
        GameScreen.MatchDay => matchDayDocument,
        GameScreen.GameResults => gameResultsDocument,
        GameScreen.SeasonSummary => seasonSummaryDocument,
        GameScreen.PlayerAwards => playerAwardsDocument,
        GameScreen.EndSeason => endSeasonDocument,
        GameScreen.NewSeason => newSeasonDocument,
        GameScreen.PlayerProfile => playerProfileDocument,
        GameScreen.Trajectory => trajectoryDocument,
        GameScreen.BestXI => bestXIDocument,
        _ => null
    };

    private void ShowOnly(UIDocument target)
    {
        if (target == null) return;

        foreach (var doc in _all)
        {
            if (doc == null || doc == target) continue;
            doc.gameObject.SetActive(false);
        }

        target.gameObject.SetActive(true);
    }
}
