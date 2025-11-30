using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialogo : MonoBehaviour, IInteractuable
{
    [SerializeField] GameObject dialogoCanvas;
    [SerializeField] TMP_Text dialogoTexto;

    [Header("Líneas de diálogo")]
    [TextArea(2, 5)]
    [SerializeField] string[] lineas;

    [Header("Configuración de interacción")]
    [SerializeField] float distanciaInteraccion = 2f; // 🔑 distancia máxima
    [SerializeField] LayerMask layerInteractuable;      // capa de objetos interactuables

    private int indiceLinea = 0;
    private Transform jugador;

    private void Start()
    {
        dialogoCanvas.SetActive(false);
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Calcular distancia entre jugador y este objeto
        float distancia = Vector3.Distance(jugador.position, transform.position);

        // Si está dentro de la distancia y presiona E
        if (distancia <= distanciaInteraccion && Input.GetKeyDown(KeyCode.E))
        {
            if (!dialogoCanvas.activeSelf)
            {
                dialogoCanvas.SetActive(true);
                MostrarLinea();
            }
            else
            {
                MostrarLinea();
            }
        }
    }

    public void MostrarLinea()
    {
        if (indiceLinea < lineas.Length)
        {
            dialogoTexto.text = lineas[indiceLinea];
            indiceLinea++;
        }
        else
        {
            dialogoCanvas.SetActive(false);
            indiceLinea = 0;
        }
    }

    public void Interactuar()
    {
        throw new System.NotImplementedException();
    }
}
