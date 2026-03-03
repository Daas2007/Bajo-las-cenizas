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

            // 🔧 Asegurar que el spawn inicial quede registrado
            gm.spawnInicial = jugador.transform;

            Debug.Log("✅ Checkpoint inicial guardado y spawn registrado.");
        }
    }
}