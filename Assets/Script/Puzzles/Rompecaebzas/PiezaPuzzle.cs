using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
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
    private BoxCollider boxCol; // 🔹 siempre será BoxCollider

    void Awake()
    {
        this.enabled = true;
        escalaOriginal = transform.localScale;

        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        boxCol = GetComponent<BoxCollider>();

        rb.useGravity = true;
        rb.isKinematic = false;
        boxCol.enabled = true;
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
            if (manoIzquierda != null && manoIzquierda.childCount == 0)
            {
                enMano = true;
                transform.SetParent(manoIzquierda);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = escalaOriginal;

                rb.isKinematic = true;
                rb.useGravity = false;
                boxCol.enabled = false;

                gameObject.tag = "Puzzle";
            }
            else
            {
                Debug.LogWarning("[PiezaPuzzle] manoIzquierda no asignada o ya ocupada.");
            }
        }
        else
        {
            Soltar();
        }
    }

    // 🔹 Ajustado: desactiva el BoxCollider si está en el slot correcto
    public void MarcarColocada(bool estado = true, int slotID = -1)
    {
        colocada = estado;
        enMano = false;

        rb.isKinematic = true;
        rb.useGravity = false;

        boxCol.enabled = true;

        if (estado && slotID == piezaID)
        {
            boxCol.enabled = false; // ✅ desactivar collider
            gameObject.tag = "Untagged";
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    public void SetMano(Transform mano)
    {
        manoIzquierda = mano;
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
        boxCol.enabled = true;

        gameObject.tag = "Puzzle";
    }

    public void Soltar()
    {
        enMano = false;
        transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = true;
        boxCol.enabled = true;

        gameObject.tag = "Puzzle";
    }
}
