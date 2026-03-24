using UnityEngine;

public class CandadoPuerta : MonoBehaviour, IInteractuable
{
    [SerializeField] private PuertaCandado puerta;

    public void Interactuar()
    {
        // ✅ Buscar todas las llaves en la escena
        LlaveInteractuable[] llaves = FindObjectsOfType<LlaveInteractuable>();

        foreach (LlaveInteractuable llave in llaves)
        {
            // ✅ Solo funciona si la llave está en la mano y corresponde a este candado
            if (llave.EstaEnMano() && llave.candado == this)
            {
                llave.UsarEnCandado();
                return;
            }
        }

        Debug.Log("🔒 Este candado está cerrado. Necesitas su llave correspondiente.");
    }

    public void DestruirCandado()
    {
        Destroy(gameObject);
        Debug.Log("✅ Candado destruido con la llave.");
        if (puerta != null) puerta.RevisarCandados();
    }
}
