using UnityEngine;

public class CanvasController: MonoBehaviour
{
    //---------------Paneles principales---------------
    [Header("Paneles principales")]
    [SerializeField] GameObject panelMainMenu;
    [SerializeField] GameObject panelPausa;
    [SerializeField] GameObject panelOpciones;
    [SerializeField] GameObject panelMuerte;
    [SerializeField] GameObject panelDialogo;

    //---------------HUD---------------
    [Header("HUD siempre visible")]
    [SerializeField] GameObject panelHUD;

    private GameObject panelActivo;
    private GameObject panelAnterior;

    void Start()
    {
        DesactivarTodos();
        if (panelHUD != null) panelHUD.SetActive(true);
        MostrarMainMenu(); // arranca en menú principal
    }

    void Update()
    {
        //---------------Control ESC---------------
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Si estamos en juego (sin panel activo y tiempo corriendo)
            if (panelActivo == null && Time.timeScale == 1f)
            {
                MostrarPausa();
            }
            // Si estamos en pausa y no en opciones
            else if (panelActivo == panelPausa)
            {
                Reanudar();
            }
            // Si estamos en opciones, cerrarlas y volver al panel anterior
            else if (panelActivo == panelOpciones)
            {
                CerrarOpciones();
            }
        }
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

        CerrarPanelActivo();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 🔧 Ya no mostramos tutorial bloqueante aquí.
        // El nuevo TutorialInteractivo se encarga de mostrar instrucciones en HUD.
    }

    public void CargarPartidaDesdeMenu()
    {
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        GameManager gm = GameManager.Instancia;

        if (jugador != null && gm != null)
        {
            gm.ReiniciarEstado();
            SistemaGuardar.Cargar(jugador, gm);

            if (!gm.tieneLinterna && gm.linternaPickup != null)
                gm.linternaPickup.SetActive(true);

            jugador.enabled = true;
            Camera cam = jugador.GetComponentInChildren<Camera>();
            if (cam != null) cam.enabled = true;
        }

        CerrarPanelActivo();

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("📂 Partida cargada desde menú principal.");
    }

    public void SalirJuego() => Application.Quit();

    //---------------Pausa---------------
    public void MostrarPausa()
    {
        ActivarPanel(panelPausa, true);
    }

    public void Reanudar()
    {
        panelPausa.SetActive(false);
        panelActivo = null;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SalirAlMenuDesdePausa()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        panelActivo = null;

        if (panelMainMenu != null) panelMainMenu.SetActive(true);
        panelActivo = panelMainMenu;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("✅ Volviendo al menú principal desde pausa.");
    }

    //---------------Opciones---------------
    public void MostrarOpciones()
    {
        panelAnterior = panelActivo;
        if (panelAnterior != null) panelAnterior.SetActive(false);

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

            if (panelAnterior != null)
            {
                panelAnterior.SetActive(true);
                panelActivo = panelAnterior;
                panelAnterior = null;

                if (panelActivo == panelPausa || panelActivo == panelMainMenu)
                {
                    Time.timeScale = 0f;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }
    }

    //---------------Opciones: Guardar/Cargar---------------
    public void GuardarPartida()
    {
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        GameManager gm = GameManager.Instancia;

        if (jugador != null && gm != null)
        {
            SistemaGuardar.Guardar(jugador, gm);
            Debug.Log("💾 Partida guardada desde menú de opciones.");
        }
    }

    public void CargarPartida()
    {
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        GameManager gm = GameManager.Instancia;

        if (jugador != null && gm != null)
        {
            SistemaGuardar.Cargar(jugador, gm);
            Debug.Log("📂 Último punto de guardado cargado desde menú de opciones.");
        }
    }

    //---------------Pantalla de Muerte---------------
    public void MostrarPantallaMuerte()
    {
        DesactivarTodos();
        if (panelMuerte != null) panelMuerte.SetActive(true);
        panelActivo = panelMuerte;

        PantallaDeMuerte pm = panelMuerte.GetComponent<PantallaDeMuerte>();
        if (pm != null) pm.ActivarPantallaMuerte();

        Debug.Log("☠️ Pantalla de muerte activada.");
    }

    public void ReintentarDesdeMuerte()
    {
        if (panelMuerte != null) panelMuerte.SetActive(false);
        panelActivo = null;

        if (panelHUD != null) panelHUD.SetActive(true);

        GameManager gm = GameManager.Instancia;
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        if (gm != null && jugador != null)
        {
            gm.ReiniciarEstado();
            SistemaGuardar.Cargar(jugador, gm);

            if (!gm.tieneLinterna && gm.linternaPickup != null)
                gm.linternaPickup.SetActive(true);
        }

        if (jugador != null) jugador.enabled = true;
        Camera cam = jugador.GetComponentInChildren<Camera>();
        if (cam != null) cam.enabled = true;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("🔄 Reintentando partida desde pantalla de muerte.");
    }

    public void SalirAlMenuDesdeMuerte()
    {
        if (panelMuerte != null) panelMuerte.SetActive(false);
        panelActivo = null;

        if (panelMainMenu != null) panelMainMenu.SetActive(true);
        panelActivo = panelMainMenu;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("✅ Volviendo al menú principal desde pantalla de muerte.");
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
        if (panelDialogo != null) panelDialogo.SetActive(false);
        // 🔧 Ya no desactivamos tutorial aquí, porque ahora es parte del HUD interactivo.
    }
}