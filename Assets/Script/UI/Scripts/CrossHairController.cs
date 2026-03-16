using UnityEngine;
using UnityEngine.UI;

public class CrosshairSwap : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image crosshairCirculo;
    [SerializeField] private Image crosshairMano;
    [SerializeField] private Image crosshairCandado;
    [SerializeField] private Image crosshairPuzzle;

    [Header("Raycast")]
    [SerializeField] private Camera camara;
    [SerializeField] private float distanciaRaycast = 3f;
    [SerializeField] private LayerMask capaInteractuable;

    void Update()
    {
        if (camara == null)
        {
            Debug.LogError("❌ No se asignó la cámara en CrosshairSwap.");
            return;
        }

        Ray ray = new Ray(camara.transform.position, camara.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaRaycast, capaInteractuable))
        {
            IInteractuable interactuable = hit.collider.GetComponent<IInteractuable>();
            if (interactuable != null)
            {
                // 🔹 Cambiar crosshair según el tag
                if (hit.collider.CompareTag("Candado"))
                {
                    SetCrosshair(false, false, true, false);
                }
                else if (hit.collider.CompareTag("Puzzle"))
                {
                    SetCrosshair(false, false, false, true);
                }
                else
                {
                    SetCrosshair(false, true, false, false); // mano
                }
                return;
            }
        }

        // Si no cumple condiciones → mostrar círculo
        SetCrosshair(true, false, false, false);
    }

    private void SetCrosshair(bool circulo, bool mano, bool candado, bool puzzle)
    {
        if (crosshairCirculo != null) crosshairCirculo.enabled = circulo;
        if (crosshairMano != null) crosshairMano.enabled = mano;
        if (crosshairCandado != null) crosshairCandado.enabled = candado;
        if (crosshairPuzzle != null) crosshairPuzzle.enabled = puzzle;
    }
}
