using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [Header("Paneles principales")]
    [SerializeField] GameObject panelMainMenu;
    [SerializeField] GameObject panelPausa;
    [SerializeField] GameObject panelOpciones;
    [SerializeField] GameObject panelMuerte;
    [SerializeField] GameObject panelTutorial;
    [SerializeField] GameObject panelDialogo;
    [SerializeField] GameObject panelPuzzleCandado;
    [SerializeField] GameObject panelPuzzleRompecabezas;
    [SerializeField] GameObject panelPista;

    [Header("HUD siempre visible")]
    [SerializeField] GameObject panelHUD;

    private GameObject panelActivo;
    private GameObject panelAnterior; // 👈 nuevo: recuerda el panel debajo de Opciones

    void Start()
    {
        DesactivarTodos();
        if (panelHUD != null) panelHUD.SetActive(true);
        MostrarMainMenu(); // arranca en menú principal
    }

    // ---------------- OPCIONES ----------------
    public void MostrarOpciones()
    {
        // Guardar el panel que estaba activo antes
        panelAnterior = panelActivo;

        DesactivarTodos();
        panelOpciones.SetActive(true);
        panelActivo = panelOpciones;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CerrarOpciones()
    {
        if (panelActivo == panelOpciones)
        {
            panelOpciones.SetActive(false);
            panelActivo = null;

            // Volver al panel anterior si existía
            if (panelAnterior != null)
            {
                panelAnterior.SetActive(true);
                panelActivo = panelAnterior;
                panelAnterior = null;
            }
        }
    }

    // ---------------- MENÚ PRINCIPAL ----------------
    public void MostrarMainMenu()
    {
        ActivarPanel(panelMainMenu, true);
    }

    public void Jugar()
    {
        GameManager gm = GameManager.Instancia;
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();

        if (jugador != null && gm != null && gm.spawnInicial != null)
        {
            // Colocar al jugador en el spawn inicial
            jugador.transform.position = gm.spawnInicial.position;

            // Reiniciar estado del juego
            gm.ReiniciarEstado();
        }

        // Cerrar el menú principal
        if (panelMainMenu != null) panelMainMenu.SetActive(false);

        // Reanudar el tiempo
        Time.timeScale = 1f;

        // Bloquear cursor para gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Activar HUD
        if (panelHUD != null) panelHUD.SetActive(true);

        // Activar tutorial
        CanvasController cc = FindObjectOfType<CanvasController>();
        if (cc != null)
        {
            cc.MostrarTutorial();
        }

        Debug.Log("✅ Juego iniciado en el checkpoint inicial con tutorial activo.");
    }



    public void CargarPartida()
    {
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        GameManager gm = GameManager.Instancia;
        if (jugador != null && gm != null)
        {
            SistemaGuardar.Cargar(jugador, gm);
        }
        CerrarPanelActivo();
    }

    public void SalirJuego() => Application.Quit();
    //----------------- TUTORIAL----------------------
    public void MostrarTutorial()
    {
        // Busca el CanvasController y activa el panel de tutorial
        CanvasController cc = FindObjectOfType<CanvasController>();
        if (cc != null)
        {
            cc.MostrarTutorial(); // llama al método que activa el panel tutorial
        }
    }


    // ---------------- MENÚ DE PAUSA ----------------
    public void MostrarPausa()
    {
        ActivarPanel(panelPausa, true);
    }

    public void Reanudar()
    {
        CerrarPanelActivo();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SalirAlMenu() => MostrarMainMenu();

    // ---------------- PANTALLA DE MUERTE ----------------
    public void MostrarPantallaMuerte()
    {
        ActivarPanel(panelMuerte, true);
    }

    public void Reintentar() => CargarPartida();

    // ---------------- CONTROL GENERAL ----------------
    public void CerrarPanelActivo()
    {
        if (panelActivo != null)
        {
            panelActivo.SetActive(false);
            panelActivo = null;
        }
    }

    private void ActivarPanel(GameObject panel, bool pausarJuego)
    {
        DesactivarTodos();
        if (panel != null)
        {
            panel.SetActive(true);
            panelActivo = panel;

            if (pausarJuego)
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    private void DesactivarTodos()
    {
        if (panelMainMenu != null) panelMainMenu.SetActive(false);
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (panelMuerte != null) panelMuerte.SetActive(false);
        if (panelTutorial != null) panelTutorial.SetActive(false);
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (panelPuzzleCandado != null) panelPuzzleCandado.SetActive(false);
        if (panelPuzzleRompecabezas != null) panelPuzzleRompecabezas.SetActive(false);
        if (panelPista != null) panelPista.SetActive(false);
    }
}
