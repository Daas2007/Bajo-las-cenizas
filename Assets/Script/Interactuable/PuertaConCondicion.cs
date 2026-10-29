
using UnityEngine;

public class PuertaConCondicion : MonoBehaviour, IInteractuable
{
    [Header("Puerta que se debe abrir")]
    [SerializeField] PuertaInteractuable puertaBase;

    [Header("NPC requerido")]
    [SerializeField] DialogoInteractuable npcDialogo;

    // Evita consultar continuamente si la puerta ya se desbloqueo.
    bool puertaDesbloqueada;

    private void Start()
    {
        ComprobarDesbloqueo();
    }

    private void Update()
    {
        // Comprueba si el NPC ya fue escuchado por primera vez.
        if (!puertaDesbloqueada)
        {
            ComprobarDesbloqueo();
        }
    }

    void ComprobarDesbloqueo()
    {
        if (npcDialogo == null)
            return;

        if (npcDialogo.HaHablado)
        {
            puertaDesbloqueada = true;

            // Cambia el Tag para permitir la interaccion normal.
            gameObject.tag = "Interactuable";

            Debug.Log("Puerta desbloqueada: ya hablaste con el NPC.");
        }
        else
        {
            gameObject.tag = "Candado";
        }
    }

    public void Interactuar()
    {
        if (puertaBase == null)
        {
            Debug.LogWarning("No se ha asignado Puerta Base.");
            return;
        }

        ComprobarDesbloqueo();

        if (puertaDesbloqueada)
        {
            puertaBase.Interactuar();
        }
        else
        {
            Debug.Log("Debes hablar con el NPC antes de abrir esta puerta.");
        }
    }
}
