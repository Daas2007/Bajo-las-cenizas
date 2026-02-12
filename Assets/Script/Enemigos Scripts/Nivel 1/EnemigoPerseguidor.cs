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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Si olvidaste asignar la pantalla en el Inspector, la busca automáticamente
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
            // Activa pantalla de muerte
            pantallaDeMuerte.ActivarPantallaMuerte();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pantallaDeMuerte.ActivarPantallaMuerte();
        }
    }
}
