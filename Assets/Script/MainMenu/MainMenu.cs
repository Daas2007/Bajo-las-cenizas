using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Paneles UI")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject opcionesPanel;
    [SerializeField] private GameObject panelLoading;

    [Header("Configuración de Transición")]
    [SerializeField] private Image loadingImage;
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField] private float fadeDurationRapido = 0.2f; // 🔹 Fundido rápido para opciones
    [SerializeField] private float holdDuration = 1.0f;

    private bool isTransitioning;

    private void Awake()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (mainPanel != null) mainPanel.SetActive(true);
        if (opcionesPanel != null) opcionesPanel.SetActive(false);
    }

    private async void Start()
    {
        if (panelLoading != null && loadingImage != null)
        {
            panelLoading.SetActive(true);
            SetImageAlpha(1f);

            await WaitRealtimeAsync(0.2f);
            await DoFadeAsync(1f, 0f, fadeDuration);

            panelLoading.SetActive(false);
        }
    }

    public async void JugarConLoading(string nombreEscena)
    {
        if (isTransitioning) return;
        isTransitioning = true;

        if (panelLoading != null && loadingImage != null)
        {
            panelLoading.SetActive(true);
            SetImageAlpha(0f);

            await DoFadeAsync(0f, 1f, fadeDuration);

            AsyncOperation loadOp = SceneManager.LoadSceneAsync(nombreEscena);
            loadOp.allowSceneActivation = false;

            float elapsed = 0f;
            while (elapsed < holdDuration || loadOp.progress < 0.9f)
            {
                elapsed += Time.unscaledDeltaTime;
                await Awaitable.NextFrameAsync();
            }

            loadOp.allowSceneActivation = true;
        }
        else
        {
            SceneManager.LoadScene(nombreEscena);
        }
    }

    // 🔹 Entrar a opciones con un fundido rápido de pantalla (Fade Out -> Cambiar Panel -> Fade In)
    public async void AbrirOpciones()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        if (panelLoading != null && loadingImage != null)
        {
            panelLoading.SetActive(true);
            SetImageAlpha(0f);
            await DoFadeAsync(0f, 1f, fadeDurationRapido); // Oscurece rápido

            if (mainPanel != null) mainPanel.SetActive(false);
            if (opcionesPanel != null) opcionesPanel.SetActive(true);

            await DoFadeAsync(1f, 0f, fadeDurationRapido); // Aclara rápido
            panelLoading.SetActive(false);
        }
        else
        {
            TogglePanels(main: false, opciones: true);
        }

        isTransitioning = false;
    }

    // 🔹 Salir de opciones con el mismo fundido rápido de pantalla
    public async void CerrarOpciones()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        if (panelLoading != null && loadingImage != null)
        {
            panelLoading.SetActive(true);
            SetImageAlpha(0f);
            await DoFadeAsync(0f, 1f, fadeDurationRapido); // Oscurece rápido

            if (opcionesPanel != null) opcionesPanel.SetActive(false);
            if (mainPanel != null) mainPanel.SetActive(true);

            await DoFadeAsync(1f, 0f, fadeDurationRapido); // Aclara rápido
            panelLoading.SetActive(false);
        }
        else
        {
            TogglePanels(main: true, opciones: false);
        }

        isTransitioning = false;
    }

    private async Awaitable DoFadeAsync(float startAlpha, float targetAlpha, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            SetImageAlpha(currentAlpha);
            await Awaitable.NextFrameAsync();
        }
        SetImageAlpha(targetAlpha);
    }

    private async Awaitable WaitRealtimeAsync(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            await Awaitable.NextFrameAsync();
        }
    }

    private void SetImageAlpha(float alpha)
    {
        if (loadingImage == null) return;
        Color c = loadingImage.color;
        c.a = alpha;
        loadingImage.color = c;
    }

    public void CargarPartida()
    {
        if (GameManager.Instancia != null)
        {
            SistemaGuardar.Cargar(null, GameManager.Instancia);
        }
    }

    private void TogglePanels(bool main, bool opciones)
    {
        if (mainPanel != null) mainPanel.SetActive(main);
        if (opcionesPanel != null) opcionesPanel.SetActive(opciones);
    }

    public async void SalirJuego()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        if (panelLoading != null && loadingImage != null)
        {
            panelLoading.SetActive(true);
            SetImageAlpha(0f);
            await DoFadeAsync(0f, 1f, fadeDuration);
            await WaitRealtimeAsync(0.5f);
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}