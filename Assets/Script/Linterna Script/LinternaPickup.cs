using UnityEngine;

public class LinternaPickup : MonoBehaviour, IInteractuable
{
    [SerializeField] private PuertaBloqueada puertaBloqueada;

    public void Interactuar()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            JugadorLinterna jugadorLinterna = player.GetComponent<JugadorLinterna>();
            if (jugadorLinterna != null)
            {
                jugadorLinterna.DarLinternaEncendida(); // activa linterna en la mano con luz encendida
            }
        }

        // 🔑 Desbloquear la puerta
        if (puertaBloqueada != null)
        {
            puertaBloqueada.DesbloquearPuerta();
            Debug.Log("🔓 Puerta desbloqueada al recoger la linterna.");
        }

        // Destruir el objeto linterna del mapa
        Destroy(gameObject);
    }
}
