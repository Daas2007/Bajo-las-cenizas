using UnityEngine;

public class VictoriaMinijuego2 : MonoBehaviour
{
    [Header("Configuración de Victoria")]
    [SerializeField] private bool cerrarAlEntrarEnTrigger = true;
    [SerializeField] private string tagJugador = "Player";

    /// <summary>
    /// Ejecuta la secuencia de victoria y ordena el cierre de la escena aditiva.
    /// </summary>
    public void GanarMinijuego()
    {
        Debug.Log("🏆 ¡Victoria detectada en el Minijuego 2!");

        // Llama a la instancia principal para ejecutar la descarga asíncrona de la escena
        if (MiniJuego2.Instance != null)
        {
            MiniJuego2.Instance.FinalizarMiniJuego();
        }
        else
        {
            Debug.LogError("❌ No se encontró la instancia de MiniJuego2. Asegúrate de cargar la escena principal primero.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cerrarAlEntrarEnTrigger && other.CompareTag(tagJugador))
        {
            GanarMinijuego();
        }
    }
}