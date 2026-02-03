using UnityEngine;

public class PuertaInteractuable : MonoBehaviour, IInteractuable
{
    [Header("Configuración de apertura")]
    [SerializeField] private Transform engranaje;   // Empty en la bisagra
    [SerializeField] private float anguloApertura = 90f; // Ángulo final en Y
    [SerializeField] private float duracion = 1f;        // Tiempo de animación

    private bool abierta = false;
    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;

    private void Start()
    {
        if (engranaje == null)
        {
            Debug.LogError("⚠️ No se asignó el engranaje/bisagra en el Inspector.");
            return;
        }

        rotacionInicial = engranaje.rotation;
        rotacionFinal = Quaternion.Euler(0, anguloApertura, 0) * rotacionInicial;
    }

    public void Interactuar()
    {
        if (engranaje == null) return;

        StopAllCoroutines();

        if (!abierta)
            StartCoroutine(RotarPuerta(rotacionFinal, true));
        else
            StartCoroutine(RotarPuerta(rotacionInicial, false));
    }

    private System.Collections.IEnumerator RotarPuerta(Quaternion destino, bool abrir)
    {
        float tiempo = 0f;
        Quaternion inicio = engranaje.rotation;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;
            engranaje.rotation = Quaternion.Lerp(inicio, destino, t);
            tiempo += Time.deltaTime;
            yield return null;
        }

        engranaje.rotation = destino;
        abierta = abrir;
        Debug.Log(abrir ? "🚪 Puerta abierta." : "🚪 Puerta cerrada.");
    }

    // 🔑 Método añadido para que otros scripts (como DoorLock) puedan consultar el estado
    public bool EstaAbierta() => abierta;
}
