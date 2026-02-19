// PickupLinterna.cs
using UnityEngine;

public class PickupLinterna : MonoBehaviour, IInteractuable
{
    public void Interactuar()
    {
        JugadorLinterna jugadorLinterna = FindObjectOfType<JugadorLinterna>();
        if (jugadorLinterna != null)
        {
            jugadorLinterna.DarLinterna();
            Debug.Log("Linterna recogida ✅");
        }
        else
        {
            Debug.LogError("⚠ No se encontró JugadorLinterna en la escena");
        }

        // Desactivar linterna en la escena
        gameObject.SetActive(false);
    }
}
