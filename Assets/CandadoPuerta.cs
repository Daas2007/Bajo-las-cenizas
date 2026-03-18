using UnityEngine;

public class CandadoPuerta : MonoBehaviour, IInteractuable
{
    [Header("Referencia a la puerta")]
    [SerializeField] private PuertaCandado puerta;

    private bool desbloqueado = false;

    public void Interactuar()
    {
        if (!desbloqueado)
        {
            Debug.Log("🔒 El candado está cerrado. Necesitas una llave.");
        }
        else
        {
            Debug.Log("🔓 El candado ya está abierto.");
        }
    }

    // ✅ Método que la llave llamará
    public void UsarLlave()
    {
        desbloqueado = true;
        if (puerta != null)
        {
            puerta.DesbloquearPuerta();
        }
        Debug.Log("✅ Candado abierto con la llave.");
    }
}
