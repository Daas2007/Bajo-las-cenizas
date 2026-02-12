using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PantallaDeMuerte : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] GameObject panelMuerte;     // El mismo panel que hace de fondo
    [SerializeField] TMP_Text textoMoriste;
    [SerializeField] GameObject botonReintentar;
    [SerializeField] GameObject botonSalir;
    [SerializeField] GameObject mainMenuPanel;   // Panel del menú principal
    [SerializeField] GameObject gameplayUI;      // UI de gameplay (stamina, interacción, etc.)

    [Header("Configuración")]
    [SerializeField] float tiempoRojo = 1.5f;    // tiempo para pasar de transparente a rojo
    [SerializeField] float tiempoNegro = 1.5f;   // tiempo para pasar de rojo a negro

    private Image fondo; // el Image del mismo Panel

    void Start()
    {
        fondo = panelMuerte.GetComponent<Image>();
        panelMuerte.SetActive(false);
        textoMoriste.gameObject.SetActive(false);
        botonReintentar.SetActive(false);
        botonSalir.SetActive(false);
    }

    public void ActivarPantallaMuerte()
    {
        panelMuerte.SetActive(true);
        textoMoriste.gameObject.SetActive(false);
        botonReintentar.SetActive(false);
        botonSalir.SetActive(false);

        // Ocultar UI de gameplay
        if (gameplayUI != null) gameplayUI.SetActive(false);

        // Reset color a transparente
        fondo.color = new Color(1f, 0f, 0f, 0f);

        StartCoroutine(FadeRojoANegro());
    }

    IEnumerator FadeRojoANegro()
    {
        // Paso 1: de transparente a rojo
        float t = 0f;
        while (t < tiempoRojo)
        {
            t += Time.deltaTime;
            float progreso = t / tiempoRojo;
            fondo.color = new Color(1f, 0f, 0f, progreso); // rojo con alfa progresivo
            yield return null;
        }

        // Paso 2: de rojo a negro
        t = 0f;
        while (t < tiempoNegro)
        {
            t += Time.deltaTime;
            float progreso = t / tiempoNegro;
            fondo.color = Color.Lerp(new Color(1f, 0f, 0f, 1f), new Color(0f, 0f, 0f, 1f), progreso);
            yield return null;
        }

        // Mostrar texto y botones
        textoMoriste.gameObject.SetActive(true);
        botonReintentar.SetActive(true);
        botonSalir.SetActive(true);

        // Pausar el juego
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Reintentar()
    {
        // Reinicia el gameplay dentro de la misma escena
        Time.timeScale = 1f;
        panelMuerte.SetActive(false);

        if (gameplayUI != null) gameplayUI.SetActive(true);
    }

    public void SalirAlMenu()
    {
        // Volver al menú principal dentro de la misma escena
        Time.timeScale = 0f;
        panelMuerte.SetActive(false);

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
}
