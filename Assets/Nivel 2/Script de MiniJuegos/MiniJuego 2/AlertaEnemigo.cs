
using System.Collections;
using UnityEngine;

public class AlertaEnemigo : MonoBehaviour
{
    [Header("Alertas visuales")]
    [SerializeField] private GameObject AlertaDerechaEnemigo;
    [SerializeField] private GameObject AlertaIzquierdaEnemigo;

    [Header("Audio")]
    [SerializeField] private AudioClip AudioAlertaClip;
    [SerializeField] private AudioSource Audio;

    [Header("Aparición")]
    [SerializeField]
    private ControladorAparicionEnemigo controladorEnemigo;

    [SerializeField, Range(0f, 1f)]
    private float probabilidadAlerta = 0.55f;

    [SerializeField] private float duracionAlerta = 3f;

    private bool alertaActiva;

    private void Awake()
    {
        if (AlertaDerechaEnemigo != null)
            AlertaDerechaEnemigo.SetActive(false);

        if (AlertaIzquierdaEnemigo != null)
            AlertaIzquierdaEnemigo.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || alertaActiva)
            return;

        if (Random.value > probabilidadAlerta)
            return;

        if (AudioAlertaClip == null || Audio == null
            || controladorEnemigo == null)
        {
            Debug.LogWarning(
                "Asigna el audio y el controlador de aparición.", this
            );
            return;
        }

        StartCoroutine(TiempoAlerta());
    }

    private IEnumerator TiempoAlerta()
    {
        alertaActiva = true;

        // Elegir por qué lado aparecerá el enemigo.
        bool izquierda = Random.Range(0, 2) == 0;

        if (izquierda)
        {
            if (AlertaIzquierdaEnemigo != null)
                AlertaIzquierdaEnemigo.SetActive(true);

            Audio.panStereo = -1f;

            controladorEnemigo.AparecerDesdeIzquierda();
        }
        else
        {
            if (AlertaDerechaEnemigo != null)
                AlertaDerechaEnemigo.SetActive(true);

            Audio.panStereo = 1f;

            controladorEnemigo.AparecerDesdeDerecha();
        }

        Audio.PlayOneShot(AudioAlertaClip);

        yield return new WaitForSeconds(duracionAlerta);

        if (AlertaIzquierdaEnemigo != null)
            AlertaIzquierdaEnemigo.SetActive(false);

        if (AlertaDerechaEnemigo != null)
            AlertaDerechaEnemigo.SetActive(false);

        Audio.panStereo = 0f;
        alertaActiva = false;
    }
}