using UnityEngine;

public class GameManagerNivel1 : MonoBehaviour
{
    public int muertesVentana = 0;

    public void RegistrarMuerteVentana()
    {
        muertesVentana++;
        Debug.Log("Muertes contra enemigo ventana: " + muertesVentana);
    }
}
