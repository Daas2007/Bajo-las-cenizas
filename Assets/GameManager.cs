
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia;

    public int piezasRecogidas = 0;
    public int piezasNecesarias = 5;
    public bool nivelCompletado = false;

    public System.Action OnNivelCompletado;

    public bool tieneLinterna = false;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void TomarLinterna()
    {
        tieneLinterna = true;
    }

    public void RecogerPieza()
    {
        piezasRecogidas++;
        Debug.Log($"🧩 Fragmento recogido: {piezasRecogidas}/{piezasNecesarias}");

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
}
