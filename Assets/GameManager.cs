using UnityEngine;

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

    public void RegistrarMuerte()
    {
        muertes++;
    }

    public int GetMuertes() => muertes;
    public bool CristalDañado() => muertes > 2;

    // ------------------- MÉTODO NUEVO -------------------

    public void ReiniciarEstado()
    {
        // Reiniciar valores básicos
        muertes = 0;
        piezasRecogidas = 0;
        osoCompleto = false;
        puzzle1Completado = false;
        puzzle2Completado = false;
        cristalMetaActivo = false;
        tieneLinterna = false;

        // Reiniciar linterna
        if (linternaEnMano != null) linternaEnMano.SetActive(false);
        if (linternaPickup != null) linternaPickup.SetActive(true);

        // Reiniciar puertas bloqueadas
        if (puertasBloqueadas != null)
        {
            foreach (GameObject puerta in puertasBloqueadas)
            {
                if (puerta != null) puerta.SetActive(true);
            }
        }

        Debug.Log("✅ Estado del juego reiniciado correctamente.");
    }
}
