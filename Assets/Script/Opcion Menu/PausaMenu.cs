using UnityEngine;

public class PausaMenu : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;      // Panel de pausa
    [SerializeField] GameObject opcionesPanel;   // Panel de opciones compartido
    [SerializeField] GameObject mainPanel;       // Panel del menú principal

    bool isPaused = false;

    void Awake()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (opcionesPanel != null) opcionesPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale == 1f)
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
        opcionesPanel.SetActive(true); // se activa encima del PausaPanel
    }

    public void CerrarOpciones()
    {
        opcionesPanel.SetActive(false); // simplemente se oculta
        pausePanel.SetActive(true);     // vuelve a mostrarse el PausaPanel
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
        pausePanel.SetActive(false);
        opcionesPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);

        Time.timeScale = 0f; // volver a pausar el juego
        isPaused = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Volviendo al menú principal.");
    }
}
