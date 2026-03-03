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

    // ✅ Guardamos la escala original
    private Vector3 escalaOriginal;

    void Awake()
    {
        this.enabled = true;
        escalaOriginal = transform.localScale; // guardamos la escala inicial
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

                // ✅ Restaurar la escala original al agarrar
                transform.localScale = escalaOriginal;
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
    }

    public void Soltar()
    {
        enMano = false;
        transform.SetParent(null);
    }
}
