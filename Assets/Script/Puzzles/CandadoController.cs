using UnityEngine;
using UnityEngine.Events;

public class CandadoController : MonoBehaviour
{
    [Header("Configuración")]
    [Min(1)] public int cantidadDigitos = 4;
    public int[] codigoCorrecto;

    [Header("Referencias")]
    public CandadoDigito[] digitos;

    [Header("Eventos")]
    public UnityEvent AlDesbloquear;
    public UnityEvent AlIntentoFallido;

    [Header("Integración con puzzle de caja")]
    [SerializeField] private CandadoPuzzle puzzle;

    [Header("Bloqueo de jugador/cámara")]
    [SerializeField] private MonoBehaviour scriptMovimientoJugador;
    [SerializeField] private MonoBehaviour scriptCamara;

    private bool puzzleActivo = false;

    private void Awake()
    {
        if (digitos == null || digitos.Length != cantidadDigitos)
            Debug.LogWarning("[CandadoController] La cantidad de dígitos no coincide con 'cantidadDigitos'.");

        if (codigoCorrecto == null || codigoCorrecto.Length != cantidadDigitos)
            Debug.LogWarning("[CandadoController] El código correcto debe tener la misma cantidad de dígitos.");

        // Conectar automáticamente el evento AlDesbloquear con puzzle.Desbloquear si hay puzzle asignado
        if (puzzle != null)
        {
            AlDesbloquear.RemoveListener(puzzle.Desbloquear);
            AlDesbloquear.AddListener(puzzle.Desbloquear);
            Debug.Log("[CandadoController] Listener agregado: AlDesbloquear -> puzzle.Desbloquear");
        }
    }

    public void ActivarPuzzle()
    {
        puzzleActivo = true;
        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = false;
        if (scriptCamara != null) scriptCamara.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("[CandadoController] Puzzle activado: jugador y cámara bloqueados, cursor liberado.");
    }

    public void DesactivarPuzzle()
    {
        puzzleActivo = false;
        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = true;
        if (scriptCamara != null) scriptCamara.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("[CandadoController] Puzzle desactivado: jugador y cámara restaurados, cursor bloqueado.");
    }

    public void VerificarCodigo()
    {
        if (digitos == null || codigoCorrecto == null)
        {
            Debug.LogWarning("[CandadoController] Digitos o codigoCorrecto no asignados.");
            return;
        }
        if (digitos.Length != codigoCorrecto.Length)
        {
            Debug.LogWarning("[CandadoController] digitos.Length != codigoCorrecto.Length");
            return;
        }

        for (int i = 0; i < digitos.Length; i++)
        {
            if (digitos[i].valor != codigoCorrecto[i])
            {
                Debug.Log("[CandadoController] Código incorrecto. Disparando AlIntentoFallido.");
                AlIntentoFallido?.Invoke();
                return;
            }
        }

        Debug.Log("[CandadoController] Código correcto. Disparando AlDesbloquear.");
        AlDesbloquear?.Invoke();

        // Llamada redundante segura al puzzle
        if (puzzle != null)
        {
            puzzle.Desbloquear();
        }

        DesactivarPuzzle();
    }

    public void EstablecerCodigo(int[] nuevoCodigo)
    {
        if (nuevoCodigo == null || nuevoCodigo.Length != cantidadDigitos)
        {
            Debug.LogWarning("[CandadoController] El nuevo código no coincide con 'cantidadDigitos'.");
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

