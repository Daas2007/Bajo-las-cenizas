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

    // 🔑 Niveles completados
    private HashSet<string> nivelesCompletados = new HashSet<string>();

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
    // Piezas
    // -------------------
    public void RecogerPieza()
    {
        piezasRecogidas++;
        Debug.Log($"Fragmento recogido: {piezasRecogidas}/{piezasNecesarias}");

        if (!nivelCompletado && piezasRecogidas >= piezasNecesarias)
        {
            nivelCompletado = true;
            OnNivelCompletado?.Invoke();

            // 🔑 Marcar nivel actual como completado
            string nivelActual = SceneManager.GetActiveScene().name;
            MarcarNivelCompletado(nivelActual);
        }
    }

    public void ResetearProgresoNivel()
    {
        piezasRecogidas = 0;
        nivelCompletado = false;
    }

    // -------------------
    // Muertes
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

    // -------------------
    // 🔑 Bloqueo de niveles
    // -------------------
    public void MarcarNivelCompletado(string nombreNivel)
    {
        if (!nivelesCompletados.Contains(nombreNivel))
        {
            nivelesCompletados.Add(nombreNivel);
            Debug.Log($"✅ Nivel completado: {nombreNivel}");
        }
    }

    public bool EstaNivelCompletado(string nombreNivel)
    {
        return nivelesCompletados.Contains(nombreNivel);
    }
}