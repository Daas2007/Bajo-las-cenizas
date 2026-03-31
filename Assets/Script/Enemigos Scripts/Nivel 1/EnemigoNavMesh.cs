using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider), typeof(Animator), typeof(NavMeshAgent))]
public class EnemigoNavMesh : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform objetivo; // 🔹 asigna el jugador en el inspector
    [SerializeField] private PantallaDeMuerte pantallaDeMuerte;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoMuerte;

    private Animator animator;
    private NavMeshAgent agente;
    private Vector3 posicionInicial;
    private Quaternion rotInicial;

    private enum Estado { Idle, Persiguiendo, Atacando }
    private Estado estadoActual = Estado.Idle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agente = GetComponent<NavMeshAgent>();

        // Guardar posición inicial en coordenadas globales
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
    }

    private void OnEnable()
    {
        agente.isStopped = true; // 🔹 detenido al inicio
        agente.ResetPath();
        estadoActual = Estado.Idle;
        ActualizarAnimacion();

        // 🔹 Si ya tienes asignado el jugador como objetivo, empieza persiguiendo
        if (objetivo != null)
        {
            ActivarPersecucion();
        }
    }

    private void Update()
    {
        if (objetivo == null) return;

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
        agente.SetDestination(objetivo.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MatarJugador();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
        estadoActual = Estado.Persiguiendo;
        agente.isStopped = false;
        ActualizarAnimacion();
    }
}
