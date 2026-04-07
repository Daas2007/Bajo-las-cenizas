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
    [SerializeField] GameObject gameplayUI;

    [Header("Pantalla de carga")]
    [SerializeField] private GameObject panelLoading;
    [SerializeField] private float fadeDuration = 0.5f;   // transición rápida
    [SerializeField] private float holdDuration = 5f;     // permanece opaco 5 segundos

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

        textoMoriste.gameObject.SetActive(true);
        botonReintentar.SetActive(true);
        botonSalir.SetActive(true);

        if (gameplayUI != null) gameplayUI.SetActive(false);

        if (fondo != null)
            fondo.color = new Color(0f, 0f, 0f, 1f); // fondo negro fijo

        if (panelLoading != null)
            panelLoading.SetActive(false); // asegurarse que no tape la UI

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void Reintentar()
    {
        panelMuerte.SetActive(false);
        CanvasController cc = FindObjectOfType<CanvasController>();
        if (cc != null) cc.ReintentarDesdeMuerte();
        cc.panelUIActivo();
    }

    public void SalirAlMenu()
    {
        panelMuerte.SetActive(false);
        StartCoroutine(SalirMenuCoroutine("MainMenu"));
    }

    private IEnumerator SalirMenuCoroutine(string nombreEscena)
    {
        if (panelLoading != null)
        {
            panelLoading.SetActive(true);
            yield return StartCoroutine(FadeIn());
            yield return new WaitForSecondsRealtime(holdDuration);

            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
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
}
