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

    [Header("UI Ojo Irritado")]
    [SerializeField] private GameObject panelOjoEstado;   // primer panel (estado 2 y 3)
    [SerializeField] private GameObject panelOjoEnemigo;  // segundo panel (cuando entra)

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

    [Header("Ajustes internos")]
    [SerializeField] private float tiempoAvisoAntesDeEntrar = 3f;

    [Header("Opcional: centrar efecto en posición del enemigo (no usado)")]
    [SerializeField] private bool seguirPosicionEnemigo = true;

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

        // Escala dificultad con el tiempo
        if (tiempoTotalJuego >= nivelAgresividad * tiempoPorNivel)
        {
            nivelAgresividad++;
            tiempoParaAvanzar = Mathf.Max(tiempoMinimoAvance, tiempoParaAvanzar - reduccionPorNivel);
        }

        // Si recibe luz, retrocede o resetea cuenta
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

            // Avanza de estado si ha pasado el tiempo requerido
            if (tiempoEnEstado >= tiempoParaAvanzar)
            {
                if (estadoActual < 3)
                {
                    AvanzarEstado();
                }
                else if (estadoActual == 3 && !enemigoSpawned)
                {
                    // ✅ Si ya está en estado 3 y se cumple el tiempo, entra a la habitación
                    EntrarAHabitacion();
                }
            }
        }

        // --- Lógica de cuenta regresiva para entrar ---
        // Ahora la cuenta corre si cuentaRegresivaActiva == true (no se exige estadoActual == 3)
        if (cuentaRegresivaActiva && !recibiendoLuz && !enemigoSpawned)
        {
            tiempoRestanteParaEntrar -= deltaT;

            // (Opcional) aquí podrías actualizar efectos visuales o UI según tiempoRestanteParaEntrar
            // Si llega a cero, spawnear enemigo
            if (tiempoRestanteParaEntrar <= 0f && !enemigoSpawned)
            {
                EntrarAHabitacion();
            }
        }
    }
    void AvanzarEstado()
    {
        tiempoEnEstado = 0f;
        estadoActual = Mathf.Min(estadoActual + 1, 3);
        ActualizarColorVentana();
        ActualizarUIOjo();
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
        ActualizarUIOjo();
        ReproducirAudio(clipRetroceder);
    }
    void EntrarAHabitacion()
    {
        cuentaRegresivaActiva = false;
        tiempoRestanteParaEntrar = 0f;
        enemigoSpawned = true;

        ActualizarUIOjo();
        ReproducirAudio(clipEntrar);

        if (enemigoEnEscena == null)
        {
            Debug.LogWarning("[EnemigoVentana] EntrarAHabitacion: enemigoEnEscena no asignado.");
            return;
        }

        // Colocar al enemigo en el spawn
        if (spawnParent != null)
        {
            enemigoEnEscena.transform.SetParent(spawnParent, false);
            enemigoEnEscena.transform.localPosition = Vector3.zero;
            enemigoEnEscena.transform.localRotation = Quaternion.identity;
        }

        // ✅ Activar el GameObject
        enemigoEnEscena.SetActive(true);

        // ✅ Reiniciar y activar persecución
        EnemigoPerseguidor script = enemigoEnEscena.GetComponent<EnemigoPerseguidor>();
        if (script != null)
        {
            script.ResetEnemigo();

            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null)
            {
                script.objetivo = jugador.transform;
            }

            script.ActivarPersecucion(); // 🔥 aquí se activa la persecución
        }
    }
    public void StartTriggerSequence()
    {
        // NO forzamos estadoActual = 3 aquí.
        // Solo arrancamos la cuenta regresiva si aún no está spawneado.
        if (enemigoSpawned) return;

        cuentaRegresivaActiva = true;
        tiempoRestanteParaEntrar = tiempoAntesDeEntrar;

        // Reset timers para evitar saltos
        tiempoEnEstado = 0f;
        contadorLuz = 0f;
    }
    public void StopTriggerSequence()
    {
        cuentaRegresivaActiva = false;
        tiempoRestanteParaEntrar = 0f;
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

        ActualizarUIOjo();
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
    void ActualizarUIOjo()
    {
        // Panel del estado (azul/naranja/rojo)
        if (panelOjoEstado != null)
        {
            var img = panelOjoEstado.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                Color c = img.color;

                if (estadoActual == 2)
                {
                    panelOjoEstado.SetActive(true);
                    c.a = 0.5f; // semi-transparente
                }
                else if (estadoActual == 3)
                {
                    panelOjoEstado.SetActive(true);
                    c.a = 1f;   // opaco completo
                }
                else
                {
                    panelOjoEstado.SetActive(false); // estado 1 → oculto
                }

                img.color = c;
            }
        }

        // Panel del enemigo entrando
        if (panelOjoEnemigo != null)
        {
            var img2 = panelOjoEnemigo.GetComponent<UnityEngine.UI.Image>();
            if (img2 != null)
            {
                if (enemigoSpawned)
                {
                    panelOjoEnemigo.SetActive(true);
                    Color c2 = img2.color;
                    c2.a = 1f; // opaco completo
                    img2.color = c2;
                }
                else
                {
                    panelOjoEnemigo.SetActive(false);
                }
            }
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