using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// S1 — Header común de las pantallas del sidebar (club, manager, presupuesto,
/// química, temporada y fecha). Se inyecta en runtime, igual que la sidebar, y
/// deja el header a todo el ancho con la sidebar debajo.
/// </summary>
public static class HeaderController
{
    private static VisualTreeAsset _template;
    private static Dictionary<string, Sprite> _logos;

    public static void Attach(VisualElement root, UIScreenController controller = null)
    {
        var template = GetTemplate();
        if (template == null || root == null) return;

        var container = root.childCount > 0 ? root[0] : root;
        if (container == null) container = root;

        var existing = container.Q<VisualElement>("TopHeader");

        // Ya está en la posición correcta: hijo directo o dentro de su
        // TemplateContainer (cuando el header se inyecta desde Header.uxml).
        bool alreadyAtTop = existing != null
            && (existing.parent == container
                || (existing.parent is TemplateContainer templateParent
                    && templateParent.parent == container));

        if (alreadyAtTop)
        {
            Populate(existing);
            return;
        }

        // Si el header existe pero está anidado (p. ej. dentro de main-area),
        // se saca para colocarlo a todo el ancho.
        bool injected = existing == null;

        VisualElement header;
        if (existing != null)
        {
            existing.parent.Remove(existing);
            header = existing;
        }
        else
        {
            header = template.CloneTree();
        }

        // Recoge los hijos actuales para reordenarlos.
        var children = new List<VisualElement>();
        while (container.childCount > 0)
        {
            children.Add(container[0]);
            container.RemoveAt(0);
        }

        // La sidebar se inyecta como TemplateContainer: su raíz real "Sidebar"
        // está dentro, así que se busca por nombre en el subárbol.
        var sidebar = children.FirstOrDefault(c =>
            c.name == "Sidebar" || c.Q<VisualElement>("Sidebar") != null);

        var main = children.FirstOrDefault(c => c != sidebar && !IsOverlay(c));
        var rest = children.Where(c => c != sidebar && c != main).ToList();

        container.Add(header);

        if (sidebar != null || main != null)
        {
            var body = new VisualElement();
            body.style.flexDirection = FlexDirection.Row;
            body.style.flexGrow = 1;
            body.style.minHeight = 0;

            if (sidebar != null) body.Add(sidebar);
            if (main != null) body.Add(main);

            container.Add(body);
        }

        // Los overlays/modales y avisos absolutos se quedan al final.
        foreach (var child in rest)
            container.Add(child);

        container.style.flexDirection = FlexDirection.Column;

        Populate(header);

        if (injected)
            WireHeaderButtons(header, controller);
    }

    private static void WireHeaderButtons(VisualElement header, UIScreenController controller)
    {
        var configIcon = header.Q<VisualElement>("ConfigIcon");
        if (configIcon != null && controller != null)
        {
            configIcon.RegisterCallback<ClickEvent>(_ =>
            {
                PlayClick();
                controller.ShowConfigModal();
            });
            CursorManager.Instance?.RegisterHandCursor(configIcon);
        }

        var mainMenu = header.Q<Button>("BtnMainMenu");
        if (mainMenu != null)
        {
            mainMenu.RegisterCallback<ClickEvent>(_ =>
            {
                PlayClick();
                ScreenManager.Instance?.GoTo(GameScreen.Dashboard);
            });
            CursorManager.Instance?.RegisterHandCursor(mainMenu);
        }
    }

    private static void PlayClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("click");
    }

    private static bool IsOverlay(VisualElement element)
        => element != null && element.ClassListContains("modal-overlay");

    private static VisualTreeAsset GetTemplate()
    {
        if (_template == null)
            _template = Resources.Load<VisualTreeAsset>("UI/Core/Header");
        return _template;
    }

    private static void Populate(VisualElement header)
    {
        var db = DatabaseManager.Instance;
        var manager = db?.GetActiveManager();
        if (manager == null) return;

        var team = db.GetTeamById(manager.TeamId);
        var season = db.GetActiveSeason(manager.Id);
        if (team == null) return;

        SetText(header, "HeaderTeamName", team.Name.ToUpperInvariant());
        SetText(header, "HeaderManagerName",
            LocalizationService.T("ui.common.manager", manager.Name));

        SetText(header, "HeaderBudget", $"{team.TransferBudget:N0} €");

        var chemistry = header.Q<Label>("HeaderChemistry");
        if (chemistry != null)
        {
            chemistry.text = $"{team.Chemistry}%";
            chemistry.RemoveFromClassList("header-stat-value--negative");
            chemistry.RemoveFromClassList("header-stat-value--gold");
            if (team.Chemistry < 40) chemistry.AddToClassList("header-stat-value--negative");
            else if (team.Chemistry < 70) chemistry.AddToClassList("header-stat-value--gold");
        }

        if (season != null)
        {
            SetText(header, "HeaderSeason",
                LocalizationService.T("ui.dashboard.temporada", season.YearStart, season.YearEnd));
            SetText(header, "HeaderDate", GameDayRules.FormatFullDateEs(season.CurrentDate));
        }

        var logo = header.Q<VisualElement>("HeaderTeamLogo");
        if (logo != null)
        {
            string key = string.IsNullOrEmpty(team.Logo) ? team.Id.ToString() : team.Logo;
            if (Logos().TryGetValue(key, out var sprite))
                logo.style.backgroundImage = new StyleBackground(sprite);
        }
    }

    private static void SetText(VisualElement header, string name, string text)
    {
        var label = header.Q<Label>(name);
        if (label != null) label.text = text;
    }

    private static Dictionary<string, Sprite> Logos()
    {
        if (_logos != null) return _logos;

        _logos = new Dictionary<string, Sprite>();
        foreach (var sprite in Resources.LoadAll<Sprite>("Teams/Logos/64x64"))
            _logos[sprite.name] = sprite;
        return _logos;
    }
}
