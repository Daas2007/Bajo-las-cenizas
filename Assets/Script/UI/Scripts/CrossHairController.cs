using UnityEngine;
using UnityEngine.UI;

public class CrosshairSwap : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image crosshairCirculo;
    [SerializeField] private Image crosshairMano;

    [Header("Raycast")]
    [SerializeField] private Camera camara;
    [SerializeField] private float distanciaRaycast = 3f;
    [SerializeField] private LayerMask capaInteractuable;

    [Header("Menús")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject pausaMenuCanvas;
    [SerializeField] private GameObject opcionesMenuCanvas;

    void Update()
    {
        // 🔑 Si algún menú está activo → ocultar crosshair
        if ((mainMenuCanvas != null && mainMenuCanvas.activeSelf) ||
            (pausaMenuCanvas != null && pausaMenuCanvas.activeSelf) ||
            (opcionesMenuCanvas != null && opcionesMenuCanvas.activeSelf))
        {
            crosshairCirculo.enabled = false;
            crosshairMano.enabled = false;
            return;
        }

        // Si no hay menús activos → usar raycast normal
        Ray ray = new Ray(camara.transform.position, camara.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaRaycast))
        {
            // Verificamos que esté en la capa interactuable
            if (((1 << hit.collider.gameObject.layer) & capaInteractuable) != 0)
            {
                // Verificamos que tenga la interfaz IInteractuable
                IInteractuable interactuable = hit.collider.GetComponent<IInteractuable>();
                if (interactuable != null)
                {
                    crosshairCirculo.enabled = false;
                    crosshairMano.enabled = true;
                    return;
                }
            }
        }

        // Si no cumple condiciones → mostrar círculo
        crosshairCirculo.enabled = true;
        crosshairMano.enabled = false;
    }
}
