using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public void EntrarHabitacion(string id)
    {
        var estado = LevelGateManager.Instancia?.GetRoomState(id) ?? LevelGateManager.RoomState.NoEntrado;

        if (estado != LevelGateManager.RoomState.Completado)
        {
            LevelGateManager.Instancia.EntrarHabitacion(id);
        }
        else
        {
            Debug.Log($"🚫 No puedes entrar a {id}, ya completada.");
        }
    }
}
