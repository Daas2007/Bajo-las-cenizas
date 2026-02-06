using UnityEngine;
using TMPro;

public class InteraccionJugador : MonoBehaviour
{
    [Header("Configuración de interacción")]
    [SerializeField] float distanciaInteraccion;
    [SerializeField] LayerMask layerInteractuable;
    [SerializeField] Camera camara;

    [Header("UI de interacción")]
    [SerializeField] GameObject panelInteraccion;
    [SerializeField] TMP_Text textoInteraccion;

    private IInteractuable objetoActual;
    private Transform objetoTransform; // para leer el Tag

    void Update()
    {
        DetectarObjeto();

        if (objetoActual != null)
        {
            if (!panelInteraccion.activeSelf)
                panelInteraccion.SetActive(true);

            // Mostrar texto según el Tag
            if (objetoTransform.CompareTag("Pickup"))
                textoInteraccion.text = "Presiona [E] para agarrar";
            else
                textoInteraccion.text = "Presiona [E] para interactuar";

            if (Input.GetKeyDown(KeyCode.E))
            {
                objetoActual.Interactuar();
            }
        }
        else
        {
            if (panelInteraccion.activeSelf)
                panelInteraccion.SetActive(false);
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
                objetoTransform = hit.collider.transform;
                return;
            }
        }

        objetoActual = null;
        objetoTransform = null;
    }
}
