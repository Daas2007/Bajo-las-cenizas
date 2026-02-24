using UnityEngine;
using TMPro;
using System.Collections;

public class PuertaTutorial : MonoBehaviour, IInteractuable
{
    //---------------Configuración de apertura---------------
    [Header("Configuración de apertura")]
    [SerializeField] float anguloApertura = 90f;
    [SerializeField] float velocidadRotacion = 2f;
    [SerializeField] Transform pivote;

    //---------------Puerta vinculada---------------
    [Header("Puerta vinculada")]
    [SerializeField] PuertaTutorial puertaVinculada; // referencia a la otra puerta

    //---------------UI de diálogo---------------
    [Header("UI de diálogo")]
    [SerializeField] GameObject panelDialogo;
    [SerializeField] TMP_Text textoDialogo;
    [SerializeField] TMP_Text textoSaltar;

    //---------------Configuración del texto---------------
    [Header("Configuración del texto")]
    [SerializeField] float tiempoEntreLetras;
    [SerializeField] float tiempoVisibleDespues = 5f;

    //---------------Movimiento personaje---------------
    [Header("PersonajeMovimiento")]
    [SerializeField] MovimientoPersonaje quedateQuieto;

    private bool abierta = false;
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
        if (panelDialogo.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (escribiendo)
            {
                if (rutinaTexto != null) StopCoroutine(rutinaTexto);
                textoDialogo.text = mensajeActual;
                escribiendo = false;
                CerrarDialogo(); // cerrar inmediatamente
            }
            else
            {
                CerrarDialogo();
            }
        }
    }

    //---------------Interacción---------------
    public void Interactuar()
    {
        JugadorLinterna jugadorLinterna = FindFirstObjectByType<JugadorLinterna>();
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

    //---------------Apertura simultánea---------------
    private void AbrirPuertasSimultaneas()
    {
        abierta = true;
        gameObject.layer = LayerMask.NameToLayer("Default");

        // Iniciar rotación de esta puerta
        StartCoroutine(RotarPuerta());

        // Iniciar rotación de la puerta vinculada al mismo tiempo
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
