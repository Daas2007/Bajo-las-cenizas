using UnityEngine;

public class PickupPiezaFaltante : MonoBehaviour, IInteractuable
{
    //---------------Interacción---------------
    public void Interactuar()
    {
        if (GestorRompecabezas.Instancia != null)
            GestorRompecabezas.Instancia.ActivarPiezaFaltante();

        Destroy(gameObject);
    }
}