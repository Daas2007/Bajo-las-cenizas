using UnityEngine;

public class CandadoPuzzle : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject candado;    // objeto candado
    [SerializeField] private GameObject fragmento;  // fragmento que aparece
    [SerializeField] private TapaInteractuable tapa; // referencia al script de la tapa

    private bool desbloqueado = false;

    //---------------Desbloquear puzzle---------------
    public void Desbloquear()
    {
        if (desbloqueado) return;
        desbloqueado = true;

        // Ocultar el candado
        if (candado != null) candado.SetActive(false);

        // Mostrar el fragmento
        if (fragmento != null) fragmento.SetActive(true);

        // Avisar a la tapa que ya puede interactuar
        if (tapa != null) tapa.HabilitarInteraccion();

        Debug.Log("🔓 Candado desbloqueado. Fragmento revelado. La tapa ahora puede abrirse.");
    }

    //---------------Estado del candado---------------
    public bool EstaDesbloqueado()
    {
        return desbloqueado;
    }
}