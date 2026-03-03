using UnityEngine;
using System.Collections.Generic;

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

        // Resetear jugador al spawn inicial
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        if (jugador != null && spawnInicial != null)
        {
            jugador.transform.position = spawnInicial.position;
            jugador.transform.rotation = spawnInicial.rotation;

            CharacterController cc = jugador.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                cc.enabled = true;
            }
        }

        // Dar linterna inicial
        if (linternaEnMano != null) linternaEnMano.SetActive(true);
        if (linternaPickup != null) linternaPickup.SetActive(false);
        tieneLinterna = true;

        // Reactivar UI
        GameObject gameplayUI = GameObject.Find("GameplayUI");
        if (gameplayUI != null) gameplayUI.SetActive(true);

        muertes = 0;
        muertesPorHabitacion.Clear();
    }

    // ------------------- CARGAR PARTIDA -------------------
    public void CargarPartida()
    {
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        if (jugador != null)
        {
            SistemaGuardar.Cargar(jugador, this);
        }
    }
}
