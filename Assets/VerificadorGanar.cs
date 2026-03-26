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
    [SerializeField] private PuertaInteractuable puerta;   // ✅ referencia a tu script de puerta
    [SerializeField] private GameObject enemigoVentana;
    [SerializeField] private GameObject enemigoNormal;
    [SerializeField] private Transform jugador;
    [SerializeField] private MonoBehaviour scriptMovimientoJugador; // ✅ para bloquear movimiento
    [SerializeField] private MonoBehaviour scriptCamara;            // ✅ para bloquear cámara

    [Header("Trigger de enemigo")]
    [SerializeField] private Collider triggerEnemigo; // collider con isTrigger

    private bool enemigoPasoTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MovimientoPersonaje MP = other.GetComponent<MovimientoPersonaje>();
            if (MP != null && MP.TieneCristal())
            {
                Debug.Log("✅ Jugador con cristal pasó por el trigger. Iniciando diálogo de victoria...");

                if (dialogo != null)
                {
                    dialogo.ResetHaHablado();
                    dialogo.OnDialogoCompleto.AddListener(() =>
                    {
                        StartCoroutine(FlujoVictoria());
                    });
                    dialogo.IniciarDialogo();
                }
                else
                {
                    StartCoroutine(FlujoVictoria());
                }
            }
            else
            {
                Debug.Log("⚠️ Jugador pasó por el trigger pero NO tiene el cristal.");
            }
        }
    }

    private IEnumerator FlujoVictoria()
    {
        // 🔹 1. Desactivar enemigo ventana
        if (enemigoVentana != null) enemigoVentana.SetActive(false);

        // 🔹 2. Activar enemigo normal
        if (enemigoNormal != null) enemigoNormal.SetActive(true);

        // 🔹 3. Girar jugador 180° en Y
        if (jugador != null)
        {
            Vector3 rot = jugador.eulerAngles;
            jugador.eulerAngles = new Vector3(rot.x, rot.y + 180f, rot.z);
        }

        // 🔹 4. Bloquear movimiento y cámara
        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = false;
        if (scriptCamara != null) scriptCamara.enabled = false;

        // 🔹 5. Esperar a que enemigo pase por el trigger
        enemigoPasoTrigger = false;
        while (!enemigoPasoTrigger)
        {
            yield return null;
        }

        // 🔹 6. Cerrar puerta de golpe (rotación inicial)
        if (puerta != null)
        {
            puerta.Interactuar(); // fuerza cierre hacia rotacionInicialEuler
            Debug.Log("🚪 Puerta cerrada de golpe.");
        }

        // 🔹 7. Esperar a que la puerta esté cerrada
        while (puerta != null && puerta.EstaAbierta())
        {
            yield return null;
        }

        // 🔹 8. Desactivar enemigo normal
        if (enemigoNormal != null) enemigoNormal.SetActive(false);

        // 🔹 9. Fade y volver al menú
        yield return StartCoroutine(SalirMenuCoroutine("MainMenu"));
    }

    private void OnTriggerStay(Collider other)
    {
        // ✅ Si el enemigo normal pasa por el trigger, marcar bandera
        if (triggerEnemigo != null && other.gameObject == enemigoNormal)
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
}
