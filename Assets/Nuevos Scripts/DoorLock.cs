using UnityEngine;

public class DoorLock : MonoBehaviour
{
    [SerializeField] private PuertaInteractuable puerta; // tu componente que anima/abre la puerta
    [SerializeField] private bool bloqueada = false;

    private void Start()
    {
        // Asegurar estado inicial coherente con la variable bloqueada
        ApplyStateToPuerta();
    }

    public void Lock()
    {
        bloqueada = true;
        ApplyStateToPuerta();
    }

    public void Unlock()
    {
        bloqueada = false;
        ApplyStateToPuerta();
    }

    public bool EstaBloqueada() => bloqueada;

    private void ApplyStateToPuerta()
    {
        if (puerta == null) return;

        // Suponemos que PuertaInteractuable tiene métodos públicos para abrir/cerrar o un boolean de estado.
        // Si PuertaInteractuable solo tiene Interactuar() que hace toggle, comprobamos estado antes de llamar.
        if (bloqueada)
        {
            if (puerta.EstaAbierta()) puerta.Interactuar(); // cerrar si está abierta
        }
        else
        {
            if (!puerta.EstaAbierta()) puerta.Interactuar(); // abrir si está cerrada
        }
    }
}
