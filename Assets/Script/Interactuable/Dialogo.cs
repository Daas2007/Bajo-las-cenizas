using UnityEngine;
using TMPro;
using System.Collections;

public class Dialogo : MonoBehaviour, IInteractuable
{
    [SerializeField] public GameObject dialogoCanvas;
    [SerializeField] public TMP_Text dialogoTexto;

    [Header("Líneas de diálogo")]
    [TextArea(2, 5)]
    [SerializeField] public string[] lineas;

    [Header("Configuración")]
    [SerializeField] public float tiempoEntreLetras = 0.05f;
    [SerializeField] public float tiempoAutoAvance = 5f;

    public int indiceLinea = 0;
    public Transform jugador;
    public bool mostrandoDialogo = false;
    public Coroutine rutinaTexto;

    // Referencia al script de movimiento del jugador
    [SerializeField] public MonoBehaviour scriptMovimiento;

    void Start()
    {
        if (dialogoCanvas != null)
            dialogoCanvas.SetActive(false);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            jugador = playerObj.transform;
    }

    void Update()
    {
        if (!mostrandoDialogo) return;

        // Permitir saltar con E
        if (Input.GetKeyDown(KeyCode.E))
        {
            CancelInvoke(nameof(AvanzarDialogo)); // evita doble avance
            AvanzarDialogo();
        }
    }

    public void Interactuar()
    {
        if (!mostrandoDialogo)
        {
            IniciarDialogo();
        }
        else
        {
            CancelInvoke(nameof(AvanzarDialogo));
            AvanzarDialogo();
        }
    }

    public void IniciarDialogo()
    {
        dialogoCanvas.SetActive(true);
        indiceLinea = 0;
        mostrandoDialogo = true;
        MostrarLinea();
        BloquearJugador(true);

        // Ocultar texto de interacción si existe
        GameObject panelInteraccion = GameObject.Find("PanelInteraccion");
        if (panelInteraccion != null) panelInteraccion.SetActive(false);
    }

    public void MostrarLinea()
    {
        if (rutinaTexto != null) StopCoroutine(rutinaTexto);
        rutinaTexto = StartCoroutine(EscribirLinea(lineas[indiceLinea]));
        Invoke(nameof(AvanzarDialogo), tiempoAutoAvance);
    }

    public IEnumerator EscribirLinea(string linea)
    {
        dialogoTexto.text = "";
        foreach (char letra in linea)
        {
            dialogoTexto.text += letra;
            yield return new WaitForSeconds(tiempoEntreLetras);
        }
    }

    public void AvanzarDialogo()
    {
        CancelInvoke(nameof(AvanzarDialogo));
        indiceLinea++;

        if (indiceLinea < lineas.Length)
        {
            MostrarLinea();
        }
        else
        {
            TerminarDialogo();
        }
    }

    public void TerminarDialogo()
    {
        mostrandoDialogo = false;
        dialogoCanvas.SetActive(false);
        indiceLinea = 0; // reset para próxima vez
        BloquearJugador(false);
    }

    public void BloquearJugador(bool bloquear)
    {
        if (scriptMovimiento != null)
            scriptMovimiento.enabled = !bloquear;
    }
}
