using UnityEngine;

public class LlaveInteractuable : MonoBehaviour, IInteractuable
{
    [SerializeField] public CandadoPuerta candado; // ✅ cada llave apunta a su candado
    private bool enMano = false;
    private Transform manoIzquierda;

    private Rigidbody rb;
    private Collider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        col = GetComponent<Collider>();
        if (col == null) col = gameObject.AddComponent<BoxCollider>();

        rb.useGravity = true;
        rb.isKinematic = false;
        col.enabled = true;
    }

    // ✅ Método para asignar la mano desde InteraccionJugador
    public void SetMano(Transform mano)
    {
        manoIzquierda = mano;
    }

    public void Interactuar()
    {
        if (!enMano && manoIzquierda != null && manoIzquierda.childCount == 0)
        {
            enMano = true;
            transform.SetParent(manoIzquierda);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            rb.isKinematic = true;
            rb.useGravity = false;
            col.enabled = false;
        }
    }

    public bool EstaEnMano() => enMano;

    public void UsarEnCandado()
    {
        if (candado != null)
        {
            candado.DestruirCandado();
            Destroy(gameObject); // ✅ destruir llave tras usarla
        }
    }

    public void Soltar()
    {
        enMano = false;
        transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = true;
        col.enabled = true;
    }
}
