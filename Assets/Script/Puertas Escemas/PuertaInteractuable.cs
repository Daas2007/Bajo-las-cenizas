using UnityEngine;
using System.Collections;

public class PuertaInteractuable : MonoBehaviour, IInteractuable
{
    [Header("Configuración de apertura")]
    [SerializeField] private Transform engranaje;   // Empty en la bisagra
    [SerializeField] private float anguloApertura = 90f; // Ángulo final en Y
    [SerializeField] private float duracion = 1f;        // Tiempo de animación

    private bool abierta = false;
    private bool enMovimiento = false; // 🔑 evita interacción doble
    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;

    private void Awake()
    {
        if (engranaje == null)
        {
            Debug.LogError("⚠️ No se asignó el engranaje/bisagra en el Inspector.");
            return;
        }

        // Guardar rotación inicial tal cual está en la escena
        rotacionInicial = engranaje.localRotation;

        // Calcular rotación final relativa
        rotacionFinal = rotacionInicial * Quaternion.Euler(0, anguloApertura, 0);
    }

    public void Interactuar()
    {
        if (engranaje == null || enMovimiento) return;

        StopAllCoroutines();

        if (!abierta)
            StartCoroutine(RotarPuerta(rotacionFinal, true));
        else
            StartCoroutine(RotarPuerta(rotacionInicial, false));
    }

    private IEnumerator RotarPuerta(Quaternion destino, bool abrir)
    {
        enMovimiento = true;
        float tiempo = 0f;
        Quaternion inicio = engranaje.localRotation;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;
            engranaje.localRotation = Quaternion.Lerp(inicio, destino, t);
            tiempo += Time.deltaTime;
            yield return null;
        }

        engranaje.localRotation = destino;
        abierta = abrir;
        enMovimiento = false;

        Debug.Log(abrir ? "🚪 Puerta abierta." : "🚪 Puerta cerrada.");
    }

    public bool EstaAbierta() => abierta;
}

