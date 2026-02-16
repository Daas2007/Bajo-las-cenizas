using UnityEngine;
using System.Collections;

public class CajonesInteractuables : MonoBehaviour, IInteractuable
{
    [Header("Configuración del cajón")]
    [SerializeField] private Transform cajon;          // referencia al cajón
    [SerializeField] private Vector3 direccionMovimiento = Vector3.forward; // dirección del movimiento
    [SerializeField] private float distancia = 0.5f;   // cuánto se moverá
    [SerializeField] private float duracion = 1f;      // tiempo de animación

    private Vector3 posicionInicial;
    private Vector3 posicionFinal;
    private bool abierto = false;
    private Coroutine rutinaMovimiento;

    void Start()
    {
        if (cajon == null)
        {
            Debug.LogError("⚠️ No se asignó el cajón en el Inspector.");
            return;
        }

        // Guardar posición inicial y calcular final
        posicionInicial = cajon.localPosition;
        posicionFinal = posicionInicial + direccionMovimiento.normalized * distancia;
    }

    public void Interactuar()
    {
        if (cajon == null) return;

        if (rutinaMovimiento != null) StopCoroutine(rutinaMovimiento);

        if (!abierto)
            rutinaMovimiento = StartCoroutine(MoverCajon(posicionFinal, true));
        else
            rutinaMovimiento = StartCoroutine(MoverCajon(posicionInicial, false));
    }

    private IEnumerator MoverCajon(Vector3 destino, bool abrir)
    {
        float tiempo = 0f;
        Vector3 inicio = cajon.localPosition;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;
            cajon.localPosition = Vector3.Lerp(inicio, destino, t);
            tiempo += Time.deltaTime;
            yield return null;
        }

        cajon.localPosition = destino;
        abierto = abrir;
        rutinaMovimiento = null;
    }
}

