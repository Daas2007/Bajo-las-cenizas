using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialInteractivo : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panelTutorial;
    [SerializeField] private TMP_Text textoTutorial;
    [SerializeField] private TMP_Text textoTemporal;

    [Header("Dialogo inicial")]
    [SerializeField] private Dialogo dialogo; // 🔹 referencia al sistema de diálogo

    [Header("Pasos del tutorial")]
    [SerializeField]
    private string[] pasos = {
        "Presiona dos teclas entre W A S D para moverte",
        "Mantén SHIFT mientras te mueves para correr",
        "Presiona E para interactuar (recoge la linterna si la ves)",
        "Presiona F para encender la linterna"
    };

    private int pasoActual = 0;
    private bool tutorialActivo = false; // 🔹 ahora empieza desactivado, se activa tras el diálogo

    // Flags
    private bool presionoMovimiento = false;
    private bool corrio = false;
    private bool encendioLinterna = false;
    private bool tieneLinterna = false;

    [Header("Interacción (raycast)")]
    [SerializeField] private Camera camaraPrincipal;
    [SerializeField] private float distanciaInteraccion = 3f;
    [SerializeField] private string layerInteraccionName = "Interaccion";

    [Header("Mensajes")]
    [SerializeField] private float duracionMensajeTemporal = 2f;

    [Header("Objetos del tutorial")]
    [SerializeField] private GameObject linternaObjeto;

    void Start()
    {
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;
        pasoActual = 0;

        if (textoTemporal != null) textoTemporal.text = "";

        // 🔹 Linterna desactivada al inicio
        if (linternaObjeto != null) linternaObjeto.SetActive(false);

        // 🔹 Panel del tutorial apagado al inicio
        if (panelTutorial != null) panelTutorial.SetActive(false);

        // 🔹 Iniciar diálogo antes del tutorial
        if (dialogo != null)
        {
            dialogo.ResetHaHablado();
            dialogo.OnDialogoCompleto.AddListener(() =>
            {
                ActivarTutorial();
            });
            dialogo.IniciarDialogo();
        }
        else
        {
            // Si no hay diálogo, activar tutorial directamente
            ActivarTutorial();
        }
    }

    private void ActivarTutorial()
    {
        tutorialActivo = true;
        if (panelTutorial != null) panelTutorial.SetActive(true);
        MostrarPaso();
    }

    void Update()
    {
        if (!tutorialActivo) return;

        bool w = Input.GetKey(KeyCode.W);
        bool a = Input.GetKey(KeyCode.A);
        bool s = Input.GetKey(KeyCode.S);
        bool d = Input.GetKey(KeyCode.D);

        int movimientoCount = 0;
        if (w) movimientoCount++;
        if (a) movimientoCount++;
        if (s) movimientoCount++;
        if (d) movimientoCount++;

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
                if (!corrio)
                {
                    if ((movimientoSostenido && shiftPresionadoDown) ||
                        (shiftSostenido && movimientoPresionadoDown) ||
                        (shiftSostenido && movimientoSostenido))
                    {
                        corrio = true;
                        SiguientePaso();
                    }
                }
                break;

            case 2:
                if (linternaObjeto != null && !linternaObjeto.activeSelf)
                    linternaObjeto.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (camaraPrincipal == null)
                    {
                        Debug.LogWarning("[TutorialInteractivo] Cámara principal no asignada.");
                        break;
                    }

                    Ray ray = new Ray(camaraPrincipal.transform.position, camaraPrincipal.transform.forward);
                    if (Physics.Raycast(ray, out RaycastHit hit, distanciaInteraccion))
                    {
                        if (hit.collider.GetComponent<LinternaPickup>() != null)
                        {
                            tieneLinterna = true;
                            MostrarMensajeTemporal("Has recogido la linterna. Ahora presiona F para encenderla.", duracionMensajeTemporal);
                            SiguientePaso();
                        }
                        else if (hit.collider.GetComponent<PuertaTutorial>() != null)
                        {
                            SiguientePaso();
                        }
                        else if (hit.collider.GetComponent<CajonesInteractuables>() != null)
                        {
                            SiguientePaso();
                        }
                        else
                        {
                            Debug.Log("[TutorialInteractivo] Objeto interactuado no válido para el tutorial.");
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
        if (panelTutorial != null) panelTutorial.SetActive(false);
        pasoActual = 0;
        Debug.Log("✅ Tutorial completado y panel apagado.");
    }

    public void NotificarLinternaRecogida()
    {
        tieneLinterna = true;
        Debug.Log("[TutorialInteractivo] Notificado: jugador tiene linterna.");
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
