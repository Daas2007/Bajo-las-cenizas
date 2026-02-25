using UnityEngine;

public class ControladorPuzzle : MonoBehaviour
{
    public static ControladorPuzzle Instancia;

    //---------------Referencias---------------
    [Header("Referencias")]
    public MonoBehaviour scriptMovimientoJugador; // arrastra tu script de movimiento
    public MonoBehaviour scriptControlCamara;     // arrastra tu script de cámara

    private bool enPuzzle = false;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (Instancia != null)
            Instancia.SalirModoPuzzle();
    }

    //---------------Entrar puzzle---------------
    public void EntrarModoPuzzle()
    {
        if (enPuzzle) return;
        enPuzzle = true;
        Debug.Log("Puzzle activado");

        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = false;
        if (scriptControlCamara != null) scriptControlCamara.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //---------------Salir puzzle---------------
    public void SalirModoPuzzle()
    {
        if (!enPuzzle) return;
        enPuzzle = false;

        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = true;
        if (scriptControlCamara != null) scriptControlCamara.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
