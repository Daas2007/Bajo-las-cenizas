using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZoneTrigger : MonoBehaviour
{
    public enum TipoTrigger
    {
        IntercambiarHabitacion,
        CerrarPuerta
    }

    [SerializeField] private string idHabitacion = "Habitacion1";
    [SerializeField] private TipoTrigger tipo = TipoTrigger.IntercambiarHabitacion;

    [Header("Habitaciones (asignar en Inspector)")]
    [Tooltip("Habitación 'lobby' que debe estar activa al inicio de la escena")]
    [SerializeField] private GameObject lobbyRoom;
    [Tooltip("Habitación alternativa que se activa cuando NO hay cristal y se atraviesa el trigger")]
    [SerializeField] private GameObject alternateRoom;

    // Indica si ya se aplicó el intercambio (alternate activo)
    private bool hasSwapped = false;

    // Estado interno para saber si GameManager tiene cristal (se actualiza con el evento)
    private bool estadoHasCrystal = false;

    private void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    private void Start()
    {
        // Forzar estado visual inicial: lobby ON, alternate OFF
        if (lobbyRoom != null) lobbyRoom.SetActive(true);
        if (alternateRoom != null) alternateRoom.SetActive(false);

        // Inicializar estadoHasCrystal según GameManager (si existe)
        estadoHasCrystal = GameManager.Instancia != null && GameManager.Instancia.HasCrystal;

        // Suscribirse al evento para actualizar solo el estado interno si se recoge el cristal
        if (GameManager.Instancia != null)
            GameManager.Instancia.OnCrystalCollected += OnCrystalCollected;

        Debug.Log($"[ZoneTrigger Start] {name} inicializado. estadoHasCrystal={estadoHasCrystal}, hasSwapped={hasSwapped}");
    }

    private void OnDestroy()
    {
        if (GameManager.Instancia != null)
            GameManager.Instancia.OnCrystalCollected -= OnCrystalCollected;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ZoneTrigger] OnTriggerEnter en {name} por {other.name} (tag={other.tag})");

        if (!other.CompareTag("Player"))
        {
            Debug.Log($"[ZoneTrigger] Ignorado: collider no es Player ({other.tag})");
            return;
        }

        switch (tipo)
        {
            case TipoTrigger.IntercambiarHabitacion:
                HandleIntercambio();
                break;

            case TipoTrigger.CerrarPuerta:
                LevelGateManager.Instancia?.CerrarPuertaLobby(idHabitacion);
                LevelGateManager.Instancia?.EntrarHabitacion(idHabitacion);

                var puerta = Object.FindFirstObjectByType<PuertaInteractuable>();
                if (puerta != null) puerta.CerrarSiCristal();
                break;
        }
    }

    private void HandleIntercambio()
    {
        if (GameManager.Instancia == null)
        {
            Debug.LogWarning($"[ZoneTrigger] GameManager.Instancia es null en {name}. No se puede determinar estado del cristal.");
            return;
        }

        bool hasCrystalNow = GameManager.Instancia.HasCrystal;
        Debug.Log($"[ZoneTrigger] HandleIntercambio en {name}: hasCrystalNow={hasCrystalNow}, hasSwapped={hasSwapped}");

        // Caso 1: no tiene cristal y aún no hemos hecho swap -> activar alternate
        if (!hasCrystalNow && !hasSwapped)
        {
            if (lobbyRoom == null || alternateRoom == null)
            {
                Debug.LogWarning($"[ZoneTrigger] lobbyRoom o alternateRoom NO asignadas en {name}.");
                return;
            }

            lobbyRoom.SetActive(false);
            alternateRoom.SetActive(true);
            hasSwapped = true;
            estadoHasCrystal = false;
            Debug.Log($"[ZoneTrigger] Swap aplicado: jugador SIN cristal -> lobby OFF, alternate ON ({name})");
            return;
        }

        // Caso 2: tiene cristal y previamente hicimos swap -> volver a lobby
        if (hasCrystalNow && hasSwapped)
        {
            if (lobbyRoom == null || alternateRoom == null)
            {
                Debug.LogWarning($"[ZoneTrigger] lobbyRoom o alternateRoom NO asignadas en {name}.");
                return;
            }

            lobbyRoom.SetActive(true);
            alternateRoom.SetActive(false);
            hasSwapped = false;
            estadoHasCrystal = true;
            Debug.Log($"[ZoneTrigger] Swap revertido: jugador CON cristal -> lobby ON, alternate OFF ({name})");
            return;
        }

        // Si llegamos aquí, no hay cambio necesario
        Debug.Log($"[ZoneTrigger] No se requiere cambio en {name} (hasCrystalNow={hasCrystalNow}, hasSwapped={hasSwapped}).");
    }

    // Método público para forzar actualización desde otros scripts (opcional)
    public void ForzarActualizarEstado()
    {
        if (tipo != TipoTrigger.IntercambiarHabitacion) return;
        // No aplicamos visualmente aquí; ForzarActualizarEstado puede usarse para forzar la lógica
        HandleIntercambio();
    }

    // Callback cuando GameManager notifica recolección del cristal.
    // Actualiza el estado interno para que la próxima vez que atravieses el trigger se aplique la reversión.
    private void OnCrystalCollected()
    {
        estadoHasCrystal = true;
        Debug.Log($"[ZoneTrigger] OnCrystalCollected: estado interno actualizado a true ({name}). El swap se aplicará/revertirá al atravesar el trigger.");
    }
}
