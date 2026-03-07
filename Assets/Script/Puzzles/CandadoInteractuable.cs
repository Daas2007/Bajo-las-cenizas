using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CandadoInteractuable : MonoBehaviour, IInteractuable
{
    [Header("UI del puzzle (opcional si usas controller)")]
    [SerializeField] private GameObject panelPuzzle; // usado si no hay controller
    [SerializeField] private CandadoController controller; // referencia al controller (recomendada)

    private bool resuelto = false;

    private void Reset()
    {
        var c = GetComponent<Collider>();
        if (c != null) c.isTrigger = false; // evitar triggers: usamos raycast
    }

    private void Start()
    {
        var c = GetComponent<Collider>();
        if (c != null) c.isTrigger = false; // forzamos collider físico

        int layerIndex = LayerMask.NameToLayer("Interaccion");
        if (layerIndex != -1)
        {
            gameObject.layer = layerIndex;
        }
        else
        {
            Debug.LogWarning("[CandadoInteractuable] Layer 'Interaccion' no existe. Crea la layer para interacción.");
        }
    }

    public void Interactuar()
    {
        Debug.Log($"[CandadoInteractuable] Interactuar() llamado. resuelto={resuelto} controller={(controller != null)} panelPuzzle={(panelPuzzle != null)}");

        if (resuelto)
        {
            Debug.Log("[CandadoInteractuable] Ya resuelto, ignorando.");
            return;
        }

        if (controller != null)
        {
            controller.ActivarPuzzle();
            return;
        }

        // Si no hay controller, abrir panel directamente
        if (panelPuzzle != null)
        {
            panelPuzzle.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            Debug.Log("[CandadoInteractuable] Panel del candado activado directamente (sin controller).");
        }
        else
        {
            Debug.LogWarning("[CandadoInteractuable] No hay controller ni panelPuzzle asignado.");
        }
    }

    public void CerrarPuzzle()
    {
        if (panelPuzzle != null)
        {
            panelPuzzle.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
            Debug.Log("[CandadoInteractuable] Panel del puzzle cerrado (directo).");
        }
    }

    public void MarcarResuelto()
    {
        resuelto = true;
        CerrarPuzzle();
        gameObject.layer = LayerMask.NameToLayer("Default");
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
