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

    [Header("Modelos de enemigo por estado")]
    [SerializeField] GameObject enemigoEstado1;   // azul
    [SerializeField] GameObject enemigoEstado2;   // naranja
    [SerializeField] GameObject enemigoEstado3;   // rojo (golpeando ventana)
    [SerializeField] GameObject enemigoEstado4;   // nuevo: animación de entrar habitación
    [SerializeField] GameObject enemigoReal;      // enemigo que persigue al jugador

    [Header("Spawn parent (objeto vacío que define la posición local 0,0,0)")]
    [SerializeField] Transform spawnParent;

    [Header("Temporizador fase 3")]
    [SerializeField] float tiempoAntesDeEntrar = 10f;
    [SerializeField] bool cuentaRegresivaActiva = false;
    [SerializeField] float tiempoRestanteParaEntrar;
    [SerializeField] bool enemigoSpawned = false;

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
        ActivarModeloPorEstado(); // inicia el modelo correcto según estadoActual
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

        ActivarModeloPorEstado(); // activa el modelo correspondiente
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

        ActivarModeloPorEstado(); // activa el modelo correspondiente
    }
    void EntrarAHabitacion()
    {
        cuentaRegresivaActiva = false;
        tiempoRestanteParaEntrar = 0f;
        enemigoSpawned = true;

        ActualizarUIOjo();
        ReproducirAudio(clipEntrar);

        // 🔹 Pasar al nuevo estado 4
        estadoActual = 4;
        ActivarModeloPorEstado();

        // ⚠️ IMPORTANTE:
        // La animación de "entrar a la habitación" en enemigoEstado4
        // debe tener un Animation Event que llame a FinalizarAnimacionEntradaHabitacion()
    }
    public void FinalizarAnimacionEntradaHabitacion()
    {
        if (enemigoEstado4 != null)
            enemigoEstado4.SetActive(false);

        if (enemigoReal != null)
        {
            enemigoReal.SetActive(true);

            EnemigoPerseguidor script = enemigoReal.GetComponent<EnemigoPerseguidor>();
            if (script != null)
            {
                script.ResetEnemigo();

                GameObject jugador = GameObject.FindGameObjectWithTag("Player");
                if (jugador != null)
                    script.objetivo = jugador.transform;

                script.ActivarPersecucion();
            }
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

        ActivarModeloPorEstado(); // asegura que se muestre el modelo correcto

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
        if (enemigoEstado3 == enemigoObj || enemigoReal == enemigoObj)
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
    void ActivarModeloPorEstado()
    {
        DesactivarTodosModelos();

        if (estadoActual == 1 && enemigoEstado1 != null) enemigoEstado1.SetActive(true);
        if (estadoActual == 2 && enemigoEstado2 != null) enemigoEstado2.SetActive(true);
        if (estadoActual == 3 && enemigoEstado3 != null) enemigoEstado3.SetActive(true);
        if (estadoActual == 4 && enemigoEstado4 != null) enemigoEstado4.SetActive(true);
    }
    void DesactivarTodosModelos()
    {
        if (enemigoEstado1 != null) enemigoEstado1.SetActive(false);
        if (enemigoEstado2 != null) enemigoEstado2.SetActive(false);
        if (enemigoEstado3 != null) enemigoEstado3.SetActive(false);
        if (enemigoEstado4 != null) enemigoEstado4.SetActive(false);
        if (enemigoReal != null) enemigoReal.SetActive(false);
    }
}