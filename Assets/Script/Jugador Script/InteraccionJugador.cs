using UnityEngine;
using TMPro;

public class InteraccionJugador : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] float distanciaInteraccion = 2f;
    [SerializeField] LayerMask layerInteractuable;
    [SerializeField] Camera camara;

    [Header("UI")]
    [SerializeField] GameObject panelInteraccion;
    [SerializeField] TMP_Text textoInteraccion;
    [SerializeField] GameObject dialogoCanvas;

    [Header("Referencias")]
    [SerializeField] private Transform manoIzquierda; // Empty object para la mano

    private IInteractuable objetoActual;
    private Transform objetoTransform;

    void Update()
    {
        DetectarObjeto();

        if (objetoActual != null && Time.timeScale == 1f && (dialogoCanvas == null || !dialogoCanvas.activeSelf))
        {
            if (!panelInteraccion.activeSelf) panelInteraccion.SetActive(true);

            // Mensajes según el tag
            if (objetoTransform.CompareTag("Pickup") || objetoTransform.CompareTag("Agarrar"))
                textoInteraccion.text = "Presiona [E] para agarrar";
            else if (objetoTransform.CompareTag("Slot") || objetoTransform.CompareTag("Colocar"))
                textoInteraccion.text = "Presiona [E] para colocar";
            else
                textoInteraccion.text = "Presiona [E] para interactuar";

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (objetoTransform.CompareTag("Colocar"))
                {
                    IntentarColocar();
                }
                else
                {
                    objetoActual.Interactuar();
                }
            }
        }
        else
        {
            if (panelInteraccion.activeSelf) panelInteraccion.SetActive(false);
        }
    }

    void DetectarObjeto()
    {
        Ray rayo = new Ray(camara.transform.position, camara.transform.forward);
        RaycastHit hit;

        // IMPORTANTE: usamos Physics.Raycast con un único hit, no AllRaycast
        if (Physics.Raycast(rayo, out hit, distanciaInteraccion))
        {
            // Verificamos si el objeto impactado está en el layer interactuable
            if (((1 << hit.collider.gameObject.layer) & layerInteractuable) != 0)
            {
                IInteractuable interactuable = hit.collider.GetComponent<IInteractuable>();
                if (interactuable != null)
                {
                    objetoActual = interactuable;
                    objetoTransform = hit.collider.transform;
                    return;
                }
            }
        }

        objetoActual = null;
        objetoTransform = null;
    }


    // Método para que otros scripts (como PiezaPuzzle) accedan a la mano izquierda
    public Transform GetManoIzquierda()
    {
        return manoIzquierda;
    }

    // Intentar colocar lo que esté en la mano izquierda
    private void IntentarColocar()
    {
        if (manoIzquierda.childCount > 0)
        {
            PiezaPuzzle pieza = manoIzquierda.GetChild(0).GetComponent<PiezaPuzzle>();
            if (pieza != null)
            {
                pieza.Interactuar(); // reutiliza la lógica de la pieza para colocarla
            }
        }
    }
}
