using UnityEngine;

public class PapelInteraccion : MonoBehaviour, IInteractuable
{
    [SerializeField] GameObject canvasPapel;
    public Light LuzInterracion;
    private bool yaInteractuado = false;

    // referencias runtime
    MovimientoPersonaje movimientoJugador;
    MonoBehaviour scriptCamara; // referencia genérica al script de control de cámara (ajusta nombre si tu script no se llama Camara)
    bool canvasAbierto = false;

    // para detectar muerte
    int muertesRegistradas = 0;

    private void Awake()
    {
        if (LuzInterracion != null) LuzInterracion.gameObject.SetActive(!yaInteractuado);
        if (canvasPapel != null) canvasPapel.SetActive(false);

        movimientoJugador = FindObjectOfType<MovimientoPersonaje>();

        // Intentamos obtener el script de cámara (ajusta "Camara" si tu script tiene otro nombre)
        if (movimientoJugador != null)
            scriptCamara = movimientoJugador.GetComponentInChildren<Camara>() as MonoBehaviour;

        if (scriptCamara == null)
            scriptCamara = FindObjectOfType<Camara>() as MonoBehaviour;

        // inicializar contador de muertes (si GameManager existe)
        if (GameManager.Instancia != null)
            muertesRegistradas = GameManager.Instancia.GetMuertes();
    }

    void Update()
    {
        // cerrar con ESC si el canvas está abierto
        if (canvasAbierto && Input.GetKeyDown(KeyCode.Escape))
        {
            CerrarCanvas(); // cierre normal (restaura control y bloquea cursor)
            return;
        }

        // detectar muerte: si aumentó el contador de muertes y el canvas está abierto, cerrarlo por muerte
        if (canvasAbierto && GameManager.Instancia != null)
        {
            int muertesActual = GameManager.Instancia.GetMuertes();
            if (muertesActual > muertesRegistradas)
            {
                // cerrar por muerte: ocultar canvas, dejar cursor visible y desbloqueado,
                // y NO reactivar movimiento ni cámara (para evitar que el jugador se mueva mientras está la pantalla de muerte)
                CerrarCanvasPorMuerte();
                muertesRegistradas = muertesActual;
            }
        }
    }

    public void Interactuar()
    {
        if (!yaInteractuado)
        {
            yaInteractuado = true;
            if (LuzInterracion != null) LuzInterracion.gameObject.SetActive(!yaInteractuado);
        }

        AbrirCanvas();
    }

    void AbrirCanvas()
    {
        if (canvasPapel == null) return;

        canvasPapel.SetActive(true);
        canvasAbierto = true;

        // Desactivar control de movimiento del jugador
        if (movimientoJugador != null)
            movimientoJugador.enabled = false;

        // Desactivar script de control de cámara
        if (scriptCamara != null)
            scriptCamara.enabled = false;

        // Limpiar velocidades si tiene Rigidbody
        if (movimientoJugador != null)
        {
            Rigidbody rb = movimientoJugador.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // evitar problemas con CharacterController
            CharacterController cc = movimientoJugador.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                cc.enabled = true;
            }
        }

        // Mostrar cursor y desbloquear
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // actualizar muertes registradas por si hubo cambios antes de abrir
        if (GameManager.Instancia != null)
            muertesRegistradas = GameManager.Instancia.GetMuertes();
    }

    // Cierre normal: restaura control y bloquea cursor (uso por botón o ESC)
    public void CerrarCanvas()
    {
        if (canvasPapel == null) return;

        canvasPapel.SetActive(false);
        canvasAbierto = false;

        // Reactivar control de movimiento del jugador
        if (movimientoJugador != null)
            movimientoJugador.enabled = true;

        // Reactivar script de cámara
        if (scriptCamara != null)
            scriptCamara.enabled = true;

        // Restaurar bloqueo del cursor para jugar
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // actualizar muertes registradas
        if (GameManager.Instancia != null)
            muertesRegistradas = GameManager.Instancia.GetMuertes();
    }

    // Cierre por muerte: ocultar canvas, dejar cursor visible y desbloqueado,
    // NO reactivar movimiento ni cámara (para que la pantalla de muerte controle el flujo)
    void CerrarCanvasPorMuerte()
    {
        if (canvasPapel == null) return;

        canvasPapel.SetActive(false);
        canvasAbierto = false;

        // NO reactivar movimiento ni cámara aquí

        // Dejar cursor visible y desbloqueado para que el jugador pueda usar la UI de muerte
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
