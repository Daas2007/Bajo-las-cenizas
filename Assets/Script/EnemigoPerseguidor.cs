using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemigoPerseguidor : MonoBehaviour
{
    [Header("Movimiento")]
    public Transform objetivo;
    [SerializeField] float velocidad = 3f;
    [SerializeField] float distanciaAtaque = 1.2f;

    [Header("Configuración")]
    [SerializeField] float tiempoParaReinicio = 2f;

    bool jugadorMuerto = false;

    void Update()
    {
        if (objetivo == null || jugadorMuerto) return;

        // Seguir al jugador
        Vector3 direccion = (objetivo.position - transform.position).normalized;
        transform.position += direccion * velocidad * Time.deltaTime;
        transform.LookAt(objetivo);

        float distancia = Vector3.Distance(transform.position, objetivo.position);
        if (distancia <= distanciaAtaque)
        {
            MatarJugador();
        }
    }

    void MatarJugador()
    {
        if (jugadorMuerto) return;

        jugadorMuerto = true;
        Debug.Log("💀 El enemigo te atrapó. Reiniciando nivel...");
        Invoke(nameof(ReiniciarNivel), tiempoParaReinicio);
    }

    void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
