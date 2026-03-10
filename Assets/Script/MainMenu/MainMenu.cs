using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;       // Panel principal del menú
    [SerializeField] GameObject opcionesPanel;   // Panel de opciones
    [SerializeField] GameObject panelLoading;    // Panel de carga con Image
    [SerializeField] float fadeDuration = 1f;    // 🔹 Duración del fade in/out ahora 1 segundo (más rápido)
    [SerializeField] float holdDuration = 2f;    // 🔹 Tiempo que permanece opaco reducido a 2 segundos

    private Image loadingImage;

    private void Awake()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (opcionesPanel != null) opcionesPanel.SetActive(false);

        if (panelLoading != null)
        {
            loadingImage = panelLoading.GetComponent<Image>();
            panelLoading.SetActive(false);
        }

        Time.timeScale = 1f;
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

            // Cargar escena mientras sigue opaco
            SceneManager.LoadScene(nombreEscena);

            // 🔹 Al entrar en la nueva escena, puedes llamar a FadeOut desde un script de inicio
        }
    }

    // 🔹 Fade In (pantalla se opaca en 0.3 segundos)
    private IEnumerator FadeIn()
    {
        float t = 0f;
        Color c = loadingImage.color;
        while (t < 0.4f) // ahora dura 0.3 segundos
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / 0.4f);
            loadingImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
        loadingImage.color = new Color(c.r, c.g, c.b, 1f);
    }

    // 🔹 Fade Out (pantalla se aclara en 0.3 segundos)
    public IEnumerator FadeOut()
    {
        float t = 0f;
        Color c = loadingImage.color;
        while (t < 0.4f) // también dura 0.3 segundos
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / 0.4f);
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CerrarOpciones()
    {
        if (opcionesPanel != null) opcionesPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    public void SalirJuego()
    {
        Application.Quit();
        Debug.Log("Juego cerrado desde el menú principal.");
    }
}
