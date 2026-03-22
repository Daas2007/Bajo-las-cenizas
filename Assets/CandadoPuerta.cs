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
            Debug.Log("🔒 El candado está cerrado. Necesitas una llave.");
        }
    }

    public void DestruirCandado()
    {
        if (puerta != null) puerta.DesbloquearPuerta();
        Destroy(gameObject);
        Debug.Log("✅ Candado destruido con la llave.");
    }
}
