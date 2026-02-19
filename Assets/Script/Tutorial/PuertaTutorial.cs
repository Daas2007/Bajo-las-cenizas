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
    [SerializeField] GameObject panelDialogo;   // 👈 arrastra tu panel del Canvas
    [SerializeField] TMP_Text textoDialogo;     // 👈 arrastra tu TextMeshPro vacío

    [Header("Configuración del texto")]
    [SerializeField] float tiempoEntreLetras = 0.05f;
    [SerializeField] float cooldownReactivar = 3f;

    private bool abierta = false;
    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;

    private Coroutine rutinaTexto;
    private bool escribiendo = false;
    private bool puedeActivar = true;

    private void Awake()
    {
        panelDialogo.SetActive(false);
    }
    void Start()
    {
        if (pivote == null) pivote = transform;
        rotacionInicial = pivote.rotation;
        rotacionFinal = rotacionInicial * Quaternion.Euler(0f, anguloApertura, 0f);

        if (panelDialogo != null) panelDialogo.SetActive(false);
    }

    void Update()
    {
        // Si el panel está activo y presionas E → cerrar
        if (panelDialogo.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            CerrarDialogo();
        }
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
            if (puedeActivar)
                MostrarDialogo("Antes necesito algo para ver a donde me dirijo...");
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

        panelDialogo.SetActive(true);

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
            yield return new WaitForSeconds(tiempoEntreLetras);
        }
        escribiendo = false;
    }

    private void CerrarDialogo()
    {
        if (rutinaTexto != null) StopCoroutine(rutinaTexto);
        panelDialogo.SetActive(false);
        textoDialogo.text = "";
        puedeActivar = false;
        Invoke(nameof(ReactivarDialogo), cooldownReactivar);
    }

    private void ReactivarDialogo()
    {
        puedeActivar = true;
    }
}

