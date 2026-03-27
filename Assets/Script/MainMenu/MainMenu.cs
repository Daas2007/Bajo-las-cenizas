using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;       // Panel principal del menú
    [SerializeField] GameObject opcionesPanel;   // Panel de opciones
    [SerializeField] GameObject panelLoading;    // Panel de carga con Image
    [SerializeField] float fadeDuration = 0.5f;  // Duración del fade in/out
    [SerializeField] float holdDuration = 5f;    // Tiempo opaco antes de cambiar de escena

    private Image loadingImage;

    private void Awake()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (opcionesPanel != null) opcionesPanel.SetActive(false);

        if (panelLoading != null)
        {
            loadingImage = panelLoading.GetComponent<Image>();
        }

        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        if (panelLoading != null && loadingImage != null)
        {
            panelLoading.SetActive(true);
            loadingImage.color = new Color(loadingImage.color.r, loadingImage.color.g, loadingImage.color.b, 1f);
            StartCoroutine(FadeOut()); // aclarar al entrar al menú
        }
    }

    private void Update()
    {
        // 🔹 Forzar siempre el mouse visible y desbloqueado
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 🔹 Jugar con transición de pantalla de carga
    public void JugarConLoading(string nombreEscena)
    {
        StartCoroutine(LoadingTransitionCoroutine(nombreEscena));
    }

    private IEnumerator LoadingTransitionCoroutine(string nombreEscena)
    {
        if (panelLoading != null)
        {
            panelLoading.SetActive(true);
            yield return StartCoroutine(FadeIn());
            yield return new WaitForSecondsRealtime(holdDuration);

            SceneManager.LoadScene(nombreEscena);
        }
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

    public IEnumerator FadeOut()
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

    public void CargarPartida()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (opcionesPanel != null) opcionesPanel.SetActive(false);

        Time.timeScale = 1f;

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
        if (mainPanel != null) mainPanel.SetActive(false);
    }

    public void CerrarOpciones()
    {
        if (opcionesPanel != null) opcionesPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    public void SalirJuego()
    {
        StartCoroutine(SalirJuegoConFade());
    }

    private IEnumerator SalirJuegoConFade()
    {
        if (panelLoading != null)
        {
            panelLoading.SetActive(true);
            yield return StartCoroutine(FadeIn());
            yield return new WaitForSecondsRealtime(1f);
        }

        Application.Quit();
        Debug.Log("Juego cerrado desde el menú principal con fade.");
    }
}
