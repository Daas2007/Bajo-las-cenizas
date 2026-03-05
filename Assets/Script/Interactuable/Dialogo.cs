using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

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

    // Nuevo: evento y flag público para saber si el jugador ya habló con este NPC
    [Header("Eventos")]
    public UnityEvent OnDialogoCompleto;
    public bool HaHablado { get; private set; } = false;

    // Guardar líneas originales para poder restaurarlas si usamos líneas temporales
    private string[] lineasOriginales;

    void Start()
    {
        if (dialogoCanvas != null)
            dialogoCanvas.SetActive(false);

        // clonar las líneas originales
        if (lineas != null)
            lineasOriginales = (string[])lineas.Clone();
    }

    void Update()
    {
        if (!mostrandoDialogo) return;

        // Avanzar/saltar con Space (mantengo Space como control de avance)
        if (Input.GetKeyDown(KeyCode.Space))
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
        if (dialogoCanvas == null || dialogoTexto == null)
        {
            Debug.LogWarning("[Dialogo] UI no asignada.");
            return;
        }

        dialogoCanvas.SetActive(true);
        indiceLinea = 0;
        mostrandoDialogo = true;
        MostrarLinea();
        BloquearJugador(true);
    }

    // Nuevo: iniciar diálogo con líneas temporales (no sobrescribe permanentemente las originales)
    // marcarComoHablado: si true, al terminar se marcará HaHablado = true y disparará OnDialogoCompleto
    public void IniciarDialogoConLineas(string[] nuevasLineas, bool marcarComoHablado = false)
    {
        if (nuevasLineas == null || nuevasLineas.Length == 0)
            return;

        // Guardar las originales si no están guardadas
        if (lineasOriginales == null && lineas != null)
            lineasOriginales = (string[])lineas.Clone();

        // Reemplazar temporalmente
        lineas = (string[])nuevasLineas.Clone();
        indiceLinea = 0;
        mostrandoDialogo = true;

        // Si queremos que al terminar esto marque como hablado, lo haremos en TerminarDialogo usando una bandera temporal
        StartCoroutine(IniciarDialogoTemporalCoroutine(marcarComoHablado));
    }

    private IEnumerator IniciarDialogoTemporalCoroutine(bool marcarComoHablado)
    {
        // Mostrar la primera línea
        if (dialogoCanvas != null) dialogoCanvas.SetActive(true);
        MostrarLinea();

        // Esperar hasta que el diálogo termine (TerminarDialogo restaurará las líneas originales)
        while (mostrandoDialogo)
            yield return null;

        // Si se pidió marcar como hablado, hacerlo
        if (marcarComoHablado)
        {
            HaHablado = true;
            OnDialogoCompleto?.Invoke();
        }
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

        // Marcar que ya habló (solo si no estaba ya marcado)
        if (!HaHablado)
        {
            HaHablado = true;
            OnDialogoCompleto?.Invoke();
        }

        // Si usamos líneas temporales, restaurar las originales
        if (lineasOriginales != null)
        {
            lineas = (string[])lineasOriginales.Clone();
        }
    }

    public void BloquearJugador(bool bloquear)
    {
        if (scriptMovimiento != null)
            scriptMovimiento.enabled = !bloquear;
    }

    // Método público para resetear el estado de "ha hablado" (útil para testing o reinicios)
    public void ResetHaHablado()
    {
        HaHablado = false;
    }
}

