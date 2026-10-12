
using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectorReinicio : MonoBehaviour
{
    [SerializeField] private string etiquetaObjetivo = "Modelo";

    private bool reiniciando;

    private void OnTriggerEnter(Collider other)
    {
        if (reiniciando)
            return;

        if (other.CompareTag(etiquetaObjetivo))
        {
            reiniciando = true;

            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}
