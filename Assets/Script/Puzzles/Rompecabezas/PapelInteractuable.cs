// PapelInteractuable.cs
using UnityEngine;

public class PapelInteractuable : MonoBehaviour, IInteractuable
{
    [Header("Opcional")]
    [Tooltip("Sonido que se reproducirá al abrir el puzzle")]
    public AudioClip sonidoAbrir;

    [Tooltip("Si está en true, el objeto se destruirá al interactuar; si está en false, solo se ocultará")]
    public bool desaparecerAlInteractuar = false;

    public void Interactuar()
    {
        // reproducir sonido si existe
        if (sonidoAbrir != null)
            AudioSource.PlayClipAtPoint(sonidoAbrir, transform.position);

        // abrir el panel del rompecabezas
        if (GestorRompecabezas.Instancia != null)
            GestorRompecabezas.Instancia.IniciarPuzzle();

        // comportamiento del objeto en el mundo
        if (desaparecerAlInteractuar)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}

