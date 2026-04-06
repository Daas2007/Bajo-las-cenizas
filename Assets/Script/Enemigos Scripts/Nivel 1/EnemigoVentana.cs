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

    [Header("Audio Ojo Irritado")]
    [SerializeField] private AudioSource audioSourceOjo;
    [SerializeField] private AudioClip clipOjo;
    [SerializeField] private float velocidadNormal = 1f;
    [SerializeField] private float velocidadRapida = 2f;

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
        ActivarModeloPorEstado();
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

        if (recibiendoLuz && estadoActual != 4) // ✅ ignorar luz en estado 4
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
                if (estadoActual < 3)
                {
                    AvanzarEstado();
                }
                else if (estadoActual == 3 && !enemigoSpawned)
                {
                    EntrarAHabitacion();
                }
            }
        }

        if (cuentaRegresivaActiva && !recibiendoLuz && !enemigoSpawned)
        {
            tiempoRestanteParaEntrar -= deltaT;
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

        ActivarModeloPorEstado();
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
        ActivarModeloPorEstado();
    }
    void EntrarAHabitacion()
    {
        cuentaRegresivaActiva = false;
        tiempoRestanteParaEntrar = 0f;
        enemigoSpawned = true;

        ActualizarUIOjo();
        ReproducirAudio(clipEntrar);

        estadoActual = 4;
        ActivarModeloPorEstado();
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

        enemigoSpawned = true;
    }
    public void StartTriggerSequence()
    {
        if (enemigoSpawned) return;

        cuentaRegresivaActiva = true;
        tiempoRestanteParaEntrar = tiempoAntesDeEntrar;

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
        if (estadoActual == 4) return; // ✅ no responder a la linterna en estado 4

        recibiendoLuz = valor;
        if (!valor) contadorLuz = 0f;
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

        ActivarModeloPorEstado();

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
        if (panelOjoEstado != null)
        {
            var img = panelOjoEstado.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                Color c = img.color;

                if (estadoActual == 1)
                {
                    panelOjoEstado.SetActive(false);
                    c.a = 0f;
                }
                else if (estadoActual == 2)
                {
                    panelOjoEstado.SetActive(true);
                    c.a = 0.05f; // 🔹 5% opacidad
                }
                else if (estadoActual == 3)
                {
                    panelOjoEstado.SetActive(true);
                    c.a = 0.3f; // 🔹 30% opacidad
                }
                else if (estadoActual == 4) // enemigo entra
                {
                    panelOjoEstado.SetActive(false);
                    c.a = 0f;
                }

                img.color = c;
            }
        }

        // 🔊 reproducir audio según estado
        ReproducirAudioOjo();
    }
    private void ReproducirAudioOjo()
    {
        if (audioSourceOjo == null || clipOjo == null) return;

        if (estadoActual == 2)
        {
            audioSourceOjo.pitch = velocidadNormal;
            if (!audioSourceOjo.isPlaying)
            {
                audioSourceOjo.clip = clipOjo;
                audioSourceOjo.loop = true; // 🔹 aseguramos que sea loop
                audioSourceOjo.Play();
            }
        }
        else if (estadoActual == 3)
        {
            audioSourceOjo.pitch = velocidadRapida;
            if (!audioSourceOjo.isPlaying)
            {
                audioSourceOjo.clip = clipOjo;
                audioSourceOjo.loop = true; // 🔹 aseguramos que sea loop
                audioSourceOjo.Play();
            }
        }
        else
        {
            audioSourceOjo.Stop();
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