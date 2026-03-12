using UnityEngine;
using TMPro;
using System.Collections;

public class PuertaConCondicion : MonoBehaviour, IInteractuable
{
    [Header("Puerta base")]
    [SerializeField] private PuertaInteractuable puertaBase;

    [Header("Condición: NPC con el que hay que hablar primero")]
    [SerializeField] private Dialogo npcDialogo;

    [Header("UI Mensaje temporal")]
    [SerializeField] private GameObject panelMensaje;
    [SerializeField] private TMP_Text textoMensaje;
    [SerializeField] private float duracionMensaje = 2.5f;

    [Header("Movimiento")]
    [Tooltip("Arrastra aquí el componente que controla el movimiento del jugador (por ejemplo MovimientoPersonaje)")]
    [SerializeField] private MonoBehaviour Personaje;

    [Header("Texto si no ha hablado")]
    [SerializeField] private string mensajeSiNoHaHablado = "Hey tú, ven al fondo";

    [Header("Escritura tipo Undertale")]
    [SerializeField] private float tiempoEntreLetras = 0.03f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoLetra;
    [SerializeField] private AudioClip sonidoInicio;

    // Estado
    private Coroutine hideCoroutine;
    private Coroutine writeCoroutine;
    private bool escribiendo = false;
    private string currentFullText = "";

    void Start()
    {
        if (panelMensaje != null)
            panelMensaje.SetActive(false);
    }

    void Update()
    {
        // Si hay un diálogo global activo, ignorar entradas para evitar interferencias
        if (Dialogo.AnyDialogActive) return;

        if (panelMensaje != null && panelMensaje.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (escribiendo)
                    TerminarEscrituraInstantanea();
                else
                    CancelarMensajeTemporal();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                CancelarMensajeTemporal();
        }
    }

    public void Interactuar()
    {
        if (puertaBase == null)
        {
            Debug.LogWarning("[PuertaConCondicion] puertaBase no asignada.");
            return;
        }

        if (npcDialogo == null)
        {
            MostrarMensajeTemporal(mensajeSiNoHaHablado);
            return;
        }

        if (npcDialogo.HaHablado)
            puertaBase.Interactuar();
        else
            MostrarMensajeTemporal(mensajeSiNoHaHablado);
    }

    private void MostrarMensajeTemporal(string texto)
    {
        if (panelMensaje == null || textoMensaje == null) return;

        // Detener coroutines previas
        if (hideCoroutine != null) { StopCoroutine(hideCoroutine); hideCoroutine = null; }
        if (writeCoroutine != null) { StopCoroutine(writeCoroutine); writeCoroutine = null; }

        panelMensaje.SetActive(true);
        textoMensaje.text = "";
        currentFullText = texto;

        // Bloquear movimiento
        if (Personaje != null) Personaje.enabled = false;

        // Sonido de inicio (opcional)
        if (audioSource != null && sonidoInicio != null) audioSource.PlayOneShot(sonidoInicio);

        // Iniciar escritura
        writeCoroutine = StartCoroutine(WriteRoutine(currentFullText));

        // Programar ocultado
        if (duracionMensaje > 0f)
            hideCoroutine = StartCoroutine(HideRoutine(duracionMensaje));
    }

    private IEnumerator WriteRoutine(string texto)
    {
        escribiendo = true;
        textoMensaje.text = "";

        for (int i = 0; i < texto.Length; i++)
        {
            // Si escribiendo fue cancelado externamente, salir
            if (!escribiendo) break;

            textoMensaje.text += texto[i];

            if (escribiendo && audioSource != null && sonidoLetra != null)
                audioSource.PlayOneShot(sonidoLetra);

            yield return new WaitForSecondsRealtime(tiempoEntreLetras);
        }

        // Si no fue cancelado, asegurar texto completo
        if (escribiendo)
            textoMensaje.text = texto;

        escribiendo = false;
        writeCoroutine = null;
    }

    private void TerminarEscrituraInstantanea()
    {
        if (writeCoroutine != null)
        {
            StopCoroutine(writeCoroutine);
            writeCoroutine = null;
        }

        textoMensaje.text = currentFullText;
        escribiendo = false;
    }

    private IEnumerator HideRoutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        // Si aún escribiendo, completar primero
        if (escribiendo)
        {
            TerminarEscrituraInstantanea();
            yield return new WaitForSecondsRealtime(0.15f);
        }

        // Ocultar y reactivar movimiento
        if (panelMensaje != null) panelMensaje.SetActive(false);
        if (Personaje != null) Personaje.enabled = true;

        hideCoroutine = null;
        currentFullText = "";
    }

    public void CancelarMensajeTemporal()
    {
        // Detener coroutines
        if (hideCoroutine != null) { StopCoroutine(hideCoroutine); hideCoroutine = null; }
        if (writeCoroutine != null) { StopCoroutine(writeCoroutine); writeCoroutine = null; }

        // Ocultar y reactivar movimiento
        if (panelMensaje != null) panelMensaje.SetActive(false);
        if (Personaje != null) Personaje.enabled = true;

        escribiendo = false;
        currentFullText = "";
    }
}
