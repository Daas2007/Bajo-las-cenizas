using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Animator))]
public class EnemigoPerseguidor : MonoBehaviour
{
    public Transform objetivo;
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private PantallaDeMuerte pantallaDeMuerte;

    private Animator animator;
    private Vector3 posicionInicialLocal;
    private Quaternion rotInicialLocal;

    private enum Estado { Idle, Persiguiendo, Atacando }
    private Estado estadoActual = Estado.Idle;

    private void OnEnable()
    {
        if (!enabled) enabled = true;

        if (objetivo != null)
            estadoActual = Estado.Persiguiendo; // ✅ empieza persiguiendo si ya tiene objetivo
        else
            estadoActual = Estado.Idle;

        ActualizarAnimacion();
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();

        posicionInicialLocal = transform.localPosition;
        rotInicialLocal = transform.localRotation;

        if (pantallaDeMuerte == null)
            pantallaDeMuerte = Object.FindFirstObjectByType<PantallaDeMuerte>();
    }

    private void FixedUpdate()
    {
        if (objetivo == null) return;

        switch (estadoActual)
        {
            case Estado.Persiguiendo:
                Perseguir();
                break;
            case Estado.Atacando:
                // lógica de ataque si quieres
                break;
            case Estado.Idle:
                // quieto en su posición inicial
                break;
        }
    }

    private void Perseguir()
    {
        if (estadoActual != Estado.Persiguiendo) return;

        animator.SetBool("IsWalking", true);

        Vector3 direccion = (objetivo.position - transform.position).normalized;
        float distancia = Vector3.Distance(objetivo.position, transform.position);

        // rotar hacia el jugador
        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, 5f * Time.fixedDeltaTime);

        // avanzar hacia el jugador
        if (distancia > 1.2f)
        {
            transform.position += direccion * velocidad * Time.fixedDeltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            estadoActual = Estado.Atacando;
            ActualizarAnimacion();
            GameManager.Instancia?.RegistrarMuerte();
            pantallaDeMuerte?.ActivarPantallaMuerte();
            Debug.Log("HAS MUERTO");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            estadoActual = Estado.Atacando;
            ActualizarAnimacion();
            GameManager.Instancia?.RegistrarMuerte();
            pantallaDeMuerte?.ActivarPantallaMuerte();
            Debug.Log("HAS MUERTO");
        }
    }

    public void ResetEnemigo()
    {
        transform.localPosition = posicionInicialLocal;
        transform.localRotation = rotInicialLocal;

        estadoActual = Estado.Idle;
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
        ActualizarAnimacion();
    }
}