using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// F7 — Sidebar compartida (réplica de Tactical5): se inyecta en la raíz de la
/// pantalla y navega con <see cref="ScreenManager"/>. Los destinos que todavía no
/// existen avisan mediante <see cref="ScreenUnavailable"/>.
/// </summary>
public static class SidebarController
{
    struct ScreenNavMapping
    {
        public GameScreen Screen;
        public string NavName;
        public string SubmenuName;
        public string SubmenuItemName;
    }

    public static System.Action<GameScreen> ScreenUnavailable;

    private static readonly ScreenNavMapping[] Mappings =
    {
        new() { Screen = GameScreen.Dashboard, NavName = "NavDashboard" },

        new() { Screen = GameScreen.Roster, NavName = "NavRoster", SubmenuName = "RosterSubmenu", SubmenuItemName = "SubmenuJugadores" },
        new() { Screen = GameScreen.Lineup, NavName = "NavRoster", SubmenuName = "RosterSubmenu", SubmenuItemName = "SubmenuAlineacion" },
        new() { Screen = GameScreen.Tactics, NavName = "NavRoster", SubmenuName = "RosterSubmenu", SubmenuItemName = "SubmenuTactica" },
        new() { Screen = GameScreen.Training, NavName = "NavRoster", SubmenuName = "RosterSubmenu", SubmenuItemName = "SubmenuEntrenamiento" },
        new() { Screen = GameScreen.Staff, NavName = "NavRoster", SubmenuName = "RosterSubmenu", SubmenuItemName = "SubmenuStaff" },
        new() { Screen = GameScreen.Injured, NavName = "NavRoster", SubmenuName = "RosterSubmenu", SubmenuItemName = "SubmenuLesionados" },
        new() { Screen = GameScreen.Dorsales, NavName = "NavRoster", SubmenuName = "RosterSubmenu", SubmenuItemName = "SubmenuDorsales" },

        new() { Screen = GameScreen.Calendar, NavName = "NavCalendar" },

        new() { Screen = GameScreen.Results, NavName = "NavCompeticiones", SubmenuName = "CompetitionsSubmenu", SubmenuItemName = "SubmenuResultados" },
        new() { Screen = GameScreen.Standings, NavName = "NavCompeticiones", SubmenuName = "CompetitionsSubmenu", SubmenuItemName = "SubmenuClasificacion" },
        new() { Screen = GameScreen.CompetitionInfo, NavName = "NavCompeticiones", SubmenuName = "CompetitionsSubmenu", SubmenuItemName = "SubmenuInfoLiga" },
        new() { Screen = GameScreen.Filial, NavName = "NavCompeticiones", SubmenuName = "CompetitionsSubmenu", SubmenuItemName = "SubmenuFilial" },
        new() { Screen = GameScreen.Rivalidades, NavName = "NavCompeticiones", SubmenuName = "CompetitionsSubmenu", SubmenuItemName = "SubmenuRivalidades" },

        new() { Screen = GameScreen.Palmares, NavName = "NavPalmares", SubmenuName = "PalmaresSubmenu", SubmenuItemName = "SubmenuPalmares" },
        new() { Screen = GameScreen.Records, NavName = "NavPalmares", SubmenuName = "PalmaresSubmenu", SubmenuItemName = "SubmenuRecords" },
        new() { Screen = GameScreen.Premios, NavName = "NavPalmares", SubmenuName = "PalmaresSubmenu", SubmenuItemName = "SubmenuPremios" },

        new() { Screen = GameScreen.Knockouts, NavName = "NavKnockouts" },
        new() { Screen = GameScreen.Stats, NavName = "NavStats" },

        new() { Screen = GameScreen.Market, NavName = "NavMarket", SubmenuName = "MarketSubmenu", SubmenuItemName = "SubmenuOfertas" },
        new() { Screen = GameScreen.Cartera, NavName = "NavMarket", SubmenuName = "MarketSubmenu", SubmenuItemName = "SubmenuCartera" },
        new() { Screen = GameScreen.Historial, NavName = "NavMarket", SubmenuName = "MarketSubmenu", SubmenuItemName = "SubmenuHistorial" },
        new() { Screen = GameScreen.Buscador, NavName = "NavMarket", SubmenuName = "MarketSubmenu", SubmenuItemName = "SubmenuBuscador" },

        new() { Screen = GameScreen.Finanzas, NavName = "NavFinances", SubmenuName = "FinanceSubmenu", SubmenuItemName = "SubmenuDecisiones" },
        new() { Screen = GameScreen.Loans, NavName = "NavFinances", SubmenuName = "FinanceSubmenu", SubmenuItemName = "SubmenuPrestamos" },
        new() { Screen = GameScreen.Sponsors, NavName = "NavFinances", SubmenuName = "FinanceSubmenu", SubmenuItemName = "SubmenuSponsors" },
        new() { Screen = GameScreen.TV, NavName = "NavFinances", SubmenuName = "FinanceSubmenu", SubmenuItemName = "SubmenuTV" },

        new() { Screen = GameScreen.City, NavName = "NavCity", SubmenuName = "CitySubmenu", SubmenuItemName = "SubmenuVerCiudad" },
        new() { Screen = GameScreen.Stadium, NavName = "NavCity", SubmenuName = "CitySubmenu", SubmenuItemName = "SubmenuEstadio" },

        new() { Screen = GameScreen.Manager, NavName = "NavManager", SubmenuName = "ManagerSubmenu", SubmenuItemName = "SubmenuManager" },
        new() { Screen = GameScreen.Logros, NavName = "NavManager", SubmenuName = "ManagerSubmenu", SubmenuItemName = "SubmenuLogros" },

        new() { Screen = GameScreen.Messages, NavName = "NavActualidad", SubmenuName = "ActualidadSubmenu", SubmenuItemName = "SubmenuNoticias" },
        new() { Screen = GameScreen.Hilos, NavName = "NavActualidad", SubmenuName = "ActualidadSubmenu", SubmenuItemName = "SubmenuHilos" },
    };

