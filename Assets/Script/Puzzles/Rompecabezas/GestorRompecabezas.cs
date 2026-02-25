using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GestorRompecabezas : MonoBehaviour
{
    public static GestorRompecabezas Instancia;

    //---------------UI---------------
    [Header("UI")]
    [SerializeField] private GameObject panelRompecabezas; // Panel principal del puzzle
    [SerializeField] private RectTransform areaGrid;       // Contenedor de slots
    [SerializeField] private GameObject prefabPieza;       // Prefab de cada pieza
    [SerializeField] private TMP_Text textoMensaje;        // Texto de feedback

    //---------------Configuración---------------
    [Header("Configuración del rompecabezas")]
    [SerializeField] private Sprite[] spritesPiezas; // sprites por pieza (orden)
    [SerializeField] private int cantidadPiezas = 9;
    [SerializeField] private int cantidadRotables = 3; // cuántas piezas serán rotables
    [SerializeField] private int indicePiezaFaltante = 4; // índice de la pieza que falta (0..n-1)

    //---------------Estado interno---------------
    private List<PiezaRompecabezasUI> listaPiezas = new List<PiezaRompecabezasUI>();
    private bool puzzleActivo = false;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    //---------------Inicio puzzle---------------
    public void IniciarPuzzle()
    {
        ControladorPuzzle.Instancia.EntrarModoPuzzle();
        panelRompecabezas.SetActive(true);
        puzzleActivo = true;

        LimpiarGrid();
        GenerarPiezas();
        SeleccionarRotablesAleatorias();
        MostrarMensaje("Resuelve el rompecabezas");
    }

    //---------------Salir puzzle---------------
    public void SalirPuzzle()
    {
        panelRompecabezas.SetActive(false);
        puzzleActivo = false;
        ControladorPuzzle.Instancia.SalirModoPuzzle();
    }

    //---------------Generación piezas---------------
    private void LimpiarGrid()
    {
        foreach (Transform t in areaGrid) Destroy(t.gameObject);
        listaPiezas.Clear();
    }

    private void GenerarPiezas()
    {
        for (int i = 0; i < cantidadPiezas; i++)
        {
            // Crear casilla (slot)
            GameObject casilla = new GameObject("Casilla_" + i, typeof(RectTransform));
            casilla.transform.SetParent(areaGrid, false);
            RectTransform rt = casilla.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(100, 100);

            // Instanciar pieza
            GameObject p = Instantiate(prefabPieza, areaGrid);
            p.name = "Pieza_" + i;
            PiezaRompecabezasUI piezaUI = p.GetComponent<PiezaRompecabezasUI>();
            piezaUI.casillaObjetivo = rt;
            piezaUI.distanciaSnap = 40f;
            piezaUI.toleranciaRotacion = 12f;

            Image img = p.GetComponent<Image>();
            if (img != null && i < spritesPiezas.Length) img.sprite = spritesPiezas[i];

            if (i == indicePiezaFaltante)
                p.SetActive(false);

            listaPiezas.Add(piezaUI);
        }
    }

    private void SeleccionarRotablesAleatorias()
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

            int pasos = Random.Range(1, 4);
            p.GetComponent<RectTransform>().Rotate(0, 0, -90 * pasos);
        }
    }

    //---------------Validación puzzle---------------
    public void PiezaColocada()
    {
        bool todasCorrectas = true;
        foreach (var pieza in listaPiezas)
        {
            if (!pieza.EstaColocada())
            {
                todasCorrectas = false;
                break;
            }
        }

        if (todasCorrectas)
            AlCompletarPuzzle();
    }

    private void AlCompletarPuzzle()
    {
        MostrarMensaje("Rompecabezas completado");
        // Aquí puedes desbloquear algo o dar un objeto
        Invoke(nameof(SalirPuzzle), 1.2f);
    }

    //---------------Activar pieza faltante---------------
    public void ActivarPiezaFaltante()
    {
        if (indicePiezaFaltante >= 0 && indicePiezaFaltante < listaPiezas.Count)
        {
            listaPiezas[indicePiezaFaltante].gameObject.SetActive(true);
            MostrarMensaje("Encontraste la pieza faltante");
        }
    }

    //---------------Mensajes---------------
    private void MostrarMensaje(string msg)
    {
        if (textoMensaje != null)
        {
            textoMensaje.text = msg;
            CancelInvoke(nameof(LimpiarMensaje));
            Invoke(nameof(LimpiarMensaje), 3f);
        }
    }

    private void LimpiarMensaje()
    {
        if (textoMensaje != null) textoMensaje.text = "";
    }

    //---------------Update---------------
    void Update()
    {
        if (puzzleActivo && Input.GetKeyDown(KeyCode.Escape))
            SalirPuzzle();
    }
}
