using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VerificadorGanar : MonoBehaviour
{
    [Header("UI Fade")]
    [SerializeField] private GameObject panelLoading;
    [SerializeField] private UnityEngine.UI.Image loadingImage;
    [SerializeField] private float fadeDuration = 2f;

    [Header("Dialogo")]
    [SerializeField] private Dialogo dialogo;

    [Header("Objetos de escena")]
    [SerializeField] private PuertaInteractuable puerta;
    [SerializeField] private GameObject enemigoVentana;
    [SerializeField] private GameObject enemigoNormal;
    [SerializeField] private Transform jugador;
    [SerializeField] private MonoBehaviour scriptMovimientoJugador;
    [SerializeField] private MonoBehaviour scriptCamara;

    [Header("Trigger de enemigo")]
    [SerializeField] private GameObject triggerEnemigo;

    [Header("Rotación al terminar diálogo")]
    [SerializeField] private float anguloFinalY = 180f; // 🔹 configurable en Inspector

    private bool enemigoPasoTrigger = false;

    private void Awake()
    {
        triggerEnemigo.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MovimientoPersonaje MP = other.GetComponentInParent<MovimientoPersonaje>();
            if (MP != null && MP.TieneCristal())
            {
                Debug.Log("✅ Jugador con cristal pasó por el trigger. Iniciando diálogo de victoria...");

                if (triggerEnemigo != null && !triggerEnemigo.activeSelf)
                    triggerEnemigo.SetActive(true);

                if (dialogo != null)
                {
                    dialogo.ResetHaHablado();
                    dialogo.OnDialogoCompleto.AddListener(() =>
                    {
                        if (jugador != null)
                        {
                            Vector3 rot = jugador.eulerAngles;
                            jugador.eulerAngles = new Vector3(rot.x, anguloFinalY, rot.z);
                            Debug.Log("🔄 Personaje girado al ángulo Y: " + anguloFinalY);
                        }

                        if (gameObject.activeInHierarchy) // ✅ aseguramos que este objeto esté activo
                            StartCoroutine(FlujoVictoria());
                    });
                    dialogo.IniciarDialogo();
                }
                else
                {
                    if (gameObject.activeInHierarchy) // ✅ aseguramos que este objeto esté activo
                        StartCoroutine(FlujoVictoria());
                }
            }
            else
            {
                Debug.Log("⚠️ Jugador pasó por el trigger pero NO tiene el cristal.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemigoPerseguidor"))
        {
            if (puerta != null)
            {
                puerta.Interactuar();
                Debug.Log("🚪 Puerta cerrada porque el enemigo salió del área.");
            }

            if (enemigoNormal != null)
            {
                enemigoNormal.SetActive(false);
                Debug.Log("❌ Enemigo desactivado al salir del área.");
            }

            enemigoPasoTrigger = true;
        }
    }

    private IEnumerator FlujoVictoria()
    {
        if (enemigoVentana != null) enemigoVentana.SetActive(false);
        if (enemigoNormal != null) enemigoNormal.SetActive(true);

        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = false;
        if (scriptCamara != null) scriptCamara.enabled = false;

        enemigoPasoTrigger = false;
        while (!enemigoPasoTrigger)
        {
            yield return null;
        }

        if (puerta != null)
        {
            puerta.Interactuar();
            Debug.Log("🚪 Puerta cerrada de golpe.");
        }

        while (puerta != null && puerta.EstaAbierta())
        {
            yield return null;
        }

        if (enemigoNormal != null) enemigoNormal.SetActive(false);

        IrAlMenu("MainMenu");
    }

    private void OnTriggerStay(Collider other)
    {
        if (triggerEnemigo != null && other.CompareTag("EnemigoPerseguidor"))
        {
            enemigoPasoTrigger = true;
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

    private IEnumerator SalirMenuCoroutine(string nombreEscena)
    {
        if (panelLoading != null)
        {
            panelLoading.SetActive(true);
            yield return StartCoroutine(FadeIn());
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            SceneManager.LoadScene(nombreEscena);
        }
    }

    public void IrAlMenu(string nombreEscena)
    {
        StartCoroutine(SalirMenuCoroutine(nombreEscena));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
