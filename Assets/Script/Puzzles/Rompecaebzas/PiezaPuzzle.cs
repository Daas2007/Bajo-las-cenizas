using UnityEngine;

public class PiezaPuzzle : MonoBehaviour, IInteractuable
{
    [Header("Configuración")]
    public int piezaID;
    public float toleranciaRotacion = 5f;

    private bool enMano = false;
    public bool colocada { get; private set; }
    private bool puedeRotarX = false;
    private Transform manoIzquierda;

    private Vector3 escalaOriginal;
    private Rigidbody rb;

    void Awake()
    {
        this.enabled = true;
        escalaOriginal = transform.localScale;
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>(); // asegura que tenga Rigidbody
        rb.useGravity = true;
        rb.isKinematic = false; // por defecto con física activa
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

    void Update()
    {
        if (enMano && !colocada)
        {
            if (Input.GetKeyDown(KeyCode.R))
                transform.Rotate(0, 0, -90);

            // 🔹 Soltar con Q
            if (Input.GetKeyDown(KeyCode.Q))
                Soltar();
        }

        if (colocada && puedeRotarX && Input.GetKeyDown(KeyCode.X))
        {
            transform.Rotate(90, 0, 0);
        }
    }

    public void Interactuar()
    {
        if (colocada) return;

        if (!enMano)
        {
            // ✅ Solo agarrar si la mano está vacía
            if (manoIzquierda.childCount == 0)
            {
                enMano = true;
                transform.SetParent(manoIzquierda);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = escalaOriginal;

                // 🔹 Desactivar física mientras está en la mano
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }
        else
        {
            Soltar();
        }
    }

    public void MarcarColocada(bool estado = true)
    {
        colocada = estado;
        enMano = false;

        // 🔹 Al colocar en un slot, desactivar física siempre
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void PermitirRotacionX(bool estado)
    {
        puedeRotarX = estado;
    }

    public void ResetColocada()
    {
        colocada = false;
        enMano = false;
        puedeRotarX = false;

        // 🔹 Al resetear, volver a activar física
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    public void Soltar()
    {
        enMano = false;
        transform.SetParent(null);

        // 🔹 Reactivar física para que caiga al piso
        rb.isKinematic = false;
        rb.useGravity = true;
    }
}
