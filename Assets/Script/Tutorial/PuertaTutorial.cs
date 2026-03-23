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

    [Header("Audio")]
    [SerializeField] AudioClip sonidoAbrir;   // ✅ tu clip de sonido
    [SerializeField] private AudioSource audioSource;          // ✅ referencia al AudioSource de la puerta

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

        // ✅ usar el AudioSource ya existente en la puerta
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (panelDialogo != null && panelDialogo.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (escribiendo)
            {
                if (rutinaTexto != null) StopCoroutine(rutinaTexto);
                textoDialogo.text = mensajeActual;
                escribiendo = false;
            }
            else
            {
                CerrarDialogo();
            }
        }
    }

    public void Interactuar()
    {
        TutorialInteractivo tutorial = FindObjectOfType<TutorialInteractivo>();
        if (tutorial != null && tutorial.EstaActivo())
        {
            MostrarDialogo("Aun no puedo salir, me falta hacer algo");
            return;
        }

        GameManager gm = GameManager.Instancia;
        if (gm != null && gm.tieneLinterna)
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

    public void ResetPuerta()
    {
        abierta = false;
        pivote.rotation = rotacionInicial;
        gameObject.layer = LayerMask.NameToLayer("Interaccion");
    }

    private void AbrirPuertasSimultaneas()
    {
        abierta = true;
        gameObject.layer = LayerMask.NameToLayer("Default");

        // ✅ reproducir sonido al abrir
        if (sonidoAbrir != null && audioSource != null)
            audioSource.PlayOneShot(sonidoAbrir);

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

        // ✅ reproducir sonido también en la puerta vinculada
        if (sonidoAbrir != null && audioSource != null)
            audioSource.PlayOneShot(sonidoAbrir);

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
