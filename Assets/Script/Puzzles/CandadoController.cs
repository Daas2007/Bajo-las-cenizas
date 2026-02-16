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

    [Header("Bloqueo de jugador/cámara")]
    [SerializeField] private MonoBehaviour scriptMovimientoJugador; // tu script de movimiento
    [SerializeField] private MonoBehaviour scriptCamara;           // tu script de cámara

    private bool puzzleActivo = false;

    private void Awake()
    {
        if (digitos == null || digitos.Length != cantidadDigitos)
            Debug.LogWarning("La cantidad de dígitos no coincide con 'cantidadDigitos'.");
        if (codigoCorrecto == null || codigoCorrecto.Length != cantidadDigitos)
            Debug.LogWarning("El código correcto debe tener la misma cantidad de dígitos.");
    }

    // -------------------
    // Activar puzzle
    // -------------------
    public void ActivarPuzzle()
    {
        puzzleActivo = true;

        // Bloquear movimiento y cámara
        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = false;
        if (scriptCamara != null) scriptCamara.enabled = false;

        // Liberar el cursor para interactuar con el UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // -------------------
    // Desactivar puzzle
    // -------------------
    public void DesactivarPuzzle()
    {
        puzzleActivo = false;

        // Restaurar movimiento y cámara
        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = true;
        if (scriptCamara != null) scriptCamara.enabled = true;

        // Volver a bloquear el cursor para gameplay normal
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // -------------------
    // Verificar código
    // -------------------
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

        // 🔑 Al terminar el puzzle, desbloqueamos al jugador
        DesactivarPuzzle();
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

