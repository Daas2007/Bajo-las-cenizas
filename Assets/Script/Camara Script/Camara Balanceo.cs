using UnityEngine;

public class CameraBalanceo : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform camara; // la cámara del jugador

    [Header("Idle")]
    [SerializeField] private float idleAmplitude = 0.02f;
    [SerializeField] private float idleFrequency = 1.5f;

    [Header("Walk")]
    [SerializeField] private float walkAmplitude = 0.05f;
    [SerializeField] private float walkFrequency = 3f;

    [Header("Run")]
    [SerializeField] private float runAmplitude = 0.1f;
    [SerializeField] private float runFrequency = 6f;

    private Vector3 posicionInicial;
    private float velocidadActual;

    public enum Estado { Idle, Walk, Run }
    public Estado estado = Estado.Idle;

    void Start()
    {
        if (camara == null) camara = transform;
        posicionInicial = camara.localPosition;
    }

    void Update()
    {
        float tiempo = Time.time;
        float amplitud = 0f;
        float frecuencia = 0f;

        switch (estado)
        {
            case Estado.Idle:
                amplitud = idleAmplitude;
                frecuencia = idleFrequency;
                break;
            case Estado.Walk:
                amplitud = walkAmplitude;
                frecuencia = walkFrequency;
                break;
            case Estado.Run:
                amplitud = runAmplitude;
                frecuencia = runFrequency;
                break;
        }

        // Movimiento vertical (respiración / pasos)
        float offsetY = Mathf.Sin(tiempo * frecuencia) * amplitud;

        // Movimiento lateral (vaivén de hombros)
        float offsetX = Mathf.Cos(tiempo * frecuencia * 0.5f) * (amplitud * 0.5f);

        camara.localPosition = posicionInicial + new Vector3(offsetX, offsetY, 0f);
    }

    // Método para cambiar estado desde tu controlador de movimiento
    public void SetEstado(float velocidad)
    {
        if (velocidad <= 0.1f) estado = Estado.Idle;
        else if (velocidad < 3f) estado = Estado.Walk;
        else estado = Estado.Run;
    }
}

