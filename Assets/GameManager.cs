using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia;

    // Sistema de piezas
    public int piezasRecogidas = 0;
    public int piezasNecesarias = 5;
    public bool nivelCompletado = false;
    public System.Action OnNivelCompletado;

    // Linterna
    public bool tieneLinterna = false;

    // Contador de muertes por nivel
    private Dictionary<string, int> muertesPorNivel = new Dictionary<string, int>();

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // -------------------
    // Métodos existentes
    // -------------------
    public void TomarLinterna()
    {
        tieneLinterna = true;
    }

    public void RecogerPieza()
    {
        piezasRecogidas++;
        Debug.Log($"Fragmento recogido: {piezasRecogidas}/{piezasNecesarias}");

        if (!nivelCompletado && piezasRecogidas >= piezasNecesarias)
        {
            nivelCompletado = true;
            OnNivelCompletado?.Invoke();
        }
    }

    public void ResetearProgresoNivel()
    {
        piezasRecogidas = 0;
        nivelCompletado = false;
    }

    // -------------------
    //  Muertes
    // -------------------
    public void RegistrarMuerte()
    {
        string nivelActual = SceneManager.GetActiveScene().name;

        if (!muertesPorNivel.ContainsKey(nivelActual))
            muertesPorNivel[nivelActual] = 0;

        muertesPorNivel[nivelActual]++;
        Debug.Log($"Muertes en {nivelActual}: {muertesPorNivel[nivelActual]}");
    }

    public int GetMuertesNivelActual()
    {
        string nivelActual = SceneManager.GetActiveScene().name;
        return muertesPorNivel.ContainsKey(nivelActual) ? muertesPorNivel[nivelActual] : 0;
    }

    public bool CristalDañadoNivelActual()
    {
        return GetMuertesNivelActual() > 2;
    }
}
