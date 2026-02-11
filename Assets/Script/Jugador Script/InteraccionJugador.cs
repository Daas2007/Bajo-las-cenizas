using UnityEngine;
using TMPro;

public class InteraccionJugador : MonoBehaviour
{
    [Header("Configuración de interacción")]
    [SerializeField] float distanciaInteraccion = 2f;
    [SerializeField] LayerMask layerInteractuable;
    [SerializeField] Camera camara;

    [Header("UI de interacción")]
    [SerializeField] GameObject panelInteraccion;
    [SerializeField] TMP_Text textoInteraccion;
    [SerializeField] GameObject dialogoCanvas; // referencia al panel de diálogo

    private IInteractuable objetoActual;
    private Transform objetoTransform;

    void Update()
    {
        DetectarObjeto();

        // Condición: no mostrar si el juego está pausado (timeScale < 1) o si hay diálogo activo
        if (objetoActual != null && Time.timeScale == 1f && (dialogoCanvas == null || !dialogoCanvas.activeSelf))
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
