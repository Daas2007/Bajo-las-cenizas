using UnityEngine;

public class CanvasController : MonoBehaviour
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

    //---------------Panel del candado (nuevo)---------------
    [Header("Panel específico del candado")]
    [Tooltip("Panel UI del puzzle del candado")]
    [SerializeField] private GameObject panelCandado;
    [Tooltip("Referencia opcional al CandadoController para restaurar controles al cerrar")]
    [SerializeField] private CandadoController candadoControllerRef;

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
            // Si está abierto el panel del candado, cerrarlo y abrir pausa
            if (panelActivo == panelCandado)
            {
                CerrarPanelCandado();
                MostrarPausa();
                return;
            }

            // Si estamos en juego (sin panel activo y tiempo corriendo)
            if (panelActivo == null && Time.timeScale == 1f)
            {
                MostrarPausa();
                return;
            }
            // Si estamos en pausa y no en opciones
            else if (panelActivo == panelPausa)
            {
                Reanudar();
                return;
            }
            // Si estamos en opciones, cerrarlas y volver al panel anterior
            else if (panelActivo == panelOpciones)
            {
                CerrarOpciones();
                return;
            }
        }
    }

    //---------------MainMenu---------------
    public void MostrarMainMenu()
    {
        ActivarPanel(panelMainMenu, true);
    }

    // Jugar: siempre inicia desde spawnInicial con estado inicial
    public void Jugar()
    {
        GameManager gm = GameManager.Instancia;
        if (gm != null)
        {
            gm.NuevaPartida(); // reinicia todo al inicio y borra guardado
        }

        // Cerrar cualquier panel y reanudar juego
        CerrarPanelActivo();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("▶️ Jugar: inicio limpio desde spawn inicial.");
    }

    // Cargar desde menú principal: si hay guardado, cargar; si no, respawnear en spawnInicial
    public void CargarPartidaDesdeMenu()
    {
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        GameManager gm = GameManager.Instancia;

        if (jugador != null && gm != null)
        {
            // Intentar cargar; SistemaGuardar.Cargar devuelve true si cargó desde archivo
            bool cargado = SistemaGuardar.Cargar(jugador, gm);
            if (!cargado)
            {
                // No hay guardado: dejar estado inicial y colocar en spawn inicial
                gm.ReiniciarEstado();
                if (gm.spawnInicial != null)
                {
                    jugador.transform.position = gm.spawnInicial.position;
                    jugador.transform.rotation = gm.spawnInicial.rotation;
                }

                if (gm.linternaPickup != null) gm.linternaPickup.SetActive(true);
                if (gm.linternaEnMano != null) gm.linternaEnMano.SetActive(false);
                gm.tieneLinterna = false;

                Debug.Log("📂 No había guardado: respawneado en spawn inicial.");
            }
            else
            {
                Debug.Log("📂 Guardado cargado desde menú.");
            }

            jugador.enabled = true;
            Camera cam = jugador.GetComponentInChildren<Camera>();
            if (cam != null) cam.enabled = true;
        }

        CerrarPanelActivo();

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SalirJuego() => Application.Quit();

    //---------------Pausa---------------
    public void MostrarPausa()
    {
        ActivarPanel(panelPausa, true);
    }

    public void Reanudar()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        panelActivo = null;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Salir al menú desde pausa: reiniciar estado y mostrar menú principal (no conservar guardado activo al volver a jugar)
    public void SalirAlMenuDesdePausa()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        panelActivo = null;

        if (panelMainMenu != null) panelMainMenu.SetActive(true);
        panelActivo = panelMainMenu;

        // Reiniciar estado del juego para asegurar que al volver a Jugar se inicie limpio
        GameManager gm = GameManager.Instancia;
        if (gm != null)
        {
            gm.NuevaPartida(); // borra guardado y deja todo en estado inicial
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("✅ Volviendo al menú principal desde pausa (estado inicial aplicado).");
    }

    //---------------Opciones---------------
    public void MostrarOpciones()
    {
        panelAnterior = panelActivo;

        if (panelOpciones != null) panelOpciones.SetActive(true);
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

    // Cargar desde opciones: intenta cargar, si no existe guardado respawnea en spawn inicial
    public void CargarPartida()
    {
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        GameManager gm = GameManager.Instancia;

        if (jugador != null && gm != null)
        {
            bool cargado = SistemaGuardar.Cargar(jugador, gm);
            if (!cargado)
            {
                gm.ReiniciarEstado();
                if (gm.spawnInicial != null)
                {
                    jugador.transform.position = gm.spawnInicial.position;
                    jugador.transform.rotation = gm.spawnInicial.rotation;
                }
                if (gm.linternaPickup != null) gm.linternaPickup.SetActive(true);
                if (gm.linternaEnMano != null) gm.linternaEnMano.SetActive(false);
                gm.tieneLinterna = false;

                Debug.Log("📂 No había guardado: respawneado en spawn inicial desde opciones.");
            }
            else
            {
                Debug.Log("📂 Guardado cargado desde opciones.");
            }
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

    // Reintentar desde muerte: cargar último guardado si existe; si no, respawnear en spawn inicial
    public void ReintentarDesdeMuerte()
    {
        if (panelMuerte != null) panelMuerte.SetActive(false);
        panelActivo = null;

        if (panelHUD != null) panelHUD.SetActive(true);

        GameManager gm = GameManager.Instancia;
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        if (gm != null && jugador != null)
        {
            bool cargado = SistemaGuardar.Cargar(jugador, gm);
            if (!cargado)
            {
                // No hay guardado: respawnear en spawn inicial con estado inicial
                gm.ReiniciarEstado();
                if (gm.spawnInicial != null)
                {
                    jugador.transform.position = gm.spawnInicial.position;
                    jugador.transform.rotation = gm.spawnInicial.rotation;
                }
                if (gm.linternaPickup != null) gm.linternaPickup.SetActive(true);
                if (gm.linternaEnMano != null) gm.linternaEnMano.SetActive(false);
                gm.tieneLinterna = false;

                Debug.Log("🔄 Reintentar: no había guardado, respawneado en spawn inicial.");
            }
            else
            {
                Debug.Log("🔄 Reintentar: guardado cargado correctamente.");
            }
        }

        if (jugador != null) jugador.enabled = true;
        Camera cam = jugador.GetComponentInChildren<Camera>();
        if (cam != null) cam.enabled = true;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Salir al menú desde muerte: reiniciar estado y mostrar menú principal
    public void SalirAlMenuDesdeMuerte()
    {
        if (panelMuerte != null) panelMuerte.SetActive(false);
        panelActivo = null;

        if (panelMainMenu != null) panelMainMenu.SetActive(true);
        panelActivo = panelMainMenu;

        GameManager gm = GameManager.Instancia;
        if (gm != null)
        {
            gm.NuevaPartida(); // reinicia desde inicio y borra guardado
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("✅ Volviendo al menú principal desde pantalla de muerte (estado inicial).");
    }

    //---------------Dialogo---------------
    public void MostrarDialogo() => ActivarPanel(panelDialogo, true);

    //---------------Panel Candado (nuevo)---------------
    public void MostrarPanelCandado()
    {
        ActivarPanel(panelCandado, true);

        // Si el controller no está asignado, intentamos buscar uno en escena (fallback)
        if (candadoControllerRef == null)
        {
            candadoControllerRef = FindObjectOfType<CandadoController>();
        }
    }

    public void CerrarPanelCandado()
    {
        // Restaurar controles a través del controller si está asignado
        if (candadoControllerRef == null)
        {
            candadoControllerRef = FindObjectOfType<CandadoController>();
        }

        if (candadoControllerRef != null)
        {
            candadoControllerRef.DesactivarPuzzle();
        }
        else
        {
            // Fallback: ocultar panel y restaurar cursor y tiempo por seguridad
            if (panelCandado != null) panelCandado.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }

        panelActivo = null;
        Debug.Log("[CanvasController] Panel candado cerrado y controles restaurados.");
    }

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
        if (panelCandado != null) panelCandado.SetActive(false); // aseguramos que se apague al resetear paneles
        panelActivo = null;
    }
}
