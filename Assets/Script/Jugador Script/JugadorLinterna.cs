using UnityEngine;

public class JugadorLinterna : MonoBehaviour
{
    public GameObject linternaObjeto; // referencia al objeto linterna (luz, modelo, etc.)
    private bool tieneLinterna = false;

    void Start()
    {
        // Revisar si ya estaba guardado que el jugador tiene linterna
        if (PlayerPrefs.GetInt("TieneLinterna", 0) == 1)
        {
            ActivarLinterna();
        }
        else
        {
            linternaObjeto.SetActive(false); // no disponible al inicio
        }
    }

    public void ActivarLinterna()
    {
        tieneLinterna = true;
        linternaObjeto.SetActive(false);
    }

    void Update()
    {
        if (tieneLinterna && Input.GetKeyDown(KeyCode.F))
        {
            // Encender/apagar linterna
            linternaObjeto.SetActive(!linternaObjeto.activeSelf);
        }
    }
}
