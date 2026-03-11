using UnityEngine;
using TMPro;
using System.Collections;

public class PuertaConCondicion : MonoBehaviour, IInteractuable
{
    [Header("Puerta base")]
    [SerializeField] private PuertaInteractuable puertaBase;

    [Header("Condición: NPC con el que hay que hablar primero")]
    [SerializeField] private Dialogo npcDialogo;

    [Header("UI Mensaje temporal")]
    [SerializeField] private GameObject panelMensaje;
    [SerializeField] private TMP_Text textoMensaje;
    [SerializeField] private float duracionMensaje = 2.5f;

    [Header("Texto si no ha hablado")]
    [SerializeField] private string mensajeSiNoHaHablado = "Hey tú, ven al fondo";

    public void Interactuar()
    {
        if (puertaBase == null)
        {
            Debug.LogWarning("[PuertaConCondicion] puertaBase no asignada.");
            return;
        }

        if (npcDialogo == null)
        {
            Debug.LogWarning("[PuertaConCondicion] npcDialogo no asignado.");
            MostrarMensajeTemporal(mensajeSiNoHaHablado);
            return;
        }

        // ✅ Si ya habló con el NPC, abrir la puerta
        if (npcDialogo.HaHablado)
        {
            puertaBase.Interactuar();
        }
        else
        {
            // ✅ Mostrar mensaje temporal
            MostrarMensajeTemporal(mensajeSiNoHaHablado);
        }
    }

    private void MostrarMensajeTemporal(string texto)
    {
        if (panelMensaje == null || textoMensaje == null) return;

        panelMensaje.SetActive(true);
        textoMensaje.text = texto;
        StartCoroutine(DesactivarPanel());
    }

    private IEnumerator DesactivarPanel()
    {
        yield return new WaitForSeconds(duracionMensaje);
        panelMensaje.SetActive(false);
    }
}
