using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class EnemigoNavMesh : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform objetivo; // Jugador
    [SerializeField] private PantallaDeMuerte pantallaDeMuerte;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 3f;
    private NavMeshAgent agente;

    [Header("Detección / Área de matar")]
    [SerializeField] private BoxCollider areaDeteccion; // 🔹 arrástralo en el inspector

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoMuerte;

    private Animator animator;
    private Vector3 posicionInicial;
    private Quaternion rotInicial;

    private enum Estado { Idle, Persiguiendo, Atacando }
    private Estado estadoActual = Estado.Idle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agente = GetComponent<NavMeshAgent>();

        posicionInicial = transform.position;
        rotInicial = transform.rotation;

        if (pantallaDeMuerte == null)
            pantallaDeMuerte = Object.FindFirstObjectByType<PantallaDeMuerte>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        audioSource.ignoreListenerPause = true;

        // 🔹 Configurar el área de detección
        if (areaDeteccion != null)
        {
            areaDeteccion.isTrigger = true; // debe ser trigger
        }
    }

    private void OnEnable()
    {
        agente.isStopped = true;
        agente.ResetPath();
        estadoActual = Estado.Idle;
        ActualizarAnimacion();

        if (objetivo != null)
        {
            ActivarPersecucion();
        }
    }

    private void Update()
    {
        if (objetivo == null) return;

        agente.speed = velocidad;

        switch (estadoActual)
        {
            case Estado.Persiguiendo:
                Perseguir();
                break;
            case Estado.Atacando:
                agente.isStopped = true;
                break;
            case Estado.Idle:
                agente.isStopped = true;
                break;
        }
    }

    private void Perseguir()
    {
        if (estadoActual != Estado.Persiguiendo || objetivo == null) return;

        animator.SetBool("IsWalking", true);

        agente.isStopped = false;

        if (agente.isOnNavMesh)
        {
            agente.SetDestination(objetivo.position);
        }
    }

    // 🔹 El área de detección se encarga de matar
    private void OnTriggerEnter(Collider other)
    {
        if (areaDeteccion != null && other == areaDeteccion) return; // evitar auto-trigger

        if (other.CompareTag("Player"))
        {
            MatarJugador();
        }
    }

    private void MatarJugador()
    {
        estadoActual = Estado.Atacando;
        ActualizarAnimacion();

        GameManager.Instancia?.RegistrarMuerte();
        pantallaDeMuerte?.ActivarPantallaMuerte();

        if (sonidoMuerte != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoMuerte);
        }

        Debug.Log("HAS MUERTO");
    }

    public void ResetEnemigo()
    {
        transform.position = posicionInicial;
        transform.rotation = rotInicial;

        estadoActual = Estado.Idle;
        agente.isStopped = true;
        agente.ResetPath();
        ActualizarAnimacion();
    }

    private void ActualizarAnimacion()
    {
        if (animator == null) return;

        animator.SetBool("IsWalking", estadoActual == Estado.Persiguiendo);
        animator.SetBool("IsKilling", estadoActual == Estado.Atacando);
    }

    public void ActivarPersecucion()
    {
        if (objetivo == null)
        {
            Debug.LogWarning("⚠️ No se asignó objetivo al enemigo.");
            return;
        }

        estadoActual = Estado.Persiguiendo;
        agente.isStopped = false;
        ActualizarAnimacion();
    }
}
