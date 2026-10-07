using UnityEngine;

public class EsconderseMiniJuego2 : MonoBehaviour
{
    [SerializeField] MovimientojugadorMiniJuego2 movimiento;
    [SerializeField] private GameObject ModeloJugador;
    [SerializeField] private BoxCollider ColliderJugador;
    [SerializeField] private bool Escondido = false;
    [SerializeField] GameObject Canvas_Texto_Esconderse;

    private bool enZonaEscondite = false;

    private void Awake()
    {
        movimiento = GetComponent<MovimientojugadorMiniJuego2>();
        if (Canvas_Texto_Esconderse != null)
        {
            Canvas_Texto_Esconderse.SetActive(false);
        }
    }

    private void Update()
    {
        if (enZonaEscondite && Input.GetKeyDown(KeyCode.E))
        {
            ToggleEsconder();
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Escondite"))
        {
            if (Canvas_Texto_Esconderse != null && !Escondido)
            {
                Canvas_Texto_Esconderse.SetActive(true);
            }

            enZonaEscondite = true;
            Debug.Log("Entraste a un area de escondite");
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Escondite"))
        {
            if (Canvas_Texto_Esconderse != null)
            {
                Canvas_Texto_Esconderse.SetActive(false);
            }
            enZonaEscondite = false;
            Debug.Log("Saliste del area de escondite");
        }
    }

    private void ToggleEsconder()
    {
        Escondido = !Escondido;

        // Ocultamos/activamos solo el modelo del jugador
        if (ModeloJugador != null)
        {
            ModeloJugador.SetActive(!Escondido);
        }

        // Activamos o desactivamos el movimiento del personaje y la cámara
        if (movimiento != null)
        {
            // Si está escondido (true), el movimiento se deshabilita (false)
            movimiento.SetMovimientoHabilitado(!Escondido);
        }

        // Actualizamos el texto del canvas según el estado
        if (Canvas_Texto_Esconderse != null)
        {
            Canvas_Texto_Esconderse.SetActive(!Escondido && enZonaEscondite);
        }

        Debug.Log(Escondido ? "El jugador se escondio" : "El jugador ha salido del escondite");
    }
}