using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;       // Panel principal del menú
    [SerializeField] GameObject opcionesPanel;   // Panel de opciones compartido
    [SerializeField] GameObject pausaPanel;      // Panel de pausa (para asegurarnos que arranca oculto)

    void Start()
    {
        // Al iniciar, solo mostrar el menú principal
        mainPanel.SetActive(true);
        opcionesPanel.SetActive(false);
        if (pausaPanel != null) pausaPanel.SetActive(false);

        Time.timeScale = 0f; // detener el juego mientras está el menú principal
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Jugar()
    {
        mainPanel.SetActive(false);
        opcionesPanel.SetActive(false);

        Time.timeScale = 1f; // reanudar el juego
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Juego iniciado en la misma escena.");
    }

    public void AbrirOpciones()
    {
        mainPanel.SetActive(false);
        opcionesPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CerrarOpciones()
    {
        opcionesPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void SalirJuego()
    {
        Application.Quit();
        Debug.Log("Juego cerrado desde el menú principal.");
    }
}
