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

    [Header("Advertencia volumétrica (efecto cámara)")]
    [Tooltip("Referencia al componente VolumetricOjoEffect en la cámara")]
    [SerializeField] private VolumetricOjoEffect volumetricOjo;
    [Tooltip("Cuando queden <= este valor (segundos) se mostrará el pulso volumétrico")]
    [SerializeField] private float tiempoAvisoAntesDeEntrar = 3f;
    private bool avisoActivo = false;

    [Header("Opcional: centrar efecto en posición del enemigo")]
    [Tooltip("Si true, el centro del efecto seguirá la posición world del spawnParent/enemigo")]
    [SerializeField] private bool seguirPosicionEnemigo = true;

    void Start()
    {
        ActualizarColorVentana();

        if (enemigoEnEscena != null)
            enemigoEnEscena.SetActive(false);

        // Asegurarse de que el efecto volumétrico arranque en su intensidad base
        if (volumetricOjo != null)
        {
            volumetricOjo.HidePulse();
            // opcional: inicializar centro si no se va a seguir al enemigo
            // volumetricOjo.center = new Vector2(0.5f, 0.5f);
        }
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

                // ocultar efecto volumétrico si estaba activo
                if (volumetricOjo != null && avisoActivo)
                {
                    volumetricOjo.HidePulse();
                    avisoActivo = false;
                }
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

        // Lógica de cuenta regresiva para entrar cuando está en estado 3
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

                // actualizar centro del efecto para que siga al enemigo (opcional)
                if (seguirPosicionEnemigo && volumetricOjo != null)
                {
                    Vector3 worldPos = Vector3.zero;
                    if (spawnParent != null) worldPos = spawnParent.position;
                    else if (enemigoEnEscena != null) worldPos = enemigoEnEscena.transform.position;

                    Camera cam = Camera.main;
                    if (cam != null)
                    {
                        Vector3 vp = cam.WorldToViewportPoint(worldPos);
                        // solo actualizar si está delante de la cámara
                        if (vp.z > 0f)
                            volumetricOjo.center = new Vector2(Mathf.Clamp01(vp.x), Mathf.Clamp01(vp.y));
                    }
                }

                // Mostrar advertencia volumétrica cuando quede poco tiempo
                if (volumetricOjo != null)
                {
                    if (tiempoRestanteParaEntrar <= tiempoAvisoAntesDeEntrar && tiempoRestanteParaEntrar > 0f)
                    {
                        if (!avisoActivo)
                        {
                            volumetricOjo.ShowPulse(tiempoRestanteParaEntrar);
                            avisoActivo = true;
                        }
                        else
                        {
                            // opcional: actualizar duración reiniciando el pulso
                            // volumetricOjo.ShowPulse(tiempoRestanteParaEntrar);
                        }
                    }
                    else if (tiempoRestanteParaEntrar > tiempoAvisoAntesDeEntrar)
                    {
                        if (avisoActivo)
                        {
                            volumetricOjo.HidePulse();
                            avisoActivo = false;
                        }
                    }
                }

                if (tiempoRestanteParaEntrar <= 0f && !enemigoSpawned)
                {
                    // ocultar efecto antes de entrar
                    if (volumetricOjo != null && avisoActivo)
                    {
                        volumetricOjo.HidePulse();
                        avisoActivo = false;
                    }

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

        // Si había una alerta activa, ocultarla
        if (volumetricOjo != null && avisoActivo)
        {
            volumetricOjo.HidePulse();
            avisoActivo = false;
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

        if (spawnParent != null)
        {
            enemigoEnEscena.transform.SetParent(spawnParent, worldPositionStays: false);
            enemigoEnEscena.transform.localPosition = Vector3.zero;
            enemigoEnEscena.transform.localRotation = Quaternion.identity;
        }
        else
        {
            enemigoEnEscena.transform.SetParent(null, worldPositionStays: true);
            enemigoEnEscena.transform.position = Vector3.zero;
            enemigoEnEscena.transform.rotation = Quaternion.identity;
        }

        EnemigoPerseguidor script = enemigoEnEscena.GetComponent<EnemigoPerseguidor>();
        if (script != null)
        {
            script.ResetEnemigo();
            enemigoEnEscena.SetActive(true);

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

        if (volumetricOjo != null && avisoActivo)
        {
            volumetricOjo.HidePulse();
            avisoActivo = false;
        }

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
