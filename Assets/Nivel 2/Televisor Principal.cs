using UnityEngine;
using UnityEngine.Events;

public class TelevisorPrincipal : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private int vidaMaxima = 10;
    [SerializeField] private int vidaActual;

    [Header("Referencias del Enemigo Grande")]
    [Tooltip("GameObject del Enemigo Grande (debe iniciar desactivado en la escena)")]
    [SerializeField] private GameObject enemigoGrande;

    [Header("Efectos Visuales / Audios (Opcionales)")]
    [SerializeField] private GameObject efectoPantallaRota;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoGolpe;
    [SerializeField] private AudioClip sonidoRuptura;

    [Header("Eventos")]
    public UnityEvent OnTelevisorRoto;

    private bool estaRoto = false;

    private void Awake()
    {
        vidaActual = vidaMaxima;
    }

    private void Start()
    {
        // Asegurar que el Enemigo Grande empiece desactivado si se asignó
        if (enemigoGrande != null && enemigoGrande.activeSelf)
        {
            enemigoGrande.SetActive(false);
        }
    }

    /// <summary>
    /// Método llamado por el MiniEnemigoController cuando golpea la TV.
    /// </summary>
    public void RecibirDaño(int cantidad)
    {
        if (estaRoto) return;

        vidaActual -= cantidad;
        Debug.Log($"📺 Televisor Principal recibió {cantidad} de daño. Vida restante: {vidaActual}/{vidaMaxima}");

        // Reproducir sonido de golpe si está configurado
        if (audioSource != null && sonidoGolpe != null)
        {
            audioSource.PlayOneShot(sonidoGolpe);
        }

        // Verificar si la TV fue destruida
        if (vidaActual <= 0)
        {
            RomperTelevisor();
        }
    }

    private void RomperTelevisor()
    {
        estaRoto = true;
        vidaActual = 0;

        Debug.Log("💥 ¡El Televisor Principal se ha roto! Liberando al Enemigo Grande...");

        // Activar efecto visual de rotura si existe
        if (efectoPantallaRota != null)
        {
            efectoPantallaRota.SetActive(true);
        }

        // Reproducir sonido de ruptura
        if (audioSource != null && sonidoRuptura != null)
        {
            audioSource.PlayOneShot(sonidoRuptura);
        }

        // Activar el Enemigo Grande para que persiga al jugador
        if (enemigoGrande != null)
        {
            enemigoGrande.SetActive(true);
        }
        else
        {
            Debug.LogError("⚠️ ¡ERROR! No se asignó la referencia del Enemigo Grande en el Televisor Principal.");
        }

        // Invocar eventos personalizados si se configuraron en el Inspector
        OnTelevisorRoto?.Invoke();
    }

    /// <summary>
    /// Utiliza este método para reiniciar la TV si el jugador reintenta el nivel.
    /// </summary>
    public void RepararTelevisor()
    {
        vidaActual = vidaMaxima;
        estaRoto = false;

        if (efectoPantallaRota != null)
        {
            efectoPantallaRota.SetActive(false);
        }

        if (enemigoGrande != null)
        {
            enemigoGrande.SetActive(false);
        }
    }

    // Método de utilidad para consultar el estado desde otros scripts
    public bool EstaRoto() => estaRoto;
    public int GetVidaActual() => vidaActual;
}