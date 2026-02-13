using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZoneTrigger : MonoBehaviour
{
    public enum TipoTrigger
    {
        ActivarMuro,   // Trigger colocado ANTES de entrar al cuarto
        CerrarPuerta   // Trigger colocado DENTRO del cuarto
    }

    [SerializeField] private string idHabitacion = "Habitacion1";
    [SerializeField] private TipoTrigger tipo = TipoTrigger.ActivarMuro;

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

        switch (tipo)
        {
            case TipoTrigger.ActivarMuro:
                LevelGateManager.Instancia?.ActivarMuroRetorno(idHabitacion);
                break;

            case TipoTrigger.CerrarPuerta:
                LevelGateManager.Instancia?.CerrarPuertaLobby(idHabitacion);
                LevelGateManager.Instancia?.EntrarHabitacion(idHabitacion);
                break;
        }
    }
}
