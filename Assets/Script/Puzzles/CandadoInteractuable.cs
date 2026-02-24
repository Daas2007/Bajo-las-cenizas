using UnityEngine;

public class CandadoInteractuable : MonoBehaviour, IInteractuable
{
    //---------------UI del puzzle---------------
    [Header("UI del puzzle")]
    [SerializeField] private GameObject panelPuzzle; // Canvas/Panel del candado
    [SerializeField] private CandadoController controller; // opcional para cerrar al desbloquear

    private bool resuelto = false;

    //---------------Interacción---------------
    public void Interactuar()
    {
        if (resuelto) return;

        if (panelPuzzle != null)
        {
            panelPuzzle.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("Puzzle del candado abierto.");
        }
    }

    //---------------Cerrar puzzle---------------
    public void CerrarPuzzle()
    {
        if (panelPuzzle != null)
        {
            panelPuzzle.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("Puzzle del candado cerrado.");
        }
    }

    //---------------Marcar resuelto---------------
    public void MarcarResuelto()
    {
        resuelto = true;
        CerrarPuzzle();
        Debug.Log("Candado resuelto.");
    }

    //---------------Eventos al habilitar/deshabilitar---------------
    private void OnEnable()
    {
        if (controller != null)
            controller.AlDesbloquear.AddListener(MarcarResuelto);
    }

    private void OnDisable()
    {
        if (controller != null)
            controller.AlDesbloquear.RemoveListener(MarcarResuelto);
    }
}
