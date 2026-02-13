using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    [SerializeField] private GameObject ventanaRoot; // solo la ventana

    private void Awake()
    {
       
    }
    void Start()
    {
        if (ventanaRoot != null)
        {
            ventanaRoot.SetActive(false); // ventana desactivada al inicio
        }
    }

    public void ActivarVentana(bool activo)
    {
        if (ventanaRoot != null)
        {
            ventanaRoot.SetActive(activo);
        }
        else
        {
            Debug.LogWarning($"EnemyActivator [{name}] no tiene ventanaRoot asignado.");
        }
    }
}
