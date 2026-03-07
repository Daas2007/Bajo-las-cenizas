using UnityEngine;
using UnityEngine.Events;

public class CandadoController : MonoBehaviour
{
    [Header("Configuración")]
    [Min(1)] public int cantidadDigitos = 4;
    public int[] codigoCorrecto;

    [Header("Referencias")]
    public CandadoDigito[] digitos;

    [Header("UI")]
    [Tooltip("Panel UI del puzzle. Se activará/desactivará desde aquí (fallback).")]
    public GameObject panelPuzzle;

    [Header("Eventos")]
    public UnityEvent AlDesbloquear;
    public UnityEvent AlIntentoFallido;

    [Header("Integración con puzzle de caja")]
    [SerializeField] private CandadoPuzzle puzzle;

    [Header("Bloqueo de jugador/cámara")]
    [SerializeField] private MonoBehaviour scriptMovimientoJugador;
    [SerializeField] private MonoBehaviour scriptCamara;

    private bool puzzleActivo = false;
    private PuzzleUI puzzleUI;

    private void Awake()
    {
        if (digitos == null || digitos.Length != cantidadDigitos)
            Debug.LogWarning("[CandadoController] La cantidad de dígitos no coincide con 'cantidadDigitos'.");

        if (codigoCorrecto == null || codigoCorrecto.Length != cantidadDigitos)
            Debug.LogWarning("[CandadoController] El código correcto debe tener la misma cantidad de dígitos.");

        if (puzzle != null)
        {
            AlDesbloquear.RemoveListener(puzzle.Desbloquear);
            AlDesbloquear.AddListener(puzzle.Desbloquear);
            Debug.Log("[CandadoController] Listener agregado: AlDesbloquear -> puzzle.Desbloquear");
        }

        // intentar encontrar PuzzleUI en el panel asignado
        if (panelPuzzle != null)
        {
            puzzleUI = panelPuzzle.GetComponentInChildren<PuzzleUI>();
            if (puzzleUI == null)
            {
                Debug.Log("[CandadoController] No se encontró PuzzleUI en panelPuzzle (opcional).");
            }
        }

        // Suscribir AlIntentoFallido a mostrar feedback
        AlIntentoFallido.RemoveAllListeners();
        AlIntentoFallido.AddListener(() =>
        {
            if (puzzleUI != null) puzzleUI.MostrarFeedback("Código incorrecto", 2f);
            else Debug.Log("[CandadoController] Intento fallido: Código incorrecto.");
        });
    }

    public void ActivarPuzzle()
    {
        if (puzzleActivo)
        {
            Debug.Log("[CandadoController] ActivarPuzzle llamado pero el puzzle ya está activo.");
            return;
        }

        puzzleActivo = true;

        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = false;
        if (scriptCamara != null) scriptCamara.enabled = false;

        // Intentar mostrar el panel a través del CanvasController (centraliza UI)
        CanvasController canvas = FindObjectOfType<CanvasController>();
        if (canvas != null)
        {
            canvas.MostrarPanelCandado();
        }
        else if (panelPuzzle != null)
        {
            // Fallback: si no hay CanvasController, activar panel local
            panelPuzzle.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("[CandadoController] No se encontró CanvasController ni panelPuzzle asignado.");
        }

        Debug.Log("[CandadoController] Puzzle activado: jugador y cámara bloqueados.");
    }

    public void DesactivarPuzzle()
    {
        if (!puzzleActivo)
        {
            Debug.Log("[CandadoController] DesactivarPuzzle llamado pero el puzzle no estaba activo.");
        }

        puzzleActivo = false;

        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = true;
        if (scriptCamara != null) scriptCamara.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Aseguramos que el panel del puzzle se cierre si está asignado localmente
        if (panelPuzzle != null) panelPuzzle.SetActive(false);

        // Restaurar tiempo por seguridad
        Time.timeScale = 1f;

        Debug.Log("[CandadoController] Puzzle desactivado: jugador y cámara restaurados.");
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
