using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialInteractivo : MonoBehaviour
{
    //---------------UI---------------
    [Header("UI")]
    [SerializeField] private GameObject panelTutorial;
    [SerializeField] private TMP_Text textoTutorial;

    //---------------Pasos del tutorial---------------
    [Header("Pasos del tutorial")]
    [SerializeField]
    private string[] pasos = {
        "Presiona W A S D para moverte",
        "Presiona SHIFT mientras te mueves para correr",
        "Presiona E para interactuar",
        "Presiona F para encender la linterna",
    };

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
        }
    }

    //---------------Mostrar paso---------------
    private void MostrarPaso()
    {
        if (textoTutorial != null && pasoActual < pasos.Length)
        {
            textoTutorial.text = pasos[pasoActual];
        }
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
        Debug.Log("✅ Tutorial completado y panel apagado.");
    }
}
