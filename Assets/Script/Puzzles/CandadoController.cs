using UnityEngine;
using UnityEngine.Events;

public class CandadoController : MonoBehaviour
{
    [Header("Configuración")]
    [Min(1)] public int cantidadDigitos = 4;
    [Tooltip("Código correcto (debe coincidir con cantidadDigitos)")]
    public int[] codigoCorrecto;

    [Header("Referencias")]
    [Tooltip("Dígitos en orden de izquierda a derecha")]
    public CandadoDigito[] digitos;

    [Header("Eventos")]
    public UnityEvent AlDesbloquear;
    public UnityEvent AlIntentoFallido;

    [Header("Integración con puzzle de caja")]
    [SerializeField] private CandadoPuzzle puzzle; // opcional, para llamar directamente

    private void Awake()
    {
        if (digitos == null || digitos.Length != cantidadDigitos)
            Debug.LogWarning("La cantidad de dígitos no coincide con 'cantidadDigitos'.");
        if (codigoCorrecto == null || codigoCorrecto.Length != cantidadDigitos)
            Debug.LogWarning("El código correcto debe tener la misma cantidad de dígitos.");
    }

    public void VerificarCodigo()
    {
        if (digitos == null || codigoCorrecto == null) return;
        if (digitos.Length != codigoCorrecto.Length) return;

        for (int i = 0; i < digitos.Length; i++)
        {
            if (digitos[i].valor != codigoCorrecto[i])
            {
                AlIntentoFallido?.Invoke();
                return;
            }
        }

        AlDesbloquear?.Invoke();

        if (puzzle != null)
            puzzle.Desbloquear();
    }

    public void EstablecerCodigo(int[] nuevoCodigo)
    {
        if (nuevoCodigo == null || nuevoCodigo.Length != cantidadDigitos)
        {
            Debug.LogWarning("El nuevo código no coincide con 'cantidadDigitos'.");
            return;
        }
        codigoCorrecto = nuevoCodigo;
    }

    public int[] ObtenerCodigoActual()
    {
        int[] actual = new int[cantidadDigitos];
        for (int i = 0; i < cantidadDigitos; i++)
            actual[i] = digitos[i].valor;
        return actual;
    }
}
