using UnityEngine;

public class CandadoPuerta : MonoBehaviour, IInteractuable
{
    [SerializeField] private PuertaCandado puerta;

    public void Interactuar()
    {
        LlaveInteractuable llave = FindObjectOfType<LlaveInteractuable>();
        if (llave != null && llave.EstaEnMano())
        {
            llave.UsarEnCandado();
        }
        else
        {
            Debug.Log("🔒 Este candado está cerrado. Necesitas su llave.");
        }
    }

    public void DestruirCandado()
    {
        Destroy(gameObject);
        Debug.Log("✅ Candado destruido con la llave.");
        if (puerta != null) puerta.RevisarCandados();
    }
}
