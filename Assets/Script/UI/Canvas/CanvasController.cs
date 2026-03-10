using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

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

    //---------------Panel del candado---------------
    [Header("Panel específico del candado")]
    [Tooltip("Panel UI del puzzle del candado")]
    [SerializeField] private GameObject panelCandado;
    [Tooltip("Referencia opcional al CandadoController para restaurar controles al cerrar")]
    [SerializeField] private CandadoController candadoControllerRef;

    [Header("Pantalla de carga")]
    [SerializeField] private GameObject panelLoading; // Panel de carga con Image
    [SerializeField] private float fadeDuration = 2f; // Duración del fade in/out
    [SerializeField] private float holdDuration = 3f; // Tiempo que permanece opaco

    private Image loadingImage;

    private GameObject panelActivo;
    private GameObject panelAnterior;

    void Start()
    {
        DesactivarTodos();
        BloquearMouse();
        if (panelHUD != null) panelHUD.SetActive(true);
        if (panelLoading != null)
        {
            loadingImage = panelLoading.GetComponent<Image>();
            panelLoading.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panelActivo == panelCandado)
            {
                CerrarPanelCandado();
                MostrarPausa();
                return;
            }

            if (panelActivo == null && Time.timeScale == 1f)
            {
                MostrarPausa();
                return;
            }
            else if (panelActivo == panelPausa)
            {
                Reanudar();
                return;
            }
            else if (panelActivo == panelOpciones)
            {
                CerrarOpciones();
                return;
            }
        }
    }

    //---------------MainMenu---------------
    //public void MostrarMainMenu()
    //{
    //    ActivarPanel(panelMainMenu, true);
    //}

    //public void Jugar()
    //{
    //    if (panelHUD != null) panelHUD.SetActive(true);
    //    GameManager gm = GameManager.Instancia;
    //    if (gm != null) gm.NuevaPartida();

    //    CerrarPanelActivo();
    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;
    //    Time.timeScale = 1f;
    //    panelUIActivo();
    //    Debug.Log("▶️ Jugar: inicio limpio desde spawn inicial.");
    //}

    //public void CargarPartidaDesdeMenu()
    //{
    //    MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
    //    GameManager gm = GameManager.Instancia;

    //    if (jugador != null && gm != null)
    //    {
    //        bool cargado = SistemaGuardar.Cargar(jugador, gm);
    //        if (!cargado)
    //        {
    //            gm.ReiniciarEstado();
    //            if (gm.spawnInicial != null)
    //            {
    //                jugador.transform.position = gm.spawnInicial.position;
    //                jugador.transform.rotation = gm.spawnInicial.rotation;
    //            }
    //            if (gm.linternaPickup != null) gm.linternaPickup.SetActive(true);
    //            if (gm.linternaEnMano != null) gm.linternaEnMano.SetActive(false);
    //            gm.tieneLinterna = false;
    //            Debug.Log("📂 No había guardado: respawneado en spawn inicial.");
    //        }
    //        else
    //        {
    //            Debug.Log("📂 Guardado cargado desde menú.");
    //        }

    //        jugador.enabled = true;
    //        Camera cam = jugador.GetComponentInChildren<Camera>();
    //        if (cam != null) cam.enabled = true;
    //    }

    //    CerrarPanelActivo();
    //    Time.timeScale = 1f;
    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;
    //}

    //public void SalirJuego() => Application.Quit();

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
        panelUIActivo();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void SalirAlMenuDesdePausa()
    {
        StartCoroutine(SalirMenuCoroutine("MainMenu"));
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;
        Color c = loadingImage.color;
        while (t < fadeDuration) // 🔹 usa la variable
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            loadingImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
        loadingImage.color = new Color(c.r, c.g, c.b, 1f);
    }

    private IEnumerator SalirMenuCoroutine(string nombreEscena)
    {
        if (panelLoading != null)
        {
            panelLoading.SetActive(true);
            yield return StartCoroutine(FadeIn());
            yield return new WaitForSecondsRealtime(holdDuration); // 🔹 ajusta este valor

            SceneManager.LoadScene(nombreEscena);
            // Al entrar en la nueva escena, puedes llamar a FadeOut()
        }
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        Color c = loadingImage.color;
        while (t < fadeDuration) // 🔹 usa la variable
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            loadingImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
        loadingImage.color = new Color(c.r, c.g, c.b, 0f);
        panelLoading.SetActive(false);
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
    public void SalirAlMenuDesdeMuerte()
    {       
        SceneManager.LoadScene("MainMenu");
        if (panelMuerte != null) panelMuerte.SetActive(false);

        GameManager gm = GameManager.Instancia;
        if (gm != null) gm.NuevaPartida();

 // nombre exacto de tu escena de menú

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("✅ Volviendo al menú principal desde pantalla de muerte (estado inicial).");
    }
    //---------------Dialogo---------------
    public void MostrarDialogo() => ActivarPanel(panelDialogo, true);
    //---------------Panel Candado---------------
    public void MostrarPanelCandado()
    {
        ActivarPanel(panelCandado, true);
        if (candadoControllerRef == null)
            candadoControllerRef = FindObjectOfType<CandadoController>();
    }
    public void CerrarPanelCandado()
    {
        if (candadoControllerRef == null)
            candadoControllerRef = FindObjectOfType<CandadoController>();

        if (candadoControllerRef != null)
        {
            candadoControllerRef.DesactivarPuzzle();
        }
        else
        {
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
    public void panelUIActivo()
    {
        if (Time.timeScale !=0)
        panelHUD.SetActive(true);
    }
    private void DesactivarTodos()
    {
        if (panelHUD != null) panelHUD.SetActive(false);
        if (panelMainMenu != null) panelMainMenu.SetActive(false);
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (panelMuerte != null) panelMuerte.SetActive(false);
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (panelCandado != null) panelCandado.SetActive(false); // aseguramos que se apague al resetear paneles
        panelActivo = null;
    }
    public void BloquearMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
