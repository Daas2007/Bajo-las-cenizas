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
    public bool mostrandoDialogo = false;
    public Coroutine rutinaTexto;
    private bool escribiendoLinea = false;

    [SerializeField] public MonoBehaviour scriptMovimiento;

    void Start()
    {
        if (dialogoCanvas != null)
            dialogoCanvas.SetActive(false);
    }

    void Update()
    {
        if (!mostrandoDialogo) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (escribiendoLinea)
            {
                TerminarLineaInstantanea();
            }
            else
            {
                CancelInvoke(nameof(AvanzarDialogo));

                // 🔑 Si estamos en la última línea, cerrar diálogo
                if (indiceLinea >= lineas.Length - 1)
                {
                    TerminarDialogo();
                }
                else
                {
                    AvanzarDialogo();
                }
            }
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
            if (escribiendoLinea)
            {
                TerminarLineaInstantanea();
            }
            else
            {
                CancelInvoke(nameof(AvanzarDialogo));

                if (indiceLinea >= lineas.Length - 1)
                {
                    TerminarDialogo();
                }
                else
                {
                    AvanzarDialogo();
                }
            }
        }
    }

    public void IniciarDialogo()
    {
        dialogoCanvas.SetActive(true);
        indiceLinea = 0; // 🔑 reinicia siempre al empezar
        mostrandoDialogo = true;
        MostrarLinea();
        BloquearJugador(true);
    }

    public void MostrarLinea()
    {
        if (rutinaTexto != null) StopCoroutine(rutinaTexto);
        rutinaTexto = StartCoroutine(EscribirLinea(lineas[indiceLinea]));
        Invoke(nameof(AvanzarDialogo), tiempoAutoAvance);
    }

    public IEnumerator EscribirLinea(string linea)
    {
        escribiendoLinea = true;
        dialogoTexto.text = "";
        foreach (char letra in linea)
        {
            dialogoTexto.text += letra;
            yield return new WaitForSeconds(tiempoEntreLetras);
        }
        escribiendoLinea = false;
    }

    private void TerminarLineaInstantanea()
    {
        if (rutinaTexto != null) StopCoroutine(rutinaTexto);
        dialogoTexto.text = lineas[indiceLinea];
        escribiendoLinea = false;
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
        dialogoCanvas.SetActive(false);   
        mostrandoDialogo = false;
        BloquearJugador(false);
        // 🔑 No reiniciamos aquí, solo al iniciar un nuevo diálogo
    }

    public void BloquearJugador(bool bloquear)
    {
        if (scriptMovimiento != null)
            scriptMovimiento.enabled = !bloquear;
    }
}
