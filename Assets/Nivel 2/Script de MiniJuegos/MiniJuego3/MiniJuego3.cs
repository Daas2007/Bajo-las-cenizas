using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniJuego3 : MonoBehaviour, IMiniJuego
{
    public static MiniJuego3 Instance { get; private set; }

    [Header("Configuración de Escenas")]
    [SerializeField] private string nombreEscenaMinijuego = "MiniJuego2 Escena";

    [Header("Estado del Minijuego")]
    [SerializeField] private bool estaActivo = false;

    [Header("Elementos a Ocultar en la Escena Principal")]
    [Tooltip("Arrastra aquí el GameObject del Jugador o elementos que deban apagarse durante el minijuego.")]
    [SerializeField] private List<GameObject> objetosParaDesactivar = new List<GameObject>();

    private Coroutine rutinaCarga;
    private Coroutine rutinaDescarga;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IniciarMiniJuego()
    {
        if (estaActivo) return;

        // 1. Desactivar el movimiento/interacción de la cámara principal mediante su script
        Camara cam = Camera.main != null ? Camera.main.GetComponent<Camara>() : FindFirstObjectByType<Camara>();
        if (cam != null)
        {
            cam.SetModoUI(true);
        }

        // 2. Desactivar todos los objetos de la lista (Jugador, UI de juego, etc.)
        foreach (GameObject obj in objetosParaDesactivar)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        if (rutinaCarga != null) StopCoroutine(rutinaCarga);
        rutinaCarga = StartCoroutine(CargarMinijuegoRutina());
    }

    private IEnumerator CargarMinijuegoRutina()
    {
        estaActivo = true;

        AsyncOperation carga = SceneManager.LoadSceneAsync(nombreEscenaMinijuego, LoadSceneMode.Additive);

        while (!carga.isDone)
        {
            yield return null;
        }

        Scene escenaMinijuego = SceneManager.GetSceneByName(nombreEscenaMinijuego);
        if (escenaMinijuego.IsValid())
        {
            SceneManager.SetActiveScene(escenaMinijuego);
        }

        // CORRECCIÓN: En lugar de apagar el Camera.main genérico (que puede ser la cámara principal),
        // desactivamos únicamente el componente de la cámara anterior si sigue activa, 
        // o dejamos que la cámara del minijuego (que está en su propia escena) tome el control visual.
        Camera[] todasLasCamaras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera c in todasLasCamaras)
        {
            // Si la cámara pertenece a la escena principal (no a la del minijuego), la desactivamos temporalmente
            if (c.gameObject.scene != escenaMinijuego)
            {
                c.gameObject.SetActive(false);
            }
        }

        Debug.Log($"🎮 Minijuego Cargado: {nombreEscenaMinijuego}");
    }

    public void FinalizarMiniJuego()
    {
        if (!estaActivo) return;

        if (rutinaDescarga != null) StopCoroutine(rutinaDescarga);
        rutinaDescarga = StartCoroutine(DescargarMinijuegoRutina());
    }

    private IEnumerator DescargarMinijuegoRutina()
    {
        // 1. Reactivar la cámara principal del mundo y sus objetos
        Camara cam = FindFirstObjectByType<Camara>(FindObjectsInactive.Include);
        if (cam != null)
        {
            cam.gameObject.SetActive(true);
            cam.SetModoUI(false);
        }

        // 2. Volver a encender los objetos que estaban en la lista (Jugador, etc.)
        foreach (GameObject obj in objetosParaDesactivar)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        AsyncOperation descarga = SceneManager.UnloadSceneAsync(nombreEscenaMinijuego);

        while (!descarga.isDone)
        {
            yield return null;
        }

        estaActivo = false;
        Debug.Log($"✅ Minijuego Finalizado y Escena Descargada: {nombreEscenaMinijuego}");
    }
}
