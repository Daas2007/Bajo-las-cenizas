using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGateManager : MonoBehaviour
{
    public static LevelGateManager Instancia { get; private set; }

    public enum RoomState { NoEntrado, EnCurso, Completado }

    [Serializable]
    public class RoomData
    {
        public string id;
        [Header("Puertas")]
        public DoorLock puertaLobby;
        public DoorLock puertaInterna;
        [Header("Bloqueadores y enemigos")]
        public GameObject muroRetorno;
        public EnemyActivator enemyActivator;
        [Header("Estado (solo lectura en runtime)")]
        public RoomState estado = RoomState.NoEntrado;
    }

    [Header("Configuración de habitaciones")]
    [SerializeField] private List<RoomData> habitaciones = new List<RoomData>();

    private Dictionary<string, RoomData> habitacionesMap = new Dictionary<string, RoomData>(StringComparer.OrdinalIgnoreCase);

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Debug.LogWarning("LevelGateManager: ya existe una instancia. Destruyendo esta.");
            Destroy(gameObject);
            return;
        }
        Instancia = this;

        habitacionesMap.Clear();
        foreach (var r in habitaciones)
        {
            if (string.IsNullOrEmpty(r.id))
            {
                Debug.LogWarning($"LevelGateManager: Una RoomData no tiene id asignado (GameObject: {name}).");
                continue;
            }

            if (habitacionesMap.ContainsKey(r.id))
            {
                Debug.LogWarning($"LevelGateManager: id duplicado '{r.id}'. Ignorando duplicado.");
                continue;
            }

            habitacionesMap.Add(r.id, r);

            // Estado inicial coherente
            if (r.puertaLobby != null) r.puertaLobby.Unlock();
            if (r.puertaInterna != null) r.puertaInterna.Lock();
            if (r.muroRetorno != null) r.muroRetorno.SetActive(false);
            if (r.enemyActivator != null) r.enemyActivator.Activar(false);
            r.estado = RoomState.NoEntrado;
        }
    }

    public void EntrarHabitacion(string id)
    {
        if (!habitacionesMap.TryGetValue(id, out var room))
        {
            Debug.LogWarning($"LevelGateManager.EntrarHabitacion: id '{id}' no encontrado.");
            return;
        }

        if (room.estado == RoomState.Completado)
        {
            if (room.puertaLobby != null) room.puertaLobby.Lock();
            Debug.Log($"EntrarHabitacion: '{id}' ya completada. No se reabre el acceso desde lobby.");
            return;
        }

        if (room.puertaLobby != null) room.puertaLobby.Lock();
        if (room.muroRetorno != null) room.muroRetorno.SetActive(true);
        if (room.enemyActivator != null) room.enemyActivator.Activar(true);

        room.estado = RoomState.EnCurso;
        Debug.Log($"EntrarHabitacion: '{id}' estado -> EnCurso");
    }

    public void SalirHabitacion(string id)
    {
        if (!habitacionesMap.TryGetValue(id, out var room))
        {
            Debug.LogWarning($"LevelGateManager.SalirHabitacion: id '{id}' no encontrado.");
            return;
        }

        if (room.estado == RoomState.EnCurso)
        {
            if (room.enemyActivator != null) room.enemyActivator.OnPlayerLeft();
            Debug.Log($"SalirHabitacion: '{id}' jugador salió del trigger (estado EnCurso).");
        }
    }

    public void CompletarHabitacion(string id)
    {
        if (!habitacionesMap.TryGetValue(id, out var room))
        {
            Debug.LogWarning($"LevelGateManager.CompletarHabitacion: id '{id}' no encontrado.");
            return;
        }

        if (room.estado == RoomState.Completado)
        {
            Debug.Log($"CompletarHabitacion: '{id}' ya estaba completada.");
            return;
        }

        if (room.puertaInterna != null) room.puertaInterna.Unlock();
        if (room.muroRetorno != null) room.muroRetorno.SetActive(false);
        if (room.enemyActivator != null) room.enemyActivator.Activar(false);

        room.estado = RoomState.Completado;
        Debug.Log($"CompletarHabitacion: '{id}' estado -> Completado");
    }

    public void ResetHabitacion(string id)
    {
        if (!habitacionesMap.TryGetValue(id, out var room))
        {
            Debug.LogWarning($"LevelGateManager.ResetHabitacion: id '{id}' no encontrado.");
            return;
        }

        if (room.puertaLobby != null) room.puertaLobby.Unlock();
        if (room.puertaInterna != null) room.puertaInterna.Lock();
        if (room.muroRetorno != null) room.muroRetorno.SetActive(false);
        if (room.enemyActivator != null) room.enemyActivator.Activar(false);
        room.estado = RoomState.NoEntrado;
        Debug.Log($"ResetHabitacion: '{id}' reiniciada a NoEntrado.");
    }

    public RoomState GetRoomState(string id)
    {
        if (!habitacionesMap.TryGetValue(id, out var room))
        {
            Debug.LogWarning($"LevelGateManager.GetRoomState: id '{id}' no encontrado.");
            return RoomState.NoEntrado;
        }
        return room.estado;
    }
}
