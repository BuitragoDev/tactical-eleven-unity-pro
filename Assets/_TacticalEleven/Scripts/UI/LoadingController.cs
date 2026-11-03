using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>Pantalla de carga: consejo aleatorio y salto al menú con clic, tecla o espera.</summary>
public class LoadingController : UIScreenController
{
    private static readonly string[] Tips =
    {
        "El fútbol es un deporte de once contra once y al final gana Alemania. — Gary Lineker",
        "Juega con la cabeza y te seguirán los pies. — Johan Cruyff",
        "No es la fuerza, es la calidad la que gana los partidos. — Johan Cruyff",
        "El talento gana partidos, pero el trabajo en equipo y la inteligencia ganan campeonatos. — Michael Jordan",
        "El éxito no es un accidente. Es trabajo duro, perseverancia, aprendizaje, sacrificio y amor por lo que haces. — Pelé",
        "Prefiero perder un partido que perder la dignidad. — Pep Guardiola",
        "La presión es un privilegio. — Pep Guardiola",
        "En el fútbol, el que no arriesga no gana. — Vicente del Bosque",
        "El fútbol no es una cuestión de vida o muerte, es mucho más importante que eso. — Bill Shankly",
        "Ningún jugador es tan bueno como todos juntos. — Alfredo Di Stéfano"
    };

    private Label _tipLabel;
    private bool _transitioning;

    public override GameScreen ScreenId => GameScreen.Loading;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_root == null) return;

        // Al volver a esta pantalla el flag debe estar limpio.
        _transitioning = false;
        _root.style.flexGrow = 1f;

        _tipLabel = _root.Q<Label>("LoadingTip");
        if (_tipLabel != null)
            _tipLabel.text = Tips[Random.Range(0, Tips.Length)];

        _root.RegisterCallback<ClickEvent>(_ => OnInput());
        _root.RegisterCallback<KeyDownEvent>(_ => OnInput());
        _root.focusable = true;
        _root.Focus();

        StartCoroutine(LoadingTimer());
    }

    IEnumerator LoadingTimer()
    {
        yield return new WaitForSeconds(10f);
        OnInput();
    }

    void OnInput()
    {
        if (_transitioning) return;
        _transitioning = true;
        PlayClick();
        ScreenManager.Instance.GoTo(GameScreen.MainMenu);
    }
}
