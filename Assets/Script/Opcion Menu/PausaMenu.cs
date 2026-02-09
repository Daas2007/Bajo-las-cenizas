using UnityEngine;

public class PausaMenu : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject opcionesPanel;
    [SerializeField] GameObject mainPanel; // referencia al panel del menú principal

    bool isPaused = false;

    void Awake()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (opcionesPanel != null) opcionesPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(false); // arranca oculto
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                Pause();
            }
            else
            {
                if (opcionesPanel.activeSelf)
                {
                    CerrarOpciones();
                }
                else
                {
                    Resume();
                }
            }
        }
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        opcionesPanel.SetActive(false);

        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AbrirOpciones()
    {
        pausePanel.SetActive(false);
        opcionesPanel.SetActive(true);
    }

    public void CerrarOpciones()
    {
        opcionesPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        opcionesPanel.SetActive(false);

        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void VolverAlMenu()
    {
        // Ocultar paneles de pausa y opciones
        pausePanel.SetActive(false);
        opcionesPanel.SetActive(false);

        // Mostrar el menú principal
        if (mainPanel != null) mainPanel.SetActive(true);

        // Pausar el juego
        Time.timeScale = 0f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Volviendo al menú principal.");
    }
}
