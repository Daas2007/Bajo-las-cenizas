using UnityEngine;
using System.Collections.Generic;

public class TutorialCuaderno : MonoBehaviour, IInteractuable
{
    [SerializeField] GameObject canvasCuaderno;
    [SerializeField] List<GameObject> paginas; // lista de páginas (imágenes)

    private bool yaInteractuado = false;
    private bool canvasAbierto = false;
    private int paginaActual = 0; // índice de la primera página visible

    // referencias runtime
    MovimientoPersonaje movimientoJugador;
    MonoBehaviour scriptCamara;

    private void Awake()
    {
        if (canvasCuaderno != null) canvasCuaderno.SetActive(false);

        movimientoJugador = FindObjectOfType<MovimientoPersonaje>();
        if (movimientoJugador != null)
            scriptCamara = movimientoJugador.GetComponentInChildren<Camara>() as MonoBehaviour;

        if (scriptCamara == null)
            scriptCamara = FindObjectOfType<Camara>() as MonoBehaviour;

        // Apagar todas las páginas al inicio
        foreach (var p in paginas)
            if (p != null) p.SetActive(false);
    }
    void Update()
    {
        if (canvasAbierto && Input.GetKeyDown(KeyCode.Escape))
        {
            CerrarCanvas();
            return;
        }
    }
    public void Interactuar()
    {
        if (!yaInteractuado)
        {
            yaInteractuado = true;
        }

        AbrirCanvas();
    }
    void AbrirCanvas()
    {
        if (canvasCuaderno == null) return;

        canvasCuaderno.SetActive(true);
        canvasAbierto = true;

        if (movimientoJugador != null) movimientoJugador.enabled = false;
        if (scriptCamara != null) scriptCamara.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        MostrarPaginas(0); // mostrar las dos primeras páginas
    }
    public void CerrarCanvas()
    {
        if (canvasCuaderno == null) return;

        canvasCuaderno.SetActive(false);
        canvasAbierto = false;

        if (movimientoJugador != null) movimientoJugador.enabled = true;
        if (scriptCamara != null) scriptCamara.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    // ✅ Mostrar dos páginas a la vez
    private void MostrarPaginas(int indice)
    {
        // Apagar todas
        foreach (var p in paginas)
            if (p != null) p.SetActive(false);

        paginaActual = indice;

        // Encender las dos siguientes si existen
        if (indice < paginas.Count) paginas[indice].SetActive(true);
        if (indice + 1 < paginas.Count) paginas[indice + 1].SetActive(true);
    }
    // ✅ Botón siguiente
    public void PaginaSiguiente()
    {
        if (paginaActual + 2 < paginas.Count)
        {
            MostrarPaginas(paginaActual + 2);
        }
    }
    // ✅ Botón anterior
    public void PaginaAnterior()
    {
        if (paginaActual - 2 >= 0)
        {
            MostrarPaginas(paginaActual - 2);
        }
    }
}