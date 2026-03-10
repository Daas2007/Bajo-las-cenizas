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

        // Si hay objeto interactuable enfrente
        if (objetoActual != null && Time.timeScale == 1f && (dialogoCanvas == null || !dialogoCanvas.activeSelf))
        {
            if (!panelInteraccion.activeSelf) panelInteraccion.SetActive(true);

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
                    IntentarColocar(objetoTransform.GetComponent<SlotPuzzle>());
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

        // 🔹 Mostrar opción de soltar con Q si hay algo en la mano
        if (manoIzquierda.childCount > 0)
        {
            if (!panelInteraccion.activeSelf) panelInteraccion.SetActive(true);
            textoInteraccion.text = "Presiona [Q] para soltar";

            if (Input.GetKeyDown(KeyCode.Q))
            {
                PiezaPuzzle pieza = manoIzquierda.GetChild(0).GetComponent<PiezaPuzzle>();
                if (pieza != null)
                {
                    pieza.Soltar();
                }
            }
        }
    }
    void DetectarObjeto()
    {
        Ray rayo = new Ray(camara.transform.position, camara.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(rayo, out hit, distanciaInteraccion))
        {
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

    public Transform GetManoIzquierda()
    {
        return manoIzquierda;
    }

    private void IntentarColocar(SlotPuzzle slot)
    {
        if (manoIzquierda.childCount > 0 && slot != null)
        {
            PiezaPuzzle pieza = manoIzquierda.GetChild(0).GetComponent<PiezaPuzzle>();
            if (pieza != null)
            {
                // ✅ La pieza deja de ser hija de la mano y se vuelve hija del slot
                pieza.transform.SetParent(slot.transform);
                pieza.transform.position = slot.transform.position;
                pieza.transform.rotation = slot.transform.rotation;

                // ✅ Forzar escala a 1,1,1 en el slot
                pieza.transform.localScale = Vector3.one;

                slot.piezaActual = pieza;

                if (pieza.piezaID == slot.slotID)
                {
                    pieza.MarcarColocada(true);
                    pieza.PermitirRotacionX(true);
                }
                else
                {
                    pieza.MarcarColocada(false);
                    pieza.PermitirRotacionX(false);
                }
            }
        }
    }

}
