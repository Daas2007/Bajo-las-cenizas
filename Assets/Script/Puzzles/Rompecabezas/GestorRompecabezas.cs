// GestorRompecabezas.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GestorRompecabezas : MonoBehaviour
{
    public static GestorRompecabezas Instancia;

    [Header("UI")]
    public GameObject panelRompecabezas; // PanelRompecabezas
    public RectTransform areaGrid; // AreaGrid
    public GameObject prefabPieza; // PrefabPieza
    public TMP_Text textoMensaje; // TextoMensaje

    [Header("Configuración del rompecabezas")]
    public Sprite[] spritesPiezas; // sprites por pieza (orden)
    public int cantidadPiezas = 9;
    public int cantidadRotables = 3; // cuántas piezas serán rotables
    public int indicePiezaFaltante = 4; // índice de la pieza que falta (0..n-1)

    List<PiezaRompecabezasUI> listaPiezas = new List<PiezaRompecabezasUI>();
    int contadorColocadas = 0;
    bool puzzleActivo = false;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    public void IniciarPuzzle()
    {
        ControladorPuzzle.Instancia.EntrarModoPuzzle();
        panelRompecabezas.SetActive(true);
        puzzleActivo = true;
        contadorColocadas = 0;
        LimpiarGrid();
        GenerarPiezas();
        SeleccionarRotablesAleatorias();
        MostrarMensaje("Resuelve el rompecabezas");
    }

    public void SalirPuzzle()
    {
        panelRompecabezas.SetActive(false);
        puzzleActivo = false;
        ControladorPuzzle.Instancia.SalirModoPuzzle();
    }

    void LimpiarGrid()
    {
        foreach (Transform t in areaGrid) Destroy(t.gameObject);
        listaPiezas.Clear();
    }

    void GenerarPiezas()
    {
        for (int i = 0; i < cantidadPiezas; i++)
        {
            // Crear casilla (slot)
            GameObject casilla = new GameObject("Casilla_" + i, typeof(RectTransform));
            casilla.transform.SetParent(areaGrid, false);
            RectTransform rt = casilla.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(100, 100); // ajustar según UI

            // Instanciar pieza
            GameObject p = Instantiate(prefabPieza, areaGrid);
            p.name = "Pieza_" + i;
            PiezaRompecabezasUI piezaUI = p.GetComponent<PiezaRompecabezasUI>();
            piezaUI.casillaObjetivo = rt;
            piezaUI.distanciaSnap = 40f;
            piezaUI.toleranciaRotacion = 12f;

            Image img = p.GetComponent<Image>();
            if (img != null && i < spritesPiezas.Length) img.sprite = spritesPiezas[i];

            // Si es la pieza faltante, mantenerla desactivada hasta recogerla
            if (i == indicePiezaFaltante)
            {
                p.SetActive(false);
            }

            listaPiezas.Add(piezaUI);
        }
    }

    void SeleccionarRotablesAleatorias()
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < listaPiezas.Count; i++)
            if (i != indicePiezaFaltante) indices.Add(i);

        for (int r = 0; r < cantidadRotables && indices.Count > 0; r++)
        {
            int idx = Random.Range(0, indices.Count);
            int indicePieza = indices[idx];
            indices.RemoveAt(idx);

            PiezaRompecabezasUI p = listaPiezas[indicePieza];
            p.puedeRotar = true;

            // Rotar aleatoriamente en múltiplos de 90 para desordenar
            int pasos = Random.Range(1, 4);
            p.GetComponent<RectTransform>().Rotate(0, 0, -90 * pasos);
        }
    }

    public void PiezaColocada()
    {
        contadorColocadas++;
        if (contadorColocadas >= cantidadPiezas - 1) // -1 porque falta una pieza
        {
            AlCompletarPuzzle();
        }
    }

    void AlCompletarPuzzle()
    {
        MostrarMensaje("Rompecabezas completado");
        // Aquí puedes desbloquear algo o dar un objeto
        Invoke(nameof(SalirPuzzle), 1.2f);
    }

    public void ActivarPiezaFaltante()
    {
        if (indicePiezaFaltante >= 0 && indicePiezaFaltante < listaPiezas.Count)
        {
            listaPiezas[indicePiezaFaltante].gameObject.SetActive(true);
            MostrarMensaje("Encontraste la pieza faltante");
        }
    }

    void MostrarMensaje(string msg)
    {
        if (textoMensaje != null)
        {
            textoMensaje.text = msg;
            CancelInvoke(nameof(LimpiarMensaje));
            Invoke(nameof(LimpiarMensaje), 3f);
        }
    }

    void LimpiarMensaje()
    {
        if (textoMensaje != null) textoMensaje.text = "";
    }

    void Update()
    {
        // Cerrar con ESC
        if (puzzleActivo && Input.GetKeyDown(KeyCode.Escape))
        {
            SalirPuzzle();
        }
    }
}

