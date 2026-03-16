using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    [Tooltip("Solo la ventana (GameObject que contiene el UI o el objeto ventana)")]
    [SerializeField] private GameObject ventanaRoot;

    // referencia al script EnemigoVentana dentro de ventanaRoot (si existe)
    private EnemigoVentana ventanaScript;

    private void Awake()
    {
        if (ventanaRoot != null)
            ventanaScript = ventanaRoot.GetComponentInChildren<EnemigoVentana>();
        else
            ventanaScript = GetComponentInChildren<EnemigoVentana>();
    }

    void Start()
    {
        if (ventanaRoot != null)
            ventanaRoot.SetActive(false); // ventana desactivada al inicio
    }

    // Requiere que el collider esté marcado como "Is Trigger" y que el Player tenga tag "Player"
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (ventanaRoot != null)
            ventanaRoot.SetActive(true);

        if (ventanaScript != null)
            Debug.Log($"EnemyActivator [{name}] activó la ventana.");

        // ✅ destruir este objeto después de activar la ventana
        Destroy(gameObject);
    }

    // Método público para activar/desactivar desde código si lo necesitas
    public void ActivarVentana(bool activo)
    {
        if (ventanaRoot != null)
            ventanaRoot.SetActive(activo);
        else
            Debug.LogWarning($"EnemyActivator [{name}] no tiene ventanaRoot asignado.");
    }
}
