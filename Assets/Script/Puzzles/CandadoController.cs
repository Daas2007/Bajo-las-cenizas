using UnityEngine;
using UnityEngine.Events;

public class CandadoController : MonoBehaviour
{
    //---------------Configuración---------------
    [Header("Configuración")]
    [Min(1)] public int cantidadDigitos = 4; // número de dígitos del candado
    [Tooltip("Código correcto (debe coincidir con cantidadDigitos)")]
    public int[] codigoCorrecto; // combinación correcta

    //---------------Referencias---------------
    [Header("Referencias")]
    [Tooltip("Dígitos en orden de izquierda a derecha")]
    public CandadoDigito[] digitos; // cada rueda del candado

    //---------------Eventos---------------
    [Header("Eventos")]
    public UnityEvent AlDesbloquear;     // se dispara al acertar el código
    public UnityEvent AlIntentoFallido;  // se dispara al fallar el código

    //---------------Integración con puzzle---------------
    [Header("Integración con puzzle de caja")]
    [SerializeField] private CandadoPuzzle puzzle; // opcional, para abrir caja/fragmento

    //---------------Bloqueo de jugador/cámara---------------
    [Header("Bloqueo de jugador/cámara")]
    [SerializeField] private MonoBehaviour scriptMovimientoJugador; // tu script de movimiento
    [SerializeField] private MonoBehaviour scriptCamara;           // tu script de cámara

    private bool puzzleActivo = false;

    //---------------Validaciones iniciales---------------
    private void Awake()
    {
        if (digitos == null || digitos.Length != cantidadDigitos)
            Debug.LogWarning("La cantidad de dígitos no coincide con 'cantidadDigitos'.");
        if (codigoCorrecto == null || codigoCorrecto.Length != cantidadDigitos)
            Debug.LogWarning("El código correcto debe tener la misma cantidad de dígitos.");
    }

    //---------------Activar puzzle---------------
    public void ActivarPuzzle()
    {
        puzzleActivo = true;

        // Bloquear movimiento y cámara
        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = false;
        if (scriptCamara != null) scriptCamara.enabled = false;

        // Liberar cursor para interactuar con UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //---------------Desactivar puzzle---------------
    public void DesactivarPuzzle()
    {
        puzzleActivo = false;

        // Restaurar movimiento y cámara
        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = true;
        if (scriptCamara != null) scriptCamara.enabled = true;

        // Bloquear cursor para gameplay normal
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //---------------Verificar código---------------
    public void VerificarCodigo()
    {
        if (digitos == null || codigoCorrecto == null) return;
        if (digitos.Length != codigoCorrecto.Length) return;

        for (int i = 0; i < digitos.Length; i++)
        {
            if (digitos[i].valor != codigoCorrecto[i])
            {
                AlIntentoFallido?.Invoke(); // evento de fallo
                return;
            }
        }

        AlDesbloquear?.Invoke(); // evento de éxito

        if (puzzle != null)
            puzzle.Desbloquear();

        // desbloquear jugador al terminar puzzle
        DesactivarPuzzle();
    }

    //---------------Cambiar código---------------
    public void EstablecerCodigo(int[] nuevoCodigo)
    {
        if (nuevoCodigo == null || nuevoCodigo.Length != cantidadDigitos)
        {
            Debug.LogWarning("El nuevo código no coincide con 'cantidadDigitos'.");
            return;
        }
        codigoCorrecto = nuevoCodigo;
    }

    //---------------Obtener código actual---------------
    public int[] ObtenerCodigoActual()
    {
        int[] actual = new int[cantidadDigitos];
        for (int i = 0; i < cantidadDigitos; i++)
            actual[i] = digitos[i].valor;
        return actual;
    }
}
