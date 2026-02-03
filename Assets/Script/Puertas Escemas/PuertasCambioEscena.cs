using UnityEngine;

public class PuertaCambioEscena : MonoBehaviour, IInteractuable
{
    [SerializeField] private string idHabitacion = "Habitacion1";

    public void Interactuar()
    {
        var manager = LevelGateManager.Instancia;
        if (manager == null)
        {
            Debug.LogWarning("LevelGateManager no está inicializado.");
            return;
        }

        var estado = manager.GetRoomState(idHabitacion);
        if (estado != LevelGateManager.RoomState.Completado)
        {
            manager.EntrarHabitacion(idHabitacion);
        }
        else
        {
            Debug.Log($"🚫 No puedes entrar a {idHabitacion}, ya completada.");
        }
    }
}
