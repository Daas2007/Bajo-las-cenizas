using Unity.Mathematics;
using UnityEngine;

public class Camara : MonoBehaviour
{
    [Header("Rotación con Mouse")]
    [SerializeField] float sensibilidad = 100f;
    [SerializeField] Transform jugador; // referencia al cuerpo del jugador
    float rotacionHorizontal = 0f;
    float rotacionVertical = 0f;

    [Header("Balanceo (Head Bob)")]
    [SerializeField] float idleAmplitude = 0.05f;
    [SerializeField] float idleFrequency = 1.5f;
    [SerializeField] float walkAmplitude = 0.1f;
    [SerializeField] float walkFrequency = 3f;
    [SerializeField] float runAmplitude = 0.15f;
    [SerializeField] float runFrequency = 6f;

    private Vector3 posicionInicial;

    public enum Estado { Idle, Walk, Run }
    public Estado estado = Estado.Idle;

    void Start()
    {
        OcultarMouse();
        posicionInicial = transform.localPosition;
    }

    void Update()
    {
        RotacionMouse();
        AplicarBalanceo();
    }

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

    void AplicarBalanceo()
    {
        float tiempo = Time.time;
        float amplitud = 0f;
        float frecuencia = 0f;

        switch (estado)
        {
            case Estado.Idle: amplitud = idleAmplitude; frecuencia = idleFrequency; break;
            case Estado.Walk: amplitud = walkAmplitude; frecuencia = walkFrequency; break;
            case Estado.Run: amplitud = runAmplitude; frecuencia = runFrequency; break;
        }

        float offsetY = Mathf.Sin(tiempo * frecuencia) * amplitud;
        float offsetX = Mathf.Cos(tiempo * frecuencia * 0.5f) * (amplitud * 0.5f);

        transform.localPosition = posicionInicial + new Vector3(offsetX, offsetY, 0f);
    }

    // Método que se llama desde MovimientoPersonaje
    public void SetEstado(float velocidad)
    {
        if (velocidad <= 0.1f) estado = Estado.Idle;
        else if (velocidad < 3f) estado = Estado.Walk;
        else estado = Estado.Run;
    }

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

