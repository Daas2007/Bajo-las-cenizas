using UnityEngine;

public class CandadoInteractuable : MonoBehaviour, IInteractuable
{
    [Header("UI del puzzle")]
    [SerializeField] private GameObject panelPuzzle; // Canvas/Panel del candado
    [SerializeField] private CandadoController controller; // opcional para cerrar al desbloquear

    private bool resuelto = false;

    public void Interactuar()
    {
        if (resuelto) return;

        if (panelPuzzle != null)
        {
            panelPuzzle.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // ❌ elimina Time.timeScale = 0f;
            Debug.Log("Puzzle del candado abierto.");
        }
    }

    public void CerrarPuzzle()
    {
        if (panelPuzzle != null)
        {
            panelPuzzle.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // ❌ elimina Time.timeScale = 1f;
            Debug.Log("Puzzle del candado cerrado.");
        }
    }

    public void MarcarResuelto()
    {
        resuelto = true;
        CerrarPuzzle();
        Debug.Log("Candado resuelto.");
    }

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

