using UnityEngine;

public class PuertaCandado : MonoBehaviour, IInteractuable
{
    [SerializeField] private CandadoPuerta candadoDerecho;
    [SerializeField] private CandadoPuerta candadoIzquierdo;

    [SerializeField] private float anguloApertura = 90f;
    [SerializeField] private float velocidadRotacion = 2f;

    private bool abierta = false;
    private bool bloqueada = true;
    private Quaternion rotInicial;
    private Quaternion rotObjetivo;

    void Start()
    {
        rotInicial = transform.rotation;
        rotObjetivo = rotInicial;
    }

    public void Interactuar()
    {
        if (bloqueada)
        {
            Debug.Log("🚪 La puerta está bloqueada por los candados.");
            return;
        }

        abierta = !abierta;
        rotObjetivo = abierta ? Quaternion.Euler(0, anguloApertura, 0) * rotInicial : rotInicial;
    }

    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, rotObjetivo, Time.deltaTime * velocidadRotacion);
    }

    public void RevisarCandados()
    {
        // ✅ La puerta se desbloquea solo si ambos candados fueron destruidos
        if (candadoDerecho == null && candadoIzquierdo == null)
        {
            bloqueada = false;
            Debug.Log("🔓 Ambos candados destruidos, la puerta puede abrirse.");
        }
    }
}
