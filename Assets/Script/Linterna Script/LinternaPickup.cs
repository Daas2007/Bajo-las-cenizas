using UnityEngine;

public class LinternaPickup : MonoBehaviour, IInteractuable
{
    [SerializeField] GameObject MuroBloqueoInicio;
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
                MuroBloqueoInicio.transform.position = new Vector3(MuroBloqueoInicio.transform.position.x, MuroBloqueoInicio.transform.position.y + 3f, MuroBloqueoInicio.transform.position.z);
                //MuroBloqueoInicio.SetActive(false);
            }
        }

        // Destruir el objeto linterna del mapa
        Destroy(gameObject);
    }
}
