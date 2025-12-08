using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemigoVentana : MonoBehaviour
{
    [Header("Estados del enemigo")]
    [SerializeField] int estadoActual = 1;
    [SerializeField] float tiempoEnEstado = 0f;

    [Header("Agresividad y tiempos")]
    [SerializeField] float tiempoParaAvanzar = 10f;
    [SerializeField] float tiempoParaRetroceder = 4f;
    [SerializeField] float tiempoTotalJuego = 0f;
    [SerializeField] int nivelAgresividad = 1;

    [Header("Detección de luz (desde linterna)")]
    [SerializeField] bool recibiendoLuz = false;
    [SerializeField] float contadorLuz = 0f;

    [Header("Configuración de dificultad")]
    [SerializeField] float reduccionPorNivel = 2f;
    [SerializeField] float tiempoPorNivel = 90f;
    [SerializeField] float tiempoMinimoAvance = 3f;

    [Header("Spawn del enemigo físico")]
    [SerializeField] GameObject prefabEnemigo;
    [SerializeField] Transform puntoSpawn;

    [Header("Temporizador fase 3")]
    [SerializeField] float tiempoAntesDeEntrar = 10f; // tiempo para iluminarlo
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
        Debug.Log("Enemigo iniciado en estado 1 (observando por la ventana)");
        ActualizarColorVentana();
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
            Debug.Log($"El enemigo se vuelve más agresivo (Nivel {nivelAgresividad}) → Avanza cada {tiempoParaAvanzar}s sin luz.");
        }

        // --- Reacción a la linterna ---
        if (recibiendoLuz)
        {
            contadorLuz += deltaT;

            if (contadorLuz >= tiempoParaRetroceder)
            {
                RetrocederAEstado1();
            }

            if (cuentaRegresivaActiva)
            {
                cuentaRegresivaActiva = false;
                tiempoRestanteParaEntrar = 0f;
                Debug.Log("Lo iluminaste a tiempo, el enemigo se retira!");
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

        // --- Lógica especial en estado 3 ---
        if (estadoActual == 3 && !recibiendoLuz)
        {
            if (!cuentaRegresivaActiva)
            {
                cuentaRegresivaActiva = true;
                tiempoRestanteParaEntrar = tiempoAntesDeEntrar;
                Debug.Log($"El enemigo está listo para entrar... tienes {tiempoAntesDeEntrar} segundos para iluminarlo!");
            }

            if (cuentaRegresivaActiva)
            {
                tiempoRestanteParaEntrar -= deltaT;

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

        switch (estadoActual)
        {
            case 2:
                Debug.Log("El enemigo se acerca a la ventana (Estado 2).");
                break;
            case 3:
                Debug.Log("El enemigo está a punto de entrar (Estado 3).");
                break;
        }

        ActualizarColorVentana();
    }

    void RetrocederAEstado1()
    {
        tiempoEnEstado = 0f;
        contadorLuz = 0f;
        recibiendoLuz = false;

        if (estadoActual != 1)
        {
            estadoActual = 1;
            cuentaRegresivaActiva = false;
            tiempoRestanteParaEntrar = 0f;
            Debug.Log("La luz lo ha repelido, vuelve al estado 1 (tranquilo).");
        }

        ActualizarColorVentana();
    }

    void EntrarAHabitacion()
    {
        Debug.Log("El enemigo ha entrado en la habitación... comienza la persecución.");
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
            Debug.LogWarning("No hay prefab o punto de spawn asignado para el enemigo físico.");
        }
    }

    public void SetIluminado(bool valor)
    {
        recibiendoLuz = valor;
    }

    // Nuevo: cambia el color de la ventana según estado
 

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
