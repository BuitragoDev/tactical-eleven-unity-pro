using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Genera la escena única del juego (F0) por código, para no versionar YAML a mano.
/// Uso: menú <c>Tactical Eleven → Bootstrap → Main Menu Scene</c> o en batchmode:
/// <c>Unity -batchmode -quit -executeMethod SceneBootstrap.Create</c>.
/// </summary>
public static class SceneBootstrap
{
    private const string ScenePath = "Assets/_TacticalEleven/Scenes/MainMenu.unity";
    private const string PanelPath = "Assets/_TacticalEleven/UI/Resources/TacticalElevenPanelSettings.asset";
    private const string LoadingUxmlPath = "Assets/_TacticalEleven/UI/Screens/Loading/Loading.uxml";
    private const string MainMenuUxmlPath = "Assets/_TacticalEleven/UI/Screens/MainMenu/MainMenu.uxml";

    [MenuItem("Tactical Eleven/Bootstrap/Main Menu Scene")]
    public static void Create()
    {
        // La escena se crea PRIMERO: NewScene(Single) descarga assets y dejaría
        // inválidas las referencias cargadas antes.
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var panel = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelPath);
        var loadingUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(LoadingUxmlPath);
        var mainMenuUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(MainMenuUxmlPath);

        if (panel == null || loadingUxml == null || mainMenuUxml == null)
        {
            Debug.LogError("[Bootstrap] Faltan assets: panel, Loading.uxml o MainMenu.uxml.");
            return;
        }

        // Cámara de fondo (la UI es overlay; evita el aviso "No cameras rendering").
        var cameraGo = new GameObject("Main Camera");
        cameraGo.tag = "MainCamera";
        var camera = cameraGo.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color32(18, 21, 28, 255);
        camera.orthographic = true;
        camera.cullingMask = 0;

        // Singletons persistentes
        var managerGo = new GameObject("ScreenManager");
        var screenManager = managerGo.AddComponent<ScreenManager>();
        managerGo.AddComponent<DatabaseManager>();

        new GameObject("CursorManager").AddComponent<CursorManager>();

        // Pantallas
        var loadingDoc = CreateDocument("LoadingDocument", loadingUxml, panel, typeof(LoadingController));
        var mainMenuDoc = CreateDocument("MainMenuDocument", mainMenuUxml, panel, typeof(MainMenuController));

        // Registro de pantallas en ScreenManager
        var so = new SerializedObject(screenManager);
        var screens = so.FindProperty("screens");
        screens.arraySize = 2;
        SetEntry(screens.GetArrayElementAtIndex(0), GameScreen.Loading, loadingDoc);
        SetEntry(screens.GetArrayElementAtIndex(1), GameScreen.MainMenu, mainMenuDoc);
        so.FindProperty("initialDocument").objectReferenceValue = loadingDoc;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);

        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[Bootstrap] Escena creada en {ScenePath}");
    }

    private static UIDocument CreateDocument(string name, VisualTreeAsset uxml,
        PanelSettings panel, Type controller)
    {
        var go = new GameObject(name);
        var doc = go.AddComponent<UIDocument>();

        // Asignación por SerializedObject: es la que persiste de forma fiable en
        // edit mode headless (los setters públicos de UIDocument no siempre lo hacen).
        var so = new SerializedObject(doc);
        var panelProp = so.FindProperty("m_PanelSettings");
        var uxmlProp = so.FindProperty("sourceAsset");
        if (panelProp != null) panelProp.objectReferenceValue = panel;
        if (uxmlProp != null) uxmlProp.objectReferenceValue = uxml;
        so.ApplyModifiedPropertiesWithoutUndo();

        go.AddComponent<FullScreenUI>();
        go.AddComponent(controller);
        return doc;
    }

    private static void SetEntry(SerializedProperty element, GameScreen screen, UIDocument doc)
    {
        element.FindPropertyRelative("screen").enumValueIndex = (int)screen;
        element.FindPropertyRelative("document").objectReferenceValue = doc;
    }
}
