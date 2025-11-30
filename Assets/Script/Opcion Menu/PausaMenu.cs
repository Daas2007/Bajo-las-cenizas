using UnityEngine;
using UnityEngine.SceneManagement;

public class PausaMenu : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject opcionesPanel; // referencia al panel de opciones

    bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
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
        pausePanel.SetActive(true);
        opcionesPanel.SetActive(false);
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        opcionesPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false; 
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("Main Menu Start");
    }
}

