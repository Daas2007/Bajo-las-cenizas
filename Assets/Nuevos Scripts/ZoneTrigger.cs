using UnityEngine;
using System.Collections.Generic;

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

    [Header("Objetos del Lobby")]
    [Tooltip("Todos los objetos que deben estar activos en el lobby")]
    [SerializeField] private List<GameObject> lobbyObjects = new List<GameObject>();

    [Header("Objetos del Escenario Alternativo")]
    [Tooltip("Todos los objetos que deben estar activos en el escenario alternativo")]
    [SerializeField] private List<GameObject> alternateObjects = new List<GameObject>();

    private bool hasSwapped = false;
    private bool estadoHasCrystal = false;

    private void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    private void Start()
    {
        // Estado inicial: lobby ON, alternate OFF
        SetActiveList(lobbyObjects, true);
        SetActiveList(alternateObjects, false);

        estadoHasCrystal = GameManager.Instancia != null && GameManager.Instancia.HasCrystal;

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
        if (!other.CompareTag("Player")) return;

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
        if (GameManager.Instancia == null) return;

        bool hasCrystalNow = GameManager.Instancia.HasCrystal;

        // Caso 1: no tiene cristal y aún no hemos hecho swap -> activar alternate
        if (!hasCrystalNow && !hasSwapped)
        {
            SetActiveList(lobbyObjects, false);
            SetActiveList(alternateObjects, true);
            hasSwapped = true;
            estadoHasCrystal = false;
            Debug.Log($"[ZoneTrigger] Swap aplicado: jugador SIN cristal -> lobby OFF, alternate ON ({name})");
            return;
        }

        // Caso 2: tiene cristal y previamente hicimos swap -> volver a lobby
        if (hasCrystalNow && hasSwapped)
        {
            SetActiveList(lobbyObjects, true);
            SetActiveList(alternateObjects, false);
            hasSwapped = false;
            estadoHasCrystal = true;
            Debug.Log($"[ZoneTrigger] Swap revertido: jugador CON cristal -> lobby ON, alternate OFF ({name})");
            return;
        }
    }

    public void ForzarActualizarEstado()
    {
        if (tipo != TipoTrigger.IntercambiarHabitacion) return;
        HandleIntercambio();
    }

    private void OnCrystalCollected()
    {
        estadoHasCrystal = true;
        Debug.Log($"[ZoneTrigger] OnCrystalCollected: estado interno actualizado a true ({name}).");
    }

    // 🔹 Helper para activar/desactivar listas
    private void SetActiveList(List<GameObject> lista, bool estado)
    {
        foreach (var obj in lista)
        {
            if (obj != null) obj.SetActive(estado);
        }
    }
}
