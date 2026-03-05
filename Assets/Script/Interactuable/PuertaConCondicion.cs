using UnityEngine;

public class PuertaConCondicion : MonoBehaviour, IInteractuable
{
    [Header("Puerta base (reutiliza PuertaInteractuable)")]
    [SerializeField] private PuertaInteractuable puertaBase; // puerta que realiza la rotación real

    [Header("Condición: NPC con el que hay que hablar primero")]
    [Tooltip("Arrastra aquí el componente Dialogo del NPC con el que el jugador debe hablar antes de abrir.")]
    [SerializeField] private Dialogo npcDialogo;

    [Header("Mensaje que aparece si no ha hablado")]
    [Tooltip("Si no asignas un Dialogo para el mensaje, se usará el Dialogo del NPC para mostrar la línea temporal.")]
    [SerializeField] private string mensajeSiNoHaHablado = "Hey tú, ven al fondo";

    [Header("Opciones")]
    [SerializeField] private float duracionMensajeTemporal = 2.5f;

    public void Interactuar()
    {
        if (puertaBase == null)
        {
            Debug.LogWarning("[PuertaConCondicion] puertaBase no asignada.");
            return;
        }

        // Si no hay NPC asignado, por seguridad no permitir abrir
        if (npcDialogo == null)
        {
            Debug.LogWarning("[PuertaConCondicion] npcDialogo no asignado. No se puede abrir la puerta.");
            MostrarMensajeRapido(mensajeSiNoHaHablado);
            return;
        }

        // Si el jugador ya habló con el NPC, delegar la interacción a la puerta normal
        if (npcDialogo.HaHablado)
        {
            puertaBase.Interactuar();
            return;
        }

        // Si no ha hablado: mostrar mensaje corto y no abrir
        // Usamos el propio sistema de Dialogo para mostrar la línea temporal (no marca como "ha hablado")
        string[] temp = new string[] { mensajeSiNoHaHablado };
        npcDialogo.IniciarDialogoConLineas(temp, marcarComoHablado: false);
    }

    // Si quieres mostrar un mensaje rápido sin usar Dialogo, puedes implementar aquí una alternativa (UI temporal).
    private void MostrarMensajeRapido(string texto)
    {
        // Intentamos usar npcDialogo si existe
        if (npcDialogo != null)
        {
            npcDialogo.IniciarDialogoConLineas(new string[] { texto }, false);
            return;
        }

        // Si no hay Dialogo disponible, solo loguear (puedes reemplazar esto con tu propio UI)
        Debug.Log("[PuertaConCondicion] " + texto);
    }
}

