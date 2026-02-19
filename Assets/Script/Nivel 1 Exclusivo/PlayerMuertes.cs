using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMuertes : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemigoPerseguidor"))
        {
            // Avisar al GameManager
            GameManager.Instancia.RegistrarMuerte();

            // Reiniciar la escena
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.Log("No chocaste con el enemigo");
        }
    }
}

