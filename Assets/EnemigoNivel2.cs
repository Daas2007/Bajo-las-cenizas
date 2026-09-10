using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemigoNivel2 : MonoBehaviour
{
    [Header("Configuración de Persecución")]
    [SerializeField] private Transform Jugador;
    [SerializeField] private float distanciaAtaque = 2f;
    [SerializeField] private float tiempoEntreAtaques = 2.5f;

    [Header("Nombres de Parámetros del Animator")]
    [SerializeField] private string parametroCaminar = "estaCaminando";
    [SerializeField] private string parametroAtaque = "atacar";

    private NavMeshAgent agent;
    private Animator anim;

    private float cronometroAtaque = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // Buscar al jugador por Tag solo si no se asignó desde el Inspector
        if (Jugador == null)
        {
            GameObject objJugador = GameObject.FindWithTag("Player");
            if (objJugador != null)
            {
                Jugador = objJugador.transform;
            }
            else
            {
                Debug.LogError("[EnemigoNivel2] No se asignó 'Jugador' en el Inspector ni se encontró un objeto con el Tag 'Player'.");
            }
        }

        // Asegurar que el NavMeshAgent esté activo de inmediato
        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        Debug.Log("👹 EnemigoNivel2 activado y persiguiendo al jugador.");
    }

    private void Update()
    {
        if (Jugador == null || agent == null || !agent.enabled) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, Jugador.position);

        if (distanciaAlJugador <= distanciaAtaque)
        {
            // Detener el movimiento y cambiar animación a Idle
            agent.isStopped = true;
            anim.SetBool(parametroCaminar, false);

            cronometroAtaque += Time.deltaTime;
            if (cronometroAtaque >= tiempoEntreAtaques)
            {
                cronometroAtaque = 0f;
                EjecutarAtaque();
            }
        }
        else
        {
            // Perseguir al jugador
            agent.isStopped = false;
            agent.SetDestination(Jugador.position);

            // Activar la animación Caminar si hay movimiento en el NavMesh
            bool moviendose = agent.velocity.sqrMagnitude > 0.1f;
            anim.SetBool(parametroCaminar, moviendose);
        }
    }

    private void EjecutarAtaque()
    {
        // Mirar hacia el jugador al atacar
        Vector3 direccion = (Jugador.position - transform.position).normalized;
        direccion.y = 0;
        if (direccion != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direccion);
        }

        // Transición a la animación Ataque
        anim.SetTrigger(parametroAtaque);
        Debug.Log("⚔️ EnemigoNivel2 atacó al jugador.");
    }
}