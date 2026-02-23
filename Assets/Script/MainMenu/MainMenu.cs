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

        // Colocar al jugador en el checkpoint inicial y reiniciar estado
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        GameManager gm = GameManager.Instancia;

        if (jugador != null && gm != null && gm.spawnInicial != null)
        {
            jugador.transform.position = gm.spawnInicial.position;

            // Reiniciar valores del GameManager
            gm.muertes = 0;
            gm.piezasRecogidas = 0;
            gm.osoCompleto = false;
            gm.puzzle1Completado = false;
            gm.puzzle2Completado = false;
            gm.cristalMetaActivo = false;
            gm.tieneLinterna = false;

            // Reiniciar linterna y objetos
            if (gm.linternaEnMano != null) gm.linternaEnMano.SetActive(false);
            if (gm.linternaPickup != null) gm.linternaPickup.SetActive(true);

            // Reiniciar todas las puertas bloqueadas
            if (gm.puertasBloqueadas != null)
            {
                foreach (GameObject puerta in gm.puertasBloqueadas)
                {
                    if (puerta != null) puerta.SetActive(true);
                }
            }

            Debug.Log("✅ Juego iniciado en el checkpoint inicial con valores y objetos reiniciados.");
        }
    }

    public void CargarPartida()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (opcionesPanel != null) opcionesPanel.SetActive(false);

        // Reanudar el juego
        Time.timeScale = 1f;

        // Bloquear cursor para gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Cargar partida desde SistemaGuardar
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        GameManager gm = GameManager.Instancia;

        if (jugador != null && gm != null)
        {
            SistemaGuardar.Cargar(jugador, gm);
            Debug.Log("✅ Partida cargada desde archivo de guardado.");
        }
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
