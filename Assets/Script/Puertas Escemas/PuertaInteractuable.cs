using UnityEngine;
using System.Collections;

public class PuertaInteractuable : MonoBehaviour, IInteractuable
{
    [Header("Configuración de apertura")]
    [SerializeField] private Transform engranaje;
    [SerializeField] private Vector3 rotacionInicialEuler;
    [SerializeField] private Vector3 rotacionFinalEuler;
    [SerializeField] private float duracion = 1f;

    [Header("Opciones especiales")]
    [SerializeField] private bool cierreRapidoConCristal = false;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoAbrir;
    [SerializeField] private AudioClip sonidoCerrar;

    private bool abierta = false;
    private bool enMovimiento = false;
    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;

    private void Awake()
    {
        if (engranaje == null)
        {
            Debug.LogError($"⚠️ [PuertaInteractuable] El objeto '{gameObject.name}' no tiene engranaje asignado.");
            return;
        }

        rotacionInicial = Quaternion.Euler(rotacionInicialEuler);
        rotacionFinal = Quaternion.Euler(rotacionFinalEuler);

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    public void Interactuar()
    {
        if (engranaje == null) return;
        if (enMovimiento) return;

        StopAllCoroutines();

        if (!abierta)
        {
            ReproducirSonido(sonidoAbrir);
            StartCoroutine(RotarPuerta(rotacionFinal, true, duracion));
        }
        else
        {
            StartCoroutine(RotarPuerta(rotacionInicial, false, duracion));
        }
    }

    private IEnumerator RotarPuerta(Quaternion destino, bool abrir, float tiempoAnim)
    {
        enMovimiento = true;
        float tiempo = 0f;
        Quaternion inicio = engranaje.localRotation;
        bool sonidoCerrarReproducido = false;

        while (tiempo < tiempoAnim)
        {
            float t = tiempo / tiempoAnim;
            engranaje.localRotation = Quaternion.Lerp(inicio, destino, t);

            if (!abrir && !sonidoCerrarReproducido)
            {
                float anguloRestante = Quaternion.Angle(engranaje.localRotation, destino);
                if (anguloRestante <= 10f)
                {
                    ReproducirSonido(sonidoCerrar);
                    sonidoCerrarReproducido = true;
                }
            }

            tiempo += Time.deltaTime;
            yield return null;
        }

        engranaje.localRotation = destino;
        abierta = abrir;
        enMovimiento = false;
    }

    public void CerrarSiCristal()
    {
        if (!cierreRapidoConCristal) return;

        if (PlayerPrefs.GetInt("TieneCristal", 0) == 1)
        {
            StopAllCoroutines();
            StartCoroutine(RotarPuerta(rotacionInicial, false, 0.3f));
        }
    }

    public bool EstaAbierta() => abierta;

    private void ReproducirSonido(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    // 🔹 Nuevo método público para forzar el engranaje a 0,0,0
    public void ForzarCerrarEngranaje()
    {
        StopAllCoroutines();
        if (engranaje != null)
        {
            engranaje.localEulerAngles = Vector3.zero;
            abierta = false;
            enMovimiento = false;
            Debug.Log("⚙️ Engranaje forzado a rotación inicial (0,0,0).");
        }
    }
}
