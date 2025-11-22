using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemigoPerseguidor : MonoBehaviour
{
    public Transform objetivo;
    [SerializeField] float distanciaAtaque = 1.2f;
    [SerializeField] float tiempoParaReinicio = 2f;

    NavMeshAgent agent;
    bool jugadorMuerto = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = distanciaAtaque * 0.9f;
    }

    void Update()
    {
        if (objetivo == null || jugadorMuerto) return;
        agent.SetDestination(objetivo.position);

        if (!jugadorMuerto && Vector3.Distance(transform.position, objetivo.position) <= distanciaAtaque)
            MatarJugador();
    }

    void MatarJugador()
    {
        jugadorMuerto = true;
        agent.isStopped = true;
        Invoke(nameof(ReiniciarNivel), tiempoParaReinicio);
    }

    void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
