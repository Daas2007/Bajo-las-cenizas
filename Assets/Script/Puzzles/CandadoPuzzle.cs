using UnityEngine;

public class CandadoPuzzle : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject candado;
    [SerializeField] private GameObject fragmento;
    [SerializeField] private TapaInteractuable tapa;

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

        if (candado != null)
        {
            candado.SetActive(false);
            candado.layer = LayerMask.NameToLayer("Default");
            Debug.Log("[CandadoPuzzle] Candado ocultado y layer seteada a Default.");
        }
        else
        {
            Debug.LogWarning("[CandadoPuzzle] candado no asignado en el inspector.");
        }

        if (fragmento != null)
        {
            fragmento.SetActive(true);
            Debug.Log("[CandadoPuzzle] Fragmento activado.");
        }
        else
        {
            Debug.LogWarning("[CandadoPuzzle] fragmento no asignado en el inspector.");
        }

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


