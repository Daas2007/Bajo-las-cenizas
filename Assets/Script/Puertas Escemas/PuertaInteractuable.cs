using UnityEngine;
using System.Collections;

public class PuertaInteractuable : MonoBehaviour, IInteractuable
{
    [Header("Configuración de apertura")]
    [SerializeField] private Transform engranaje;
    [SerializeField] private Vector3 rotacionInicialEuler; // editable en Inspector
    [SerializeField] private Vector3 rotacionFinalEuler;   // editable en Inspector
    [SerializeField] private float duracion = 1f;

    [Header("Opciones especiales")]
    [SerializeField] private bool cierreRapidoConCristal = false; // 👈 toggle en Inspector

    private bool abierta = false;
    private bool enMovimiento = false;
    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;

    private void Awake()
    {
        if (engranaje == null)
        {
            Debug.LogError("⚠️ No se asignó el engranaje/bisagra en el Inspector.");
            return;
        }

        rotacionInicial = Quaternion.Euler(rotacionInicialEuler);
        rotacionFinal = Quaternion.Euler(rotacionFinalEuler);
    }

    public void Interactuar()
    {
        if (engranaje == null || enMovimiento) return;

        StopAllCoroutines();

        if (!abierta)
            StartCoroutine(RotarPuerta(rotacionFinal, true, duracion));
        else
            StartCoroutine(RotarPuerta(rotacionInicial, false, duracion));
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

    // 👇 Método para cerrar rápido si la puerta tiene el bool activado
    public void CerrarSiCristal()
    {
        if (cierreRapidoConCristal && PlayerPrefs.GetInt("TieneCristal", 0) == 1)
        {
            StopAllCoroutines();
            StartCoroutine(RotarPuerta(rotacionInicial, false, 0.3f)); // cierre rápido
            Debug.Log("🚪 Puerta cerrada rápido por condición de cristal.");
        }
    }

    public bool EstaAbierta() => abierta;
}

