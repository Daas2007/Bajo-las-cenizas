using UnityEngine;

public class CandadoPuerta : MonoBehaviour, IInteractuable
{
    [SerializeField] private PuertaCandado puerta;

    public void Interactuar()
    {
        // Buscar si el jugador tiene la llave en la mano
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
        Destroy(gameObject); // destruir candado
        Debug.Log("✅ Candado destruido con la llave.");
    }
}
