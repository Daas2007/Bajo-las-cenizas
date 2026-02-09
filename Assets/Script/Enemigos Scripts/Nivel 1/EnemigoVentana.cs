using UnityEngine;

public class EnemigoVentana : MonoBehaviour
{
    [Header("Estados del enemigo")]
    public int estadoActual = 1; // público para ver en Inspector
    public bool recibiendoLuz = false; // público para ver en Inspector
    [SerializeField] float tiempoEnEstado = 0f;

    [Header("Agresividad y tiempos")]
    [SerializeField] float tiempoParaAvanzar = 10f;
    [SerializeField] float tiempoParaRetroceder = 4f;
    [SerializeField] float tiempoTotalJuego = 0f;
    [SerializeField] int nivelAgresividad = 1;

    [Header("Detección de luz (desde linterna)")]
    [SerializeField] float contadorLuz = 0f;

    [Header("Configuración de dificultad")]
    [SerializeField] float reduccionPorNivel = 2f;
    [SerializeField] float tiempoPorNivel = 90f;
    [SerializeField] float tiempoMinimoAvance = 3f;

    [Header("Spawn del enemigo físico")]
    [SerializeField] GameObject prefabEnemigo;
    [SerializeField] Transform puntoSpawn;

    [Header("Temporizador fase 3")]
    [SerializeField] float tiempoAntesDeEntrar = 10f;
    bool cuentaRegresivaActiva = false;
    float tiempoRestanteParaEntrar;
    bool enemigoSpawned = false;

    [Header("Colores Fases Ventana")]
    [SerializeField] Material azulMat;
    [SerializeField] Material naranjaMat;
    [SerializeField] Material rojoMat;

    [Header("Visual de la ventana")]
    [SerializeField] Renderer ventanaRenderer;

    void Start()
    {
        Debug.Log("[Ventana] Iniciado en estado 1 (observando).");
        ActualizarColorVentana();
    }

    void Update()
    {
        float deltaT = Time.deltaTime;
        tiempoTotalJuego += deltaT;
        tiempoEnEstado += deltaT;

        // Escalado de agresividad
        if (tiempoTotalJuego >= nivelAgresividad * tiempoPorNivel)
        {
            nivelAgresividad++;
            tiempoParaAvanzar = Mathf.Max(tiempoMinimoAvance, tiempoParaAvanzar - reduccionPorNivel);
            Debug.Log($"[Ventana] Nivel agresividad {nivelAgresividad}, avanza cada {tiempoParaAvanzar}s sin luz.");
        }

        // Reacción a la linterna
        if (recibiendoLuz)
        {
            contadorLuz += deltaT;
            Debug.Log($"[Ventana] Recibiendo luz, contador={contadorLuz:F2}");

            // Retrocede inmediatamente si está en estado 3
            if (estadoActual == 3)
            {
                RetrocederAEstado1();
            }
            else if (contadorLuz >= tiempoParaRetroceder)
            {
                RetrocederAEstado1();
            }

            if (cuentaRegresivaActiva)
            {
                cuentaRegresivaActiva = false;
                tiempoRestanteParaEntrar = 0f;
                Debug.Log("[Ventana] Lo iluminaste a tiempo, el enemigo se retira!");
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

        // Lógica especial en estado 3
        if (estadoActual == 3 && !recibiendoLuz)
        {
            if (!cuentaRegresivaActiva)
            {
                cuentaRegresivaActiva = true;
                tiempoRestanteParaEntrar = tiempoAntesDeEntrar;
                Debug.Log($"[Ventana] Estado 3, tienes {tiempoAntesDeEntrar}s para iluminarlo!");
            }

            if (cuentaRegresivaActiva)
            {
                tiempoRestanteParaEntrar -= deltaT;
                Debug.Log($"[Ventana] Tiempo restante para entrar: {tiempoRestanteParaEntrar:F2}");

                if (tiempoRestanteParaEntrar <= 0f && !enemigoSpawned)
                {
                    EntrarAHabitacion();
                }
            }
        }
    }

    void AvanzarEstado()
    {
        tiempoEnEstado = 0f;
        estadoActual++;

        if (estadoActual > 3)
            estadoActual = 3;

        Debug.Log($"[Ventana] Avanza a estado {estadoActual}");
        ActualizarColorVentana();
    }

    void RetrocederAEstado1()
    {
        tiempoEnEstado = 0f;
        contadorLuz = 0f;

        if (estadoActual != 1)
        {
            estadoActual = 1;
            cuentaRegresivaActiva = false;
            tiempoRestanteParaEntrar = 0f;
            Debug.Log("[Ventana] La luz lo ha repelido, vuelve al estado 1.");
        }

        ActualizarColorVentana();
    }

    void EntrarAHabitacion()
    {
        Debug.Log("[Ventana] El enemigo ha entrado en la habitación!");
        cuentaRegresivaActiva = false;
        enemigoSpawned = true;

        if (prefabEnemigo != null && puntoSpawn != null)
        {
            GameObject enemigoFisico = Instantiate(prefabEnemigo, puntoSpawn.position, Quaternion.identity);
            EnemigoPerseguidor script = enemigoFisico.GetComponent<EnemigoPerseguidor>();

            if (script != null)
            {
                GameObject jugador = GameObject.FindGameObjectWithTag("Player");
                if (jugador != null)
                {
                    script.objetivo = jugador.transform;
                }
            }
        }
        else
        {
            Debug.LogWarning("[Ventana] No hay prefab o punto de spawn asignado.");
        }
    }

    public void SetIluminado(bool valor)
    {
        recibiendoLuz = valor;
        Debug.Log("[Ventana] SetIluminado llamado → " + valor);
    }

    void ActualizarColorVentana()
    {
        switch (estadoActual)
        {
            case 1: ventanaRenderer.material = azulMat; break;
            case 2: ventanaRenderer.material = naranjaMat; break;
            case 3: ventanaRenderer.material = rojoMat; break;
        }
    }
}
