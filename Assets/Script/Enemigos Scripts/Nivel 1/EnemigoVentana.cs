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

    [Header("Spawn parent (objeto vacío que define la posición local 0,0,0)")]
    [Tooltip("Arrastra aquí el Transform del objeto vacío que debe ser el padre del enemigo (su localPosition 0,0,0 será la posición de spawn)")]
    [SerializeField] Transform spawnParent;

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
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip clipAvanzar;
    [SerializeField] AudioClip clipRetroceder;
    [SerializeField] AudioClip clipEntrar;
    [SerializeField] float cooldownAudio = 0.25f;
    [SerializeField] float pitchVariance = 0.05f;

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

        if (tiempoTotalJuego >= nivelAgresividad * tiempoPorNivel)
        {
            nivelAgresividad++;
            tiempoParaAvanzar = Mathf.Max(tiempoMinimoAvance, tiempoParaAvanzar - reduccionPorNivel);
        }

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
        ReproducirAudio(clipRetroceder);
    }

    void EntrarAHabitacion()
    {
        cuentaRegresivaActiva = false;
        tiempoRestanteParaEntrar = 0f;
        enemigoSpawned = true;

        ReproducirAudio(clipEntrar);

        if (enemigoEnEscena == null)
        {
            Debug.LogWarning("[EnemigoVentana] EntrarAHabitacion: enemigoEnEscena no asignado.");
            return;
        }

        // Si hay spawnParent configurado, parentar al enemigo a ese transform y usar localPosition = Vector3.zero
        if (spawnParent != null)
        {
            // 1) Parentar (sin mantener posición mundial)
            enemigoEnEscena.transform.SetParent(spawnParent, worldPositionStays: false);

            // 2) Asegurar localPosition/localRotation en cero
            enemigoEnEscena.transform.localPosition = Vector3.zero;
            enemigoEnEscena.transform.localRotation = Quaternion.identity;
        }
        else
        {
            // Si no hay spawnParent, por compatibilidad colocamos en world 0,0,0
            enemigoEnEscena.transform.SetParent(null, worldPositionStays: true);
            enemigoEnEscena.transform.position = Vector3.zero;
            enemigoEnEscena.transform.rotation = Quaternion.identity;
        }

        // Resetear física y estado del perseguidor de forma segura
        EnemigoPerseguidor script = enemigoEnEscena.GetComponent<EnemigoPerseguidor>();
        if (script != null)
        {
            script.ResetEnemigo();

            // Activar después de reposicionar y resetear
            enemigoEnEscena.SetActive(true);

            // Asignar objetivo si existe
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null)
            {
                script.objetivo = jugador.transform;
            }

            script.enabled = true;
        }
        else
        {
            enemigoEnEscena.SetActive(true);
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

        if (audioSource != null) audioSource.Stop();

        ActualizarColorVentana();
    }

    public void ForzarReset()
    {
        ResetVentana();
    }

    public void EnemigoDesactivado(GameObject enemigoObj)
    {
        if (enemigoEnEscena == enemigoObj)
        {
            enemigoSpawned = false;
        }
    }

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
