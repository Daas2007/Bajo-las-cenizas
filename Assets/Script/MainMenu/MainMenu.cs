using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Paneles UI")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject menuOpciones;
    [SerializeField] private GameObject pauseMenu;

    [Header("Gameplay Objects")]
    [SerializeField] private GameObject jugador;
    [SerializeField] private GameObject enemigos; // puedes usar un empty con todos los enemigos dentro

    private bool isPaused = false;

    private void Awake()
    {
        // Al inicio: solo menú principal
        mainMenu.SetActive(true);
        menuOpciones.SetActive(false);
        pauseMenu.SetActive(false);

        jugador.SetActive(false);
        enemigos.SetActive(false);
    }

    // --- Menú Principal ---
    public void AbrirPanelDeOpciones()
    {
        mainMenu.SetActive(false);
        menuOpciones.SetActive(true);
    }

    public void AbrirPanelDeMenu()
    {
        mainMenu.SetActive(true);
        menuOpciones.SetActive(false);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }

    public void IniciarJuego()
    {
        // Activa gameplay y oculta menú principal
        mainMenu.SetActive(false);
        menuOpciones.SetActive(false);

        jugador.SetActive(true);
        enemigos.SetActive(true);
    }

    // --- Pausa ---
    private void Update()
    {
        if (jugador.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ReanudarJuego();
            else PausarJuego();
        }
    }

    public void PausarJuego()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // congela el tiempo
        isPaused = true;
    }

    public void ReanudarJuego()
    {
        pauseMenu.SetActive(false);
        menuOpciones.SetActive(false); // por si estaba abierto
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void AbrirOpcionesDesdePausa()
    {
        pauseMenu.SetActive(false);
        menuOpciones.SetActive(true);
    }

    public void VolverDeOpcionesAPausa()
    {
        menuOpciones.SetActive(false);
        pauseMenu.SetActive(true);
    }
}

