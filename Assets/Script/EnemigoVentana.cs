using UnityEngine;

public class EnemigoVentana : MonoBehaviour
{
    [Header("Estados del enemigo")]
    [SerializeField] int estadoActual = 1; 
    [SerializeField] float tiempoEnEstado = 0f;

    [Header("Agresividad y tiempos")]
    [SerializeField] float tiempoParaAvanzar = 10f; // tiempo sin luz para cambiar de estado
    [SerializeField] float tiempoParaRetroceder = 4f; // tiempo con luz para volver al estado 1
    [SerializeField] float tiempoTotalJuego = 0f;
    [SerializeField] int nivelAgresividad = 1;

    [Header("Detección de luz (desde linterna)")]
    [SerializeField] bool recibiendoLuz = false;
    [SerializeField] float contadorLuz = 0f;

    [Header("Configuración de dificultad")]
    [SerializeField] float reduccionPorNivel = 2f; // cada nivel reduce este tiempo
    [SerializeField] float tiempoPorNivel = 90f;   // cada 90s se vuelve más agresivo
    [SerializeField] float tiempoMinimoAvance = 3f; // límite de velocidad máxima

    void Start()
    {
        Debug.Log("Enemigo iniciado en estado 1 (observando por la ventana)");
    }

    void Update()
    {
        float deltaT = Time.deltaTime;
        tiempoTotalJuego += deltaT;
        tiempoEnEstado += deltaT;

        // --- Escalado de agresividad ---
        if (tiempoTotalJuego >= nivelAgresividad * tiempoPorNivel)
        {
            nivelAgresividad++;
            tiempoParaAvanzar = Mathf.Max(tiempoMinimoAvance, tiempoParaAvanzar - reduccionPorNivel);
            Debug.Log($" El enemigo se vuelve más agresivo (Nivel {nivelAgresividad}) → Avanza cada {tiempoParaAvanzar}s sin luz.");
        }

        // --- Reacción a la linterna ---
        if (recibiendoLuz)
        {
            contadorLuz += deltaT;

            if (contadorLuz >= tiempoParaRetroceder)
            {
                RetrocederAEstado1();
            }
        }
        else
        {
            contadorLuz = 0f;
            if (tiempoEnEstado >= tiempoParaAvanzar)
            {
                AvanzarEstado();
            }
        }

        // --- Comportamiento por estado ---
        switch (estadoActual)
        {
            case 1:
                // En la ventana, observando
                break;

            case 2:
                // Más agresivo
                break;

            case 3:
                // A punto de entrar
                if (!recibiendoLuz && tiempoEnEstado >= tiempoParaAvanzar)
                {
                    EntrarAHabitacion();
                }
                break;
        }
    }

    void AvanzarEstado()
    {
        tiempoEnEstado = 0f;
        estadoActual++;

        if (estadoActual > 3)
            estadoActual = 3;

        switch (estadoActual)
        {
            case 2:
                Debug.Log(" El enemigo se acerca a la ventana (Estado 2).");
                break;
            case 3:
                Debug.Log(" El enemigo está a punto de entrar (Estado 3).");
                break;
        }
    }

    void RetrocederAEstado1()
    {
        tiempoEnEstado = 0f;
        contadorLuz = 0f;
        recibiendoLuz = false;

        if (estadoActual != 1)
        {
            estadoActual = 1;
            Debug.Log(" La luz lo ha repelido, vuelve al estado 1 (tranquilo).");
        }
    }

    void EntrarAHabitacion()
    {
        Debug.Log(" El enemigo ha entrado en la habitación... GAME OVER ");
        enabled = false; // desactiva el comportamiento
    }

    // Método público que puede llamar la linterna
    public void SetIluminado(bool valor)
    {
        recibiendoLuz = valor;
    }
}
