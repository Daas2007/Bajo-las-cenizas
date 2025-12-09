using UnityEngine;
using UnityEngine.SceneManagement;

public class PuertaCambioEscena : MonoBehaviour, IInteractuable
{
    [Header("Configuración de puerta")]
    [SerializeField] string nombreEscenaDestino; // Nombre de la escena destino
    [SerializeField] string nombreEscenaLobby; // nombre exacto de tu escena de lobby

    public void Interactuar()
    {
        if (string.IsNullOrEmpty(nombreEscenaDestino))
        {
            Debug.LogWarning("⚠️ No hay ruta de escena destinada");
            return;
        }

        string escenaActual = SceneManager.GetActiveScene().name;

        // ✅ Si estoy en el lobby, siempre me deja entrar
        if (escenaActual == nombreEscenaLobby)
        {
            Debug.Log($"🚪 Desde el lobby entrando a {nombreEscenaDestino}...");
            SceneManager.LoadScene(nombreEscenaDestino);
            return;
        }

        // 🔒 Si NO estoy en el lobby, bloqueo salida si el nivel actual NO está completado
        if (GameManager.Instancia != null && !GameManager.Instancia.nivelCompletado)
        {
            Debug.Log("🚫 No puedes salir todavía, faltan fragmentos.");
            return;
        }

        // 🔑 Opcional: bloquear entrada si el nivel destino ya fue completado
        if (GameManager.Instancia != null && GameManager.Instancia.EstaNivelCompletado(nombreEscenaDestino))
        {
            Debug.Log($"🚫 No puedes entrar a {nombreEscenaDestino}, ya fue completado.");
            return;
        }

        Debug.Log($"🚪 Entrando a {nombreEscenaDestino}...");
        SceneManager.LoadScene(nombreEscenaDestino);
    }
}
