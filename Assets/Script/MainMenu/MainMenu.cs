using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;       // Panel principal del menú
    [SerializeField] GameObject opcionesPanel;   // Panel de opciones
    [SerializeField] GameObject pausaPanel;      // Panel de pausa

    private void Awake()
    {
        // Al iniciar, solo mostrar el menú principal
        if (mainPanel != null) mainPanel.SetActive(true);
        if (opcionesPanel != null) opcionesPanel.SetActive(false);
        if (pausaPanel != null) pausaPanel.SetActive(false);

        // Cursor desbloqueado para poder usar botones
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pausar el juego hasta que se presione Jugar
        Time.timeScale = 0f;
    }

    public void Jugar()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (opcionesPanel != null) opcionesPanel.SetActive(false);

        // Reanudar el juego
        Time.timeScale = 1f;

        // Bloquear cursor para gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Juego iniciado en la misma escena.");
    }

    public void AbrirOpciones()
    {
        if (opcionesPanel != null) opcionesPanel.SetActive(true);

        // Cursor visible para interactuar con opciones
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CerrarOpciones()
    {
        if (opcionesPanel != null) opcionesPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);

        // Cursor visible porque seguimos en menú
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SalirJuego()
    {
        Application.Quit();
        Debug.Log("Juego cerrado desde el menú principal.");
    }
}
