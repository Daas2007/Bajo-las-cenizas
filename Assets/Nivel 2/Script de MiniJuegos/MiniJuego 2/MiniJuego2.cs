using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniJuego2 : MonoBehaviour, IMiniJuego
{
    [Header("Configuración de Escenas")]
    [SerializeField] private string nombreEscenaMinijuego = "MinijuegoDino";

    [Header("Estado del Minijuego")]
    [SerializeField] private bool estaActivo = false;

    private Coroutine rutinaCarga;
    private Coroutine rutinaDescarga;

    private void OnEnable()
    {
        // Mantiene consistencia si el objeto se activa manualmente en la jerarquía
        IniciarMiniJuego();
    }

    /// <summary>
    /// Implementación de IMiniJuego para arrancar la secuencia de carga aditiva.
    /// </summary>
    public void IniciarMiniJuego()
    {
        if (estaActivo) return;

        // Desactivar movimiento/interacción de la cámara principal mediante el script del juego
        Camara cam = Camera.main != null ? Camera.main.GetComponent<Camara>() : FindFirstObjectByType<Camara>();
        if (cam != null)
        {
            cam.SetModoUI(true);
        }

        if (rutinaCarga != null) StopCoroutine(rutinaCarga);
        rutinaCarga = StartCoroutine(CargarMinijuegoRutina());
    }

    /// <summary>
    /// Carga asíncrona de la escena aditiva y transferencia de control de cámara.
    /// </summary>
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

        // Ocultar cámara principal si la escena aditiva tiene su propia cámara
        if (Camera.main != null)
        {
            Camera.main.gameObject.SetActive(false);
        }

        Debug.Log($"🎮 Minijuego Cargado: {nombreEscenaMinijuego}");
    }

    /// <summary>
    /// Implementación de IMiniJuego para cerrar la escena y restaurar el mundo 3D/Principal.
    /// </summary>
    public void FinalizarMiniJuego()
    {
        if (!estaActivo) return;

        if (rutinaDescarga != null) StopCoroutine(rutinaDescarga);
        rutinaDescarga = StartCoroutine(DescargarMinijuegoRutina());
    }

    /// <summary>
    /// Descarga la escena aditiva y reactiva el estado de la cámara principal.
    /// </summary>
    private IEnumerator DescargarMinijuegoRutina()
    {
        // Reactivar cámara principal si fue desactivada
        Camara cam = FindFirstObjectByType<Camara>(FindObjectsInactive.Include);
        if (cam != null)
        {
            cam.gameObject.SetActive(true);
            cam.SetModoUI(false);
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