using UnityEngine;
using UnityEngine.SceneManagement;

public class CristalMeta : MonoBehaviour, IInteractuable
{
    [SerializeField] string escenaHabitacionInicial = "HabitacionInicial";

    public void Interactuar()
    {
        string nivelActual = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetInt(nivelActual + "_completado", 1);
        PlayerPrefs.Save();

        GameManager.Instancia.ResetearProgresoNivel();

        SceneManager.LoadScene(escenaHabitacionInicial);
    }
}


