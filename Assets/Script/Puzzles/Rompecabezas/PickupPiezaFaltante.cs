// PickupPiezaFaltante.cs
using UnityEngine;

public class PickupPiezaFaltante : MonoBehaviour, IInteractuable
{
    public void Interactuar()
    {
        if (GestorRompecabezas.Instancia != null)
        {
            GestorRompecabezas.Instancia.ActivarPiezaFaltante();
        }
        Destroy(gameObject);
    }
}
