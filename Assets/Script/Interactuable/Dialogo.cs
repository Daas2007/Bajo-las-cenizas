using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class Dialogo : MonoBehaviour, IInteractuable
{
    [SerializeField] public GameObject dialogoCanvas;
    [SerializeField] public TMP_Text dialogoTexto;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoLetra;     // sonido tipo Undertale por letra
    [SerializeField] private AudioClip sonidoInicio;    // sonido al empezar diálogo

    [Header("Líneas de diálogo")]
    [TextArea(2, 5)]
    [SerializeField] public string[] lineas;

    [Header("Configuración")]
    [SerializeField] public float tiempoEntreLetras = 0.05f;
    [SerializeField] public float tiempoAutoAvance = 5f;

    public int indiceLinea = 0;
    public bool mostrandoDialogo = false;
    public Coroutine rutinaTexto;
    private bool escribiendoLinea = false;

    [SerializeField] public MonoBehaviour scriptMovimiento;

    [Header("Eventos")]
    public UnityEvent OnDialogoCompleto;
    public bool HaHablado { get; private set; } = false;

    private string[] lineasOriginales;

    void Start()
    {
        if (dialogoCanvas != null)
            dialogoCanvas.SetActive(false);

        if (lineas != null)
            lineasOriginales = (string[])lineas.Clone();
    }

    void Update()
    {
        if (!mostrandoDialogo) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (escribiendoLinea)
            {
                TerminarLineaInstantanea();
            }
            else
            {
                CancelInvoke(nameof(AvanzarDialogo));

                if (indiceLinea >= lineas.Length - 1)
                {
                    TerminarDialogo();
                }
                else
                {
                    AvanzarDialogo();
                }
            }
        }
    }

    public void Interactuar()
    {
        if (!mostrandoDialogo)
        {
            IniciarDialogo();
        }
        else
        {
            if (escribiendoLinea)
            {
                TerminarLineaInstantanea();
            }
            else
            {
                CancelInvoke(nameof(AvanzarDialogo));

                if (indiceLinea >= lineas.Length - 1)
                {
                    TerminarDialogo();
                }
                else
                {
                    AvanzarDialogo();
                }
            }
        }
    }

    public void IniciarDialogo()
    {
        if (dialogoCanvas == null || dialogoTexto == null)
        {
            Debug.LogWarning("[Dialogo] UI no asignada.");
            return;
        }

        dialogoCanvas.SetActive(true);
        indiceLinea = 0;
        mostrandoDialogo = true;
        MostrarLinea();
        BloquearJugador(true);

        // 🔊 reproducir sonido de inicio
        if (audioSource != null && sonidoInicio != null)
            audioSource.PlayOneShot(sonidoInicio);
    }

    // 🔹 Método que tu PuertaConCondicion necesita
    public void IniciarDialogoConLineas(string[] nuevasLineas, bool marcarComoHablado = false)
    {
        if (nuevasLineas == null || nuevasLineas.Length == 0)
            return;

        if (lineasOriginales == null && lineas != null)
            lineasOriginales = (string[])lineas.Clone();

        lineas = (string[])nuevasLineas.Clone();
        indiceLinea = 0;
        mostrandoDialogo = true;

        if (dialogoCanvas != null) dialogoCanvas.SetActive(true);
        MostrarLinea();

        StartCoroutine(RestaurarLineasAlTerminar(marcarComoHablado));
    }

    private IEnumerator RestaurarLineasAlTerminar(bool marcarComoHablado)
    {
        while (mostrandoDialogo)
            yield return null;

        if (lineasOriginales != null)
            lineas = (string[])lineasOriginales.Clone();

        if (marcarComoHablado)
        {
            HaHablado = true;
            OnDialogoCompleto?.Invoke();
        }
    }

    public void MostrarLinea()
    {
        if (rutinaTexto != null) StopCoroutine(rutinaTexto);
        rutinaTexto = StartCoroutine(EscribirLinea(lineas[indiceLinea]));
        Invoke(nameof(AvanzarDialogo), tiempoAutoAvance);
    }

    public IEnumerator EscribirLinea(string linea)
    {
        escribiendoLinea = true;
        dialogoTexto.text = "";
        foreach (char letra in linea)
        {
            dialogoTexto.text += letra;

            // 🔊 reproducir sonido por cada letra
            if (audioSource != null && sonidoLetra != null)
                audioSource.PlayOneShot(sonidoLetra);

            yield return new WaitForSeconds(tiempoEntreLetras);
        }
        escribiendoLinea = false;
    }

    private void TerminarLineaInstantanea()
    {
        if (rutinaTexto != null) StopCoroutine(rutinaTexto);
        dialogoTexto.text = lineas[indiceLinea];
        escribiendoLinea = false;
    }

    public void AvanzarDialogo()
    {
        CancelInvoke(nameof(AvanzarDialogo));
        indiceLinea++;

        if (indiceLinea < lineas.Length)
        {
            MostrarLinea();
        }
        else
        {
            TerminarDialogo();
        }
    }

    public void TerminarDialogo()
    {
        dialogoCanvas.SetActive(false);
        mostrandoDialogo = false;
        BloquearJugador(false);

        if (!HaHablado)
        {
            HaHablado = true;
            OnDialogoCompleto?.Invoke();
        }

        if (lineasOriginales != null)
            lineas = (string[])lineasOriginales.Clone();
    }

    public void BloquearJugador(bool bloquear)
    {
        if (scriptMovimiento != null)
            scriptMovimiento.enabled = !bloquear;
    }

    public void ResetHaHablado()
    {
        HaHablado = false;
    }
}
