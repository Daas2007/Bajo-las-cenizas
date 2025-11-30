using UnityEngine;

public class LinternaPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Guardar que el jugador ya tiene la linterna
            PlayerPrefs.SetInt("TieneLinterna", 1);
            PlayerPrefs.Save();

            // Activar la linterna en el jugador
            other.GetComponent<JugadorLinterna>().ActivarLinterna();

            // Destruir el objeto linterna en el mundo
            Destroy(gameObject);
        }
    }
}
