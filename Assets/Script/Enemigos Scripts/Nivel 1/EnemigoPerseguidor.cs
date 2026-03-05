using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class EnemigoPerseguidor : MonoBehaviour
{
    [Header("Objetivo a perseguir")]
    public Transform objetivo;

    [Header("Velocidad de movimiento")]
    [SerializeField] private float velocidad = 3f;

    [Header("Pantalla de Muerte")]
    [SerializeField] private PantallaDeMuerte pantallaDeMuerte;

    private Rigidbody rb;

    // Guardamos la posición/rotación local inicial (relativa al parent)
    private Vector3 posicionInicialLocal;
    private Quaternion rotInicialLocal;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Guardar localPosition/localRotation para respetar el parent
        posicionInicialLocal = transform.localPosition;
        rotInicialLocal = transform.localRotation;

        if (pantallaDeMuerte == null)
        {
            pantallaDeMuerte = Object.FindFirstObjectByType<PantallaDeMuerte>();
        }
    }

    private void FixedUpdate()
    {
        if (objetivo == null) return;

        Vector3 direccion = (objetivo.position - transform.position).normalized;
        rb.MovePosition(transform.position + direccion * velocidad * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (pantallaDeMuerte != null) pantallaDeMuerte.ActivarPantallaMuerte();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pantallaDeMuerte != null) pantallaDeMuerte.ActivarPantallaMuerte();
        }
    }

    public void SetVelocidadExtra()
    {
        velocidad *= 2f;
        Debug.Log("⚡ Enemigo activado con velocidad extra!");
    }

    // Reset seguro que respeta el parent y usa localPosition/localRotation
    public void ResetEnemigo()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        // Limpiar velocidades
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Guardar estado físico previo
        bool prevKinematic = rb.isKinematic;
        bool prevUseGravity = rb.useGravity;

        // Desactivar física temporalmente para reposicionar sin que caiga
        rb.isKinematic = true;
        rb.useGravity = false;

        // Reposicionar relativo al parent (si tiene parent, usa localPosition guardada)
        transform.localPosition = posicionInicialLocal;
        transform.localRotation = rotInicialLocal;

        // Restaurar estado físico
        rb.isKinematic = prevKinematic;
        rb.useGravity = prevUseGravity;

        // Asegurar que esté activo
        gameObject.SetActive(true);

        Debug.Log("[EnemigoPerseguidor] Reset: reposicionado en localPosition inicial.");
    }
}
