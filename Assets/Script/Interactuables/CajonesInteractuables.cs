using UnityEngine;
using System.Collections;

public class CajonesInteractuables : MonoBehaviour, IInteractuable
{
    [Header("Configuración del cajón")]
    [SerializeField] private Transform cajon;          // referencia al cajón
    [SerializeField] private Transform pivoteInicial;  // posición inicial
    [SerializeField] private Transform pivoteFinal;    // posición final
    [SerializeField] private float duracion = 1f;      // tiempo de animación

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoAbrir;
    [SerializeField] private AudioClip sonidoCerrar;

    private bool abierto = false; // estado actual
    private Coroutine rutinaMovimiento;

    void Start()
    {
        if (cajon == null || pivoteInicial == null || pivoteFinal == null)
        {
            Debug.LogError("⚠️ Faltan referencias en el Inspector (cajón o pivotes).");
            return;
        }

        // Colocar cajón en la posición inicial al empezar
        cajon.localPosition = pivoteInicial.localPosition;

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    public void Interactuar()
    {
        if (cajon == null || pivoteInicial == null || pivoteFinal == null) return;

        if (rutinaMovimiento != null) StopCoroutine(rutinaMovimiento);

        if (!abierto)
        {
            ReproducirSonido(sonidoAbrir);
            rutinaMovimiento = StartCoroutine(MoverCajon(pivoteFinal.localPosition, true));
        }
        else
        {
            ReproducirSonido(sonidoCerrar);
            rutinaMovimiento = StartCoroutine(MoverCajon(pivoteInicial.localPosition, false));
        }
    }

    private IEnumerator MoverCajon(Vector3 destino, bool abrir)
    {
        float tiempo = 0f;
        Vector3 inicio = cajon.localPosition;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;
            cajon.localPosition = Vector3.Lerp(inicio, destino, t);
            tiempo += Time.deltaTime;
            yield return null;
        }

        cajon.localPosition = destino;
        abierto = abrir;
        rutinaMovimiento = null;
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
