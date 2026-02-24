using UnityEngine;

public class CanvasController : MonoBehaviour
{
    //---------------Paneles principales---------------
    [Header("Paneles principales")]
    [SerializeField] GameObject panelMainMenu;
    [SerializeField] GameObject panelPausa;
    [SerializeField] GameObject panelOpciones;
    [SerializeField] GameObject panelMuerte;
    [SerializeField] GameObject panelTutorial;
    [SerializeField] GameObject panelDialogo;

    //---------------HUD---------------
    [Header("HUD siempre visible")]
    [SerializeField] GameObject panelHUD;

    private GameObject panelActivo;
    private GameObject panelAnterior; // recuerda el panel debajo de Opciones

    void Start()
    {
        DesactivarTodos();
        if (panelHUD != null) panelHUD.SetActive(true);
        MostrarMainMenu(); // arranca en menú principal
    }

    //---------------MainMenu---------------
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
            jugador.transform.position = gm.spawnInicial.position;
            gm.ReiniciarEstado();
        }

        // cerrar menú principal y activar tutorial
        CerrarPanelActivo();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        MostrarTutorial();
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

    //---------------Pausa---------------
    public void MostrarPausa()
    {
        ActivarPanel(panelPausa, true);
    }

    public void Reanudar()
    {
        if (panelActivo == panelPausa)
        {
            panelPausa.SetActive(false);
            panelActivo = null;

            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    //---------------Opciones---------------
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

    //---------------Tutorial---------------
    public void MostrarTutorial()
    {
        DesactivarTodos();
        panelTutorial.SetActive(true);
        panelActivo = panelTutorial;

        // Pausar juego y bloquear movimiento
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        if (jugador != null) jugador.enabled = false;
    }

    public void CerrarTutorial()
    {
        if (panelActivo == panelTutorial)
        {
            panelTutorial.SetActive(false);
            panelActivo = null;

            // Reanudar juego y habilitar movimiento
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
            if (jugador != null) jugador.enabled = true;
        }
    }

    //---------------Dialogo---------------
    public void MostrarDialogo() => ActivarPanel(panelDialogo, true);

    //---------------Control general---------------
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
    }
}
