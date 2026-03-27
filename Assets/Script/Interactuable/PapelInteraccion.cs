using UnityEngine;

public class PapelInteraccion : MonoBehaviour, IInteractuable
{
    [SerializeField] GameObject canvasPapel;
    [SerializeField] private Dialogo dialogo;   // referencia al sistema de diálogo
    public Light LuzInterracion;

    private bool yaInteractuado = false;   // controla si ya se mostró el diálogo
    private bool canvasAbierto = false;

    MovimientoPersonaje movimientoJugador;
    MonoBehaviour scriptCamara;
    int muertesRegistradas = 0;

    private void Awake()
    {
        if (LuzInterracion != null) LuzInterracion.gameObject.SetActive(!yaInteractuado);
        if (canvasPapel != null) canvasPapel.SetActive(false);

        movimientoJugador = FindObjectOfType<MovimientoPersonaje>();
        if (movimientoJugador != null)
            scriptCamara = movimientoJugador.GetComponentInChildren<Camara>() as MonoBehaviour;

        if (scriptCamara == null)
            scriptCamara = FindObjectOfType<Camara>() as MonoBehaviour;

        if (GameManager.Instancia != null)
            muertesRegistradas = GameManager.Instancia.GetMuertes();
    }

    void Update()
    {
        if (canvasAbierto && Input.GetKeyDown(KeyCode.Escape))
        {
            CerrarCanvas();
            return;
        }

        if (canvasAbierto && GameManager.Instancia != null)
        {
            int muertesActual = GameManager.Instancia.GetMuertes();
            if (muertesActual > muertesRegistradas)
            {
                CerrarCanvasPorMuerte();
                muertesRegistradas = muertesActual;
            }
        }
    }

    public void Interactuar()
    {
        AbrirCanvas();
    }

    void AbrirCanvas()
    {
        if (canvasPapel == null) return;

        canvasPapel.SetActive(true);
        canvasAbierto = true;

        if (movimientoJugador != null) movimientoJugador.enabled = false;
        if (scriptCamara != null) scriptCamara.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (GameManager.Instancia != null)
            muertesRegistradas = GameManager.Instancia.GetMuertes();
    }

    public void CerrarCanvas()
    {
        if (canvasPapel == null) return;

        canvasPapel.SetActive(false);
        canvasAbierto = false;

        if (movimientoJugador != null) movimientoJugador.enabled = true;
        if (scriptCamara != null) scriptCamara.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (GameManager.Instancia != null)
            muertesRegistradas = GameManager.Instancia.GetMuertes();

        // 🔹 Mostrar diálogo SOLO la primera vez que se cierra el papel
        if (!yaInteractuado && dialogo != null)
        {
            yaInteractuado = true;
            dialogo.ResetHaHablado();
            dialogo.IniciarDialogo();
        }
    }

    void CerrarCanvasPorMuerte()
    {
        if (canvasPapel == null) return;

        canvasPapel.SetActive(false);
        canvasAbierto = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
