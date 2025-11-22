using UnityEngine;

public class TriggerDialogo : MonoBehaviour
{
    [SerializeField] ControladorDialogo controlador;   // Referencia al controlador de diálogos
    [SerializeField] NuevoDialogo dialogoInicial;      // El diálogo que quieres mostrar

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate que tu personaje tenga el tag "Player"
        {
            controlador.MostrarDialogo(dialogoInicial);
        }
    }
}
