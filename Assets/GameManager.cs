using UnityEngine;
using System.Collections.Generic;

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

    // Contador de muertes por nivel (en una sola escena basta con un contador global)
    private int muertes = 0;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
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
            Debug.Log("✅ Nivel completado en esta escena");
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
        muertes++;
        Debug.Log($"Muertes: {muertes}");
    }

    public int GetMuertes()
    {
        return muertes;
    }

    public bool CristalDañado()
    {
        return muertes > 2;
    }
}
