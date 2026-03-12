//using UnityEngine;

///// <summary>
///// Mapea la distancia entre enemigo y objetivo a un valor 0..1 y lo envía a EyeCloseEffect.
///// - distanciaMax: distancia a partir de la cual el efecto es 0 (lejos).
///// - distanciaMin: distancia a la que el efecto es 1 (muy cerca).
///// - responseCurve permite ajustar la sensibilidad.
///// </summary>
//public class EnemyProximityEye : MonoBehaviour
//{
//    [Header("Referencias")]
//    [Tooltip("Transform del enemigo que se acerca")]
//    [SerializeField] private Transform enemigo;
//    [Tooltip("Transform del objetivo (jugador o cámara)")]
//    [SerializeField] private Transform objetivo;

//    [Header("Rango de influencia")]
//    [Tooltip("Distancia a la que el efecto empieza a notarse (lejos)")]
//    [SerializeField] private float distanciaMax = 20f;
//    [Tooltip("Distancia a la que el ojo está completamente cerrado (cerca)")]
//    [SerializeField] private float distanciaMin = 2f;

//    [Header("Efecto visual")]
//    [SerializeField] private EyeCloseEffect eyeEffect;

//    [Header("Opciones")]
//    [Tooltip("Invertir la respuesta (true = más cerca -> menor amount)")]
//    [SerializeField] private bool invertir = false;
//    [Tooltip("Curva para ajustar la respuesta (input 0..1 -> output 0..1)")]
//    [SerializeField] private AnimationCurve responseCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

//    void Start()
//    {
//        if (objetivo == null && Camera.main != null)
//            objetivo = Camera.main.transform;

//        if (eyeEffect == null)
//            Debug.LogWarning("[EnemyProximityEye] eyeEffect no asignado.");

//        if (enemigo == null)
//            Debug.LogWarning("[EnemyProximityEye] enemigo no asignado.");
//    }

//    void Update()
//    {
//        if (eyeEffect == null || enemigo == null || objetivo == null) return;

//        float dist = Vector3.Distance(enemigo.position, objetivo.position);

//        // Mapear distancia a 0..1 (1 = muy cerca)
//        float t = Mathf.InverseLerp(distanciaMax, distanciaMin, dist);
//        t = Mathf.Clamp01(t);

//        if (invertir) t = 1f - t;

//        float amount = responseCurve.Evaluate(t);

//        eyeEffect.SetTargetAmount(amount);
//    }
//}
