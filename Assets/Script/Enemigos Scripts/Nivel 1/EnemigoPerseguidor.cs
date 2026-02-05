using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemigoPerseguidor : MonoBehaviour
{
    [Header("Objetivo a perseguir")]
    public Transform objetivo;

    [Header("Velocidad de movimiento")]
    [SerializeField] private float velocidad = 3f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // evitar que se caiga o rote raro
    }

    private void FixedUpdate()
    {
        if (objetivo == null) return;

        // Dirección hacia el jugador
        Vector3 direccion = (objetivo.position - transform.position).normalized;

        // Movimiento físico hacia el jugador
        rb.MovePosition(transform.position + direccion * velocidad * Time.fixedDeltaTime);
    }
}
