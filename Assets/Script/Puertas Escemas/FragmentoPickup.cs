using UnityEngine;

public class FragmentoPickup : MonoBehaviour, IInteractuable
{
    public void Interactuar()
    {
        GameManager.Instancia.RecogerPieza();
        Destroy(gameObject);
    }
}

