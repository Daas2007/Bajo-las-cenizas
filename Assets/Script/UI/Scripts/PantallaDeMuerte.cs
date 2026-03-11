using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PantallaDeMuerte : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] GameObject panelMuerte;
    [SerializeField] TMP_Text textoMoriste;
    [SerializeField] GameObject botonReintentar;
    [SerializeField] GameObject botonSalir;
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject gameplayUI;

    [Header("Pantalla de carga")]
    [SerializeField] private GameObject panelLoading;
    [SerializeField] private float fadeDuration = 0.5f;   // 🔹 transición rápida: 0.5 segundos
    [SerializeField] private float holdDuration = 5f;     // 🔹 permanece opaco 5 segundos

    [Header("Configuración")]
    [SerializeField] float tiempoRojo = 1f;
    [SerializeField] float tiempoNegro = 1f;

    private Image fondo;
    private Image loadingImage;

    void Start()
    {
        fondo = panelMuerte.GetComponent<Image>();
        panelMuerte.SetActive(false);
        textoMoriste.gameObject.SetActive(false);
        botonReintentar.SetActive(false);
        botonSalir.SetActive(false);

        if (panelLoading != null)
        {
            loadingImage = panelLoading.GetComponent<Image>();
            panelLoading.SetActive(false);
        }
    }

    public void ActivarPantallaMuerte()
    {
        panelMuerte.SetActive(true);
        textoMoriste.gameObject.SetActive(false);
        botonReintentar.SetActive(false);
        botonSalir.SetActive(false);

        if (gameplayUI != null) gameplayUI.SetActive(false);

        fondo.color = new Color(1f, 0f, 0f, 0f);

        StartCoroutine(FadeRojoANegro());
    }

    IEnumerator FadeRojoANegro()
    {
        float t = 0f;
        while (t < tiempoRojo)
        {
            t += Time.deltaTime;
            float progreso = t / tiempoRojo;
            fondo.color = new Color(1f, 0f, 0f, progreso);
            yield return null;
        }

        t = 0f;
        while (t < tiempoNegro)
        {
            t += Time.deltaTime;
            float progreso = t / tiempoNegro;
            fondo.color = Color.Lerp(new Color(1f, 0f, 0f, 1f), new Color(0f, 0f, 0f, 1f), progreso);
            yield return null;
        }

        textoMoriste.gameObject.SetActive(true);
        botonReintentar.SetActive(true);
        botonSalir.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Reintentar()
    {
        CanvasController cc = FindObjectOfType<CanvasController>();
        if (cc != null) cc.ReintentarDesdeMuerte();
        cc.panelUIActivo();
    }

    public void SalirAlMenu()
    {
        StartCoroutine(SalirMenuCoroutine("MainMenu"));
    }

    private IEnumerator SalirMenuCoroutine(string nombreEscena)
    {
        if (panelLoading != null)
        {
            panelLoading.SetActive(true);
            yield return StartCoroutine(FadeIn());
            yield return new WaitForSecondsRealtime(holdDuration); // 🔹 espera 5 segundos opaco

            SceneManager.LoadScene(nombreEscena);
        }
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;
        Color c = loadingImage.color;
        while (t < fadeDuration) // 🔹 dura 0.5 segundos
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
        while (t < fadeDuration) // 🔹 dura 0.5 segundos
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            loadingImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
        loadingImage.color = new Color(c.r, c.g, c.b, 0f);
        panelLoading.SetActive(false);
    }
}

