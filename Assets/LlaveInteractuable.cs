using UnityEngine;

public class LlaveInteractuable : MonoBehaviour, IInteractuable
{
    [SerializeField] private CandadoPuerta candado;
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

    void Start()
    {
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador != null)
        {
            InteraccionJugador interaccion = jugador.GetComponent<InteraccionJugador>();
            if (interaccion != null)
                manoIzquierda = interaccion.GetManoIzquierda();
        }
    }

    public void Interactuar()
    {
        if (!enMano && manoIzquierda != null && manoIzquierda.childCount == 0)
        {
            enMano = true;
            transform.SetParent(manoIzquierda);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            rb.isKinematic = true;   // ✅ no usa física
            rb.useGravity = false;   // ✅ no cae
            col.enabled = false;     // ✅ no choca mientras está en la mano
        }
    }

    public bool EstaEnMano()
    {
        return enMano;
    }

    public void UsarEnCandado()
    {
        if (candado != null)
        {
            candado.DestruirCandado();
            Destroy(gameObject); // ✅ destruir llave
        }
    }
}
