using UnityEngine;

public class PausaMenu : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject opcionesPanel;
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject dialogoCanvas;

    [Header("UI adicional")]
    [SerializeField] GameObject PanleEstamina;
    [SerializeField] GameObject TextoInteracion;

    bool isPaused = false;
    bool dialogoActivo = false;

    public bool IsPaused => isPaused; // propiedad pública para que InteraccionJugador lo consulte

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
        if (dialogoCanvas != null && dialogoCanvas.activeSelf)
        {
            dialogoCanvas.SetActive(false);
            dialogoActivo = true;
        }

        pausePanel.SetActive(true);
        opcionesPanel.SetActive(false);

        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AbrirOpciones()
    {
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

        if (dialogoActivo && dialogoCanvas != null)
        {
            dialogoCanvas.SetActive(true);
            dialogoActivo = false;
        }
    }

    public void VolverAlMenu()
    {
        pausePanel.SetActive(false);
        opcionesPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);

        // Apagar UI adicional
        if (PanleEstamina != null) PanleEstamina.SetActive(false);
        if (TextoInteracion != null) TextoInteracion.SetActive(false);

        Time.timeScale = 0f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Volviendo al menú principal.");
    }
}
