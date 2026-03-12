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

    [Header("Movimiento")]
    [Tooltip("Arrastra aquí el componente que controla el movimiento del jugador (por ejemplo MovimientoPersonaje)")]
    [SerializeField] private MonoBehaviour Personaje;

    [Header("Texto si no ha hablado")]
    [SerializeField] private string mensajeSiNoHaHablado = "Hey tú, ven al fondo";

    // Coroutine para controlar el tiempo del mensaje y reactivar movimiento
    private Coroutine mensajeCoroutine;

    void Update()
    {
        // Si el panel está activo y el jugador presiona Space, cerrar inmediatamente y reactivar movimiento
        if (panelMensaje != null && panelMensaje.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            CancelarMensajeTemporal();
        }
    }

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

        // Si ya habló con el NPC, abrir la puerta
        if (npcDialogo.HaHablado)
        {
            puertaBase.Interactuar();
        }
        else
        {
            MostrarMensajeTemporal(mensajeSiNoHaHablado);
        }
    }

    private void MostrarMensajeTemporal(string texto)
    {
        if (panelMensaje == null || textoMensaje == null) return;

        // Si ya hay un mensaje en curso, reiniciamos el temporizador y el texto
        if (mensajeCoroutine != null)
        {
            StopCoroutine(mensajeCoroutine);
            mensajeCoroutine = null;
        }

        panelMensaje.SetActive(true);
        textoMensaje.text = texto;

        // Bloquear movimiento del jugador si se asignó el componente
        if (Personaje != null)
            Personaje.enabled = false;

        mensajeCoroutine = StartCoroutine(DesactivarPanelCoroutine(duracionMensaje));
    }

    private IEnumerator DesactivarPanelCoroutine(float duracion)
    {
        // Usar tiempo real para que no dependa de Time.timeScale
        yield return new WaitForSecondsRealtime(duracion);

        if (panelMensaje != null)
            panelMensaje.SetActive(false);

        // Reactivar movimiento del jugador si corresponde
        if (Personaje != null)
            Personaje.enabled = true;

        mensajeCoroutine = null;
    }

    // Por si necesitas cancelar manualmente el mensaje y reactivar movimiento
    public void CancelarMensajeTemporal()
    {
        if (mensajeCoroutine != null)
        {
            StopCoroutine(mensajeCoroutine);
            mensajeCoroutine = null;
        }

        if (panelMensaje != null)
            panelMensaje.SetActive(false);

        if (Personaje != null)
            Personaje.enabled = true;
    }
}
