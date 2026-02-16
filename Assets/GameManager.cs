using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia;

    // Sistema de piezas del oso
    [Header("Puzzle del oso")]
    public int piezasRecogidas = 0;
    public int piezasNecesarias = 5; // brazos, piernas y cabeza
    public bool osoCompleto = false;
    public System.Action OnOsoCompleto; // evento que dispara el spawn del cristal

    // Linterna
    public bool tieneLinterna = false;

    // Contador de muertes global
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
    // Piezas del oso
    // -------------------
    public void RecogerPieza()
    {
        piezasRecogidas++;
        Debug.Log($"🧩 Pieza del oso recogida: {piezasRecogidas}/{piezasNecesarias}");

        if (!osoCompleto && piezasRecogidas >= piezasNecesarias)
        {
            osoCompleto = true;
            OnOsoCompleto?.Invoke(); // dispara el evento
            Debug.Log("✅ Oso completado, se puede generar el cristal.");
        }
    }

    public void ResetearPuzzleOso()
    {
        piezasRecogidas = 0;
        osoCompleto = false;
    }

    // -------------------
    // Muertes
    // -------------------
    public void RegistrarMuerte()
    {
        muertes++;
        Debug.Log($"☠️ Muertes acumuladas: {muertes}");
    }

    public int GetMuertes()
    {
        return muertes;
    }

    public bool CristalDañado()
    {
        // 🔑 Si el jugador tiene más de 2 muertes, el cristal será dañado
        return muertes > 2;
    }
}
