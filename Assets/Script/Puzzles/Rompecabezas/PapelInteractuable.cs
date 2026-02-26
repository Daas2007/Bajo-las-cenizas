using UnityEngine;

public class PapelInteractuable : MonoBehaviour, IInteractuable
{
    [Header("Opcional")]
    [Tooltip("Sonido que se reproducirá al abrir el puzzle")]
    public AudioClip sonidoAbrir;

    [Header("Referencias")]
    [Tooltip("Panel del puzzle que se abrirá")]
    public GameObject panelPuzzle;

    [Tooltip("Puertas del armario que se activarán al completar el puzzle")]
    public GameObject puertaArmarioIzquierda;
    public GameObject puertaArmarioDerecha;

    [Tooltip("Tag que se asignará a las puertas para poder interactuar")]
    public string tagInteraccionPuerta = "Interaccion";

    private bool puzzleCompletado = false;

    public void Interactuar()
    {
        if (puzzleCompletado) return;

        if (sonidoAbrir != null)
            AudioSource.PlayClipAtPoint(sonidoAbrir, transform.position);

        if (GestorRompecabezas.Instancia != null)
            GestorRompecabezas.Instancia.IniciarPuzzle();

        // ✅ Mostrar puzzle con PuzzleController
        PuzzleController pc = FindObjectOfType<PuzzleController>();
        if (pc != null)
            pc.MostrarPanelPuzzle();
    }

    public void PuzzleCompletado()
    {
        puzzleCompletado = true;

        // ✅ Cerrar puzzle con PuzzleController
        PuzzleController pc = FindObjectOfType<PuzzleController>();
        if (pc != null)
            pc.CerrarPanelPuzzle();

        // Ocultar el papel permanentemente
        gameObject.SetActive(false);

        // Activar las dos puertas del armario
        if (puertaArmarioIzquierda != null)
        {
            puertaArmarioIzquierda.SetActive(true);
            puertaArmarioIzquierda.tag = tagInteraccionPuerta;
        }

        if (puertaArmarioDerecha != null)
        {
            puertaArmarioDerecha.SetActive(true);
            puertaArmarioDerecha.tag = tagInteraccionPuerta;
        }

        Debug.Log("✅ Puzzle completado, puertas del armario activadas.");
    }
}
