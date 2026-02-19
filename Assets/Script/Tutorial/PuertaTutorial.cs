using UnityEngine;
using TMPro;
using System.Collections;

public class PuertaTutorial : MonoBehaviour, IInteractuable
{
    [Header("Configuración de apertura")]
    [SerializeField] float anguloApertura = 90f;
    [SerializeField] float velocidadRotacion = 2f;
    [SerializeField] Transform pivote;

    [Header("UI de diálogo")]
    [SerializeField] GameObject panelDialogo;
    [SerializeField] TMP_Text textoDialogo;

    [Header("Configuración del texto")]
    [SerializeField] float tiempoEntreLetras = 0.05f;
    [SerializeField] float tiempoVisibleDespues = 5f;

    [Header("Bloqueo de jugador")]
    [SerializeField] MonoBehaviour scriptMovimiento; // tu script de movimiento
    [SerializeField] Camara scriptCamara;     // tu script de cámara

    private bool abierta = false;
    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;

    private Coroutine rutinaTexto;
    private bool puedeActivar = true;
    private string mensajeActual = "";

    void Start()
    {
        if (pivote == null) pivote = transform;
        rotacionInicial = pivote.rotation;
        rotacionFinal = rotacionInicial * Quaternion.Euler(0f, anguloApertura, 0f);

        if (panelDialogo != null) panelDialogo.SetActive(false);
    }

    public void Interactuar()
    {
        JugadorLinterna jugadorLinterna = FindFirstObjectByType<JugadorLinterna>();
        if (jugadorLinterna != null && jugadorLinterna.TieneLinterna())
        {
            if (!abierta)
                StartCoroutine(AbrirPuerta());
        }
        else
        {
            if (puedeActivar && !panelDialogo.activeSelf)
                MostrarDialogo("Mmm... está bastante oscuro afuera, será mejor que busque algo para iluminar");
        }
    }

    private IEnumerator AbrirPuerta()
    {
        abierta = true;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * velocidadRotacion;
            pivote.rotation = Quaternion.Slerp(rotacionInicial, rotacionFinal, t);
            yield return null;
        }
    }

    private void MostrarDialogo(string mensaje)
    {
        if (panelDialogo == null || textoDialogo == null) return;

        mensajeActual = mensaje;
        panelDialogo.SetActive(true);

        // Bloquear jugador y cámara
        if (scriptMovimiento != null) scriptMovimiento.enabled = false;
        if (scriptCamara != null) scriptCamara.enabled = false;

        if (rutinaTexto != null) StopCoroutine(rutinaTexto);
        rutinaTexto = StartCoroutine(EscribirLinea(mensaje));
    }

    private IEnumerator EscribirLinea(string linea)
    {
        textoDialogo.text = "";
        foreach (char letra in linea)
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(tiempoEntreLetras);
        }

        // Cuando termina de escribir → esperar 5 segundos y cerrar
        yield return new WaitForSeconds(tiempoVisibleDespues);
        CerrarDialogo();
    }

    private void CerrarDialogo()
    {
        if (rutinaTexto != null) StopCoroutine(rutinaTexto);
        panelDialogo.SetActive(false);
        textoDialogo.text = "";

        // Desbloquear jugador y cámara
        if (scriptMovimiento != null) scriptMovimiento.enabled = true;
        if (scriptCamara != null) scriptCamara.enabled = true;

        // Cooldown antes de poder reactivar
        puedeActivar = false;
        Invoke(nameof(ReactivarDialogo), 2f);
    }

    private void ReactivarDialogo()
    {
        puedeActivar = true;
    }
}


