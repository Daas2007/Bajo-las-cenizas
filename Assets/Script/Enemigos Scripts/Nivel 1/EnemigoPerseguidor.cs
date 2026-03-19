using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(Animator))]
public class EnemigoPerseguidor : MonoBehaviour
{
    public Transform objetivo;
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private PantallaDeMuerte pantallaDeMuerte;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 posicionInicialLocal;
    private Quaternion rotInicialLocal;

    private enum Estado { Entrando, Persiguiendo, Atacando }
    private Estado estadoActual = Estado.Entrando;

    private void OnEnable()
    {
        if (!enabled) enabled = true;

        estadoActual = Estado.Entrando;
        ActualizarAnimacion();

        // 🔒 Desactivar física durante la animación de entrada
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

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
                // Aquí podrías poner lógica adicional de ataque
                break;
            case Estado.Entrando:
                // Quieto en su posición inicial
                transform.localPosition = posicionInicialLocal;
                break;
        }
    }

    private void Perseguir()
    {
        if (estadoActual != Estado.Persiguiendo) return;

        if (animator != null)
            animator.SetBool("IsWalking", true);

        Vector3 direccion = (objetivo.position - transform.position).normalized;
        float distancia = Vector3.Distance(objetivo.position, transform.position);

        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, 5f * Time.fixedDeltaTime);

        if (distancia > 1.2f)
        {
            rb.MovePosition(transform.position + direccion * velocidad * Time.fixedDeltaTime);
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
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
        rb.useGravity = false;

        transform.localPosition = posicionInicialLocal;
        transform.localRotation = rotInicialLocal;

        estadoActual = Estado.Entrando;
        ActualizarAnimacion();
    }

    private void ActualizarAnimacion()
    {
        if (animator == null) return;

        animator.SetBool("IsWalking", estadoActual == Estado.Persiguiendo);
        animator.SetBool("IsKilling", estadoActual == Estado.Atacando);
        animator.SetBool("IsEntering", estadoActual == Estado.Entrando);
    }

    // ✅ Método llamado desde Animation Event al final de la animación de entrada
    public void TerminarEntrada()
    {
        // 🔓 Reactivar física al terminar la animación
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (objetivo != null)
        {
            estadoActual = Estado.Persiguiendo;
            ActualizarAnimacion();
            Perseguir(); // empieza a moverse inmediatamente
        }
        else
        {
            estadoActual = Estado.Persiguiendo;
            ActualizarAnimacion();
        }
    }

    public void ActivarPersecucion()
    {
        if (estadoActual == Estado.Entrando) return;
        estadoActual = Estado.Persiguiendo;
        ActualizarAnimacion();
    }
}
