using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialInteractivo : MonoBehaviour
{
    //---------------UI---------------
    [Header("UI")]
    [SerializeField] private GameObject panelTutorial;
    [SerializeField] private TMP_Text textoTutorial;

    //---------------Imágenes especiales---------------
    [Header("Imágenes especiales")]
    [SerializeField] private Image imagenGuardar;
    [SerializeField] private Image imagenCargar;

    //---------------Pasos del tutorial---------------
    [Header("Pasos del tutorial")]
    [SerializeField]
    private string[] pasos = {
        "Presiona W A S D para moverte",
        "Presiona SHIFT mientras te mueves para correr",
        "Presiona E para interactuar",
        "Presiona F para encender la linterna",
        "Presiona ESC para activar el menu de pausa",
        "Usar \"Simbolo de guardado\" para guardar partida",
        "Usa \"simbolo de cargado de partida\" para cargar el punto de guardado"
    };

    //---------------Índices de pasos especiales---------------
    [Header("Índices de pasos especiales")]
    [SerializeField] private int pasoImagenGuardar = 5;
    [SerializeField] private int pasoImagenCargar = 6;

    //---------------Estado interno---------------
    private int pasoActual = 0;
    private bool tutorialActivo = true;

    // Cooldown entre pasos
    private float cooldown = 1.5f;
    private float tiempoCooldown = 0f;

    // Para el paso WASD
    private bool presionoW, presionoA, presionoS, presionoD;

    //---------------Inicio---------------
    void Start()
    {
        if (panelTutorial != null) panelTutorial.SetActive(true);
        if (imagenGuardar != null) imagenGuardar.gameObject.SetActive(false);
        if (imagenCargar != null) imagenCargar.gameObject.SetActive(false);
        MostrarPaso();
    }

    //---------------Update---------------
    void Update()
    {
        if (!tutorialActivo) return;

        // Cooldown entre pasos
        if (tiempoCooldown > 0f)
        {
            tiempoCooldown -= Time.deltaTime;
            return;
        }

        switch (pasoActual)
        {
            case 0: // WASD → debe presionar todas al menos una vez
                if (Input.GetKeyDown(KeyCode.W)) presionoW = true;
                if (Input.GetKeyDown(KeyCode.A)) presionoA = true;
                if (Input.GetKeyDown(KeyCode.S)) presionoS = true;
                if (Input.GetKeyDown(KeyCode.D)) presionoD = true;

                if (presionoW && presionoA && presionoS && presionoD)
                {
                    SiguientePaso();
                }
                break;

            case 1: // SHIFT
                if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                     Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) &&
                    Input.GetKey(KeyCode.LeftShift))
                {
                    SiguientePaso();
                }
                break;

            case 2: // E
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SiguientePaso();
                }
                break;

            case 3: // F
                if (Input.GetKeyDown(KeyCode.F))
                {
                    SiguientePaso();
                }
                break;

            case 4: // ESC
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    pasoActual = pasoImagenGuardar;
                    MostrarPaso();

                    // Después de 3 segundos → pasar al paso de cargar
                    EsperarYAccion(3f, () =>
                    {
                        pasoActual = pasoImagenCargar;
                        MostrarPaso();

                        // Después de otros 3 segundos → cerrar tutorial
                        EsperarYAccion(3f, () =>
                        {
                            CompletarTutorial();
                        });
                    });
                }
                break;
        }
    }

    //---------------Mostrar paso---------------
    private void MostrarPaso()
    {
        if (textoTutorial != null && pasoActual < pasos.Length)
        {
            textoTutorial.text = pasos[pasoActual];
        }

        if (imagenGuardar != null)
            imagenGuardar.gameObject.SetActive(pasoActual == pasoImagenGuardar);

        if (imagenCargar != null)
            imagenCargar.gameObject.SetActive(pasoActual == pasoImagenCargar);
    }

    //---------------Siguiente paso---------------
    private void SiguientePaso()
    {
        pasoActual++;
        tiempoCooldown = cooldown; // aplicar cooldown de 1.5s

        if (pasoActual >= pasos.Length)
        {
            CompletarTutorial();
        }
        else
        {
            MostrarPaso();
        }
    }

    //---------------Esperar y ejecutar acción---------------
    private void EsperarYAccion(float segundos, System.Action accion)
    {
        StartCoroutine(EsperarAccion(segundos, accion));
    }

    private System.Collections.IEnumerator EsperarAccion(float segundos, System.Action accion)
    {
        yield return new WaitForSeconds(segundos);
        accion?.Invoke();
    }

    //---------------Completar tutorial---------------
    private void CompletarTutorial()
    {
        tutorialActivo = false;
        if (panelTutorial != null) panelTutorial.SetActive(false);

        if (imagenGuardar != null) imagenGuardar.gameObject.SetActive(false);
        if (imagenCargar != null) imagenCargar.gameObject.SetActive(false);

        Debug.Log("✅ Tutorial completado y panel apagado.");
    }
}
