using UnityEngine;
using TMPro;

public class TutorialInteractivo : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panelTutorial;
    [SerializeField] private TMP_Text textoTutorial;

    [Header("Pasos del tutorial")]
    [SerializeField]
    private string[] pasos = {
        "Presiona W A S D para moverte",
        "Presiona SHIFT mientras te mueves para correr",
        "Presiona E para interactuar y recoger objetos",
        "Presiona F para encender la linterna (si la tienes)"
    };

    private int pasoActual = 0;
    private bool tutorialActivo = true;

    // Flags de acciones
    private bool presionoMovimiento, corrio, interactuo, encendioLinterna;
    private bool tieneLinterna;

    void Start()
    {
        if (panelTutorial != null) panelTutorial.SetActive(true);
        MostrarPaso();
    }

    void Update()
    {
        if (!tutorialActivo) return;

        switch (pasoActual)
        {
            case 0: // WASD
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                    Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                {
                    presionoMovimiento = true;
                    SiguientePaso();
                }
                break;

            case 1: // SHIFT
                if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                     Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) &&
                    Input.GetKey(KeyCode.LeftShift))
                {
                    corrio = true;
                    SiguientePaso();
                }
                break;

            case 2: // Interactuar
                if (interactuo) SiguientePaso();
                break;

            case 3: // Encender linterna
                if (tieneLinterna && Input.GetKeyDown(KeyCode.F))
                {
                    encendioLinterna = true;
                    SiguientePaso();
                }
                break;
        }
    }

    private void MostrarPaso()
    {
        if (textoTutorial != null && pasoActual < pasos.Length)
            textoTutorial.text = pasos[pasoActual];
    }

    public void SiguientePaso()
    {
        pasoActual++;
        if (pasoActual >= pasos.Length)
        {
            CompletarTutorial();
        }
        else
        {
            MostrarPaso();
        }
    }

    private void CompletarTutorial()
    {
        tutorialActivo = false;
        if (panelTutorial != null) panelTutorial.SetActive(false);
        Debug.Log("✅ Tutorial completado y panel apagado.");
    }

    // 🔑 Notificaciones externas
    public void NotificarInteraccion()
    {
        interactuo = true;
        if (pasoActual == 2) SiguientePaso();
    }

    public void NotificarLinternaRecogida()
    {
        tieneLinterna = true;
        // No avanza automáticamente, solo habilita condición para paso 3
    }

    public bool EstaActivo()
    {
        return tutorialActivo;
    }
}
