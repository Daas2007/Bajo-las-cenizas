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

        if (Physics.Raycast(ray, out hit, distanciaRaycast, capaInteractuable))
        {
            crosshairCirculo.enabled = false;
            crosshairMano.enabled = true;
        }
        else
        {
            crosshairCirculo.enabled = true;
            crosshairMano.enabled = false;
        }
    }
}

