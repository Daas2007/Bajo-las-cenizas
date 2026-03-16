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
    }

    public void Interactuar()
    {
        if (engranaje == null)
        {
            Debug.LogError($"❌ [PuertaInteractuable] '{gameObject.name}' no puede interactuar: engranaje nulo.");
            return;
        }

        if (enMovimiento)
        {
            Debug.Log($"⏳ [PuertaInteractuable] '{gameObject.name}' está en movimiento, ignorando interacción.");
            return;
        }

        StopAllCoroutines();

        if (!abierta)
        {
            Debug.Log($"🔓 [PuertaInteractuable] '{gameObject.name}' abriendo puerta.");
            StartCoroutine(RotarPuerta(rotacionFinal, true, duracion));
        }
        else
        {
            Debug.Log($"🔒 [PuertaInteractuable] '{gameObject.name}' cerrando puerta.");
            StartCoroutine(RotarPuerta(rotacionInicial, false, duracion));
        }
    }

    private IEnumerator RotarPuerta(Quaternion destino, bool abrir, float tiempoAnim)
    {
        enMovimiento = true;
        float tiempo = 0f;
        Quaternion inicio = engranaje.localRotation;

        while (tiempo < tiempoAnim)
        {
            float t = tiempo / tiempoAnim;
            engranaje.localRotation = Quaternion.Lerp(inicio, destino, t);
            tiempo += Time.deltaTime;
            yield return null;
        }

        engranaje.localRotation = destino;
        abierta = abrir;
        enMovimiento = false;
    }

    public void CerrarSiCristal()
    {
        if (!cierreRapidoConCristal)
        {
            Debug.Log($"🛑 [PuertaInteractuable] '{gameObject.name}' no tiene cierre rápido activado.");
            return;
        }

        if (PlayerPrefs.GetInt("TieneCristal", 0) == 1)
        {
            StopAllCoroutines();
            StartCoroutine(RotarPuerta(rotacionInicial, false, 0.3f));
            Debug.Log($"🚪 [PuertaInteractuable] '{gameObject.name}' cerrada rápidamente por condición de cristal.");
        }
        else
        {
            Debug.Log($"🔍 [PuertaInteractuable] '{gameObject.name}' no se cierra: el jugador no tiene cristal.");
        }
    }

    public bool EstaAbierta() => abierta;
}
