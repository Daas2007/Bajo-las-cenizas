using UnityEngine;

public class EnemigoVentana : MonoBehaviour
{
    [Header("Estados del enemigo")]
    public int estadoActual = 1;
    public bool recibiendoLuz = false;
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

    [Header("Enemigo en escena (desactivado al inicio)")]
    [Tooltip("Arrastra aquí el GameObject del enemigo que se activará cuando entre en la habitación")]
    [SerializeField] GameObject enemigoEnEscena;

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

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;               // referencia al AudioSource (puede estar en el mismo GameObject)
    [SerializeField] AudioClip clipAvanzar;                 // sonido cuando avanza de estado (1->2, 2->3)
    [SerializeField] AudioClip clipRetroceder;              // sonido cuando retrocede a estado 1
    [SerializeField] AudioClip clipEntrar;                  // sonido cuando entra el enemigo a la habitación
    [SerializeField] float cooldownAudio = 0.25f;           // evita spam de sonidos muy seguidos
    [SerializeField] float pitchVariance = 0.05f;           // variación ligera de pitch para evitar repetición

    // campo privado para cooldown
    float tiempoUltimoAudio = -999f;

    void Start()
    {
        ActualizarColorVentana();

        if (enemigoEnEscena != null)
            enemigoEnEscena.SetActive(false);
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
        }

        // Reacción a la linterna
        if (recibiendoLuz)
        {
            contadorLuz += deltaT;

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
        estadoActual = Mathf.Min(estadoActual + 1, 3);
        ActualizarColorVentana();

        // reproducir sonido de avance
        ReproducirAudio(clipAvanzar);
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
        }

        ActualizarColorVentana();

        // reproducir sonido de retroceso
        ReproducirAudio(clipRetroceder);
    }

    void EntrarAHabitacion()
    {
        cuentaRegresivaActiva = false;
        tiempoRestanteParaEntrar = 0f;
        enemigoSpawned = true;

        // reproducir sonido de entrada
        ReproducirAudio(clipEntrar);

        if (enemigoEnEscena != null)
        {
            EnemigoPerseguidor script = enemigoEnEscena.GetComponent<EnemigoPerseguidor>();
            if (script != null)
            {
                // Resetear estado del perseguidor antes de activarlo
                script.ResetEnemigo();

                // Asegurar que el GameObject esté activo
                enemigoEnEscena.SetActive(true);

                // Asignar objetivo (player) si existe
                GameObject jugador = GameObject.FindGameObjectWithTag("Player");
                if (jugador != null)
                {
                    script.objetivo = jugador.transform;
                }

                // Asegurar que el script esté habilitado
                script.enabled = true;
            }
            else
            {
                enemigoEnEscena.SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning("[EnemigoVentana] EntrarAHabitacion: enemigoEnEscena no asignado.");
        }
    }

    public void SetIluminado(bool valor)
    {
        recibiendoLuz = valor;
    }

    void ActualizarColorVentana()
    {
        if (ventanaRenderer == null) return;

        switch (estadoActual)
        {
            case 1:
                if (azulMat != null) ventanaRenderer.material = azulMat;
                break;
            case 2:
                if (naranjaMat != null) ventanaRenderer.material = naranjaMat;
                break;
            case 3:
                if (rojoMat != null) ventanaRenderer.material = rojoMat;
                break;
        }
    }

    // Reset completo de la ventana y del enemigo asociado
    public void ResetVentana()
    {
        estadoActual = 1;
        tiempoEnEstado = 0f;
        tiempoTotalJuego = 0f;
        contadorLuz = 0f;
        cuentaRegresivaActiva = false;
        tiempoRestanteParaEntrar = 0f;
        enemigoSpawned = false;

        if (enemigoEnEscena != null)
        {
            EnemigoPerseguidor ep = enemigoEnEscena.GetComponent<EnemigoPerseguidor>();
            if (ep != null)
            {
                ep.ResetEnemigo();
                ep.gameObject.SetActive(false);
                ep.enabled = false;
            }
            else
            {
                enemigoEnEscena.SetActive(false);
            }
        }

        // detener audio en curso (opcional)
        if (audioSource != null) audioSource.Stop();

        ActualizarColorVentana();
    }

    // Método público para que GameManager o SistemaGuardar forcen el reset
    public void ForzarReset()
    {
        ResetVentana();
    }

    // Llamar desde EnemigoPerseguidor cuando se desactive o muera
    public void EnemigoDesactivado(GameObject enemigoObj)
    {
        if (enemigoEnEscena == enemigoObj)
        {
            enemigoSpawned = false;
        }
    }

    // Reproducción de audio con cooldown y ligera variación de pitch
    void ReproducirAudio(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;

        if (Time.time - tiempoUltimoAudio < cooldownAudio) return;
        tiempoUltimoAudio = Time.time;

        float originalPitch = audioSource.pitch;
        if (pitchVariance > 0f)
            audioSource.pitch = originalPitch + Random.Range(-pitchVariance, pitchVariance);

        audioSource.PlayOneShot(clip);

        audioSource.pitch = originalPitch;
    }
}
