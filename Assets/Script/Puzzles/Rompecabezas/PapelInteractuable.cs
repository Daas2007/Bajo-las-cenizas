// PapelInteractuable.cs
using UnityEngine;

public class PapelInteractuable : MonoBehaviour, IInteractuable
{
    [Header("Opcional")]
    public AudioClip sonidoAbrir; // sonido al abrir el papel
    public bool destruirAlInteractuar = true;

    public void Interactuar()
    {
        // reproducir sonido si hay
        if (sonidoAbrir != null)
            AudioSource.PlayClipAtPoint(sonidoAbrir, transform.position);

        // Abrir el rompecabezas
        if (GestorRompecabezas.Instancia != null)
            GestorRompecabezas.Instancia.IniciarPuzzle();

        // desactivar o destruir el papel en el mundo
        if (destruirAlInteractuar) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}
