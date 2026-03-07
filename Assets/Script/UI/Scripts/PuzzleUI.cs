using UnityEngine;

public class PuzzleUI : MonoBehaviour
{
    [Tooltip("Referencia al CandadoController que gestiona el puzzle")]
    public CandadoController controller;

    [Tooltip("Panel raíz del puzzle (este GameObject o el padre)")]
    public GameObject panelRoot;

    [Tooltip("Texto o método para mostrar feedback (opcional)")]
    public UnityEngine.UI.Text textoFeedback; // opcional, asigna un Text si quieres mostrar mensajes

    // Llamar desde el botón Confirmar (OnClick)
    public void Confirmar()
    {
        if (controller == null)
        {
            Debug.LogWarning("[PuzzleUI] controller no asignado. No se puede verificar el código.");
            return;
        }

        Debug.Log("[PuzzleUI] Confirmar pulsado -> VerificarCodigo()");
        controller.VerificarCodigo();
        // controller.VerificarCodigo() ya llama a DesactivarPuzzle() si es correcto
        // y a puzzle.Desbloquear() vía evento AlDesbloquear
    }

    // Llamar desde el botón Cancelar (OnClick) o Close
    public void Cancelar()
    {
        Debug.Log("[PuzzleUI] Cancelar pulsado -> DesactivarPuzzle()");
        if (controller != null)
        {
            controller.DesactivarPuzzle();
        }
        else
        {
            // fallback: ocultar panel y restaurar cursor
            if (panelRoot != null) panelRoot.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Método público para mostrar feedback desde CandadoController (por ejemplo en AlIntentoFallido)
    public void MostrarFeedback(string mensaje, float duracion = 2f)
    {
        if (textoFeedback != null)
        {
            StopAllCoroutines();
            StartCoroutine(MostrarCoroutine(mensaje, duracion));
        }
        else
        {
            Debug.Log("[PuzzleUI] Feedback: " + mensaje);
        }
    }

    private System.Collections.IEnumerator MostrarCoroutine(string mensaje, float duracion)
    {
        textoFeedback.text = mensaje;
        textoFeedback.gameObject.SetActive(true);
        yield return new WaitForSeconds(duracion);
        textoFeedback.gameObject.SetActive(false);
    }
}