    // Pantalla que abre cada menú de primer nivel al pulsarlo.
    private static readonly Dictionary<string, GameScreen> NavDefault = new()
    {
        { "NavRoster", GameScreen.Roster },
        { "NavCompeticiones", GameScreen.Standings },
        { "NavPalmares", GameScreen.Palmares },
        { "NavMarket", GameScreen.Market },
        { "NavFinances", GameScreen.Finanzas },
        { "NavCity", GameScreen.City },
        { "NavManager", GameScreen.Manager },
        { "NavActualidad", GameScreen.Messages },
    };

    private static readonly Dictionary<string, string> NavSubmenu = new()
    {
        { "NavRoster", "RosterSubmenu" },
        { "NavCompeticiones", "CompetitionsSubmenu" },
        { "NavPalmares", "PalmaresSubmenu" },
        { "NavMarket", "MarketSubmenu" },
        { "NavFinances", "FinanceSubmenu" },
        { "NavCity", "CitySubmenu" },
        { "NavManager", "ManagerSubmenu" },
        { "NavActualidad", "ActualidadSubmenu" },
    };

    private static VisualTreeAsset _template;

    private static VisualTreeAsset GetTemplate()
    {
        if (_template == null)
            _template = Resources.Load<VisualTreeAsset>("UI/Core/Sidebar");
        return _template;
    }

    /// <summary>Inserta la sidebar en la raíz de la pantalla (idempotente).</summary>
    public static void Attach(VisualElement root, GameScreen activeScreen)
    {
        var template = GetTemplate();
        if (template == null)
        {
            Debug.LogWarning("[Sidebar] No se encontró Resources/UI/Core/Sidebar.uxml");
            return;
        }

        var container = root.childCount > 0 ? root[0] : root;
        if (container == null) container = root;

        var existing = container.Q<VisualElement>("Sidebar");
        if (existing != null)
        {
            UpdateActiveState(existing, activeScreen);
            return;
        }

        var sidebar = template.CloneTree();
        container.Insert(0, sidebar);

        ApplyActiveState(sidebar, activeScreen);
        LoadIcons(sidebar);
        RegisterCallbacks(sidebar, activeScreen);
    }

    // ── CALLBACKS ──────────────────────────────────────────────────────────

    private static void RegisterCallbacks(VisualElement sidebar, GameScreen activeScreen)
    {
        foreach (var nav in sidebar.Query<Button>(null, "nav-item").ToList())
            RegisterNavButton(sidebar, nav);

        foreach (var item in sidebar.Query<Button>(null, "nav-submenu-item").ToList())
        {
            string name = item.name;
            item.RegisterCallback<ClickEvent>(_ =>
            {
                PlayClick();
                var target = FindScreenBySubmenuItem(name);
                if (target.HasValue) Navigate(target.Value);
            });
            RegisterCursor(item);
        }
    }

    private static void RegisterNavButton(VisualElement sidebar, Button nav)
    {
        string name = nav.name;

        nav.RegisterCallback<ClickEvent>(_ =>
        {
            PlayClick();

            if (NavSubmenu.TryGetValue(name, out string submenuName))
            {
                var submenu = sidebar.Q<VisualElement>(submenuName);
                if (submenu != null)
                {
                    bool visible = submenu.ClassListContains("nav-submenu--visible");
                    foreach (var other in sidebar.Query<VisualElement>(null, "nav-submenu").ToList())
                        other.RemoveFromClassList("nav-submenu--visible");
                    if (!visible) submenu.AddToClassList("nav-submenu--visible");
                }
            }

            var target = FindScreenByNav(name);
            if (target.HasValue) Navigate(target.Value);
        });

        RegisterCursor(nav);
    }

