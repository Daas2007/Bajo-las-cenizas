using UnityEngine;

public class OsoPieza : MonoBehaviour, IInteractuable
{
    [SerializeField] private OsoManager osoManager;

    public void Interactuar()
    {
        osoManager.RecogerPieza(this);
        gameObject.SetActive(false);
        Debug.Log("🧩 Pieza del oso recogida mediante interacción.");
    }
}

