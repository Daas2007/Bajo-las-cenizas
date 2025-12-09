using UnityEngine;

public class CandadoPuzzle : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject candado;
    [SerializeField] private GameObject puertaCaja;
    [SerializeField] private GameObject fragmento;

    private bool desbloqueado = false;

    public void Desbloquear()
    {
        if (desbloqueado) return;
        desbloqueado = true;

        // Ocultar candado y puerta
        if (candado != null) candado.SetActive(false);
        if (puertaCaja != null) puertaCaja.SetActive(false);

        // Mostrar fragmento
        if (fragmento != null) fragmento.SetActive(true);

        Debug.Log("🔓 Candado desbloqueado. Fragmento revelado.");
    }
}

