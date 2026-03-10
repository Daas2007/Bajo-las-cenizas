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

    // Nuevo: si tras interactuar con algo que no es la linterna, el tutorial exige que recojas la linterna
    private bool requiereIrPorLinterna = false;

    [Header("Interacción (raycast)")]
    [SerializeField] private Camera camaraPrincipal;
    [SerializeField] private float distanciaInteraccion = 3f;
    [SerializeField] private string layerInteraccionName = "Interaccion";

    // Identificación de la linterna: tag o componente
    [Header("Identificación de la linterna")]
    [SerializeField] private string Pickup = "Linterna";

    [Header("Mensajes")]
    [SerializeField] private float duracionMensajeTemporal = 2f;

    public void Start()
    {
        if (panelTutorial != null) panelTutorial.SetActive(true);
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;
        pasoActual = 0;
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
                    if (movimientoSostenido && shiftPresionadoDown)
                    {
                        corrio = true;
                        SiguientePaso();
                        break;
                    }

                    if (shiftSostenido && movimientoPresionadoDown)
                    {
                        corrio = true;
                        SiguientePaso();
                        break;
                    }

                    if (shiftSostenido && movimientoSostenido)
                    {
                        corrio = true;
                        SiguientePaso();
                        break;
                    }
                }
                break;
            case 2:
                // Interactuar con E con cualquier objeto en la layer Interaccion
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
                        int layerIndex = LayerMask.NameToLayer(layerInteraccionName);
                        if (layerIndex != -1 && hit.collider.gameObject.layer == layerIndex)
                        {
                            // Ejecutar interacción si existe
                            var interactuable = hit.collider.GetComponent<IInteractuable>();
                            if (interactuable != null) interactuable.Interactuar();

                            // Si el objeto es la linterna, marcar que la tiene
                            if (hit.collider.CompareTag(Pickup) || hit.collider.GetComponent("LinternaPickup") != null)
                            {
                                tieneLinterna = true;
                                MostrarMensajeTemporal("Has recogido la linterna. Ahora presiona F para encenderla.", duracionMensajeTemporal);
                            }

                            // Avanzar al siguiente paso (mostrar texto de F)
                            SiguientePaso();
                        }
                    }
                }
                break;
            case 3:
                // Encender linterna con F (solo si la tiene)
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

    // Notificaciones externas (por si tu sistema de recolección/objetos quiere notificar)
    // Llamar cuando el jugador recoge la linterna desde otro sistema
    public void NotificarLinternaRecogida()
    {
        tieneLinterna = true;
        Debug.Log("[TutorialInteractivo] Notificado: jugador tiene linterna.");

        // Si estamos en el paso de interactuar (2) y el tutorial estaba esperando que fueras por la linterna, avanzamos al paso de encender (3)
        if (pasoActual == 2)
        {
            if (requiereIrPorLinterna)
            {
                MostrarMensajeTemporal("Has recogido la linterna. Ahora presiona F para encenderla.", duracionMensajeTemporal);
                requiereIrPorLinterna = false;
                SiguientePaso();
            }
            else
            {
                // Si por alguna razón no se requería, aún así avanzamos para mantener flujo
                MostrarMensajeTemporal("Has recogido la linterna. Ahora presiona F para encenderla.", duracionMensajeTemporal);
                SiguientePaso();
            }
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
            // Si el objeto es la linterna, marcar y avanzar
            bool esLinterna = false;
            if (!string.IsNullOrEmpty(Pickup) && obj.CompareTag(Pickup))
                esLinterna = true;
            else if (obj.GetComponent("LinternaPickup") != null)
                esLinterna = true;

            if (esLinterna)
            {
                tieneLinterna = true;
                requiereIrPorLinterna = false;
                if (pasoActual == 2) SiguientePaso();
            }
            else
            {
                // Interactuó con otro objeto: pedir que vaya por la linterna
                requiereIrPorLinterna = true;
                MostrarMensajeTemporal("Ahora ve y recoge la linterna.", duracionMensajeTemporal);
            }
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
