using UnityEngine;
using UnityEngine.SceneManagement;

public class PausaMenu : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject opcionesPanel; // referencia al panel de opciones

    bool isPaused = false;

    private void Awake()
    {
        pausePanel.SetActive(false);
        opcionesPanel.SetActive(false);
    }

    void Start()
    {
        Time.timeScale = 1f; // siempre arrancar con tiempo normal
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
                // Si estoy en opciones, volver al panel de pausa
                if (opcionesPanel.activeSelf)
                {
                    opcionesPanel.SetActive(false);
                    pausePanel.SetActive(true);
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
        if (pausePanel == null)
        {
            Debug.LogWarning("⚠️ pausePanel no está asignado.");
            return;
        }

        pausePanel.SetActive(true);
        opcionesPanel?.SetActive(false); // el ? evita error si es null
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
        isPaused = false; // 🔑 ahora sí se actualiza

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("Main Menu Start");
    }
}