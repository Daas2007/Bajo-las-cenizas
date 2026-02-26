using Unity.Mathematics;
using UnityEngine;

public class Camara : MonoBehaviour
{
    //---------------Rotación con Mouse---------------
    [Header("Rotación con Mouse")]
    [SerializeField] float sensibilidad = 100f;
    [SerializeField] Transform jugador; // referencia al cuerpo del jugador
    float rotacionHorizontal = 0f;
    float rotacionVertical = 0f;

    //---------------Balanceo (Head Bob)---------------
    [Header("Balanceo (Head Bob)")]
    [SerializeField] float idleAmplitude = 0.02f;
    [SerializeField] float idleFrequency = 1.5f;
    [SerializeField] float walkAmplitude = 0.05f;
    [SerializeField] float walkFrequency = 3f;
    [SerializeField] float runAmplitude = 0.08f;
    [SerializeField] float runFrequency = 5f;

    private Vector3 posicionInicial;

    // Estado de movimiento
    public enum Estado { Idle, Walk, Run }
    public Estado estado = Estado.Idle;

    // Valores interpolados
    private float amplitudActual = 0f;
    private float frecuenciaActual = 0f;

    //---------------Inicio---------------
    void Start()
    {
        posicionInicial = transform.localPosition;
        // ❌ No ocultar el mouse aquí, que lo maneje el menú
    }

    //---------------Update---------------
    void Update()
    {
        RotacionMouse();
        AplicarBalanceo();
    }

    //---------------Rotación---------------
    void RotacionMouse()
    {
        float valorX = Input.GetAxis("Mouse X") * sensibilidad * Time.deltaTime;
        float valorY = Input.GetAxis("Mouse Y") * sensibilidad * Time.deltaTime;

        rotacionHorizontal += valorX;
        rotacionVertical -= valorY;
        rotacionVertical = math.clamp(rotacionVertical, -70f, 70f);

        transform.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);

        if (jugador != null)
        {
            jugador.Rotate(Vector3.up * valorX);
        }
        else
        {
            Debug.LogWarning("⚠ Falta asignar el jugador en el script Camara");
        }
    }

    //---------------Balanceo---------------
    void AplicarBalanceo()
    {
        float tiempo = Time.time;

        // Valores objetivo según estado
        float targetAmplitud = 0f;
        float targetFrecuencia = 0f;

        switch (estado)
        {
            case Estado.Idle: targetAmplitud = idleAmplitude; targetFrecuencia = idleFrequency; break;
            case Estado.Walk: targetAmplitud = walkAmplitude; targetFrecuencia = walkFrequency; break;
            case Estado.Run: targetAmplitud = runAmplitude; targetFrecuencia = runFrequency; break;
        }

        // Interpolación suave hacia los valores objetivo
        amplitudActual = Mathf.Lerp(amplitudActual, targetAmplitud, Time.deltaTime * 5f);
        frecuenciaActual = Mathf.Lerp(frecuenciaActual, targetFrecuencia, Time.deltaTime * 5f);

        // Oscilación más natural
        float offsetY = Mathf.Sin(tiempo * frecuenciaActual) * amplitudActual;
        float offsetX = Mathf.Sin(tiempo * frecuenciaActual * 0.5f) * (amplitudActual * 0.5f);

        // Transición suave de posición
        Vector3 targetPos = posicionInicial + new Vector3(offsetX, offsetY, 0f);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * 5f);
    }

    //---------------Estado desde MovimientoPersonaje---------------
    public void SetEstado(float velocidad)
    {
        if (velocidad <= 0.1f) estado = Estado.Idle;
        else if (velocidad < 3f) estado = Estado.Walk;
        else estado = Estado.Run;
    }

    //---------------Control del Mouse---------------
    public void OcultarMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void MostrarMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}