using UnityEngine;
using UnityEngine.SceneManagement;

public class Bloqueadordenivel : MonoBehaviour
{
    [SerializeField] GameObject[] objetosBloqueables;

    void Start()
    {
        string nivel = SceneManager.GetActiveScene().name;
        bool completado = PlayerPrefs.GetInt(nivel + "_completado", 0) == 1;

        if (completado)
        {
            foreach (GameObject obj in objetosBloqueables)
            {
                if (obj != null) obj.SetActive(false);
            }

            Debug.Log("🔒 Nivel ya completado. Fragmentos y cristal bloqueados.");
        }
    }
}
