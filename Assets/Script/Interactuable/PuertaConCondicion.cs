using UnityEngine;

public class PuertaConCondicion : MonoBehaviour, IInteractuable
{
    [Header("Puerta base")]
    [SerializeField] private PuertaInteractuable puertaBase;

    [Header("Condición: NPC con el que hay que hablar primero")]
    [SerializeField] private Dialogo npcDialogo;

    public void Interactuar()
    {
        if (puertaBase == null)
        {
            Debug.LogWarning("[PuertaConCondicion] puertaBase no asignada.");
            return;
        }

        // ✅ Solo abre si el NPC ya habló
        if (npcDialogo != null && npcDialogo.HaHablado)
        {
            puertaBase.Interactuar();
        }
        else
        {
            // No hace nada, simplemente no abre
            Debug.Log("[PuertaConCondicion] El jugador aún no ha hablado con el NPC, la puerta no se abre.");
        }
    }
}
