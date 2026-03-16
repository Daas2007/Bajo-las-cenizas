using UnityEngine;

[RequireComponent(typeof(Transform))]
public class LinternaControl : MonoBehaviour
{
    [Header("Configuración de luz")]
    [SerializeField] private Light spotLight;
    [SerializeField] private float intensidadBase = 1f;
    [SerializeField] private float intensidadMinima = 0.3f;

    [Header("Configuración de atenuación")]
    [Tooltip("Distancia a partir de la cual la linterna empezará a debilitarse al acercarse.")]
    [SerializeField] private float distanciaAtenuacion = 2f;
    [Tooltip("Capas que afectan la atenuación de la linterna.")]
    [SerializeField] private LayerMask capasDetectables = ~0;

    [Header("Suavizado")]
    [SerializeField] private bool usarSmoothStep = true;
    [SerializeField] private float velocidadLerp = 10f; // mayor = más rápido el cambio

    private float intensidadObjetivo;

    void Start()
    {
        if (spotLight == null)
        {
            Debug.LogWarning("[LinternaControl] No hay Light asignada en el Inspector.");
        }
        intensidadObjetivo = intensidadBase;
    }

    void Update()
    {
        if (spotLight == null) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanciaAtenuacion, capasDetectables))
        {
            // factor: 0 cuando está en el límite (lejos), 1 cuando está en contacto (muy cerca)
            float raw = 1f - (hit.distance / Mathf.Max(0.0001f, distanciaAtenuacion));
            float factor = Mathf.Clamp01(raw);

            if (usarSmoothStep)
                factor = Mathf.SmoothStep(0f, 1f, factor);

            // intensidadObjetivo se interpola entre base (lejos) y minima (cerca)
            intensidadObjetivo = Mathf.Lerp(intensidadBase, intensidadMinima, factor);
        }
        else
        {
            // No hay objeto dentro de la distancia de atenuación → intensidad normal
            intensidadObjetivo = intensidadBase;
        }

        // Aplicar suavizado en el tiempo para evitar saltos bruscos
        spotLight.intensity = Mathf.Lerp(spotLight.intensity, intensidadObjetivo, Time.deltaTime * velocidadLerp);
    }

    // Métodos públicos para ajustar parámetros en tiempo de ejecución
    public void SetDistanciaAtenuacion(float nuevaDistancia)
    {
        distanciaAtenuacion = Mathf.Max(0.01f, nuevaDistancia);
    }

    public void SetCapasDetectables(LayerMask nuevasCapas)
    {
        capasDetectables = nuevasCapas;
    }

    public void SetIntensidades(float baseIntensidad, float minimaIntensidad)
    {
        intensidadBase = baseIntensidad;
        intensidadMinima = minimaIntensidad;
    }

    // Propiedades de solo lectura por si otros scripts necesitan consultar valores
    public float DistanciaAtenuacion => distanciaAtenuacion;
    public LayerMask CapasDetectables => capasDetectables;
}
 