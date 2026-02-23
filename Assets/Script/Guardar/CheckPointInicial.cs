using UnityEngine;

public class CheckpointInicial : MonoBehaviour
{
    void Start()
    {
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        GameManager gm = GameManager.Instancia;

        if (jugador != null && gm != null)
        {
            // Guardar automáticamente al inicio
            SistemaGuardar.Guardar(jugador, gm);
            Debug.Log("✅ Checkpoint inicial guardado.");
        }
    }
}
