using UnityEngine;

public class PuertaCandado : MonoBehaviour, IInteractuable
{
    [SerializeField] private CandadoPuerta candadoDerecho;
    [SerializeField] private CandadoPuerta candadoIzquierdo;

    [Header("Configuración de apertura")]
    [SerializeField] private float anguloApertura = 90f;
    [SerializeField] private float velocidadRotacion = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;   // ✅ referencia al AudioSource
    [SerializeField] private AudioClip sonidoAbrir;     // ✅ clip de abrir

    private bool abierta = false;
    private bool bloqueada = true;
    private Quaternion rotInicial;
    private Quaternion rotObjetivo;

    void Start()
    {
        rotInicial = transform.rotation;
        rotObjetivo = rotInicial;

        // ✅ si no asignaste un AudioSource en el Inspector, se crea automáticamente
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

            // ✅ reproducir sonido de abrir
            ReproducirSonido(sonidoAbrir);
        }
    }

    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, rotObjetivo, Time.deltaTime * velocidadRotacion);
    }

    public void RevisarCandados()
    {
        // ✅ La puerta se desbloquea solo si ambos candados fueron destruidos
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
