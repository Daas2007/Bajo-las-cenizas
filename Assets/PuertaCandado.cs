using UnityEngine;

public class PuertaCandado : MonoBehaviour, IInteractuable
{
    [SerializeField] private CandadoPuerta candadoDerecho;
    [SerializeField] private CandadoPuerta candadoIzquierdo;

    [Header("Configuración de apertura")]
    [SerializeField] private float anguloApertura = 90f;
    [SerializeField] private float velocidadRotacion = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoAbrir;

    private bool abierta = false;
    private bool bloqueada = true;
    private Quaternion rotInicial;
    private Quaternion rotObjetivo;

    void Start()
    {
        rotInicial = transform.rotation;
        rotObjetivo = rotInicial;

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    public void Interactuar()
    {
        if (bloqueada)
        {
            Debug.Log("🚪 La puerta está bloqueada por los candados.");
            return;
        }

        if (!abierta)
        {
            abierta = true;
            rotObjetivo = Quaternion.Euler(0, anguloApertura, 0) * rotInicial;
            ReproducirSonido(sonidoAbrir);
            Debug.Log("🚪 Puerta abierta.");
        }
    }

    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, rotObjetivo, Time.deltaTime * velocidadRotacion);
    }

    public void NotificarCandadoDestruido(CandadoPuerta candado)
    {
        if (candado == candadoDerecho) candadoDerecho = null;
        if (candado == candadoIzquierdo) candadoIzquierdo = null;

        RevisarCandados();
    }

    public void RevisarCandados()
    {
        if (candadoDerecho == null && candadoIzquierdo == null)
        {
            bloqueada = false;
            Debug.Log("🔓 Ambos candados destruidos, la puerta puede abrirse.");
        }
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
