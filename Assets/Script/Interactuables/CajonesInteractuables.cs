using UnityEngine;
using System.Collections;

public class CajonesInteractuables : MonoBehaviour, IInteractuable
{
    [Header("Configuración del cajón")]
    [SerializeField] private Transform cajon;          // referencia al cajón
    [SerializeField] private Vector3 direccionMovimiento = Vector3.forward; // dirección del movimiento
    [SerializeField] private float distancia = 0.5f;   // cuánto se moverá
    [SerializeField] private float duracion = 1f;      // tiempo de animación

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;  // ✅ referencia asignada desde el Inspector
    [SerializeField] private AudioClip sonidoAbrir;    // clip de abrir
    [SerializeField] private AudioClip sonidoCerrar;   // clip de cerrar

    private Vector3 posicionInicial;
    private Vector3 posicionFinal;
    private bool abierto = false;
    private Coroutine rutinaMovimiento;

    void Start()
    {
        if (cajon == null)
        {
            Debug.LogError("⚠️ No se asignó el cajón en el Inspector.");
            return;
        }

        // Guardar posición inicial y calcular final
        posicionInicial = cajon.localPosition;
        posicionFinal = posicionInicial + direccionMovimiento.normalized * distancia;

        // ✅ si no asignaste un AudioSource en el Inspector, lo crea automáticamente
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    public void Interactuar()
    {
        if (cajon == null) return;

        if (rutinaMovimiento != null) StopCoroutine(rutinaMovimiento);

        if (!abierto)
        {
            ReproducirSonido(sonidoAbrir);
            rutinaMovimiento = StartCoroutine(MoverCajon(posicionFinal, true));
        }
        else
        {
            ReproducirSonido(sonidoCerrar);
            rutinaMovimiento = StartCoroutine(MoverCajon(posicionInicial, false));
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
