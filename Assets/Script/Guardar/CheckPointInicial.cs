using UnityEngine;

public class CheckpointInicial : MonoBehaviour
{
    void Start()
    {
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        GameManager gm = GameManager.Instancia;

        if (jugador != null && gm != null)
        {
            gm.spawnInicial = this.transform; // usar el transform del objeto spawn
            Debug.Log($"[CheckpointInicial] Spawn inicial registrado en {transform.position}");
        }
        else
        {
            Debug.LogWarning("[CheckpointInicial] No se encontró jugador o GameManager.");
        }
    }
}
