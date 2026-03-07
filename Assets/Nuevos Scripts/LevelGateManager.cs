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
            Destroy(gameObject);
            return;
        }
        Instancia = this;

        habitacionesMap.Clear();
        foreach (var r in habitaciones)
        {
            if (string.IsNullOrEmpty(r.id))
            {
                Debug.LogWarning($"[LevelGateManager] Una RoomData no tiene id asignado. Ignorando entrada.");
                continue;
            }

            if (habitacionesMap.ContainsKey(r.id))
            {
                Debug.LogWarning($"[LevelGateManager] id duplicado '{r.id}'. Ignorando duplicado.");
                continue;
            }

            habitacionesMap.Add(r.id, r);

            // Estado inicial coherente
            if (r.puertaLobby != null) r.puertaLobby.Unlock();
            if (r.puertaInterna != null) r.puertaInterna.Lock();
            if (r.muroRetorno != null) r.muroRetorno.SetActive(false);
            if (r.enemyActivator != null) r.enemyActivator.ActivarVentana(false);
            r.estado = RoomState.NoEntrado;
        }
    }

    public void EntrarHabitacion(string id)
    {
        if (!habitacionesMap.TryGetValue(id, out var room))
        {
            Debug.LogWarning($"[LevelGateManager] EntrarHabitacion: id '{id}' no encontrado.");
            return;
        }

        if (room.estado == RoomState.Completado)
        {
            if (room.puertaLobby != null) room.puertaLobby.Lock();
            return;
        }

        // Abrir puerta interna para mostrar hacia dónde ir
        if (room.puertaInterna != null) room.puertaInterna.Unlock();

        // Activar muro de retorno
        if (room.muroRetorno != null) room.muroRetorno.SetActive(true);

        // Activar ventana (enemigo se activará más tarde en estado 3)
        if (room.enemyActivator != null) room.enemyActivator.ActivarVentana(true);

        room.estado = RoomState.EnCurso;
        Debug.Log($"[LevelGateManager] EntrarHabitacion: '{id}' marcado EnCurso. Muro activado.");
    }

    public void SalirHabitacion(string id)
    {
        if (!habitacionesMap.TryGetValue(id, out var room))
        {
            Debug.LogWarning($"[LevelGateManager] SalirHabitacion: id '{id}' no encontrado.");
            return;
        }

        if (room.estado == RoomState.EnCurso)
        {
            // Comportamiento por defecto: no hacemos nada especial aquí
            Debug.Log($"[LevelGateManager] SalirHabitacion: '{id}' jugador salió del trigger (estado EnCurso).");
        }
    }

    public void CompletarHabitacion(string id)
    {
        if (!habitacionesMap.TryGetValue(id, out var room))
        {
            Debug.LogWarning($"[LevelGateManager] CompletarHabitacion: id '{id}' no encontrado.");
            return;
        }

        if (room.estado == RoomState.Completado) return;

        // Abrir puerta interna y quitar muro
        if (room.puertaInterna != null) room.puertaInterna.Unlock();
        if (room.muroRetorno != null) room.muroRetorno.SetActive(false);

        if (room.enemyActivator != null) room.enemyActivator.ActivarVentana(false);

        room.estado = RoomState.Completado;
        Debug.Log($"[LevelGateManager] CompletarHabitacion: '{id}' completada. Muro desactivado.");
    }

    public void CerrarHabitacionYIniciar(string id)
    {
        if (!habitacionesMap.TryGetValue(id, out var room))
        {
            Debug.LogWarning($"[LevelGateManager] CerrarHabitacionYIniciar: id '{id}' no encontrado.");
            return;
        }

        if (room.puertaInterna != null) room.puertaInterna.Lock();

        if (id == "Habitacion1")
        {
            Debug.Log("[LevelGateManager] Iniciando juego en Habitación 1");
            // Aquí puedes llamar a GameManager.Instancia.IniciarNivel1() si lo necesitas
        }
    }

    public void ResetHabitacion(string id)
    {
        if (!habitacionesMap.TryGetValue(id, out var room))
        {
            Debug.LogWarning($"[LevelGateManager] ResetHabitacion: id '{id}' no encontrado.");
            return;
        }

        if (room.puertaLobby != null) room.puertaLobby.Unlock();
        if (room.puertaInterna != null) room.puertaInterna.Lock();
        if (room.muroRetorno != null) room.muroRetorno.SetActive(false);
        if (room.enemyActivator != null) room.enemyActivator.ActivarVentana(false);
        room.estado = RoomState.NoEntrado;
        Debug.Log($"[LevelGateManager] ResetHabitacion: '{id}' reiniciada a NoEntrado.");
    }

    public RoomState GetRoomState(string id)
    {
        if (!habitacionesMap.TryGetValue(id, out var room))
        {
            Debug.LogWarning($"[LevelGateManager] GetRoomState: id '{id}' no encontrado.");
            return RoomState.NoEntrado;
        }
        return room.estado;
    }

    public void ActivarMuroRetorno(string id)
    {
        if (!habitacionesMap.TryGetValue(id, out var room))
        {
            Debug.LogWarning($"[LevelGateManager] ActivarMuroRetorno: id '{id}' no encontrado.");
            return;
        }
        if (room.muroRetorno != null)
        {
            room.muroRetorno.SetActive(true);
            Debug.Log($"[LevelGateManager] Muro de retorno activado en '{id}'.");
        }
    }

    public void DesactivarMuroRetorno(string id)
    {
        if (!habitacionesMap.TryGetValue(id, out var room))
        {
            Debug.LogWarning($"[LevelGateManager] DesactivarMuroRetorno: id '{id}' no encontrado.");
            return;
        }
        if (room.muroRetorno != null)
        {
            room.muroRetorno.SetActive(false);
            Debug.Log($"[LevelGateManager] Muro de retorno desactivado en '{id}'.");
        }
    }

    public void CerrarPuertaLobby(string id)
    {
        if (!habitacionesMap.TryGetValue(id, out var room))
        {
            Debug.LogWarning($"[LevelGateManager] CerrarPuertaLobby: id '{id}' no encontrado.");
            return;
        }
        if (room.puertaLobby != null) room.puertaLobby.Lock();
        Debug.Log($"[LevelGateManager] Puerta lobby cerrada en '{id}'.");
    }

    // -----------------------
    // Métodos adicionales útiles
    // -----------------------

    // Desactiva todos los muros de retorno registrados (útil cuando el jugador recoge el cristal)
    public void DesactivarTodosLosMurosRetorno()
    {
        foreach (var kv in habitacionesMap)
        {
            var room = kv.Value;
            if (room.muroRetorno != null)
            {
                room.muroRetorno.SetActive(false);
            }
        }
        Debug.Log("[LevelGateManager] Todos los muros de retorno desactivados.");
    }

    // Notificar que el jugador recogió el cristal: actualiza PlayerPrefs y desactiva muros relevantes
    public void NotificarCristalRecogidoGlobal()
    {
        PlayerPrefs.SetInt("TieneCristal", 1);
        PlayerPrefs.Save();

        // Desactivar todos los muros (o podrías desactivar solo los de ciertas habitaciones si lo prefieres)
        DesactivarTodosLosMurosRetorno();

        Debug.Log("[LevelGateManager] Notificado: cristal recogido. Muros desactivados.");
    }
}
