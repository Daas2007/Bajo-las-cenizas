using UnityEngine;

public class LinternaPickup : MonoBehaviour, IInteractuable
{
    public void Interactuar()
    {
        JugadorLinterna jugadorLinterna = FindObjectOfType<JugadorLinterna>();
        if (jugadorLinterna != null)
        {
            jugadorLinterna.DarLinterna(); // entrega la linterna correctamente
        }

        GameManager.Instancia.tieneLinterna = true;

        // Notificar al tutorial
        TutorialInteractivo tutorial = FindObjectOfType<TutorialInteractivo>();
        if (tutorial != null) tutorial.NotificarLinternaRecogida();

        gameObject.SetActive(false);
        Debug.Log("🔦 Linterna recogida.");
    }
}
