using UnityEngine;
using TMPro;
using System.Collections;

public class PuertaTutorial : MonoBehaviour, IInteractuable
{
    [Header("Configuración de apertura")]
    [SerializeField] float anguloApertura = 90f;
    [SerializeField] float velocidadRotacion = 2f;
    [SerializeField] Transform pivote;

    [Header("Puerta vinculada")]
    [SerializeField] PuertaTutorial puertaVinculada;

    [Header("UI de diálogo")]
    [SerializeField] GameObject panelDialogo;
    [SerializeField] TMP_Text textoDialogo;
    [SerializeField] TMP_Text textoSaltar;

    [Header("Configuración del texto")]
    [SerializeField] float tiempoEntreLetras;
    [SerializeField] float tiempoVisibleDespues = 5f;

    [Header("PersonajeMovimiento")]
    [SerializeField] MovimientoPersonaje quedateQuieto;

    public bool abierta = false;
    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;

    private Coroutine rutinaTexto;
    private bool puedeActivar = true;
    private string mensajeActual = "";
    private bool escribiendo = false;

    void Start()
    {
        if (pivote == null) pivote = transform;
        rotacionInicial = pivote.rotation;
        rotacionFinal = rotacionInicial * Quaternion.Euler(0f, anguloApertura, 0f);

        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (textoSaltar != null) textoSaltar.gameObject.SetActive(false);
    }
    void Update()
    {
        if (panelDialogo != null && panelDialogo.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (escribiendo)
            {
                // Terminar escritura inmediatamente
                if (rutinaTexto != null) StopCoroutine(rutinaTexto);
                textoDialogo.text = mensajeActual;
                escribiendo = false;
            }
            else
            {
                // Cerrar diálogo si ya terminó
                CerrarDialogo();
            }
        }
    }
    //---------------Interfaz---------------
    public void Interactuar()
    {
        JugadorLinterna jugadorLinterna = FindObjectOfType<JugadorLinterna>();
        if (jugadorLinterna != null && jugadorLinterna.TieneLinterna())
        {
            if (!abierta)
                AbrirPuertasSimultaneas();
        }
        else
        {
            if (puedeActivar && !panelDialogo.activeSelf)
                MostrarDialogo("Mmm... está bastante oscuro afuera, será mejor que busque algo para iluminar");
        }
    }
    //---------------Reset---------------
    public void ResetPuerta()
    {
        abierta = false;
        pivote.rotation = rotacionInicial;
        gameObject.layer = LayerMask.NameToLayer("Interaccion");
    }

    //---------------Apertura simultánea---------------
    private void AbrirPuertasSimultaneas()
    {
        abierta = true;
        gameObject.layer = LayerMask.NameToLayer("Default");

        StartCoroutine(RotarPuerta());

        if (puertaVinculada != null && !puertaVinculada.abierta)
        {
            puertaVinculada.AbrirPuertaVinculada();
        }
    }

    public void AbrirPuertaVinculada()
    {
        abierta = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        StartCoroutine(RotarPuerta());
    }

    private IEnumerator RotarPuerta()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * velocidadRotacion;
            pivote.rotation = Quaternion.Slerp(rotacionInicial, rotacionFinal, t);
            yield return null;
        }
    }

    //---------------Diálogo---------------
    private void MostrarDialogo(string mensaje)
    {
        if (panelDialogo == null || textoDialogo == null) return;

        mensajeActual = mensaje;
        panelDialogo.SetActive(true);
        if (textoSaltar != null) textoSaltar.gameObject.SetActive(true);

        if (quedateQuieto != null) quedateQuieto.enabled = false;

        if (rutinaTexto != null) StopCoroutine(rutinaTexto);
        rutinaTexto = StartCoroutine(EscribirLinea(mensaje));
    }

    private IEnumerator EscribirLinea(string linea)
    {
        escribiendo = true;
        textoDialogo.text = "";
        foreach (char letra in linea)
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(tiempoEntreLetras * Time.deltaTime);
        }

        escribiendo = false;
        yield return new WaitForSeconds(tiempoVisibleDespues);
        CerrarDialogo();
    }

    private void CerrarDialogo()
    {
        if (rutinaTexto != null) StopCoroutine(rutinaTexto);
        panelDialogo.SetActive(false);
        if (textoSaltar != null) textoSaltar.gameObject.SetActive(false);
        textoDialogo.text = "";

        if (quedateQuieto != null) quedateQuieto.enabled = true;

        puedeActivar = false;
        Invoke(nameof(ReactivarDialogo), 2f);
    }

    private void ReactivarDialogo()
    {
        puedeActivar = true;
    }
}