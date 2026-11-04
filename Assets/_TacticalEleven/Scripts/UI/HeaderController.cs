using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// S1 — Header común de las pantallas del sidebar (club, manager, presupuesto,
/// química, temporada y fecha). Se inyecta en runtime, igual que la sidebar.
/// </summary>
public static class HeaderController
{
    private static VisualTreeAsset _template;
    private static Dictionary<string, Sprite> _logos;

    public static void Attach(VisualElement root)
    {
        var template = GetTemplate();
        if (template == null || root == null) return;

        var container = root.childCount > 0 ? root[0] : root;
        if (container == null) container = root;

        var existing = container.Q<VisualElement>("TopHeader");
        if (existing != null)
        {
            Populate(existing);
            return;
        }

        // Reestructura: container (columna) → header + body-row (sidebar + main).
        var children = new List<VisualElement>();
        while (container.childCount > 0)
        {
            children.Add(container[0]);
            container.RemoveAt(0);
        }

        var header = template.CloneTree();
        container.Add(header);

        var body = new VisualElement();
        body.style.flexDirection = FlexDirection.Row;
        body.style.flexGrow = 1;
        body.style.minHeight = 0;

        foreach (var child in children)
            body.Add(child);

        container.Add(body);
        container.style.flexDirection = FlexDirection.Column;

        Populate(header);
    }

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
        SetText(header, "HeaderChemistry", $"{team.Chemistry}%");

        if (season != null)
        {
            SetText(header, "HeaderSeason",
                LocalizationService.T("ui.dashboard.temporada", season.YearStart, season.YearEnd));
            SetText(header, "HeaderDate", GameDayRules.FormatLongDate(season.CurrentDate));
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
