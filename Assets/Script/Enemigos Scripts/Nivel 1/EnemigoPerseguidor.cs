using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class EnemigoPerseguidor : MonoBehaviour
{
    public Transform objetivo;
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private PantallaDeMuerte pantallaDeMuerte;

    private Rigidbody rb;
    private Vector3 posicionInicialLocal;
    private Quaternion rotInicialLocal;

    private enum Estado { Idle, Persiguiendo, Atacando }
    private Estado estadoActual = Estado.Idle;

    private void OnEnable()
    {
        // ✅ Siempre que el GameObject se active, el script queda habilitado
        if (!enabled) enabled = true;

        // Opcional: reiniciar estado si quieres que siempre arranque limpio
        estadoActual = Estado.Idle;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

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
                // Aquí podrías poner animación o lógica de ataque
                break;
        }
    }

    private void Perseguir()
    {
        Vector3 direccion = (objetivo.position - transform.position).normalized;
        float distancia = Vector3.Distance(objetivo.position, transform.position);

        // Rotación suave hacia el jugador
        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, 5f * Time.fixedDeltaTime);

        // Movimiento solo si está a cierta distancia
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
            pantallaDeMuerte?.ActivarPantallaMuerte();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            estadoActual = Estado.Atacando;
            pantallaDeMuerte?.ActivarPantallaMuerte();
        }
    }

    public void ActivarPersecucion()
    {
        estadoActual = Estado.Persiguiendo;
    }

    public void ResetEnemigo()
    {
        rb.linearVelocity = Vector3.zero; // ✅ corregido
        rb.angularVelocity = Vector3.zero;

        bool prevKinematic = rb.isKinematic;
        bool prevUseGravity = rb.useGravity;

        rb.isKinematic = true;
        rb.useGravity = false;

        transform.localPosition = posicionInicialLocal;
        transform.localRotation = rotInicialLocal;

        rb.isKinematic = prevKinematic;
        rb.useGravity = prevUseGravity;

        estadoActual = Estado.Idle;
    }

}
