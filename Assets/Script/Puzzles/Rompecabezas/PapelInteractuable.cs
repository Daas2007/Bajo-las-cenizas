using UnityEngine;

public class PapelInteractuable : MonoBehaviour, IInteractuable
{
    //---------------Opcional---------------
    [Header("Opcional")]
    [Tooltip("Sonido que se reproducirá al abrir el puzzle")]
    public AudioClip sonidoAbrir;

    [Tooltip("Si está en true, el objeto se destruirá al interactuar; si está en false, solo se ocultará")]
    public bool desaparecerAlInteractuar = false;

    //---------------Interacción---------------
    public void Interactuar()
    {
        if (sonidoAbrir != null)
            AudioSource.PlayClipAtPoint(sonidoAbrir, transform.position);

        if (GestorRompecabezas.Instancia != null)
            GestorRompecabezas.Instancia.IniciarPuzzle();

        if (desaparecerAlInteractuar)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}