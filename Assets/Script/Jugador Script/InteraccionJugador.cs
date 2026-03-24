using UnityEngine;
using TMPro;
using System.Collections;

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

    // 🔹 Referencia a MovimientoPersonaje
    MovimientoPersonaje movimientoJugador;

    void Awake()
    {
        movimientoJugador = FindObjectOfType<MovimientoPersonaje>();
    }

    void Update()
    {
        DetectarObjeto();

        if (objetoActual != null && Time.timeScale == 1f && (dialogoCanvas == null || !dialogoCanvas.activeSelf))
        {
            if (!panelInteraccion.activeSelf) panelInteraccion.SetActive(true);

            // ✅ Mensajes según el tag
            if (objetoTransform.CompareTag("Puzzle"))
                textoInteraccion.text = "Presiona [E] para agarrar pieza de puzzle";
            else if (objetoTransform.CompareTag("Agarrar"))
                textoInteraccion.text = "Presiona [E] para agarrar objeto";
            else if (objetoTransform.CompareTag("Slot") || objetoTransform.CompareTag("Colocar"))
                textoInteraccion.text = "Presiona [E] para colocar";
            else if (objetoTransform.CompareTag("OsoTorso"))
                textoInteraccion.text = "Presiona [E] para colocar pieza del oso";
            else
                textoInteraccion.text = "Presiona [E] para interactuar";

            // ✅ Interacción con E
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (objetoTransform.CompareTag("Colocar"))
                {
                    IntentarColocar(objetoTransform.GetComponent<SlotPuzzle>());
                }
                else if (objetoTransform.CompareTag("Puzzle") || objetoTransform.CompareTag("Agarrar"))
                {
                    // ✅ Pasar referencia de la mano directamente
                    PiezaPuzzle pieza = objetoTransform.GetComponent<PiezaPuzzle>();
                    if (pieza != null)
                    {
                        pieza.SetMano(manoIzquierda);
                    }

                    objetoActual.Interactuar();
                    Debug.Log("[InteraccionJugador] Interactuando con: " + objetoTransform.name);

                    // ✅ Activar flags en Animator
                    if (movimientoJugador != null)
                    {
                        movimientoJugador.tieneObjeto = true;
                        Animator anim = movimientoJugador.GetComponent<Animator>();
                        if (anim != null)
                        {
                            anim.SetBool("TieneObjeto", true);
                            anim.SetBool("AgarraObjeto", true);
                            StartCoroutine(ResetBool(anim, "AgarraObjeto"));
                        }
                    }
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

        // ✅ Soltar con Q
        if (manoIzquierda.childCount > 0)
        {
            if (!panelInteraccion.activeSelf) panelInteraccion.SetActive(true);
            textoInteraccion.text = "Presiona [Q] para soltar";

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Transform objetoEnMano = manoIzquierda.GetChild(0);

                PiezaPuzzle piezaPuzzle = objetoEnMano.GetComponent<PiezaPuzzle>();
                if (piezaPuzzle != null)
                {
                    piezaPuzzle.Soltar();
                }
                else
                {
                    OsoPieza piezaOso = objetoEnMano.GetComponent<OsoPieza>();
                    if (piezaOso != null)
                    {
                        piezaOso.Soltar();
                    }
                    else if (objetoEnMano.CompareTag("Agarrar"))
                    {
                        objetoEnMano.SetParent(null);
                    }
                }

                if (movimientoJugador != null)
                {
                    movimientoJugador.tieneObjeto = false;
                    Animator anim = movimientoJugador.GetComponent<Animator>();
                    if (anim != null)
                        anim.SetBool("TieneObjeto", false);
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
                pieza.transform.SetParent(slot.transform);
                pieza.transform.position = slot.transform.position;
                pieza.transform.rotation = slot.transform.rotation;
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

    // 🔹 Corrutina para resetear bool en Animator
    private IEnumerator ResetBool(Animator anim, string parametro)
    {
        yield return null; // esperar un frame
        anim.SetBool(parametro, false);
    }
}
