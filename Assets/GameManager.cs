using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia;

    [Header("Puzzle del oso")]
    public int piezasRecogidas = 0;
    public int piezasNecesarias = 5;
    public bool osoCompleto = false;
    public System.Action OnOsoCompleto;

    private int muertes = 0;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    public void RecogerPieza()
    {
        piezasRecogidas++;
        Debug.Log($"🧩 Pieza recogida: {piezasRecogidas}/{piezasNecesarias}");

        if (!osoCompleto && piezasRecogidas >= piezasNecesarias)
        {
            osoCompleto = true;
            OnOsoCompleto?.Invoke();
            Debug.Log("✅ Oso completado, se puede generar el cristal.");
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
        Debug.Log($"☠️ Muertes acumuladas: {muertes}");
    }

    public int GetMuertes() => muertes;

    public bool CristalDañado() => muertes > 2;
}
