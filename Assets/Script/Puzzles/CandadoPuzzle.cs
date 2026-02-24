using UnityEngine;

public class CandadoPuzzle : MonoBehaviour
{
    //---------------Referencias---------------
    [Header("Referencias")]
    [SerializeField] private GameObject candado;    // objeto candado
    [SerializeField] private GameObject puertaCaja; // puerta/caja que se abre
    [SerializeField] private GameObject fragmento;  // fragmento que aparece

    private bool desbloqueado = false;

    //---------------Desbloquear puzzle---------------
    public void Desbloquear()
    {
        if (desbloqueado) return;
        desbloqueado = true;

        if (candado != null) candado.SetActive(false);
        if (puertaCaja != null) puertaCaja.SetActive(false);
        if (fragmento != null) fragmento.SetActive(true);

        Debug.Log("🔓 Candado desbloqueado. Fragmento revelado.");
    }
}