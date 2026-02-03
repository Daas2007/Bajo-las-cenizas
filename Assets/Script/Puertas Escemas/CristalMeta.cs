using UnityEngine;

public class CristalMeta : MonoBehaviour, IInteractuable
{
    [SerializeField] string idHabitacion = "Habitacion1";

    public void Interactuar()
    {
        PlayerPrefs.SetInt(idHabitacion + "_completado", 1);
        PlayerPrefs.Save();

        GameManager.Instancia.ResetearProgresoNivel();
        LevelGateManager.Instancia?.CompletarHabitacion(idHabitacion);
        GameManager.Instancia?.MarcarNivelCompletado(idHabitacion);
    }
}
