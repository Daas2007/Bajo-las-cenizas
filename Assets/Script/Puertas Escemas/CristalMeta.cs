using UnityEngine;

public class CristalMeta : MonoBehaviour
{
    [SerializeField] private string idHabitacion;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        LevelGateManager.Instancia?.CompletarHabitacion(idHabitacion);

        if (GameManager.Instancia != null)
        {
            GameManager.Instancia.NotifyCrystalCollected();
            GameManager.Instancia.RecogerCristal(); // opcional: lógica global al recoger
        }

        Destroy(gameObject);
    }
}
