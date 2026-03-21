using UnityEngine;

public class PapelInteraccion : MonoBehaviour, IInteractuable
{
    [SerializeField] GameObject canvasPapel;
    public Light LuzInterracion;
    private bool yaInteractuado = false;

    MovimientoPersonaje movimientoJugador;
    MonoBehaviour scriptCamara;
    bool canvasAbierto = false;

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
        if (!yaInteractuado)
        {
            yaInteractuado = true;
            if (LuzInterracion != null) LuzInterracion.gameObject.SetActive(false);
        }

        AbrirCanvas();
    }
    void AbrirCanvas()
    {
        if (canvasPapel == null) return;

        canvasPapel.SetActive(true);
        canvasAbierto = true;

        if (movimientoJugador != null) movimientoJugador.enabled = false;
        if (scriptCamara != null) scriptCamara.enabled = false;

        if (movimientoJugador != null)
        {
            Rigidbody rb = movimientoJugador.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            CharacterController cc = movimientoJugador.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                cc.enabled = true;
            }
        }

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