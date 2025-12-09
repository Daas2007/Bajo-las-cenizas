using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public void EntrarHabitacion1()
    {
        IntentarEntrarANivel("Habitacion 1");
    }

    public void EntrarHabitacion2()
    {
        IntentarEntrarANivel("Habitacion 2");
    }

    public void EntrarHabitacion3()
    {
        IntentarEntrarANivel("Habitacion 3");
    }

    public void EntrarHabitacion4()
    {
        IntentarEntrarANivel("Habitacion 4");
    }

    void IntentarEntrarANivel(string nombreNivel)
    {
        if (!GameManager.Instancia.EstaNivelCompletado(nombreNivel))
        {
            SceneManager.LoadScene(nombreNivel);
        }
        else
        {
            Debug.Log($"🚫 No puedes entrar a {nombreNivel}, ya fue completado.");
            // Aquí puedes desactivar el botón o mostrar un mensaje en UI
        }
    }
}