    private static void Navigate(GameScreen screen)
    {
        var manager = ScreenManager.Instance;
        if (manager != null && manager.IsRegistered(screen))
        {
            manager.GoTo(screen);
            return;
        }

        ScreenUnavailable?.Invoke(screen);
    }

    // ── ESTADO ACTIVO ──────────────────────────────────────────────────────

    private static void ApplyActiveState(VisualElement sidebar, GameScreen activeScreen)
    {
        var mapping = FindMapping(activeScreen);
        if (mapping == null) return;

        var activeNav = sidebar.Q<Button>(mapping.Value.NavName);
        activeNav?.AddToClassList("nav-item--active");

        if (!string.IsNullOrEmpty(mapping.Value.SubmenuName))
            sidebar.Q<VisualElement>(mapping.Value.SubmenuName)?.AddToClassList("nav-submenu--visible");

        if (!string.IsNullOrEmpty(mapping.Value.SubmenuItemName))
            sidebar.Q<Button>(mapping.Value.SubmenuItemName)?.AddToClassList("nav-submenu-item--active");
    }

    private static void UpdateActiveState(VisualElement sidebar, GameScreen activeScreen)
    {
        foreach (var nav in sidebar.Query<Button>(null, "nav-item").ToList())
            nav.RemoveFromClassList("nav-item--active");
        foreach (var sub in sidebar.Query<VisualElement>(null, "nav-submenu").ToList())
            sub.RemoveFromClassList("nav-submenu--visible");
        foreach (var item in sidebar.Query<Button>(null, "nav-submenu-item").ToList())
            item.RemoveFromClassList("nav-submenu-item--active");

        ApplyActiveState(sidebar, activeScreen);
    }

    private static ScreenNavMapping? FindMapping(GameScreen screen)
    {
        for (int i = 0; i < Mappings.Length; i++)
            if (Mappings[i].Screen == screen) return Mappings[i];
        return null;
    }

    private static GameScreen? FindScreenByNav(string navName)
    {
        if (navName == "NavDashboard") return GameScreen.Dashboard;
        if (navName == "NavCalendar") return GameScreen.Calendar;
        if (navName == "NavKnockouts") return GameScreen.Knockouts;
        if (navName == "NavStats") return GameScreen.Stats;
        return NavDefault.TryGetValue(navName, out var screen) ? screen : null;
    }

    private static GameScreen? FindScreenBySubmenuItem(string itemName)
    {
        for (int i = 0; i < Mappings.Length; i++)
            if (Mappings[i].SubmenuItemName == itemName) return Mappings[i].Screen;
        return null;
    }

    // ── ICONOS Y CURSOR ────────────────────────────────────────────────────

    private static void LoadIcons(VisualElement sidebar)
    {
        var iconMap = new Dictionary<string, string[]>
        {
            { "NavDashboardIcon", new[] { "inicio" } },
            { "NavRosterIcon", new[] { "plantilla" } },
            { "NavCalendarIcon", new[] { "calendario" } },
            { "NavCompeticionesIcon", new[] { "competiciones", "trofeo" } },
            { "NavPalmaresIcon", new[] { "palmares" } },
            { "NavKnockoutsIcon", new[] { "eliminatorias", "playoff" } },
            { "NavStatsIcon", new[] { "estadisticas" } },
            { "NavMarketIcon", new[] { "mercado" } },
            { "NavFinancesIcon", new[] { "finanzas" } },
            { "NavCityIcon", new[] { "ciudad", "arena" } },
            { "NavManagerIcon", new[] { "manager", "empleado" } },
            { "NavActualidadIcon", new[] { "mensajes" } },
        };

        foreach (var pair in iconMap)
        {
            var icon = sidebar.Q<VisualElement>(pair.Key);
            if (icon == null) continue;

            foreach (string name in pair.Value)
            {
                var texture = Resources.Load<Texture2D>($"Icons/{name}");
                if (texture == null) continue;
                icon.style.backgroundImage = new StyleBackground(texture);
                break;
            }
        }
    }

    private static void RegisterCursor(VisualElement element)
    {
        if (element != null && CursorManager.Instance != null)
            CursorManager.Instance.RegisterHandCursor(element);
    }

    private static void PlayClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("click");
    }
}
