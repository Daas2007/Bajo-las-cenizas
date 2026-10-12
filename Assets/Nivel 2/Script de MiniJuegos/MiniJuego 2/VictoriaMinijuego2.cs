
using UnityEngine;

public class VictoriaMinijuego2 : MonoBehaviour
{
    [Header("Configuración de Victoria")]
    [SerializeField] private bool cerrarAlEntrarEnTrigger = true;
    [SerializeField] private string tagJugador = "Player";

    [Header("Referencia al activador")]
    [SerializeField] private ActivadorDeMinijuegos activador;

    private bool victoriaActivada;



    public void GanarMinijuego()
    {
        if (victoriaActivada)
            return;

        victoriaActivada = true;

        Debug.Log("¡Victoria detectada!");

        // Buscar el activador en las escenas cargadas.
        ActivadorDeMinijuegos activador =
            FindFirstObjectByType<ActivadorDeMinijuegos>();

        if (activador != null)
        {
            activador.CerrarMinijuego();
        }
        else
        {
            Debug.LogWarning(
                "No se encontró ActivadorDeMinijuegos."
            );
        }

        // Después, finalizar el minijuego.
        if (MiniJuego2.Instance != null)
        {
            MiniJuego2.Instance.FinalizarMiniJuego();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cerrarAlEntrarEnTrigger &&
            other.CompareTag(tagJugador))
        {
            GanarMinijuego();
        }
    }
}