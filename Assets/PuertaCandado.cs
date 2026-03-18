using UnityEngine;

public class PuertaCandado : MonoBehaviour, IInteractuable
{
    [Header("Configuración de apertura")]
    [SerializeField] private float anguloApertura = 90f;
    [SerializeField] private float velocidadRotacion = 2f;

    private bool abierta = false;
    private bool bloqueada = true; // ✅ por defecto bloqueada
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
            Debug.Log("🚪 La puerta está bloqueada por el candado.");
            return;
        }

        abierta = !abierta;
        rotObjetivo = abierta ? Quaternion.Euler(0, anguloApertura, 0) * rotInicial : rotInicial;
    }

    void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, rotObjetivo, Time.deltaTime * velocidadRotacion);
    }

    // ✅ Método para desbloquear desde el candado
    public void DesbloquearPuerta()
    {
        bloqueada = false;
        Debug.Log("🔓 Puerta desbloqueada, ahora puede abrirse.");
    }
}
