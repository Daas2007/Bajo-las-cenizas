using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class NewCanvasController : MonoBehaviour
{
    [Header("Paneles principales")]
    [SerializeField] private GameObject panelPausa;
    [SerializeField] private GameObject panelOpciones;
    [SerializeField] private GameObject panelMuerte;

    [Header("HUD siempre visible")]
    [SerializeField] private GameObject panelHUD;

    [Header("Pantalla de carga")]
    [SerializeField] private GameObject panelLoading;
    [SerializeField] private float fadeDuration = 2f;

    private Image loadingImage;
    private GameObject panelActivo;
    private GameObject panelAnterior;

    private void Awake()
    {
        if (panelLoading != null)
            loadingImage = panelLoading.GetComponent<Image>();

        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        DesactivarTodos();
        if (panelHUD != null) panelHUD.SetActive(true);

        if (panelLoading != null && loadingImage != null)
        {
            panelLoading.SetActive(true);
            loadingImage.color = new Color(0f, 0f, 0f, 1f);
            StartCoroutine(FadeOut());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panelActivo == null && Time.timeScale == 1f)
            {
                MostrarPausa();
            }
            else if (panelActivo == panelPausa)
            {
                Reanudar();
            }
            else if (panelActivo == panelOpciones)
            {
                CerrarOpciones();
            }
        }
    }

    // --------------- Pausa ---------------
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
            panelHUD.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SalirAlMenuDesdePausa()
    {
        StartCoroutine(SalirMenuCoroutine("MainMenu"));
    }

    // --------------- Opciones ---------------
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

                Time.timeScale = (panelActivo == panelPausa) ? 0f : 1f;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    // --------------- Pantalla de Muerte ---------------
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
        if (panelLoading != null && loadingImage != null)
        {
            panelLoading.SetActive(true);
            yield return StartCoroutine(FadeIn());
            yield return new WaitForSecondsRealtime(2f);
        }

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
                gm.TeleportarASpawnInicial();
                Debug.Log("🔄 Reintentar: no había guardado, respawneado en spawn inicial.");
            }
            else
            {
                Debug.Log("🔄 Reintentar: guardado cargado correctamente.");
            }
        }

        if (jugador != null) jugador.enabled = true;
        Camera cam = jugador != null ? jugador.GetComponentInChildren<Camera>() : null;
        if (cam != null) cam.enabled = true;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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

    // --------------- Pantalla de Carga y Fades ---------------
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

    public IEnumerator CanvasFadeInRapido()
    {
        if (panelLoading == null || loadingImage == null)
            yield break;

        panelLoading.SetActive(true);

        float t = 0f;
        float duracionRapida = 0.5f;
        Color c = loadingImage.color;

        while (t < duracionRapida)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / duracionRapida);
            loadingImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        loadingImage.color = new Color(c.r, c.g, c.b, 1f);
        panelLoading.SetActive(false);
    }

    // --------------- Control General y Utilidades ---------------
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
            panelHUD.SetActive(false);

        if (panel != null)
        {
            panel.SetActive(true);
            panelActivo = panel;
            Time.timeScale = pausarJuego ? 0f : 1f;
        }
    }

    public void panelUIActivo()
    {
        if (Time.timeScale != 0 && panelHUD != null)
            panelHUD.SetActive(true);
    }

    private void DesactivarTodos()
    {
        if (panelHUD != null) panelHUD.SetActive(false);
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (panelMuerte != null) panelMuerte.SetActive(false);

        panelActivo = null;
    }
}