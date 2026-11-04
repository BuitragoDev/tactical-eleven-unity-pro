using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// F0 — Humo de UI: los UXML de arranque se instancian, tienen los elementos que
/// esperan sus controllers y todas sus claves de localización existen. Se ejecuta
/// en EditMode (no necesita entrar en Play).
/// </summary>
public class UiSmokeTests
{
    private const string LoadingPath = "Assets/_TacticalEleven/UI/Screens/Loading/Loading.uxml";
    private const string MainMenuPath = "Assets/_TacticalEleven/UI/Screens/MainMenu/MainMenu.uxml";
    private const string LoadGamePath = "Assets/_TacticalEleven/UI/Screens/LoadGame/LoadGame.uxml";
    private const string SelectTeamPath = "Assets/_TacticalEleven/UI/Screens/SelectTeam/SelectTeam.uxml";
    private const string PreseasonPath = "Assets/_TacticalEleven/UI/Screens/Preseason/Preseason.uxml";
    private const string DashboardPath = "Assets/_TacticalEleven/UI/Screens/Dashboard/Dashboard.uxml";
    private const string SidebarPath = "Assets/_TacticalEleven/UI/Resources/UI/Core/Sidebar.uxml";
    private const string PreGamePath = "Assets/_TacticalEleven/UI/Screens/PreGame/PreGame.uxml";
    private const string MatchDayPath = "Assets/_TacticalEleven/UI/Screens/MatchDay/MatchDay.uxml";
    private const string GameResultsPath = "Assets/_TacticalEleven/UI/Screens/GameResults/GameResults.uxml";

    private static readonly Regex TokenRegex = new Regex(@"\{loc:([A-Za-z0-9_.-]+)\}");

    [SetUp]
    public void SetUp()
    {
        LocalizationService.SetLanguage(LocalizationService.DefaultLanguage);
        LocalizationService.Reload();
    }

    [Test]
    public void LoadingUxml_InstantiatesAndHasRootAndTip()
    {
        var tree = Load(LoadingPath);
        var root = tree.Instantiate();
        Assert.That(root.Q<VisualElement>("LoadingRoot"), Is.Not.Null);
        Assert.That(root.Q<Label>("LoadingTip"), Is.Not.Null);
    }

    [Test]
    public void MainMenuUxml_HasTheFiveMenuOptions()
    {
        var tree = Load(MainMenuPath);
        var root = tree.Instantiate();

        Assert.That(root.Q<Button>("BtnManager"), Is.Not.Null, "Liga Manager");
        Assert.That(root.Q<Button>("BtnProManager"), Is.Not.Null, "ProManager");
        Assert.That(root.Q<Button>("BtnLoadGame"), Is.Not.Null, "Cargar partida");
        Assert.That(root.Q<Button>("BtnEditor"), Is.Not.Null, "Editor");
        Assert.That(root.Q<Button>("BtnExit"), Is.Not.Null, "Salir");
    }

    [TestCase(LoadingPath)]
    [TestCase(MainMenuPath)]
    [TestCase(LoadGamePath)]
    [TestCase(SelectTeamPath)]
    [TestCase(PreseasonPath)]
    [TestCase(DashboardPath)]
    [TestCase(SidebarPath)]
    [TestCase(PreGamePath)]
    [TestCase(MatchDayPath)]
    [TestCase(GameResultsPath)]
    public void UxmlTokens_ExistInSpanishTable(string path)
    {
        var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        Assert.That(uxml, Is.Not.Null);

        string text = System.IO.File.ReadAllText(path);
        var missing = new List<string>();
        foreach (Match match in TokenRegex.Matches(text))
        {
            string key = match.Groups[1].Value;
            if (!LocalizationService.TryGet(key, out _)) missing.Add(key);
        }

        Assert.That(missing, Is.Empty, $"{path}: claves sin entrada → {string.Join(", ", missing)}");
    }

    [TestCase(LoadingPath)]
    [TestCase(MainMenuPath)]
    [TestCase(LoadGamePath)]
    [TestCase(SelectTeamPath)]
    [TestCase(PreseasonPath)]
    [TestCase(DashboardPath)]
    [TestCase(SidebarPath)]
    [TestCase(PreGamePath)]
    [TestCase(MatchDayPath)]
    [TestCase(GameResultsPath)]
    public void UxmlTexts_AreResolvedAfterApply(string path)
    {
        var tree = Load(path);
        var root = tree.Instantiate();
        LocalizationService.Apply(root);

        foreach (var label in root.Query<Label>().ToList())
            Assert.That(label.text.Contains("{loc:"), Is.False,
                $"Token sin resolver en {path}: {label.text}");
    }

    [Test]
    public void LoadGameUxml_HasSlotsAreaAndDeleteModal()
    {
        var root = Load(LoadGamePath).Instantiate();
        Assert.That(root.Q<VisualElement>("SlotsGrid"), Is.Not.Null);
        Assert.That(root.Q<VisualElement>("EmptyState"), Is.Not.Null);
        Assert.That(root.Q<VisualElement>("DeleteModalOverlay"), Is.Not.Null);
        Assert.That(root.Q<Button>("BtnDeleteYes"), Is.Not.Null);
        Assert.That(root.Q<Button>("BtnDeleteNo"), Is.Not.Null);
        Assert.That(root.Q<Button>("BtnBack"), Is.Not.Null);
    }

    [Test]
    public void Scene_HasDashboardDocumentWiredToScreenManager()
    {
        string scene = File.ReadAllText("Assets/_TacticalEleven/Scenes/MainMenu.unity");
        Assert.That(scene.Contains("m_Name: DashboardDocument"), Is.True,
            "Falta el GameObject DashboardDocument en la escena");
        Assert.That(Regex.IsMatch(scene, @"dashboardDocument: \{fileID: (?!0\})"), Is.True,
            "El campo dashboardDocument del ScreenManager no está enlazado");
    }

    [Test]
    public void ModalOverlays_DoNotUseInlineDisplayNone()
    {
        // Un `display:none` inline gana a `.modal-overlay--visible`, así que el modal
        // nunca se abriría (bug de los modales PRO y de plantilla). La clase ya oculta
        // el overlay por defecto.
        string root = Path.Combine(Application.dataPath, "_TacticalEleven", "UI", "Screens");
        var offenders = new System.Collections.Generic.List<string>();

        foreach (string file in Directory.GetFiles(root, "*.uxml", SearchOption.AllDirectories))
        {
            foreach (string line in File.ReadAllLines(file))
            {
                if (line.Contains("class=\"modal-overlay\"") && line.Contains("display: none"))
                    offenders.Add(Path.GetFileName(file) + ": " + line.Trim());
            }
        }

        Assert.That(offenders, Is.Empty,
            "Overlays con display inline:\n" + string.Join("\n", offenders));
    }

    private static VisualTreeAsset Load(string path)
    {
        var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        Assert.That(asset, Is.Not.Null, $"No se encontró {path}");
        return asset;
    }
}
