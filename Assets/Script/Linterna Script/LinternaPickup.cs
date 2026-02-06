using UnityEngine;

public class LinternaPickup : MonoBehaviour, IInteractuable
{
    public void Interactuar()
    {
        // Buscar el jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            JugadorLinterna jugadorLinterna = player.GetComponent<JugadorLinterna>();
            if (jugadorLinterna != null)
            {
                jugadorLinterna.DarLinternaEncendida(); // activa linterna en la mano con luz encendida
            }
        }

        // Destruir el objeto linterna del mapa
        Destroy(gameObject);
    }
}
