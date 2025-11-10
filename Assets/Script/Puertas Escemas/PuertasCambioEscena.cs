using UnityEngine;
using UnityEngine.SceneManagement;

public class PuertaCambioEscena : MonoBehaviour, IInteractuable
{
    [Header("Configuración de puerta")]
    [SerializeField] string nombreEscenaDestino; //Nombre de la escena

    public void Interactuar()
    {
        if (string.IsNullOrEmpty(nombreEscenaDestino))
        {
            Debug.LogWarning("No hay ruta de escena destinada");
            return;
        }

        Debug.Log("Se cambio de escena Correctamente");
        SceneManager.LoadScene(nombreEscenaDestino);
    }
}

