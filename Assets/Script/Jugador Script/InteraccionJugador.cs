using UnityEngine;

public class InteraccionJugador : MonoBehaviour
{
    [Header("Configuración de interacción")]
    [SerializeField] float distanciaInteraccion = 3f; // Distancia máxima para interactuar
    [SerializeField] LayerMask layerInteractuable; // Capa de objetos interactuables
    [SerializeField] Camera camara; // Cámara del jugador

    private IInteractuable objetoActual;

    void Update()
    {
        DetectarObjeto();

        if (objetoActual != null && Input.GetKeyDown(KeyCode.E))
        {
            objetoActual.Interactuar();
        }
    }

    void DetectarObjeto()
    {
        Ray rayo = new Ray(camara.transform.position, camara.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(rayo, out hit, distanciaInteraccion, layerInteractuable))
        {
            IInteractuable interactuable = hit.collider.GetComponent<IInteractuable>();

            if (interactuable != null)
            {
                objetoActual = interactuable;
                // Debug.DrawRay(camara.transform.position, camara.transform.forward * distanciaInteraccion, Color.green);
                return;
            }
        }

        objetoActual = null;
    }
}

