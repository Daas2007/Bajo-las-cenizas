using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialInteractivo : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panelTutorial;
    [SerializeField] private TMP_Text textoTutorial;
    [SerializeField] private TMP_Text textoTemporal;

    [Header("Control externo")]
    [SerializeField] private GameObject objetoControl; // 🔹 objeto que controla el panel

    [Header("Pasos del tutorial")]
    [SerializeField]
    private string[] pasos = {
        "Presiona dos teclas entre W A S D para moverte",
        "Mantén SHIFT mientras te mueves para correr",
        "Presiona E para interactuar (recoge la linterna si la ves)",
        "Presiona F para encender la linterna"
    };

    private int pasoActual = 0;
    private bool tutorialActivo = true;

    // Flags
    private bool presionoMovimiento = false;
    private bool corrio = false;
    private bool encendioLinterna = false;
    private bool tieneLinterna = false;

    [Header("Interacción (raycast)")]
    [SerializeField] private Camera camaraPrincipal;
    [SerializeField] private float distanciaInteraccion = 3f;

    [Header("Mensajes")]
    [SerializeField] private float duracionMensajeTemporal = 2f;

    [Header("Objetos del tutorial")]
    [SerializeField] private GameObject linternaObjeto;

    void Start()
    {
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;
        pasoActual = 0;

        if (textoTemporal != null) textoTemporal.text = "";
        if (linternaObjeto != null) linternaObjeto.SetActive(false);

        MostrarPaso();
    }

    void Update()
    {
        // 🔹 Control del panel según el objeto externo, pero solo mientras el tutorial esté activo
        if (tutorialActivo && panelTutorial != null && objetoControl != null)
        {
            if (objetoControl.activeSelf)
                panelTutorial.SetActive(false);
            else
                panelTutorial.SetActive(true);
        }

        // Si el tutorial ya terminó, no hacer nada más
        if (!tutorialActivo) return;

        bool w = Input.GetKey(KeyCode.W);
        bool a = Input.GetKey(KeyCode.A);
        bool s = Input.GetKey(KeyCode.S);
        bool d = Input.GetKey(KeyCode.D);

        int movimientoCount = (w ? 1 : 0) + (a ? 1 : 0) + (s ? 1 : 0) + (d ? 1 : 0);

        bool movimientoPresionadoDown = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                                        Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D);

        bool movimientoSostenido = movimientoCount > 0;

        bool shiftSostenido = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool shiftPresionadoDown = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);

        switch (pasoActual)
        {
            case 0:
                if (!presionoMovimiento && movimientoCount >= 2)
                {
                    presionoMovimiento = true;
                    SiguientePaso();
                }
                break;

            case 1:
                if (!corrio && ((movimientoSostenido && shiftPresionadoDown) ||
                                (shiftSostenido && movimientoPresionadoDown) ||
                                (shiftSostenido && movimientoSostenido)))
                {
                    corrio = true;
                    SiguientePaso();
                }
                break;

            case 2:
                if (linternaObjeto != null && !linternaObjeto.activeSelf)
                    linternaObjeto.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Ray ray = new Ray(camaraPrincipal.transform.position, camaraPrincipal.transform.forward);
                    if (Physics.Raycast(ray, out RaycastHit hit, distanciaInteraccion))
                    {
                        if (hit.collider.GetComponent<LinternaPickup>() != null)
                        {
                            tieneLinterna = true;
                            MostrarMensajeTemporal("Has recogido la linterna. Ahora presiona F para encenderla.", duracionMensajeTemporal);
                            SiguientePaso();
                        }
                        else if (hit.collider.GetComponent<PuertaTutorial>() != null ||
                                 hit.collider.GetComponent<CajonesInteractuables>() != null)
                        {
                            SiguientePaso();
                        }
                    }
                }
                break;

            case 3:
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (tieneLinterna)
                    {
                        encendioLinterna = true;
                        SiguientePaso();
                    }
                    else
                    {
                        MostrarMensajeTemporal("No tienes linterna, ve y búscala.", duracionMensajeTemporal);
                    }
                }
                break;
        }
    }

    public void MostrarPaso()
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
        pasoActual = 0;

        if (panelTutorial != null)
            panelTutorial.SetActive(false); // 🔹 apagar panel definitivamente

        Debug.Log("✅ Tutorial completado y panel apagado.");
    }


    public void NotificarLinternaRecogida()
    {
        tieneLinterna = true;
        if (pasoActual == 2)
        {
            MostrarMensajeTemporal("Has recogido la linterna. Ahora presiona F para encenderla.", duracionMensajeTemporal);
            SiguientePaso();
        }
    }

    private Coroutine mensajeCoroutine;
    private void MostrarMensajeTemporal(string mensaje, float duracion)
    {
        if (textoTemporal == null) return;
        if (mensajeCoroutine != null) StopCoroutine(mensajeCoroutine);
        mensajeCoroutine = StartCoroutine(MostrarMensajeCoroutine(mensaje, duracion));
    }

    private IEnumerator MostrarMensajeCoroutine(string mensaje, float duracion)
    {
        textoTemporal.text = mensaje;
        textoTemporal.gameObject.SetActive(true);
        yield return new WaitForSeconds(duracion);
        textoTemporal.text = "";
        textoTemporal.gameObject.SetActive(false);
        mensajeCoroutine = null;
    }

    public bool EstaActivo() => tutorialActivo;
    public int ObtenerPasoActual() => pasoActual;
    public bool TieneLinterna() => tieneLinterna;
}
