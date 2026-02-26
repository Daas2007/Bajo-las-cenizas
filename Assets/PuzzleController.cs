using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("HUD principal del juego")]
    [SerializeField] private GameObject panelHUD;

    [Tooltip("Panel del puzzle rompecabezas")]
    [SerializeField] private GameObject panelPuzzleRompecabezas;

    public void MostrarPanelPuzzle()
    {
        if (panelHUD != null) panelHUD.SetActive(false);
        if (panelPuzzleRompecabezas != null) panelPuzzleRompecabezas.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("🧩 Panel de puzzle activado, HUD oculto.");
    }
    public void CerrarPanelPuzzle()
    {
        if (panelPuzzleRompecabezas != null)
            panelPuzzleRompecabezas.SetActive(false);

        if (panelHUD != null)
            panelHUD.SetActive(true);

        // ✅ Restaurar tiempo y cursor
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ✅ Reactivar movimiento y cámara del jugador
        MovimientoPersonaje jugador = FindObjectOfType<MovimientoPersonaje>();
        if (jugador != null) jugador.enabled = true;

        // Aquí reactivamos el script de cámara
        Camara camController = FindObjectOfType<Camara>();
        if (camController != null) camController.enabled = true;

        Debug.Log("🧩 Panel de puzzle cerrado, HUD reactivado, movimiento y cámara restaurados.");
    }
}
