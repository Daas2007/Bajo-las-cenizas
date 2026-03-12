using UnityEngine;
using System.Collections;

/// <summary>
/// CajonesTV: mueve un Transform entre dos puntos (startPoint -> endPoint) en un tiempo dado.
/// Implementa IInteractuable para ser usado con tu sistema de interacción.
/// </summary>
[DisallowMultipleComponent]
public class CajonesTV : MonoBehaviour, IInteractuable
{
    [Header("Referencias")]
    [Tooltip("Transform que se moverá (por ejemplo el cajón). Si está vacío se usará este GameObject.")]
    [SerializeField] private Transform cajon;

    [Tooltip("Transform que indica la posición inicial (si se deja vacío se toma la posición actual del cajón).")]
    [SerializeField] private Transform startPoint;

    [Tooltip("Transform que indica la posición destino a la que se moverá el cajón.")]
    [SerializeField] private Transform endPoint;

    [Header("Configuración")]
    [Tooltip("Tiempo en segundos que tarda en moverse de inicio a destino")]
    [SerializeField] private float duracion = 1f;

    [Tooltip("Mover usando posiciones locales en lugar de posiciones globales")]
    [SerializeField] private bool usarLocalPosition = false;

    // Estado interno
    private Vector3 posicionInicial;
    private Vector3 posicionDestino;
    private bool abierto = false; // false = en inicial, true = en destino
    private Coroutine rutinaMovimiento;

    private void Reset()
    {
        // Si no se asignó cajon, usar el mismo GameObject
        if (cajon == null) cajon = this.transform;
    }

    private void Start()
    {
        if (cajon == null) cajon = this.transform;

        // Determinar posiciones inicial y destino según los Transforms proporcionados
        if (startPoint != null)
            posicionInicial = usarLocalPosition ? startPoint.localPosition : startPoint.position;
        else
            posicionInicial = usarLocalPosition ? cajon.localPosition : cajon.position;

        if (endPoint != null)
            posicionDestino = usarLocalPosition ? endPoint.localPosition : endPoint.position;
        else
            posicionDestino = posicionInicial; // si no hay endPoint, no se moverá

        // Asegurar que el cajón comience en la posición inicial
        if (usarLocalPosition)
            cajon.localPosition = posicionInicial;
        else
            cajon.position = posicionInicial;
    }

    /// <summary>
    /// Método de la interfaz IInteractuable: alterna entre posición inicial y destino.
    /// </summary>
    public void Interactuar()
    {
        if (cajon == null)
        {
            Debug.LogWarning("[CajonesTV] No hay Transform asignado para mover.");
            return;
        }

        // Si no hay destino distinto, no hacer nada
        if (posicionDestino == posicionInicial)
        {
            Debug.LogWarning("[CajonesTV] startPoint y endPoint son iguales o endPoint no asignado. Nada que mover.");
            return;
        }

        // Si ya hay una rutina en curso, detenerla para iniciar la nueva (permite interrumpir)
        if (rutinaMovimiento != null)
        {
            StopCoroutine(rutinaMovimiento);
            rutinaMovimiento = null;
        }

        // Elegir destino según estado actual
        Vector3 objetivo = abierto ? posicionInicial : posicionDestino;
        bool nuevoEstado = !abierto;

        rutinaMovimiento = StartCoroutine(MoverCajon(cajon, objetivo, duracion, nuevoEstado));
    }

    private IEnumerator MoverCajon(Transform objetivoTransform, Vector3 destino, float tiempoTotal, bool estadoFinal)
    {
        float tiempo = 0f;
        Vector3 inicio = usarLocalPosition ? objetivoTransform.localPosition : objetivoTransform.position;

        // Si la duración es 0 o negativa, saltar la interpolación
        if (tiempoTotal <= 0f)
        {
            if (usarLocalPosition) objetivoTransform.localPosition = destino;
            else objetivoTransform.position = destino;

            abierto = estadoFinal;
            rutinaMovimiento = null;
            yield break;
        }

        while (tiempo < tiempoTotal)
        {
            float t = tiempo / tiempoTotal;
            // suavizado opcional: t = Mathf.SmoothStep(0f, 1f, t);
            Vector3 nuevaPos = Vector3.Lerp(inicio, destino, t);

            if (usarLocalPosition) objetivoTransform.localPosition = nuevaPos;
            else objetivoTransform.position = nuevaPos;

            tiempo += Time.deltaTime;
            yield return null;
        }

        // Asegurar posición final exacta
        if (usarLocalPosition) objetivoTransform.localPosition = destino;
        else objetivoTransform.position = destino;

        abierto = estadoFinal;
        rutinaMovimiento = null;
    }

    /// <summary>
    /// Opcional: método público para forzar que el cajón vuelva a la posición inicial.
    /// </summary>
    public void ResetToInitial(bool instant = false)
    {
        if (rutinaMovimiento != null)
        {
            StopCoroutine(rutinaMovimiento);
            rutinaMovimiento = null;
        }

        if (cajon == null) cajon = this.transform;

        if (instant)
        {
            if (usarLocalPosition) cajon.localPosition = posicionInicial;
            else cajon.position = posicionInicial;
            abierto = false;
        }
        else
        {
            rutinaMovimiento = StartCoroutine(MoverCajon(cajon, posicionInicial, duracion, false));
        }
    }
}
