using Unity.Mathematics;
using UnityEngine;

public class NewCamara : MonoBehaviour
{
    //---------------Rotación con Mouse---------------
    [Header("Rotación con Mouse")]
    [SerializeField] private float sensibilidad = 100f;
    [SerializeField] private Transform jugador; // Referencia al cuerpo del jugador
    private float rotacionHorizontal = 0f;
    private float rotacionVertical = 0f;

    //---------------Balanceo (Head Bob)---------------
    [Header("Balanceo (Head Bob)")]
    [SerializeField] private float idleAmplitude = 0.02f;
    [SerializeField] private float idleFrequency = 1.5f;
    [SerializeField] private float walkAmplitude = 0.05f;
    [SerializeField] private float walkFrequency = 3f;
    [SerializeField] private float runAmplitude = 0.08f;
    [SerializeField] private float runFrequency = 5f;

    private Vector3 posicionInicial;

    // Estado de movimiento
    public enum Estado { Idle, Walk, Run }
    public Estado estado = Estado.Idle;

    // Valores interpolados
    private float amplitudActual = 0f;
    private float frecuenciaActual = 0f;

    //---------------Control Externo del Cursor---------------
    private bool estaEnModoUI = false;

    /// <summary>
    /// Propiedad para saber si estamos en modo UI o cambiar el estado.
    /// </summary>
    public bool EstaEnModoUI => estaEnModoUI;

    public bool CursorBloqueado
    {
        get => !estaEnModoUI;
        set => SetModoUI(!value);
    }

    //---------------Inicio---------------
    private void Start()
    {
        posicionInicial = transform.localPosition;

        // Iniciamos con el ratón bloqueado para el juego normal
        SetModoUI(false);
    }

    //---------------Update---------------
    private void Update()
    {
        // Si estamos interactuando con la UI, un minijuego o el juego está pausado, 
        // congelamos la rotación de la cámara y el balanceo.
        if (estaEnModoUI || Time.timeScale <= 0f)
        {
            return;
        }

        RotacionMouse();
        AplicarBalanceo();
    }

    //---------------Métodos de Control de UI / Cursor---------------
    /// <summary>
    /// Cambia el estado del cursor y de la cámara.
    /// </summary>
    /// <param name="activarUI">True si abres un minijuego/menú. False al volver al juego.</param>
    public void SetModoUI(bool activarUI)
    {
        estaEnModoUI = activarUI;

        if (estaEnModoUI)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Método rápido para activar el modo UI (Mouse libre).
    /// </summary>
    public void EntrarModoUI() => SetModoUI(true);

    /// <summary>
    /// Método rápido para salir del modo UI y volver al gameplay (Mouse bloqueado).
    /// </summary>
    public void SalirModoUI() => SetModoUI(false);

    //---------------Funciones Compatibles Antiguas---------------
    public void OcultarMouse() => SetModoUI(false);
    public void MostrarMouse() => SetModoUI(true);

    //---------------Rotación---------------
    private void RotacionMouse()
    {
        float valorX = Input.GetAxis("Mouse X") * sensibilidad * Time.deltaTime;
        float valorY = Input.GetAxis("Mouse Y") * sensibilidad * Time.deltaTime;

        rotacionHorizontal += valorX;
        rotacionVertical -= valorY;
        rotacionVertical = math.clamp(rotacionVertical, -60f, 50f);

        transform.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);

        if (jugador != null)
        {
            jugador.Rotate(Vector3.up * valorX);
        }
        else
        {
            Debug.LogWarning("⚠ Falta asignar el jugador en el script NewCamara");
        }
    }

    //---------------Balanceo---------------
    private void AplicarBalanceo()
    {
        float tiempo = Time.time;

        float targetAmplitud = 0f;
        float targetFrecuencia = 0f;

        switch (estado)
        {
            case Estado.Idle: targetAmplitud = idleAmplitude; targetFrecuencia = idleFrequency; break;
            case Estado.Walk: targetAmplitud = walkAmplitude; targetFrecuencia = walkFrequency; break;
            case Estado.Run: targetAmplitud = runAmplitude; targetFrecuencia = runFrequency; break;
        }

        amplitudActual = Mathf.Lerp(amplitudActual, targetAmplitud, Time.deltaTime * 5f);
        frecuenciaActual = Mathf.Lerp(frecuenciaActual, targetFrecuencia, Time.deltaTime * 5f);

        float offsetY = Mathf.Sin(tiempo * frecuenciaActual) * amplitudActual;
        float offsetX = Mathf.Sin(tiempo * frecuenciaActual * 0.5f) * (amplitudActual * 0.5f);

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
}