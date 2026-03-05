using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialInteractivo : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panelTutorial;
    [SerializeField] private TMP_Text textoTutorial;
    [SerializeField] private TMP_Text textoTemporal; // para mensajes cortos (ej. "No tienes linterna")

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

    // Flags de acciones
    private bool presionoMovimiento = false;
    private bool corrio = false;
    private bool interactuo = false;
    private bool encendioLinterna = false;
    private bool tieneLinterna = false;

    [Header("Interacción (raycast)")]
    [SerializeField] private Camera camaraPrincipal;
    [SerializeField] private float distanciaInteraccion = 3f;
    [SerializeField] private string layerInteraccionName = "Interaccion";

    [Header("Mensajes")]
    [SerializeField] private float duracionMensajeTemporal = 2f;

    void Start()
    {
        if (panelTutorial != null) panelTutorial.SetActive(true);
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;
        MostrarPaso();
        if (textoTemporal != null) textoTemporal.text = "";
    }

    void Update()
    {
        if (!tutorialActivo) return;

        // Detectores de movimiento
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
                // Requiere al menos 2 teclas de movimiento sostenidas simultáneamente
                if (!presionoMovimiento && movimientoCount >= 2)
                {
                    presionoMovimiento = true;
                    SiguientePaso();
                }
                break;

            case 1:
                // Requiere SHIFT mientras se mueve (acepta secuencia: moverse y luego SHIFT, o SHIFT y luego moverse)
                if (!corrio)
                {
                    // Caso: ya se mueve y presiona SHIFT
                    if (movimientoSostenido && shiftPresionadoDown)
                    {
                        corrio = true;
                        SiguientePaso();
                        break;
                    }

                    // Caso: ya mantiene SHIFT y luego empieza a moverse
                    if (shiftSostenido && movimientoPresionadoDown)
                    {
                        corrio = true;
                        SiguientePaso();
                        break;
                    }

                    // Caso: ambas sostenidas
                    if (shiftSostenido && movimientoSostenido)
                    {
                        corrio = true;
                        SiguientePaso();
                        break;
                    }
                }
                break;

            case 2:
                // Interactuar con E: solo avanza si el raycast golpea un objeto en la layer "Interaccion"
                if (Input.GetKeyDown(KeyCode.E))
                {
                    bool hitInteraccion = IntentarInteractuarRaycast();
                    if (hitInteraccion)
                    {
                        interactuo = true;
                        SiguientePaso();
                    }
                    else
                    {
                        // Si no hay objeto de interacción y no tiene linterna, mostrar mensaje para recoger linterna
                        if (!tieneLinterna)
                        {
                            MostrarMensajeTemporal("No tienes  alguna linterna, ve y buscala", duracionMensajeTemporal);
                        }
                        else
                        {
                            // Si tiene linterna pero no hay objeto en la dirección, dar feedback genérico
                            MostrarMensajeTemporal("No hay nada con qué interactuar aquí.", duracionMensajeTemporal);
                        }
                    }
                }
                break;

            case 3:
                // Encender linterna con F (solo si tiene linterna)
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (tieneLinterna)
                    {
                        encendioLinterna = true;
                        SiguientePaso();
                    }
                    else
                    {
                        MostrarMensajeTemporal("No tienes  alguna linterna, ve y buscala", duracionMensajeTemporal);
                    }
                }
                break;
        }
    }

    private bool IntentarInteractuarRaycast()
    {
        if (camaraPrincipal == null)
        {
            Debug.LogWarning("[TutorialInteractivo] Cámara principal no asignada para raycast.");
            return false;
        }

        Ray ray = new Ray(camaraPrincipal.transform.position, camaraPrincipal.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, distanciaInteraccion))
        {
            int layerIndex = LayerMask.NameToLayer(layerInteraccionName);
            if (layerIndex == -1)
            {
                Debug.LogWarning($"[TutorialInteractivo] Layer '{layerInteraccionName}' no existe.");
                return false;
            }

            if (hit.collider != null && hit.collider.gameObject.layer == layerIndex)
            {
                // Si quieres, puedes notificar al objeto interactuado aquí (si implementa IInteractuable)
                var interactuable = hit.collider.gameObject.GetComponent<IInteractuable>();
                if (interactuable != null)
                {
                    interactuable.Interactuar();
                }

                return true;
            }
        }

        return false;
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

    // Notificaciones externas (por si tu sistema de recolección/objetos quiere notificar)
    // Llamar cuando el jugador recoge la linterna
    public void NotificarLinternaRecogida()
    {
        tieneLinterna = true;
        Debug.Log("[TutorialInteractivo] Notificado: jugador tiene linterna.");

        // Si estamos en el paso de interactuar (2), avanzamos automáticamente al paso de encender (3)
        if (pasoActual == 2)
        {
            MostrarMensajeTemporal("Has recogido la linterna. Ahora presiona F para encenderla.", duracionMensajeTemporal);
            SiguientePaso();
        }
    }

    // Si tu sistema de interacción prefiere notificar directamente que se interactuó con un objeto:
    // Llamar NotificarInteraccion(obj) con el GameObject con el que se interactuó.
    public void NotificarInteraccion(GameObject obj)
    {
        if (obj == null) return;

        int layerIndex = LayerMask.NameToLayer(layerInteraccionName);
        if (layerIndex == -1)
        {
            Debug.LogWarning($"[TutorialInteractivo] Layer '{layerInteraccionName}' no existe.");
            return;
        }

        if (obj.layer == layerIndex)
        {
            interactuo = true;
            if (pasoActual == 2) SiguientePaso();
        }
    }

    // Mensajes temporales en pantalla
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

    // Estado público por si otros sistemas quieren consultarlo
    public bool EstaActivo() => tutorialActivo;
    public int ObtenerPasoActual() => pasoActual;
    public bool TieneLinterna() => tieneLinterna;
}