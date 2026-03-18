using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia;

    [Header("Puzzle del oso")]
    public int piezasRecogidas = 0;
    public int piezasNecesarias = 5;
    public bool osoCompleto = false;
    public Action OnOsoCompleto;

    [Header("Objetos de la escena")]
    public GameObject linternaEnMano;
    public GameObject linternaPickup;

    [Header("Puertas bloqueadas")]
    public GameObject[] puertasBloqueadas;

    [Header("CheckPoint")]
    public Transform spawnInicial;

    [Header("Estado del juego")]
    public int muertes = 0;
    public Dictionary<string, int> muertesPorHabitacion = new Dictionary<string, int>();
    public bool puzzle1Completado = false;
    public bool puzzle2Completado = false;
    public bool cristalMetaActivo = false;

    [Header("Referencias")]
    public GameObject enemigo;
    public bool tieneLinterna = false;

    // -----------------------
    // Estado de sesión: si el jugador tiene el cristal en la partida actual.
    // NO se persiste en PlayerPrefs ni en disco.
    // -----------------------
    public bool HasCrystal { get; private set; } = false;

    // Evento para notificar a otros sistemas que se recogió el cristal
    public event Action OnCrystalCollected;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }
    // ------------------- MÉTODOS DE JUEGO -------------------
    public void RecogerPieza()
    {
        piezasRecogidas++;
        if (!osoCompleto && piezasRecogidas >= piezasNecesarias)
        {
            osoCompleto = true;
            OnOsoCompleto?.Invoke();
        }
    }
    public void ResetearPuzzleOso()
    {
        piezasRecogidas = 0;
        osoCompleto = false;
    }
    public void RegistrarMuerte(string habitacion = "global")
    {
        muertes++;
        if (!muertesPorHabitacion.ContainsKey(habitacion))
            muertesPorHabitacion[habitacion] = 0;
        muertesPorHabitacion[habitacion]++;
    }
    public int GetMuertes() => muertes;
    public bool CristalDañado(string habitacion = "global")
    {
        if (!muertesPorHabitacion.ContainsKey(habitacion)) return false;
        return muertesPorHabitacion[habitacion] > 2;
    }
    // ------------------- REINICIO -------------------
    public void ReiniciarEstado()
    {
        // Reset linterna
        JugadorLinterna linterna = FindObjectOfType<JugadorLinterna>();
        if (linterna != null) linterna.ResetLinterna();

        if (linternaPickup != null) linternaPickup.SetActive(true);
        if (linternaEnMano != null) linternaEnMano.SetActive(false);
        tieneLinterna = false;

        // Reset puertas
        foreach (PuertaTutorial puerta in FindObjectsOfType<PuertaTutorial>())
            puerta.ResetPuerta();

        // Reset enemigos
        foreach (EnemigoPerseguidor enemigo in FindObjectsOfType<EnemigoPerseguidor>())
            enemigo.ResetEnemigo();

        // Reset ventanas
        foreach (EnemigoVentana ventana in FindObjectsOfType<EnemigoVentana>())
            ventana.ResetVentana();

        // Reset muros
        foreach (MuroBloqueo muro in FindObjectsOfType<MuroBloqueo>())
            muro.ResetMuro();

        // Reset puzzles
        ResetearPuzzleOso();
        puzzle1Completado = false;
        puzzle2Completado = false;
        cristalMetaActivo = false;

        // Reset cristal de sesión (no persistente)
        HasCrystal = false;
    }
    // ------------------- NUEVA PARTIDA -------------------
    public void NuevaPartida()
    {
        ReiniciarEstado();
        // Borrar archivo de guardado para asegurar inicio limpio
        SistemaGuardar.BorrarArchivo();
        // Teleport seguro al spawn inicial
        TeleportarASpawnInicial();
        // Linterna inicial
        if (linternaEnMano != null) linternaEnMano.SetActive(false);
        if (linternaPickup != null) linternaPickup.SetActive(true);
        tieneLinterna = false;
        // Reactivar UI
        GameObject gameplayUI = GameObject.Find("GameplayUI");
        if (gameplayUI != null) gameplayUI.SetActive(true);
        muertes = 0;
        muertesPorHabitacion.Clear();
        Debug.Log("[GameManager] NuevaPartida: inicio limpio aplicado.");
    }
    //-------------------- SPAWN INICIAL---------------------
    public void TeleportarASpawnInicial()
    {
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        if (jugador == null)
        {
            Debug.LogWarning("[GameManager] TeleportarASpawnInicial: no se encontró jugador.");
            return;
        }
        if (spawnInicial == null)
        {
            Debug.LogWarning("[GameManager] TeleportarASpawnInicial: spawnInicial no asignado.");
            return;
        }

        var movimientoScript = jugador.GetComponent<MovimientoPersonaje>();
        if (movimientoScript != null) movimientoScript.enabled = false;

        var cc = jugador.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        var rb = jugador.GetComponent<Rigidbody>();
        bool rbWasKinematic = false;
        if (rb != null)
        {
            rbWasKinematic = rb.isKinematic;
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        jugador.transform.position = spawnInicial.position;
        jugador.transform.rotation = spawnInicial.rotation;

        if (rb != null) rb.isKinematic = rbWasKinematic;
        if (cc != null) cc.enabled = true;
        if (movimientoScript != null) movimientoScript.enabled = true;

        Debug.Log($"[GameManager] Jugador teletransportado a spawnInicial {spawnInicial.position}");
    }
    // ------------------- GUARDAR PARTIDA -------------------
    public void GuardarPartida()
    {
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        if (jugador != null)
        {
            SistemaGuardar.Guardar(jugador, this);
            CanvasController cv = FindAnyObjectByType<CanvasController>();
            cv.Reanudar();
            Debug.Log("💾 Partida guardada correctamente.");
        }
        else
        {
            Debug.LogWarning("⚠️ No se pudo guardar: jugador no encontrado.");
        }
    }
    // ------------------- REINTENTAR DESDE GUARDADO -------------------
    public void ReintentarDesdeGuardado()
    {
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        if (jugador != null)
        {
            bool cargado = SistemaGuardar.Cargar(jugador, this);
            if (!cargado)
            {
                // Si no hay guardado, respawn inicial
                ReiniciarEstado();
                TeleportarASpawnInicial();
                Debug.Log("🔄 No había guardado, respawn en spawn inicial.");
            }
            else
            {
                Debug.Log("📂 Partida cargada correctamente desde guardado.");
            }

            // Reactivar jugador y cámara
            jugador.enabled = true;
            Camera cam = jugador.GetComponentInChildren<Camera>();
            if (cam != null) cam.enabled = true;

            // Restaurar tiempo y controles
            CanvasController cv = FindAnyObjectByType<CanvasController>(); 
            cv.Reanudar();
        }
        else
        {
            Debug.LogWarning("⚠️ No se pudo reintentar: jugador no encontrado.");
        }
    }
    public void NotifyCrystalCollected()
    {
        if (HasCrystal) return;
        HasCrystal = true;
        OnCrystalCollected?.Invoke();
        Debug.Log("[GameManager] NotifyCrystalCollected: cristal recogido (estado en memoria).");
    }
    public void RecogerCristal()
    {
        NotifyCrystalCollected();

        MuroBloqueo[] muros = FindObjectsOfType<MuroBloqueo>();
        foreach (var muro in muros)
        {
            if (muro != null)
                muro.QuitarMuro();
        }

        Debug.Log("[GameManager] RecogerCristal: muros quitados en la escena.");
    }
}