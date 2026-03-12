using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    [Tooltip("Solo la ventana (GameObject que contiene el UI o el objeto ventana)")]
    [SerializeField] private GameObject ventanaRoot;

    [Tooltip("Si true, al salir del trigger se desactiva la ventana y se cancela la secuencia")]
    [SerializeField] private bool deactivateOnExit = false;

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
            ventanaScript.StartTriggerSequence();
        else
            Debug.LogWarning($"EnemyActivator [{name}] no encontró EnemigoVentana en ventanaRoot.");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (deactivateOnExit)
        {
            if (ventanaRoot != null)
                ventanaRoot.SetActive(false);
        }

        if (ventanaScript != null)
            ventanaScript.StopTriggerSequence();
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
