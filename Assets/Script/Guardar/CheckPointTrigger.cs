using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] string checkpointID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MovimientoPersonaje mov = other.GetComponent<MovimientoPersonaje>();
            GameManager gm = GameManager.Instancia;

            if (mov != null && gm != null)
            {
                SistemaGuardar.Guardar(mov, gm);
                Debug.Log($"Checkpoint {checkpointID} activado.");
            }
        }
    }
}
