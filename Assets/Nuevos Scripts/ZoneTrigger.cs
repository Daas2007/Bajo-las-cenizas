using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZoneTrigger : MonoBehaviour
{
    [SerializeField] private string idHabitacion = "Habitacion1";
    [SerializeField] private bool detectarSalida = false;

    private void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (string.IsNullOrEmpty(idHabitacion))
        {
            Debug.LogWarning($"ZoneTrigger [{name}] no tiene idHabitacion asignado.");
            return;
        }

        LevelGateManager.Instancia?.EntrarHabitacion(idHabitacion);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!detectarSalida) return;
        if (!other.CompareTag("Player")) return;
        if (string.IsNullOrEmpty(idHabitacion)) return;

        LevelGateManager.Instancia?.SalirHabitacion(idHabitacion);
    }
}
