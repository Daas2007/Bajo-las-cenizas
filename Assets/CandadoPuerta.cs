using UnityEngine;

public class CandadoPuerta : MonoBehaviour, IInteractuable
{
    [SerializeField] private PuertaCandado puerta;

    public void Interactuar()
    {
        LlaveInteractuable[] llaves = FindObjectsOfType<LlaveInteractuable>();

        foreach (LlaveInteractuable llave in llaves)
        {
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
        PuertaCandado puertaCandado = puerta;

        Destroy(gameObject);
        Debug.Log("✅ Candado destruido con la llave.");

        if (puertaCandado != null)
        {
            puertaCandado.NotificarCandadoDestruido(this);
        }
    }
}
