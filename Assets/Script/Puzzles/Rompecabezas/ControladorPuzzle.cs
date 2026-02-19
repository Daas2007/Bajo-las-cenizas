// ControladorPuzzle.cs
using UnityEngine;

public class ControladorPuzzle : MonoBehaviour
{
    public static ControladorPuzzle Instancia;

    [Header("Referencias")]
    public MonoBehaviour scriptMovimientoJugador; // arrastra aquí tu script de movimiento
    public MonoBehaviour scriptControlCamara; // arrastra aquí tu script de control de cámara

    bool enPuzzle = false;


    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        if (ControladorPuzzle.Instancia != null)
            ControladorPuzzle.Instancia.SalirModoPuzzle();
    }

    public void EntrarModoPuzzle()
    {
        if (enPuzzle) return;
        enPuzzle = true;
        Debug.Log("Se activo puzzle");
        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = false;
        if (scriptControlCamara != null) scriptControlCamara.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

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
