using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;       // Panel principal del menú
    [SerializeField] GameObject opcionesPanel;   // Panel de opciones compartido
    [SerializeField] GameObject pausaPanel;      // Panel de pausa

    void Start()
    {
        // Al iniciar, mostrar solo el menú principal
        mainPanel.SetActive(true);
        if (opcionesPanel != null) opcionesPanel.SetActive(false);
        if (pausaPanel != null) pausaPanel.SetActive(false);

        Time.timeScale = 0f; // detener el juego hasta que se presione Jugar
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Jugar()
    {
        mainPanel.SetActive(false);
        if (opcionesPanel != null) opcionesPanel.SetActive(false);

        Time.timeScale = 1f; // reanudar el juego
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Juego iniciado en la misma escena.");
    }

    public void AbrirOpciones()
    {
        opcionesPanel.SetActive(true); // se activa encima del MainMenu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CerrarOpciones()
    {
        opcionesPanel.SetActive(false); // simplemente se oculta
        mainPanel.SetActive(true);      // vuelve a mostrarse el MainMenu
    }

    public void SalirJuego()
    {
        Application.Quit();
        Debug.Log("Juego cerrado desde el menú principal.");
    }
}
