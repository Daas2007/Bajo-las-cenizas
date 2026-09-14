using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MiniEnemigoController : MonoBehaviour
{
    [Header("Referencias del Objetivo")]
    [Tooltip("Arrastra aquí la TV Principal o asignará una con el Tag 'TVPrincipal'")]
    [SerializeField] private Transform televisorPrincipal;

    [Header("Ajustes de Ataque")]
    [SerializeField] private float distanciaParaAtacar = 1.5f;
    [SerializeField] private float tiempoEntreGolpes = 2f;
    [SerializeField] private int dañoPorGolpe = 1;

    [Header("Referencias de Spawn")]
    [Tooltip("Televisor secundario de donde salió este mini enemigo")]
    public Transform tvDeOrigen;

    private NavMeshAgent agent;
    private bool estaEnMano = false;
    private float cronometroAtaque = 0f;
    [SerializeField] private Transform manoJugador;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        // Al activarse, resetea estados y busca la TV principal si no la tiene
        estaEnMano = false;
        if (agent != null) agent.enabled = true;

        if (televisorPrincipal == null)
        {
            GameObject tvObj = GameObject.FindWithTag("TVPrincipal");
            if (tvObj != null) televisorPrincipal = tvObj.transform;
        }

        // Inicia el movimiento hacia la TV principal
        MoverHaciaTV();
    }

    private void Update()
    {
        // Si el jugador lo tiene cargado en la mano, sigue la posición de la mano
        if (estaEnMano)
        {
            if (manoJugador != null)
            {
                transform.position = manoJugador.position;
                transform.rotation = manoJugador.rotation;
            }
            return;
        }

        // Si se está moviendo hacia la TV
        if (televisorPrincipal != null && agent.enabled && agent.isOnNavMesh)
        {
            float distancia = Vector3.Distance(transform.position, televisorPrincipal.position);

            if (distancia <= distanciaParaAtacar)
            {
                // Detener movimiento y atacar la TV principal
                agent.isStopped = true;
                cronometroAtaque += Time.deltaTime;

                if (cronometroAtaque >= tiempoEntreGolpes)
                {
                    cronometroAtaque = 0f;
                    AtacarTelevisor();
                }
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(televisorPrincipal.position);
            }
        }
    }

    private void MoverHaciaTV()
    {
        if (televisorPrincipal != null && agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(televisorPrincipal.position);
        }
    }

    private void AtacarTelevisor()
    {
        // Esta función llamará al script de la TV Principal cuando lo creemos
        TelevisorPrincipal tv = televisorPrincipal.GetComponent<TelevisorPrincipal>();
        if (tv != null)
        {
            tv.RecibirDaño(dañoPorGolpe);
            Debug.Log($"👾 {gameObject.name} atacó al Televisor Principal.");
        }
    }

    // ------------------- IMPLEMENTACIÓN DE INTERACCIÓN -------------------


}