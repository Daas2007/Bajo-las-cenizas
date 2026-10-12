
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ControladorAparicionEnemigo : MonoBehaviour
{
    [Header("Enemigo")]
    [SerializeField] private GameObject enemigo;
    [SerializeField] private NavMeshAgent agente;

    [Header("Puntos fuera de pantalla")]
    [SerializeField] private Transform entradaIzquierda;
    [SerializeField] private Transform salidaDerecha;
    [SerializeField] private Transform entradaDerecha;
    [SerializeField] private Transform salidaIzquierda;

    [Header("Velocidades")]
    [SerializeField] private float velocidadRapida = 18f;
    [SerializeField] private float velocidadNormal = 3.5f;

    private bool ocupado;

    private void Awake()
    {
        if (agente == null && enemigo != null)
            agente = enemigo.GetComponent<NavMeshAgent>();

        if (enemigo != null)
            enemigo.SetActive(false);
    }

    public void AparecerDesdeIzquierda()
    {
        IniciarCruce(entradaIzquierda, salidaDerecha);
    }

    public void AparecerDesdeDerecha()
    {
        IniciarCruce(entradaDerecha, salidaIzquierda);
    }

    private void IniciarCruce(Transform entrada, Transform salida)
    {
        if (ocupado || enemigo == null || agente == null ||
            entrada == null || salida == null)
        {
            Debug.LogWarning(
                "Falta asignar una referencia del enemigo o los puntos.",
                this
            );
            return;
        }

        StartCoroutine(CruzarPantalla(entrada, salida));
    }

    private IEnumerator CruzarPantalla(
        Transform entrada, Transform salida)
    {
        ocupado = true;

        enemigo.transform.SetPositionAndRotation(
            entrada.position, entrada.rotation
        );

        enemigo.SetActive(true);

        yield return null;

        if (agente == null || !agente.isOnNavMesh)
        {
            Debug.LogError(
                "El enemigo no está sobre el NavMesh."
            );

            enemigo.SetActive(false);
            ocupado = false;
            yield break;
        }

        agente.ResetPath();
        agente.isStopped = false;
        agente.speed = velocidadRapida;
        agente.SetDestination(salida.position);

        while (agente.pathPending)
            yield return null;

        if (!agente.hasPath ||
            agente.pathStatus != NavMeshPathStatus.PathComplete)
        {
            Debug.LogWarning("No hay una ruta completa hasta la salida.");
            enemigo.SetActive(false);
            ocupado = false;
            yield break;
        }

        while (enemigo.activeSelf &&
               agente.isOnNavMesh &&
               (agente.pathPending ||
                agente.remainingDistance >
                Mathf.Max(agente.stoppingDistance, 0.1f)))
        {
            yield return null;
        }

        // Si llegó al destino sin haber sido desactivado por la zona.
        if (enemigo != null && enemigo.activeSelf)
            enemigo.SetActive(false);

        ocupado = false;
    }

    public void CambiarVelocidadNormal()
    {
        if (enemigo != null && enemigo.activeSelf &&
            agente != null)
        {
            agente.speed = velocidadNormal;
        }
    }

    public void DesactivarEnemigo()
    {
        if (enemigo != null && enemigo.activeSelf)
            enemigo.SetActive(false);

        ocupado = false;
    }

    public void PonerVelocidadNormal()
    {
        if (enemigo != null && enemigo.activeSelf && agente != null)
        {
            agente.speed = velocidadNormal;
        }
    }

    public void Desactivar()
    {
        if (enemigo != null && enemigo.activeSelf)
        {
            enemigo.SetActive(false);
        }

        ocupado = false;
    }
    public bool EsEnemigo(Collider other)
    {
        return enemigo != null &&
               (other.gameObject == enemigo ||
                other.transform.IsChildOf(enemigo.transform));
    }
}