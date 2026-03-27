using UnityEngine;
using System.Collections.Generic;

public class TutorialCuaderno : MonoBehaviour, IInteractuable
{
    [SerializeField] GameObject canvasCuaderno;
    [SerializeField] List<GameObject> paginas; // lista de páginas (imágenes)
    [SerializeField] private Dialogo dialogo;   // 🔹 referencia al sistema de diálogo

    private bool yaInteractuado = false;
    private bool canvasAbierto = false;
    private int paginaActual = 0;

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

            // 🔹 Mostrar diálogo la primera vez
            if (dialogo != null && dialogo.dialogoCanvas != null && dialogo.dialogoTexto != null)
            {
                dialogo.ResetHaHablado();
                dialogo.IniciarDialogo();

                // Cuando termine el diálogo, abrir el cuaderno
                dialogo.OnDialogoCompleto.AddListener(() =>
                {
                    AbrirCanvas();
                });
                return;
            }
            else
            {
                Debug.LogWarning("⚠️ No hay diálogo asignado o referencias faltantes, abriendo cuaderno directamente.");
            }
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

    private void MostrarPaginas(int indice)
    {
        foreach (var p in paginas)
            if (p != null) p.SetActive(false);

        paginaActual = indice;

        if (indice < paginas.Count) paginas[indice].SetActive(true);
        if (indice + 1 < paginas.Count) paginas[indice + 1].SetActive(true);
    }

    public void PaginaSiguiente()
    {
        if (paginaActual + 2 < paginas.Count)
        {
            MostrarPaginas(paginaActual + 2);
        }
    }

    public void PaginaAnterior()
    {
        if (paginaActual - 2 >= 0)
        {
            MostrarPaginas(paginaActual - 2);
        }
    }
}
