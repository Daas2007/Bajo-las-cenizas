using UnityEngine;

public class CandadoPuzzle : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject candado;      // objeto físico del candado (interactuable)
    [SerializeField] private GameObject fragmento;    // fragmento que aparece al desbloquear
    [SerializeField] private TapaInteractuable tapa;  // referencia a la tapa que debe habilitar interacción

    private bool desbloqueado = false;

    public void Desbloquear()
    {
        if (desbloqueado)
        {
            Debug.Log("[CandadoPuzzle] Ya desbloqueado, ignorando.");
            return;
        }

        desbloqueado = true;
        Debug.Log("[CandadoPuzzle] Desbloqueando puzzle...");

        // 1) Desactivar el objeto candado (ya no debe ser interactuable)
        if (candado != null)
        {
            var interact = candado.GetComponent<CandadoInteractuable>();
            if (interact != null)
            {
                interact.MarcarResuelto();
            }

            candado.SetActive(false);
            Debug.Log("[CandadoPuzzle] Candado ocultado.");
        }
        else
        {
            Debug.LogWarning("[CandadoPuzzle] candado no asignado en el inspector.");
        }

        // 2) Activar el fragmento (recompensa)
        if (fragmento != null)
        {
            fragmento.SetActive(true);
            Debug.Log("[CandadoPuzzle] Fragmento activado.");
        }
        else
        {
            Debug.LogWarning("[CandadoPuzzle] fragmento no asignado en el inspector.");
        }

        // 3) Habilitar la tapa para que pase a layer Interaccion y pueda abrirse
        if (tapa != null)
        {
            tapa.HabilitarInteraccion();
            Debug.Log("[CandadoPuzzle] Tapa: HabilitarInteraccion() llamado.");
        }
        else
        {
            Debug.LogWarning("[CandadoPuzzle] tapa no asignada en el inspector.");
        }
    }

    public bool EstaDesbloqueado()
    {
        return desbloqueado;
    }
}
