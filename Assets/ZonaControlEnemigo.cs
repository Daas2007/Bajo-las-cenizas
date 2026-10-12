
using UnityEngine;

public class ZonaControlEnemigo : MonoBehaviour
{
    [SerializeField] private ControladorAparicionEnemigo controlador;

    private void OnTriggerEnter(Collider other)
    {
        if (EsElEnemigo(other))
        {
            controlador.PonerVelocidadNormal();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (EsElEnemigo(other))
        {
            controlador.Desactivar();
        }
    }

    private bool EsElEnemigo(Collider other)
    {
        return controlador != null &&
               controlador.EsEnemigo(other);
    }
}