using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia;

    [Header("Puzzle del oso")]
    public int piezasRecogidas = 0;
    public int piezasNecesarias = 5;
    public bool osoCompleto = false;
    public System.Action OnOsoCompleto;

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

        // Reset muro
        MuroBloqueo muro = FindObjectOfType<MuroBloqueo>();
        if (muro != null) muro.ResetMuro();

        // Reset puzzles
        ResetearPuzzleOso();
        puzzle1Completado = false;
        puzzle2Completado = false;
        cristalMetaActivo = false;
    }

    // ------------------- NUEVA PARTIDA -------------------
    public void NuevaPartida()
    {
        ReiniciarEstado(); 
        // Borrar archivo de guardado para asegurar inicio limpio
        SistemaGuardar.BorrarArchivo(); 
        // Teleport seguro al spawn inicial (maneja Rigidbody/CharacterController y el script de movimiento)
        TeleportarASpawnInicial(); 
        // Linterna inicial
         if (linternaEnMano != null) linternaEnMano.SetActive(false); if (linternaPickup != null) linternaPickup.SetActive(true); tieneLinterna = false; 
        // Reactivar UI
        GameObject gameplayUI = GameObject.Find("GameplayUI");
        if (gameplayUI != null) gameplayUI.SetActive(true);
        muertes = 0;
        muertesPorHabitacion.Clear();
        Debug.Log("[GameManager] NuevaPartida: inicio limpio aplicado."); }
    //-------------------- SPAWN INICIAL---------------------
    // GameManager.cs (añadir dentro de la clase)
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

        // Desactivar temporalmente el script de movimiento para evitar que sobrescriba la posición
        var movimientoScript = jugador.GetComponent<MovimientoPersonaje>();
        if (movimientoScript != null) movimientoScript.enabled = false;

        // Si tiene CharacterController, desactivarlo temporalmente
        var cc = jugador.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Si tiene Rigidbody, ponerlo en kinematic y limpiar velocidades
        var rb = jugador.GetComponent<Rigidbody>();
        bool rbWasKinematic = false;
        if (rb != null)
        {
            rbWasKinematic = rb.isKinematic;
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Teleportar
        jugador.transform.position = spawnInicial.position;
        jugador.transform.rotation = spawnInicial.rotation;

        // Restaurar Rigidbody/CharacterController y script
        if (rb != null) rb.isKinematic = rbWasKinematic;
        if (cc != null) cc.enabled = true;
        if (movimientoScript != null) movimientoScript.enabled = true;

        Debug.Log($"[GameManager] Jugador teletransportado a spawnInicial {spawnInicial.position}");
    }
    // ------------------- CARGAR PARTIDA -------------------
    public void CargarPartida()
    {
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        if (jugador != null)
        {
            SistemaGuardar.Cargar(jugador, this);

            // 🔧 Reset enemigos y ventanas al cargar
            foreach (EnemigoPerseguidor enemigo in FindObjectsOfType<EnemigoPerseguidor>())
                enemigo.ResetEnemigo();

            foreach (EnemigoVentana ventana in FindObjectsOfType<EnemigoVentana>())
                ventana.ResetVentana();
        }
    }
    public void RecogerCrital()
    {
        MuroBloqueo muro = FindAnyObjectByType<MuroBloqueo>();
        muro.QuitarMuro();

    } 
}
