using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CandadoInteractuable : MonoBehaviour, IInteractuable
{
    [Header("UI del puzzle")]
    [SerializeField] private GameObject panelPuzzle; // Canvas/Panel del candado
    [SerializeField] private CandadoController controller; // referencia opcional al controller

    private bool resuelto = false;

    public void Interactuar()
    {
        if (resuelto)
        {
            Debug.Log("[CandadoInteractuable] Ya resuelto, no se puede interactuar.");
            return;
        }

        if (panelPuzzle != null)
        {
            panelPuzzle.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("[CandadoInteractuable] Puzzle del candado abierto (UI activada).");
        }
        else
        {
            Debug.LogWarning("[CandadoInteractuable] panelPuzzle no asignado.");
        }
    }

    public void CerrarPuzzle()
    {
        if (panelPuzzle != null)
        {
            panelPuzzle.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("[CandadoInteractuable] Puzzle del candado cerrado (UI desactivada).");
        }
    }

    public void MarcarResuelto()
    {
        resuelto = true;
        CerrarPuzzle();
        Debug.Log("[CandadoInteractuable] Candado marcado como resuelto.");
    }

    private void OnEnable()
    {
        if (controller != null)
        {
            controller.AlDesbloquear.AddListener(MarcarResuelto);
            Debug.Log("[CandadoInteractuable] Listener agregado a controller.AlDesbloquear.");
        }
    }

    private void OnDisable()
    {
        if (controller != null)
        {
            controller.AlDesbloquear.RemoveListener(MarcarResuelto);
            Debug.Log("[CandadoInteractuable] Listener removido de controller.AlDesbloquear.");
        }
    }
}

