using UnityEngine;

public class CristalMeta : MonoBehaviour
{
    [SerializeField] private string idHabitacion;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        LevelGateManager.Instancia.CompletarHabitacion(idHabitacion);
        Destroy(gameObject); // eliminar cristal
    }
}

