using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [System.Serializable]
    public struct Dialogo
    {
        public string nombreHablante;
        [TextArea(3, 5)] public string[] lineas;
    }

    [Header("Configuración del Diálogo")]
    public Dialogo dialogo;

    [Header("Modo de Activación")]
    [SerializeField] private bool activarPorTrigger = false;
    [SerializeField] private bool desactivarTriggerAlUsar = true;

    private bool jugadorCerca = false;

    void Update()
    {
        // Para interactuar con Tecla 'E' (NPC u Objetos)
        if (!activarPorTrigger && jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            DispararDialogo();
        }
    }

    public void DispararDialogo()
    {
        if (DialogueManager.Instancia != null)
        {
            DialogueManager.Instancia.IniciarDialogo(dialogo.nombreHablante, dialogo.lineas);
        }
    }

    // --------------- Detección para Triggers y Proximidad ---------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;

            // Caso: Zona invisible que activa el diálogo automáticamente al pisar
            if (activarPorTrigger)
            {
                DispararDialogo();

                if (desactivarTriggerAlUsar)
                {
                    GetComponent<Collider>().enabled = false;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }


}