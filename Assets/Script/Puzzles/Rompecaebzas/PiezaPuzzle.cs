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
    private Collider col;

    void Awake()
    {
        this.enabled = true;
        escalaOriginal = transform.localScale;

        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        col = GetComponent<Collider>();
        if (col == null) col = gameObject.AddComponent<BoxCollider>();

        rb.useGravity = true;
        rb.isKinematic = false;
        if (col != null) col.enabled = true;
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
            if (manoIzquierda.childCount == 0)
            {
                enMano = true;
                transform.SetParent(manoIzquierda);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = escalaOriginal;

                rb.isKinematic = true;
                rb.useGravity = false;
                if (col != null) col.enabled = false;

                gameObject.tag = "Puzzle";
            }
        }
        else
        {
            Soltar();
        }
    }

    // 🔹 Ajustado: neutraliza la pieza si está en el slot correcto
    public void MarcarColocada(bool estado = true, int slotID = -1)
    {
        colocada = estado;
        enMano = false;

        rb.isKinematic = true;
        rb.useGravity = false;

        if (col != null) col.enabled = true;

        // ✅ Si la pieza está en el slot correcto → neutralizar
        if (estado && slotID == piezaID)
        {
            if (col != null) col.enabled = false; // desactivar collider
            gameObject.tag = "Untagged";          // quitar tag
            gameObject.layer = LayerMask.NameToLayer("Default"); // layer default
        }
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

        rb.isKinematic = false;
        rb.useGravity = true;
        if (col != null) col.enabled = true;

        gameObject.tag = "Puzzle";
    }

    public void Soltar()
    {
        enMano = false;
        transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = true;
        if (col != null) col.enabled = true;

        gameObject.tag = "Puzzle";
    }
}