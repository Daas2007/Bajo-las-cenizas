using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class Dialogo : MonoBehaviour, IInteractuable
{
    [Header("UI")]
    [SerializeField] public GameObject dialogoCanvas;
    [SerializeField] public TMP_Text dialogoTexto;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoLetra;
    [SerializeField] private AudioClip sonidoInicio;

    [Header("Líneas de diálogo")]
    [TextArea(2, 5)]
    [SerializeField] public string[] lineas;

    [Header("Configuración")]
    [Tooltip("Tiempo entre cada letra (usa tiempo real para no depender de Time.timeScale)")]
    [SerializeField] public float tiempoEntreLetras = 0.05f;
    [Tooltip("Tiempo en segundos para auto-avanzar a la siguiente línea (si <= 0 no auto-avanza)")]
    [SerializeField] public float tiempoAutoAvance = 5f;
    [Tooltip("Delay antes de empezar la primera línea (segundos, usa tiempo real)")]
    [SerializeField] public float delayInicio = 1f;

    [Header("Movimiento")]
    [SerializeField] public MonoBehaviour scriptMovimiento;

    [Header("Eventos")]
    public UnityEvent OnDialogoCompleto;
    public bool HaHablado { get; private set; } = false;

    // Estado interno
    public int indiceLinea = 0;
    public bool mostrandoDialogo = false;

    // Coroutines y flags
    private Coroutine rutinaTexto;
    private Coroutine autoAvanceCoroutine;
    private Coroutine inicioDelayCoroutine;
    private bool escribiendoLinea = false;
    private string textoActualCompleto = "";


    [SerializeField] private Animator animator;

    // Bandera global para evitar interferencias con otras UIs
    public static bool AnyDialogActive { get; private set; } = false;

    void Awake()
    {
        ResetEstadoInterno();
    }

    void Start()
    {
        if (dialogoCanvas != null)
            dialogoCanvas.SetActive(false);
    }

    void OnDisable()
    {
        DetenerTodo();
        AnyDialogActive = false;
    }

    void Update()
    {
        if (!mostrandoDialogo) return;

        // ESC -> terminar todo el diálogo inmediatamente
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TerminarDialogo();
            return;
        }

        // SPACE -> comportamiento:
        // 1) si hay delay inicial, cancelarlo y empezar ya
        // 2) si se está escribiendo la línea, completar la línea instantáneamente
        // 3) si la línea ya está completa y hay siguiente, avanzar a la siguiente
        // 4) si la línea ya está completa y no hay siguiente, cerrar el panel
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (inicioDelayCoroutine != null)
            {
                StopCoroutine(inicioDelayCoroutine);
                inicioDelayCoroutine = null;
                MostrarLinea(); // iniciar inmediatamente
                return;
            }

            if (escribiendoLinea)
            {
                TerminarLineaInstantanea();
            }
            else
            {
                if (autoAvanceCoroutine != null)
                {
                    StopCoroutine(autoAvanceCoroutine);
                    autoAvanceCoroutine = null;
                }

                if (indiceLinea < lineas.Length - 1)
                    AvanzarDialogo();
                else
                    TerminarDialogo();
            }
        }
    }

    // Interfaz pública
    public void Interactuar()
    {
        if (!mostrandoDialogo)
        {
            IniciarDialogo();
            return;
        }

        // Si ya está mostrando diálogo, replicar la lógica de Space
        if (escribiendoLinea)
            TerminarLineaInstantanea();
        else
        {
            if (autoAvanceCoroutine != null)
            {
                StopCoroutine(autoAvanceCoroutine);
                autoAvanceCoroutine = null;
            }

            if (indiceLinea < lineas.Length - 1)
                AvanzarDialogo();
            else
                TerminarDialogo();
        }
    }

    // Inicia diálogo desde la primera línea (estado limpio)
    public void IniciarDialogo()
    {
        if (dialogoCanvas == null || dialogoTexto == null)
        {
            Debug.LogWarning("[Dialogo] UI no asignada.");
            return;
        }

        DetenerTodo();
        ResetEstadoInterno();

        dialogoCanvas.SetActive(true);
        indiceLinea = 0;
        mostrandoDialogo = true;
        AnyDialogActive = true;

        BloquearJugador(true);

        // ✅ activar animación de hablar
        if (animator != null)
            animator.SetBool("Hablando", true);

        inicioDelayCoroutine = StartCoroutine(DelayYMostrarPrimeraLinea(delayInicio));
    }

    private IEnumerator DelayYMostrarPrimeraLinea(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        inicioDelayCoroutine = null;
        MostrarLinea();
    }

    // Mostrar la línea actual (cancela escritura previa y programa auto-avance si aplica)
    public void MostrarLinea()
    {
        if (lineas == null || lineas.Length == 0)
        {
            TerminarDialogo();
            return;
        }

        // cancelar escritura previa si existe
        if (rutinaTexto != null)
        {
            StopCoroutine(rutinaTexto);
            rutinaTexto = null;
            escribiendoLinea = false;
        }

        textoActualCompleto = lineas[indiceLinea];
        dialogoTexto.text = "";
        rutinaTexto = StartCoroutine(EscribirLinea(textoActualCompleto));

        // cancelar auto-avance previo
        if (autoAvanceCoroutine != null)
        {
            StopCoroutine(autoAvanceCoroutine);
            autoAvanceCoroutine = null;
        }

        // programar auto-avance usando tiempo real (no depende de Time.timeScale)
        if (tiempoAutoAvance > 0f)
            autoAvanceCoroutine = StartCoroutine(AutoAvanzarCoroutine(tiempoAutoAvance));
    }

    // Escritura letra a letra (usa WaitForSecondsRealtime)
    private IEnumerator EscribirLinea(string linea)
    {
        escribiendoLinea = true;
        dialogoTexto.text = "";

        for (int i = 0; i < linea.Length; i++)
        {
            // Si escribiendo fue cancelado externamente, salir del bucle
            if (!escribiendoLinea) break;

            dialogoTexto.text += linea[i];

            // Solo reproducir sonido si seguimos en modo escribiendo
            if (escribiendoLinea && audioSource != null && sonidoLetra != null)
                audioSource.PlayOneShot(sonidoLetra);

            yield return new WaitForSecondsRealtime(tiempoEntreLetras);
        }

        // Si no fue cancelado, asegurar texto completo
        if (escribiendoLinea)
            dialogoTexto.text = linea;

        escribiendoLinea = false;
        rutinaTexto = null;
    }

    // Completa la línea instantáneamente
    private void TerminarLineaInstantanea()
    {
        if (rutinaTexto != null)
        {
            StopCoroutine(rutinaTexto);
            rutinaTexto = null;
        }

        if (lineas != null && indiceLinea >= 0 && indiceLinea < lineas.Length)
            dialogoTexto.text = lineas[indiceLinea];

        escribiendoLinea = false;
    }

    public void AvanzarDialogo()
    {
        // cancelar auto-avance actual
        if (autoAvanceCoroutine != null)
        {
            StopCoroutine(autoAvanceCoroutine);
            autoAvanceCoroutine = null;
        }

        indiceLinea++;

        if (indiceLinea < lineas.Length)
            MostrarLinea();
        else
            TerminarDialogo();
    }

    private IEnumerator AutoAvanzarCoroutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        // si aún se está escribiendo, terminar instantáneamente y esperar un pequeño margen
        if (escribiendoLinea)
        {
            TerminarLineaInstantanea();
            yield return new WaitForSecondsRealtime(0.25f);
        }

        if (indiceLinea >= lineas.Length - 1)
            TerminarDialogo();
        else
            AvanzarDialogo();

        autoAvanceCoroutine = null;
    }

    // Termina diálogo y limpia todo (detiene coroutines y sonidos)
    public void TerminarDialogo()
    {
        DetenerTodo();

        if (dialogoCanvas != null)
            dialogoCanvas.SetActive(false);

        mostrandoDialogo = false;
        escribiendoLinea = false;
        AnyDialogActive = false;

        BloquearJugador(false);

        // ✅ volver a estado normal
        if (animator != null)
            animator.SetBool("Hablando", false);

        if (!HaHablado)
        {
            HaHablado = true;
            OnDialogoCompleto?.Invoke();
        }
    }

    // Detiene todas las coroutines y evita que se sigan reproduciendo sonidos por escritura
    private void DetenerTodo()
    {
        if (rutinaTexto != null) { StopCoroutine(rutinaTexto); rutinaTexto = null; }
        if (autoAvanceCoroutine != null) { StopCoroutine(autoAvanceCoroutine); autoAvanceCoroutine = null; }
        if (inicioDelayCoroutine != null) { StopCoroutine(inicioDelayCoroutine); inicioDelayCoroutine = null; }

        // Marcar que no se está escribiendo para que EscribirLinea deje de reproducir sonidos
        escribiendoLinea = false;

        // Intentar detener cualquier clip que esté sonando en el AudioSource (si aplica)
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void BloquearJugador(bool bloquear)
    {
        if (scriptMovimiento != null)
            scriptMovimiento.enabled = !bloquear;
    }

    // Resetea variables internas sin tocar UI
    private void ResetEstadoInterno()
    {
        indiceLinea = 0;
        mostrandoDialogo = false;
        escribiendoLinea = false;
        rutinaTexto = null;
        autoAvanceCoroutine = null;
        inicioDelayCoroutine = null;
        textoActualCompleto = "";
    }

    public void ResetHaHablado()
    {
        HaHablado = false;
    }
}
