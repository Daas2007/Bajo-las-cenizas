using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialogo : MonoBehaviour, IInteractuable
{
    [SerializeField] private GameObject dialogoCanvas;
    [SerializeField] private TMP_Text dialogoTexto;

    [Header("Líneas de diálogo")]
    [TextArea(2, 5)]
    [SerializeField] private string[] lineas;

    [Header("Configuración de interacción")]
    [SerializeField] private float distanciaInteraccion = 2f;
    [SerializeField] private LayerMask layerInteractuable;

    private int indiceLinea = 0;
    private Transform jugador;

    private void Start()
    {
        if (dialogoCanvas != null)
            dialogoCanvas.SetActive(false);
        else
            Debug.LogError("⚠️ No se asignó el Canvas de diálogo en el Inspector.");

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            jugador = playerObj.transform;
        else
            Debug.LogError("⚠️ No se encontró un objeto con tag 'Player'.");
    }

    private void Update()
    {
        if (jugador == null || dialogoCanvas == null || dialogoTexto == null) return;

        float distancia = Vector3.Distance(jugador.position, transform.position);

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
        if (dialogoTexto == null) return;

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
        // Puedes usar esta función si quieres que el sistema de interacción general
        // también dispare el diálogo sin necesidad de presionar E.
        if (dialogoCanvas != null && !dialogoCanvas.activeSelf)
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
