using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VerificadorGanar : MonoBehaviour
{
    [Header("UI Fade")]
    [SerializeField] private GameObject panelLoading;   // Panel con la imagen de fade
    [SerializeField] private UnityEngine.UI.Image loadingImage;
    [SerializeField] private float fadeDuration = 2f;

    [Header("Dialogo")]
    [SerializeField] private Dialogo dialogo; // referencia al script de diálogo

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MovimientoPersonaje MP = other.GetComponent<MovimientoPersonaje>();
            if (MP != null)
            {
                // ✅ Solo si el cristal ya fue obtenido
                if (MP.TieneCristal())
                {
                    Debug.Log("✅ Jugador con cristal pasó por el trigger. Iniciando diálogo de victoria...");

                    if (dialogo != null)
                    {
                        dialogo.ResetHaHablado();
                        dialogo.OnDialogoCompleto.AddListener(() =>
                        {
                            StartCoroutine(SalirMenuCoroutine("MainMenu"));
                        });
                        dialogo.IniciarDialogo();
                    }
                    else
                    {
                        // Si no hay diálogo asignado, ir directo al menú
                        StartCoroutine(SalirMenuCoroutine("MainMenu"));
                    }
                }
                else
                {
                    Debug.Log("⚠️ Jugador pasó por el trigger pero NO tiene el cristal.");
                }
            }
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
        if (panelLoading != null) panelLoading.SetActive(false);
    }

    private IEnumerator SalirMenuCoroutine(string nombreEscena)
    {
        if (panelLoading != null)
        {
            panelLoading.SetActive(true);
            yield return StartCoroutine(FadeIn());
            yield return new WaitForSecondsRealtime(3f); // tiempo de espera antes de cargar
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            SceneManager.LoadScene(nombreEscena);
        }
    }
}
