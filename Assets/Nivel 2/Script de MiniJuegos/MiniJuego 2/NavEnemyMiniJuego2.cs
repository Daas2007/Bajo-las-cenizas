
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavEnemyMiniJuego2 : MonoBehaviour
{
    [Header("Puntos de patrulla")]
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;

    [Header("Jugador")]
    [SerializeField] private Transform jugador;

    [Header("Velocidades")]
    [SerializeField] private float velocidadNormal = 3.5f;
    [SerializeField] private float velocidadLejana = 18f;

    [Header("Distancias de velocidad")]
    [SerializeField] private float distanciaVelocidadMaxima = 25f;
    [SerializeField] private float distanciaDesaceleracion = 12f;

    [Header("Configuración de patrulla")]
    [SerializeField] private float distanciaLlegada = 0.5f;
    [SerializeField] private float tiempoEspera = 1f;

    [Header("Suavizado")]
    [SerializeField] private float suavizadoVelocidad = 3f;

    private NavMeshAgent agente;
    private Transform destinoActual;
    private float tiempoRestante;
    private bool esperando;

    private void Awake()
    {
        agente = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (puntoA == null || puntoB == null)
        {
            Debug.LogError(
                "Asigna los puntos A y B en el Inspector.", this
            );
            enabled = false;
            return;
        }

        if (jugador == null)
        {
            Debug.LogError(
                "Asigna el Transform del jugador en el Inspector.", this
            );
            enabled = false;
            return;
        }

        if (!agente.isOnNavMesh)
        {
            Debug.LogError(
                "El enemigo no está colocado sobre el NavMesh.", this
            );
            enabled = false;
            return;
        }

        agente.speed = velocidadNormal;

        destinoActual = puntoB;
        agente.SetDestination(destinoActual.position);
    }

    private void Update()
    {
        ActualizarVelocidad();
        ActualizarPatrulla();
    }

    private void ActualizarVelocidad()
    {
        // Distancia horizontal entre el enemigo y el jugador.
        Vector3 posicionEnemigo = transform.position;
        Vector3 posicionJugador = jugador.position;

        posicionEnemigo.y = 0f;
        posicionJugador.y = 0f;

        float distancia = Vector3.Distance(
            posicionEnemigo, posicionJugador
        );

        // Lejos: velocidad máxima.
        // Cerca: velocidad normal.
        float factor = Mathf.InverseLerp(
            distanciaDesaceleracion,
            distanciaVelocidadMaxima,
            distancia
        );

        float velocidadObjetivo = Mathf.Lerp(
            velocidadNormal,
            velocidadLejana,
            factor
        );

        // Cambio progresivo de velocidad.
        agente.speed = Mathf.MoveTowards(
            agente.speed,
            velocidadObjetivo,
            suavizadoVelocidad * Time.deltaTime
        );
    }

    private void ActualizarPatrulla()
    {
        if (esperando)
        {
            tiempoRestante -= Time.deltaTime;

            if (tiempoRestante <= 0f)
            {
                esperando = false;
                CambiarDestino();
            }

            return;
        }

        if (agente.pathPending || !agente.hasPath)
            return;

        if (agente.remainingDistance <= distanciaLlegada)
        {
            esperando = true;
            tiempoRestante = tiempoEspera;
            agente.isStopped = true;
        }
    }

    private void CambiarDestino()
    {
        destinoActual = destinoActual == puntoA
            ? puntoB
            : puntoA;

        agente.isStopped = false;
        agente.SetDestination(destinoActual.position);
    }
}