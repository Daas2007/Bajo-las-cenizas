using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour
{
    [Header("Paneles principales")]
    [SerializeField] GameObject panelPausa;
    [SerializeField] GameObject panelOpciones;
    [SerializeField] GameObject panelMuerte;
    [SerializeField] GameObject panelDialogo;

    [Header("Paneles de diálogo adicionales")]
    [SerializeField] private GameObject panelDialogoCuaderno;
    [SerializeField] private GameObject panelDialogoPapelTutorial;
    [SerializeField] private GameObject panelDialogoIntroduccion;
    [SerializeField] private GameObject panelDialogoMasContexto;


    [Header("HUD siempre visible")]
    [SerializeField] GameObject panelHUD;

    [Header("Panel específico del candado")]
    [SerializeField] private GameObject panelCandado;
    [SerializeField] private CandadoController candadoControllerRef;

    [Header("Pantalla de carga")]
    [SerializeField] private GameObject panelLoading;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float holdDuration = 3f;

    private Image loadingImage;
    private GameObject panelActivo;
    private GameObject panelAnterior;

    void Awake()
    {
        if (panelLoading != null)
            loadingImage = panelLoading.GetComponent<Image>();

        Time.timeScale = 1f;
    }
    private void OnEnable()
    {
        DesactivarTodos();
        BloquearMouse();
        if (panelHUD != null) panelHUD.SetActive(true);

        if (panelLoading != null && loadingImage != null)
        {
            panelLoading.SetActive(true);
            loadingImage.color = new Color(0f, 0f, 0f, 1f);
            StartCoroutine(FadeOut());
        }
        else
        {
            // ✅ Si no hay fade, activar directamente introducción
            MostrarDialogoIntroduccionInicial();
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 🔹 Caso especial: cerrar diálogo si está activo
            if (Dialogo.AnyDialogActive)
            {
                Dialogo[] dialogos = FindObjectsOfType<Dialogo>();
                foreach (var d in dialogos)
                {
                    if (d.mostrandoDialogo)
                    {
                        d.TerminarDialogo();
                    }
                }

                // ✅ restaurar HUD y controles
                if (panelHUD != null) panelHUD.SetActive(true);
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                return;
            }

            // 🔹 Lógica normal de pausa
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

        if (panelHUD != null)
            panelHUD.SetActive(true); // ✅ restaurar HUD al volver al juego

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
        while (t < fadeDuration)
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
            yield return new WaitForSecondsRealtime(5f);

            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            SceneManager.LoadScene(nombreEscena);
        }
    }
    private IEnumerator FadeOut()
    {
        float t = 0f;
        Color c = loadingImage.color;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            loadingImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
        loadingImage.color = new Color(c.r, c.g, c.b, 0f);
        panelLoading.SetActive(false);

        // ✅ Mostrar introducción sin pausar y con HUD activo
        MostrarDialogoIntroduccionInicial();
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

                if (panelActivo == panelPausa)
                {
                    Time.timeScale = 0f; // ✅ pausa solo si era el menú de pausa
                }
                else
                {
                    Time.timeScale = 1f; // ✅ cualquier otro panel mantiene tiempo en 1
                }

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
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
        StartCoroutine(ReintentarConFade());
    }
    private IEnumerator ReintentarConFade()
    {
        // ✅ Mostrar panel de carga y hacer fade in
        if (panelLoading != null && loadingImage != null)
        {
            panelLoading.SetActive(true);
            yield return StartCoroutine(FadeIn());
            yield return new WaitForSecondsRealtime(2f); // tiempo opaco antes de reaparecer
        }

        // ✅ Cerrar panel de muerte y restaurar HUD
        if (panelMuerte != null) panelMuerte.SetActive(false);
        panelActivo = null;
        if (panelHUD != null) panelHUD.SetActive(true);

        // ✅ Restaurar estado del juego
        GameManager gm = GameManager.Instancia;
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();

        if (gm != null && jugador != null)
        {
            bool cargado = SistemaGuardar.Cargar(jugador, gm);
            if (!cargado)
            {
                gm.ReiniciarEstado();
                gm.TeleportarASpawnInicial();
                Debug.Log("🔄 Reintentar: no había guardado, respawneado en spawn inicial.");
            }
            else
            {
                Debug.Log("🔄 Reintentar: guardado cargado correctamente.");
            }
        }

        // ✅ Reactivar jugador y cámara
        if (jugador != null) jugador.enabled = true;
        Camera cam = jugador.GetComponentInChildren<Camera>();
        if (cam != null) cam.enabled = true;

        // ✅ Restaurar tiempo y controles
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ✅ Fade out para volver al juego
        if (panelLoading != null && loadingImage != null)
        {
            yield return StartCoroutine(FadeOut());
        }
    }
    public void SalirAlMenuDesdeMuerte()
    {
        SceneManager.LoadScene("MainMenu");
        if (panelMuerte != null) panelMuerte.SetActive(false);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("✅ Volviendo al menú principal desde pantalla de muerte.");
    }
    //---------------Dialogo---------------
    public void MostrarDialogo() => ActivarPanel(panelDialogo, true);
    //---------------Dialogos adicionales---------------
    public void MostrarDialogoCuaderno()
    {
        ActivarPanel(panelDialogoCuaderno, true);
    }

    public void MostrarDialogoPapelTutorial()
    {
        ActivarPanel(panelDialogoPapelTutorial, true);
    }

    public void MostrarDialogoIntroduccion()
    {
        ActivarPanel(panelDialogoIntroduccion, true);
    }

    public void MostrarDialogoMasContexto()
    {
        ActivarPanel(panelDialogoMasContexto, true);
    }

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

        if (panelHUD != null)
            panelHUD.SetActive(false); // ✅ siempre desactivar HUD al abrir cualquier panel

        if (panel != null)
        {
            panel.SetActive(true);
            panelActivo = panel;

            if (pausarJuego)
            {
                // ✅ solo los paneles que realmente pausan el juego
                Time.timeScale = 0f;
            }
            else
            {
                // ✅ forzar que el tiempo siga en 1 para evitar problemas
                Time.timeScale = 1f;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public void panelUIActivo()
    {
        if (Time.timeScale != 0)
            panelHUD.SetActive(true);
    }
    private void DesactivarTodos()
    {
        if (panelHUD != null) panelHUD.SetActive(false);
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (panelMuerte != null) panelMuerte.SetActive(false);
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (panelCandado != null) panelCandado.SetActive(false);

        // 🔹 nuevos paneles
        if (panelDialogoCuaderno != null) panelDialogoCuaderno.SetActive(false);
        if (panelDialogoPapelTutorial != null) panelDialogoPapelTutorial.SetActive(false);
        if (panelDialogoIntroduccion != null) panelDialogoIntroduccion.SetActive(false);
        if (panelDialogoMasContexto != null) panelDialogoMasContexto.SetActive(false);

        panelActivo = null;
    }
    public void MostrarDialogoIntroduccionInicial()
    {
        DesactivarTodos();

        // ✅ Mantener HUD activo
        if (panelHUD != null)
            panelHUD.SetActive(true);

        if (panelDialogoIntroduccion != null)
        {
            panelDialogoIntroduccion.SetActive(true);
            panelActivo = panelDialogoIntroduccion;

            // ✅ No pausar el juego
            Time.timeScale = 1f;

            // ✅ Bloquear mouse como en gameplay normal
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void BloquearMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}