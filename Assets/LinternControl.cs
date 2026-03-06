using UnityEngine;

public class LinternaControl : MonoBehaviour
{
    [SerializeField] private Light spotLight;
    [SerializeField] private float intensidadBase = 1f;
    [SerializeField] private float distanciaMax = 5f;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanciaMax))
        {
            // Si hay pared cerca, baja la intensidad
            float factor = hit.distance / distanciaMax; // valor entre 0 y 1
            spotLight.intensity = Mathf.Lerp(0.3f, intensidadBase, factor);
        }
        else
        {
            // Si no hay nada cerca, usa la intensidad normal
            spotLight.intensity = intensidadBase;
        }
    }
}
