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
    private Vector3 posicionInicial;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        posicionInicial = transform.position;

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

    public void SetVelocidadExtra()
    {
        velocidad *= 2f;
        Debug.Log("⚡ Enemigo activado con velocidad extra!");
    }

    //---------------Reset---------------
    public void ResetEnemigo()
    {
        transform.position = posicionInicial;
        velocidad = 3f;
        gameObject.SetActive(false); // 🔧 desactivado al reiniciar
    }
}
