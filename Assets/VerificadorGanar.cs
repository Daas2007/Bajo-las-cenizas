using UnityEngine;
using UnityEngine.SceneManagement;

public class VerificadorGanar : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MovimientoPersonaje MP = other.GetComponent<MovimientoPersonaje>();
            if (MP != null && MP.TieneCristal())
            {
                Debug.Log("✅ Jugador con cristal pasó por el trigger. Cargando MainMenu...");
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                Debug.Log("⚠️ Jugador pasó por el trigger pero no tiene el cristal.");
            }
        }
    }
}
