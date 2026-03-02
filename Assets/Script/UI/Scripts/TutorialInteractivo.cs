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
        "Presiona E para abrir puertas",
        "Presiona E para recoger la linterna",
        "Presiona F para encender la linterna",
    };

    private int pasoActual = 0;
    private bool tutorialActivo = true;

    private float cooldown = 0.7f;   // ⏱ Tiempo de espera reducido a 0.7 segundos
    private float tiempoCooldown = 0f;

    // Flags de acciones
    private bool presionoW, presionoA, presionoS, presionoD;
    private bool corrio, abrioPuerta, agarroLinterna, encendioLinterna;

    void Start()
    {
        if (panelTutorial != null) panelTutorial.SetActive(true);
        MostrarPaso();
    }

    void Update()
    {
        if (!tutorialActivo) return;

        if (tiempoCooldown > 0f)
        {
            tiempoCooldown -= Time.deltaTime;
            return;
        }

        switch (pasoActual)
        {
            case 0: // WASD → ahora solo necesita 2 teclas distintas
                if (Input.GetKeyDown(KeyCode.W)) presionoW = true;
                if (Input.GetKeyDown(KeyCode.A)) presionoA = true;
                if (Input.GetKeyDown(KeyCode.S)) presionoS = true;
                if (Input.GetKeyDown(KeyCode.D)) presionoD = true;

                int teclasPresionadas = 0;
                if (presionoW) teclasPresionadas++;
                if (presionoA) teclasPresionadas++;
                if (presionoS) teclasPresionadas++;
                if (presionoD) teclasPresionadas++;

                if (teclasPresionadas >= 2)
                    SiguientePaso();
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

            case 2: // Puerta
                if (abrioPuerta) SiguientePaso();
                break;

            case 3: // Linterna
                if (agarroLinterna) SiguientePaso();
                break;

            case 4: // Encender linterna
                if (Input.GetKeyDown(KeyCode.F))
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
        tiempoCooldown = cooldown; // ⏱ aplica cooldown de 0.7s

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

    // 🔑 Métodos públicos para que otros scripts notifiquen
    public void NotificarPuerta()
    {
        abrioPuerta = true;
        if (pasoActual == 2) SiguientePaso();
    }

    public void NotificarLinterna()
    {
        agarroLinterna = true;
        if (pasoActual <= 3) pasoActual = 3; // saltar directo a linterna si aún no pasó
        MostrarPaso();
    }
}
